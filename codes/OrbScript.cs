using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbScript : MonoBehaviour
{
    private OrbSpawner spawner;

    void Start()
    {
        // Obtém a referência ao objeto Spawner na cena
        spawner = FindObjectOfType<OrbSpawner>();
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
                spawner.OrbCount -= 5;
                Debug.Log("OrbCount atualizado: " + spawner.OrbCount);

                // Chama a função de spawn para criar um novo orbe comum
                spawner.SpawnOrb();
            }

            // Destrói o orbe imediatamente
            Destroy(gameObject);
        }
    }
}
