using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Proyecto26;
using UnityEngine.SceneManagement;
using RSG;
using TMPro;

public class Register : MonoBehaviour
{
    [SerializeField] TMP_InputField full_name;
    [SerializeField] TMP_InputField email;
    [SerializeField] TMP_InputField password;
    WebNuruk webNuruk;
    DetailError responseErrAuth = new DetailError();
    [SerializeField] Text ErrorMessage;

    void Start()
    {
        webNuruk = gameObject.GetComponent<WebNuruk>();
        ErrorMessage.enabled = false;
    }

    public void RegisterNuruk()
    {
        ErrorMessage.enabled = false;

        if((full_name.text != "") && (email.text != "") && (password.text != "")) {
            webNuruk.Register_Post(full_name.text, email.text, password.text).Then((res)=>{
                WebNuruk.User_datos_authRes = res;
                Debug.Log(JsonUtility.ToJson(res));
                Log_in();
            }).Catch((err) => {
                Debug.Log("err");
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

    public void Log_in()
    {
        ErrorMessage.enabled = false;

        if((email.text != "") && (password.text != "")) {
            webNuruk.Login_Post(email.text, password.text).Then((res) => {
                WebNuruk.login_Response = res;
                Debug.Log(JsonUtility.ToJson(res));
                SceneManager.LoadScene("City");
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
