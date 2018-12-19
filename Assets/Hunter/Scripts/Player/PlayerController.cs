using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public float jumpPower;
    public int currHealth;
    public int maxHealth;
    public float playerGravity;
    public float maxClimbAngle;



    float ySpeed = 0;
    float xSpeed = 0;
    bool hasJumped = false;
    bool hasDoubleJumped = false;
    bool isFacingRight = true;
    bool isFalling = false;
    float xScale = 1;
    bool isWalking = false;
    bool fixBottom = false;



    private Animator animator;
    private SpriteRenderer spRender;
    private BoxCollider2D bbox;
    private float prevY;
    private Vector3 velocity;


    private void Start()
    {
        animator = GetComponent<Animator>();

        spRender = animator.GetComponent<SpriteRenderer>();
        // GetComponent<SpriteRenderer>();
        bbox = GetComponent<BoxCollider2D>();

    }

    void Update()
    {

        prevY = transform.position.y;
        bool jumpDown = Input.GetButtonDown("Jump");
        var move = Input.GetAxis("Horizontal");
        var x = move * Time.deltaTime * moveSpeed;
        //var y = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

        this.isWalking = true;
        if (x > 0)
        {
            xScale = 1;
            this.isFacingRight = true;
        }
        else if (x < 0)
        {
            xScale = -1;
            this.isFacingRight = false;
        }
        else
        {
            this.isWalking = false;
        }




        //  var bbox = GetComponent<BoxCollider2D>(); //Players Collider
        var bound1 = bbox.bounds.min;//Min x and y points on collider
        var bound2 = bbox.bounds.max;//Max x and y point on collider 

        var y = 0f;

      

        if (!isGrounded(bound1, bound2, Mathf.Abs(this.ySpeed)))
        {
            this.ySpeed = this.ySpeed + this.playerGravity;
            if (!this.hasDoubleJumped && jumpDown)
            {
                this.ySpeed = jumpPower;
                this.hasDoubleJumped = true;
            }
        }
        else if (ySpeed <= 0)
        {
            this.hasJumped = this.hasDoubleJumped = false;
            if (jumpDown)
            {
                this.hasJumped = true;
                this.ySpeed = this.jumpPower;
            }
            else
            {

                RaycastHit2D hit;
               // RaycastHit2D hit = Physics2D.Raycast(bound1, Vector2.down, .1f, 1 << 8);

                RaycastHit2D hit1 = Physics2D.Raycast(bound1, Vector2.down, .1f, 1 << 8);
                RaycastHit2D hit2 = Physics2D.Raycast(bound2, Vector2.down, .1f, 1 << 8);


                float hitDistance = isGroundedF(bound1, bound2, Mathf.Abs(this.ySpeed));

                if(hit1.distance <= hit2.distance)
                {
                    hit = hit1;
                }
                else
                {
                    hit = hit2;
                }

               // Debug.DrawRay(bound1, Vector2.down, Color.yellow);
                if (hit.distance > .01f)
                {
                    // this.ySpeed = -(hit.distance - .01f);
                    this.ySpeed = hitDistance;
                    //fixBottom = true;
                }
                else
                {
                    this.ySpeed = 0;
                }

            }
        }
        if (ySpeed > 0)//check if his dumb head will hit something while jumping
        {
            if (isBlockedY(bound1, bound2))
            {
                this.ySpeed = 0;
            }
        }

        y = ySpeed * Time.deltaTime;
       /* if (fixBottom && (ySpeed < 0))
        {
            y = ySpeed;
            fixBottom = false;
        }
        */

        if (isBlockedX(bound1, bound2, (Mathf.Abs(x) > .05f) ? Mathf.Abs(x): .05f))
        {
            RaycastHit2D hitBottom = isBlockedBottomRightX(bound1, bound2, Mathf.Abs(x));

            float slopeAngle = Vector2.Angle(hitBottom.normal, Vector2.up);



            if (slopeAngle <= maxClimbAngle)
            {
                velocity.x = x;
                velocity.y = y;
                ClimbSlope(ref velocity, slopeAngle);
                x = velocity.x;
                y = velocity.y;
            }
            else
            {
                x = 0;
            }
            
        }


        transform.Translate(x, y, 0);
        //ySpeed = y;
        //  animator.SetFloat("move", move);

        animator.SetBool("isWalking", this.isWalking);
        animator.SetBool("isFacingRight", this.isFacingRight);
      /*  if (this.ySpeed < 0)
        {
            print(this.ySpeed);
        }
        */
        animator.SetBool("isFalling", (this.ySpeed < -.01) ? true : false);


        

    }

    private void LateUpdate()
    {
        if (!this.isFacingRight)
        {
            this.spRender.flipX = true;

        }
        else
        {
            this.spRender.flipX = false;
        }
    }

    private bool isGrounded(Vector3 min, Vector3 max, float yDist)
    {
        Vector2 leftPoint;
        Vector2 rightPoint;


        leftPoint = new Vector2(min.x, min.y);
        rightPoint = new Vector2(max.x, min.y);

        var origin = transform.position;
        origin.y = min.y;
        int layerMask = 1 << 8;
        Debug.DrawRay(origin, -Vector3.up, Color.red);
        Debug.DrawRay(leftPoint, Vector2.down, Color.red);
        Debug.DrawRay(rightPoint, Vector2.down, Color.red);
        //Debug
        bool hit = false;

     //   hit  = Physics2D.Raycast(origin, Vector2.down);
        hit |= Physics2D.Raycast(leftPoint, Vector2.down, .1f, layerMask);
        hit |=  Physics2D.Raycast(rightPoint, Vector2.down, .1f, layerMask);
        //return Physics2D.Raycast(origin, Vector2.down, .1f, layerMask);
        return (hit);
    }

    private float isGroundedF(Vector3 min, Vector3 max, float yDist)
    {
        Vector2 leftPoint;
        Vector2 rightPoint;


        leftPoint = new Vector2(min.x, min.y);
        rightPoint = new Vector2(max.x, min.y);

        var origin = transform.position;
        origin.y = min.y;
        int layerMask = 1 << 8;
        Debug.DrawRay(origin, -Vector3.up, Color.red);
        Debug.DrawRay(leftPoint, Vector2.down, Color.red);
        Debug.DrawRay(rightPoint, Vector2.down, Color.red);
        //Debug
        float hit = -1.0f;

        //   hit  = Physics2D.Raycast(origin, Vector2.down);
        RaycastHit2D hit1 = Physics2D.Raycast(leftPoint, Vector2.down, .1f, layerMask);
        RaycastHit2D hit2 = Physics2D.Raycast(rightPoint, Vector2.down, .1f, layerMask);

        if(hit1.distance <= hit2.distance)
        {
            hit = hit1.distance;
        }
        else
        {
            hit = hit2.distance;
        }

        //hit |= Physics2D.Raycast(leftPoint, Vector2.down, .1f, layerMask);
        //hit |= Physics2D.Raycast(rightPoint, Vector2.down, .1f, layerMask);
        //return Physics2D.Raycast(origin, Vector2.down, .1f, layerMask);
        return (hit);
    }

    // private 

    private bool isBlockedX(Vector3 min, Vector3 max, float xDist)
    {
        Vector2 topPoint;
        Vector2 bottomPoint;

        Vector2 direction;
        if (isFacingRight)
        {
            direction = Vector2.right;
            //max x, max y
            //max x, min y
            topPoint = new Vector2(max.x, max.y);
            bottomPoint = new Vector2(max.x, min.y);

        }
        else
        {
            //min x, max y
            //min x, min y
            topPoint = new Vector2(min.x, max.y);
            bottomPoint = new Vector2(min.x, min.y);
            direction = Vector2.left;
        }

        int layerMask = 1 << 8;
        //   layerMask = ~layerMask;
        Debug.DrawRay(topPoint, direction, Color.blue);
        Debug.DrawRay(bottomPoint, direction, Color.blue);

        bool hit = false;

        hit = Physics2D.Raycast(topPoint, direction, xDist, layerMask);
        hit |= Physics2D.Raycast(bottomPoint, direction, xDist, layerMask);
        return (hit);

    }
    

    private RaycastHit2D isBlockedBottomRightX(Vector3 min, Vector3 max, float xDist)
    {
        Vector2 topPoint;
        Vector2 bottomPoint;

        Vector2 direction;
        if (isFacingRight)
        {
            direction = Vector2.right;
            //max x, max y
            //max x, min y
         //   topPoint = new Vector2(max.x, max.y);
            bottomPoint = new Vector2(max.x, min.y);

        }
        else
        {
            //min x, max y
            //min x, min y
            //topPoint = new Vector2(min.x, max.y);
            bottomPoint = new Vector2(min.x, min.y);
            direction = Vector2.left;
        }

        int layerMask = 1 << 8;
        //   layerMask = ~layerMask;
    //    Debug.DrawRay(topPoint, direction, Color.blue);
      //  Debug.DrawRay(bottomPoint, direction, Color.blue);

        //bool hit = false;

        RaycastHit2D hit = Physics2D.Raycast(bottomPoint, direction, xDist, layerMask);
        return (hit);

    }

    private bool isBlockedY(Vector3 min, Vector3 max)
    {
        Vector2 leftPoint = new Vector2(min.x, max.y);
        Vector2 rightPoint = new Vector2(max.x, max.y);


       // bool hit = false;

       // hit = Physics2D.Raycast(topPoint, direction, xDist, layerMask);
        //hit |= Physics2D.Raycast(bottomPoint, direction, xDist, layerMask);

        return false;

    }

    void ClimbSlope(ref Vector3 velocity, float slopeAngle)
    {
        float moveDistance = Mathf.Abs(velocity.x);

        velocity.y = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
        velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("Hello");
    }


}