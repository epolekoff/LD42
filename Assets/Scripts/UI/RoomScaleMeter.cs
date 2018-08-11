using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomScaleMeter : MonoBehaviour
{
    public Image Fill;
    public Gradient FillGradient;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Fill.fillAmount = GameManager.Instance.Room.ScaleRatio;
        Fill.color = FillGradient.Evaluate(1-Fill.fillAmount);
    }
}