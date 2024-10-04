using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Xml.Serialization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : Entity
{
    [Header("Movement+")]
    public float sprintMult = 2f;
    public bool isSprinting = false;

    [Header("Hunger")]
    public bool changingHunger = true;
    public float hunger = 100f;
    public float maxHunger = 100f;
    public float hungerChangeRate = -1f;
    public float hungerToHealth = 1f;

    [Header("Temperature")]
    public bool ChangingTemperature = true;
    public float temperature = 100f;
    public float maxTemperature = 100f;
    public float temperatureChangeRate = 1f;

    [Header("Anger")]
    public bool ChangingAnger = true;
    public float anger = 0;
    public float maxAnger = 100f;
    public float angerChangeRate = -.5f;

    [Header("Sadness")]
    public bool ChangingSadness = true;
    public float sadness = 0;
    public float maxSadness = 100f;
    public float sadnessChangeRate = -.5f;

    [Header("Fear")]
    public bool ChangingFear = true;
    public float fear = 0;
    public float maxFear = 100f;
    public float fearChangeRate = -2f;

    [Header("Bars")]
    public RectTransform HealthBar;
    public RectTransform HungerBar;
    public RectTransform TemperatureBar;
    public RectTransform AngerBar;
    public RectTransform SadnessBar;
    public RectTransform FearBar;

    [Header("Inventory")]
    public RectTransform SelectedItemKarat;
    public TMP_Text SelectedItemCount;
    public int selectedItem = 0;
    public float dropWaitTime = 1f;
    public Image[] ItemsHotbar;
    public Sprite emptyItem;
    public Item[] Inventory = { null, null, null, null, null, null };
    public bool useInputCached = false;


    [Header("Other Stuff")]

    // Rigidbody2D rb;
    // Vector2 inputs = new Vector2(0, 0);
    public EmotionWeatherManager emoWeather;
    public Transform holdPoint;
    PlayerInputActionsThing pInputs;

    [SerializeField]
    // private Camera playerCamera;

    protected override void Awake()
    {
        base.Awake();

        // replace later if we figure out a different way to do enemies
        EnemyBasicAI.playerTransform = this.transform;
        // Inventory = { null, null};



        // rb = GetComponent<Rigidbody2D>();
        // playerCamera = Camera.main;
        // playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y, playerCamera.transform.position.z);
        // playerCamera.transform.SetParent(transform);
        pInputs = new PlayerInputActionsThing();
        pInputs.Player.Enable();
        pInputs.Player.Sprint.performed += ToggleSprint;
        pInputs.Player.Scroll.performed += Scroll;
        pInputs.Player.Use.started += UseS;
        pInputs.Player.Use.canceled += UseE;
        pInputs.Player.Drop.performed += Drop;
    }

    private void OnDestroy()
    {

        pInputs.Player.Sprint.performed -= ToggleSprint;
        pInputs.Player.Scroll.performed -= Scroll;
        pInputs.Player.Use.started -= UseS;
        pInputs.Player.Use.canceled -= UseE;
        pInputs.Player.Drop.performed -= Drop;
    }

    protected override void Update()
    {
        base.Update();
        // Gather Input

        // inputs = pInputs.Player.Move.ReadValue<Vector2>();
        // print((Vector2)Camera.main.ScreenToWorldPoint(pInputs.Player.Look.ReadValue<Vector2>()));


        // Use Item

        if (useInputCached)
        {
            if (Inventory[selectedItem] != null)
            {
                Inventory[selectedItem].Use((Vector2)Camera.main.ScreenToWorldPoint(pInputs.Player.Look.ReadValue<Vector2>()));
                if (Inventory[selectedItem].count == 0)
                {
                    EmptySlot(selectedItem);
                }
                else
                {
                    SelectedItemCount.text = Inventory[selectedItem].count.ToString();
                }
                // print(Inventory[selectedItem].gameObject);
            }
        }



        // Movement

        if (canMove)
        {
            // calc moveSpeedMult
            moveSpeedMult = 1f;
            moveSpeedMult = moveSpeedMult * .7f + .3f * temperature / maxTemperature;
            if (isSprinting)
            {
                moveSpeedMult *= sprintMult;
            }

            // calculate dir want to move and desired velo
            Vector2 targetSpeed = pInputs.Player.Move.ReadValue<Vector2>() * moveSpeed * moveSpeedMult;
            // change accell depending on situation(if our target target speed wants to not be 0 use decell)
            float accelRate = targetSpeed.magnitude > .01f ? accel : deccel;
            // calc diff between current and target
            Vector2 speedDif = targetSpeed - rb.velocity;
            // applies accel to speed diff, raises to power so accel will increase with higher speeds then applies to desired dir
            Vector2 movement = Mathf.Pow(speedDif.magnitude * accelRate, velPower) * speedDif.normalized;
            // apply force
            rb.AddForce(movement);

        }



        // Update Stats

        // Stats based on Stats

        // fear -> fear
        if (health / maxHealth > .7f) // passive decrease above 70 health
        {
            ChangeFear(Time.deltaTime * fearChangeRate);
        }
        else if (health / maxHealth < .5f) // passive increase below 50 health based on health
        {
            ChangeFear(Time.deltaTime * -fearChangeRate / (health / maxHealth));
        }

        // hunger -> health
        ChangeHunger(Time.deltaTime * hungerChangeRate);
        if (hunger > 50) // pasive health increase 
        {
            ChangeHealth(hungerToHealth * Time.deltaTime, false);
        }
        else if (hunger < 15)
        {
            ChangeHealth(-hungerToHealth * Time.deltaTime, false);
        }

        // if sad(it will be raining) and afraid(will be windy) change temp accordingly
        ChangeTemperature(Time.deltaTime * -1.5f * (sadness / maxSadness + fear / maxFear));
        ChangeTemperature(Time.deltaTime * temperatureChangeRate);
        ChangeAnger(Time.deltaTime * angerChangeRate);
        ChangeSadness(Time.deltaTime * sadnessChangeRate);

    }

    // private void Use(InputAction.CallbackContext context)
    // {
    //     if (Inventory[selectedItem] != null)
    //     {
    //         Inventory[selectedItem].Use((Vector2)Camera.main.ScreenToWorldPoint(pInputs.Player.Look.ReadValue<Vector2>()));
    //     }
    // }
    private void EmptySlot(int i)
    {
        ItemsHotbar[i].sprite = emptyItem;
        Inventory[i] = null;
        SelectedItemCount.text = "0";
    }
    private void Drop(InputAction.CallbackContext context)
    {
        if (Inventory[selectedItem] != null)
        {
            Inventory[selectedItem].transform.SetParent(null);
            Inventory[selectedItem].owner = null;
            Inventory[selectedItem].dropWaitTime = dropWaitTime;
            EmptySlot(selectedItem);
        }
    }

    private void UseS(InputAction.CallbackContext context)
    {
        useInputCached = true;
    }

    private void UseE(InputAction.CallbackContext context)
    {
        useInputCached = false;
    }



    private void Scroll(InputAction.CallbackContext context)
    {
        // print(context.ReadValue<float>());
        if (Inventory[selectedItem] != null)
        {
            Inventory[selectedItem].GetComponent<SpriteRenderer>().enabled = false;
        }
        if (context.ReadValue<float>() > 0)
        {
            selectedItem++;
            if (selectedItem >= ItemsHotbar.Length)
            {
                selectedItem = 0;
            }
        }
        else
        {
            selectedItem--;
            if (selectedItem < 0)
            {
                selectedItem = ItemsHotbar.Length - 1;
            }

        }
        SelectedItemKarat.localPosition = new Vector2(-42 + 17 * selectedItem, 12);
        if (Inventory[selectedItem] != null)
        {
            Inventory[selectedItem].GetComponent<SpriteRenderer>().enabled = true;
            SelectedItemCount.text = Inventory[selectedItem].count.ToString();
        }
        else
        {
            SelectedItemCount.text = "0";
        }
    }

    public void AttemptPickup(Item item)
    {
        int placeLoc = -1;
        for (int i = 0; i < Inventory.Length; i++)
        {
            if (Inventory[i] == null)
            {
                if (placeLoc == -1)
                {
                    placeLoc = i;
                }
            }
            else if (Inventory[i].itemId == item.itemId && item.stacks && item.maxStack > Inventory[i].count)
            {
                int temp = Mathf.Min(item.maxStack - Inventory[i].count, item.count);
                Inventory[i].count += temp;
                item.ChangeCount(-temp);
                if (selectedItem == i)
                {
                    SelectedItemCount.text = Inventory[selectedItem].count.ToString();
                }
                // Destroy(item.gameObject);
                return;
            }
        }
        if (placeLoc != -1)
        {
            Inventory[placeLoc] = item;
            item.owner = gameObject;
            ItemsHotbar[placeLoc].sprite = item.GetComponent<SpriteRenderer>().sprite;
            item.transform.parent = transform;
            item.transform.position = holdPoint.position;
            if (placeLoc != selectedItem)
            {
                item.GetComponent<SpriteRenderer>().enabled = false;
            }
            else
            {
                SelectedItemCount.text = Inventory[selectedItem].count.ToString();
            }
        }
    }

    private void ToggleSprint(InputAction.CallbackContext context)
    {
        isSprinting = !isSprinting;
    }

    public override void ChangeHealth(float value, bool addsIframes = true)
    {
        if (value < -5.5f && iFrameTime <= 0)
        {
            // anger -= value;
            ChangeAnger(-.5f * value);
            // fear -= value * .7f;
            ChangeFear(-.3f * value);
            // sadness -= value * .5f;
            ChangeSadness(-.1f * value);
            emoWeather.lightningTime = 0;
        }
        base.ChangeHealth(value, addsIframes);
        HealthBar.localScale = new Vector3(1, 1 - health / maxHealth, 1);
    }

    public void ChangeHunger(float value)
    {
        if (changingHunger)
        {
            hunger += value;
            if (hunger > maxHunger)
            {
                hunger = maxHunger;
            }
            else if (hunger < 0)
            {
                hunger = 0;
            }
        }
        HungerBar.localScale = new Vector3(1, 1 - hunger / maxHunger, 1);
    }
    public void ChangeTemperature(float value)
    {
        if (ChangingTemperature)
        {
            temperature += value;
            if (temperature > maxTemperature)
            {
                temperature = maxTemperature;
            }
            else if (temperature < 0)
            {
                temperature = 0;
            }
        }
        TemperatureBar.localScale = new Vector3(1, 1 - temperature / maxTemperature, 1);
    }
    public void ChangeAnger(float value)
    {
        if (ChangingAnger)
        {
            if (value < 0 && hunger < 25) // if hungry multiply emotion values
            {
                anger *= 1.5f;
            }
            anger += value;
            if (anger > maxAnger)
            {
                anger = maxAnger;
            }
            else if (anger < 0)
            {
                anger = 0;
            }
        }
        AngerBar.localScale = new Vector3(1, 1 - anger / maxAnger, 1);
    }
    public void ChangeSadness(float value)
    {
        if (hunger > 50 && value > 0) // if hungry multiply emotion values
        {
            value *= 1.5f;
        }
        if (ChangingSadness)
        {
            sadness += value;
            if (sadness > maxSadness)
            {
                sadness = maxSadness;
            }
            else if (sadness < 0)
            {
                sadness = 0;
            }
        }
        SadnessBar.localScale = new Vector3(1, 1 - sadness / maxSadness, 1);
        emoWeather.ResetSadboiTime();
    }
    public void ChangeFear(float value)
    {
        if (ChangingFear)
        {
            fear += value;
            if (fear > maxFear)
            {
                fear = maxFear;
            }
            else if (fear < 0)
            {
                fear = 0;
            }
        }
        FearBar.localScale = new Vector3(1, 1 - fear / maxFear, 1);
    }

    public override void Die()
    {
        SceneManager.LoadScene("OtherScene");
    }


}