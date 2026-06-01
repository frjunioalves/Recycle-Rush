using UnityEngine;
using UnityEngine.InputSystem;

public class MovePlayer : MonoBehaviour
{
    public float velocidade = 7f;
    public InputAction movimento;

    private SpriteRenderer spriteRenderer;
    // Comentamos as variáveis de animação por enquanto
    // private Animator animator;

    private Rigidbody2D rb;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        // Comentamos a busca do Animator
        // animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        movimento.Enable();
    }

    void OnDisable()
    {
        movimento.Disable();
    }

    void FixedUpdate()
    {
        Vector2 direcao = movimento.ReadValue<Vector2>();

        // Move o personagem para todos os lados (X e Y)
        rb.linearVelocity = direcao * velocidade;

        // Vira o sprite dependendo da direção horizontal
        if (direcao.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (direcao.x > 0)
        {
            spriteRenderer.flipX = false;
        }

        // Comentamos o aviso para o Animator
        // bool estaAndando = direcao.magnitude > 0;
        // animator.SetBool("isMoving", estaAndando);
    }
}
