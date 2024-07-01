using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestionController : MonoBehaviour
{
    private MatchmakingController matchmakingController;
    private FirebaseController fireController;
    Question currentQuestion;
    int currentQuestionIndex;
    public TextAsset jsonFile;

    public TMP_Text diceText;
	public TMP_Text conditionText;

	List<Question> questionList = new List<Question>();
    public Button responseQuestions;

    public TextMeshProUGUI questionTextFront;
    public CanvasGroup anwserPanel;
    public CanvasGroup playerTools;
	public CanvasGroup challengePanel;
	bool haveQuestions = true;
    public AudioClip diceSound;
    bool canAnswer = true;
    bool questionRight = false;
    bool diceRight = false;

    void Awake()
    {
        matchmakingController = GetComponent<MatchmakingController>();
        fireController = GetComponent<FirebaseController>();
        Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(jsonFile.text);

        foreach (Question item in myDeserializedClass.questions)
        {
            questionList.Add(item);
        }
    }

    void Update()
    {
        SelectNextQuestion();
        if (Input.GetKeyDown(KeyCode.K)) TestDiceAnimation(.1f);
    }

    public void TryAnwser()
    {
        if (haveQuestions)
        {
            canAnswer = true;
        }
    }

    public void SelectNextQuestion()
    {
        if (questionList.Count > 1)
        {
            int turn = matchmakingController.match.game.turn;
            currentQuestionIndex = matchmakingController.match.challenges[turn];
            currentQuestion = questionList[currentQuestionIndex];
            SetQuestionUI();
        }
        else
        {
            haveQuestions = false;
            questionTextFront.text = "Todos os Desafios foram resolvidos";
        }
    }

    public void SetQuestionUI()
    {
        questionTextFront.text = currentQuestion.questionText;
    }

    public void AnswerQuestion(int value)
    {
        if (canAnswer && haveQuestions)
        {
            StartCoroutine(AnimationQuestion(value));
            canAnswer = false;
        }
    }

    public IEnumerator AnimationQuestion(int value)
    {
		MenuTransitions.instance.HideCanvasGroup(anwserPanel);
		MenuTransitions.instance.HideCanvasGroup(challengePanel);
		MenuTransitions.instance.ShowCanvasGroup(playerTools);
		foreach (var item in currentQuestion.anwser)
        {
            if (value == item.anwser)
            {
                questionRight = true;
                int dicerolled = Random.Range(1, 7);
                yield return StartCoroutine(AnimationDice(dicerolled, questionRight, diceRight, item.diceroll));
            }
        }
        CheckIfAnwserIsCorrect(questionRight, diceRight);
		MenuTransitions.instance.HideCanvasGroup(playerTools);
		MenuTransitions.instance.ShowCanvasGroup(challengePanel);
	}

    public IEnumerator AnimationDice(int finalDiceValue, bool questionRight, bool diceRight, List<int> item)
    {
        string feedbackMessage = "Para pontuar você\nprecisa tirar: (" + string.Join(",", item) + ") \n\n";
		//diceText.fontSize = 22;
		conditionText.text = feedbackMessage + "Dado rola em 4s";
        yield return new WaitForSeconds(1f);
		conditionText.text = feedbackMessage + "Dado rola em 3s";
        yield return new WaitForSeconds(1f);
		conditionText.text = feedbackMessage + "Dado rola em 2s";
        yield return new WaitForSeconds(1f);
		conditionText.text = feedbackMessage + "Dado rola em 1s";
        yield return new WaitForSeconds(1f);
		conditionText.text = feedbackMessage;
        yield return new WaitForSeconds(1f);
        AudioManager.instance.PlaySound(diceSound);
        diceText.text =  1.ToString();
        yield return new WaitForSeconds(0.1f);
        diceText.text = 3.ToString();
        yield return new WaitForSeconds(0.1f);
        diceText.text = 5.ToString();
        yield return new WaitForSeconds(0.1f);
        diceText.text = 2.ToString();
        yield return new WaitForSeconds(0.1f);
        diceText.text = 6.ToString();
        yield return new WaitForSeconds(0.1f);
        diceText.text = 1.ToString();
        yield return new WaitForSeconds(0.1f);
        diceText.text = 4.ToString();
        yield return new WaitForSeconds(0.1f);
        diceText.text = 2.ToString();
        yield return new WaitForSeconds(0.1f);
        diceText.text = 5.ToString();
        yield return new WaitForSeconds(0.1f);
        diceText.text = "<b>" + finalDiceValue.ToString() + "<b>";
        yield return new WaitForSeconds(1);
        diceText.text = finalDiceValue.ToString();


        foreach (var dice in item)
        {
            if (finalDiceValue == dice)
            {
                diceRight = true;
                int auxGameTurn = matchmakingController.match.game.turn + 1 >= questionList.Count ? 
                0 : matchmakingController.match.game.turn + 1;
                int auxPlayerTurn = matchmakingController.match.game.playerTurn + 1 >= matchmakingController.match.players.Count ? 
                0 : matchmakingController.match.game.playerTurn + 1;

                Game game = new Game()
                {
                    turn = auxGameTurn,
                    playerTurn = auxPlayerTurn,
                    matchStarted = true
                };

                fireController.database
                    .Child("matchmaking")
                    .Child(matchmakingController.match.id)
                    .Child("game").SetRawJsonValueAsync(JsonConvert.SerializeObject(game));

                fireController.database
                    .Child("matchmaking")
                    .Child(matchmakingController.match.id)
                    .Child("players")
                    .Child(matchmakingController.currentPlayerID)
                    .Child("score")
                    .SetValueAsync(matchmakingController.match.players[matchmakingController.currentPlayerID].score + 10);

            }
        }
        //anwserPanel.SetActive(false);
    }

    public void CheckIfAnwserIsCorrect(bool questionRight, bool diceRight)
    {
        if (!questionRight || questionRight && !diceRight)
        {
            int auxPlayerTurn = matchmakingController.match.game.playerTurn + 1 >= matchmakingController.match.players.Count ?
                                0 : matchmakingController.match.game.playerTurn + 1;

            fireController.database
                .Child("matchmaking")
                .Child(matchmakingController.match.id)
                .Child("game")
                .Child("playerTurn")
                .SetValueAsync(auxPlayerTurn);
        }
    }

    public void TestDiceAnimation(float newTime)
    {
        StartCoroutine(DiceAnimation(newTime));
    }

    public IEnumerator DiceAnimation(float newTime)
    {
        AudioManager.instance.PlaySound(diceSound);
        yield return new WaitForSeconds(newTime);
        diceText.text = 1.ToString();
        yield return new WaitForSeconds(newTime);
        diceText.text = 3.ToString();
        yield return new WaitForSeconds(newTime);
        diceText.text = 5.ToString();
        yield return new WaitForSeconds(newTime);
        diceText.text = 2.ToString();
        yield return new WaitForSeconds(newTime);
        diceText.text = 6.ToString();
        yield return new WaitForSeconds(newTime);
        diceText.text = 1.ToString();
        yield return new WaitForSeconds(newTime);
        diceText.text = 4.ToString();
        yield return new WaitForSeconds(newTime);
        diceText.text = 2.ToString();
        yield return new WaitForSeconds(newTime);
        diceText.text = 5.ToString();
        yield return new WaitForSeconds(newTime);
        diceText.text = "<b>" + 2 + "<b>";
        yield return new WaitForSeconds(1);
        diceText.text = "2";
    }
}
