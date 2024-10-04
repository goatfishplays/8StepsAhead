using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public TMP_Text endless_best;


    // Start is called before the first frame update
    void Start()
    {
        endless_best.text = "Best Endless Score: " + TimeCounter.maxScore.ToString("#.");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void loadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
