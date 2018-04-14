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

using System.Collections;
using System.Collections.Generic;
using cn.sharesdk.unity3d;
using UnityEngine;

public class Login : MonoBehaviour
{
    private ShareSDK ssdk;

    void Start()
    {
        ssdk = ShareSDKManager.Instance.ssdk;
        ssdk.authHandler = OnAuthResultHandler;
    }

    public void OnSinaLoginButtonClick()
    {
        //检测指定平台是否授权
        bool bAuth = ssdk.IsAuthorized(PlatformType.SinaWeibo);
        if (bAuth)
        {
            Utility.WriteFile(Application.persistentDataPath, "AuthInfo.dat", ssdk.GetAuthInfo(PlatformType.SinaWeibo).toJson());
            Utility.MakeToast("bAuth 微薄用户：" + ssdk.GetAuthInfo(PlatformType.SinaWeibo)["userName"] + "登录成功");
            UnityEngine.SceneManagement.SceneManager.LoadScene(2);
        }
        else
        {
            // 使用指定平台授权 
            ssdk.Authorize(PlatformType.SinaWeibo);
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
            Utility.MakeToast("OnAuthResultHandler 微薄用户：" + ssdk.GetAuthInfo(PlatformType.SinaWeibo)["userName"] + "登录成功");
            // 授权成功后通过 GetAuthInfo获取授权信息
            Utility.WriteFile(Application.persistentDataPath, "AuthInfo.dat", ssdk.GetAuthInfo(PlatformType.SinaWeibo).toJson());
            UnityEngine.SceneManagement.SceneManager.LoadScene(2);
        }
        else if (state == ResponseState.Fail)
        {
            Debug.Log("授权失败");
            ssdk.CancelAuthorize(type);
        }
        else if (state == ResponseState.Cancel)
        {
            Debug.Log("授权取消");
            ssdk.CancelAuthorize(type);
        }
        else
        {
            Debug.Log("no state");
        }
    }
}