using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameWinController : MonoBehaviour
{
    public Text scoreText;  
    private PersistantGameState gameStateRef;
    void Start()
    {
        gameStateRef = GameObject.Find("PersistantGameState").GetComponent<PersistantGameState>();
        scoreText.text = "Score: " + gameStateRef.gameScore;
    }

    // Update is called once per frame
    void Update()
    {
         if(Input.touchCount > 0 || Input.GetKeyDown(KeyCode.Space)){
             SceneManager.LoadScene("Stackr");
         }
    }
}
