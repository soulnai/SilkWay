using UnityEngine;
using System.Collections;

namespace EnumSpace
{

    public enum GameStates
    {
        GlobalMap = 0,
        EventWindow = 1,
        PlayerMovement = 2,
        GameMenu = 3
    }

    public enum EventType
    {
        Nothing = 0,
        Trade = 1,
        Fight = 2,
        Luck = 3,
        Misfortune = 4,
        Choice = 5
    }
}