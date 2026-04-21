using UnityEngine;
using TMPro;

public class GameOverScoreDisplay : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    void Start()
    {
        if (scoreText != null)
        {
            scoreText.text = "" + GameData.finalScore;
        }
    }
}