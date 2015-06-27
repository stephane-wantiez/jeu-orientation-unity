public class LocalizedMessage
{
    public string localizationKey;
    public object[] messageParameters;

    public LocalizedMessage(string localizationKey, params object[] messageParameters)
    {
        this.localizationKey = localizationKey;
        this.messageParameters = messageParameters;
    }

    public string getLocalizedMessage()
    {
        return LocalizationUtils.GetLocalizedMessage(localizationKey, messageParameters);
    }

    public override string ToString()
    {
        return getLocalizedMessage();
    }
}
