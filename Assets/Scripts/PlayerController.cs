using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Transform Orientation;
    public Transform groundCheck;
    public Transform mesh;
    public LayerMask groundMask;
    public Animator animator;

    [Header("Movement")]
    public float moveSpeed = 6f;
    public float rotationSpeed = 10f;
    public float jumpForce = 7f;
    public float gravity = -20f;
    public float slopeRayLength = 0.6f;
    public float slopeLimit = 45f;

    [Header("Ground Check")]
    public float groundDistance = 0.4f;

    private Rigidbody rb;
    private Vector3 input;
    private bool isGrounded;
    private bool onSlope;

    private Vector3 moveDirection;

    private RaycastHit slopeHit;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {

        HandleInput();
        HandleGroundCheck();
        HandleJumpInput();

        float speed = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;
        animator.SetBool("isRunning", speed > 0.1f);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("X", input.x);
        animator.SetFloat("Y", input.z);
    }

    void FixedUpdate()
    {
        ApplyMovement();
        ApplyGravity();
    }

    void HandleInput()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.z = Input.GetAxisRaw("Vertical");
        input = Vector3.ClampMagnitude(input, 1f);
    }

    void ApplyMovement()
    {
        moveDirection = Orientation.forward * input.z + Orientation.right * input.x;
        moveDirection.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        if(moveDirection != Vector3.zero)
        {
            mesh.localRotation = Quaternion.Slerp(mesh.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        moveDirection.Normalize();

        print(OnSlope());

        Vector3 velocity;
        if (OnSlope())
        {
            velocity = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized * moveSpeed;
        }
        else
        {
            velocity = moveDirection * moveSpeed;
        }
        velocity.y = rb.linearVelocity.y; // preserve y
        rb.linearVelocity = velocity;



    }

    void HandleJumpInput()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            animator.Play("Jump");
            animator.SetBool("isJumping", true);
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }
    }

    public void ResetIsJumpingAnim()
    {
        animator.SetBool("isJumping", false);
    }

    void ApplyGravity()
    {
        if (!isGrounded)
        {
            rb.linearVelocity += Vector3.up * gravity * Time.fixedDeltaTime;
        }
    }

    void HandleGroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, slopeRayLength, groundMask))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle > 0f && angle <= slopeLimit;
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;

            // Starting point of the ray
            Vector3 rayStart = groundCheck.position + Vector3.up * 0.1f;
            Vector3 rayDirection = Vector3.down * (groundDistance + 0.2f);

            // Draw the ray
            Gizmos.DrawLine(rayStart, rayStart + rayDirection);

            // Optional: Draw a small sphere at the end of the ray
            Gizmos.DrawWireSphere(rayStart + rayDirection, 0.05f);
        }
    }

}
