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
using System.Security.Policy;
using UnityEngine;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

public static class Utility
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <param name="name"></param>
    /// <param name="info"></param>
    public static void WriteFile(string path, string name, string info)
    {
        StreamWriter sw;
        FileInfo fi = new FileInfo(path+"/"+name);
        sw = fi.CreateText();
        sw.WriteLine(info);
        sw.Close();
        sw.Dispose();
    }

    public static string ReadFile(string path, string name)
    {
        StreamReader sw;
        FileInfo fi = new FileInfo(path + "/" + name);
        sw = fi.OpenText();
        string info = sw.ReadLine();
        sw.Close();
        sw.Dispose();

        return info;
    }

    public static void MakeToast(string info)
    {
        AndroidJavaObject currentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaClass Toast = new AndroidJavaClass("android.widget.Toast");
        currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(
            () =>
            {
                Toast.CallStatic<AndroidJavaObject>("makeText", currentActivity, info, Toast.GetStatic<int>("LENGTH_LONG")).Call("show");
            }
            ));

        //AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        //AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        //AndroidJavaClass toast = new AndroidJavaClass("android.widget.Toast");
        //AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
        //currentActivity.Call("runOnUiThread",
        //    new AndroidJavaRunnable(() =>
        //    {
        //        toast.CallStatic<AndroidJavaObject>("makeText",
        //            context,
        //            info,
        //            toast.GetStatic<int>("LENGTH_LONG")).Call("show");
        //    })
        //);
    }

    public static string UnicodeToString(string unicode)
    {
        Regex reg = new Regex(@"(?i)\\[uU]([0-9a-f]{4})");
        return reg.Replace(unicode, delegate(Match m)
        {
            return ((char) Convert.ToInt32(m.Groups[1].Value, 16)).ToString();
        });
    }

    public static string UnicodeToString2(string sIn)
    {
        if (!sIn.Contains("\\u"))
        {
            return sIn;
        }
        StringBuilder sOut = new StringBuilder();

        string[] arr = sIn.Split(new string[] { "\\u" }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string s in arr)
        {
            sOut.Append((char)Convert.ToInt32(s.Substring(0, 4), 16) + s.Substring(4));
        }
        return sOut.ToString();
    }
}