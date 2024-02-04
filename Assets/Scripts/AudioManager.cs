using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    
    public AudioSource bgmAS;
    public AudioSource sfxAS;
    public Slider bgmSlider;
    public Slider sfxSlider;
    public Toggle bgmToggle;
    public Toggle sfxToggle;

    public CanvasGroup target;

    public float musicTransitionTime;

    void Awake() {
        if (instance == null)
            instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(this.gameObject);
    }

    void Start() {
        SetUItoVolumeValues();
    }

    //Faz load do volume de musicas e efeitos sonoros e atualiza valores na interface
    public void SetUItoVolumeValues() {
        bgmAS.volume = PlayerPrefs.GetFloat("BGMVolume", 0.4f);
        sfxAS.volume = PlayerPrefs.GetFloat("SFXVolume", 0.4f);
        bgmSlider.value = bgmAS.volume * 10;
        sfxSlider.value = sfxAS.volume * 10;

        if (bgmAS.volume == 0) {
            bgmAS.mute = true;
        }

        if (sfxAS.volume == 0) {
            sfxAS.mute = true;
        }

        if (bgmAS.mute) {
            bgmToggle.isOn = false;
        }

        if (sfxAS.mute) {
            sfxToggle.isOn = false;
        }
    }

    public void MuteBGM(AudioClip uiClip) {
        PlaySound(uiClip);
        bgmAS.mute = bgmToggle.isOn;
    }

    public void MuteSFX(AudioClip uiClip) {
        PlaySound(uiClip);
        sfxAS.mute = sfxToggle.isOn;
        PlaySound(uiClip);
    }

    //Muda volume das musicas salvando em preferencias
    public void ChangeBGMVolume() {
        if (MenuTransitions.instance != null) {

            if (MenuTransitions.instance.pausedGame) {
                bgmAS.volume = bgmSlider.value / 20;
            } else {
                bgmAS.volume = bgmSlider.value / 10;
            }

            PlayerPrefs.SetFloat("BGMVolume", bgmAS.volume);

        } else Debug.Log("Game Manager is Missing");

        if (bgmAS.volume == 0) {
            bgmToggle.isOn = true;
            bgmAS.mute = bgmToggle.isOn;
        } else if(bgmToggle.isOn == true) {
            bgmToggle.isOn = false;
            bgmAS.mute = bgmToggle.isOn;
        }
    }

    //Muda volume dos efeitos sonoros salvando em preferencias
    public void ChangeSFXVolume() {
        sfxAS.volume = sfxSlider.value / 10;        
        PlayerPrefs.SetFloat("SFXVolume", sfxAS.volume);

        if (sfxAS.volume == 0) {
            sfxToggle.isOn = true;
            sfxAS.mute = sfxToggle.isOn;
        } else if (sfxToggle.isOn == true) {
            sfxToggle.isOn = false;
            sfxAS.mute = sfxToggle.isOn;
        }
    }

    //Muda abruptamente a musica que est� tocando
    public void ChangeMusicNow(AudioClip music) {
        bgmAS.Pause();
        bgmAS.clip = music;
        bgmAS.Play();
    }

    //Muda a musica que est� tocando fazendo um fade
    public void FadeMusic(AudioClip music) {
        StopCoroutine(FadeAudioIenumerator(music, musicTransitionTime));
        StartCoroutine(FadeAudioIenumerator(music, musicTransitionTime));
    }

    //Salva volume atual, fade out e fade in
    IEnumerator FadeAudioIenumerator(AudioClip music, float delayTime) {

        //float currentVolume = bgmAS.volume;
        float currentVolume = bgmSlider.value / 10;
        float timeElapse = 0f;

        while (timeElapse < delayTime) {
            bgmAS.volume = Mathf.Lerp(currentVolume,0 , timeElapse / delayTime);
            timeElapse += Time.deltaTime;
            yield return null;
        }

        bgmAS.Pause();
        yield return new WaitForSeconds(0.5f);
        bgmAS.clip = music;
        timeElapse = 0;
        bgmAS.Play();

        while (timeElapse < delayTime) {
            bgmAS.volume = Mathf.Lerp(0, currentVolume, timeElapse / delayTime);
            timeElapse += Time.deltaTime;
            yield return null;
        }
        if (bgmAS.volume != currentVolume) bgmAS.volume = currentVolume;
    }

    //Toca efeito sonoro
    public void PlaySound(AudioClip sound) {
        sfxAS.PlayOneShot(sound);
    }

    //Diminue o volume da musica durante o jogo pausado
    public void MusicPauseGame(bool paused) {
        if (paused) {
            bgmAS.volume = bgmAS.volume / 2;
        } else {
            bgmAS.volume = bgmAS.volume * 2;
        }
    }

    public void EnterOptions(CanvasGroup target) {
        MenuTransitions.instance.ShowCanvasGroup(this.GetComponentInChildren<CanvasGroup>());
        if (target != null)
            MenuTransitions.instance.HideCanvasGroup(target);
    }

    public void CloseOptions() {
        if(target != null) {
            ExitOptions(target);
        } else {
            ExitOptions();
        }
    }

    public void ExitOptions(CanvasGroup target) {
        MenuTransitions.instance.HideCanvasGroup(this.GetComponentInChildren<CanvasGroup>());
        if (target != null)
            MenuTransitions.instance.ShowCanvasGroup(target);
    }

    public void ExitOptions() {
        MenuTransitions.instance.HideCanvasGroup(this.GetComponentInChildren<CanvasGroup>());
    }
}
