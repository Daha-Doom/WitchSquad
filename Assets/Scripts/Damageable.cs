using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    [SerializeField]
    GameObject Menu;

    [SerializeField]
    Animator animator;

    [SerializeField]
    GameObject healthBar;
    [SerializeField]
    BarIcon icon;

    [SerializeField]
    private bool isInvincible = false;

    private float timeSinceHit = 0f;
    public float invincibleTime = 0.25f;

    [SerializeField]
    private int _maxHealth = 100;
    public int MaxHealth
    {
        get
        {
            return _maxHealth;
        }
        set
        {
            _maxHealth = value;
        }
    }

    [SerializeField]
    private int _health;
    public int Health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = value;

            if (_health <= 0)
                IsAlive = false;
        }
    }

    private bool _isAlive;
    public bool IsAlive
    {
        get
        {
            return _isAlive;
        }
        set
        {
            _isAlive = value;
            animator.SetBool("IsAlive", value);
        }
    }

    private void Awake()
    {
        Health = MaxHealth;
        IsAlive = true;
    }

    public void Hit(int damage)
    {
        if (IsAlive && !isInvincible)
        {
            Health -= damage;
            isInvincible = true;

            if (healthBar != null)
                switch (Health)
                {
                    case 0:
                        healthBar.GetComponent<SpriteRenderer>().sprite = icon.sprites[0];
                        Menu.SetActive(true);
                        break;

                    case 10:
                        healthBar.GetComponent<SpriteRenderer>().sprite = icon.sprites[1];
                        break;

                    case 20:
                        healthBar.GetComponent<SpriteRenderer>().sprite = icon.sprites[2];
                        break;

                    case 30:
                        healthBar.GetComponent<SpriteRenderer>().sprite = icon.sprites[3];
                        break;

                    case 40:
                        healthBar.GetComponent<SpriteRenderer>().sprite = icon.sprites[4];
                        break;

                    case 50:
                        healthBar.GetComponent<SpriteRenderer>().sprite = icon.sprites[5];
                        break;

                    case 60:
                        healthBar.GetComponent<SpriteRenderer>().sprite = icon.sprites[6];
                        break;

                    case 70:
                        healthBar.GetComponent<SpriteRenderer>().sprite = icon.sprites[7];
                        break;

                    case 80:
                        healthBar.GetComponent<SpriteRenderer>().sprite = icon.sprites[8];
                        break;

                    case 90:
                        healthBar.GetComponent<SpriteRenderer>().sprite = icon.sprites[9];
                        break;

                    case 100:
                        healthBar.GetComponent<SpriteRenderer>().sprite = icon.sprites[10];
                        break;
                }

            animator.SetTrigger("Hited");
        }
    }

    private void FixedUpdate()
    {
        if (isInvincible)
        {
            if (timeSinceHit > invincibleTime)
            {
                isInvincible = false;
                timeSinceHit = 0f;
            }
            else timeSinceHit += Time.deltaTime;
        }
    }
}
