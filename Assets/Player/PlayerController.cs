using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //serialized field so they can be changed
    [SerializeField] private float movementSpeed = 10f;

    [SerializeField] private float lookSpeed = 5f;

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

    //function for the player's regular movememnt
    private void RegularMovement()
    {
        float dirX = Input.GetAxis("Horizontal") * movementSpeed;
        float dirZ = Input.GetAxis("Vertical") * movementSpeed;

        Vector3 moveDir = new Vector3(dirX, 0f, dirZ);
        // helpful piece of code from: https://answers.unity.com/questions/804400/movement-based-on-camera-direction.html
        //basically makes it so that 'forward' changes based on your camera is looking
        moveDir = Camera.main.transform.TransformDirection(moveDir);
        moveDir.y = 0f;
        transform.position += moveDir * Time.deltaTime;

        //get mouse input
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        //rotate based on mouse input
        transform.Rotate(0f, mouseX, 0f, Space.World);
        transform.Rotate(-mouseY, 0f, 0f, Space.Self);
    }

    //function for piloting the ship
    private void Pilot()
    {
        //if the player is piloting, and they hit E, stop piloting
        if (Input.GetKeyDown(KeyCode.E) && timeSinceInteraction >= interactionCooldown)
        {
            interactionObject.GetComponent<InteractEnter>().SetEntityInteracting(null);
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
                interactionObject.GetComponent<InteractEnter>().SetEntityInteracting(this.transform);
                playerState = interactionState.PILOTING;
                timeSinceInteraction = 0.0f;
                //and make the cursor visible
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}