using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text CurrentScoreUI;

    public int currentScore = 5;

    void Update()
    {
        CurrentScoreUI.text = "남은 몬스터 : " + currentScore;
    }
}
