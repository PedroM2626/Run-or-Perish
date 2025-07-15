using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Level_1_script : MonoBehaviour
{
    public bool shouldBlink;
    public float blinkInterval = 0.5f;
    public AudioClip doorOpenSound; // Som da porta
    private AudioSource audioSource; // AudioSource da porta
    public bool isOpened;

    public MetalDoorController metalDoorController; // Referência ao script MetalDoorController

    public AudioClip rageSound; // Som para o RageMode
    public AudioClip preRageSound; // Som antes do RageMode
    public Material[] monsterMaterials; // Materiais do monstro para alterar as cores
    public Monster monster; // Referência ao objeto do monstro

    public ParticleSystem rageEffect; // Sistema de partículas para o efeito do RageMode

    public TextMeshProUGUI textoRage; // Texto a ser manipulado
    public RawImage rageImage; // Imagem a ser exibida
    public float tremorMagnitude = 10f; // Magnitude do tremor
    public float tremorSpeed = 0.05f; // Velocidade do tremor

    private Color defaultFogColor;
    private float defaultFogDensity;

    public Color rageFogColor = Color.red; // Cor do fog no modo Rage
    public float rageFogDensity = 0.1f; // Densidade do fog no modo Rage

    private Color[] defaultMonsterColors;

    // Configurações de tremor da câmera
    public Camera playerCamera;
    public float cameraShakeMagnitude = 0.3f; // Intensidade do tremor
    public float cameraShakeSpeed = 0.1f; // Frequência do tremor

    private Vector3 originalCameraPosition;

    // Variáveis para a tela preta, mensagem e botões
    public GameObject blackScreen; // Tela preta
    public GameObject winMessage; // Mensagem de sucesso
    public Button retryButton; // Botão de tentar novamente
    public Button mainMenuButton; // Botão de ir para o menu principal
    public Button exitButton; // Botão de sair do jogo

    // Variável para a barreira
    public GameObject barreira; // GameObject que representa a barreira

    public AudioClip happyAudioClip; // Som de congratulações

    private bool hasCollidedWithBarrier = false; // Flag para verificar se o jogador já colidiu com a barreira
    Light[] allLights; 

    void Start()
    {
        allLights = FindObjectsOfType<Light>();
        isOpened = false;

        audioSource = gameObject.AddComponent<AudioSource>();
        if (doorOpenSound != null)
        {
            audioSource.clip = doorOpenSound;
        }

        defaultFogColor = RenderSettings.fogColor;
        defaultFogDensity = RenderSettings.fogDensity;

        defaultMonsterColors = new Color[monsterMaterials.Length];
        for (int i = 0; i < monsterMaterials.Length; i++)
        {
            defaultMonsterColors[i] = monsterMaterials[i].color;
        }

        if (textoRage != null)
        {
            textoRage.gameObject.SetActive(false);
        }

        if (rageImage != null)
        {
            rageImage.gameObject.SetActive(false);
        }

        if (rageEffect != null)
        {
            rageEffect.Stop(); // Certifique-se de que o efeito não comece ativo
        }

        if (playerCamera != null)
        {
            originalCameraPosition = playerCamera.transform.localPosition;
        }

        // Desativar elementos da tela de vitória
        blackScreen.SetActive(false);
        winMessage.SetActive(false);
        retryButton.gameObject.SetActive(false);
        mainMenuButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);
    }

    void Update()
    {
        if (PauseResume.GamePaused || hasCollidedWithBarrier)
        {
            // Se o jogo estiver pausado ou o jogador já colidiu com a barreira, impede qualquer outra ação
            return;
        }

        if (ContadorObjetos.objetoCount == 0 && !isOpened)
        {
            isOpened = true;
            OpenDoor();
        }
    }

    public void OpenDoor()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            metalDoorController.OpenDoor();
            audioSource.Play();
            StartCoroutine(WaitForOpenSoundToEnd());
        }

        if (metalDoorController != null)
        {
            metalDoorController.OpenDoor();
        }
        else
        {
            Debug.LogWarning("MetalDoorController não está atribuído!");
        }
    }

    private IEnumerator WaitForOpenSoundToEnd()
    {
        if (audioSource != null)
        {
            yield return new WaitWhile(() => audioSource.isPlaying);
        }

        if (preRageSound != null)
        {
            // Toca o áudio do ponto de audição do jogador
            AudioSource playerAudioSource = playerCamera.GetComponent<AudioSource>();
            if (playerAudioSource == null)
            {
                playerAudioSource = playerCamera.gameObject.AddComponent<AudioSource>();
            }

            playerAudioSource.clip = preRageSound;
            playerAudioSource.loop = false;
            playerAudioSource.Play();

            yield return new WaitForSeconds(preRageSound.length);
        }

        // Obtenha todas as luzes na cena
        Light[] allLights = FindObjectsOfType<Light>(); // Percorra todas as luzes e altere a cor 
        foreach (Light light in allLights) 
            { 
                light.color = Color.red;
            }

        barreira.SetActive(true);

        RageMode();
    }

    void RageMode()
{
    // Tocar o áudio do RageMode
    if (rageSound != null && monster != null)
    {
        AudioSource monsterAudioSource = monster.gameObject.GetComponent<AudioSource>();
        if (monsterAudioSource == null)
        {
            monsterAudioSource = monster.gameObject.AddComponent<AudioSource>();
        }

        monsterAudioSource.clip = rageSound;
        monsterAudioSource.loop = true;
        monsterAudioSource.Play();
    }

    // Alterar velocidades do monstro
    if (monster != null)
    {
        monster.walkSpeed *= 3;
        monster.runSpeed *= 3;
    }

    // Ativar efeito de partículas de RageMode
    if (rageEffect != null)
    {
        rageEffect.Play(); // Ativa o efeito de partículas
    }

    // Iniciar a transição para o modo Rage
    StartCoroutine(SmoothTransitionToRage());
    StartCoroutine(ShakeCamera());
    monster.chaseDistance = 300;
    // Ativar a exibição da mensagem e imagem de Rage
    StartCoroutine(ShowRageTextAndImage()); // Garantir que o texto e imagem sejam ativados
    if (shouldBlink)
    {
        StartCoroutine(BlinkLights());
    }
    }

    private IEnumerator SmoothTransitionToRage()
    {
        float duration = 2f; // Tempo de transição para o modo fúria
        float elapsed = 0f;

        Debug.Log("Iniciando transição para RageMode...");

        while (elapsed < duration)
        {
            RenderSettings.fogColor = Color.Lerp(defaultFogColor, rageFogColor, elapsed / duration);
            RenderSettings.fogDensity = Mathf.Lerp(defaultFogDensity, rageFogDensity, elapsed / duration);

            for (int i = 0; i < monsterMaterials.Length; i++)
            {
                if (monsterMaterials[i] != null)
                {
                    monsterMaterials[i].color = Color.Lerp(defaultMonsterColors[i], Color.black, elapsed / duration);
                }
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        RenderSettings.fogColor = rageFogColor;
        RenderSettings.fogDensity = rageFogDensity;

        for (int i = 0; i < monsterMaterials.Length; i++)
        {
            if (monsterMaterials[i] != null)
            {
                monsterMaterials[i].color = Color.black;
            }
        }

        Debug.Log("Transição para RageMode concluída!");
    }

    private IEnumerator ShakeCamera()
    {
        while (true) // Continuará enquanto o modo Rage estiver ativo
        {
            if (playerCamera != null)
            {
                Vector3 randomOffset = Random.insideUnitSphere * cameraShakeMagnitude;
                playerCamera.transform.localPosition = originalCameraPosition + randomOffset;
            }

            yield return new WaitForSeconds(cameraShakeSpeed);
        }
    }

    private IEnumerator ShowRageTextAndImage()
{
    if (textoRage != null && rageImage != null)
    {
        // Ativar o texto e imagem de rage
        textoRage.gameObject.SetActive(true);
        rageImage.gameObject.SetActive(true);

        // Aplicar o tremor na tela
        float timer = 0f;
        bool isVisible = true;

        Vector3 originalPos = textoRage.transform.position;

        while (timer < 3f) // Exibe por 3 segundos
        {
            // Tremor na posição da mensagem e imagem
            textoRage.transform.position = originalPos + Random.insideUnitSphere * tremorMagnitude;
            rageImage.transform.position = originalPos + Random.insideUnitSphere * tremorMagnitude;

            textoRage.color = isVisible ? Color.red : Color.white;
            rageImage.color = isVisible ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0);

            isVisible = !isVisible;

            timer += tremorSpeed;
            yield return new WaitForSeconds(tremorSpeed);
        }

        // Após o tremor, desativar a imagem e o texto
        textoRage.gameObject.SetActive(false);
        rageImage.gameObject.SetActive(false);
    }
}

    private IEnumerator TremorEAlternanciaDeCor()
    {
        float timer = 0f;
        bool isRed = false;

        Vector3 originalPos = textoRage.transform.position;

        while (timer < 3f) // Efeito de tremor por 3 segundos
        {
            textoRage.transform.position = originalPos + Random.insideUnitSphere * tremorMagnitude;
            rageImage.transform.position = originalPos + Random.insideUnitSphere * tremorMagnitude;

            textoRage.color = isRed ? Color.red : Color.white;
            rageImage.color = isRed ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0);

            isRed = !isRed;

            timer += tremorSpeed;
            yield return new WaitForSeconds(tremorSpeed);
        }

        textoRage.gameObject.SetActive(false);
        rageImage.gameObject.SetActive(false);
    }

    public void OnPlayerCollidedWithBarreira()
    {
        // Aqui você pode chamar o método que vai mostrar a tela de fim de jogo
        StartCoroutine(ShowEndScene());
    }

    public IEnumerator ShowEndScene()
    {
        monster.StopAllAudioInScene();
        Destroy(monster);
        // Mostrar a tela preta e os botões
        blackScreen.SetActive(true);
        winMessage.SetActive(true);

        retryButton.gameObject.SetActive(true);
        mainMenuButton.gameObject.SetActive(true);
        exitButton.gameObject.SetActive(true);

        // Liberar o mouse para interação
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Parar todos os áudios e tocar o áudio de vitória
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audio in allAudioSources)
        {
            audio.Stop();
        }

        if (happyAudioClip != null)
        {
            AudioSource.PlayClipAtPoint(happyAudioClip, transform.position);
        }

        // Impedir o acesso à tela de pause
        PauseResume.GamePaused = false;

        yield return null;
    }

    private IEnumerator BlinkLights()
{ 
	while (true) 
	{ 
		SetAllLightsActive(false); yield return new
        WaitForSeconds(blinkInterval);
        SetAllLightsActive(true); yield return new
        WaitForSeconds(blinkInterval); 
	} 
}
    private void SetAllLightsActive(bool state) 
    { 
        foreach (Light light in allLights) 
        { 
            light.enabled = state; 
        }
}
}
