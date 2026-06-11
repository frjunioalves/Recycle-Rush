using UnityEngine;
using TMPro; // 1. IMPORTANTE: Biblioteca que controla os textos da UI

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI - Textos na Tela")]
    // 2. Variáveis para você arrastar os textos lá no Inspector
    public TextMeshProUGUI textoDinheiro;
    public TextMeshProUGUI textoTempo;

    [Header("Sistema de Dinheiro")]
    public int dinheiroAtual = 0;
    public int metaDinheiro = 500;

    [Header("Sistema de Tempo")]
    public float tempoRestante = 105f; 
    public bool turnoAtivo = true;

    [Header("Inventário Simples")]
    public int itensNaMao = 0;

    [Header("Sistema de Vidas")]
    public int vidas = 3;

    [Header("Configurações Globais da Esteira")]
    public float velocidadeGlobalEsteira = 2f;
    public float intervaloSpawnGlobal = 3f;
    public bool esteiraAtiva = true; // Nova variável para ligar/desligar

    [Header("Configurações Globais de Itens")]
    [Tooltip("Controla o tamanho de todos os itens spawnados na esteira")]
    public float escalaGlobalItens = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    private void Start()
    {
        // 3. Assim que o jogo começa, ele injeta os valores iniciais na tela
        AtualizarUIDinheiro();
    }

    private void Update()
    {
        if (turnoAtivo)
        {
            tempoRestante -= Time.deltaTime; 
            
            // 4. Atualiza o relógio visualmente frame a frame
            AtualizarUITempo(); 

            if (tempoRestante <= 0)
            {
                tempoRestante = 0;
                turnoAtivo = false;
                
                // Muda o texto para avisar que acabou
                if(textoTempo != null) textoTempo.text = "FIM DE TURNO!";
                Debug.Log("O TURNO ACABOU! Fim do expediente.");
            }
        }
    }

    public void AdicionarDinheiro(int valor)
    {
        dinheiroAtual += valor;
        Debug.Log($"Dinheiro atualizado: R$ {dinheiroAtual} / R$ {metaDinheiro}");
        
        // 5. Atualiza a tela sempre que o dinheiro mudar
        AtualizarUIDinheiro(); 
        
        if (dinheiroAtual >= metaDinheiro)
        {
            Debug.Log("META BATIDA! A empresa não vai falir!");
        }
    }

    public void ColetarItem()
    {
        itensNaMao++;
        Debug.Log($"Inventário: {itensNaMao} item(s) na mão.");
    }

    public void AplicarPenalidade()
    {
        vidas--;
        Debug.Log("Item perdido! Penalidade aplicada. Vidas restantes: " + vidas);

        if (vidas <= 0)
        {
            Debug.Log("Game Over!");
            // Lógica de Game Over aqui
        }
    }

    // --- MÉTODOS PARA ATUALIZAR A INTERFACE (FRONT-END) ---

    private void AtualizarUIDinheiro()
    {
        // A checagem (!= null) evita erros caso você esqueça de arrastar o texto no Inspector
        if (textoDinheiro != null)
        {
            textoDinheiro.text = $"R$ {dinheiroAtual} / R$ {metaDinheiro}";
        }
    }

    private void AtualizarUITempo()
    {
        if (textoTempo != null)
        {
            // Matemática simples para transformar os segundos flutuantes em "Minutos:Segundos"
            int minutos = Mathf.FloorToInt(tempoRestante / 60);
            int segundos = Mathf.FloorToInt(tempoRestante % 60);
            
            // Formata a string para sempre ter 2 casas (ex: 01:05)
            textoTempo.text = string.Format("{0:00}:{1:00}", minutos, segundos);
        }
    }
}