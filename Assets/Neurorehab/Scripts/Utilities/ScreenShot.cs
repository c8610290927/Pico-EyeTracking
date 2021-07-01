using System;
using System.IO;
using UnityEngine;

namespace Neurorehab.Scripts.Utilities
{
    public class ScreenShot : MonoBehaviour
    {
        void Start()
        {
        }
        

        private void LateUpdate()
        {
            if (!Input.GetKeyDown(KeyCode.F12)) return;

            var directoryPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (Directory.Exists(directoryPath) == false)
            {
                Directory.CreateDirectory(directoryPath);
            }

            ScreenCapture.CaptureScreenshot(directoryPath + "\\" + DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss.fff") + ".png");
            print("screenshot shaved at " + directoryPath + "\\" + DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss.fff") + ".png");

            //var test = new ScreenShotThread();
            //var testThread = new Thread(test.SaveScreenshot);
            //testThread.Start();
        }
    }

    //class ScreenShotThread
    //{
    //    internal void SaveScreenshot()
    //    {
    //        Application.CaptureScreenshot("Screenshots/Screenshot" + DateTime.Now.ToString("ddMMyyyyHHmmssfff") + ".png");
    //    }
    //}
}