using UnityEngine;
using TMPro; // Biblioteca que controla os textos da UI
using UnityEngine.UI; // Biblioteca necessária para manipular Imagens na UI
using UnityEngine.SceneManagement; // Biblioteca para recarregar a cena (Restart)
using UnityEngine.InputSystem; // <-- ESSA É A BIBLIOTECA QUE FALTAVA

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Telas do Jogo (UI Panels)")]
    public GameObject painelStart;
    public GameObject painelPause;
    public GameObject painelFimDeJogo;

    [Header("UI - Textos na Tela")]
    public TextMeshProUGUI textoDinheiro;
    public TextMeshProUGUI textoTempo;

    [Header("UI - Corações (Vidas)")]
    [Tooltip("Arraste os objetos de UI Image dos corações aqui")]
    public Image[] coracoesUI; 

    [Header("Sistema de Dinheiro")]
    public int dinheiroAtual = 0;
    public int metaDinheiro = 500;

    [Header("Sistema de Tempo")]
    public float tempoRestante = 105f; 
    public bool turnoAtivo = false; 

    [Header("Inventário Simples")]
    public int itensNaMao = 0;

    [Header("Sistema de Vidas")]
    public int vidas = 3;

    [Header("Configurações Globais da Esteira")]
    public float velocidadeGlobalEsteira = 2f;
    public float intervaloSpawnGlobal = 3f;
    public bool esteiraAtiva = true;

    [Header("Configurações Globais de Itens")]
    public float escalaGlobalItens = 1f;

    private bool jogoPausado = false;
    private bool jogoFinalizado = false;

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
        AtualizarUIDinheiro();
        AtualizarUIVidas(); 
        
        MostrarTelaStart();
    }

    private void Update()
    {
        // CORREÇÃO AQUI: Usando o Novo Input System para capturar a tecla ESC
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (turnoAtivo && !jogoFinalizado)
            {
                PausarOuRetomarJogo();
            }
        }

        if (turnoAtivo && !jogoPausado && !jogoFinalizado)
        {
            tempoRestante -= Time.deltaTime; 
            
            AtualizarUITempo(); 

            if (tempoRestante <= 0)
            {
                tempoRestante = 0;
                AtualizarUITempo();
                FinalizarJogo("FIM DE TURNO! O tempo acabou.");
            }
        }
    }

    public void AdicionarDinheiro(int valor)
    {
        dinheiroAtual += valor;
        Debug.Log($"Dinheiro atualizado: R$ {dinheiroAtual} / R$ {metaDinheiro}");
        
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
        if (jogoFinalizado) return; 

        vidas--;
        Debug.Log("Item perdido! Penalidade aplicada. Vidas restantes: " + vidas);

        AtualizarUIVidas(); 

        if (vidas <= 0)
        {
            FinalizarJogo("GAME OVER! A empresa faliu (Você perdeu todas as vidas).");
        }
    }

    // --- MÉTODOS PARA ATUALIZAR A INTERFACE ---

    private void AtualizarUIDinheiro()
    {
        if (textoDinheiro != null)
        {
            textoDinheiro.text = $"R$ {dinheiroAtual} / R$ {metaDinheiro}";
        }
    }

    private void AtualizarUITempo()
    {
        if (textoTempo != null)
        {
            int minutos = Mathf.FloorToInt(tempoRestante / 60);
            int segundos = Mathf.FloorToInt(tempoRestante % 60);
            
            textoTempo.text = string.Format("{0:00}:{1:00}", minutos, segundos);
        }
    }

    private void AtualizarUIVidas()
    {
        for (int i = 0; i < coracoesUI.Length; i++)
        {
            if (i < vidas) coracoesUI[i].enabled = true;
            else coracoesUI[i].enabled = false;
        }
    }

    // --- MÉTODOS DE CONTROLE DAS TELAS ---

    public void MostrarTelaStart()
    {
        if(painelStart != null) painelStart.SetActive(true);
        if(painelPause != null) painelPause.SetActive(false);
        if(painelFimDeJogo != null) painelFimDeJogo.SetActive(false);
        
        Time.timeScale = 0f; 
    }

    public void IniciarJogo()
    {
        if(painelStart != null) painelStart.SetActive(false);
        
        Time.timeScale = 1f; 
        turnoAtivo = true;
    }

    public void PausarOuRetomarJogo()
    {
        jogoPausado = !jogoPausado; 

        if (jogoPausado)
        {
            if(painelPause != null) painelPause.SetActive(true);
            Time.timeScale = 0f; 
        }
        else
        {
            if(painelPause != null) painelPause.SetActive(false);
            Time.timeScale = 1f; 
        }
    }

    public void BotaoRetomar() 
    {
        if (jogoPausado) PausarOuRetomarJogo();
    }

    private void FinalizarJogo(string mensagem)
    {
        Debug.Log(mensagem);
        jogoFinalizado = true;
        turnoAtivo = false;
        
        if(painelFimDeJogo != null) painelFimDeJogo.SetActive(true);
        Time.timeScale = 0f; 
    }

    public void ReiniciarJogo()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}