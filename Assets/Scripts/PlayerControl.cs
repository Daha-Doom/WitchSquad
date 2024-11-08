using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; 
using Cinemachine;

[RequireComponent(typeof(Rigidbody2D))]

public class PlayerControl : MonoBehaviour
{
    Damageable damageable;
    TouchingDirections touchingDirections;
    Colisions col;

    [SerializeField]
    CinemachineVirtualCamera cameraShotgun, cameraVampire, cameraPotion;
    Rigidbody2D rb;
    Animator animator;

    Vector2 moveInput;

    [SerializeField]
    Transform shotgunWitch, potionWitch, vampireWitch, potionBomb, puf;
    [SerializeField]
    Collider2D wallCheck;

    [SerializeField]
    int witchID;

    [SerializeField]
    Sprite shotgunSptite, vampireSprite, potionSprite;
    [SerializeField]
    GameObject healthRing, leftRing, rightRing;

    private bool canAttack = true;
    private bool canMove = true;
    private bool isMoving;

    [SerializeField]
    private float timerAttack = 1f;
    [SerializeField]
    private float timerMove = 0.75f;

    private float timerAt = 0f;
    private float timerM = 0f;

    public float speed = 5f;
    public float jumpPower = 16f;

    public float curentSpeed
    {
        get
        {
            if (IsMoving && !touchingDirections.IsOnWall && canMove)
                return speed;
            else
                return 0;
        }
    }

    [SerializeField]
    private bool _isMoving = false;
    public bool IsMoving { get
        {
            return _isMoving;
        }
        private set
        {
            _isMoving = value;
            animator.SetBool("IsMoving", value);
        }
    }

    public bool _isFacingRight = true;
    public bool IsFacingRight { get
        {
            return _isFacingRight;
        }
        private set
        {
            if (_isFacingRight != value)
                transform.localScale *= new Vector2(-1, 1);

            _isFacingRight = value;
        }
    }

    public bool IsAlive
    {
        get
        {
            return animator.GetBool("IsAlive");
        }
    }

    // Start is called before the first frame update
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        damageable = GetComponent<Damageable>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (IsAlive)
        {
            rb.velocity = new Vector2(moveInput.x * curentSpeed, rb.velocity.y);

            animator.SetFloat("yVelocity", rb.velocity.y);

            if (!canAttack)
            {
                if (touchingDirections.IsGrounded)
                {
                    timerAt += 1f * Time.deltaTime;

                    if (timerAttack < timerAt)
                    {
                        timerAt = 0f;
                        canAttack = true;
                    }
                }
            }

            if (!canMove)
            {
                timerM += 1f * Time.deltaTime;

                if (timerMove < timerM)
                {
                    timerM = 0f;
                    canMove = true;
                }
            }
        }
        else
        {
            canMove = false;
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.started && touchingDirections.IsGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);

            animator.SetTrigger("Jump");

            canAttack = true;
        }

        //if (context.canceled && rigidbody.velocity.y > 0f)
        //{
        //    rigidbody.velocity = new Vector2(rigidbody.velocity.x, rigidbody.velocity.y * 0.5f);

        //    animator.SetTrigger("Jump");

        //    canShoot = true;
        //}
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        IsMoving = moveInput != Vector2.zero;

        Flip(moveInput);
    }

    public void onAttack(InputAction.CallbackContext context)
    {
        if (touchingDirections.IsGrounded && canAttack)
        {
            animator.SetTrigger("Attack");
            canAttack = false;
            canMove = false;
        }
    }

    public void TrowBomb(InputAction.CallbackContext context)
    {
        if (touchingDirections.IsGrounded && canAttack)
        {
            animator.SetTrigger("Attack");

            Instantiate(potionBomb, new Vector3 (transform.position.x - 0.3f, transform.position.y + 1.5f, 5), Quaternion.identity);

            canAttack = false;
            canMove = false;
        }
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (!touchingDirections.IsGrounded && canAttack)
        {
            animator.SetTrigger("Shoot");
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            canAttack = false;
        }

        if (touchingDirections.IsGrounded && canAttack)
        {
            animator.SetTrigger("Shoot");
            canAttack = false;
            canMove = false;
        }
    }

    private void Flip(Vector2 moveInput)
    {
        if (moveInput.x > 0 && !IsFacingRight)
        {
            IsFacingRight = true;
        }
        else if (moveInput.x < 0 && IsFacingRight)
        {
            IsFacingRight = false;
            
        }
        //Vector3 localScale = transform.localScale;
        //localScale.x *= -1f;
        //transform.localScale = localScale;
    }

    public void ChangeWitchRight(InputAction.CallbackContext context)
    {
        Vector3 positionWitch = this.transform.position;

        switch (witchID)
        {
            case 1:
                {
                    cameraShotgun.Priority = 1;
                    cameraVampire.Priority = 2;

                    vampireWitch.gameObject.SetActive(true);
                    vampireWitch.transform.position = positionWitch;

                    PlayerControl playerControl = vampireWitch.GetComponent<PlayerControl>();
                    Damageable dm = vampireWitch.GetComponent<Damageable>();

                    playerControl.IsFacingRight = IsFacingRight;
                    dm.Health = damageable.Health;

                    healthRing.GetComponent<SpriteRenderer>().sprite = vampireSprite;
                    leftRing.GetComponent<SpriteRenderer>().sprite = shotgunSptite;
                    rightRing.GetComponent<SpriteRenderer>().sprite = potionSprite;

                    this.gameObject.SetActive(false);
                }
                break;
            case 2:
                {
                    cameraVampire.Priority = 1;
                    cameraPotion.Priority = 2;

                    potionWitch.gameObject.SetActive(true);
                    potionWitch.transform.position = positionWitch;

                    PlayerControl playerControl = potionWitch.GetComponent<PlayerControl>();
                    Damageable dm = potionWitch.GetComponent<Damageable>();

                    playerControl.IsFacingRight = IsFacingRight;
                    dm.Health = damageable.Health;

                    healthRing.GetComponent<SpriteRenderer>().sprite = potionSprite;
                    leftRing.GetComponent<SpriteRenderer>().sprite = vampireSprite;
                    rightRing.GetComponent<SpriteRenderer>().sprite = shotgunSptite;

                    this.gameObject.SetActive(false);
                }
                break;

            case 3:
                {
                    cameraPotion.Priority = 1;
                    cameraShotgun.Priority = 2;

                    shotgunWitch.gameObject.SetActive(true);
                    shotgunWitch.transform.position = positionWitch;

                    PlayerControl playerControl = shotgunWitch.GetComponent<PlayerControl>();
                    Damageable dm = shotgunWitch.GetComponent<Damageable>();

                    playerControl.IsFacingRight = IsFacingRight;
                    dm.Health = damageable.Health;

                    healthRing.GetComponent<SpriteRenderer>().sprite = shotgunSptite;
                    leftRing.GetComponent<SpriteRenderer>().sprite = potionSprite;
                    rightRing.GetComponent<SpriteRenderer>().sprite = vampireSprite;

                    this.gameObject.SetActive(false);
                }
                break;
        }
    }

    public void ChangeWitchLeft(InputAction.CallbackContext context)
    {
        Vector3 positionWitch = this.transform.position;

        switch (witchID)
        {
            case 1:
                {
                    cameraShotgun.Priority = 1;
                    cameraPotion.Priority = 2;

                    potionWitch.gameObject.SetActive(true); 
                    potionWitch.transform.position = positionWitch;
                    
                    PlayerControl playerControl = potionWitch.GetComponent<PlayerControl>();
                    Damageable dm = potionWitch.GetComponent<Damageable>();

                    playerControl.IsFacingRight = IsFacingRight;
                    dm.Health = damageable.Health;

                    healthRing.GetComponent<SpriteRenderer>().sprite = potionSprite;
                    leftRing.GetComponent<SpriteRenderer>().sprite = vampireSprite;
                    rightRing.GetComponent<SpriteRenderer>().sprite = shotgunSptite;

                    this.gameObject.SetActive(false);
                }
                break;
            case 2:
                {
                    cameraVampire.Priority = 1;
                    cameraShotgun.Priority = 2;

                    shotgunWitch.gameObject.SetActive(true);
                    shotgunWitch.transform.position = positionWitch;

                    PlayerControl playerControl = shotgunWitch.GetComponent<PlayerControl>();
                    Damageable dm = shotgunWitch.GetComponent<Damageable>();

                    playerControl.IsFacingRight = IsFacingRight;
                    dm.Health = damageable.Health;

                    healthRing.GetComponent<SpriteRenderer>().sprite = shotgunSptite;
                    leftRing.GetComponent<SpriteRenderer>().sprite = potionSprite;
                    rightRing.GetComponent<SpriteRenderer>().sprite = vampireSprite;

                    this.gameObject.SetActive(false);
                }
                break;

            case 3:
                {
                    cameraPotion.Priority = 1;
                    cameraVampire.Priority = 2;

                    vampireWitch.gameObject.SetActive(true);
                    vampireWitch.transform.position = positionWitch;

                    PlayerControl playerControl = vampireWitch.GetComponent<PlayerControl>();
                    Damageable dm = vampireWitch.GetComponent<Damageable>();

                    playerControl.IsFacingRight = IsFacingRight;
                    dm.Health = damageable.Health;

                    healthRing.GetComponent<SpriteRenderer>().sprite = vampireSprite;
                    leftRing.GetComponent<SpriteRenderer>().sprite = shotgunSptite;
                    rightRing.GetComponent<SpriteRenderer>().sprite = potionSprite;

                    this.gameObject.SetActive(false);
                }
                break;
        }
    }

    public void ActiveShotgun(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Vector3 positionWitch = this.transform.position;

            shotgunWitch.gameObject.SetActive(true);
            shotgunWitch.position = positionWitch;
            cameraShotgun.Priority += 2;
            this.gameObject.SetActive(false);
        }
    }

    public void ActiveVampire(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Vector3 positionWitch = this.transform.position;

            vampireWitch.gameObject.SetActive(true);
            vampireWitch.position = positionWitch;
            cameraVampire.Priority += 2;
            this.gameObject.SetActive(false);
        }
    }

    public void ActivePotion(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Vector3 positionWitch = this.transform.position;

            potionWitch.gameObject.SetActive(true);
            potionWitch.position = positionWitch;
            cameraPotion.Priority += 2;
            this.gameObject.SetActive(false);
        }
    }

    public void Teleport(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            col = wallCheck.GetComponent<Colisions>();

            if (!col.colision)
            {
                Instantiate(puf, new Vector3(transform.position.x, transform.position.y + 1f, 5), Quaternion.identity);

                transform.position = new Vector3(transform.position.x + (IsFacingRight ? 3 : -3), transform.position.y, transform.position.z);

                Instantiate(puf, new Vector3(transform.position.x, transform.position.y + 1f, 5), Quaternion.identity);
            }
        }
    }
}
