using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Para manipular o TextMeshProUGUI

public class ContadorObjetos : MonoBehaviour
{
    public TextMeshProUGUI contadorText; // Referência ao TextMeshProUGUI para exibir a contagem
    public static int objetoCount;

    void Start()
    {
        AtualizarContador();
    }

    void Update()
    {
        AtualizarContador();
    }

    void AtualizarContador()
    {
        // Zera a contagem e faz a verificação em todos os filhos
        objetoCount = 0;
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Interactable"))
            {
                objetoCount++;
            }
        }

        // Atualiza o TextMeshProUGUI com o número de objetos, ou deixa vazio se objetoCount for 0
        if (contadorText != null)
        {
            if (objetoCount == 0)
            {
                contadorText.text = "";  // Deixa o texto vazio se não houver objetos
            }
            else
            {
                contadorText.text = "Objetos Restantes: " + objetoCount;  // Exibe a contagem quando maior que 0
            }
        }
    }
}
