
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmotionWeatherManager : MonoBehaviour
{
    public ParticleSystem rain;
    public Collider2D box;

    public Vector2 windDir;
    public float maxWindForce = 20f;
    public Player player;
    public float windDirChangeTime = 10f;
    public float windDirChangeTimeEraticism = 30f;

    public float lightningDist = 18f;
    public float lightningTime = 10f;
    public float maxLightningTime = 10f;
    public float lightningMinSize = .2f;
    public float lightningMinTime = .05f;
    public GameObject lightningSpawner;

    public float sadboiDist = 18f;
    public float sadboiTime = 10f;
    public float maxSadBoiTime = 10f;
    public float sadboiMinTime = .5f;
    public GameObject sadboiSpawner;
    public AudioSource windSound;
    public AudioSource RainSound;



    public RectTransform windDirUI;


    // Start is called before the first frame update
    void Start()
    {
        windDir = new Vector2(Random.value, Random.value).normalized;
        box = GetComponent<Collider2D>();
        ResetLightningTime();
        ResetSadboiTime();

    }

    // Update is called once per frame
    void Update()
    {
        windSound.volume = player.fear / player.maxFear;
        RainSound.volume = player.sadness / player.maxSadness;


        var rainEmmission = rain.emission;
        rainEmmission.rateOverTime = 500 * Mathf.Pow(player.sadness / player.maxSadness, 3); // can probs just mult sadness by 5 but using ratios because idk
        var rainVelo = rain.velocityOverLifetime;
        rainVelo.x = 150 * windDir.x * player.fear / player.maxFear;
        // wind change dir
        windDirChangeTime -= Time.deltaTime;
        if (windDirChangeTime < 0)
        {
            windDir = new Vector2(Random.value - .5f, Random.value - .5f).normalized;
            // windDirUI.rotation = Quaternion.LookRotation(windDir, Vector3.right);
            float dir = Mathf.Rad2Deg * Mathf.Atan2(windDir.y, windDir.x);
            // if (windDir.x < 0) {

            // }
            windDirUI.rotation = Quaternion.Euler(0, 0, dir);
            // print(Mathf.Atan(windDir.y / windDir.x));
            // print(Mathf.Rad2Deg * Mathf.Atan(windDir.y / windDir.x));

            windDirChangeTime = windDirChangeTimeEraticism * Random.value;
        }


        lightningTime -= Time.deltaTime;
        if (lightningTime < 0)
        {
            SummonLightning();
        }

        sadboiTime -= Time.deltaTime;
        if (sadboiTime < 0)
        {
            SummonSadboi();
        }


        // apply wind effects to player
        player.GetComponent<Rigidbody2D>().AddForce(windDir * (maxWindForce * player.fear / player.maxFear));
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        other.attachedRigidbody.AddForce(windDir * (maxWindForce * player.fear / player.maxFear));
    }

    public void ResetLightningTime()
    {
        // lightningTime = Mathf.Pow((player.maxAnger - player.anger) / player.maxAnger, 2);
        maxLightningTime = Mathf.Tan(3.14f / 2 * (player.maxAnger - player.anger) / player.maxAnger) + lightningMinTime;
        if (maxLightningTime > lightningTime)
        {
            lightningTime = maxLightningTime;
        }
    }
    public void ResetSadboiTime()
    {
        // sadboiTime = Mathf.Pow((player.maxAnger - player.anger) / player.maxAnger, 2);
        maxSadBoiTime = Mathf.Tan(3.14f / 2 * Mathf.Pow((player.maxSadness - player.sadness) / player.maxSadness, .25f)) + sadboiMinTime;
        if (maxSadBoiTime < sadboiTime)
        {
            sadboiTime = maxSadBoiTime;
        }
    }

    public void SummonLightning()
    {
        ResetLightningTime();
        GameObject lightning = Instantiate(lightningSpawner, new Vector2(Random.value * lightningDist - lightningDist / 2 + transform.position.x, Random.value * lightningDist - lightningDist / 2 + transform.position.y), transform.rotation);
        lightning.transform.localScale = lightning.transform.localScale * (Random.value * (1 - lightningMinSize) + lightningMinSize);
        lightningTime = maxLightningTime;
    }
    public void SummonSadboi()
    {
        ResetSadboiTime();
        GameObject sadboi = Instantiate(sadboiSpawner, new Vector2(Random.value * sadboiDist - sadboiDist / 2 + transform.position.x, Random.value * sadboiDist - sadboiDist / 2 + transform.position.y), transform.rotation);
        // sadboi.transform.localScale = sadboi.transform.localScale * (Random.value * (1 - sadboiMinSize) + sadboiMinSize);
        sadboiTime = maxSadBoiTime;
    }
}
