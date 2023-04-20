using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skin", menuName = "Skin")]
public class Skin : ScriptableObject
{
    [Header("Main Texture")]
    public Texture2D mainTexture;

    [Header("Color 1")]
    public Color color1;
    public Color newColor1;

    [Header("Color 2")]
    public Color color2;
    public Color newColor2;

    [Header("Color 3")]
    public Color color3;
    public Color newColor3;
}
