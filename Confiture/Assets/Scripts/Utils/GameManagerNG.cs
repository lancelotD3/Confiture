using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerNG : MonoBehaviour
{
    public Animator fadeAnimator;
    static public GameManagerNG instance;

    private string nextScene;
    public string mainMenuName = "MainMenuNG";
    public string firstLevel = "Level_01_NG";

    [HideInInspector] public string lastScene;

    [Header("Timer")]
    public float timeForGold = 10f;
    public float timeForSilver = 20f;
    public float timeForBronze = 30f;

    public Vector3 offset;
    int BN_number;

    bool canSwitch = true;
    
    [Header("Audio")]
    public AudioSource musicAudioSource;
    public AudioSource audioSource;

    public AudioClip menuMusic;
    public AudioClip gameMusic;

    public InGameManager inGameManager;
    public InGameManager inGameManagerPrefab;

    public SaveData[] saveDatas;
    private int selectedSaveIndex;

    public ChaptersDataBase chaptersDataBase;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        lastScene = mainMenuName;
    }

    public void SaveData(int saveIndex)
    {
        SaveSystem.SaveSave("Save_" + saveIndex, saveDatas[saveIndex]);
    }

    public void SaveSelectedData()
    {
        SaveSystem.SaveSave("Save_" + selectedSaveIndex, saveDatas[selectedSaveIndex]);
    }

    public void LoadDatas()
    {
        saveDatas = new SaveData[3];
        for (int i = 0; i < saveDatas.Length; i++)
        {
            saveDatas[i] = SaveSystem.LoadSaveData("Save_" + i);
        }
    }

    public void SelectSave(int index)
    {
        selectedSaveIndex = index;
    }

    public SaveData GetSelectedSave()
    {
        return saveDatas[selectedSaveIndex];
    }

    public void DeleteSelectedSave()
    {
        SaveSystem.DeleteSaveData("Save_" + selectedSaveIndex);
    }

    public void PlayFirstLevel(SaveData saveData)
    {
        inGameManager = Instantiate(inGameManagerPrefab);
        SwitchLevel(firstLevel);
        InGameManager.instance.StartLevel(-1);

        saveDatas[selectedSaveIndex] = saveData;
    }

    public void PlayFirstLevelOfChapter(int chapterIndex)
    {
        

        if (InGameManager.instance)
        {
            Destroy(InGameManager.instance);
            inGameManager = null;
        }

        inGameManager = Instantiate(inGameManagerPrefab);
        SwitchLevel(chaptersDataBase.chapters[chapterIndex].levels[0]);

        InGameManager.instance.chapterId = chapterIndex;
        InGameManager.instance.StartLevel(-1);
    }

    public void SwitchScene(string sceneName)
    {
        if (!canSwitch)
            return;

        if(lastScene == mainMenuName)
        {
            musicAudioSource.Stop();
            musicAudioSource.clip = gameMusic;
            musicAudioSource.Play();
        }

        lastScene = SceneManager.GetActiveScene().name;

        canSwitch = false;
        Invoke(nameof(WaitForNextSwitch), .5f);

        nextScene = sceneName;
        FadeIn();
    }

    public void SwitchLevel(string sceneName)
    {
        if (!canSwitch)
            return;

        if (lastScene == mainMenuName)
        {
            musicAudioSource.Stop();
            musicAudioSource.clip = gameMusic;
            musicAudioSource.Play();
        }

        lastScene = SceneManager.GetActiveScene().name;

        canSwitch = false;
        Invoke(nameof(WaitForNextSwitch), .5f);

        inGameManager.Timer(false);
        nextScene = sceneName;
        FadeIn();
    }

    public void SwitchSceneToMainMenu()
    {
        if (!canSwitch)
            return;

        lastScene = SceneManager.GetActiveScene().name;

        canSwitch = false;
        Invoke(nameof(WaitForNextSwitch), .5f);

        nextScene = mainMenuName;

        musicAudioSource.Stop();
        musicAudioSource.clip = menuMusic;
        musicAudioSource.Play();

        FadeIn();
    }

    private void WaitForNextSwitch()
    {
        canSwitch = true;
    }

    public void FadeIn()
    {
        fadeAnimator.Play("FadeIn");
    }

    public void FadeOut()
    {
        fadeAnimator.Play("FadeOut");
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(nextScene);

        //if (resetStats)
        //{
        //    ResetManagerStats();
        //    resetStats = false;
        //}

        nextScene = string.Empty;
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
