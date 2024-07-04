using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchingDirections : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField]
    Animator animator;
    CapsuleCollider2D touchingCol;
    public bool levsha = false;

    RaycastHit2D[] groundHits = new RaycastHit2D[5];
    RaycastHit2D[] wallHits = new RaycastHit2D[5];

    public ContactFilter2D castFilter = new ContactFilter2D();

    private Vector2 wallCheckDirection => gameObject.transform.localScale.x > 0 ? (levsha ? Vector2.left : Vector2.right) : (levsha ? Vector2.right : Vector2.left);

    public float groundDistance = 0.05f;
    public float wallDistance = 0.2f;

    [SerializeField]
    private bool _isGrounded = true;
    [SerializeField]
    private bool _isOnWall = true;

    public bool IsGrounded
    {
        get
        {
            return _isGrounded;
        }
        private set
        {
            _isGrounded = value;
            animator.SetBool("IsGrounded", value);
        }
    }

    public bool IsOnWall
    {
        get
        {
            return _isOnWall;
        }
        private set
        {
            _isOnWall = value;
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingCol = GetComponent<CapsuleCollider2D>();
        if (animator == null)
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        IsGrounded = touchingCol.Cast(Vector2.down, castFilter, groundHits, groundDistance) > 0;
        IsOnWall = touchingCol.Cast(wallCheckDirection, castFilter, wallHits, wallDistance) > 0;
    }
}
