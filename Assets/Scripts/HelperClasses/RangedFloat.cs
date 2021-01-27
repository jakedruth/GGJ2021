[System.Serializable]
public struct RangedFloat
{
    public float minimum;
    public float maximum;

    public RangedFloat(float minimum, float maximum)
    {
        this.minimum = minimum;
        this.maximum = maximum;
    }

    public float Mean
    {
        get { return (minimum + maximum) / 2; }
    }

    public float GetRandomValue
    {
        get { return UnityEngine.Random.Range(minimum, maximum); }
    }

    public float Sum
    {
        get { return maximum + minimum; }
    }

    public float Difference
    {
        get { return maximum - minimum; }
    }
    
    public float Product
    {
        get { return minimum * maximum; }
    }

    public float Clamp(float value)
    {
        if (value < minimum)
            value = minimum;
        else if (value > maximum)
            value = maximum;

        return value;
    }

    public float Lerp(float t)
    {
        // V = A(1 - t) + Bt
        return minimum * (1f - t) + maximum * t;
    }

    public float InverseLerp(float value)
    {
        // V = A(1 - t) + Bt
        // V = A + -At + Bt
        // V - A = -At + Bt
        // (V - A) = t(-A + B)
        // (V - A) / (B - A) = t
        return (value - minimum) / (maximum - minimum);
    }
}