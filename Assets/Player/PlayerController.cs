using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //serialized field so they can be changed
    [SerializeField] private float movementSpeed = 10f;

    [SerializeField] private float lookSpeed = 5f;

    //the transform of the parent (right now just the ship, may change if we allow the player to leave the ship in the future)
    //needed to make sure the controls work regardless of the ship's orientation
    private Transform parentSpace;

    //controls for how much the player has locally rotated
    private float yaw = 0.0f;
    private float pitch = 0.0f;
    [SerializeField] Vector2 pitchLimits;

    //the camera transform, on Y we rotate instead of the player directly so that it handles movement correctly
    [SerializeField] Transform cam;

    //enum to control different interaction states
    private enum interactionState
    {
        WALKING,
        PILOTING
    }

    //variable with the current enum for this player
    private interactionState playerState;

    //gameobject the player is currently interacting with
    private GameObject interactionObject;

    //interaction cooldown
    [SerializeField] private float interactionCooldown = 0.2f;

    private float timeSinceInteraction = 0.0f;

    //Start runs when the object first enters the scene
    private void Start()
    {
        //start the player in their default walking state
        playerState = interactionState.WALKING;
        //and confine the cursor and make it invisible
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;

        parentSpace = transform.parent.transform;
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
            case interactionState.PILOTING:
                Pilot();
                break;
        }
    }

    private void FixedUpdate()
    {

    }

    //function for the player's regular movememnt
    private void RegularMovement()
    {
        //get rotation input
        yaw += Input.GetAxis("Mouse X") * lookSpeed;
        pitch -= Input.GetAxis("Mouse Y") * lookSpeed;

        //limit how far the player can rotate while looking up and down
        pitch = Mathf.Clamp(pitch, pitchLimits.x, pitchLimits.y);

        //allign the up with the ship
        transform.rotation = Quaternion.FromToRotation(transform.up.normalized, parentSpace.up.normalized) * transform.rotation;

        //rotate based on mouse input
        cam.transform.localEulerAngles = new Vector3(pitch, 0f, 0f);
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 0f, 0f);
        transform.RotateAround(transform.position, transform.parent.up, yaw);
       
        //get input for each direction
        float dirX = Input.GetAxis("Horizontal");
        float dirZ = Input.GetAxis("Vertical");

        //use that input to move the player 
        //note: moving in local space so that the motion appears the same to the player regardless of orientation of the ship
        transform.Translate(new Vector3(dirX, 0.0f, dirZ) * movementSpeed * Time.deltaTime, Space.Self);
    }

    //function for piloting the ship
    private void Pilot()
    {
        pitch = cam.localEulerAngles.x;

        //if the player is piloting, and they hit E, stop piloting
        if (Input.GetKeyDown(KeyCode.E) && timeSinceInteraction >= interactionCooldown)
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
            //if the player is within the control panel's interaction space and presses E to interact
            if (other.name == "Control Panel" && Input.GetKey(KeyCode.E))
            {
                //begin piloting
                interactionObject = other.gameObject;
                interactionObject.GetComponent<InteractEnter>().SetEntityInteracting(this.transform, cam);
                playerState = interactionState.PILOTING;
                timeSinceInteraction = 0.0f;
                //and make the cursor visible
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}