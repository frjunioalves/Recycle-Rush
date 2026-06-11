using UnityEngine;

public class AlavancaEsteira : MonoBehaviour
{
    [Header("Configurações Visuais")]
    public SpriteRenderer spriteRenderer;
    public Sprite spriteLigado;
    public Sprite spriteDesligado;

    private bool estaLigada = true;

    private void Start()
    {
        // Sincroniza o estado inicial com o GameManager se ele existir
        if (GameManager.Instance != null)
        {
            estaLigada = GameManager.Instance.esteiraAtiva;
        }
        AtualizarVisual();
    }

    public void Interagir()
    {
        estaLigada = !estaLigada;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.esteiraAtiva = estaLigada;
        }

        AtualizarVisual();
        Debug.Log("Alavanca " + (estaLigada ? "LIGADA" : "DESLIGADA"));
    }

    private void AtualizarVisual()
    {
        if (spriteRenderer == null) return;

        if (estaLigada)
        {
            spriteRenderer.sprite = spriteLigado;
        }
        else
        {
            spriteRenderer.sprite = spriteDesligado;
        }
    }
}
