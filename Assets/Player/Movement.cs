using UnityEngine;

public class Movement : MonoBehaviour
{
    //public so they can be changed
    public float movementSpeed = 10f;
    public float lookSpeed = 5f;

    // Update is called once per frame
    private void Update()
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
}