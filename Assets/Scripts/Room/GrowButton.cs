using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowButton : SelectableItem
{

    public float RoomGrowToScale = 1f;

    public void Press()
    {
        GameManager.Instance.Room.GrowToScale(RoomGrowToScale);
    }
}
