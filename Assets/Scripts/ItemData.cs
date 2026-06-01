using UnityEngine;
using System; 

// 1. ATUALIZAÇÃO: Novas categorias baseadas na coleta seletiva real
public enum CategoriaItem { Papel, Plastico, Metal, Vidro, Organico, NaoReciclavel }

[CreateAssetMenu(fileName = "NovoItem", menuName = "Sistema de Itens/Item")]
public class ItemData : ScriptableObject
{
    [SerializeField] private string idItemAutomatico; 
    public string IdItem => idItemAutomatico; 

    [Header("Informações Base")]
    public string nomeVisivel; 
    
    [Tooltip("Selecione em qual lixeira este item deve ser jogado")]
    public CategoriaItem categoria; // Agora vai mostrar Papel, Plastico, Metal, etc.

    [Header("Visuais")]
    public Sprite iconeParaUI; 

    [Header("Economia")]
    public float valorVenda; 

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(idItemAutomatico))
        {
            idItemAutomatico = Guid.NewGuid().ToString();
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            #endif
        }
    }
}