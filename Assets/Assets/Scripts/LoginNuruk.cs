using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using RSG;
using TMPro;

public class LoginNuruk : MonoBehaviour
{
    [SerializeField] TMP_InputField email;
    [SerializeField] TMP_InputField password;
    DetailError responseErrAuth = new DetailError();

    WebNuruk webNuruk;

    void Start()
    {
        webNuruk = gameObject.GetComponent<WebNuruk>();
    }

    public void Log_in()
    {
        if((email.text != "") && (password.text != "")) {
            webNuruk.Login_Post(email.text, password.text).Then((res) => {
                WebNuruk.login_Response = res;
            }).Catch((err) => {
                var error = err as RequestException;
                responseErrAuth = JsonUtility.FromJson<DetailError>(error.Response);
                Debug.Log(responseErrAuth.detail);
            });
        }else{
            Debug.Log("campos en blanco");
        }
    }
}
