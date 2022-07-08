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

  GameObject banner;
  string saldo;

  void Start() {
    StartCoroutine(startWallet());
    banner = gameObject.transform.GetChild(0).gameObject;
    banner.SetActive(false);
  }

  IEnumerator startWallet(){
    yield return new WaitForSeconds(3f);
    Balance();
  }

  public void Balance()
  {
    saldo = BalanceWallet();
    if (saldo != ""){
      banner.SetActive(true);
      userName.text = GetAccountID();
      balance.text = /* "Realities: " +  */saldo;
    }else{
      banner.SetActive(false);
    }
  }
}
