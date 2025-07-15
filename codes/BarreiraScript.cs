using UnityEngine;

public class BarreiraScript : MonoBehaviour
{
    public Level_1_script levelScript;  // Referência para o script Level_1_script

    void OnCollisionEnter(Collision collision)
    {
        // Verificar se a colisão é com o jogador
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Colisão detectada com o jogador!");

            if (levelScript != null)
            {
                // Notifica o Level_1_script que o jogador colidiu com a barreira
                levelScript.OnPlayerCollidedWithBarreira();
            }
        }
        else
        {
            Debug.Log("Colisão com outro objeto: " + collision.gameObject.name);
        }
    }
}
