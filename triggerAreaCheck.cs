using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class triggerAreaCheck : MonoBehaviour
{
    public Enemy_behavior enemyParent;
    private void Awake()
    {
        enemyParent = GetComponentInParent<Enemy_behavior>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            gameObject.SetActive(false);
            enemyParent.target = collision.transform;
            enemyParent.inRange = true;
            enemyParent.redZone.SetActive(true);


        }

    }

}
