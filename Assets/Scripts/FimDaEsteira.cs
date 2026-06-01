using UnityEngine;

public class FimDaEsteira : MonoBehaviour
{
    // AQUI ESTÁ O SEGREDO: Tem que ser OnTriggerEnter2D e Collider2D
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se quem bateu tem a Tag "Item"
        if (other.CompareTag("Item"))
        {
            Destroy(other.gameObject);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.AplicarPenalidade();
            }
        }
    }
}