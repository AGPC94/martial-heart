using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character")]
public class Character : ScriptableObject
{
    [Header("AudioVisual")]

    public new string name;
    public string description;
    public RuntimeAnimatorController animator;
    public Skin[] skins = new Skin[2];
    public Sprite artwork;
    public Sprite portrait;

    [Header("Stats")]

    public float atk;

    public float def;

    [Header("Movement")]

    public float walkSpeed;

    public float dashSpeed;

    public float dashTime;

    [Header("Clash")]

    public float clashDistance;

    public float clashStartSpeed;

    public float clashBackDashSpeed;

    public float clashBackDashTime;

    [Header("Push")]

    public float pushTime;

    public float pushForce;

    public float wallDistance;

    [Header("Combat")]

    public float hitStun;

    public float blockStun;

    public float parryTime;

    [Header("Sounds")]

    public string slash;

    public string death;

    public string clashEnter;

    public string clashStay;

    public string unsheath;

    public string sheath;

    public string sheathEnd;

    public string parry;

    public string swoosh;
}
