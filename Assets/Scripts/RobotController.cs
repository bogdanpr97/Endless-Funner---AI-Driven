using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
   public float moveSpeed;
    public float speedMultiplier;
    public float speedIncreaseMilestone;
    private float moveSpeedStore;
    private float speedIncreaseMilestoneStore;
    public bool isRobot;

    private float speedMilestoneCount;
    private float speedMilestoneCountStore;

	public LayerMask whatIsGround;
    public Transform groundCheck;
    public float groundCheckRadius;
    private bool isOnGround;

    public float jumpForce;
    public float jumpTime;
    private float jumpTimeCounter;

    private bool stoppedJumping;
    private bool canDoubleJump;

	private Rigidbody2D rb;
	private Collider2D col;
	private Animator anim;

    public GameManager theGameManger;

    public AudioSource jumpSound;
    public AudioSource deathSound;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        jumpTimeCounter = jumpTime;
        speedMilestoneCount = speedIncreaseMilestone;
        moveSpeedStore = moveSpeed;
        speedMilestoneCountStore = speedMilestoneCount;
        speedIncreaseMilestoneStore = speedIncreaseMilestone;
        stoppedJumping = true;
    }

    // Update is called once per frame
    void Update()
    {
        isOnGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
        if(transform.position.x > speedMilestoneCount)
        {
            speedMilestoneCount += speedIncreaseMilestone;
            speedIncreaseMilestone = speedIncreaseMilestone * speedMultiplier;
            moveSpeed = moveSpeed * speedMultiplier;
        }
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);

        if(!isRobot) {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(isOnGround) 
            {
             rb.velocity = new Vector2(rb.velocity.x, jumpForce);
             stoppedJumping = false;
             jumpSound.Play();
            }
            if(!isOnGround && canDoubleJump)
            {
                canDoubleJump = false;
                stoppedJumping = false;
                jumpTimeCounter = jumpTime;
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
	            jumpSound.Play();
            }
        }

        if(Input.GetKey(KeyCode.Space) && !stoppedJumping)
        {
            if(jumpTimeCounter > 0)
            {
                 rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                 jumpTimeCounter -= Time.deltaTime;
            }
        }

        if(Input.GetKeyUp(KeyCode.Space))
        {
           jumpTimeCounter = 0;
           stoppedJumping = true;
        }

        if(isOnGround) 
        {
            jumpTimeCounter = jumpTime;
            canDoubleJump = true;
        }
    }


        anim.SetFloat("Speed", rb.velocity.x);
        anim.SetBool("Grounded", isOnGround);

    }

    public void RobotJump()
    {
        isOnGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
        if(isOnGround) 
        {
             rb.velocity = new Vector2(rb.velocity.x, jumpForce);
             jumpSound.Play();
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Killbox")
        {
            // theGameManger.RestartGame();
            if(!isRobot) {
            gameObject.SetActive(false);
           }
            moveSpeed = moveSpeedStore;
            speedMilestoneCount = speedMilestoneCountStore;
            speedIncreaseMilestone = speedIncreaseMilestoneStore;
            deathSound.Play();
        }
    }
}
