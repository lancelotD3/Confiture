using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ChaptersDataBase", menuName = "ChaptersDataBase", order = 1)]
public class ChaptersDataBase : ScriptableObject
{
    [Serializable]
    public class Chapter
    {
        public List<string> levels;
    }

    public List<Chapter> chapters;

}
