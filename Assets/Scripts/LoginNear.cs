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

}
