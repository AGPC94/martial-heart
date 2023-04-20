using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stage", menuName = "Stage")]
public class Stage : ScriptableObject
{
    public new string name;
    public Sprite portrait;
    public Sprite preview;
    public Sprite background;
    public string music;

    public bool isUnlock;


}
