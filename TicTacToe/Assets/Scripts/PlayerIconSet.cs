using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Scriptable object that contains the data for the different icons that players can choose
[CreateAssetMenu(fileName = "PlayerIcons", menuName = "Data/Icons", order = 1)]
public class PlayerIconSet : ScriptableObject
{
    public GameObject[] largeIcons;
    public GameObject[] smallIcons;
}
