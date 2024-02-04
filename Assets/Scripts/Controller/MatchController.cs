using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchController : MonoBehaviour
{
    // Start is called before the first frame update

    TMP_Text playerName;
    TMP_Text playerScore;
    GameObject playerTurnIndicator;
    GameObject playerTradeCardButton;
    public CanvasGroup gameOver;
    public TMP_Text winnerTitle;

    GameObject gameController;
    public Button responseQuestions;
    public List<GameObject> playerList;
    private MatchmakingController matchmakingController;

    void Start()
    {
        matchmakingController = GetComponent<MatchmakingController>();
    }

    // Update is called once per frame
    void Update()
    {
        ListPlayers();
        CheckCurrentPlayerAnswer();
        CheckMatchFinish();
    }

    public void CheckMatchFinish()
    {
        if (matchmakingController.match.game.turn > matchmakingController.match.challenges.Count)
        {
            Player winnerPlayer = new Player
            {
                name = "",
                id = "",
                score = 0,
                order = 0
            };
            List<Player> listWinnersPlayers = new List<Player>();
            foreach (var item in matchmakingController.match.players)
            {
                if (item.Value.score >= winnerPlayer.score)
                {
                    winnerPlayer = item.Value;
                    listWinnersPlayers.Add(winnerPlayer);
                }

            }
            string winnerText = "";
            if (listWinnersPlayers.Count == 1)
            {
                if (listWinnersPlayers[0].id.Equals(matchmakingController.currentPlayerID))
                {
                    winnerText = "Você venceu!";
                }
                else
                {
                    winnerText = listWinnersPlayers[0].name + " Venceu!";
                }
            }
            else if (listWinnersPlayers.Count > 1)
            {
                foreach (Player item in listWinnersPlayers)
                {
                    winnerText = winnerText + " " + item.name;
                }

                winnerText = winnerText + "empataram!";
            }
            winnerTitle.text = winnerText;
            MenuTransitions.instance.ShowCanvasGroup(gameOver);
            //gameOver.SetActive(true);
        }
        else
        {
            foreach (var item in matchmakingController.match.players)
            {
                string winnerText = "";

                if (item.Value.score >= 60)
                {
                    if (item.Value.id.Equals(matchmakingController.currentPlayerID))
                    {
                        winnerText = "Você venceu!";
                    }
                    else
                    {
                        winnerText = item.Value.name + " venceu!";
                    }
                    winnerTitle.text = winnerText;
                    //gameOver.SetActive(true);
                    MenuTransitions.instance.ShowCanvasGroup(gameOver);
                    break;
                }
            }
        }

    }



    public void CheckCurrentPlayerAnswer()
    {
        if (matchmakingController.match.players[matchmakingController.currentPlayerID].order
        == matchmakingController.match.game.playerTurn)
        {
            responseQuestions.gameObject.SetActive(true);
        }
        else
        {
            responseQuestions.gameObject.SetActive(false);
        }
    }
    public Dictionary<string, Player> PlayerMockup()
    {
        Dictionary<string, Player> playerListMock = new Dictionary<string, Player>();

        for (int i = 0; i < 5; i++)
        {
            Player auxPlayer = new Player()
            {
                id = "1234" + i,
                name = "teste" + i,
                order = i,
                score = 0
            };
            playerListMock.Add(i.ToString(), auxPlayer);
        }

        return playerListMock;
    }

    public void InstantiatePlayerData(GameObject playerChild, Player player)
    {
        playerName = playerChild.transform.GetChild(0).gameObject.transform.GetChild(1).GetComponent<TMP_Text>();
        playerScore = playerChild.transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<TMP_Text>();
        playerTurnIndicator = playerChild.transform.GetChild(1).gameObject;
        playerTradeCardButton = playerChild.transform.GetChild(2).gameObject;

        SetDataPlayer(playerName, playerScore, playerTurnIndicator, playerTradeCardButton, player);
    }

    public void SetDataPlayer(TMP_Text playerScore,
        TMP_Text playerName,
        GameObject playerTurnIndicator,
        GameObject playerTradeCardButton,
        Player player)
    {
        playerName.text = player.name;
        playerScore.text = player.score.ToString();
        if (player.order == matchmakingController.match.game.playerTurn)
        {
            playerTurnIndicator.gameObject.SetActive(true);
        }
        else
        {
            playerTurnIndicator.gameObject.SetActive(false);

        }
    }

    public void ListPlayers()
    {
        for (int index = 0; index < 5; index++)
        {
            if (index < /*PlayerMockup().Count*/matchmakingController.match.players.Count)
            {
                InstantiatePlayerData(playerList[index], matchmakingController.match.players.ElementAt(index).Value);
            }
            else
            {
                playerList[index].gameObject.SetActive(false);
            }
        }
    }
}
