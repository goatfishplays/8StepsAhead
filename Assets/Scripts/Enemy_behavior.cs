using System.Collections;
using System.Collections.Generic;
using System.Timers;
using Unity.VisualScripting;
// using UnityEditor.Tilemaps;
using UnityEngine;



public class Enemy_behavior : Entity
{

    #region
    // public Transform rayCast;
    //public LayerMask raycastLayer;
    // public float moveSpeed;
    //public float raycastLength;
    public float attackDistance;
    public float timer; //time cooldown
    public Transform leftLimit;
    public Transform rightLimit;
    public GameObject redZone;
    public GameObject triggerArea;
    // public Transform GroundCheckPoint;
    // public Vector2 groundChecksize;
    public LayerMask groundLayer;
    public Animator animator;

    public GameObject attack;
    public Transform attackPos;



    [HideInInspector] public Transform target;
    public bool inRange = false;
    private float distance;
    [HideInInspector] public bool attackMode;
    private float intTimer;
    private bool cooling;
    #endregion 



    protected override void Awake()
    {
        base.Awake();
        intTimer = timer;
        SelectTarget();
    }
    private void Start()
    {

        // Debug.Log("This runs only once at the start of the object.");
        // groundChecksize = new Vector2(0.5f, 0.1f);
    }
    /*private void OnTriggerEnter2D(Collider2D trig)
    {
        Debug.Log("Something entered the trigger");
        if (trig.gameObject.tag=="Player")
        {
            target = trig.transform;
            inRange = true;
            Filp();
            Debug.Log("hit");
        }

    }*/

    protected override void Update()
    {
        base.Update();
        if (attackMode == false)
        {
            move();
        }
        // if (!(Physics2D.OverlapBox(GroundCheckPoint.position, groundChecksize, 0f, groundLayer)))
        // {
        //     Filp();
        // }



        if (!inRange && !InsideLimit())
        {
            SelectTarget();

        }
        /*if (inRange)
        {
            hit = Physics2D.Raycast(rayCast.position, transform.right, raycastLength, raycastLayer);
            raycastVisualize();
        }*/
        //when it found the player
        /*if (hit.collider != null)
        {
            EnemyLogic();
        }
        else if (hit.collider == null)
        {
            inRange = false;

        }*/
        timer -= Time.deltaTime;
        // if (timer < 0)
        // {

        //     animator.SetBool("Attacking", false);
        // }
        if (inRange)
        {
            EnemyLogic();
        }

    }
    private void EnemyLogic() //attack
    {
        distance = Vector2.Distance(transform.position, target.position);//transform position is where the enemy is standing

        if (distance > attackDistance)
        {

            stopAttack();
        }
        else if (distance < attackDistance && timer <= 0)
        {
            Attack();

        }
    }

    private void Attack()
    {
        attackMode = true;
        timer = intTimer;
        animator.SetBool("Attacking", true);
        Instantiate(attack, attackPos);

    }
    private void stopAttack()
    {
        attackMode = false;
        animator.SetBool("Attacking", false);


    }
    private bool InsideLimit() // check if the enemy inside the target range
    {
        if (transform.position.x > leftLimit.position.x && transform.position.x < rightLimit.position.x)
        {
            return true;
        }
        else { return false; }

    }
    public void SelectTarget()// change destination whenever it reach the limit
    {
        if (leftLimit != null && rightLimit != null)
        {
            float leftdistance = Vector2.Distance(transform.position, leftLimit.position);

            float rightdistance = Vector2.Distance(transform.position, rightLimit.position);

            if (leftdistance > rightdistance)
            {
                target = leftLimit;
            }
            else if (rightdistance > leftdistance)
            {
                target = rightLimit;
            }
            Filp();

        }


    }



    void move()
    {
        Vector2 target_position = new Vector2(target.position.x, transform.position.y);

        transform.position = Vector2.MoveTowards(transform.position, target_position, moveSpeed * Time.deltaTime);
    }
    public void Filp()
    {
        Vector3 rotation = transform.eulerAngles;
        if (transform.position.x > target.position.x)
        {
            rotation.y = 180f;
        }

        else
        {
            rotation.y = 0f;
        }
        transform.eulerAngles = rotation;
    }



}
