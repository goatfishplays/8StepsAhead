using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Health")]
    public bool changingHealth = true;
    public float health = 100f;
    public float maxHealth = 100f;
    public float healthChangeRate = .25f;
    public float iFrameTime = 0f;
    public float iFrameAddTime = .5f;


    [Header("Movement Settings")]
    public bool canMove = true;
    public float moveSpeed = 15f;
    public float moveSpeedMult = 1f;
    public float accel = 1.2f;
    public float deccel = 3f;
    public float velPower = 1f;

    [Header("Components")]
    public Rigidbody2D rb;
    // [Header("Status")]




    // Start is called before the first frame update
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        iFrameTime -= Time.deltaTime;
        if (iFrameTime < 0)
        {
            iFrameTime = -1;
        }
        ChangeHealth(healthChangeRate * Time.deltaTime, false);
    }

    public void SlowDown()
    {
        Vector2 speedDiff = -rb.velocity;
        Vector2 movement = Mathf.Pow(speedDiff.magnitude * deccel, velPower) * speedDiff.normalized;
        rb.AddForce(movement);
    }

    public virtual void ChangeHealth(float value, bool addsIframes = true)
    {
        if (changingHealth)
        {
            if (value > 0 || iFrameTime < 0.01f)
            {
                health += value;
                if (health > maxHealth)
                {
                    health = maxHealth;
                }
                else if (health < 0)
                {
                    Die();
                    // health = 0;
                }
                if (addsIframes)
                {
                    iFrameTime += iFrameAddTime;
                }
            }
        }
    }

    public virtual void ApplyKnockback(Vector2 kb)
    {
        rb.AddForce(kb, ForceMode2D.Impulse);
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }

}
