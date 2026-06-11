using UnityEngine;

public class EsteiraSpawner : MonoBehaviour
{
    [Header("Configurações de Spawn")]
    [Tooltip("Arraste o seu único prefab base (ex: lixo_prefab) aqui")]
    public GameObject prefabItemGenerico; 
    
    [Tooltip("Arraste os arquivos .asset dos lixos (garrafaPet, papelao, etc) aqui")]
    public ItemData[] lixosDisponiveis; 
    
    private float timer;
    
    void Update()
    {
        // Só funciona se a esteira estiver ativa no GameManager
        if (GameManager.Instance != null && !GameManager.Instance.esteiraAtiva) return;

        timer += Time.deltaTime;

        // Puxa o intervalo de spawn do GameManager
        float intervalo = GameManager.Instance != null ? GameManager.Instance.intervaloSpawnGlobal : 2f;

        if (timer >= intervalo)
        {
            InstanciarItem();
            timer = 0f; 
        }
    }

    void InstanciarItem()
    {
        // Verifica se configurou tudo certinho
        if (prefabItemGenerico == null || lixosDisponiveis.Length == 0)
        {
            Debug.LogWarning("Falta configurar o prefab ou a lista de lixos no Spawner!");
            return;
        }

        // 1. Cria a "casca" vazia (o seu prefab genérico)
        GameObject novoItem = Instantiate(prefabItemGenerico, transform.position, transform.rotation);

        // 2. Sorteia qual será a "alma" do lixo (os dados)
        int indexAleatorio = Random.Range(0, lixosDisponiveis.Length);
        ItemData dadoSorteado = lixosDisponiveis[indexAleatorio];

        // 3. Pega o script ItemColetavel do objeto recém-criado e injeta os dados sorteados
        ItemColetavel coletavel = novoItem.GetComponent<ItemColetavel>();
        if (coletavel != null)
        {
            coletavel.ConfigurarItem(dadoSorteado); // Isso vai chamar o AtualizarVisual() que você criou!
        }
    }
}