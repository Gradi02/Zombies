using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Steamworks;


public class SC_FPSController : NetworkBehaviour
{
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;
    float rotationX = 0;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;

    [HideInInspector]
    public bool canMove = true;
    public bool canSprint = true;

    [SerializeField] private GameObject head;
    [SerializeField] private Animator anim;
    [SerializeField] private Transform playerMeshs;

    void Start()
    {
        if (!IsOwner)
        {
            playerCamera.gameObject.SetActive(false);
            GetComponent<CharacterController>().enabled = false;
            GetComponent<PlayerShooting>().enabled = false;
            enabled = false;
            return;
        }

        GetComponent<PlayerShooting>().enabled = true;

        foreach (Transform g1 in playerMeshs.GetComponentInChildren<Transform>())
        {
            foreach (Transform g2 in g1.GetComponentInChildren<Transform>())
            {
                g2.gameObject.layer = 11;

                foreach (Transform g3 in g2.GetComponentInChildren<Transform>())
                {
                    g3.gameObject.layer = 11;
                }
            }
        }

        characterController = GetComponent<CharacterController>();
        gameObject.name = SteamClient.Name;

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!IsOwner) return;

        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Press Left Shift to run
        bool isRunning = canSprint && Input.GetKey(KeyCode.LeftShift);
        float speed = isRunning ? runningSpeed : walkingSpeed;

        float curSpeedX = canMove ? Input.GetAxisRaw("Vertical") : 0;
        float curSpeedY = canMove ? Input.GetAxisRaw("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = ((forward * curSpeedX) + (right * curSpeedY)).normalized;
        moveDirection *= speed;

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        Vector3 localVel = transform.InverseTransformDirection(characterController.velocity);
        anim.SetFloat("velocityX", localVel.x);
        anim.SetFloat("velocityZ", localVel.z);
        anim.SetBool("isJumping", !characterController.isGrounded);

        // Player and Camera rotation
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            head.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }
}