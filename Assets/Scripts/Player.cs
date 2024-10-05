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

    [Header("Friction")]
    public float frictionAmount = 0.2f;


    [Header("Ground Check")]
    public Transform groundCheckPoint;
    public Vector2 groundCheckSize = new Vector2(1, 1);
    public LayerMask groundLayer;
    public float groundedTimeBuffer = 0.2f;
    public float lastGroundedTime = 0f;
    public int groundsTouching;

    [Header("Jump")]
    public float jumpForce = 12f;
    public float jumpCutMultiplier = 1f;
    public float jumpTimeBuffer = 0.1f;
    public float lastJumpTime = 0f;
    public bool isJumping = false;
    public bool jumpInputReleased = false;

    [Header("Gravity")]
    public float gravityScale = 1f;
    public float fallGravityMultiplier = 1.2f;
    public float grip = 1.5f;
    public Vector2 centerOfGrav;
    // public float rotFix = 1f;



    [Header("Hunger")]
    public bool changingHunger = true;
    public float hunger = 100f;
    public float maxHunger = 100f;
    public float hungerChangeRate = -1f;
    public float hungerToHealth = 1f;

    [Header("Bars")]
    public RectTransform HealthBar;
    public RectTransform HungerBar;

    public bool useInputCached = false;


    [Header("Other Stuff")]

    // public Transform holdPoint;
    InputsThing pInputs;
    private Vector2 moveInputs;
    private Vector2 relVel;

    // [SerializeField]
    // // private Camera playerCamera;

    protected override void Awake()
    {
        base.Awake();

        pInputs = new InputsThing();
        pInputs.Player.Enable();
        pInputs.Player.Use.started += UseS;
        pInputs.Player.Use.canceled += UseE;
        pInputs.Player.Jump.started += JumpS;
        pInputs.Player.Jump.canceled += JumpE;
    }

    private void OnDestroy()
    {
        pInputs.Player.Use.started -= UseS;
        pInputs.Player.Use.canceled -= UseE;
    }

    protected override void Update()
    {
        base.Update();
        moveInputs = pInputs.Player.Move.ReadValue<Vector2>();
        // print(moveInputs);

        #region decrementTimers
        lastGroundedTime -= Time.deltaTime;
        lastJumpTime -= Time.deltaTime;
        #endregion


        // Use key
        // -------
        if (useInputCached)
        {

        }

        // Update Stats
        #region StatsUpdatingStuff
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
        #endregion

    }

    protected void FixedUpdate()
    {
        relVel = transform.InverseTransformDirection(rb.velocity);
        // print(rb.centerOfMass);
        rb.centerOfMass = centerOfGrav;
        // print(rb.centerOfMass);


        // Movement
        // --------
        if (canMove)
        {
            #region MovementH

            // calculate dir want to move and desired velo
            float targetSpeed = moveInputs.x * moveSpeed * moveSpeedMult;
            // change accell depending on situation(if our target target speed wants to not be 0 use decell)
            float accelRate = Mathf.Abs(targetSpeed) > .01f ? accel : deccel;
            // calc diff between current and target
            // float speedDif = targetSpeed - rb.velocity.x;
            float speedDif = targetSpeed - relVel.x;
            // applies accel to speed diff, raises to power so accel will increase with higher speeds then applies to desired dir
            float movement = Mathf.Pow(Mathf.Abs(speedDif * accelRate), velPower) * Mathf.Sign(speedDif);
            // apply force
            rb.AddForce(transform.right * movement);

            #endregion

            #region Friction
            // if grounded and trying to stop apply extra friction
            if (lastGroundedTime > 0 && Mathf.Abs(moveInputs.x) < 0.01f)
            {
                // use friction ammount
                float amt = Mathf.Min(Mathf.Abs(relVel.x), Mathf.Abs(frictionAmount));
                // sets to movement dir
                // amt *= Mathf.Sign(rb.velocity.x);
                amt *= Mathf.Sign(relVel.x);
                // apply force against movement dir
                // rb.AddForce(Vector2.right * -amt, ForceMode2D.Impulse);
                rb.AddForce(transform.right * -amt, ForceMode2D.Impulse);
            }
            #endregion

            #region FallGravity
            if (rb.velocity.y < 0)
            {
                rb.gravityScale = gravityScale * fallGravityMultiplier;
                isJumping = false;
            }
            else
            {
                rb.gravityScale = gravityScale;
            }
            #endregion

            #region GroundCheck
            if (Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, groundLayer))
            {
                lastGroundedTime = groundedTimeBuffer;
                rb.gravityScale = 0;
            }
            #endregion 

            #region Jump
            if (lastGroundedTime > 0 && lastJumpTime > 0 && !isJumping)
            {
                Jump();
            }
            else if (lastGroundedTime > 0)
            {
                rb.AddForce(transform.up * -grip);
            }
            #endregion

            // if (lastGroundedTime < 0.1f)
            // {
            //     rb.AddTorque(Mathf.Sign(transform.eulerAngles.z) * rotFix);
            // }
        }
    }

    public void JumpS(InputAction.CallbackContext context)
    {
        lastJumpTime = jumpTimeBuffer;
    }

    public void JumpE(InputAction.CallbackContext context)
    {
        if (rb.velocity.y > 0 && isJumping)
        {
            // reduce current y vel
            rb.AddForce(Vector2.down * rb.velocity.y * (1 - jumpCutMultiplier), ForceMode2D.Impulse);
        }

        jumpInputReleased = true;
        lastJumpTime = 0;
    }

    private void Jump()
    {
        // print(rb.centerOfMass);
        // apply force
        // rb.AddForce(new Vector2(1, 0) * jumpForce, ForceMode2D.Impulse);
        rb.AddForce((Vector2)transform.up * jumpForce, ForceMode2D.Impulse);
        // transform.rotation = Quaternion.Euler(0, 0, 0);
        // print((Vector2)transform.up);
        // rb.AddRelativeForce(transform.up * jumpForce, ForceMode2D.Impulse);
        lastGroundedTime = 0;
        lastJumpTime = 0;
        isJumping = true;
        jumpInputReleased = false;
    }

    private void UseS(InputAction.CallbackContext context)
    {
        useInputCached = true;
    }

    private void UseE(InputAction.CallbackContext context)
    {
        useInputCached = false;
    }

    public override void ChangeHealth(float value, bool addsIframes = true)
    {
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

    public void OnCollisionEnter2D(Collision2D other)
    {
        // print(other.gameObject.layer);
        // print(groundLayer.value);
        // if (!groundCling && (1 << other.gameObject.layer & groundLayer.value) != 0)
        if ((1 << other.gameObject.layer & groundLayer.value) != 0)
        {
            if (groundsTouching == 0)
            {
                // print(other.GetContact(0));
                print(other.GetContact(0).normal);
                // transform.LookAt(other.GetContact(0).normal, transform.up);
                transform.up = other.GetContact(0).normal;

            }
            groundsTouching++;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if ((1 << other.gameObject.layer & groundLayer.value) != 0)
        {
            groundsTouching--;
        }
    }

    public override void Die()
    {
        // SceneManager.LoadScene("OtherScene");
    }


}