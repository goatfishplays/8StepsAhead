using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public Collider2D hitbox;
    public Rigidbody2D rb;
    public float liveTime = .5f;
    public float damage = 10f;
    public float kb = 50f;
    public bool lives = true;
    public bool breaksOnWall = false;
    public bool breaksOnHit = true;
    public bool ownerImmune = true;
    public GameObject owner;

    // public LayerMask targetLayer;
    // Start is called before the first frame update
    protected virtual void Awake()
    {
        hitbox = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (lives)
        {
            liveTime -= Time.deltaTime;
            if (liveTime < 0)
            {
                Break();
            }

        }
    }

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("World") && breaksOnWall)
        {
            Break();
        }
        else
        {
            if (owner != null && other.gameObject.layer == owner.gameObject.layer && ownerImmune)// don't hit owner/other teammates
            {
                return;
            }
            // print(other.gameObject.name);

            Entity otherEntity = other.gameObject.GetComponent<Entity>();
            otherEntity.ApplyKnockback((other.transform.position - transform.position).normalized * kb);
            otherEntity.ChangeHealth(-damage);
            if (breaksOnHit)
            {
                Break();
            }

        }
    }

    public virtual void Break()
    {
        Destroy(gameObject);
    }
}
