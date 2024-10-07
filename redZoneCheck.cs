using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class redZoneCheck : MonoBehaviour
{
    //declare
    private Enemy_behavior enemyParent;
    private bool inRange;



    private void Awake()
    {
        enemyParent = GetComponentInParent<Enemy_behavior>();

    }

    private void Update()
    {
        if (inRange)
        {
            enemyParent.Filp();
        }
    }

    //checkcollision then true inrange
    private void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            inRange = true;
        }
    }
    // check exit collision- set both inrange false,deactivate gameobject, select target, reset trigger zone
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            inRange = false;

            gameObject.SetActive(false);

            enemyParent.triggerArea.SetActive(true);

            enemyParent.inRange = false;

            enemyParent.SelectTarget();

        }
    }



}
