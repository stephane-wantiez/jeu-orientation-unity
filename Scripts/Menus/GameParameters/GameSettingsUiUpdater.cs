using UnityEngine;
using UnityEngine.UI;
using System;
using swantiez.unity.tools.utils;

public class GameSettingsUiUpdater : MonoBehaviour
{
    public string key;
    public TypeConverters.SupportedType type;
    private bool initialized = false;

    void Start()
    {
        refreshComponents();
    }

    private void refreshComponents()
    {
        bool componentFound = false;
        if (!componentFound) refreshSliderComponent(ref componentFound);
        if (!componentFound)   refreshEnumComponent(ref componentFound);
        if (!componentFound) refreshToggleComponent(ref componentFound);
        initialized = true;
    }

    private void refreshSliderComponent(ref bool componentFound)
    {
        Slider sliderComponent = GetComponent<Slider>();
        if (sliderComponent != null)
        {
            int sliderMinValue = 0, sliderMaxValue = 10;
            GameSettings.Instance.GetMinMaxIntValues(key, ref sliderMinValue, ref sliderMaxValue);
            sliderComponent.minValue = sliderMinValue;
            sliderComponent.maxValue = sliderMaxValue;

            float sliderValue = sliderComponent.value;
            InitComponentValue(ref sliderValue, TypeConverters.ConvertAsFloat,
                                                TypeConverters.ConvertAsFloat,
                                                TypeConverters.ConvertAsFloat,
                                                TypeConverters.ConvertAsFloat);
            sliderComponent.value = MathUtils.Clamp(sliderMinValue, sliderValue, sliderMaxValue);

            componentFound = true;
        }
    }

    private void refreshEnumComponent(ref bool componentFound)
    {
        GameSettingsUiEnumLocalizer enumLabelComponent = GetComponent<GameSettingsUiEnumLocalizer>();
        if (enumLabelComponent != null)
        {
            int enumValue = enumLabelComponent.Value;
            InitComponentValue(ref enumValue, TypeConverters.ConvertAsInt,
                                              TypeConverters.ConvertAsInt,
                                              TypeConverters.ConvertAsInt,
                                              TypeConverters.ConvertAsInt);
            enumLabelComponent.Value = enumValue;
            componentFound = true;
        }
    }

    private void refreshToggleComponent(ref bool componentFound)
    {
        Toggle toggleComponent = GetComponent<Toggle>();
        if (toggleComponent)
        {
            bool toggleValue = toggleComponent.isOn;
            InitComponentValue(ref toggleValue, TypeConverters.ConvertAsBool,
                                                TypeConverters.ConvertAsBool,
                                                TypeConverters.ConvertAsBool,
                                                TypeConverters.ConvertAsBool);
            toggleComponent.isOn = toggleValue;
            componentFound = true;
        }
    }

    private void InitComponentValue<T>(ref T value, Func<string, T> valueFromString, Func<int, T> valueFromInt, Func<float, T> valueFromFloat, Func<bool, T> valueFromBool)
    {
        GameSettings.DefaultValue defaultValue = GameSettings.Instance.GetDefaultValueForKey(key);

        if ((defaultValue != null) && (type == defaultValue.Type))
        {
	        switch (type)
	        {
	            case TypeConverters.SupportedType.String:
	                string valueAsStr = PlayerPrefs.GetString(key, defaultValue.ValueAsString);
	                value = valueFromString(valueAsStr);
	                break;
	            case TypeConverters.SupportedType.Int:
                    int valueAsInt = PlayerPrefs.GetInt(key, defaultValue.ValueAsInt);
                    value = valueFromInt(valueAsInt);
	                break;
	            case TypeConverters.SupportedType.Float:
                    float valueAsFloat = PlayerPrefs.GetFloat(key, defaultValue.ValueAsFloat);
                    value = valueFromFloat(valueAsFloat);
	                break;
	            case TypeConverters.SupportedType.Bool:
                    bool valueAsBool = TypeConverters.ConvertAsBool(PlayerPrefs.GetInt(key, TypeConverters.ConvertAsInt(defaultValue.ValueAsBool)));
                    value = valueFromBool(valueAsBool);
	                break;
	            default:
	                break;
	        }
        }
    }

    private void OnValueChange<T>(T value, Func<T, string> valueToString, Func<T, int> valueToInt, Func<T, float> valueToFloat, Func<T, bool> valueToBool)
    {
        if (initialized)
        {
	        switch (type)
	        {
	            case TypeConverters.SupportedType.String:
	                PlayerPrefs.SetString(key, valueToString(value));
	                break;
	            case TypeConverters.SupportedType.Int:
	                PlayerPrefs.SetInt(key, valueToInt(value));
	                break;
	            case TypeConverters.SupportedType.Float:
	                PlayerPrefs.SetFloat(key, valueToFloat(value));
	                break;
	            case TypeConverters.SupportedType.Bool:
	                PlayerPrefs.SetInt(key, TypeConverters.ConvertAsInt(valueToBool(value)));
	                break;
	            default:
	                break;
	        }
        }
    }

    public void OnStringValueChange(string value)
    {
        OnValueChange(value, TypeConverters.ConvertAsString,
                             TypeConverters.ConvertAsInt,
                             TypeConverters.ConvertAsFloat,
                             TypeConverters.ConvertAsBool);
    }

    public void OnIntValueChange(int value)
    {
        OnValueChange(value, TypeConverters.ConvertAsString,
                             TypeConverters.ConvertAsInt,
                             TypeConverters.ConvertAsFloat,
                             TypeConverters.ConvertAsBool);
    }

    public void OnFloatValueChange(float value)
    {
        OnValueChange(value, TypeConverters.ConvertAsString,
                             TypeConverters.ConvertAsInt,
                             TypeConverters.ConvertAsFloat,
                             TypeConverters.ConvertAsBool);
    }

    public void OnBoolValueChange(bool value)
    {
        OnValueChange(value, TypeConverters.ConvertAsString,
                             TypeConverters.ConvertAsInt,
                             TypeConverters.ConvertAsFloat,
                             TypeConverters.ConvertAsBool);
    }

    public void IncrementIntValue(int increment)
    {
        int value = 0;
        InitComponentValue(ref value, TypeConverters.ConvertAsInt,
                                      TypeConverters.ConvertAsInt,
                                      TypeConverters.ConvertAsInt,
                                      TypeConverters.ConvertAsInt);
        value += increment;

        int minValue = 0, maxValue = 0;
        if (GameSettings.Instance.GetMinMaxIntValues(key, ref minValue, ref maxValue))
        {
            if (value < minValue)
            {
                value = maxValue;
            }
            else if (value > maxValue)
            {
                value = minValue;
            }
        }
        OnIntValueChange(value);
        refreshComponents();
    }

    public void SetPreviousIntValue()
    {
        IncrementIntValue(-1);
    }

    public void SetNextIntValue()
    {
        IncrementIntValue(1);
    }
}
