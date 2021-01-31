using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationStateController : MonoBehaviour
{
    Animator animator;
    int velocityHash;
    float velocity;

    bool cameraRotateRight = false;
    bool cameraRotateLeft = false;
    float startAngle = 0;
    float endAngle = 0;
    public float rotateIncrement = 45;
    public float cameraRotationDuration = 0.01f;

    float time = 0;

    public GameObject characterContainer;
    public GameObject character;
    public Camera cameraToLookAt;

    public GameObject cameraContainer;

    public float acceleration = 2f;
    public float deceleration = 5f;
    public float velocityMultiplier = 0.4f;

    public float characterScale;


    // Start is called before the first frame update
    void Start()
    {
        animator = character.GetComponent<Animator>();
        velocityHash = Animator.StringToHash("Velocity");

        // Get initial player scale
        characterScale = characterContainer.transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInputs();

        // The sprite is always facing the camera
        characterContainer.transform.LookAt(cameraToLookAt.transform);
    }

    float easeOutCubic(float x)
    {
        return 1 - Mathf.Pow(1 - x, 3);
    }

    void ProcessInputs()
    {
        bool leftPressed = Input.GetKey(KeyCode.Q);
        bool rightPressed = Input.GetKey(KeyCode.D);
        bool upPressed = Input.GetKey(KeyCode.Z);
        bool downPressed = Input.GetKey(KeyCode.S);

        if (leftPressed || rightPressed || upPressed || downPressed)
        {
            if (velocity < 1)
            {
                velocity += Time.deltaTime * acceleration;
            }

            // Initialize direction
            Vector3 direction = Vector3.zero;

            // Check for key pressed
            if (leftPressed)
            {
                // Unflip the sprite
                characterContainer.transform.localScale = new Vector3(-characterScale, characterScale, characterScale);

                direction -= cameraContainer.transform.right;
            }

            if (rightPressed)
            {
                // Flip the sprite
                characterContainer.transform.localScale = Vector3.one * characterScale;

                direction += cameraContainer.transform.right;
            }

            if (upPressed)
            {
                direction += cameraContainer.transform.forward;
            }

            if (downPressed)
            {
                direction -= cameraContainer.transform.forward;
            }

            // Move the player in that direction
            moveCharacter(direction);
        }
        else if (velocity > 0)
        { // Otherwise decrease the velocity
            velocity -= Time.deltaTime * deceleration;
        }

        bool rotateLeft = Input.GetKey(KeyCode.A);
        bool rotateRight = Input.GetKey(KeyCode.E);

        if (!cameraRotateRight && !cameraRotateLeft)
        {
            if (rotateRight)
            {
                cameraRotateRight = true;
            }

            if (rotateLeft)
            {
                cameraRotateLeft = true;
            }
        }

        // If it's the end of the camera rotation, stop it
        if (cameraRotateRight && Time.time - time > cameraRotationDuration)
        {
            cameraRotateRight = false;

            time = Time.time;
            startAngle = cameraContainer.transform.eulerAngles.y;
            endAngle = startAngle - rotateIncrement;
        }

        if (cameraRotateLeft && Time.time - time > cameraRotationDuration)
        {
            cameraRotateLeft = false;
            time = Time.time;

            startAngle = cameraContainer.transform.eulerAngles.y;
            endAngle = startAngle + rotateIncrement;
        }

        // If rotating right, set rotation
        if (cameraRotateRight)
        {
            cameraContainer.transform.eulerAngles = new Vector3(
                cameraContainer.transform.eulerAngles.x,
                startAngle - (endAngle - startAngle) * getCameraRotationTimeNorm(),
                cameraContainer.transform.eulerAngles.z
            );
        }

        // If rotating left, set rotation
        if (cameraRotateLeft)
        {
            cameraContainer.transform.eulerAngles = new Vector3(
                cameraContainer.transform.eulerAngles.x,
                startAngle - (endAngle - startAngle) * getCameraRotationTimeNorm(),
                cameraContainer.transform.eulerAngles.z
            );
        }

        // Update the blending parameter
        updateVelocityBlend();
    }

    float getCameraRotationTimeNorm()
    {
        return easeOutCubic((Time.time - time) / cameraRotationDuration);
    }

    void updateVelocityBlend()
    {
        animator.SetFloat(velocityHash, velocity);
    }

    void moveCharacter(Vector3 direction)
    {
        transform.Translate(direction * velocity * velocityMultiplier);
    }
}
