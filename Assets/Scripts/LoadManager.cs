using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadManager : MonoBehaviour
{
    public static LoadManager instance;
    public bool isThisMainMenu;
    public float transitionDuration;
    public CanvasGroup loadCG;

    private void Awake() {
        instance = this;
        AjustLoadScreen();
    }

    private void Start() {
        if (!isThisMainMenu)
            MenuTransitions.instance.HideCanvasGroup(loadCG);
    }

    public void AjustLoadScreen() {
        if (!isThisMainMenu) {
            if (!loadCG.blocksRaycasts)
                loadCG.blocksRaycasts = true;
            if (loadCG.alpha != 1)
                loadCG.alpha = 1;
        } else {
            if (loadCG.blocksRaycasts)
                loadCG.blocksRaycasts = false;
            if (loadCG.alpha == 1)
                loadCG.alpha = 0;
        }
    }

    public void LoadScene(int newScene) {
        StartCoroutine(Load(newScene));
    }

    public void ReturnToMenu(AudioClip c) {
        AudioManager.instance.FadeMusic(c);
    }

    public IEnumerator Load(int newScene) {
        MenuTransitions.instance.ShowCanvasGroup(loadCG);
        yield return new WaitForSeconds(transitionDuration);
        SceneManager.LoadSceneAsync(newScene);
    }

    public void ReloadScene() {
        SceneManager.LoadSceneAsync(0);
    }
}
