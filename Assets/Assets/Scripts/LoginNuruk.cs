using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using RSG;
using TMPro;
using UnityEngine.UI;

public class LoginNuruk : MonoBehaviour
{
    [SerializeField] TMP_InputField email;
    [SerializeField] TMP_InputField password;
    DetailError responseErrAuth = new DetailError();
    [SerializeField] Text ErrorMessage;

    WebNuruk webNuruk;

    void Start()
    {
        webNuruk = gameObject.GetComponent<WebNuruk>();
        ErrorMessage.enabled = false;
    }

    public void Log_in()
    {
        ErrorMessage.enabled = false;

        if((email.text != "") && (password.text != "")) {
            webNuruk.Login_Post(email.text, password.text).Then((res) => {
                WebNuruk.login_Response = res;
                Debug.Log(JsonUtility.ToJson(res));
            }).Catch((err) => {
                var error = err as RequestException;
                responseErrAuth = JsonUtility.FromJson<DetailError>(error.Response);
                ErrorMessage.enabled = true;
                ErrorMessage.text = responseErrAuth.detail;
            });
        }else{
            ErrorMessage.enabled = true;
            ErrorMessage.text = "Fields can't be empty";
        }
    }
}
