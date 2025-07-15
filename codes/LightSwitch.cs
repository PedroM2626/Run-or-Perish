using System.Collections;
using UnityEngine;
using TMPro;

public class LightSwitch : MonoBehaviour
{
    public Light targetLight;  // Referência para a luz que será controlada
    public float offRange = 0f;  // Alcance da luz quando está desligada
    public float onRange = 10f;  // Alcance da luz quando está ligada

    public TextMeshProUGUI interactionText;  // Texto exibido para interagir
    public AudioClip switchSound;  // Som a ser tocado ao apertar o botão

    private AudioSource audioSource;  // Fonte de áudio para tocar o som
    private bool isLightOn = false;  // Estado atual da luz (ligada/desligada)
    private bool isPlayerLooking = false;  // Verifica se o jogador está olhando para o botão

    private Transform playerCamera;  // Referência à câmera do jogador
    public float interactionDistance = 4f;  // Distância de interação permitida

    // Configuração da rotação do interruptor
    public Transform switchHandle;  // Objeto que será rotacionado (alça do interruptor)
    private Quaternion offRotation;  // Rotação quando o interruptor está desligado
    private Quaternion onRotation;  // Rotação quando o interruptor está ligado
    public float rotationAngle = 30f;  // Ângulo de rotação (ajuste conforme desejado)
    public float rotationSpeed = 5f;  // Velocidade da rotação da animação

    // Para garantir que a mensagem só seja exibida quando o jogador olhar para o interruptor
    private static LightSwitch focusedSwitch = null;

    void Start()
    {
        // Configura o componente de áudio para tocar o som do interruptor
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = switchSound;
        audioSource.playOnAwake = false;  // Desativa o play automático ao iniciar

        // Esconde o texto de interação inicialmente
        if (interactionText != null)
        {
            interactionText.text = "";
        }

        // Define a luz inicialmente como desligada
        if (targetLight != null)
        {
            targetLight.range = offRange;
        }

        // Encontra a câmera do jogador
        playerCamera = Camera.main.transform;

        // Define as rotações para os estados ligado e desligado para o switchHandle
        if (switchHandle != null)
        {
            offRotation = switchHandle.rotation;
            onRotation = Quaternion.Euler(switchHandle.eulerAngles.x - rotationAngle, switchHandle.eulerAngles.y, switchHandle.eulerAngles.z);
        }
    }

    void Update()
    {
        // Verifica se o jogador está olhando para o interruptor e dentro da distância de interação
        CheckPlayerLookingAtSwitch();

        // Se estiver olhando para o botão e pressionar E, alterna a luz
        if (isPlayerLooking && Input.GetKeyDown(KeyCode.E))
        {
            ToggleLight();
        }
    }

    void CheckPlayerLookingAtSwitch()
    {
        // Calcula a distância entre o jogador e o interruptor
        float distance = Vector3.Distance(playerCamera.position, transform.position);

        // Se estiver dentro da distância de interação
        if (distance <= interactionDistance)
        {
            // Raycast da câmera do jogador em direção ao objeto
            Ray ray = new Ray(playerCamera.position, playerCamera.forward);
            RaycastHit hit;

            // Verifica se o raycast atinge o botão atual
            if (Physics.Raycast(ray, out hit, interactionDistance) && hit.collider.gameObject == gameObject)
            {
                isPlayerLooking = true;

                // Exibe a mensagem de interação apenas se o interruptor não for o foco atual
                if (focusedSwitch != this)
                {
                    if (interactionText != null)
                    {
                        interactionText.text = "Pressione [E] para interagir";
                    }
                    focusedSwitch = this;
                }
                return;
            }
        }

        // Se o jogador não estiver olhando diretamente para o botão ou estiver longe, esconde a mensagem
        if (focusedSwitch == this)
        {
            if (interactionText != null)
            {
                interactionText.text = "";  // Esconde a mensagem de interação
            }
            focusedSwitch = null;
        }
        isPlayerLooking = false;
    }

    void ToggleLight()
    {
        // Alterna o estado da luz
        isLightOn = !isLightOn;

        // Ajusta o alcance da luz com base no estado
        if (targetLight != null)
        {
            targetLight.range = isLightOn ? onRange : offRange;
        }

        // Toca o som ao alternar o interruptor
        if (audioSource != null && switchSound != null)
        {
            audioSource.Play();
        }

        // Esconde o texto de interação após a interação
        if (interactionText != null)
        {
            interactionText.text = "";
        }

        // Inicia a animação de rotação
        StopAllCoroutines();
        if (switchHandle != null)
        {
            StartCoroutine(RotateSwitch(isLightOn));
        }
    }

    private IEnumerator RotateSwitch(bool turnOn)
    {
        // Define a rotação alvo para o switchHandle
        Quaternion targetRotation = turnOn ? onRotation : offRotation;

        // Anima a rotação do switchHandle até atingir a rotação alvo
        while (Quaternion.Angle(switchHandle.rotation, targetRotation) > 0.01f)
        {
            switchHandle.rotation = Quaternion.Slerp(switchHandle.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            yield return null;
        }

        // Garante que a rotação final seja exatamente a rotação alvo
        switchHandle.rotation = targetRotation;
    }
}
