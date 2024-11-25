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

    // Cosmetics
    public float R;
    public float G;
    public float B;

    // Chapter

    float[] chrono_1;
    float[] chrono_2;

    public SaveData(int chapter, int levelsCompleteInLastChapter, int death, int kills, int levelsComplete, float overallTime, int runNumber, int openedDoors, int dashNumber, int shootNumber, float r, float g, float b)
    {
        this.chapter = chapter;
        this.levelsCompleteInLastChapter = levelsCompleteInLastChapter;
        this.death = death;
        this.kills = kills;
        this.levelsComplete = levelsComplete;
        this.overallTime = overallTime;
        this.runNumber = runNumber;
        this.openedDoors = openedDoors;
        this.dashNumber = dashNumber;
        this.shootNumber = shootNumber;
        R = r;
        G = g;
        B = b;
    }

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
        R = 0;
        G = 0;
        B = 0;
    }
}

