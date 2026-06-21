using UnityEngine;

public class PontoDeVenda : MonoBehaviour
{
    // Método que o PlayerInteract vai chamar ao apertar E
    public bool ReceberVenda(ItemData itemParaVender)
    {
        // Garante que o item realmente existe e tem valor
        if (itemParaVender != null && itemParaVender.valorVenda > 0)
        {
            // Adiciona o valor no GameManager
            GameManager.Instance.AdicionarDinheiro((int)itemParaVender.valorVenda);
            
            // Opcional: Atualiza o inventário simples do GameManager
            if (GameManager.Instance.itensNaMao > 0)
            {
                GameManager.Instance.itensNaMao--;
            }

            Debug.Log($"Vendido! {itemParaVender.nomeVisivel} por R$ {itemParaVender.valorVenda}");
            
            // Retorna true para o PlayerInteract saber que pode limpar a mão do jogador
            return true; 
        }

        Debug.Log("Este item não pode ser vendido ou não tem valor.");
        return false; // Retorna false para o jogador continuar segurando
    }
}