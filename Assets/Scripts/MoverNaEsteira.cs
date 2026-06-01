using UnityEngine;

public class MoverNaEsteira : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public float velocidade = 5f;
    public bool moverParaEsquerda = false; 

    [Header("Controle de Estado")]
    // Essa variável diz se o item deve ou não se mover. 
    // Ela começa como 'true' para os itens que nascem na esteira.
    public bool estaNaEsteira = true; 

    void Update()
    {
        // O código de movimento agora só funciona SE estaNaEsteira for verdadeiro
        if (estaNaEsteira)
        {
            Vector3 direcao = moverParaEsquerda ? Vector3.left : Vector3.right;
            transform.Translate(direcao * velocidade * Time.deltaTime, Space.World);
        }
    }

    // Você pode chamar essa função no seu script de interação quando o jogador pegar o item
    public void PegarItem()
    {
        estaNaEsteira = false;
    }

    // Caso o jogador solte o item de volta na esteira, você pode chamar essa
    public void DevolverParaEsteira()
    {
        estaNaEsteira = true;
    }
}