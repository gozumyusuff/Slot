using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private const int scorePoint = 10;
    private int score = 0;

    [SerializeField]
    private TextMeshProUGUI scoreText;

    /// <summary>
    /// 10 puan eklemek için default býrakýn
    /// </summary>
    /// <param name="value">bu bir matchlength</param>
    public void AddScore(int value = 3)
    {
        score += scorePoint * (value - 2);
        scoreText.text = score.ToString();
    }

}
