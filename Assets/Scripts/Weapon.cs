using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;

using UnityEngine;

public class Weapon : Item
{

    public float attackForce = 5f;
    public float attackStartDist = .5f;
    public GameObject attackPrefab;

    public override void Use(Vector2 target)
    {
        // base.Use(target);
        if (useCooldownCur > 0)
        {
            return;
        }
        useCooldownCur = useCooldownMax;
        Vector2 direction = target - (Vector2)transform.position;
        direction = direction.normalized;
        GameObject attackObj = Instantiate(attackPrefab, transform.position + (Vector3)direction * attackStartDist, Quaternion.Euler(0, 0, 0));
        attackObj.transform.right = direction;
        transform.right = direction;
        Vector3 nRot = transform.rotation.eulerAngles;
        // if (nRot.z > 90) // needs fixing but I really don't wanna learn quaternions rn
        // {
        //     nRot.z = 0;
        //     nRot.y += 180;
        //     transform.rotation = Quaternion.Euler(nRot);
        // }
        attackObj.GetComponent<Rigidbody2D>().velocity = owner.GetComponent<Rigidbody2D>().velocity;
        attackObj.GetComponent<Rigidbody2D>().AddForce(direction * attackForce, ForceMode2D.Impulse);
        attackObj.GetComponent<Attack>().owner = owner;
        owner.GetComponent<Rigidbody2D>().AddForce((owner.transform.position - (Vector3)target).normalized * useRecoil, ForceMode2D.Impulse);
        if (depletes)
        {
            ChangeCount(-1);
        }
    }
}
