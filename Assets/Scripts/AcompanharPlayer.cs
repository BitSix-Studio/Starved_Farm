using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcompanharPlayer : MonoBehaviour
{
    [Header("Configurações")]
    public Transform jogador; // Referência ao jogador
    public Vector3 offset = new Vector3(0, 1.5f, 0); // Posição fixa relativa ao jogador

    private RectTransform imagemRectTransform;

    void Start()
    {
        // Obtém o RectTransform da imagem (se estiver no Canvas)
        imagemRectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        // Converte a posição do jogador no mundo para a posição da tela
        Vector3 posicaoNaTela = Camera.main.WorldToScreenPoint(jogador.position + offset);

        // Atualiza a posição da imagem (UI)
        imagemRectTransform.position = posicaoNaTela;
    }
}
