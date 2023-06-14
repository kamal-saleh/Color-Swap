using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LineController : MonoBehaviour
{
    public List<CircleController> connectedCircles = new List<CircleController>();
    
    [HideInInspector] public bool checkTemp;

    public void SwapCircle(CircleController oldCircle, CircleController newCircle)
    {
        int index = connectedCircles.IndexOf(oldCircle);
        if (index >= 0)
        {
            connectedCircles[index] = newCircle;
        }
    }

    public bool CheckLines()
    {
        bool result = true;
        Color checkColor = connectedCircles[0].color;

        var circle = connectedCircles[1];

        if (circle.color == checkColor)
        {
            result = false;
        }
        else
        {
            result = true;
        }
        return result;
    }
}