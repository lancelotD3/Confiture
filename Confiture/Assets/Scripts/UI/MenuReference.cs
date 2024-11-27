using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuReference : MonoBehaviour
{
    public List<GameObject> menus;

    public int GetIdOfMenu(GameObject go)
    {
        int id = 0;

        foreach (GameObject menu in menus)
        {
            if (menu == go)
                return id;
            id++;
        }

        return -1;
    }

    public GameObject GetMenu(int id) => menus[id];
}
