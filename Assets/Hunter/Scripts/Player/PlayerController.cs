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
        if(x > 0)
        {
            xScale = 1;
            this.isFacingRight = true;
        }
        else if(x < 0)
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

        if(isBlockedX(bound1, bound2, Mathf.Abs(x)))
        {
            x = 0;
        }

        if(!isGrounded(bound1))
        {
            this.ySpeed = this.ySpeed + this.playerGravity;
            if(!this.hasDoubleJumped && jumpDown)
            {
                this.ySpeed = jumpPower;
                this.hasDoubleJumped = true;
            }
        }
        else if(ySpeed <= 0)
        {
            this.hasJumped = this.hasDoubleJumped = false;
            if(jumpDown)
            {
                this.hasJumped = true;
                this.ySpeed = this.jumpPower;
            }
            else
            {
                
                RaycastHit2D hit = Physics2D.Raycast(bound1, Vector2.down,.1f, 1 << 8);
                if(hit.distance >.01f)
                {
                    this.ySpeed = -(hit.distance -.01f);
                    fixBottom = true;
                }
                else
                {
                    this.ySpeed = 0;
                }
                
            } 
        }
        if(ySpeed > 0)//check if his dumb head will hit something while jumping
        {
            if(isBlockedY(bound1, bound2))
            {
                this.ySpeed = 0;
            }
        }

        y = ySpeed * Time.deltaTime;
        if(fixBottom && (ySpeed < 0))
        {
            y = ySpeed;
            fixBottom = false;
        } 
        transform.Translate(x, y, 0);
        //ySpeed = y;
      //  animator.SetFloat("move", move);

        animator.SetBool("isWalking", this.isWalking);
        animator.SetBool("isFacingRight", this.isFacingRight);
        if(this.ySpeed < 0)
        {
            print(this.ySpeed);
        }
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

    private bool isGrounded(Vector3 v3)
    {
        var origin = transform.position;
        origin.y = v3.y;
        int layerMask = 1 << 8;
        Debug.DrawRay(origin, -Vector3.up, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down);
        return Physics2D.Raycast(origin, Vector2.down, .1f, layerMask);
    }

    private bool isBlockedX(Vector3 min, Vector3 max, float xDist)
    {
        Vector2 topPoint;
        Vector2 bottomPoint;

        Vector2 direction;
        if(isFacingRight)
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

        hit = Physics2D.Raycast(topPoint, direction,  xDist, layerMask);
        hit |= Physics2D.Raycast(bottomPoint, direction, xDist, layerMask);
        return (hit);

    }

    private bool isBlockedY(Vector3 min, Vector3 max)
    {
        Vector2 leftPoint = new Vector2(min.x, max.y);
        Vector2 rightPoint = new Vector2(max.x, max.y);


        return false;

    }
}