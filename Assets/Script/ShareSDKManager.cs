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
using UnityEngine;
using cn.sharesdk;
using cn.sharesdk.unity3d;

public class ShareSDKManager : MonoBehaviour
{
    private static ShareSDKManager _instance;

    [HideInInspector]
    public ShareSDK ssdk;


    public static ShareSDKManager Instance
    {
        get { return _instance; }
    }

    void Start()
    {
        _instance = this;
        DontDestroyOnLoad(this.gameObject);

        ssdk = GetComponent<ShareSDK>();

        // 做完准备工作就可以跳转到下一个场景了
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }



}