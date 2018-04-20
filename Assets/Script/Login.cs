/*************************************************************************************
 * CLR version      : version 5.6.2f1
 * TargetFW Version : 5.0
 * Class Name       :
 * Machine Name     : SC-201609201141 Hp Win7
 * Name Space       :
 * File Name        :
 * Create Date      : 2017/06/09 14:01:32
 * Author           : XieJiulong
 *
 * Modify Date      :
 * Author           :
 * Description      :
 *************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using cn.sharesdk.unity3d;
using UnityEngine;

public class Login : MonoBehaviour
{
    private ShareSDK ssdk;

    public wxUserInofo wxinfo;

    public GameObject webviewPrefab;

    void Start()
    {
        ssdk = ShareSDKManager.Instance.ssdk;
        ssdk.authHandler = OnAuthResultHandler;

        ssdk.shareHandler = OnShareResultHandler;
        ssdk.showUserHandler = OnGetUserInfoResultHandler;
        ssdk.getFriendsHandler = OnGetFriendsResultHandler;
        ssdk.followFriendHandler = OnFollowFriendResultHandler;
    }

    public void OnSinaLoginButtonClick()
    {
        //检测指定平台是否授权
        bool bAuth = ssdk.IsAuthorized(PlatformType.WeChat);
        if (bAuth)
        {
            Utility.WriteFile(Application.persistentDataPath, "AuthInfo.dat", ssdk.GetAuthInfo(PlatformType.WeChat).toJson());
            Utility.MakeToast("bAuth用户：" + ssdk.GetAuthInfo(PlatformType.WeChat)["userName"] + "登录成功");
            UnityEngine.SceneManagement.SceneManager.LoadScene(2);
        }
        else
        {
            // 使用指定平台授权 
            SdkLogin(PlatformType.WeChat, "0", 0);
        }
    }

    public void SdkLogin(PlatformType platform, string strip, int port, bool bfirst = false)
    {
        Debug.Log("请求SdkLogin登录");
        if (ssdk != null)
        {
            if (!ssdk.IsAuthorized(platform))
            {
                if (!bfirst)
                {
                    ssdk.Authorize(platform);
                }
            }
            else
            {
                ssdk.GetUserInfo(platform);
            }
        }
    }

    public void SdkLoginOutWeChat()
    {
        if (ssdk != null)
        {
            ssdk.CancelAuthorize(PlatformType.WeChat);
            Utility.MakeToast("注销成功");
        }
    }

    public void OnWebTest()
    {
        GameObject go = Instantiate(webviewPrefab);
        UniWebView webView = go.GetComponent<UniWebView>();
        if (webView != null)
        {
            webView.OnLoadComplete += OnLoadComplete;
            webView.InsetsForScreenOreitation += InsetsForScreenOreitation;
            webView.OnReceivedMessage += OnReceivedMessage;
            webView.OnEvalJavaScriptFinished += OnEvalJavaScriptFinished;
            webView.toolBarShow = true;

            webView.url = "http://uniwebview.onevcat.com/demo/index.html";
            webView.Load();
        }
    }

    void OnEvalJavaScriptFinished(UniWebView webView, string r)
    {
        Utility.MakeToast(r);
        //result.text = r;
    }

    void OnReceivedMessage(UniWebView webView, UniWebViewMessage message)
    {
        // You can check the message path and arguments to know which `uniwebview` link is clicked.
        // UniWebView will help you to parse your link if it follows the url argument format.
        // However, there is also a "rawMessage" property you could use if you need to use some other formats and want to parse it yourself.
        Utility.MakeToast("OnRecvivedMessage:" +　message.rawMessage + "\n" + message.path);
        Debug.LogError("OnRecvivedMessage:" + message.rawMessage);
        if (message.path == "close")
        {
            //result.text = "";
            Utility.MakeToast("close");
            Destroy(webView);
            //_webView = null;
        }

        if (message.path == "add")
        {
            int num1 = 0;
            int num2 = 0;

            // num1 and num2 will be got from the url argument.
            if (int.TryParse(message.args["num1"], out num1) && int.TryParse(message.args["num2"], out num2))
            {
                int sum = num1 + num2;
                //result.text = num1 + " + " + num2 + " = " + sum;
                Utility.MakeToast("sum = " + sum.ToString());
            }
            else
            {
                //result.text = "Invalid Input";
            }
        }
    }

    void OnLoadComplete(UniWebView webView, bool success, string errorMessage)
    {
        if (success)
        {
            Utility.MakeToast("加载页面完成");
            webView.Show();
        }
        else
        {
            Debug.Log("Something wrong in webview loading: " + errorMessage);
        }
    }

    // This method will be called when the screen orientation changed. Here we return UniWebViewEdgeInsets(5,5,5,5)
    // for both situation, which means the inset is 5 point for iOS and 5 pixels for Android from all edges.
    // Note: UniWebView is using point instead of pixel in iOS. However, the `Screen.width` and `Screen.height` will give you a
    // pixel-based value. 
    // You could get a point-based screen size by using the helper methods: `UniWebViewHelper.screenHeight` and `UniWebViewHelper.screenWidth` for iOS.
    UniWebViewEdgeInsets InsetsForScreenOreitation(UniWebView webView, UniWebViewOrientation orientation)
    {
        if (orientation == UniWebViewOrientation.Portrait)
        {
            return new UniWebViewEdgeInsets(50, 50, 50, 50);
        }
        else
        {
            return new UniWebViewEdgeInsets(50, 50, 50, 50);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reqID"></param>
    /// <param name="state">授权状态</param>
    /// <param name="type">对应的平台</param>
    /// <param name="data">返回的数据</param>
    void OnAuthResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable data)
    {
        if (state == ResponseState.Success)
        {
            Utility.WriteFile(Application.persistentDataPath, "AuthResult.dat", data.toJson());
            Debug.Log("授权成功");
            Utility.MakeToast("授权成功");
            Utility.MakeToast("微信用户：" + ssdk.GetAuthInfo(PlatformType.WeChat)["userName"] + "登录成功");
            // 授权成功后通过 GetAuthInfo获取授权信息
            Utility.WriteFile(Application.persistentDataPath, "AuthInfo.dat", ssdk.GetAuthInfo(PlatformType.WeChat).toJson());

            ssdk.GetUserInfo(type);

            //UnityEngine.SceneManagement.SceneManager.LoadScene(2);
        }
        else if (state == ResponseState.Fail)
        {
            Debug.Log("授权失败");
            Utility.MakeToast("授权失败");
            ssdk.CancelAuthorize(type);
        }
        else if (state == ResponseState.Cancel)
        {
            Debug.Log("授权取消");
            Utility.MakeToast("授权取消");
            ssdk.CancelAuthorize(type);
        }
        else
        {
            Utility.MakeToast("unknow error");
            Debug.Log("no state");
        }
    }

    void OnShareResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
    {
        if (state == ResponseState.Success)
        {
            Debug.Log("share successfully - share result :");
            Debug.Log(MiniJSON.jsonEncode(result));
        }
        else if (state == ResponseState.Fail)
        {
#if UNITY_ANDROID
            Debug.Log("fail! throwable stack = " + result["stack"] + "; error msg = " + result["msg"]);
#elif UNITY_IPHONE
			Debug.Log ("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
#endif
        }
        else if (state == ResponseState.Cancel)
        {
            Debug.Log("cancel !");
        }
    }

    void OnGetFriendsResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
    {
        if (state == ResponseState.Success)
        {
            Debug.Log("get friend list result :");
            Debug.Log(MiniJSON.jsonEncode(result));
        }
        else if (state == ResponseState.Fail)
        {
#if UNITY_ANDROID
            Debug.Log("fail! throwable stack = " + result["stack"] + "; error msg = " + result["msg"]);
#elif UNITY_IPHONE
			Debug.Log ("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
#endif
        }
        else if (state == ResponseState.Cancel)
        {
            Debug.Log("cancel !");
        }
    }

    void OnFollowFriendResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
    {
        if (state == ResponseState.Success)
        {
            Debug.Log("Follow friend successfully !");
        }
        else if (state == ResponseState.Fail)
        {
#if UNITY_ANDROID
            Debug.Log("fail! throwable stack = " + result["stack"] + "; error msg = " + result["msg"]);
#elif UNITY_IPHONE
			Debug.Log ("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
#endif
        }
        else if (state == ResponseState.Cancel)
        {
            Debug.Log("cancel !");
        }
    }

    void OnGetUserInfoResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
    {
        if (state == ResponseState.Success)
        {       /*{"country":"CN", "province":"", "headimgurl":"http://wx.qlogo.cn/mmopen/iahEuFAk0ZNLXuxniaia7KXkxooTUA7ae5vvFHicnJrm6bN12p1oJPwS8XN7zc8oKABeON7n0tzWh9v3RRCCkJl2iaSAoy8FTywRR/0", 
    "unionid":"obQEvv1BzL6Gy8GbyHKh5BO_AMZo", "openid":"oDtixwmTFkRG4td6iq23DYoIxxr8", "nickname":"\u725b\u6e38\u79d1\u6280", "city":"", "sex":0, "language":"zh_CN", "privilege":[]}*/

            //TipManager.Instance.ShowTip("请求连接服务器");
            Debug.Log(MiniJSON.jsonEncode(result));

            string strjson = MiniJSON.jsonEncode(result);
            Utility.MakeToast(strjson);
            wxinfo = new wxUserInofo(strjson);
        }
        else if (state == ResponseState.Fail)
        {
#if UNITY_ANDROID
            Debug.Log("fail! throwable stack = " + result["stack"] + "; error msg = " + result["msg"]);
            Utility.MakeToast("fail! throwable stack = " + result["stack"] + "; error msg = " + result["msg"]);
#elif UNITY_IPHONE
			Debug.Log ("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
#endif
        }
        else if (state == ResponseState.Cancel)
        {
            Debug.Log("cancel !");
        }
    }

    public class wxUserInofo
    {
        public wxUserInofo(string strjson)
        {
            strjson = uncode(strjson);
            try
            {
                Utility.MakeToast("开始构造微信用户");
                Debug.LogError("---wxUserInfo:" + strjson);
                JSONObject json = new JSONObject(strjson);
                openid = json["openid"].str;
                nickname = Utility.UnicodeToString2(json["nickname"].str);
                if (nickname == "" && json["nickname"].str != "")
                {
                    nickname = json["nickname"].str;
                }
                if (nickname == "")
                {
                    nickname = openid;
                }
                Debug.Log("wechat nick name:" + nickname);
                sex = (byte)json["sex"].i;
                province = json["province"].str;
                city = json["city"].str;
                country = json["country"].str;
                headimgurl = json["headimgurl"].str;
                unionid = json["unionid"].str;

                Utility.MakeToast(string.Format("获取微信用户信息：nickname->{0} openid->{1} sex->{2} province->{3} city->{4} country->{5} headimgurl->{6} unionid->{7}",
                    nickname,
                    openid,
                    sex,
                    province,
                    city,
                    country,
                    headimgurl,
                    unionid));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.LogError("wxUserInofo parse err:" + e.Message);
                Utility.MakeToast(string.Format("wxUserInofo parse err:{0}", e.Message));
            }

        }
        //可以包括其他字符         
        public string uncode(string str)
        {
            string outStr = "";
            Regex reg = new Regex(@"(?i)\\u([0-9a-f]{4})");
            outStr = reg.Replace(str, delegate (Match m1)
            {
                return ((char)Convert.ToInt32(m1.Groups[1].Value, 16)).ToString();
            });
            return outStr;
        }

        public string openid;
        public string nickname;
        public byte   sex;
        public string province;
        public string city;
        public string country;
        public string headimgurl;
        public string privilege1;
        public string privilege2;
        public string unionid;
        public Texture2D texturehead;
    }
}