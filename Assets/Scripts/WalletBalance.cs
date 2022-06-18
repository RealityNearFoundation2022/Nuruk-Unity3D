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

    [DllImport("__Internal")]
    private static extern string BalanceWallet();

    string saldo;
    public void Balance()
    {
     //   Debug.Log("----");
        saldo = GetBalance();
        userName.text = GetAccounID();
        balance.text = saldo;
        Debug.Log(GetAccounID());
        Debug.Log(GetBalance());
        Debug.Log(BalanceWallet());
    }
    private void Update() {
      
    }

}
