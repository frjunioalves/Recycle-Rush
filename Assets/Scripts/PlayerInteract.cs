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
                    return; 
                }
            }

            // Tenta ver se é a Alavanca da Esteira
            AlavancaEsteira alavanca = colisor.GetComponent<AlavancaEsteira>();
            if (alavanca != null)
            {
                alavanca.Interagir();
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
            }
        }
    }

   void SoltarItem()
   {
       // 1. Tenta interagir com objetos próximos (Lixeira, Triturador ou Alavanca)
       Collider2D[] colisoresProximos = Physics2D.OverlapCircleAll(transform.position, raioInteracao);
       foreach (Collider2D colisor in colisoresProximos)
       {
           // Tenta ver se é a Alavanca da Esteira (Permite ligar/desligar mesmo segurando item)
           AlavancaEsteira alavanca = colisor.GetComponent<AlavancaEsteira>();
           if (alavanca != null)
           {
               alavanca.Interagir();
               return; 
           }

           // Tenta colocar na Lixeira
           Lixeira lixeira = colisor.GetComponent<Lixeira>();
            if (lixeira != null)
            {
                bool conseguiuDepositar = lixeira.ReceberItem(itemSeguradoAtual);
                if (conseguiuDepositar)
                {
                    LimparMao();
                    return; // Se depositou, encerra a função
                }
            }

            // NOVA PARTE: Tenta colocar no Triturador
            Triturador triturador = colisor.GetComponent<Triturador>();
            if (triturador != null)
            {
                bool conseguiuTriturar = triturador.ReceberItem(itemSeguradoAtual);
                if (conseguiuTriturar)
                {
                    LimparMao();
                    return; // Se o triturador engoliu o item, encerra a função
                }
            }
        }

        // 2. SE NÃO ACHOU LIXEIRA NEM TRITURADOR: Joga no chão
        if (prefabItemGenerico != null)
        {
            GameObject novoObjeto = Instantiate(prefabItemGenerico, transform.position, Quaternion.identity);
            
            ItemColetavel scriptDoItem = novoObjeto.GetComponent<ItemColetavel>();
            
            if (scriptDoItem != null) scriptDoItem.ConfigurarItem(itemSeguradoAtual);
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, raioInteracao);
    }
}