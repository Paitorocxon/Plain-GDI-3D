public static class MathExtensions
{
    public static int Clamp(int value, int min, int max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    public static float Clamp(float value, float min, float max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    public static double Clamp(double value, double min, double max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    public static decimal Clamp(decimal value, decimal min, decimal max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    // Methode, um ungerade Zahlen in gerade Zahlen zu konvertieren
    public static int EnsureEven(int value)
    {
        return value % 2 == 0 ? value : value + 1;
    }

    public static float EnsureEven(float value)
    {
        return value % 2 == 0 ? value : value + 1;
    }

    public static double EnsureEven(double value)
    {
        return value % 2 == 0 ? value : value + 1;
    }

    public static decimal EnsureEven(decimal value)
    {
        return value % 2 == 0 ? value : value + 1;
    }
}
