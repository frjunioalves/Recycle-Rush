using UnityEngine;

public enum DirecaoEsteira { Direita, Esquerda, Cima, Baixo }

public class MoverNaEsteira : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public DirecaoEsteira direcaoDoMovimento = DirecaoEsteira.Direita;

    private void OnTriggerStay2D(Collider2D other)
    {
        // Só move se a esteira estiver ativa no GameManager
        if (GameManager.Instance != null && !GameManager.Instance.esteiraAtiva) return;

        // Verifica se o objeto que entrou no gatilho tem a tag "Item"
        if (other.CompareTag("Item"))
        {
            Vector3 direcao = Vector3.right;

            switch (direcaoDoMovimento)
            {
                case DirecaoEsteira.Direita:  direcao = Vector3.right; break;
                case DirecaoEsteira.Esquerda: direcao = Vector3.left;  break;
                case DirecaoEsteira.Cima:     direcao = Vector3.up;    break;
                case DirecaoEsteira.Baixo:    direcao = Vector3.down;  break;
            }
            
            // Puxa a velocidade diretamente do GameManager
            float velocidade = GameManager.Instance != null ? GameManager.Instance.velocidadeGlobalEsteira : 2f;

            // Move o item que está em cima da esteira
            other.transform.Translate(direcao * velocidade * Time.deltaTime, Space.World);
        }
    }
}