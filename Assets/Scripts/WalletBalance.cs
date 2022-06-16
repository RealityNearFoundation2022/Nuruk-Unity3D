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

    //var staked;
    public void Balance()
    {
      //  staked = GetBalance();
        Debug.Log(GetBalance());
        Debug.Log("----");
        Debug.Log(GetAccounID());
        userName.text = GetAccounID();
        balance.text = GetBalance();
    }

}
