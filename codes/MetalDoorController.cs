using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class MetalDoorController : MonoBehaviour
{
    public PlayableDirector animator;
    // Referências aos objetos das portas
    public GameObject leftDoor; // Parte esquerda da porta
    public GameObject rightDoor; // Parte direita da porta

    // Câmeras
    public Camera playerCamera; // Câmera do jogador
    public Camera doorCamera; // Câmera da porta

    // Definição das rotações
    private Quaternion leftDoorClosedRotation;
    private Quaternion rightDoorClosedRotation;
    private Quaternion leftDoorOpenRotation;
    private Quaternion rightDoorOpenRotation;

    // Variável para definir a duração da animação
    public float animationDuration = 1f;

    // Multiplicador para ajustar a duração da animação
    public float timeMultiplier = 2f;

    // Estado da porta
    private bool isOpened = false;

    // Nova variável para definir o tempo que a câmera da porta ficará ativa
    public float cameraActiveDuration = 1f; // Tempo que a câmera da porta ficará ativa

    void Start()
    {
        // Define as rotações fechadas e abertas das portas
        leftDoorClosedRotation = leftDoor.transform.rotation;
        rightDoorClosedRotation = rightDoor.transform.rotation;

        // As portas vão girar 90 graus (para abrir)
        leftDoorOpenRotation = Quaternion.Euler(leftDoor.transform.eulerAngles.x, leftDoor.transform.eulerAngles.y - 90f, leftDoor.transform.eulerAngles.z); // Parte esquerda gira anti-horário
        rightDoorOpenRotation = Quaternion.Euler(rightDoor.transform.eulerAngles.x, rightDoor.transform.eulerAngles.y + 90f, rightDoor.transform.eulerAngles.z); // Parte direita gira horário

        // Garante que a câmera da porta está desativada inicialmente
        if (doorCamera != null)
        {
            doorCamera.gameObject.SetActive(false);
        }
    }

    public void OpenDoor()
    {
        // Só abre se a porta não estiver aberta
        if (!isOpened)
        {
            isOpened = true;
            animator.Play();
            StartCoroutine(RotateDoors(true)); // Inicia a animação de abrir
        }
    }

    public void CloseDoor()
    {
        // Só fecha se a porta estiver aberta
        if (isOpened)
        {
            isOpened = false;
            StartCoroutine(RotateDoors(false)); // Inicia a animação de fechar
        }
    }

    IEnumerator RotateDoors(bool open)
    {
        // Define as rotações alvo dependendo do estado (aberto ou fechado)
        Quaternion leftTargetRotation = open ? leftDoorOpenRotation : leftDoorClosedRotation;
        Quaternion rightTargetRotation = open ? rightDoorOpenRotation : rightDoorClosedRotation;

        // Ativa a câmera da porta e desativa a câmera do jogador imediatamente
        if (doorCamera != null && playerCamera != null)
        {
            doorCamera.gameObject.SetActive(true);
            playerCamera.gameObject.SetActive(false);
        }

        // Inicia o timer da câmera em paralelo com a animação
        StartCoroutine(CameraTimer());

        // Tempo de animação ajustado pelo multiplicador
        float adjustedDuration = animationDuration * timeMultiplier;
        float elapsedTime = 0f;

        // Rotaciona as portas até atingirem a rotação alvo
        while (elapsedTime < adjustedDuration)
        {
            // A rotação das portas é ajustada conforme o tempo
            leftDoor.transform.rotation = Quaternion.Slerp(leftDoor.transform.rotation, leftTargetRotation, elapsedTime / adjustedDuration);
            rightDoor.transform.rotation = Quaternion.Slerp(rightDoor.transform.rotation, rightTargetRotation, elapsedTime / adjustedDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Garante que a rotação final seja exatamente a rotação alvo
        leftDoor.transform.rotation = leftTargetRotation;
        rightDoor.transform.rotation = rightTargetRotation;
    }

    IEnumerator CameraTimer()
    {
        // Aguarda o tempo da câmera da porta ficar ativa
        yield return new WaitForSeconds(cameraActiveDuration);

        // Restaura a câmera do jogador e desativa a câmera da porta
        if (doorCamera != null && playerCamera != null)
        {
            doorCamera.gameObject.SetActive(false);
            playerCamera.gameObject.SetActive(true);
        }
    }

}
