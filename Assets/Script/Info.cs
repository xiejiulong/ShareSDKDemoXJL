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
using UnityEngine.UI;

public class Info : MonoBehaviour
{
    public Image userIcon;
    public Text userName;
    public Text userID;

    private ShareSDK ssdk;

	void Start ()
	{
	    ssdk = ShareSDKManager.Instance.ssdk;
	    ssdk.showUserHandler = OnGetUserInfoResultHandler;

	    Hashtable authInfo = Utility.ReadFile(Application.persistentDataPath, "AuthInfo.dat").hashtableFromJson();
	    userName.text = authInfo["userName"].ToString();
	    userID.text = authInfo["userID"].ToString();
        Utility.MakeToast("用户名称：" + userName.text + " 用户ID:"+userID.text);

	    StartCoroutine(DownLoadUserIcon(authInfo["userIcon"].ToString()));
	}

    IEnumerator DownLoadUserIcon(string url)
    {
        WWW www = new WWW(url);
        yield return www;

        if (www.isDone && www.error == null)
        {
            Texture2D text2D = www.texture;
            userIcon.sprite = Sprite.Create(text2D, new Rect(0, 0, text2D.width, text2D.height), Vector2.zero);
        }
    }

    public void OnEnterButtonClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(3);
    }

    public void OnDetailButtonClick()
    {
        ssdk.GetUserInfo(PlatformType.WeChat);
    }

    public void OnSignOutButtonClick()
    {
        // 取消指定平台的授权
        ssdk.CancelAuthorize(PlatformType.WeChat);
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    /*
     * {
     * "country":"CN", 
     * "province":"Jiangsu", 
     * "headimgurl":"http://thirdwx.qlogo.cn/mmopen/vi_32/DYAIOgq83eqia1ia8X0cdqEibvoWwKrRBTonZcRSxQgNXJlRg6EULfPzia4kFd1uicBItwibQ1kqpnmoIyDaUtCE2uFg/132", 
     * "unionid":"oc3_ew2FcFW6O_Y2NA6B_-wQqZHI", 
     * "openid":"odJuv1DHgTNn_1iAKioKRvFO-Fb8", 
     * "nickname":"\u8c22\u4e5d\u9f99", 
     * "city":"Suzhou", 
     * "sex":1, 
     * "language":"zh_CN", 
     * "privilege":[]
     * }
     */

    void OnGetUserInfoResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable data)
    {
        if (state == ResponseState.Success)
        {
            Utility.WriteFile(Application.persistentDataPath, "UserInfo.dat", data.toJson());

            Utility.MakeToast("获取用户详细信息:成功[您的城市：]"+ Utility.UnicodeToString(data["city"].ToString()));
        }
        else if (state == ResponseState.Fail)
        {
            Utility.MakeToast("获取用户详细信息:失败");
        }
        else if (state == ResponseState.Cancel)
        {
            Utility.MakeToast("获取用户详细信息:被取消");
        }
        else
        {
            Utility.MakeToast("获取用户详细信息:错误的标志");
        }
    }
}
