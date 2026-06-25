using UnityEngine;
using System.Collections.Generic;

public class BancadaCrafting : MonoBehaviour
{
    [Header("Receitas Disponíveis")]
    public List<ReceitaData> receitas; // Arraste suas receitas criadas no Unity aqui

    [Header("Itens Depositados")]
    public List<ItemData> itensNaBancada = new List<ItemData>();

    [Header("Configuração de Spawn")]
    public GameObject prefabItemGenerico; // O mesmo prefab genérico do PlayerInteract
    public Transform pontoDeSpawn; // Um ponto vazio (Empty) filho da bancada para o item aparecer

    [Header("Configuração de Cancelamento")]
    [Tooltip("Distância que os itens vão se espalhar ao cancelar")]
    public float forcaEjeção = 1.0f;

    // Função chamada quando o jogador solta um item na bancada
    public bool ReceberItem(ItemData item)
    {
        if (item != null)
        {
            itensNaBancada.Add(item);
            Debug.Log("Item " + item.nomeVisivel + " colocado na bancada.");
            TentarCraftar();
            return true;
        }
        return false;
    }

    // Função chamada quando o jogador interage de mãos vazias para retirar um item que colocou errado
    public ItemData RetirarItem()
    {
        if (itensNaBancada.Count > 0)
        {
            ItemData ultimoItem = itensNaBancada[itensNaBancada.Count - 1];
            itensNaBancada.RemoveAt(itensNaBancada.Count - 1);
            return ultimoItem;
        }
        return null;
    }

    // Nova função para cancelar tudo e ejetar os itens
    public void CancelarCrafting()
    {
        if (itensNaBancada.Count == 0) 
        {
            Debug.Log("A bancada já está vazia.");
            return;
        }

        foreach (ItemData item in itensNaBancada)
        {
            if (prefabItemGenerico != null && pontoDeSpawn != null)
            {
                // Cria um pequeno espalhamento aleatório para os itens não caírem no mesmo exato pixel
                Vector2 offsetAleatorio = Random.insideUnitCircle * forcaEjeção;
                Vector3 posicaoEjeção = pontoDeSpawn.position + new Vector3(offsetAleatorio.x, offsetAleatorio.y, 0);

                // Instancia o item de volta no mundo
                GameObject itemEjetado = Instantiate(prefabItemGenerico, posicaoEjeção, Quaternion.identity);
                
                ItemColetavel scriptDoItem = itemEjetado.GetComponent<ItemColetavel>();
                if (scriptDoItem != null)
                {
                    scriptDoItem.ConfigurarItem(item);
                }
            }
        }

        // Esvazia a lista da bancada após devolver tudo
        itensNaBancada.Clear();
        Debug.Log("Crafting cancelado. Itens ejetados no chão.");
    }

    private void TentarCraftar()
    {
        foreach (ReceitaData receita in receitas)
        {
            if (VerificarReceita(receita))
            {
                CriarItem(receita.resultado);
                itensNaBancada.Clear(); // Limpa a bancada após criar o item
                return;
            }
        }
    }

    private bool VerificarReceita(ReceitaData receita)
    {
        // Se a quantidade de itens for diferente, já não é essa receita
        if (receita.ingredientes.Count != itensNaBancada.Count) return false;

        // Cria listas temporárias para não apagar a receita original
        List<ItemData> ingredientesNecessarios = new List<ItemData>(receita.ingredientes);
        List<ItemData> itensAtuais = new List<ItemData>(itensNaBancada);

        // Verifica se cada item na bancada existe na lista de ingredientes
        foreach (ItemData itemAtual in itensAtuais)
        {
            if (ingredientesNecessarios.Contains(itemAtual))
            {
                ingredientesNecessarios.Remove(itemAtual); // Remove para funcionar com itens repetidos (ex: Papel + Papel)
            }
            else
            {
                return false; // Tem um item na bancada que não pertence à receita
            }
        }

        // Se removeu todos os ingredientes necessários, a receita está correta
        return ingredientesNecessarios.Count == 0;
    }

    private void CriarItem(ItemData resultado)
    {
        Debug.Log("Craftou: " + resultado.nomeVisivel);
        if (prefabItemGenerico != null && pontoDeSpawn != null)
        {
            GameObject novoObjeto = Instantiate(prefabItemGenerico, pontoDeSpawn.position, Quaternion.identity);
            
            // Assume que você tem o script ItemColetavel como mostrado no PlayerInteract
            ItemColetavel scriptDoItem = novoObjeto.GetComponent<ItemColetavel>();
            if (scriptDoItem != null)
            {
                scriptDoItem.ConfigurarItem(resultado);
            }
        }
    }
}