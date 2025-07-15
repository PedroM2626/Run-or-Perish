using UnityEngine;

public class BrownOrbScript : MonoBehaviour
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

        spawner.SpawnBrownkOrb();
        // Destrói o orbe após 3 segundos
        Destroy(gameObject, 8f);
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
                spawner.OrbCount -= 150;
                Debug.Log("OrbCount atualizado: " + spawner.OrbCount);
            }

            // Destrói o orbe imediatamente
            Destroy(gameObject);
            spawner.SpawnBrownkOrb();
        }
    }
}
