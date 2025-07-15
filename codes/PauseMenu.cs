using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseResume : MonoBehaviour
{
    public static bool GamePaused = false;
    public GameObject pauseMenuUI;
    public RawImage blackScreen;
    public float fadeDuration = 1f;

    private AudioSource[] allAudioSources;
    private FirstPersonController firstPersonController;  // Referência ao script FirstPersonController
    private AudioSource monsterAudioSource; // Referência para o AudioSource do monstro (Rage)

    void Start()
    {
        GamePaused = false;
        Time.timeScale = 1;
        pauseMenuUI.SetActive(false);
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        blackScreen.gameObject.SetActive(false);
        blackScreen.raycastTarget = false;

        allAudioSources = FindObjectsOfType<AudioSource>();

        // Obtém a referência ao FirstPersonController
        firstPersonController = FindObjectOfType<FirstPersonController>();

        // Encontra o AudioSource do monstro (Rage)
        monsterAudioSource = FindObjectOfType<Monster>()?.GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GamePaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        GamePaused = true;
        Time.timeScale = 0;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        blackScreen.gameObject.SetActive(true);
        blackScreen.raycastTarget = false;
        pauseMenuUI.SetActive(true);

        foreach (AudioSource audioSource in allAudioSources)
        {
            if (audioSource != null)
            {
                audioSource.Pause();
            }
        }

        // Pausa o áudio do Rage se o monstro estiver em RageMode
        if (monsterAudioSource != null && monsterAudioSource.isPlaying)
        {
            monsterAudioSource.Pause();
        }

        // Desativa a sprintBarCG durante a pausa
        if (firstPersonController != null && firstPersonController.sprintBarCG != null)
        {
            firstPersonController.sprintBarCG.enabled = false;
        }

        if (firstPersonController != null)
        {
            if (firstPersonController.sprintBarBG != null)
                firstPersonController.sprintBarBG.gameObject.SetActive(false);
            
            if (firstPersonController.sprintBar != null)
                firstPersonController.sprintBar.gameObject.SetActive(false);
        }
    }

    public void ResumeGame()
    {
        GamePaused = false;
        Time.timeScale = 1;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        blackScreen.gameObject.SetActive(false);
        blackScreen.raycastTarget = false;
        pauseMenuUI.SetActive(false);

        foreach (AudioSource audioSource in allAudioSources)
        {
            if (audioSource != null)
            {
                audioSource.UnPause();
            }
        }

        // Retoma o áudio do Rage se o monstro estiver em RageMode
        if (monsterAudioSource != null && !monsterAudioSource.isPlaying)
        {
            monsterAudioSource.UnPause();
        }

        // Reativa a sprintBarCG quando o jogo é retomado
        if (firstPersonController != null && firstPersonController.sprintBarCG != null)
        {
            firstPersonController.sprintBarCG.enabled = true;
        }

        if (firstPersonController != null)
        {
            if (firstPersonController.sprintBarBG != null)
                firstPersonController.sprintBarBG.gameObject.SetActive(true);
            
            if (firstPersonController.sprintBar != null)
                firstPersonController.sprintBar.gameObject.SetActive(true);
        }
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Play()
    {
        SceneManager.LoadScene(2);
    }

    public void Play2()
    {
        SceneManager.LoadScene(3);
    }

    public void RetryLevel()
    {
        Time.timeScale = 1; // Garante que o tempo está normal
        Scene currentScene = SceneManager.GetActiveScene(); // Obtém a cena atual
        SceneManager.LoadScene(currentScene.buildIndex); // Recarrega a cena pelo índice
    }

    // Por exemplo, ao clicar em um botão:
    public void SetNormalDifficulty()
    {
        DifficultyManager.Instance.currentDifficulty = Difficulty.Normal;
    }
    public void SetHardDifficulty()
    {
        DifficultyManager.Instance.currentDifficulty = Difficulty.Hard;
    }
    public void SetExtremeDifficulty()
    {
        DifficultyManager.Instance.currentDifficulty = Difficulty.Extreme;
    }

}
