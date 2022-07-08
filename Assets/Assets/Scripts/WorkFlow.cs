using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorkFlow : MonoBehaviour
{
    void Start()
    {
        
    }

    void Gologin()
    {
        SceneManager.LoadScene("LoginNuruk");
    }
     void GoRegister()
    {
        SceneManager.LoadScene("Register");
    }
}
