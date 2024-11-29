using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public bool saved;


    // Game
    public int chapter;
    public int levelsCompleteInLastChapter;

    // Stats
    public int death;
    public int kills;
    public int levelsComplete;
    public float overallTime;
    public int runNumber; // pas compris mais oklm
    public int openedDoors;
    public int dashNumber;
    public int shootNumber;
    public int jumpNumber;

    // Cosmetics
    public float[] RGB;

    // Chapter
    public float[] chronoChapter;
    public float[,] chronos;

    public SaveData()
    {
        this.saved = false;
        this.chapter = 0;
        this.levelsCompleteInLastChapter = 0;
        this.death = 0;
        this.kills = 0;
        this.levelsComplete = 0;
        this.overallTime = 0;
        this.runNumber = 0;
        this.openedDoors = 0;
        this.dashNumber = 0;
        this.shootNumber = 0;
        this.jumpNumber = 0;

        RGB = new float[3];
        RGB[0] = 0f;
        RGB[1] = 1f;
        RGB[2] = 0.38f;

        chronos = new float[10, 10];
        chronoChapter = new float[10];
    }
}

