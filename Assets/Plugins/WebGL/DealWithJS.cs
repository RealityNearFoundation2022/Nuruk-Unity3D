using System.Runtime.InteropServices;
/// <summary>
/// Class with a JS Plugin functions for WebGL.
/// </summary>
public static class DealWithJS
{
    // Importing "CallFunction"
    [DllImport("__Internal")]
    public static extern void SendAuthData(string data);

    [DllImport("__Internal")]
    public static extern string ReceiveAuthData();

    [DllImport("__Internal")]
    public static extern void OpenURL(string url);

    [DllImport("__Internal")]
    public static extern void FileUpload(string activity);
}