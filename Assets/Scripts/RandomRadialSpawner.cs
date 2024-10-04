using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRadialSpawner : MonoBehaviour
{

    public GameObject[] spawns;
    public float[] times;
    public float[] maxTimes;
    public float percentVariance;
    public float minDist = 16f;
    public float maxDist = 18f;
    public float mTimeDecPerc = .05f;


    // // Start is called before the first frame update
    // void Start()
    // {

    // }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < spawns.Length; i++)
        {
            times[i] -= Time.deltaTime;
            if (times[i] < 0)
            {
                times[i] = maxTimes[i] + maxTimes[i] * (2 * (Random.value - .5f) * percentVariance);
                float distDel = maxDist - minDist;
                float r1 = minDist + Random.value * distDel;
                // float r2 = minDist + Random.value * distDel;
                // if (Random.value < .5f)
                // {
                //     r1 *= -1;
                // }
                // if (Random.value < .5f)
                // {
                //     r2 *= -1;
                // }
                Instantiate(spawns[i], transform.position + new Vector3(Random.value - .5f, Random.value - .5f, 0).normalized * r1, transform.rotation);
                maxTimes[i] *= 1 - mTimeDecPerc;
            }
        }
    }
}
