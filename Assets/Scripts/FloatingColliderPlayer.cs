using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FloatingPlayerController : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform mesh;
    public Transform groundCheck;
    public Animator animator;
    public LayerMask groundMask;

    [Header("Movement")]
    public float moveSpeed = 6f;
    public float rotationSpeed = 10f;

    [Header("Jumping")]
    public float jumpForce = 7f;
    private bool jumpRequested = false;
    private bool isJumping = false;

    [Header("Floating Settings")]
    public float floatHeight = 1f;
    public float springStrength = 500f;
    public float springDamping = 50f;
    public float gravityForce = -20f;

    [Header("Dodge")]
    public float dodgeForce = 10f;
    public float dodgeDuration = 0.2f;
    public float dodgeCooldown = 1f;
    private bool isDodging = false;
    private bool canDodge = true;

    private Rigidbody rb;
    private Vector3 input;

    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        HandleInput();
        UpdateAnimations();
        HandleDodgeInput();
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isJumping)
        {
            HandleJumpInput();
        }
        ApplyGravity();
    }

    void FixedUpdate()
    {
        HandleGroundCheck();
        ApplyFloatingForce();
        ApplyMovement();

    }

    void HandleInput()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.z = Input.GetAxisRaw("Vertical");
        input = Vector3.ClampMagnitude(input, 1f);
    }

    void ApplyGravity()
    {
        if (!isGrounded)
        {
            rb.linearVelocity += Vector3.up * gravityForce * Time.fixedDeltaTime;
        }
    }

    void ApplyMovement()
    {
        if (isDodging) return;

        Vector3 moveDirection = orientation.forward * input.z + orientation.right * input.x;
        moveDirection.y = 0f;

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            mesh.rotation = Quaternion.Slerp(mesh.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        Vector3 desiredVelocity = moveDirection.normalized * moveSpeed;
        Vector3 velocityChange = desiredVelocity - new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    void ApplyFloatingForce()
    {
        if (isJumping) return;

        if (Physics.Raycast(groundCheck.position, -transform.up, out RaycastHit hit, floatHeight + 0.5f, groundMask))
        {
            float distanceToGround = hit.distance;
            float force = (floatHeight - distanceToGround) * springStrength;

            // damping based on current vertical velocity
            force -= rb.linearVelocity.y * springDamping;

            rb.AddForce(transform.up * force, ForceMode.Acceleration);

            if (isGrounded)
            {
                isJumping = false;
                if (animator != null) animator.SetBool("isJumping", false);
            }
        }

    }

    void HandleGroundCheck()
    {
        isGrounded = Physics.CheckSphere(new Vector3(0,-floatHeight, 0) + groundCheck.position, 0.2f, groundMask);
        //isGrounded = Physics.Raycast(groundCheck.position, -transform.up, out RaycastHit hit, floatHeight + 0.5f, groundMask);
    }

    void HandleJumpInput()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); // reset y velocity
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isJumping = true;
        isGrounded = false;

        // Optional: play animation
        if (animator != null)
        {
            animator.SetBool("isJumping", true);
            animator.Play("Jump");
        }
    }

    public void ResetIsJumpingAnim()
    {
        isJumping = false;
        animator.SetBool("isJumping", false);
    }

    void HandleDodgeInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDodge && !isDodging && input != Vector3.zero)
        {
            StartCoroutine(Dodge());
        }
    }

    IEnumerator Dodge()
    {
        isDodging = true;
        canDodge = false;
        float timer = 0f;

        Vector3 dodgeDir = orientation.forward * input.z + orientation.right * input.x;
        rb.linearVelocity = dodgeDir.normalized * dodgeForce;

        // Temporarily make player invincible
        // You can add invincibility logic here (e.g., ignore damage)

        while (timer < dodgeDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        isDodging = false;

        yield return new WaitForSeconds(dodgeCooldown);
        canDodge = true;
    }

    void UpdateAnimations()
    {
        float speed = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;
        animator.SetBool("isRunning", speed > 0.1f);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("X", input.x);
        animator.SetFloat("Y", input.z);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(new Vector3(0, -floatHeight, 0) + groundCheck.position, 0.2f);
        }
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * floatHeight * 2f);
    }
}
