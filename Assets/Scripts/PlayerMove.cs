using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float rotateSpeed = 100f; // 回転速度

    private Rigidbody rb;
    private Vector2 moveInput;
    private bool jumpInput;
    private bool isGrounded;
    public PlayerState currentState = PlayerState.Normal;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 入力取得
        moveInput = InputHandler.Instance.Move; // Vector2 (x:左右, y:前後)
        jumpInput = InputHandler.Instance.Jump; // bool

        // Qキーで状態切り替え
        if (InputHandler.Instance.ModeSwitch)
        {
            currentState = currentState == PlayerState.Normal ? PlayerState.Combine : PlayerState.Normal;
        }

        // ジャンプ入力
        if (jumpInput && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void FixedUpdate()
    {
        // 前後移動（W/S）
        Vector3 forwardMove = transform.forward * moveInput.y * moveSpeed;
        Vector3 velocity = rb.linearVelocity;
        velocity.x = forwardMove.x;
        velocity.z = forwardMove.z;
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);

        // 左右回転（A/D）
        float rotation = moveInput.x * rotateSpeed * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0, rotation, 0));
    }

    void OnCollisionEnter(Collision collision)
    {
        // 地面判定
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
