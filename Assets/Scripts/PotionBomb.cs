using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class PotionBomb : MonoBehaviour
{
    [SerializeField]
    PlayerControl playerControl;

    int directionTrow;

    Rigidbody2D rb;
    public float trowPower, distance = 0.2f;

    RaycastHit2D hit;

    [SerializeField] LayerMask enemyLayer;
    [SerializeField] LayerMask groundLayer;

    [SerializeField] Transform exsplosionGround;
    [SerializeField] Transform exsplosionAir;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        playerControl = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();

        directionTrow = playerControl.IsFacingRight ? 2 : -2;
    }

    private void Start()
    {
        rb.velocity = new Vector2(directionTrow, 1) * trowPower;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Contact with " + collision);

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Instantiate(exsplosionAir, this.transform.position, Quaternion.identity);

            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("ground"))
        {
            hit = Physics2D.Raycast(transform.position, Vector2.right * transform.localScale.x, distance, groundLayer);

            if (hit.collider != null)
            {
                Debug.Log("Right");

                Instantiate(exsplosionGround, this.transform.position, Quaternion.Euler(new Vector3(0, 0, 90)));

                Destroy(gameObject);
            }

            else
            {
                hit = Physics2D.Raycast(transform.position, Vector2.left * transform.localScale.x, distance, groundLayer);

                if (hit.collider != null)
                {
                    Debug.Log("Left");

                    Instantiate(exsplosionGround, this.transform.position, Quaternion.Euler(new Vector3(0, 0, -90)));

                    Destroy(gameObject);
                }

                else
                {
                    hit = Physics2D.Raycast(transform.position, Vector2.down * transform.localScale.y, distance, groundLayer);

                    if (hit.collider != null)
                    {
                        Debug.Log("Down");

                        Instantiate(exsplosionGround, this.transform.position, Quaternion.Euler(new Vector3(0, 0, 0)));

                        Destroy(gameObject);
                    }


                    else
                    {
                        hit = Physics2D.Raycast(transform.position, Vector2.up * transform.localScale.y, distance, groundLayer);

                        if (hit.collider != null)
                        {
                            Debug.Log("Up");

                            Instantiate(exsplosionGround, this.transform.position, Quaternion.Euler(new Vector3(0, 0, 180)));

                            Destroy(gameObject);
                        }
                    }
                }
            }
        }
    }
}
