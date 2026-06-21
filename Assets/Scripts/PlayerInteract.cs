using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    [Header("Configurações de Interação")]
    public float raioInteracao = 1.5f;
    public LayerMask layerItens; 

    [Header("Sistema de Itens")]
    public GameObject prefabItemGenerico;

    [Header("Interface do Inventário")]
    public Image imagemUI; 

    [Header("Efeitos Sonoros")]
    public AudioSource audioInteracao; // Arraste o AudioSource do jogador aqui
    public AudioClip somPegarItem;     // Som ao pegar um item do chão ou lixeira
    public AudioClip somSoltarItem;    // Som ao dropar, triturar, vender ou jogar na lixeira
    public AudioClip somInteragir;     // Som genérico para alavancas

    private ItemData itemSeguradoAtual = null;

    void Start()
    {
        imagemUI.enabled = false;
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (itemSeguradoAtual == null)
            {
                TentarPegarItem();
            }
            else
            {
                SoltarItem();
            }
        }
    }

    void TentarPegarItem()
    {
        // 1. PRIMEIRO: Tenta interagir com objetos fixos (Lixeira, Alavanca, etc.)
        Collider2D[] colisoresProximos = Physics2D.OverlapCircleAll(transform.position, raioInteracao);
        foreach (Collider2D colisor in colisoresProximos)
        {
            // Tenta ver se é uma Lixeira para retirar item
            Lixeira lixeira = colisor.GetComponent<Lixeira>();
            if (lixeira != null)
            {
                ItemData itemRetirado = lixeira.RetirarItem();
                
                if (itemRetirado != null)
                {
                    itemSeguradoAtual = itemRetirado;
                    imagemUI.sprite = itemSeguradoAtual.iconeParaUI;
                    imagemUI.enabled = true;
                    TocarSom(somPegarItem); // TOCA O SOM AQUI
                    return; 
                }
            }

            // Tenta ver se é a Alavanca da Esteira
            AlavancaEsteira alavanca = colisor.GetComponent<AlavancaEsteira>();
            if (alavanca != null)
            {
                alavanca.Interagir();
                TocarSom(somInteragir); // TOCA O SOM AQUI
                return; // Interagiu com a alavanca, encerra a função
            }
        }

        // 2. SE NÃO ACHOU OBJETOS FIXOS: Tenta pegar um item normal do chão/esteira
        Collider2D colisorChao = Physics2D.OverlapCircle(transform.position, raioInteracao, layerItens);
        if (colisorChao != null && colisorChao.CompareTag("Item"))
        {
            ItemColetavel itemNoChao = colisorChao.GetComponent<ItemColetavel>();
            if (itemNoChao != null && itemNoChao.dadosDoItem != null)
            {
                itemSeguradoAtual = itemNoChao.dadosDoItem;
                imagemUI.sprite = itemSeguradoAtual.iconeParaUI;
                imagemUI.enabled = true;
                
                Destroy(colisorChao.gameObject);
                Debug.Log("Pegou do chão: " + itemSeguradoAtual.nomeVisivel);
                TocarSom(somPegarItem); // TOCA O SOM AQUI
            }
        }
    }

   void SoltarItem()
   {
       // 1. Tenta interagir com objetos próximos (Lixeira, Triturador, Ponto de Venda ou Alavanca)
       Collider2D[] colisoresProximos = Physics2D.OverlapCircleAll(transform.position, raioInteracao);
       foreach (Collider2D colisor in colisoresProximos)
       {
           // Tenta ver se é a Alavanca da Esteira (Permite ligar/desligar mesmo segurando item)
           AlavancaEsteira alavanca = colisor.GetComponent<AlavancaEsteira>();
           if (alavanca != null)
           {
               alavanca.Interagir();
               TocarSom(somInteragir); // TOCA O SOM AQUI
               return; 
           }

           // Tenta colocar na Lixeira
           Lixeira lixeira = colisor.GetComponent<Lixeira>();
            if (lixeira != null)
            {
                bool conseguiuDepositar = lixeira.ReceberItem(itemSeguradoAtual);
                if (conseguiuDepositar)
                {
                    TocarSom(somSoltarItem); // TOCA O SOM AQUI
                    LimparMao();
                    return; // Se depositou, encerra a função
                }
            }

            // Tenta colocar no Triturador
            Triturador triturador = colisor.GetComponent<Triturador>();
            if (triturador != null)
            {
                bool conseguiuTriturar = triturador.ReceberItem(itemSeguradoAtual);
                if (conseguiuTriturar)
                {
                    TocarSom(somSoltarItem); // TOCA O SOM AQUI
                    LimparMao();
                    return; // Se o triturador engoliu o item, encerra a função
                }
            }

            // NOVA PARTE: Tenta vender o item no Ponto de Venda
            PontoDeVenda balcaoDeVenda = colisor.GetComponent<PontoDeVenda>();
            if (balcaoDeVenda != null)
            {
                bool conseguiuVender = balcaoDeVenda.ReceberVenda(itemSeguradoAtual);
                if (conseguiuVender)
                {
                    TocarSom(somSoltarItem); // TOCA O SOM AQUI (Pode ser um som de moedas depois!)
                    LimparMao();
                    return; // Vendeu com sucesso, encerra a função
                }
            }
        }

        // 2. SE NÃO ACHOU OBJETOS FIXOS: Joga no chão
        if (prefabItemGenerico != null)
        {
            GameObject novoObjeto = Instantiate(prefabItemGenerico, transform.position, Quaternion.identity);
            
            ItemColetavel scriptDoItem = novoObjeto.GetComponent<ItemColetavel>();
            
            if (scriptDoItem != null) scriptDoItem.ConfigurarItem(itemSeguradoAtual);
            
            TocarSom(somSoltarItem); // TOCA O SOM AQUI
        }
        else
        {
            Debug.LogWarning("Atenção: O Prefab Item Generico não foi arrastado para o Inspector!");
        }

        Debug.Log("Soltou no chão: " + itemSeguradoAtual.nomeVisivel);
        LimparMao();
    }

    void LimparMao()
    {
        imagemUI.sprite = null;
        imagemUI.enabled = false;
        itemSeguradoAtual = null;
    }

    // Função auxiliar para tocar os sons facilmente e sem erros
    private void TocarSom(AudioClip clipeDeSom)
    {
        if (audioInteracao != null && clipeDeSom != null)
        {
            audioInteracao.PlayOneShot(clipeDeSom);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, raioInteracao);
    }
}