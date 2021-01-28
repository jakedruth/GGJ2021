using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlusMinusAttribute : PropertyAttribute
{
    public int clampMin;
    public int clampMax;
    
    public PlusMinusAttribute(int min = int.MinValue, int max = int.MaxValue)
    {
        clampMin = min;
        clampMax = max;
    }
}
