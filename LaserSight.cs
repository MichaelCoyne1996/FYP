//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using Tobii.Gaming;
using System.Collections;
using System.Collections.Generic;

public class LaserSight : MonoBehaviour
{
	public GameObject LaserSightImpactPointPrefab;
	public Material LaserSightMaterial;
	public float Width = 0.0005f;
	public bool IsEnabled = true;

	private GameObject _laserSightImpactPoint;
	private WeaponController _weaponController;
	private LineRenderer _laserSight;

    public int countHit = 0;
    public int countMiss = 0;
    public float countdown = 20.0f;

    public Text countHitText;
    public Text countMissText;
    public Text timer;
    public Text GameOver;
    

    private float speed = 1000f;

    public bool gameOver = false;

    public string lastTarget = "";

    protected void Start()
	{
        countdown = 20.0f;
        countHitText.text = getScoreHit();
        countMissText.text = getScoreMiss();
        _weaponController = transform.root.gameObject.GetComponentInChildren<WeaponController>();

		_laserSight = gameObject.AddComponent<LineRenderer>();
		_laserSight.materials = new[] { LaserSightMaterial };
		_laserSight.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		_laserSight.receiveShadows = false;

		_laserSightImpactPoint = Instantiate(LaserSightImpactPointPrefab);
	}
	protected void LateUpdate()
	{
        
		var laserSightPositions = new[] { transform.position, transform.position + transform.forward * 0.5f };
		_laserSight.SetPosition(0, laserSightPositions[0]);
		_laserSight.SetPosition(1, laserSightPositions[1]);
	    
        SetLaserSightStartAndEndWidth(startWidth: 0.002f, endWidth: 0.0f);

        _laserSight.material.mainTextureOffset = new Vector2((-Time.time * 2.0f) % 1.0f, 0.0f);

		if (IsEnabled && _weaponController.IsWeaponHitObject)
		{
			var hitInfo = _weaponController.WeaponHitData;
			_laserSightImpactPoint.SetActive(true);
			_laserSightImpactPoint.transform.position = hitInfo.point + hitInfo.normal * 0.0005f;
			_laserSightImpactPoint.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
            if (Input.GetKeyDown(KeyCode.Mouse0) == true && gameOver == false)
            {
                if (hitInfo.collider.tag == "CloseTarget" && lastTarget != "CloseTarget")
                {
                    countHit++;
                    Debug.Log("Hit a close Target, Score + 1");
                    countHitText.text = getScoreHit();
                    lastTarget = "CloseTarget";
                }
                else if (hitInfo.collider.tag == "MediumTarget" && lastTarget != "MediumTarget")
                {
                    countHit += 3;
                    Debug.Log("Hit a medium Target, Score + 3 ");
                    countHitText.text = getScoreHit();
                    lastTarget = "MediumTarget";
                }
                else if (hitInfo.collider.tag == "FarTarget" && lastTarget != "FarTarget")
                {
                    countHit += 5;
                    Debug.Log("Hit a far target, Score + 5: ");
                    countHitText.text = getScoreHit();
                    lastTarget = "FarTarget";
                }
                else
                {
                    countMiss++;
                    countMissText.text = getScoreMiss();
                }
            }
            
        }
		else
		{
			_laserSightImpactPoint.SetActive(false);
		}

        var gazePoint = TobiiAPI.GetGazePoint();
       /* if (gazePoint.IsRecent())
        {
            Vector3 target = new Vector3(float.Parse(gazePoint.Screen.x.ToString()), float.Parse(gazePoint.Screen.y.ToString()), 0.0f);
            testTrack.transform.position = Vector3.MoveTowards(testTrack.transform.position, target, Time.deltaTime * 1);
        }*/

        timer.text = getTime();
        if(countdown <= 0.0f)
        {
            GameOver.text = "GameOver! Your score is: " + getResult();
            gameOver = true;
        }
    }
	protected void OnDisable()
	{
		if (_laserSightImpactPoint != null)
		{
			_laserSightImpactPoint.SetActive(false);
		}
	}

    private void SetLaserSightStartAndEndWidth(float startWidth, float endWidth)
    {
#if UNITY_5_5_OR_NEWER
        _laserSight.startWidth = startWidth;
        _laserSight.endWidth = endWidth;
#else
        _laserSight.SetWidth(startWidth, endWidth);
#endif
    }


    public string getScoreHit()
    {
        return "Score: " + countHit.ToString();
    }
    public string getScoreMiss()
    {
        return "Targets Missed: " + countMiss.ToString();
    }
    public string getTime()
    {
        countdown -= Time.deltaTime;
        return "Time: " + countdown.ToString("F2");
    }

    public int getResult()
    {
        int result = 0;
        result = countHit - (countMiss * 2);
        return result;

    }
}