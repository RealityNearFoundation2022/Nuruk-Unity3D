using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WalletBalance : MonoBehaviour
{
  [SerializeField] Text userName;
  [SerializeField] Text balance;
  
  [DllImport("__Internal")]
  private static extern string GetAccountID();

  [DllImport("__Internal")]
  private static extern string BalanceWallet();

 
  string saldo;

  void Start() {
    StartCoroutine(startWallet());
  }
  IEnumerator startWallet(){
    yield return new WaitForSeconds(2f);
    Balance();
  }
  public void Balance()
  {
    saldo = BalanceWallet();
    userName.text = GetAccountID();
    balance.text = "Realities: " + saldo;
  }
}
