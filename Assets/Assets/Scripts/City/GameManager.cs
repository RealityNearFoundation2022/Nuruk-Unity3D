using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject miniTutorial;
    
    void Start()
    {
        StartCoroutine(WaitMiniTutorial());
    }
    IEnumerator WaitMiniTutorial(){
        yield return new WaitForSeconds(10f);
        miniTutorial.SetActive(false);
    }
    public void CursorManagerOFF(){
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void CursorManagerON(){
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void GoToLoginNear()
    {
        Debug.Log("click");
        SceneManager.LoadScene("LoginNear");
    }
}
