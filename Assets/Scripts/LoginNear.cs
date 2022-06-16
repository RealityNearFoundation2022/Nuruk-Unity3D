using System.Runtime.InteropServices;
using UnityEngine;

public class LoginNear : MonoBehaviour
{

    [DllImport("__Internal")]
    public static extern void Login();
   

    public void InitLogin()
    {
        Login();
    }
    

    public void Register()
    {
        Application.OpenURL("https://wallet.near.org");
    }

}
