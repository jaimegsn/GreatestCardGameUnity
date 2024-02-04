using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json;
using static System.Random;


public class MatchmakingController : MonoBehaviour
{
    public Slider playercountSlider;

    public int totalQuestions;
    public TMP_InputField inputfieldName;
    public TMP_InputField inputfieldCode;
    public TextMeshProUGUI playerCountText;
    public TMP_Text waitPlayers;
    public TMP_Text conected;
    public TMP_Text waitRoomId;
    public GameObject startMatchBtn;
    public CanvasGroup gameplayCanvas;
    public Toggle experimentalRoomMode;
    FirebaseController fireController;
    public bool matchStarted = false;
    public string currentPlayerID;
    string currentRoomID;
    string leaderPlayer;
    string waitTxt;
    List<string> data = new List<string>();
    //public List<Matchmaking> fireController.matchList = new List<Matchmaking>();
    public Matchmaking match = new Matchmaking();
    void Awake()
    {
        //match.game.matchStarted = false;
    }
    void Start()
    {
        fireController = GetComponent<FirebaseController>();
        //GetMatchMaking();
    }
    void Update()
    {
        waitPlayers.text = waitTxt;
        checkConection();
        checkLeaderPlayer();
        playerCountText.text = playercountSlider.value.ToString();
    }
    void checkConection()
    {
        if (fireController.isFirebaseInitialized)
            conected.text = "Conectado";
        else
            conected.text = "Desconectado";
    }
    void checkLeaderPlayer()
    {
        if (currentPlayerID == leaderPlayer)
        {
            waitRoomId.text = "Código: " + currentRoomID;
        }
        else
        {
            waitRoomId.text = "";
        }
    }
    void checkRoomIsFull(string playersCount, string maxPlayer)
    {
        if (string.Equals(playersCount, maxPlayer) && string.Equals(currentPlayerID, leaderPlayer))
        {
            startMatchBtn.SetActive(true);
        }
    }
    public void GetMatchMaking()
    {
        fireController.database
            .Child("matchmaking")
            .ValueChanged += (object sender, ValueChangedEventArgs args) =>
            {
                if (args.DatabaseError != null)
                {
                    return;
                }
                else
                {
                    fireController.matchList.Clear();
                    foreach (var item in args.Snapshot.Children)
                    {
                        data.Add(item.Key);
                        var matchJson = JsonConvert.DeserializeObject<Matchmaking>(item.GetRawJsonValue());
                        fireController.matchList.Add(matchJson);
                    }
                    return;
                }
            };
    }
    public void WaitingRoom()
    {
        fireController.database
            .Child("matchmaking")
            .Child(currentRoomID)
            .ValueChanged += (object sender, ValueChangedEventArgs args) =>
            {
                if (args.DatabaseError != null)
                {
                    return;
                }
                else
                {
                    match = JsonConvert.DeserializeObject<Matchmaking>(args.Snapshot.GetRawJsonValue()); ;
                    leaderPlayer = args.Snapshot.Child("leader").Value.ToString();
                    var playersCount = args.Snapshot.Child("players").ChildrenCount;
                    var maxPlayer = args.Snapshot.Child("maxplayers").Value;
                    waitTxt = playersCount + "/" + maxPlayer;

                    checkRoomIsFull(playersCount.ToString(), maxPlayer.ToString());

                    if (args.Snapshot.Child("game").Child("matchStarted").Value.ToString().Equals("True"))
                    {
                        matchStarted = true;
                        MenuTransitions.instance.ShowCanvasGroup(gameplayCanvas);
                        //gameplayCanvas.SetActive(true);
                    }

                    return;
                }
            };
    }

    public void CancelWaitingRoom()
    {
        if (currentPlayerID.Equals(match.leader))
        {
            DropMatchmaking();
        }
        else
        {
            fireController.database.Child("matchmaking")
                .Child(currentRoomID)
                .Child("players")
                .Child(currentPlayerID)
                .RemoveValueAsync();
        }


        currentPlayerID = "";
        currentRoomID = "";
        leaderPlayer = "";
        waitTxt = "";
        waitRoomId.text = "";
    }

    public void DropMatchmaking()
    {
        fireController.database
        .Child("matchmaking")
        .Child(match.id)
        .RemoveValueAsync();
    }


    List<int> generateDeck(int qtd)
    {
        List<int> deck = new List<int>();

        for (int i = 0; i < qtd; i++)
        {
            deck.Add(i);
        }
        if (experimentalRoomMode.isOn == true)
        {
            return deck;
        }
        else
        {
            return shuffleDeck(deck);
        }
    }


    List<int> shuffleDeck(List<int> deck)
    {
        List<int> shuffledeck = new List<int>();
        var rnd = new System.Random();
        var randomizedeck = deck.OrderBy(item => rnd.Next());

        foreach (var item in randomizedeck)
        {
            shuffledeck.Add(item);
        }

        return shuffledeck;
    }

    string GenerateID(string format)
    {
        var date = DateTime.Now.ToString(format);
        return date;
    }

    public void FindMatchmaking()
    {
        if (fireController.isFirebaseInitialized)
        {
            bool keyExist = false;
            int code = Int32.Parse(inputfieldCode.text);
            int currentQtdPlayers = 0;
            int currentMaxPlayers = 0;

            if (fireController.matchList.Count != 0)
            {
                foreach (Matchmaking item in fireController.matchList)
                {
                    if (item.id != null)
                    {
                        int auxItem = Int32.Parse(item.id);

                        if (auxItem == code)
                        {
                            keyExist = true;
                            currentQtdPlayers = item.players.Count;
                            currentMaxPlayers = Int32.Parse(item.maxplayers);
                        }
                    }
                }
            }

            Player player = new Player
            {
                id = GenerateID("MdHmmssH"),
                name = inputfieldName.text,
                order = currentQtdPlayers,
                score = 0
            };

            currentPlayerID = player.id;
            currentRoomID = inputfieldCode.text;

            if (keyExist == true
                && currentQtdPlayers != 0
                && currentMaxPlayers != 0
                && currentQtdPlayers < currentMaxPlayers
            )
            {
                string playerjson = JsonConvert.SerializeObject(player);
                fireController.database.Child("matchmaking")
                            .Child(code.ToString())
                            .Child("players")
                            .Child(player.id)
                            .SetRawJsonValueAsync(playerjson)
                            .ContinueWith(task =>
                            {
                                if (task.IsCompleted)
                                {
                                    return;
                                }
                                else
                                {
                                    return;
                                }
                            });
            }
            else
            {
                return;
            }
        }

    }


    public void CreateMatchmaking()
    {
        if (fireController.isFirebaseInitialized)
        {
            Dictionary<string, Player> playerlist = new Dictionary<string, Player>();

            Player player = new Player
            {
                id = GenerateID("MdHmmssH"),
                name = inputfieldName.text,
                order = 0,
                score = 0
            };

            Game gameInitial = new Game
            {
                turn = 0,
                playerTurn = 0,
                matchStarted = false
            };

            playerlist.Add(player.id, player);

            currentPlayerID = player.id;

            Matchmaking match = new Matchmaking
            {
                leader = currentPlayerID,
                maxplayers = playercountSlider.value.ToString(),
                id = GenerateID("MdmmssHH"),
                challenges = generateDeck(totalQuestions),
                game = gameInitial,
                players = playerlist
            };

            currentRoomID = match.id.ToString();

            string matchjson = JsonConvert.SerializeObject(match);
            fireController.database.Child("matchmaking")
                .Child(match.id.ToString())
                .SetRawJsonValueAsync(matchjson)
                .ContinueWith(task =>
                {
                    if (task.IsCompleted)
                    {
                        return;
                    }
                    else
                    {
                        return;
                    }
                });
        }
    }
}
