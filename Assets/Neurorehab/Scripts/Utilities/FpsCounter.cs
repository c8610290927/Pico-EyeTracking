using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace Neurorehab.Scripts.Utilities
{
    public class FpsCounter : MonoBehaviour
    {
        public Text FpsDisplay;

        public float updateInterval = 0.5F;
        
        private int frames = 0; // Frames drawn over the interval

        private void Start()
        {
            StartCoroutine(FpsRoutine());
        }

        private IEnumerator FpsRoutine()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            yield return new WaitForSecondsRealtime(2); // waits for 2 seconds to let framerate stabilize
            while (true)
            {
                ++frames;
                var format = string.Format("{0:F2} FPS", 0);
                FpsDisplay.text = format;

                // Interval ended - update GUI text and start new interval
                if (stopWatch.ElapsedMilliseconds >= updateInterval * 1000)
                {
                    stopWatch.Stop();
                    // display two fractional digits (f2 format)
                    var fps = frames / (stopWatch.ElapsedMilliseconds / 1000f);
                    format = string.Format("{0:F2} FPS", fps);
                    FpsDisplay.text = format;

                    if (fps < 30)
                        FpsDisplay.color = Color.yellow;
                    else
                    if (fps < 10)
                        FpsDisplay.color = Color.red;
                    else
                        FpsDisplay.color = Color.green;
                    frames = 0;
                    stopWatch.Reset();
                    stopWatch.Start();
                }

                yield return null;
            }
        }
    }
}