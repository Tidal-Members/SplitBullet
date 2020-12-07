using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DefaultExecutionOrder(-100)]
public class Player : Entity
{
    public enum PlayerState
    {
        Idle,
        Falling,
        Walk,
        AirWalk,
        Jumping
    }
    public enum PlayerDirection
    {
        RightForward,
        RightUp,
        RightDown,
        LeftForward,
        LeftUp,
        LeftDown
    }
    public enum Personality
    {
        Uno,
        Dos
    }
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float playerSpeed = 2.0f;
    private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;
    public float zPosition = 0f;
    public bool noClip = false;
    public bool flying = false;
    public bool cooldown = false;
    private Vector3 moveDirection = Vector3.zero;
    private CharacterController characterController;
    public Look lookScript;
    public SpriteRenderer playerSprite;
    public SharedEnums.HoldType holdState = SharedEnums.HoldType.None;
    public PlayerDirection direction = PlayerDirection.LeftForward;
    public PlayerState state = PlayerState.Idle;
    public Personality personality = Personality.Uno;
    void Awake()
    {
        characterController = transform.GetComponent<CharacterController>();
        lookScript = transform.Find("Look").GetComponent<Look>();
        playerSprite = transform.GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        CommandBackend.AddConCommand("noclip",(string[] args) =>
        {
            noClip = !noClip;
            flying = noClip;
            CommandBackend.IncreaseOutputSize(CommandBackend.line);
            return "NoClip = "+noClip+"\nFlying = "+flying;
        }, "Toggle player collisions and flying.",CommandBackend.CommandType.Cheat);
        CommandBackend.AddConCommand("fly",(string[] args) =>
        {
            flying = !flying;
            return "Flying = "+flying;
        }, "Toggle player flying.",CommandBackend.CommandType.Cheat);
        CommandBackend.AddConCommand("speed",(string[] args) =>
        {
            if(args == null || args.Length == 0)
            {
                CommandBackend.IncreaseOutputSize(CommandBackend.line);
                return "Player speed is set to "+playerSpeed+".\nUsage: speed [float]";
            }
            else
            {
                playerSpeed = float.Parse(args[0]);
                return "Desired speed has been set.";
            }
        }, "Sets the player speed.",CommandBackend.CommandType.Cheat);
        CommandBackend.AddConCommand("jumpheight",(string[] args) =>
        {
            if(args == null || args.Length == 0)
            {
                CommandBackend.IncreaseOutputSize(CommandBackend.line);
                return "Player jump height is set to "+jumpHeight+".\nUsage: jumpheight [float]";
            }
            else
            {
                jumpHeight = float.Parse(args[0]);
                return "Desired jump height has been set.";
            }
        }, "Sets the player jump height.",CommandBackend.CommandType.Cheat);
    }
    // Update is called once per frame
    void Update()
    {
        if(CommandBackend.currentlyActive)
            return;
        if(Input.GetButtonDown("Pause"))
            CommandBackend.HandleConCommand("quit");
        if(Input.GetButton("Fire1") && !cooldown) {
            lookScript.activeWeapon.Fire();
            StartCoroutine(WeaponCooldown());
        }
        // If player is moving up, ignore collisions between player and platforms
        if (characterController.velocity.y > 0)
        {
            Physics.IgnoreLayerCollision(9, 10, true);
        }
        //else the collision will not be ignored
        else
        {
            Physics.IgnoreLayerCollision(9, 10, false);
        }
        //personality
        switch(personality)
        {
            default: //assume personality is first one by default
                playerSprite.color = Color.green;
                break;
            case Personality.Dos:
                playerSprite.color = Color.magenta;
                break;
        }
        //If we hit something above us AND we are moving up, reverse vertical movement
        if ((characterController.collisionFlags & CollisionFlags.Above) != 0)
        {
            if (playerVelocity.y > 0)
            {
                playerVelocity.y = 0;
            }
        }
        //player states
        if(Input.GetAxis("Horizontal") != 0 && playerVelocity.y >= -0.5f && playerVelocity.y < -0f)
            state = PlayerState.Walk;
        else if(!groundedPlayer && playerVelocity.y > 0.5f)
            state = PlayerState.Jumping;
        else if(Input.GetAxis("Horizontal") != 0 && playerVelocity.y <= -0.5f && !groundedPlayer)
            state = PlayerState.AirWalk;
        else if(Input.GetAxis("Horizontal") == 0 && playerVelocity.y <= -0.5f && !groundedPlayer)
            state = PlayerState.Falling;
        else if(playerVelocity.y >= -0.5f && playerVelocity.y < -0f) state = PlayerState.Idle;
        //movement
        if (health > 0 && !flying)
        {
            groundedPlayer = characterController.isGrounded;
            if (groundedPlayer)
            {
                playerVelocity.y = 0f;
            }

            Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
            characterController.Move(move * Time.deltaTime * playerSpeed);
            /*if (transform.position.z != zPosition)
            {
                //enter the tidal zone-style smooth z positioning v
                move.z = (zPosition - transform.position.z);
            }*/
            if (Input.GetButton("Jump") && groundedPlayer)
            {
                playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            }
            playerVelocity.y += gravityValue * Time.deltaTime;
            characterController.Move(playerVelocity * Time.deltaTime);
            transform.position = new Vector3(transform.position.x,transform.position.y,0f);
        }
        else if(flying)
        {
            Vector3 movingTo = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f);
            if(noClip)
                transform.position = new Vector3(transform.position.x+Input.GetAxis("Horizontal"),transform.position.y+Input.GetAxis("Vertical"));
            else
                characterController.Move(movingTo * Time.deltaTime * playerSpeed);
        }
        //mouse look vvv
        //Get the Screen positions of the object
        Vector2 positionOnScreen = Camera.main.WorldToViewportPoint (lookScript.transform.position);

        //Get the Screen position of the mouse
        Vector2 mouseOnScreen = (Vector2)Camera.main.ScreenToViewportPoint(Input.mousePosition);

        //Get the angle between the points
        float angle = AngleBetweenTwoPoints(positionOnScreen, mouseOnScreen);
        //this locked the position when you look down so you can't do a shot under yourself, got annoying v
        /*if(angle > 25f && angle < 90f)
            angle = 25f;
        else if(angle < 150f && angle > 90f)
            angle = 150f;*/
        //anim
        if(angle > 10f && angle < 90f)
            direction = PlayerDirection.LeftDown;
        else if(angle >= -15f && angle < 10f)
            direction = PlayerDirection.LeftForward;
        else if(angle > -90f && angle < -15f)
            direction = PlayerDirection.LeftUp;
        else if(angle >= -150f && angle < -90f)
            direction = PlayerDirection.RightUp;
        else if(angle > -180f && angle < -150f)
            direction = PlayerDirection.RightForward;
        else if(angle < 180f && angle > 90f)
            direction = PlayerDirection.RightDown;
        //Ta Daaa
        lookScript.transform.rotation =  Quaternion.Euler(new Vector3(0f,0f,angle));

        switch(direction)
        {
            case PlayerDirection.RightForward:
                lookScript.weaponSprite.flipY = true;
                break;
            case PlayerDirection.RightUp:
                lookScript.weaponSprite.flipY = true;
                break;
            case PlayerDirection.RightDown:
                lookScript.weaponSprite.flipY = true;
                break;
            default:
                lookScript.weaponSprite.flipY = false;
                break;
        }
    }
    float AngleBetweenTwoPoints(Vector3 a, Vector3 b) {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }
    public IEnumerator WeaponCooldown()
    {
        cooldown = true;
        yield return new WaitForSeconds(lookScript.activeWeapon.weaponCooldown);
        cooldown = false;
        yield return null;
    }
}
