using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public string interactionMessage = "Pressione E para interagir";
    public TextMeshProUGUI globalMessageText;
    public AudioClip lockedSound;
    public static AudioClip KeyCollectedSound;
    public AudioClip openSound;
    private static AudioSource audioSource;

    public Transform doorHandle; // Referência à maçaneta da porta
    public float handleRotationAngle = 30f; // Ângulo de rotação da maçaneta
    public float handleRotationSpeed = 5f; // Velocidade de rotação da maçaneta

    public float interactionDistance = 4f;
    private bool isPlayerNearby = false;
    private Transform playerCamera;

    public bool hasLock = false;
    public string requiredItem = "Key";

    private bool isOpened = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    private static List<string> playerInventory = new List<string>();

    private bool canInteract = true;
    public float interactionCooldown = 1f;

    private bool isAnimating = false;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        if (globalMessageText != null)
        {
            globalMessageText.text = "";
        }

        playerCamera = Camera.main.transform;
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + 90f, transform.eulerAngles.z);
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E) && canInteract && !isAnimating)
        {
            TryOpenDoor();
        }

        CheckPlayerLookingAtObject();
    }

    void CheckPlayerLookingAtObject()
    {
        float distance = Vector3.Distance(playerCamera.position, transform.position);

        if (distance <= interactionDistance)
        {
            Ray ray = new Ray(playerCamera.position, playerCamera.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactionDistance) && hit.collider.gameObject == gameObject)
            {
                isPlayerNearby = true;
                if (globalMessageText != null && string.IsNullOrEmpty(globalMessageText.text))
                {
                    globalMessageText.text = interactionMessage;
                }
                return;
            }
        }

        if (isPlayerNearby)
        {
            if (globalMessageText != null && globalMessageText.text == interactionMessage)
            {
                globalMessageText.text = "";
            }
            isPlayerNearby = false;
        }
    }

    public void TryOpenDoor()
    {
        if (hasLock && !HasItem(requiredItem))
        {
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(lockedSound);
            }
            Debug.Log("A porta está trancada. Você precisa de uma chave para abrir.");
        }
        else
        {
            isOpened = !isOpened;
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(openSound);
                Debug.Log("Porta aberta.");
            }

            StartCoroutine(RotateHandleAndDoor(isOpened));
            StartCoroutine(InteractionCooldown());
        }
    }

    IEnumerator RotateHandleAndDoor(bool open)
{
    isAnimating = true;

    // Animação da maçaneta
    if (doorHandle != null)
    {
        Quaternion handleInitialRotation = doorHandle.localRotation;
        Quaternion handleTargetRotation = handleInitialRotation * Quaternion.Euler(-handleRotationAngle, 0, 0);

        float elapsed = 0f;
        while (elapsed < 1f)
        {
            doorHandle.localRotation = Quaternion.Lerp(handleInitialRotation, handleTargetRotation, elapsed);
            elapsed += Time.deltaTime * handleRotationSpeed;
            yield return null;
        }

        doorHandle.localRotation = handleTargetRotation;
    }

    // Aguarde um curto momento antes de a porta abrir
    yield return new WaitForSeconds(0.3f);

    // Animação da porta
    Quaternion doorTargetRotation = open ? openRotation : closedRotation; // Nome diferente para evitar conflito
    while (Quaternion.Angle(transform.rotation, doorTargetRotation) > 0.1f)
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, doorTargetRotation, 90f * Time.deltaTime);
        yield return null;
    }
    transform.rotation = doorTargetRotation;

    // Reseta a posição da maçaneta após abrir a porta
    if (doorHandle != null)
    {
        Quaternion handleResetRotation = doorHandle.localRotation * Quaternion.Euler(handleRotationAngle, 0, 0);
        doorHandle.localRotation = handleResetRotation;
    }

    isAnimating = false;
}


    private IEnumerator InteractionCooldown()
    {
        canInteract = false;
        yield return new WaitForSeconds(interactionCooldown);
        canInteract = true;
    }

    public static bool HasItem(string itemName)
    {
        return playerInventory.Contains(itemName);
    }

    public static void AddItemToInventory(string item)
    {
        if (!playerInventory.Contains(item))
        {
            audioSource.PlayOneShot(KeyCollectedSound);
            playerInventory.Add(item);
            Debug.Log(item + " adicionado ao inventário.");
        }
    }
}
