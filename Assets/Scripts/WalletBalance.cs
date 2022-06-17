using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class WalletBalance : MonoBehaviour
{
    [SerializeField] Text userName;
    [SerializeField] Text balance;
    [DllImport("__Internal")]
    private static extern string GetBalance();
    
    [DllImport("__Internal")]
    private static extern string GetAccounID();
    string saldo;
    public void Balance()
    {
        Debug.Log(GetBalance());
     //   Debug.Log("----");
        Debug.Log(GetAccounID());
    }
    private void Update() {
      saldo = GetBalance();
        userName.text = GetAccounID();
        balance.text = saldo;
    }

}
