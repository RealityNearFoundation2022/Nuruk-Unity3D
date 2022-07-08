using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject popUpIniciarSesionWallet;
    public void GoToLoginNear()
    {
        SceneManager.LoadScene("LoginNear");
    }
  /*   public void ClosePopUp()
    {
        popUpIniciarSesionWallet.SetActive(false);
    } */
}
