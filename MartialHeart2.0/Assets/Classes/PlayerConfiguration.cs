using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class PlayerConfiguration
{
    public PlayerConfiguration(PlayerInput pi)
    {
        PlayerIndex = pi.playerIndex;
        Input = pi;
    }
    public PlayerConfiguration(int index)
    {
        PlayerIndex = index;
    }
    public PlayerInput Input { get; set; }
    public int PlayerIndex { get; set; }

    //Propiedades del jugador (personaje, color, etc)
    public Character character { get; set; }

    public int skinIndex { get; set; }

    
}
