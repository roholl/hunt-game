using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow_move : MonoBehaviour {

    public float xSpeed;
    public float ySpeed;
    public float aAngle;
    public float gravY;
    public float bulletLifeTime;


	// Use this for initialization
	void Start () {


	}
	
	// Update is called once per frame
	void Update () {


        var oldx = transform.position.x;
        var oldy = transform.position.y;

        this.ySpeed = this.ySpeed + (this.gravY * Time.deltaTime);
        
        var yMove = this.ySpeed * Time.deltaTime;
        var xMove = this.xSpeed * Time.deltaTime;
        var directionNew = (new Vector3(transform.position.x + xMove, transform.position.y + yMove) - transform.position);
        
        transform.Translate(new Vector2(xMove, yMove ), Space.World);
        var distanceX = transform.position.x - oldx;
        var distanceY = transform.position.y - oldy;

        var newRadians = Mathf.Atan2(distanceY, distanceX);
        transform.rotation = Quaternion.Euler(0, 0, (newRadians * Mathf.Rad2Deg));


    }

    private void Awake()
    {
        Destroy(this.gameObject, this.bulletLifeTime);
    }
}

