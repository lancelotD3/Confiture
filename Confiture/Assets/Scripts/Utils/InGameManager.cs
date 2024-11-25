using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameManager : MonoBehaviour
{
    private string nextScene;
    static public InGameManager instance;

    [Header("UI")]
    public TMP_Text timerText;
    public TMP_Text enemyRemainsText;
    public TMP_Text blobNumberText;
    public TMP_Text levelNameText;

    public List<GameObject> canvas;

    public float gameTimer = 0f;
    private bool lockTimer = true;
    private bool timerActivate = false;

    [HideInInspector] public int enemyRemaining = 0;

    private bool canReset = true;
    private bool resetStats = false;
    PlayerEntity player;

    public AudioClip killAllEnemiesClip;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (!player) player = FindAnyObjectByType<PlayerEntity>();

        if (Input.anyKey && !lockTimer && !timerActivate)
        {
            timerActivate = true;
        }

        if (timerActivate)
        {
            gameTimer += Time.deltaTime;
        }

        if (player && Input.GetKeyDown(KeyCode.R) && canReset)
        {
            player.RemoveBlobs(1000);
        }

        timerText.text = gameTimer.ToString("0.00");
        enemyRemainsText.text = enemyRemaining.ToString();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            resetStats = true;
            GameManagerNG.instance.SwitchSceneToMainMenu();

            //if (!(SceneManager.GetActiveScene().name == "MainMenu"))
            //{
            //    resetStats = true;
            //    SwitchScene("MainMenu");
            //    //ResetManagerStats();
            //}
        }
    }

    public void StartLevel()
    {
        player = FindAnyObjectByType<PlayerEntity>();

        Timer(true);

        levelNameText.text = SceneManager.GetActiveScene().name.Replace("_", " ");

        enemyRemaining = FindObjectsByType<Enemy>(FindObjectsSortMode.None).Count();

        enemyRemainsText.text = enemyRemaining.ToString();
    }

    public void PlayerDied()
    {
        Timer(false);
        GameManagerNG.instance.SwitchScene(SceneManager.GetActiveScene().name);

        LockReset();
        Invoke(nameof(UnlockReset), 2f);
    }

    private void LockReset()
    {
        canReset = false;
    }

    private void UnlockReset()
    {
        canReset = true;
    }

    public void ResetManagerStats()
    {
        enemyRemaining = 0;
        nextScene = string.Empty;
        gameTimer = 0;
        Timer(false);

        foreach (GameObject go in canvas)
        {
            if (go)
                go.SetActive(true);
        }
    }

    public void RemoveEnemy()
    {
        enemyRemaining--;
        enemyRemainsText.text = enemyRemaining.ToString();

        if (enemyRemaining == 0)
        {
            GameManagerNG.instance.PlaySound(killAllEnemiesClip);
            GameObject.FindGameObjectWithTag("ExitDoor").GetComponent<Door>().UseDoor(true);
        }
    }

    public void Timer(bool active)
    {
        lockTimer = !active;

        if (!active) timerActivate = false;
    }
}
