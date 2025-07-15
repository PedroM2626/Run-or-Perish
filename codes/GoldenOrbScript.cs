using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldenOrbScript : MonoBehaviour
{
    private OrbSpawner spawner;

    void Start()
    {
        // Obtém a referência ao objeto Spawner na cena
        spawner = FindObjectOfType<OrbSpawner>();

        // Verifica se o spawner foi encontrado e aumenta o contador de orbes
        if (spawner != null)
        {
            Debug.Log("OrbCount inicial: " + spawner.OrbCount);
        }

        // Destrói o orbe após 13 segundos
        Destroy(gameObject, 13f);
        spawner.SpawnGoldenOrb();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se o objeto colidido possui a tag "Player"
        if (other.CompareTag("Player"))
        {
            // Aumenta a intensidade da neblina
            RenderSettings.fogDensity += 0.001f;

            // Acessa e modifica a variável OrbCount do spawner
            if (spawner != null)
            {
                spawner.OrbCount -= 50;
                Debug.Log("OrbCount atualizado: " + spawner.OrbCount);

                // Chama a função de spawn para criar um novo orbe dourado
                spawner.SpawnGoldenOrb();
            }

            // Destrói o orbe imediatamente
            Destroy(gameObject);
        }
    }
}
