
public class TypeConverters
{
    public enum SupportedType { Bool, Int, Float, String };

    public static bool ConvertAsBool(bool inValue)
    {
        return inValue;
    }

    public static int ConvertAsInt(bool inValue)
    {
        return inValue ? 1 : 0;
    }

    public static float ConvertAsFloat(bool inValue)
    {
        return inValue ? 1.0f : 0.0f;
    }

    public static string ConvertAsString(bool inValue)
    {
        return inValue ? "V" : "X";
    }

    public static bool ConvertAsBool(int inValue)
    {
        return inValue != 0;
    }

    public static int ConvertAsInt(int inValue)
    {
        return inValue;
    }

    public static float ConvertAsFloat(int inValue)
    {
        return inValue;
    }

    public static string ConvertAsString(int inValue)
    {
        return "" + inValue;
    }

    public static bool ConvertAsBool(float inValue)
    {
        return swantiez.unity.tools.utils.FloatUtils.IsPreciselyNotZero(inValue);
    }

    public static int ConvertAsInt(float inValue)
    {
        return (int)inValue;
    }

    public static float ConvertAsFloat(float inValue)
    {
        return inValue;
    }

    public static string ConvertAsString(float inValue)
    {
        return string.Format("{0:0.##}", inValue);
    }

    public static bool ConvertAsBool(string inValue)
    {
        return inValue != "";
    }

    public static int ConvertAsInt(string inValue)
    {
        return int.Parse(inValue);
    }

    public static float ConvertAsFloat(string inValue)
    {
        return float.Parse(inValue);
    }

    public static string ConvertAsString(string inValue)
    {
        return inValue;
    }

    public static void Convert(bool inValue, out bool outValue)
    {
        outValue = inValue;
    }

    public static void Convert(bool inValue, out int outValue)
    {
        outValue = ConvertAsInt(inValue);
    }

    public static void Convert(bool inValue, out float outValue)
    {
        outValue = ConvertAsFloat(inValue);
    }

    public static void Convert(bool inValue, out string outValue)
    {
        outValue = ConvertAsString(inValue);
    }

    public static void Convert(int inValue, out bool outValue)
    {
        outValue = ConvertAsBool(inValue);
    }

    public static void Convert(int inValue, out int outValue)
    {
        outValue = inValue;
    }

    public static void Convert(int inValue, out float outValue)
    {
        outValue = ConvertAsFloat(inValue);
    }

    public static void Convert(int inValue, out string outValue)
    {
        outValue = ConvertAsString(inValue);
    }

    public static void Convert(float inValue, out bool outValue)
    {
        outValue = ConvertAsBool(inValue);
    }

    public static void Convert(float inValue, out int outValue)
    {
        outValue = ConvertAsInt(inValue);
    }

    public static void Convert(float inValue, out float outValue)
    {
        outValue = inValue;
    }

    public static void Convert(float inValue, out string outValue)
    {
        outValue = ConvertAsString(inValue);
    }

    public static void Convert(string inValue, out bool outValue)
    {
        outValue = ConvertAsBool(inValue);
    }

    public static void Convert(string inValue, out int outValue)
    {
        outValue = ConvertAsInt(inValue);
    }

    public static void Convert(string inValue, out float outValue)
    {
        outValue = ConvertAsFloat(inValue);
    }

    public static void Convert(string inValue, out string outValue)
    {
        outValue = inValue;
    }
}
