using UnityEngine;

public enum Difficulty
{
    Normal,
    Hard,
    Extreme
}

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance;

    public Difficulty currentDifficulty = Difficulty.Normal;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            ApplyFogSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ApplyFogSettings()
    {
        RenderSettings.fog = true;

        switch (currentDifficulty)
        {
            case Difficulty.Normal:
                RenderSettings.fogColor = Color.gray;
                RenderSettings.fogDensity = 0.01f;
                break;

            case Difficulty.Hard:
                RenderSettings.fogColor = Color.black;
                RenderSettings.fogDensity = 0.03f;
                break;

            case Difficulty.Extreme:
                RenderSettings.fogColor = new Color(0.1f, 0, 0); // dark red
                RenderSettings.fogDensity = 0.06f;
                break;
        }
    }
}
