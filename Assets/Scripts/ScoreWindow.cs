using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreWindow : MonoBehaviour
{
    // Start is called before the first frame update
    private Text scoreText;
     
    private void Awake()
    {
        scoreText = transform.Find("scoreText").GetComponent<Text>();
    }

    private void Update()
    {
        scoreText.text = GameHandler.GetScore().ToString();
    }
}
