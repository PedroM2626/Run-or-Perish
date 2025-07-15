using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public bool isPageDLYA;
    public string interactionMessage = "Pressione [E] para interagir"; // Mensagem exibida para cada objeto
    public TextMeshProUGUI globalMessageText; // Texto global para exibir a mensagem de interação
    public AudioClip interactionSound; // Som a ser tocado ao interagir
    private AudioSource audioSource; // Fonte de áudio para tocar o som

    public float interactionDistance = 4f; // Distância de interação permitida
    private bool isPlayerNearby = false; // Verifica se o jogador está próximo e olhando para o objeto

    private Transform playerCamera; // Referência à câmera do jogador
    private static InteractableObject focusedObject = null; // Objeto atualmente em foco

    // Referência ao inventário do jogador
    private static List<string> playerInventory = new List<string>();

    // Variável para controlar o delay de interação
    private bool canInteract = true; // Verifica se o jogador pode interagir
    public float interactionCooldown = 1f; // Tempo de espera (1 segundo)

    public GameObject targetObject; // Objeto cuja opacidade será alterada

    public static float dificult;

    void Start()
    {
        // Configura a fonte de áudio
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = interactionSound;
        audioSource.playOnAwake = false; // Desativa o play automático ao iniciar

        // Esconde a mensagem global no início
        if (globalMessageText != null)
        {
            globalMessageText.text = "";
        }

        // Encontra a câmera do jogador
        playerCamera = Camera.main.transform;
    }

    void Update()
    {
        // Verifica se o jogador está próximo e pressionou a tecla E
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E) && canInteract)
        {
            if (isPageDLYA)
            {
                ChangeOpacity(targetObject, 1.0f); // Aumenta a transparência do objeto alvo em 1
            }
            Interact(); // Chama a função de interação
        }

        // Verifica se o jogador está olhando para o objeto e dentro da distância de interação
        CheckPlayerLookingAtObject();
    }

    void CheckPlayerLookingAtObject()
    {
        // Calcula a distância entre o jogador e o objeto
        float distance = Vector3.Distance(playerCamera.position, transform.position);

        // Se estiver dentro da distância de interação
        if (distance <= interactionDistance)
        {
            // Raycast da câmera do jogador em direção ao objeto
            Ray ray = new Ray(playerCamera.position, playerCamera.forward);
            RaycastHit hit;

            // Verifica se o raycast atinge o objeto atual
            if (Physics.Raycast(ray, out hit, interactionDistance) && hit.collider.gameObject == gameObject)
            {
                isPlayerNearby = true;

                // Define este objeto como o foco atual, atualiza o texto e retorna
                if (focusedObject != this)
                {
                    if (globalMessageText != null)
                    {
                        globalMessageText.text = interactionMessage; // Mostra a mensagem global para este objeto
                    }
                    focusedObject = this;
                }
                return;
            }
        }

        // Se o jogador não estiver olhando diretamente para o objeto ou estiver longe, escondemos a mensagem
        if (focusedObject == this)
        {
            if (globalMessageText != null)
            {
                globalMessageText.text = ""; // Esconde a mensagem global
            }
            focusedObject = null;
        }
        isPlayerNearby = false;
    }

    void Interact()
    {
        // Se o objeto tem a tag "Key", adiciona-o ao inventário do jogador
        if (CompareTag("Key"))
        {
            playerInventory.Add("Key");
            Debug.Log("Chave adicionada ao inventário.");
        }

        // Reproduz o som e esconde a mensagem de interação
        if (globalMessageText != null)
        {
            globalMessageText.text = "";
        }
        dificult++;
        audioSource.Play();
        Destroy(gameObject, interactionSound.length); // Remove o objeto após o som

        // Desativa a interação por 1 segundo
        StartCoroutine(InteractionCooldown());
    }

    void ChangeOpacity(GameObject target, float opacity)
    {
        Renderer renderer = target.GetComponent<Renderer>();
        if (renderer != null)
        {
            Color color = renderer.material.color;
            color.a = Mathf.Clamp(color.a + opacity, 0, 1); // Aumenta a opacidade em 1, limitando entre 0 e 1
            renderer.material.color = color;
        }
    }

    // Função que controla o cooldown de interação
    private IEnumerator InteractionCooldown()
    {
        canInteract = false; // Impede interação
        yield return new WaitForSeconds(interactionCooldown); // Espera o tempo definido
        canInteract = true; // Permite a interação novamente
    }

    // Função pública para verificar o inventário do jogador
    public static bool HasItem(string itemName)
    {
        return playerInventory.Contains(itemName);
    }
}
