namespace ZLGCanComm.Api;

public sealed class ZLGApiProvider
{
    private static IZLGApi _instance = new ZLGApiWrapper();

    public static IZLGApi Instance
    {
        get => _instance;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            _instance = value;
        }
    }
}