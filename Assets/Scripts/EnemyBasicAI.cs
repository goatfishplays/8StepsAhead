using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBasicAI : Entity
{
    [Header("Enemy")]
    public bool pursuing = false;
    public float startPursRange = 30f;
    public float willAttackRange = 10f;
    public float stopRange = 2f;
    public Item item;

    public static Transform playerTransform;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        item.owner = gameObject;
        item.depletes = false;
        pursuing = false;
        // rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (playerTransform.position.y + startPursRange > transform.position.y)
        {
            pursuing = true;
        }
        base.Update();
        // Gather Input

        // inputs = pInputs.Player.Move.ReadValue<Vector2>();
        // print(pInputs.Player.Look.ReadValue<Vector2>());


        // Movement and actions

        if (canMove)
        {
            // move towards player
            if (pursuing)
            {

                // Face Player
                Vector2 playerDelta = playerTransform.position - transform.position;
                if (playerDelta.x < 0)
                {
                    transform.rotation = Quaternion.Euler(Vector3.up * 180);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(Vector3.zero);
                }



                // if need to get closer
                if (playerDelta.magnitude > stopRange)
                {
                    // calculate dir want to move and desired velo
                    Vector2 targetSpeed = playerDelta.normalized * moveSpeed * moveSpeedMult;
                    // change accell depending on situation(if our target target speed wants to not be 0 use decell)
                    float accelRate = targetSpeed.magnitude > .01f ? accel : deccel;
                    // calc diff between current and target
                    Vector2 speedDif = targetSpeed - rb.velocity;
                    // applies accel to speed diff, raises to power so accel will increase with higher speeds then applies to desired dir
                    Vector2 movement = Mathf.Pow(speedDif.magnitude * accelRate, velPower) * speedDif.normalized;
                    // apply force
                    rb.AddForce(movement);

                }
                else
                {
                    SlowDown();
                }

                if (playerDelta.magnitude <= willAttackRange)
                {
                    item.Use(playerTransform.position);
                    // Attack(playerDelta);
                    // attackCooldownCur = attackCooldownMax;
                }
            }
            else
            {
                SlowDown();
            }

        }


    }




    // public virtual void Attack(Vector2 direction)
    // {
    //     direction = direction.normalized;
    //     GameObject attackObj = Instantiate(attackPrefab, transform.position + (Vector3)direction * attackStartDist, Quaternion.Euler(0, 0, 0));
    //     attackObj.transform.right = direction;
    //     attackObj.GetComponent<Rigidbody2D>().velocity = rb.velocity;
    //     attackObj.GetComponent<Rigidbody2D>().AddForce(direction * attackForce, ForceMode2D.Impulse);
    // }
}
