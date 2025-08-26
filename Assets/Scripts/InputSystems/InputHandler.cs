using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance { get; private set; }

    public Vector2 Move { get; private set; }
    public bool Jump { get; private set; }

    private PlayerInputActions actions;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        actions = new PlayerInputActions();
        actions.Player.Enable();

        actions.Player.Move.performed += ctx => Move = ctx.ReadValue<Vector2>();
        actions.Player.Move.canceled += ctx => Move = Vector2.zero;
        actions.Player.Jump.performed += ctx => Jump = true;
        actions.Player.Jump.canceled += ctx => Jump = false;
    }

    private void OnDestroy()
    {
        actions.Player.Move.performed -= ctx => Move = ctx.ReadValue<Vector2>();
        actions.Player.Move.canceled -= ctx => Move = Vector2.zero;
        actions.Player.Jump.performed -= ctx => Jump = true;
        actions.Player.Jump.canceled -= ctx => Jump = false;
    }
}
