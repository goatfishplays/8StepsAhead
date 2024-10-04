using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedSpawner : MonoBehaviour
{
    public GameObject thingToSpawn;
    public float time = 1f;
    public float maxTime = 1f;
    public float pmPercentTimeRandomize = .05f;
    public Vector2 offset;
    public bool destroyOnCreate = true;

    // Update is called once per frame
    void Update()
    {
        time -= Time.deltaTime;
        if (time < 0)
        {
            GameObject spawned = Instantiate(thingToSpawn, offset * transform.localScale.x + (Vector2)transform.position, transform.rotation);
            spawned.transform.localScale = transform.localScale;
            if (destroyOnCreate)
            {
                Destroy(gameObject);
                return;
            }
            time = maxTime;
            time *= (Random.value - .5f) * pmPercentTimeRandomize;
        }
    }
}
