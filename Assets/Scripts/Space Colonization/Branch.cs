using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Branch : MonoBehaviour
{
    public Vector3 growDirection;
    public Vector3 originalGrowDirection;
    public int growCount;

    public void Reset()
    {
        growCount = 0;
        growDirection = originalGrowDirection;
    }
}
