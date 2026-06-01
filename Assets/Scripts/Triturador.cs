using System.Collections;
using UnityEngine;
using TMPro;

public class Triturador : MonoBehaviour
{
    [Header("Configurações Financeiras")]
    public int moedasPorCiclo = 25; // Moedas ganhas ao finalizar o cubo

    [Header("Configurações da Máquina")]
    public float tempoPorLixo = 2.0f;
    public int metaDeLixo = 10;
    
    // Variáveis de controle interno
    private int lixoAtual = 0;
    private float tempoRestante = 0f;

    [Header("Configurações de Drop (Item Sistema)")]
    [Tooltip("O arquivo de dados do Cubo de Sucata (ScriptableObject)")]
    public ItemData dadoCuboSucata; 
    [Tooltip("O prefab genérico que tem o script ItemColetavel")]
    public GameObject prefabBaseItem; 
    public Transform pontoDeDrop; // De onde o item vai pular

    [Header("Interface (UI)")]
    public TMP_Text textoDeTempo;
    public TMP_Text textoCapacidade; 

    [Header("Feedbacks Visuais e Sonoros")]
    public Animator animator;
    public AudioSource audioSource;
    public ParticleSystem particulas;
    public AudioClip somDeTrituracao;

    public bool estaProcessando { get; private set; } = false;

    private void Start()
    {
        if (textoDeTempo != null) textoDeTempo.gameObject.SetActive(false);
        AtualizarUICapacidade();
    }

    public bool ReceberItem(ItemData itemDescartado)
    {
        // Se a máquina já estiver cheia, recusa
        if (lixoAtual >= metaDeLixo)
        {
            Debug.Log("Triturador cheio! Aguardando finalizar o processo.");
            return false;
        }

        // Se for importante, você pode verificar aqui se o itemDescartado é Não Reciclável
        // if (itemDescartado.categoria != CategoriaItem.NaoReciclavel) return false;

        lixoAtual++;
        tempoRestante += tempoPorLixo;
        AtualizarUICapacidade();

        if (!estaProcessando)
        {
            StartCoroutine(RotinaDeTrituracao());
        }

        return true; 
    }

    private IEnumerator RotinaDeTrituracao()
    {
        estaProcessando = true;

        if (animator != null) animator.SetBool("Trabalhando", true);
        if (particulas != null) particulas.Play();
        
        if (audioSource != null && somDeTrituracao != null) 
        {
            audioSource.clip = somDeTrituracao;
            audioSource.Play();
        }

        if (textoDeTempo != null) textoDeTempo.gameObject.SetActive(true);

        while (tempoRestante > 0)
        {
            tempoRestante -= Time.deltaTime;
            
            if (textoDeTempo != null) 
                textoDeTempo.text = tempoRestante.ToString("F1") + "s";
                
            yield return null; 
        }

        tempoRestante = 0;
        
        if (textoDeTempo != null) textoDeTempo.gameObject.SetActive(false);
        if (animator != null) animator.SetBool("Trabalhando", false);
        if (particulas != null) particulas.Stop();
        if (audioSource != null) audioSource.Stop();

        estaProcessando = false;

        // Se atingiu a meta, gera o cubo de sucata
        if (lixoAtual >= metaDeLixo)
        {
            GerarDrop();
        }
    }

    private void GerarDrop()
    {
        Debug.Log($"Ciclo concluído! Você ganhou {moedasPorCiclo} moedas e gerou um Cubo de Sucata.");

        // Verifica se todas as referências foram colocadas no Inspector
        if (dadoCuboSucata != null && prefabBaseItem != null && pontoDeDrop != null)
        {
            // 1. Spawna o Prefab Base no ponto de Drop
            GameObject novoDrop = Instantiate(prefabBaseItem, pontoDeDrop.position, pontoDeDrop.rotation);
            
            // 2. Procura o componente ItemColetavel que está no Prefab Base
            ItemColetavel scriptColetavel = novoDrop.GetComponent<ItemColetavel>();
            
            // 3. Se encontrou o script, injeta os dados do Cubo de Sucata nele!
            if (scriptColetavel != null)
            {
                scriptColetavel.ConfigurarItem(dadoCuboSucata);
            }

            MoverNaEsteira scriptMovimento = novoDrop.GetComponent<MoverNaEsteira>();
            if (scriptMovimento != null)
            {
                scriptMovimento.PegarItem(); // Muda a variável estaNaEsteira para false
            }
        }
        else
        {
            Debug.LogWarning("Faltam referências no Triturador! Configure o Dado do Cubo, Prefab Base e Ponto de Drop.");
        }

        lixoAtual = 0;
        AtualizarUICapacidade();
    }

    private void AtualizarUICapacidade()
    {
        if (textoCapacidade != null)
        {
            textoCapacidade.text = $"{lixoAtual}/{metaDeLixo}";
        }
    }
}