using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DifficultMenu : MonoBehaviour
{
    public TextMeshProUGUI difficultyLabel;

    public TMP_FontAsset normalFont;
    public TMP_FontAsset hardFont;
    public TMP_FontAsset extremeFont;

    // Start is called before the first frame update
    void Start()
    {
        ApplyDifficulty();
    }

    // Update is called once per frame
    void Update()
    {
        ApplyDifficulty();
    }

    void ApplyDifficulty()
    {
        switch (DifficultyManager.Instance.currentDifficulty)
        {
            case Difficulty.Normal:
                difficultyLabel.color = Color.white;
                difficultyLabel.text = "NORMAL";
                difficultyLabel.font = normalFont;
                break;
            case Difficulty.Hard:
                difficultyLabel.color = Color.black;
                difficultyLabel.text = "HARD";
                difficultyLabel.font = hardFont;
                break;
            case Difficulty.Extreme:
                difficultyLabel.color = Color.red;
                difficultyLabel.text = "EXTREME";
                difficultyLabel.font = extremeFont;
                break;
        }
    }
}
