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

    [Header("Configurações de Áudio")]
    public AudioSource audioPassos; // Arraste o AudioSource do jogador aqui
    public AudioClip somDePasso;    // Arraste o arquivo de som (mp3/wav) aqui

    private SpriteRenderer spriteRenderer;
    // private Animator animator;
    private Rigidbody2D rb;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        // animator = GetComponent<Animator>();
    }

    void Start()
    {
        // Garante que o AudioSource está com o clipe certo e configurado para repetir (loop)
        if (audioPassos != null && somDePasso != null)
        {
            audioPassos.clip = somDePasso;
            audioPassos.loop = true; 
        }
    }

    void OnEnable()
    {
        movimento.Enable();
        inputCorrer.Enable(); 
    }

    void OnDisable()
    {
        movimento.Disable();
        inputCorrer.Disable(); 
    }

    void FixedUpdate()
    {
        Vector2 direcao = movimento.ReadValue<Vector2>();

        // Verifica se a ação de correr está sendo pressionada e mantida
        bool estaCorrendo = inputCorrer.IsPressed();

        // Usa um Operador Ternário para decidir a velocidade
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

        // --- LÓGICA DE ÁUDIO DOS PASSOS ---
        // Se a magnitude da direção for maior que 0, significa que o jogador está tentando se mover
        bool estaSeMovendo = direcao.magnitude > 0; 
        ControlarSomDosPassos(estaSeMovendo, estaCorrendo);

        // Exemplo de como você atualizaria o Animator depois:
        // animator.SetBool("isMoving", estaSeMovendo);
        // animator.SetBool("isRunning", estaCorrendo && estaSeMovendo); 
    }

    // Função dedicada a cuidar apenas do som, deixando o FixedUpdate mais limpo
    private void ControlarSomDosPassos(bool movendo, bool correndo)
    {
        // Se você esquecer de arrastar o AudioSource, ele ignora para não dar erro
        if (audioPassos == null) return;

        if (movendo)
        {
            // Se começou a andar e o som não está tocando, dá play
            if (!audioPassos.isPlaying)
            {
                audioPassos.Play();
            }

            // O TRUQUE: Se correr, a velocidade do som dobra (pitch = 2). Senão, normal (pitch = 1)
            audioPassos.pitch = correndo ? 2f : 1f;
        }
        else
        {
            // Se parou de andar, pausa o som
            if (audioPassos.isPlaying)
            {
                audioPassos.Pause(); 
                // Usamos Pause para que, ao voltar a andar, o passo continue do meio e não do zero
            }
        }
    }
}