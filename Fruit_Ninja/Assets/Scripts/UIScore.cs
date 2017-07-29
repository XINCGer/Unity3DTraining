using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScore : MonoBehaviour
{
    [SerializeField]
    private Text scoreValue;

    public static UIScore Instance = null;

    private int score = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void AddScore(int score)
    {
        this.score += score;
        scoreValue.text = this.score.ToString();
    }

    public void RemoveScore(int score)
    {
        this.score -= score;
        scoreValue.text = this.score.ToString();
    }
}
