using UnityEngine;

// Obriga a Unity a colocar um CircleCollider2D se você esquecer
[RequireComponent(typeof(CircleCollider2D))] 
public class DestaqueObjeto : MonoBehaviour
{
    [Header("1. Mudança de Cor (Opcional)")]
    [Tooltip("Arraste o SpriteRenderer do próprio objeto aqui")]
    public SpriteRenderer spriteDoObjeto;
    public Color corEmDestaque = new Color(1.2f, 1.2f, 1.2f); // Fica mais brilhante (ou escolha amarelo, etc)
    private Color corOriginal;

    [Header("2. Ícone de Interação (Opcional)")]
    [Tooltip("Pode ser uma seta, borda extra ou a tecla 'E' que você deixou como filho deste objeto")]
    public GameObject iconeOuBordaExtra; 

    private void Start()
    {
        // Salva a cor original para não estragar a arte quando o player sair de perto
        if (spriteDoObjeto != null)
        {
            corOriginal = spriteDoObjeto.color;
        }

        // Começa com o ícone desligado
        if (iconeOuBordaExtra != null)
        {
            iconeOuBordaExtra.SetActive(false);
        }

        // Transforma o colisor em uma "área de detecção" invisível
        GetComponent<CircleCollider2D>().isTrigger = true;
    }

    // Quando algo entra na área
    private void OnTriggerEnter2D(Collider2D outro)
    {
        // Se quem chegou perto foi o Player, liga o destaque
        if (outro.CompareTag("Player"))
        {
            MudarDestaque(true);
        }
    }

    // Quando algo sai da área
    private void OnTriggerExit2D(Collider2D outro)
    {
        // Se o Player foi embora, desliga o destaque
        if (outro.CompareTag("Player"))
        {
            MudarDestaque(false);
        }
    }

    private void MudarDestaque(bool ativar)
    {
        // Muda a cor
        if (spriteDoObjeto != null)
        {
            spriteDoObjeto.color = ativar ? corEmDestaque : corOriginal;
        }

        // Ativa/Desativa o ícone
        if (iconeOuBordaExtra != null)
        {
            iconeOuBordaExtra.SetActive(ativar);
        }
    }
}