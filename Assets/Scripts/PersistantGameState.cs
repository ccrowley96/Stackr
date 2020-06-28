using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistantGameState : MonoBehaviour
{
    public int gameScore = 0;
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        gameScore = 0;    
    }
}
