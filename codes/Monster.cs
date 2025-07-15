using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

public class Monster : MonoBehaviour
{
    public Transform player;
    public float runSpeed = 10f;
    public float walkSpeed = 5f;
    public float chaseDistance = 20f;
    public float stamina = 100f;
    public float staminaDrainRate = 5f;
    public float staminaRegenRate = 2f;

    public RawImage blackscreen;
    public AudioClip chaseMusicClip;
    private AudioSource chaseAudioSource;
    public AudioClip niggasoundClip;
    private AudioSource niggaAudioSource;
    public GameObject pausescreen;
    public TextMeshProUGUI GameOverText;
    public Button retryButton;
    public Button exitButton;
    public float shakeDuration = 1.0f;
    public float fadeDuration = 2.0f;

    public RawImage jumpscareImage;
    public AudioClip jumpscareAudioClip;

    private NavMeshAgent agent;
    private Animator animator;
    private bool isChasing;
    private bool isGameOver = false;
    private bool isPatrolling;

    public RawImage fadeImage;
    public RawImage staticaEffect;
    public float maxDistance = 20f;
    public float maxAlpha = 0.8f;
    public float maxStaticaAlpha = 0.8f;
    private float currentDistance;

    public Vector3 patrolCenter;
    public float patrolRadius = 20f;
    public float patrolPauseDuration = 2f;

    public Level_1_script level;

    public AudioClip startChaseAudioClip; // Áudio a ser tocado ao começar a perseguição
    private bool hasTriggeredChaseEffect = false; // Controle para evitar repetir o efeito
    public Transform cameraTransform; // Referência à câmera principal
    public float screenShakeMagnitude = 0.1f; // Intensidade do tremor
    public float screenShakeDuration = 0.5f;  // Duração do tremor

    void Start()
    {

        ApplyDifficulty();


        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        blackscreen.gameObject.SetActive(false);
        GameOverText.text = "";
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        chaseAudioSource = gameObject.AddComponent<AudioSource>();
        chaseAudioSource.clip = chaseMusicClip;
        chaseAudioSource.loop = true;
        chaseAudioSource.volume = 0f;

        niggaAudioSource = gameObject.AddComponent<AudioSource>();
        niggaAudioSource.clip = niggasoundClip;

        retryButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);


        StartCoroutine(Patrol());
    }

    void Update()
{
    if (player == null)
    {
        return;
    }

    if (!PauseResume.GamePaused)
    {
        if (chaseAudioSource != null && !chaseAudioSource.isPlaying && isChasing)
        {
            chaseAudioSource.Play();
        }
    }

    currentDistance = Vector3.Distance(transform.position, player.position);
    float normalizedDistance = Mathf.Clamp01(currentDistance / maxDistance);
    SetTransparency(normalizedDistance);
    SetStaticaEffect(normalizedDistance);

    float distance = Vector3.Distance(transform.position, player.position);

    if (distance <= chaseDistance && stamina > 0)
    {
        StopCoroutine(Patrol());  // Para o patrulhamento quando começa a perseguição
        isPatrolling = false;    // Garante que o estado de patrulhamento seja desativado
        isChasing = true;        // Inicia a perseguição
        agent.speed = runSpeed;
        agent.SetDestination(player.position);
        chaseAudioSource.volume = Mathf.Lerp(chaseAudioSource.volume, 1f, Time.deltaTime);
        stamina -= staminaDrainRate * Time.deltaTime;

        // Se o efeito ainda não foi acionado, inicia o tremor e o áudio
        if (!hasTriggeredChaseEffect)
        {
            hasTriggeredChaseEffect = true;
            StartCoroutine(ScreenShakeEffect());
        }
    }
    else
    {
        isChasing = false;  // Desativa a perseguição

        if (!isPatrolling && !isGameOver)
        {
            StartCoroutine(Patrol());  // Reinicia o patrulhamento se não estiver em perseguição ou Game Over
            isPatrolling = true;   // Marca que o patrulhamento está ativo
        }

        chaseAudioSource.volume = Mathf.Lerp(chaseAudioSource.volume, 0f, Time.deltaTime);

        // Reseta o efeito se a perseguição parar
        hasTriggeredChaseEffect = false;
    }

    stamina = Mathf.Clamp(stamina, 0, 100);
    animator.SetFloat("Velocity", agent.velocity.magnitude);

    // Chama a rotação do monstro
    UpdateRotation();
}

    IEnumerator ScreenShakeEffect()
{
    // Reproduz o áudio de início de perseguição
    AudioSource startChaseAudioSource = gameObject.AddComponent<AudioSource>();
    startChaseAudioSource.clip = startChaseAudioClip;
    startChaseAudioSource.Play();

    float elapsed = 0f;

    Vector3 originalCameraPosition = cameraTransform.localPosition;

    // Enquanto o tremor estiver ativo
    while (elapsed < screenShakeDuration)
    {
        // Aplica tremor na câmera
        Vector3 randomShake = Random.insideUnitSphere * screenShakeMagnitude;
        cameraTransform.localPosition = originalCameraPosition + new Vector3(randomShake.x, randomShake.y, 0);

        elapsed += Time.deltaTime;
        yield return null;
    }

    // Restaura a posição original da câmera
    cameraTransform.localPosition = originalCameraPosition;
}

    IEnumerator Patrol()
{
    agent.speed = 3.5f;
    isPatrolling = true;
    Debug.Log("Iniciando patrulhamento");

    while (!isChasing && !isGameOver)
    {
        // Gera uma posição aleatória dentro do raio de patrulha
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += patrolCenter; // Adiciona o centro do patrulhamento como base

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
        {
            Vector3 patrolTarget = hit.position; // Posição válida no NavMesh
            agent.SetDestination(patrolTarget); // Define o destino para o agente
            Debug.Log($"Achei um alvo: {patrolTarget}");
        }
        else
        {
            Debug.LogWarning("Não encontrei uma posição válida para patrulhar");
            yield return new WaitForSeconds(1f); // Aguarda antes de tentar novamente
            continue; // Recomeça o loop para tentar encontrar um novo destino
        }

        // Verifica o estado do agente e faz debug adicional
        if (agent.isStopped)
        {
            Debug.LogWarning("Agente está parado (isStopped = true). Certifique-se de que ele está ativo.");
            agent.isStopped = false; // Garante que o agente pode se mover
        }

        if (agent.speed <= 0)
        {
            Debug.LogError("A velocidade do agente está definida como 0. Corrigindo para o valor padrão.");
            agent.speed = 3.5f; // Ajuste para sua velocidade de patrulha desejada
        }

        Debug.Log($"Movendo para o destino: {agent.destination}, Velocidade: {agent.speed}");

        // Aguarda até que o monstro chegue ao destino ou entre em perseguição
        while (!isChasing && (agent.pathPending || agent.remainingDistance > agent.stoppingDistance))
        {
            Debug.Log($"Distância restante: {agent.remainingDistance}");
            yield return null;
        }

        if (!isChasing)
        {
            Debug.Log("Alvo alcançado. Pausando antes de procurar outro destino.");
            yield return new WaitForSeconds(patrolPauseDuration); // Pausa antes de procurar outro destino
        }
    }

    isPatrolling = false;
    Debug.Log("Patrulhamento finalizado");
}

    void OnTriggerEnter(Collider other)
    {
    if (other.CompareTag("Door"))
    {
        DoorController doorController = other.GetComponent<DoorController>();

        if (doorController != null)
        {
            doorController.TryOpenDoor();
        }
    }

    if (other.CompareTag("Player") && !isGameOver)
    {
        // Lógica de Game Over
        GameOverSequence();
    }

    void GameOverSequence()
    {
        if (!isGameOver)
        {
            isGameOver = true;
            Destroy(pausescreen);
            StopAllAudioInScene();
            StartCoroutine(JumpscareSequence());
        }
    }}

    public void StopAllAudioInScene()
    {
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audio in allAudioSources)
        {
            if (audio.isPlaying)
            {
                audio.Stop();
            }
        }
    }

    IEnumerator JumpscareSequence()
    {
        jumpscareImage.gameObject.SetActive(true);
        jumpscareImage.color = new Color(1, 1, 1, 0);

        AudioSource jumpscareAudioSource = gameObject.AddComponent<AudioSource>();
        jumpscareAudioSource.clip = jumpscareAudioClip;
        jumpscareAudioSource.Play();

        float elapsedTime = 0f;
        float shakeMagnitude = 10f;

        while (elapsedTime < jumpscareAudioClip.length)
        {
            jumpscareImage.rectTransform.localPosition = (Vector3)Random.insideUnitCircle * shakeMagnitude;

            Color color = jumpscareImage.color;
            color.a = Mathf.Clamp01(elapsedTime / 0.5f);
            jumpscareImage.color = color;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        jumpscareImage.rectTransform.localPosition = Vector3.zero;
        jumpscareImage.gameObject.SetActive(false);

        ActivateGameOver();
    }

    void ActivateGameOver()
    {
        GameOverText.text = "VOCÊ FOI MAMADO!";
        blackscreen.gameObject.SetActive(true);
        chaseAudioSource.Stop();
        niggaAudioSource.Play();

        runSpeed = 0;
        walkSpeed = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        PauseResume.GamePaused = true;

        StartCoroutine(ShakeGameOverText());
    }

    IEnumerator ShakeGameOverText()
    {
        Vector3 originalPosition = GameOverText.rectTransform.localPosition;
        float duration = shakeDuration;
        float magnitude = 10f;

        while (duration > 0)
        {
            GameOverText.rectTransform.localPosition = originalPosition + (Vector3)Random.insideUnitCircle * magnitude;
            yield return null;
            duration -= Time.deltaTime;
        }

        GameOverText.rectTransform.localPosition = originalPosition;
        StartCoroutine(FadeInButtons());
    }

    IEnumerator FadeInButtons()
    {
        retryButton.gameObject.SetActive(true);
        exitButton.gameObject.SetActive(true);

        CanvasGroup retryCanvasGroup = retryButton.GetComponent<CanvasGroup>();
        CanvasGroup exitCanvasGroup = exitButton.GetComponent<CanvasGroup>();

        if (retryCanvasGroup == null) retryCanvasGroup = retryButton.gameObject.AddComponent<CanvasGroup>();
        if (exitCanvasGroup == null) exitCanvasGroup = exitButton.gameObject.AddComponent<CanvasGroup>();

        retryCanvasGroup.alpha = 0;
        exitCanvasGroup.alpha = 0;

        float elapsedTime = 0;

        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            retryCanvasGroup.alpha = alpha;
            exitCanvasGroup.alpha = alpha;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        retryCanvasGroup.alpha = 1;
        exitCanvasGroup.alpha = 1;
    }

    void SetTransparency(float normalizedDistance)
    {
        Color color = fadeImage.color;
        color.a = Mathf.Lerp(maxAlpha, 0f, normalizedDistance);
        fadeImage.color = color;
    }

    void SetStaticaEffect(float normalizedDistance)
    {
        Color color = staticaEffect.color;
        color.a = Mathf.Lerp(maxStaticaAlpha, 0f, normalizedDistance);
        staticaEffect.color = color;
    }

    void UpdateRotation()
{
    if (isChasing || isPatrolling)
    {
        Vector3 directionToMove = agent.velocity;
        
        // Ignora o eixo Y para rotacionar apenas no plano horizontal
        directionToMove.y = 0f;  

        // Se o monstro estiver se movendo
        if (directionToMove.sqrMagnitude > 0.1f)
        {
            // Calcula a rotação desejada com base na direção do movimento
            Quaternion targetRotation = Quaternion.LookRotation(directionToMove.normalized);

            // Rotaciona suavemente para a direção calculada
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }
}

    void ApplyDifficulty()
    {
        switch (DifficultyManager.Instance.currentDifficulty)
        {
            case Difficulty.Normal:
                runSpeed = 10f;
                walkSpeed = 5f;
                chaseDistance = 20f;
                stamina = 100f;
                staminaDrainRate = 5f;
                break;
            case Difficulty.Hard:
                runSpeed = 13f;
                walkSpeed = 8f;
                chaseDistance = 30f;
                stamina = 120f;
                staminaDrainRate = 3f;
                break;
            case Difficulty.Extreme:
                runSpeed = 20f;
                walkSpeed = 13f;
                chaseDistance = 60f;
                stamina = 150f;
                staminaDrainRate = 1f;
                break;
        }
    }

}
