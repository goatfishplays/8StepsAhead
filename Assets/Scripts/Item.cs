using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum itemID
{
    sword,
    stick,
    rock,
    chips,
    loli,
    pills,
    bat,
    hotdog,
    pipe,
    handWarmer,

}

public class Item : MonoBehaviour
{

    [Header("Stuff")]

    public float maxDespawnTime = 30f;
    public float despawnTime = 30f;
    public GameObject owner;
    public Collider2D pickupBox;
    public float dropWaitTime = 0f;
    public float useCooldownMax = 2f;
    public float useCooldownCur = 0f;
    public float useRecoil = 0f;
    public bool stacks = true;
    public int count = 1;
    public int maxStack = 1;
    public bool depletes = false;
    // public int itemId = 0;
    public itemID itemId;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        pickupBox = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (owner != null)
        {
            pickupBox.enabled = false;
            despawnTime = maxDespawnTime;
        }
        else if (dropWaitTime > 0)
        {
            dropWaitTime -= Time.deltaTime;
        }
        else if (!pickupBox.enabled)
        {
            pickupBox.enabled = true;
        }

        if (owner == null)
        {
            despawnTime -= Time.deltaTime;
            if (despawnTime < 0)
            {
                Destroy(gameObject);
            }
        }

        if (useCooldownCur > 0f)
        {
            useCooldownCur -= Time.deltaTime;
        }
    }

    public virtual void Use(Vector2 target)
    {
        if (useCooldownCur > 0)
        {
            return;
        }
        useCooldownCur = useCooldownMax;
        owner.GetComponent<Rigidbody2D>().AddForce((owner.transform.position - (Vector3)target).normalized * useRecoil, ForceMode2D.Impulse);
        if (depletes)
        {
            // count -= 1;
            // if (count == 0)
            // {
            //     Destroy(gameObject);
            // }
            ChangeCount(-1);
        }
    }

    public void ChangeCount(int delta)
    {
        count += delta;
        if (count <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (owner != null)
        {
            return;
        }
        // print(other.name);
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            other.GetComponent<Player>().AttemptPickup(this);
        }
    }

}
