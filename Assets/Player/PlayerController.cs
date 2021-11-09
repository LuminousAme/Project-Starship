using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    //serialized field so they can be changed
    [SerializeField] private float movementSpeed = 10f;

    [SerializeField] private float lookSpeed = 5f;

    //controls for how much the player has locally rotated
    private float yaw = 0.0f;

    private float pitch = 0.0f;
    [SerializeField] private Vector2 pitchLimits;

    //quaterion to track the current rotation
    private Quaternion targetRotation;

    //input vector
    private Vector2 input = Vector2.zero;

    //the camera transform, on Y we rotate instead of the player directly so that it handles movement correctly
    [SerializeField] private Transform cam;

    //enum to control different interaction states
    public enum interactionState
    {
        WALKING,
        INTERACTING
    }

    //variable with the current enum for this player
    private interactionState playerState;

    //gameobject the player is currently interacting with
    private GameObject interactionObject;

    //interaction cooldown
    [SerializeField] private float interactionCooldown = 0.2f;

    private float timeSinceInteraction = 0.0f;

    private Energy playerEnergy;

    //Start runs when the object first enters the scene
    private void Start()
    {
        //start the player in their default walking state
        playerState = interactionState.WALKING;
        //and confine the cursor and make it invisible
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;

        //set the rotation at it's starting rotation
        targetRotation = transform.rotation;

        //get the energy
        playerEnergy = this.GetComponent<Energy>();
    }

    // Update is called once per frame
    private void Update()
    {
        //update the interaction cooldown
        timeSinceInteraction += Time.deltaTime;

        //use a simple state machine to determine what kinda of input the player is allowed to use
        switch (playerState)
        {
            //if the player is in their regular walk mode, allow them to move normally
            case interactionState.WALKING:
                RegularMovement();
                break;
            //if the player is piloting the ship, only allow them to use piloting input
            case interactionState.INTERACTING:
                Pilot();
                break;
        }
    }

    private void FixedUpdate()
    {
        Vector3 velo = (input.y * transform.forward + input.x * transform.right).normalized * movementSpeed;
        this.GetComponent<Rigidbody>().velocity = velo;
    }

    //function for the player's regular movememnt
    private void RegularMovement()
    {
        //get rotation input
        yaw += Input.GetAxis("Mouse X") * Time.deltaTime * lookSpeed;
        pitch -= Input.GetAxis("Mouse Y") * Time.deltaTime * lookSpeed;

        //limit how far the player can rotate while looking up and down
        pitch = Mathf.Clamp(pitch, pitchLimits.x, pitchLimits.y);

        //rotate based on mouse input
        cam.transform.localEulerAngles = new Vector3(pitch, 0f, 0f);
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 0f, 0f);
        transform.Rotate(Vector3.up, yaw);

        //get input for each direction
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    //function for piloting the ship
    private void Pilot()
    {
        pitch = cam.localEulerAngles.x;

        //if the player is interacting with something, and they hit E, or if they are just out of energy stop interacting
        if (!playerEnergy.AbleTask || Input.GetKeyDown(KeyCode.E) && timeSinceInteraction >= interactionCooldown)
        {
            interactionObject.GetComponent<InteractEnter>().SetEntityInteracting(null, null);
            playerState = interactionState.WALKING;
            timeSinceInteraction = 0.0f;
            //and hide the cursor
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
        }
    }

    //function for when the player enters the interaction collider of one of the tasks like piloting
    private void OnTriggerStay(Collider other)
    {
        //if the player is in their walking state and can thus interact with something
        if (playerState == interactionState.WALKING && timeSinceInteraction >= interactionCooldown)
        {
            //if the player is within the control panel's interaction space, has enough energy and presses E to interact
            if (playerEnergy.AbleTask && other.name == "Control Panel" && Input.GetKey(KeyCode.E))
            {
                //begin piloting
                interactionObject = other.gameObject;
                interactionObject.GetComponent<InteractEnter>().SetEntityInteracting(this.transform, cam);
                playerState = interactionState.INTERACTING;
                timeSinceInteraction = 0.0f;
                //and make the cursor visible
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }

    public bool GetInteracting()
    {
        if (playerState == interactionState.WALKING)
        {
            return false;
        }
        else
            return true;
    }
}