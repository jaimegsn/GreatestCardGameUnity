using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTransitions : MonoBehaviour
{
    public static MenuTransitions instance;
    public float MenuTransitionDuration;

    public bool pausedGame;
    public bool beginGame;

    public CanvasGroup target;
    // Game Objects for createRoom
    public GameObject createRoom_roomMode;
    public GameObject createRoom_playerLimitText;
    public GameObject createRoom_sliderPlayerQuantity;

    public GameObject createRoom_createGameBtn;


    // Game Objects for enterRoom

    public GameObject enterRoom_insertCodeText;
    public GameObject enterRoom_codeInput;
    public GameObject enterRoom_enterGameBtn;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (target != null)
            AudioManager.instance.target = target;
    }

    public void OpenCreateRoom()
    {
        createRoom_roomMode.SetActive(true);
        createRoom_playerLimitText.SetActive(true);
        createRoom_sliderPlayerQuantity.SetActive(true);
        createRoom_createGameBtn.SetActive(true);
        enterRoom_insertCodeText.SetActive(false);
        enterRoom_codeInput.SetActive(false);
        enterRoom_enterGameBtn.SetActive(false);
    }

    public void OpenEnterRoom()
    {
        createRoom_roomMode.SetActive(false);
        createRoom_playerLimitText.SetActive(false);
        createRoom_sliderPlayerQuantity.SetActive(false);
        createRoom_createGameBtn.SetActive(false);
        enterRoom_insertCodeText.SetActive(true);
        enterRoom_codeInput.SetActive(true);
        enterRoom_enterGameBtn.SetActive(true);
    }

    public void OpenOption()
    {
        AudioManager.instance.EnterOptions(target);
        AudioManager.instance.target = target;
    }

    public void CloseOptions()
    {
        AudioManager.instance.ExitOptions(target);
        AudioManager.instance.target = target;
    }

    public void PlaySoundEffect(AudioClip ac)
    {
        AudioManager.instance.PlaySound(ac);
    }

    public void FateMusic(AudioClip ac)
    {
        AudioManager.instance.FadeMusic(ac);
    }

    public void BeginGame()
    {
        beginGame = true;
    }

    public void EndGame()
    {
        beginGame = false;
    }

    public void PauseGame()
    {
        pausedGame = !pausedGame;
        beginGame = !pausedGame;

        if (AudioManager.instance != null)
            AudioManager.instance.MusicPauseGame(pausedGame);
        else Debug.Log("Audio Manager is Missing");

        if (pausedGame) Time.timeScale = 0;
        else Time.timeScale = 1;
    }

    public void ShowCanvasGroup(CanvasGroup CG)
    {
        if (CG != null)
            StartCoroutine(ShowCanvasGroupAlpha(CG));
        else
            Debug.Log("Panel with Canvas Groups is Missing");
    }

    IEnumerator ShowCanvasGroupAlpha(CanvasGroup CG)
    {
        CG.blocksRaycasts = true;
        float currentTime = 0f;
        while (currentTime < MenuTransitionDuration)
        {
            CG.alpha = Mathf.Lerp(0, 1, currentTime / MenuTransitionDuration);
            currentTime += Time.fixedDeltaTime;
            yield return null;
        }
        CG.alpha = 1;
        yield return null;
    }

    public void HideCanvasGroup(CanvasGroup CG)
    {
        if (CG != null)
        {
            StartCoroutine(HideCanvasGroupAplha(CG));
        }
        else
        {
            Debug.Log("Panel with Canvas Groups is Missing");
        }
    }

    IEnumerator HideCanvasGroupAplha(CanvasGroup CG)
    {
        CG.blocksRaycasts = false;
        float currentTime = 0f;
        while (currentTime < MenuTransitionDuration)
        {
            CG.alpha = Mathf.Lerp(1, 0, currentTime / MenuTransitionDuration);
            currentTime += Time.fixedDeltaTime;
            yield return null;
        }
        CG.alpha = 0;
        yield return null;
    }
}
