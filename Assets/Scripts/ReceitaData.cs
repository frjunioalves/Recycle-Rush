using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NovaReceita", menuName = "Sistema de Itens/Receita")]
public class ReceitaData : ScriptableObject
{
    [Header("Ingredientes Necessários")]
    [Tooltip("Coloque aqui os itens que formam a receita. Ex: 2 papéis")]
    public List<ItemData> ingredientes;

    [Header("Resultado Final")]
    [Tooltip("O item que será criado (Ex: Caderno Reciclado)")]
    public ItemData resultado;
}