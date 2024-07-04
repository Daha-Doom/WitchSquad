using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    Colisions colisionCell, colisionFloor, colisionDownFlor, colisionPlayer;
    TouchingDirections touchingDirections;

    Rigidbody2D rb;
    [SerializeField] Animator animator;

    [SerializeField]
    Vector2 walkDirectionVector = Vector2.left;

    [SerializeField]
    LayerMask layerPlayer, layerGroung;

    RaycastHit2D playerDetected, wallDetected;

    [SerializeField] Collider2D cellDetected, floorDetected, downFloorDetected, playerHitBox;

    [SerializeField] bool patrol = true;
    [SerializeField] bool canMove = true;
    [SerializeField] bool canAttack = true;

    GameObject player;

    [SerializeField] float rayDistancePlayer, rayDistanceWall;
    public float speed = 6f;
    public float jumpPower = 6f;
    public float kickPower = 60f;
    private float timerProverka = 5f, timerP = 5f,
        timerAttack = 1f, timerA = 0f,
        dieTime = 1f, dieTimer = 0f;

    public enum WalkDirection { Right, Left};

    [SerializeField]
    private WalkDirection _walkDirection;

    public WalkDirection walkDirection
    {
        get { return _walkDirection; }
        set
        {
            if (_walkDirection != value)
            {
                _walkDirection = value;

                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * (-1), gameObject.transform.localScale.y);

                if (value == WalkDirection.Right)
                {
                    walkDirectionVector = Vector2.right;
                }
                else
                {
                    walkDirectionVector = Vector2.left;
                }
            }
            _walkDirection = value;
        }
    }

    [SerializeField]
    private bool _isMoving = false;
    public bool IsMoving
    {
        get
        {
            return _isMoving;
        }
        private set
        {
            _isMoving = value;
            animator.SetBool("IsMoving", value);
        }
    }

    public float curentSpeed
    {
        get
        {
            if (/*IsMoving && */!touchingDirections.IsOnWall && canMove)
                return speed;
            else
                return 0;
        }
    }

    public bool IsAlive
    {
        get
        {
            return animator.GetBool("IsAlive");
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();

        colisionCell = cellDetected.GetComponent<Colisions>();
        colisionFloor = floorDetected.GetComponent<Colisions>();
        colisionDownFlor = downFloorDetected.GetComponent<Colisions>();
        colisionPlayer = playerHitBox.GetComponent<Colisions>();
    }

    private void FixedUpdate()
    {
        if (IsAlive)
        {
            player = GameObject.FindGameObjectWithTag("Player");

            rb.velocity = new Vector2(walkDirectionVector.x * curentSpeed, rb.velocity.y);

            IsMoving = rb.velocity != Vector2.zero;

            if (!colisionFloor && !colisionDownFlor)
                FlipDirection();

            if (patrol)
                PatrolMod();
            else
                FollowMode();

            if (!canAttack)
            {
                if (timerA < timerAttack)
                    timerA += Time.deltaTime;
                else
                {
                    canAttack = true;
                    timerA = 0f;
                }
            }
        }
        else
        {
            if (dieTimer < dieTime)
                dieTimer += Time.deltaTime;
            else Destroy(gameObject);
        }
    }

    private void FlipDirection()
    {
        if (walkDirection == WalkDirection.Right)
            walkDirection = WalkDirection.Left;
        else
            walkDirection = WalkDirection.Right;
    }

    private void Jump()
    {
        rb.velocity = Vector2.up * jumpPower;
        animator.SetTrigger("Jump");
    }

    private void PatrolMod()
    {
        System.Random random = new System.Random();

        if (timerP >= timerProverka)
        {
            wallDetected = Physics2D.Raycast(transform.position, transform.localScale.x * Vector2.left, rayDistanceWall, layerGroung);

            if (wallDetected.collider != null)
            {
                canMove = false;

                int doList = random.Next(0, 2);

                switch (doList)
                {
                    case 0:
                        FlipDirection();
                        break;
                    case 1:

                        if (!colisionCell.colision && touchingDirections.IsGrounded)
                        {
                            Jump();
                            canMove = true;
                            timerP = 0;
                        }
                        else FlipDirection();
                        break;
                }
            }
            else canMove = true;

            if (!colisionFloor.colision && colisionDownFlor.colision && touchingDirections.IsGrounded)
            {
                canMove = false;

                int doList = random.Next(0, 2);

                switch (doList)
                {
                    case 0:
                        FlipDirection();
                        break;
                    case 1:
                        canMove = true;
                        rb.velocity = walkDirectionVector * kickPower;
                        timerP = 0f;
                        break;
                }
            }
            else if (!colisionFloor.colision && touchingDirections.IsGrounded)
            {
                canMove = true;
                FlipDirection();
            }
        }
        else timerP += 0.1f;

        //if (touchingDirections.IsGrounded)
        //Jump();

        playerDetected = Physics2D.Raycast(transform.position, transform.localScale.x * Vector2.left, rayDistancePlayer, layerPlayer);

        if (playerDetected.collider != null)
        {
            patrol = false;
        }
    }

    private void FollowMode()
    {
        if ((player.transform.position - this.transform.position).sqrMagnitude > 100f)
            patrol = true;
        else
        {
            if (timerP >= timerProverka)
            {
                if (player.transform.position.x > this.transform.position.x && walkDirection != WalkDirection.Right)
                    FlipDirection();
                if (player.transform.position.x < this.transform.position.x && walkDirection == WalkDirection.Right)
                    FlipDirection();

                wallDetected = Physics2D.Raycast(transform.position, transform.localScale.x * Vector2.left, rayDistanceWall, layerGroung);

                if (wallDetected.collider != null)
                    if (!colisionCell.colision && touchingDirections.IsGrounded)
                    {
                        Jump();
                        canMove = true;
                        timerP = 0;
                    }
                    else canMove = false;

                if (colisionPlayer.colision && touchingDirections.IsGrounded)
                {
                    canMove = false;
                    timerP = 3f;
                    if (canAttack)
                        Attack();
                }
                else canMove = true;

                if (!colisionFloor.colision)
                    if (colisionDownFlor.colision && touchingDirections.IsGrounded)
                    {
                        canMove = true;
                        rb.velocity = walkDirectionVector * kickPower;
                    }
                    else canMove = false;
            }
            else timerP += 0.1f;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        //Gizmos.DrawLine(transform.position, transform.position + transform.localScale.x * Vector3.left * rayDistanceWall);
    }

    private void Attack()
    {
        animator.SetTrigger("Attack");
        canAttack = false;
    }
}
