using System.Runtime.InteropServices;
using UnityEngine;

public class LoginNear : MonoBehaviour
{

    [DllImport("__Internal")]
    public static extern void Login();
    [DllImport("__Internal")]
    private static extern void GetBalance();

    public void InitLogin()
    {
        Login();
    }
    public void Balance()
    {
        GetBalance();
    }

    public void Register()
    {
        Application.OpenURL("https://wallet.near.org");
    }

}
