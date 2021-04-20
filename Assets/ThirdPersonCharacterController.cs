using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class ThirdPersonCharacterController : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float smoothTurnTime = 0.1f;
    [SerializeField][Range(0f, 5f)] private float sprintMultiplier = 1.5f;
    [Space]
    [SerializeField] private Camera cam = null;
    
    private CapsuleCollider coll = null;
    private Rigidbody rb = null;

    private Vector3 _velocity = Vector3.zero;
    
    private float _turnSmoothVelocity;
    private float _constSpeed = 0f;

    private bool _isSprinting = false;

    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();

        _constSpeed = speed;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void FixedUpdate()
    {
        handleMovement();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
            _isSprinting = true;
        else if(Input.GetKeyUp(KeyCode.LeftShift))
            _isSprinting = false;
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded())
            rb.velocity += Vector3.up * jumpHeight;
    }

    private void handleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, smoothTurnTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            if(_isSprinting)
                rb.position += moveDir.normalized * speed * sprintMultiplier * Time.deltaTime;
            else
                rb.position += moveDir.normalized * speed * Time.deltaTime;
        }

        
    }

    private bool isGrounded()
    {
        float extraHeight = 0.01f;
        if (Physics.Raycast(coll.bounds.center, Vector3.down, coll.bounds.extents.y + extraHeight))
        {
            return true;
        }
        return false;
    }
}
