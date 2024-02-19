using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float plVelocityX = 0f;
    private float plVelocityY = 0f;

    private float spawnPosX;
    private float spawnPosY;

    private bool isGrounded = true;
    private bool isJumping = false; //determines if player inputs to jump on next FixedUpdate
    private bool allowInput = true; //determines whether the player can control their character at the given time, but does not affect physics simulation
    private bool stasis = false; //for temporarily putting the player's movement in a stasis where they can't move

    private bool isFlipped = false; //determines if gravity is flipped
    private bool canFlip = true; //determines if the player at a given moment can flip gravity

    private float plSpeed = 0.3f;
    private float plAirFactor = 0.6f; //decreases aerial control
    private float plJumpForce = 0.6f;
    private float gravity = 0.03f;
    private int flipFactor = 1; //positive 1 if not flipped, negative 1 if flipped

    private Vector2 playerSize;
    private LayerMask terrainMask, dangerMask;
    private RaycastHit2D[] hitArray = new RaycastHit2D[2];

    [SerializeField] private LevelScroll levelCam; //scrolling level with camera
    [SerializeField] private Timer timer;

    private BoxCollider2D hitbox;

    public void Respawn() {
        transform.position = new Vector2(spawnPosX,spawnPosY);
        plVelocityX = 0f;
        plVelocityY = 0f;
        isGrounded = true;
        isJumping = false;
        allowInput = true;
        stasis = false;
        canFlip = true;
        isFlipped = false;

        levelCam.ResetPos();
        timer.RestartTimer();
    }

    public void SetSpawn(float xPos, float yPos) {
        spawnPosX = xPos;
        spawnPosY = yPos;
    }

    public void GravityFlip() {
        if(canFlip) {
            canFlip = false;
            isFlipped = !isFlipped;
        }
    }

    void Start()
    {
        hitbox = GetComponent<BoxCollider2D>();
        playerSize = hitbox.bounds.size;
        terrainMask = LayerMask.GetMask("Terrain");
        dangerMask = LayerMask.GetMask("Hazard");

        spawnPosX = transform.position.x;
        spawnPosY = transform.position.y;
    }

    void Update() {
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space)) { //detects if player is trying to jump for next FixedUpdate
            isJumping = true;
        }
    }

    void FixedUpdate()
    {
        if(!stasis) {
            if(isFlipped) flipFactor = -1;
            else flipFactor = 1;
            plVelocityY -= gravity*flipFactor; //constant gravity

            if(isGrounded && allowInput) {
                //Horizontal Movement
                plVelocityX = Input.GetAxisRaw("Horizontal")*plSpeed;

                //Jumping
                if(isJumping) {
                    plVelocityY = plJumpForce*flipFactor;
                    isJumping = false;
                    isGrounded = false;
                }
            } else {
                //limits horizontal air mobility, but does not low top speed from grounded state
                plVelocityX += Input.GetAxisRaw("Horizontal")*plSpeed*plAirFactor;
                plVelocityX = Mathf.Clamp(plVelocityX, -plSpeed, plSpeed);
                isJumping = false;
            }

            transform.position = new Vector2(transform.position.x+plVelocityX, transform.position.y+plVelocityY);

            //Detect all current terrain collisions
            hitArray = Physics2D.BoxCastAll(hitbox.bounds.center, playerSize, 0f, new Vector2(0f,0f), playerSize.x/2f, terrainMask, -0.3f, 0.3f);
            isGrounded = false; //for indicating the player is not grounded if no collision below the player is detected 
            
            for(int i = 0; i < hitArray.Length; i++) {

                Vector2 contactPoint = hitArray[i].point;
                switch((int) Mathf.Ceil(hitArray[i].normal.y)) { //Positive y on normal means the collision was below due to the normal being outward from the collision point
                    case -1:
                        if(plVelocityY > 0) { //player needs to be moving up for this collision
                            plVelocityY = 0;
                            transform.position = new Vector2(transform.position.x, contactPoint.y-playerSize.y/2f); //snap to ceiling upon collision
                            if(isFlipped) Landed();
                            break;
                        } else {
                            break;
                        }
                    case 1:
                        if(plVelocityY < 0) { //player needs to be moving down for this collision
                            plVelocityY = 0;
                            transform.position = new Vector2(transform.position.x, contactPoint.y+playerSize.y/2f); //snap to floor upon collision
                            if(!isFlipped) Landed();
                            break;
                        } else {
                            break;
                        }
                    default:
                        break;
                }

                //Check for horizontal collision data
                switch((int) Mathf.Ceil(hitArray[i].normal.x)) { //Positive x on normal means the collision was from the left . . . this becomes more confusing than verticals
                    case -1:
                        if(plVelocityX > 0) { //player needs to be moving right for this collision
                            plVelocityX = 0;
                            transform.position = new Vector2(contactPoint.x-playerSize.x/2f, transform.position.y); //snap to left wall upon collision
                            break;
                        } else {
                            break;
                        }
                    case 1:
                        if(plVelocityX < 0) { //player needs to be moving left for this collision
                            plVelocityX = 0;
                            transform.position = new Vector2(contactPoint.x+playerSize.x/2f, transform.position.y); //snap to right wall upon collision
                            break;
                        } else {
                            break;
                        }
                    default:
                        break;
                }
            }

            RaycastHit2D hit = Physics2D.BoxCast(hitbox.bounds.center, playerSize*0.9f, 0f, new Vector2(0f,0f), playerSize.x*0.9f/2, dangerMask, -0.1f, 0.1f);
            if(hit.collider != null) {
                Respawn();
            }
        }
    }

    private void Landed() { //reset grounded state
        isGrounded = true;
        canFlip = true;
        plVelocityY = 0f;
    }
}
