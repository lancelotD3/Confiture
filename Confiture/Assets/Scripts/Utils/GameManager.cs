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

    [Header("UI")]
    public TMP_Text timerText;
    public TMP_Text enemyRemainsText;
    public TMP_Text blobNumberText;

    public float gameTimer = 0f;
    private bool lockTimer = true;
    private bool timerActivate = false;

    public Vector3 offset;
    int BN_number;

    PlayerEntity player;

    [HideInInspector] public int enemyRemaining = 0;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
     
        DontDestroyOnLoad(gameObject);
    }

    public void StartLevel()
    {
        player = FindAnyObjectByType<PlayerEntity>();
        Timer(true);

        enemyRemaining = FindObjectsByType<Enemy>(FindObjectsSortMode.None).Count();

        enemyRemainsText.text = enemyRemaining.ToString();
    }

    public void Timer(bool active)
    {
        lockTimer = !active;

        if(!active) timerActivate = false;
    }

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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(!(SceneManager.GetActiveScene().name == "MainMenu"))
            {
                SwitchScene(SceneManager.GetActiveScene().name);
                ResetManagerStats();
            }
        }

        if(player)
            blobNumberText.text = player.blobNumber.ToString();

        timerText.text = gameTimer.ToString();
        enemyRemainsText.text = enemyRemaining.ToString();
    }

    public void PlayerDied()
    {
        Timer(false);
        SwitchScene(SceneManager.GetActiveScene().name);
    }

    public void SwitchScene(string sceneName)
    {
        Timer(false);
        nextScene = sceneName;
        FadeIn();
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
        nextScene = string.Empty;
    }

    public void RemoveEnemy()
    {
        enemyRemaining--;
        enemyRemainsText.text = enemyRemaining.ToString();

        if(enemyRemaining == 0)
        {
            GameObject.FindGameObjectWithTag("ExitDoor").GetComponent<Door>().UseDoor(true);
        }
    }

    private void ResetManagerStats()
    {
        enemyRemaining = 0;
        nextScene = string.Empty;
        gameTimer = 0;
        Timer(false);
    }
}
