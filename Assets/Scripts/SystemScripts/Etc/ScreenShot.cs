using System.IO;
using System.Collections;
using UnityEngine;
using System;

public static class ScreenShot
{
    public static bool IsCapturing { get; private set; }

    public static KeyCode captureKeyCode = KeyCode.G;

    private static readonly string screenShotFolderPath = "Assets/ScreenShot/";

   public static void StartScreenShot(int width = -1, int height = -1)
   {
        if (captureKeyCode == KeyCode.None) captureKeyCode = KeyCode.G;

        if (IsCapturing) return;
        IsCapturing = true;

        if (width == -1) width = Screen.width;
        if(height == -1) height = Screen.height;

        //ScreenCapture.CaptureScreenshot(string.Concat(screenShotFolderPath, "GameScreenShot", DateTime.Now.ToString(), ".png"));

        GameManager.Instance.StartCoroutine(ScreenShotCo(width, height));
   }

   private static IEnumerator ScreenShotCo(int width, int height)
   {
        yield return new WaitForEndOfFrame();

        Texture2D renderResult = new Texture2D(width, height, TextureFormat.ARGB32, false);
        Rect rect = new Rect(0, 0, width, height);
        renderResult.ReadPixels(rect, 0, 0);
        
        SavePNG(renderResult);

        Util.MainCam.targetTexture = null;
        IsCapturing = false;
        yield break;
    }

    private static void SavePNG(Texture2D screenTex)
    {
        if(!Directory.Exists(screenShotFolderPath))
        {
            Directory.CreateDirectory(screenShotFolderPath);
        }

        //string filePath = string.Concat(screenShotFolderPath, "GameScreenShot", DateTime.Now.ToString(), ".png");
        string filePath = string.Concat(screenShotFolderPath, "GameScreenShot", DateTime.Now.ToString().Replace(':',' '), UnityEngine.Random.Range(0,99999), ".png");
        byte[] bytes = screenTex.EncodeToPNG();

        File.WriteAllBytes(filePath, bytes);
    }

}
