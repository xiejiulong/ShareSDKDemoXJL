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

public class Play : MonoBehaviour
{
    public Text resultText;
    private ShareSDK ssdk;

    private int mineral;
    private int gas;
    private int friendBuff = 0;


	void Start ()
	{
	    ssdk = ShareSDKManager.Instance.ssdk;
	    ssdk.getFriendsHandler = OnGetFriendResultHandler;
	    ssdk.shareHandler = OnShareResultHandler;
	}

    public void OnFriendButtonClick()
    {
        // 获取当前登录用户指定平台的好友列表，一页多少个，第多少页。
        // 目前只能准确获取好友总数，后两个参数已失效。
        ssdk.GetFriendList(PlatformType.SinaWeibo, 15, 0);
    }

    public void OnPlayButtonClick()
    {
        mineral = Random.Range(5, 1000) + friendBuff;
        gas = Random.Range(50, 1000) + friendBuff;
        resultText.text = "恭喜你获得了\n" + mineral + "晶矿" + gas + "瓦斯\n" + "（好友加成：" + friendBuff + ")";
    }

    public void OnSignOutButtonClick()
    {
        // 取消指定平台的授权
        ssdk.CancelAuthorize(PlatformType.SinaWeibo);
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    // 分享图文信息
    public void OnShareButtonClick()
    {
        // 创建屏幕截图
        Application.CaptureScreenshot("Screenshot.png");

        // 分享前先创建content对象
        ShareContent content = new ShareContent();
        // 设置分享的正文字
        content.SetText(resultText.text);
        // 设置分享的图片
        // SetImagePath用于本地图片，参数为图片路径
        // SetImageUrl 用于网络图片，参数为图片网址
        // SetImageArray 用于多图分享，参数为图片网址的string数组，仅支持网络图片，仅支持Android
        content.SetImagePath(Application.persistentDataPath + "/Screenshot.png");
        // 设置分享的标题与标题的url
        content.SetTitle("标题");
        content.SetTitleUrl("http://www.sikiedu.com");
        content.SetSite("站点");
        content.SetSiteUrl("http://www.sikiedu.com");
        //content.SetMusicUrl();
        content.SetUrl("http://www.sikiedu.com");
        // 设定分享类型的主要类型
        content.SetShareType(ContentType.Image);

        // 平台特异性分享内容的设置
        ShareContent sinaWeiBoContent = new ShareContent();
        sinaWeiBoContent.SetText(resultText.text + "\n via 新浪微博");

        // 在指定平台上，使用第二个参数的内容去覆盖主内容的值
        content.SetShareContentCustomize(PlatformType.SinaWeibo, sinaWeiBoContent);

        // 显示分享框
        PlatformType[] platforms = {PlatformType.SinaWeibo,PlatformType.QQ, PlatformType.WeChat};
        //ssdk.ShowPlatformList(platforms, content, 100, 100);
        string[] platformList =
            {"5", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21","24","25","26","27","28","29","30","31","32","33","34","35","36","37","38","39","40","41","42","43","44","45","46","47","48","49","50","51","52","53","54"};
        content.SetHidePlatforms(platformList);
        ssdk.ShowPlatformList(null, content, 100, 100);
    }

    void OnGetFriendResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable data)
    {
        if (state == ResponseState.Success)
        {
            Utility.WriteFile(Application.persistentDataPath, "FriendsList.dat", data.toJson());
            friendBuff = int.Parse(data["total_number"].ToString()) ;
            Utility.MakeToast("获得好友加成："+ friendBuff.ToString());
        }
        else if (state == ResponseState.Fail)
        {
            Utility.MakeToast("获取用户好友:失败");
        }
        else if (state == ResponseState.Cancel)
        {
            Utility.MakeToast("获取用户好友:被取消");
        }
        else
        {
            Utility.MakeToast("获取用户好友:错误的标志");
        }
    }

    void OnShareResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable data)
    {
        if (state == ResponseState.Success)
        {
            Utility.MakeToast("分享:成功");
        }
        else if (state == ResponseState.Fail)
        {
            Utility.MakeToast("分享:失败");
        }
        else if (state == ResponseState.Cancel)
        {
            Utility.MakeToast("分享::被取消");
        }
        else
        {
            Utility.MakeToast("分享::错误的标志");
        }
    }

}
