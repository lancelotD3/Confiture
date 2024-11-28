using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
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
    public GameObject saveButtonsGo;
    public GameObject selectChaptersGo;

    public List<GameObject> hideOnSelectSave;
    public List<GameObject> showOnSelectSave;
    public List<GameObject> chaptersGo;
    public List<Chapter> chapters;

    private int chapterIndexSelected;
    private int levelIndexSelected;

    public MenuReference menuRef;

    [Space]

    [Header("Stats refs")]
    public TMP_Text deathsText;
    public TMP_Text killsText;
    public TMP_Text levelsCompleteText;
    public TMP_Text overallTimeText;
    public TMP_Text runsText;
    public TMP_Text doorsUsedText;
    public TMP_Text dashesUsedText;
    public TMP_Text shootsText;
    public TMP_Text jumpsText;

    [Header("Cosmetics")]
    public Slider R;
    public Slider G;
    public Slider B;
    public Image exampleTexture;

    private void Start()
    {
        if(InGameManager.instance != null)
        {
            Destroy(InGameManager.instance.gameObject);
        }

        GameManagerNG.instance.FadeOut();

        if(GameManagerNG.instance.lastMenuOpenedId != -1 )
        {
            LoadSaves();
            PlaySave(GameManagerNG.instance.selectedSaveIndex);

            foreach (GameObject go in hideOnSelectSave)
            {
                go.SetActive(false);
            }
            foreach (GameObject go in showOnSelectSave)
            {
                go.SetActive(false);
            }

            SetChapterVisible(true);
            PlayChapter(GameManagerNG.instance.lastChapterOpenedId);

            ShowLastMenuSelected();

            // 0 = defaults buttons
            menuRef.GetMenu(0).SetActive(false);
        }
    }

    public void PlaySave(int index)
    {
        GameManagerNG.instance.SelectSave(index);

        if (GameManagerNG.instance.saveDatas[index] == null || !GameManagerNG.instance.saveDatas[index].saved)
        {
            SaveData saveData = new SaveData();
            saveData.saved = true;

            SaveSystem.SaveSave("Save_" + index, saveData);


            foreach (GameObject go in hideOnSelectSave)
            {
                go.SetActive(false);
            }

            GameManagerNG.instance.PlayFirstLevel(saveData);
            Invoke(nameof(SelectChapterMenu), .2f);
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

            deathsText.text = GameManagerNG.instance.GetSelectedSave().death.ToString();
            killsText.text = GameManagerNG.instance.GetSelectedSave().kills.ToString();
            levelsCompleteText.text = GameManagerNG.instance.GetSelectedSave().levelsComplete.ToString();
            overallTimeText.text = GameManagerNG.instance.GetSelectedSave().overallTime.ToString();
            runsText.text = GameManagerNG.instance.GetSelectedSave().runNumber.ToString();
            doorsUsedText.text = GameManagerNG.instance.GetSelectedSave().openedDoors.ToString();
            dashesUsedText.text = GameManagerNG.instance.GetSelectedSave().dashNumber.ToString();
            shootsText.text = GameManagerNG.instance.GetSelectedSave().shootNumber.ToString();
            jumpsText.text = GameManagerNG.instance.GetSelectedSave().jumpNumber.ToString();
        }
    }

    private void SelectChapterMenu()
    {
        SetMenuSelected(selectChaptersGo);
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

                TMP_Text chronoText = GetTextChild(chaptersGo[i], "Chrono");
                chronoText.gameObject.SetActive(true);

                if(GameManagerNG.instance.GetSelectedSave().chronoChapter[i] > 0.01f)
                {
                    chronoText.text = GameManagerNG.instance.GetSelectedSave().chronoChapter[i].ToString();
                }
            }
            else
            {
                chaptersGo[i].GetComponentInChildren<Button>().interactable = false;

                TMP_Text chronoText = GetTextChild(chaptersGo[i], "Chrono");
                chronoText.gameObject.SetActive(false);
            }
        }
    }

    public void PlayChapter(int chapterIndex)
    {
        chapterIndexSelected = chapterIndex;

        // First start
        if (chapterIndexSelected >= GameManagerNG.instance.GetSelectedSave().chapter && GameManagerNG.instance.GetSelectedSave().levelsCompleteInLastChapter == 0)
        {
            GameManagerNG.instance.PlayFirstLevelOfChapter(chapterIndexSelected);

            return;
        }

        // Chapter allready finish
        if(chapterIndexSelected < GameManagerNG.instance.GetSelectedSave().chapter)
        {
            for (int i = 0; i < chapters[chapterIndexSelected].levels.Count; i++)
            {
                TMP_Text chronoText = GetTextChild(chapters[chapterIndexSelected].levels[i], "Chrono");
                chronoText.gameObject.SetActive(true);

                if (GameManagerNG.instance.GetSelectedChronoLevel(chapterIndexSelected, i) > 0.01f)
                {
                    chronoText.text = GameManagerNG.instance.GetSelectedChronoLevel(chapterIndexSelected, i).ToString();
                }

                chapters[chapterIndexSelected].levels[i].GetComponentInChildren<Button>().interactable = true;
            }
        }
        else // Chapter not finish
        {
            for (int i = 0; i < chapters[chapterIndexSelected].levels.Count; i++)
            {
                if (i < GameManagerNG.instance.GetSelectedSave().levelsCompleteInLastChapter + 1)
                {
                    chapters[chapterIndexSelected].levels[i].GetComponentInChildren<Button>().interactable = true;

                    TMP_Text chronoText = GetTextChild(chapters[chapterIndexSelected].levels[i], "Chrono");
                    chronoText.gameObject.SetActive(true);

                    if (GameManagerNG.instance.GetSelectedChronoLevel(chapterIndexSelected, i) > 0.01f)
                    {
                        chronoText.text = GameManagerNG.instance.GetSelectedChronoLevel(chapterIndexSelected, i).ToString();
                    }
                }
                else
                {
                    TMP_Text chronoText = GetTextChild(chapters[chapterIndexSelected].levels[i], "Chrono");
                    chronoText.gameObject.SetActive(false);

                    chapters[chapterIndexSelected].levels[i].GetComponentInChildren<Button>().interactable = false;
                }
            }
        }
    }

    public void SelectLevel(int levelIndex)
    {
        levelIndexSelected = levelIndex;
    }

    public void PlayLevel(int levelIndex)
    {
        levelIndexSelected = levelIndex;
        GameManagerNG.instance.PlayLevel(chapterIndexSelected, levelIndexSelected);
    }

    public void PlayFullRun()
    {
        GameManagerNG.instance.AddRun();
        GameManagerNG.instance.PlayFirstLevelOfChapter(chapterIndexSelected);
    }

    private TMP_Text GetTextChild(GameObject go,  string name)
    {
        if (go != null)
        {
            return go.transform.Find(name).GetComponent<TMP_Text>();
        }
        return null;
    }

    public void SetMenuSelected(GameObject menuGo)
    {
        GameManagerNG.instance.lastMenuOpenedId = menuRef.GetIdOfMenu(menuGo);
    }

    public void ShowLastMenuSelected()
    {
        menuRef.GetMenu(GameManagerNG.instance.lastMenuOpenedId).SetActive(true);

        if (GameManagerNG.instance.lastMenuOpenedId == menuRef.GetIdOfMenu(selectChaptersGo))
        {
            SetMenuSelected(saveButtonsGo);
        }
    }

    public void OpenCosmetics()
    {
        R.value = GameManagerNG.instance.GetSelectedSave().RGB[0];
        G.value = GameManagerNG.instance.GetSelectedSave().RGB[1];
        B.value = GameManagerNG.instance.GetSelectedSave().RGB[2];

        exampleTexture.color = new Color(R.value, G.value, B.value, 1f);
    }

    public void ChangeColor()
    {
        exampleTexture.color = new Color(R.value, G.value, B.value, 1f);

        GameManagerNG.instance.GetSelectedSave().RGB[0] = R.value;
        GameManagerNG.instance.GetSelectedSave().RGB[1] = G.value;
        GameManagerNG.instance.GetSelectedSave().RGB[2] = B.value;
    }

    public void ExitCosmetics()
    {
        GameManagerNG.instance.SaveSelectedData();
    }
}
