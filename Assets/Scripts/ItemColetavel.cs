using UnityEngine;

// RequireComponent garante que a Unity não vai deixar esse objeto existir sem um SpriteRenderer
[RequireComponent(typeof(SpriteRenderer))]
public class ItemColetavel : MonoBehaviour
{
    [Header("Dados deste Item")]
    public ItemData dadosDoItem; 

    // O Player vai chamar essa função quando "cuspir" o item no chão
    public void ConfigurarItem(ItemData novosDados)
    {
        dadosDoItem = novosDados;
        AtualizarVisual();
    }

    // A mágica acontece aqui: OnValidate roda sozinho sempre que você altera algo lá no Inspector da Unity!
    private void OnValidate()
    {
        AtualizarVisual();
    }

    // Função que puxa a imagem do arquivo de dados e cola no SpriteRenderer do objeto
    private void AtualizarVisual()
    {
        if (dadosDoItem != null && dadosDoItem.iconeParaUI != null)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = dadosDoItem.iconeParaUI;
            }
        }
    }
}