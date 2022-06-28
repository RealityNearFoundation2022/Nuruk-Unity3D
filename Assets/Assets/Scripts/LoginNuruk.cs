using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using RSG;
using TMPro;

[System.Serializable]
public class DetailLoginError
{
    public DataError[] detail;
}

/* [System.Serializable]
public class DataError
{
    public string[] loc;
    public string msg;
    public string type;
} */
public class LoginNuruk : MonoBehaviour
{
    [SerializeField] TMP_InputField email;
    [SerializeField] TMP_InputField password;
    DetailLoginError responseErrAuth = new DetailLoginError();

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
                Debug.Log(JsonUtility.ToJson(res));

            }).Catch((err) => {
                var error = err as RequestException;

                Debug.Log(JsonUtility.ToJson(error.Response));
            });
        }else{
            Debug.Log("campos en blanco");
        }
    }
}
