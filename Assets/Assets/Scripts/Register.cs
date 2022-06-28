using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using RSG;
using TMPro;

[System.Serializable]
public class DetailRegisterError
{
    public DataError[] detail;
}

[System.Serializable]
public class DataError
{
    public string[] loc;
    public string msg;
    public string type;
}
public class Register : MonoBehaviour
{
    [SerializeField] TMP_InputField full_name;
    [SerializeField] TMP_InputField email;
    [SerializeField] TMP_InputField password;
    WebNuruk webNuruk;
    DetailRegisterError responseErrAuth = new DetailRegisterError();

    void Start()
    {
        webNuruk = gameObject.GetComponent<WebNuruk>();
    }

    public void RegisterNuruk()
    {
        if((full_name.text != "") && (email.text != "") && (password.text != "")) {
            webNuruk.Register_Post(full_name.text, email.text, password.text).Then((res)=>{
                WebNuruk.User_datos_authRes = res;
                Debug.Log(JsonUtility.ToJson(res)+" res");
            }).Catch((err) => {
                var error = err as RequestException;
                responseErrAuth = JsonUtility.FromJson<DetailRegisterError>(error.Response);
                Debug.Log(JsonUtility.ToJson(responseErrAuth));
                /// falta mostrar el error
                /* Debug.Log(error);
                Debug.Log(JsonUtility.ToJson(error)); */
            });
        }
    }
}
