using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Serializable]
    public class Chapter
    {
        public List<GameObject> levels;
    }

    public List<Image> saveButtons;

    public List<GameObject> hideOnSelectSave;
    public List<GameObject> showOnSelectSave;
    public List<GameObject> chaptersGo;
    public List<Chapter> chapters;

    private int chapterIndexSelected;
    private int levelIndexSelected;

    public void PlaySave(int index)
    {
        GameManagerNG.instance.SelectSave(index);

        if (GameManagerNG.instance.saveDatas[index] == null || !GameManagerNG.instance.saveDatas[index].saved)
        {
            Debug.Log("Started for the first time");


            SaveData saveData = new SaveData();
            saveData.saved = true;

            SaveSystem.SaveSave("Save_" + index, saveData);


            foreach (GameObject go in hideOnSelectSave)
            {
                go.SetActive(false);
            }

            GameManagerNG.instance.PlayFirstLevel(saveData);
        }
        else
        {
            foreach(GameObject go in hideOnSelectSave)
            {
                go.SetActive(false);
            }

            foreach (GameObject go in showOnSelectSave)
            {
                go.SetActive(true);
            }
        }
    }

    public void LoadSaves()
    {
        GameManagerNG.instance.LoadDatas();

        for (int i = 0; i < GameManagerNG.instance.saveDatas.Length; i++)
        {
            if (GameManagerNG.instance.saveDatas[i] != null)
            {
                if(GameManagerNG.instance.saveDatas[i].saved)
                {
                    saveButtons[i].color = Color.yellow;
                }
                else
                {
                    saveButtons[i].color = Color.grey;
                }
            }
            else
            {
                saveButtons[i].color = Color.grey;
            }
        }
    }

    public void DeleteSelectedSave()
    {
        GameManagerNG.instance.DeleteSelectedSave();
    }

    public void SetChapterVisible(bool visible)
    {
        for (int i = 0; i < chaptersGo.Count; i++)
        {
            chaptersGo[i].SetActive(visible);

            if(i <= GameManagerNG.instance.GetSelectedSave().chapter)
            {
                chaptersGo[i].GetComponentInChildren<Button>().interactable = true;
            }
            else
            {
                chaptersGo[i].GetComponentInChildren<Button>().interactable = false;
            }
        }
    }

    public void PlayChapter(int chapterIndex)
    {
        chapterIndexSelected = chapterIndex;

        // First start
        if (GameManagerNG.instance.GetSelectedSave().levelsCompleteInLastChapter == 0)
        {
            Debug.Log("Started chapter for the first time");
            return;
        }

        for (int i = 0; i < chapters[chapterIndex].levels.Count; i++)
        {
            if (i < GameManagerNG.instance.GetSelectedSave().levelsCompleteInLastChapter + 1)
            {
                chapters[chapterIndex].levels[i].GetComponentInChildren<Button>().interactable = true;
            }
            else
            {
                chapters[chapterIndex].levels[i].GetComponentInChildren<Button>().interactable = false;
            }
        }
    }

    public void SelectLevel(int levelIndex)
    {
        levelIndexSelected = levelIndex;
    }

    public void PlayLevel()
    {
        GameManagerNG.instance.PlayLevel(chapterIndexSelected, levelIndexSelected);
    }

    public void PlayFullRun()
    {
        GameManagerNG.instance.PlayFirstLevelOfChapter(chapterIndexSelected);
    }
}
