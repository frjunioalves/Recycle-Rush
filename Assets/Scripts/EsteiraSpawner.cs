using UnityEngine;

public class EsteiraSpawner : MonoBehaviour
{
    [Header("Configurações de Spawn")]
    public GameObject[] itensPrefabs; // Arraste os prefabs dos itens aqui no Inspector
    public float tempoEntreSpawns = 2f; // Tempo em segundos entre cada spawn
    
    private float timer;

    void Update()
    {
        // Incrementa o timer com o tempo que passou desde o último frame
        timer += Time.deltaTime;

        // Verifica se é hora de instanciar um novo item
        if (timer >= tempoEntreSpawns)
        {
            InstanciarItem();
            timer = 0f; // Reseta o timer
        }
    }

    void InstanciarItem()
    {
        // Previne erros caso a lista de prefabs esteja vazia
        if (itensPrefabs.Length == 0)
        {
            Debug.LogWarning("Nenhum prefab foi atribuído ao EsteiraSpawner!");
            return;
        }

        // Escolhe um item aleatório do array
        int indexAleatorio = Random.Range(0, itensPrefabs.Length);
        GameObject itemEscolhido = itensPrefabs[indexAleatorio];

        // Instancia o item na posição e rotação do Spawner
        Instantiate(itemEscolhido, transform.position, transform.rotation);
    }
}