using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Rigidbody2D))]

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;

    private PlayerInput playerInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        if (DialogueManager.Instance.isDialogueActive||Cutscene.cutsceneActive)
        {
            if (playerInput.enabled)
            {
                playerInput.enabled = false;
                GetComponent<Animator>().enabled = false;
                rb.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
                moveInput = Vector2.zero;
            }
        }
        else
        {
            if (!playerInput.enabled)
            {
                playerInput.enabled = true;
                GetComponent<Animator>().enabled = true;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
        }
        rb.linearVelocity = moveInput * moveSpeed;
    }

    public void Move(InputAction.CallbackContext context)
    {
        animator.SetBool("isWalking", true);

        if (context.canceled)
        {
            animator.SetBool("isWalking", false);
            animator.SetFloat("LastInX", moveInput.x);
            animator.SetFloat("LastInY", moveInput.y);
        }
        moveInput = context.ReadValue<Vector2>();
        animator.SetFloat("Horizontal", moveInput.x);
        animator.SetFloat("Vertical", moveInput.y);
    }
}
