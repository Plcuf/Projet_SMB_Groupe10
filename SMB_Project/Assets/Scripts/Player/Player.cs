using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    public int moveSpeed = 2;
    public int sprintMutliplier = 2;
    public float jumpForce = 1.0f;
    public bool bGrounded = true;
    public bool bLeftWall = false;
    public bool bRightWall = false;
    public bool bSprinting = false;
    public bool bJumped = false;

    private PlayerControls ia;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private Rigidbody2D rb;

    public Transform spawnPoint;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        ia = new PlayerControls();
    }

    private void OnEnable()
    {
        moveAction = ia.Player.Move;
        moveAction.Enable();
        jumpAction = ia.Player.Jump;
        jumpAction.Enable();
        sprintAction = ia.Player.Sprint;
        sprintAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        sprintAction.Disable();
    }

    private void FixedUpdate()
    {
        bSprinting = sprintAction.IsPressed();

        //vérifie les collisions avec le sol et les murs
        bGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.6f, 3);
        bLeftWall = Physics2D.Raycast(transform.position, Vector2.left, 0.7f, 3) || Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y+0.5f), Vector2.left, 0.7f, 3) || Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y-0.5f), Vector2.left, 0.7f, 3);
        bRightWall = Physics2D.Raycast(transform.position, Vector2.right, 0.7f, 3) || Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y+0.5f), Vector2.right, 0.7f, 3) || Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y-0.5f), Vector2.right, 0.7f, 3);

        if (bGrounded || !jumpAction.IsPressed())
        {
            bJumped = false;
        }

        if (moveAction.IsPressed())
        {
            Move();
        }

        if(jumpAction.IsPressed())
        {
            if (bGrounded)
            {
                Jump();
            } else if(bLeftWall)
            {
                WallJumpLeft();
            } else if(bRightWall)
            {
                WallJumpRight();
            } else if(bJumped)
            {
                MaintainJump();
            }
        }
    }

    private void Move()
    {
        float actualMoveSpeed = moveSpeed;
        if(bSprinting)
        {
            actualMoveSpeed *= sprintMutliplier;
        }

        Vector2 movDir = moveAction.ReadValue<Vector2>();

        //empêche qu'on colle au mur sans tomber
        if(bRightWall && movDir.x > 0)
        {
            movDir.x = 0;
        }
        if(bLeftWall && movDir.x < 0)
        {
            movDir.x = 0;
        }

        //le vector qu'on veut atteindre
        Vector2 targetVector = new Vector2(movDir.x * actualMoveSpeed, rb.linearVelocity.y);
        //applique une transition entre l'ancien vecteur et celui qu'on veut atteindre
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVector, 5f * Time.fixedDeltaTime);
    }

    private void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        bJumped = true;
    }

    private void WallJumpLeft()
    {
        if (!bJumped)
        {
            rb.linearVelocity = new Vector2(0, 0);
            rb.AddForce(Vector2.up * jumpForce*1.5f, ForceMode2D.Impulse);
            rb.AddForce(Vector2.right * jumpForce*2, ForceMode2D.Impulse);
            bJumped = true;
        }
    }

    private void WallJumpRight()
    {
        if(!bJumped)
        {
            rb.linearVelocity = new Vector2(0, 0);
            rb.AddForce(Vector2.up * jumpForce*1.5f, ForceMode2D.Impulse);
            rb.AddForce(Vector2.left * jumpForce*2, ForceMode2D.Impulse);
            bJumped = true;
        }
    }

    private void MaintainJump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Force);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Kill")
        {
            collision.gameObject.GetComponent<Sawblades>().ChangeSawblade(true);
            transform.position = spawnPoint.position;
        }
    }
}
