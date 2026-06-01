using UnityEngine;
using TMPro; 
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))] // Garante que o objeto terá um SpriteRenderer
public class Lixeira : MonoBehaviour
{
    [Header("Configurações da Lixeira")]
    [Tooltip("Selecione o tipo de lixo que esta lixeira vai aceitar")]
    public CategoriaItem tipoAceito; 
    public int capacidadeMaxima = 20;

    [Tooltip("Arraste o Sprite/Imagem correspondente a esta lixeira aqui")]
    public Sprite spriteDaLixeira; // <-- NOVA VARIÁVEL PARA O VISUAL

    [Header("Interface (UI)")]
    [Tooltip("Arraste o componente TextMeshPro flutuante aqui")]
    public TextMeshPro textoQuantidade; 

    private Stack<ItemData> itensGuardados = new Stack<ItemData>();
    
    public int quantidadeAtual => itensGuardados.Count;

    void Start()
    {
        AtualizarTextoVisível();
    }

    public bool ReceberItem(ItemData itemDepositado)
    {
        if (itensGuardados.Count >= capacidadeMaxima)
        {
            Debug.Log("A lixeira está cheia!");
            return false; 
        }

        if (itemDepositado.categoria == tipoAceito)
        {
            itensGuardados.Push(itemDepositado); 
            AtualizarTextoVisível();
        }
        else
        {
            Debug.Log($"Erro! Lixeira de {tipoAceito} recebeu {itemDepositado.categoria}!");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AplicarPenalidade();
            }
        }

        return true; 
    }

    public ItemData RetirarItem()
    {
        if (itensGuardados.Count > 0)
        {
            ItemData itemRecuperado = itensGuardados.Pop(); 
            AtualizarTextoVisível();
            return itemRecuperado;
        }

        Debug.Log("A lixeira já está vazia!");
        return null;
    }

    private void AtualizarTextoVisível()
    {
        if (textoQuantidade != null)
        {
            textoQuantidade.text = $"{quantidadeAtual}/{capacidadeMaxima}";
        }
    }

    // A MÁGICA ACONTECE AQUI: Atualiza o visual e o texto direto no Editor da Unity!
    private void OnValidate()
    {
        // 1. Atualiza o texto flutuante no editor para você ver como está ficando
        if (textoQuantidade != null)
        {
            textoQuantidade.text = $"0/{capacidadeMaxima}";
        }

        // 2. Pega o SpriteRenderer do objeto e injeta a imagem automaticamente
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && spriteDaLixeira != null)
        {
            sr.sprite = spriteDaLixeira;
        }
    }
}