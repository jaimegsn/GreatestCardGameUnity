using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using Newtonsoft.Json;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private FirebaseController fireController;
    private MatchmakingController matchmakingController;
    private QuestionController questionController;
    private MatchController matchController;

    void Awake()
    {
        fireController = GetComponent<FirebaseController>();
        matchmakingController = GetComponent<MatchmakingController>();
        matchController = GetComponent<MatchController>();
        questionController = GetComponent<QuestionController>();

    }
    void Update()
    {
        CheckGameStarted();
    }

    public void CreateGame()
    {
        Game gameStarted = new Game
        {
            turn = 0,
            playerTurn = 0,
            matchStarted = true
        };
        fireController.database
            .Child("matchmaking")
            .Child(matchmakingController.match.id)
            .Child("game")
            .SetRawJsonValueAsync(JsonConvert.SerializeObject(gameStarted));
    }

    public void CheckGameStarted()
    {
        if (matchmakingController.matchStarted)
        {
            matchController.enabled = true;
            questionController.enabled = true;
        }
    }

    public void FinishMatch(){
        if(matchmakingController.match.leader.Equals(matchmakingController.currentPlayerID)){
            matchmakingController.DropMatchmaking();
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
