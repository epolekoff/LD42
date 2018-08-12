using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseDoor : MonoBehaviour
{

    public void Open()
    {
        GetComponent<Animator>().enabled = true;
    }
}
