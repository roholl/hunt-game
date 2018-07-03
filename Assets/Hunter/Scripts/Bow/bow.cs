using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bow : MonoBehaviour {

    public GameObject arrowPrefab;
    private GameObject parentObj;
    public float maxPower;
    public float minPower;
    public float powerBuildFactor;
    private float currentPower;
    private bool fireHeld = false;


	// Use this for initialization
	void Start () {
        parentObj = transform.parent.gameObject;
        currentPower = minPower;
	}
	
	// Update is called once per frame
	void Update () {

        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.parent.position;
        float radAngle = Mathf.Atan2(direction.y, direction.x);
        float angle = (radAngle * Mathf.Rad2Deg); 
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        //transform.rotation = rotation;
        transform.parent.rotation = rotation;
        //transform.parent.eulerAngles
        //transform.RotateAround(parentObj.transform.position, angle);
        //transform.Rotate(Vector3.forward, angle, Space.Self);

        if(Input.GetMouseButton(0))
        {
            currentPower += Time.deltaTime * powerBuildFactor;
            currentPower = Mathf.Min(currentPower, maxPower);
            fireHeld = true;
        }
        if(Input.GetMouseButtonUp(0))
        {
            currentPower = minPower;
            fireHeld = false;
        }

        if(Input.GetMouseButtonDown(0))
        {
            Shoot(transform.parent.localEulerAngles.z);
        }
		
	}

    void Shoot(float aAngle)
    {
        var bullet = (GameObject)Instantiate(arrowPrefab, transform.position, transform.parent.rotation,null);
        bullet.GetComponent<Arrow_move>().aAngle = aAngle;

        print(aAngle);

        bullet.GetComponent<Arrow_move>().xSpeed = maxPower * Mathf.Cos(aAngle * Mathf.Deg2Rad);

       // bullet.GetComponent<Arrow_move>().xSpeed = maxPower * Mathf.Cos(transform.parent.rotation.eulerAngles.z * Mathf.Rad2Deg);
       // bullet.GetComponent<Arrow_move>().ySpeed = maxPower * Mathf.Sin(transform.parent.rotation.eulerAngles.z * Mathf.Rad2Deg);

       // print(bullet.GetComponent<Arrow_move>().xSpeed);
        bullet.GetComponent<Arrow_move>().ySpeed = maxPower * Mathf.Sin(aAngle * Mathf.Deg2Rad);
        //print(bullet.GetComponent<Arrow_move>().ySpeed);





        //   bullet.
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if(collision.)
    }


    private void LateUpdate()
    {
        
    }

}



