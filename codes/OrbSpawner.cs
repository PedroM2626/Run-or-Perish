using UnityEngine;

public class OrbSpawner : MonoBehaviour
{
    // Array contendo as posições dos objetos de spawn
    public Transform[] spawnPoints;

    // Prefabs dos orbes
    public GameObject orbPrefab;
    public GameObject goldenOrbPrefab;
    public GameObject brownOrbPrefab;

    public int OrbCount = 1000;

    void Start()
    {
        // Exemplo de chamada das funções de spawn
        SpawnOrb();
        SpawnGoldenOrb();
        SpawnBrownkOrb();
    }

    // Função para spawnar o orbe comum
    public void SpawnOrb()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("Nenhum ponto de spawn definido!");
            return;
        }

        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];

        Instantiate(orbPrefab, spawnPoint.position, spawnPoint.rotation);
        Debug.Log("Orbe spawnado em: " + spawnPoint.position);
        
    }

    // Função para spawnar o orbe dourado
    public void SpawnGoldenOrb()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("Nenhum ponto de spawn definido!");
            return;
        }

        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];

        if (OrbCount <= 900 && OrbCount <= 0) {
            Instantiate(goldenOrbPrefab, spawnPoint.position, spawnPoint.rotation);
            Debug.Log("Orbe dourado spawnado em: " + spawnPoint.position);
            }
    }

    // Função para spawnar o orbe rosa
    public void SpawnBrownkOrb()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("Nenhum ponto de spawn definido!");
            return;
        }

        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];

        if (OrbCount <= 750 && OrbCount <= 0) {
            Instantiate(goldenOrbPrefab, spawnPoint.position, spawnPoint.rotation);
            Debug.Log("Orbe dourado spawnado em: " + spawnPoint.position);
            }
    }
}
