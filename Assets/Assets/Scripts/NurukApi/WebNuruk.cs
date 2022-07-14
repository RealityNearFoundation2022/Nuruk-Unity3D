using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using RSG;

[System.Serializable]
public class UserData_login
{
   public string username;
   public string password;
}

[System.Serializable]
public class UserData_login_Response
{
   public string access_token;
   public string token_type;
}

[System.Serializable]
public class UserData_AuthRequest
{
   public string full_name;
   public string email;
   public string password;
}

[System.Serializable]
public class UserData_authResponse
{
   public string email;
   public bool is_active;
   public bool is_superuser;
   public string full_name;
   public int id;
}

[System.Serializable]
public class DetailError
{
   public string detail;
}

[System.Serializable]
public class BugReport_Response
{
   public string category;
   public string title;
   public string description;
   public int status;
   public string image;
   public int id;
   public int owner_id;
}

[System.Serializable]
public class BugReport_Data
{
   public string category;
   public string title;
   public string description;
   public int status;
   public string image;
}

public class WebNuruk : MonoBehaviour
{
   UserData_login data_login = new UserData_login();
   UserData_AuthRequest User_datos = new UserData_AuthRequest();
   public static BugReport_Response bug_response = new BugReport_Response();
   public static UserData_login_Response login_Response = new UserData_login_Response();
   public static UserData_authResponse User_datos_authRes = new UserData_authResponse();

#if UNITY_EDITOR
   private readonly string baseUri = "http://216.128.138.227/";
#endif
#if UNITY_WEBGL && !UNITY_EDITOR
    private readonly string baseUri = "http://216.128.138.227/";
#endif
#if UNITY_STANDALONE && !UNITY_EDITOR
    private readonly string baseUri = "http://216.128.138.227/";
#endif


   private RequestHelper currentRequest;

   public RSG.IPromise<UserData_authResponse> Register_Post(string full_name, string email, string password)
   {
      string jsonString_Post_Authe;

      User_datos.full_name = full_name;
      User_datos.email = email;
      User_datos.password = password;

      jsonString_Post_Authe = JsonUtility.ToJson(User_datos);
      currentRequest = new RequestHelper
      {
         Uri = baseUri + "api/v1/users/open",
         BodyString = jsonString_Post_Authe,
      };
      return RestClient.Post<UserData_authResponse>(currentRequest);
   }


   public RSG.IPromise<UserData_login_Response> Login_Post(string email, string password)
   {
      RestClient.DefaultRequestHeaders["Content-Type"] = "application/x-www-form-urlencoded";

      data_login.username = email;
      data_login.password = password;
      WWWForm wWW = new WWWForm();
      wWW.AddField("username", email);
      wWW.AddField("password", password);
      currentRequest = new RequestHelper
      {
         Uri = baseUri + "api/v1/login/access-token",
         FormData = wWW
      };
      return RestClient.Post<UserData_login_Response>(currentRequest);
   }

   public RSG.IPromise<BugReport_Response> BugReport_Post(BugReport_Data bug)
   {
      string jsonString;
      //token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE2NTcyMzM2MzAsInN1YiI6IjkifQ.dJUVscZ0e7vNJHU67UJFFs3EX13W6bSQK0FNPlBuQ6Y"
      RestClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + login_Response.access_token);
      jsonString = JsonUtility.ToJson(bug);
      currentRequest = new RequestHelper
      {
         Uri = baseUri + "api/v1/reports",
         BodyString = jsonString,
      };
      return RestClient.Post<BugReport_Response>(currentRequest);
   }
}
