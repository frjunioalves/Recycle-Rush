using UnityEngine;
using UnityEngine.InputSystem;

public class MovePlayer : MonoBehaviour
{
    [Header("Configurações de Velocidade")]
    public float velocidadeNormal = 7f;
    public float velocidadeCorrida = 12f; // Nova velocidade para quando correr

    [Header("Inputs")]
    public InputAction movimento;
    public InputAction inputCorrer; // Nova ação para o botão Shift

    private SpriteRenderer spriteRenderer;
    // private Animator animator;
    private Rigidbody2D rb;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        // animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        movimento.Enable();
        inputCorrer.Enable(); // Não esqueça de habilitar a nova ação!
    }

    void OnDisable()
    {
        movimento.Disable();
        inputCorrer.Disable(); // E desabilitar também
    }

    void FixedUpdate()
    {
        Vector2 direcao = movimento.ReadValue<Vector2>();

        // Verifica se a ação de correr está sendo pressionada e mantida
        bool estaCorrendo = inputCorrer.IsPressed();

        // Usa um Operador Ternário para decidir a velocidade:
        // Se 'estaCorrendo' for true, usa 'velocidadeCorrida', senão usa 'velocidadeNormal'
        float velocidadeAtual = estaCorrendo ? velocidadeCorrida : velocidadeNormal;

        // Aplica a velocidade atualizada no Rigidbody
        rb.linearVelocity = direcao * velocidadeAtual;

        // Vira o sprite dependendo da direção horizontal
        if (direcao.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (direcao.x > 0)
        {
            spriteRenderer.flipX = false;
        }

        // Exemplo de como você atualizaria o Animator depois:
        // bool estaAndando = direcao.magnitude > 0;
        // animator.SetBool("isMoving", estaAndando);
        // animator.SetBool("isRunning", estaCorrendo && estaAndando); 
    }
}