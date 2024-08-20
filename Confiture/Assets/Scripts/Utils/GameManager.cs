using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Animator fadeAnimator;
    static public GameManager instance;

    private string nextScene;
    [HideInInspector] public string lastScene;

    [Header("Timer")]
    public float timeForGold = 10f;
    public float timeForSilver = 20f;
    public float timeForBronze = 30f;

    [Header("UI")]
    public TMP_Text timerText;
    public TMP_Text enemyRemainsText;
    public TMP_Text blobNumberText;
    public TMP_Text levelNameText;

    public List<GameObject> canvas;

    public float gameTimer = 0f;
    private bool lockTimer = true;
    private bool timerActivate = false;

    public Vector3 offset;
    int BN_number;

    PlayerEntity player;

    [HideInInspector] public int enemyRemaining = 0;

    [Header("Audio")]
    public AudioSource musicAudioSource;
    public AudioSource audioSource;

    public AudioClip menuMusic;
    public AudioClip gameMusic;
    public AudioClip killAllEnemiesClip;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
     
        DontDestroyOnLoad(gameObject);

        lastScene = "MainMenu";
    }

    public void StartLevel()
    {
        player = FindAnyObjectByType<PlayerEntity>();

        if (!(SceneManager.GetActiveScene().name == "MainMenu"))
        {
            Timer(true);
        }

        if (!(SceneManager.GetActiveScene().name == "MainMenu") && (lastScene == "MainMenu"))
        {
            musicAudioSource.Stop();
            musicAudioSource.clip = gameMusic;
            musicAudioSource.Play();
        }
        else if((SceneManager.GetActiveScene().name == "MainMenu") && !(lastScene == "EndGame"))
        {
            musicAudioSource.Stop();
            musicAudioSource.clip = menuMusic;
            musicAudioSource.Play();
        }


        levelNameText.text = SceneManager.GetActiveScene().name.Replace("_", " ");
        enemyRemaining = FindObjectsByType<Enemy>(FindObjectsSortMode.None).Count();

        enemyRemainsText.text = enemyRemaining.ToString();
    }

    public void Timer(bool active)
    {
        lockTimer = !active;

        if(!active) timerActivate = false;
    }

    private bool resetStats = false;

    private void Update()
    {
        if(!player) player = FindAnyObjectByType<PlayerEntity>();

        if (Input.anyKey && !lockTimer && !timerActivate)
        {
            timerActivate = true;
        }

        if (timerActivate)
        {
            gameTimer += Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            player.RemoveBlobs(1000);
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.K))
        {
            if(!(SceneManager.GetActiveScene().name == "MainMenu"))
            {
                resetStats = true;
                SwitchScene("MainMenu");
                //ResetManagerStats();
            }
        }

        timerText.text = gameTimer.ToString("0.00");
        enemyRemainsText.text = enemyRemaining.ToString();
    }

    public void PlayerDied()
    {
        Timer(false);
        SwitchScene(SceneManager.GetActiveScene().name);
    }

    bool canSwitch = true;

    public void SwitchScene(string sceneName)
    {
        if (!canSwitch)
            return;

        lastScene = SceneManager.GetActiveScene().name;

        canSwitch = false;
        Invoke(nameof(WaitForNextSwitch), .5f);

        Timer(false);
        nextScene = sceneName;
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

        if (resetStats)
            ResetManagerStats();

        nextScene = string.Empty;
    }

    public void RemoveEnemy()
    {
        enemyRemaining--;
        enemyRemainsText.text = enemyRemaining.ToString();

        if(enemyRemaining == 0)
        {
            audioSource.PlayOneShot(killAllEnemiesClip);
            GameObject.FindGameObjectWithTag("ExitDoor").GetComponent<Door>().UseDoor(true);
        }
    }

    public void ResetManagerStats()
    {
        enemyRemaining = 0;
        nextScene = string.Empty;
        gameTimer = 0;
        Timer(false);

        foreach (GameObject go in canvas)
        {
            if(go)
                go.SetActive(true);
        }
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
