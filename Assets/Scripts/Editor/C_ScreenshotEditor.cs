using UnityEngine;
using UnityEditor;
using System;

public class C_ScreenshotEditor
{
    [MenuItem("Leviathan/ScreenShot")]
    private static void ScreenShot()
    {
        string tp_fileName = $"ScreenShot_{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}.png";
        ScreenCapture.CaptureScreenshot(tp_fileName);
        Debug.Log($"ScreenShot captured : {tp_fileName}");
    }
}
