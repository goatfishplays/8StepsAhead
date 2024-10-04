using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeCounter : MonoBehaviour
{
    public TMP_Text text;
    public float curScore = 0f;
    public static float maxScore = 0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        curScore += Time.deltaTime;
        if (curScore > maxScore)
        {
            maxScore = curScore;
        }
        text.text = "Time: " + curScore.ToString("#.") + "\nBest Score: " + maxScore.ToString("#.");
    }
}
