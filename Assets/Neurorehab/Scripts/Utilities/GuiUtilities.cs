using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Neurorehab.Scripts.Utilities
{
    public class GuiUtilities : MonoBehaviour
    {
        private static GuiUtilities _instance;

        public static GuiUtilities Instance
        {
            get { return _instance; }
            set
            {
                if (_instance != null)
                {
                    Destroy(value);
                    return;
                }
                _instance = value;
            }
        }

        private void Awake()
        {
            Instance = this;
        }


        public void CanvasUpdate(ScrollRect scroller, float down = 0f)
        {
            StartCoroutine(CRCanvasUpdate(scroller, down));
        }


        private IEnumerator CRCanvasUpdate(ScrollRect scroller, float down = 0f)
        {
            Canvas.ForceUpdateCanvases();
            yield return null;

            scroller.verticalScrollbar.value = down;
            Canvas.ForceUpdateCanvases();
        }

        public static void ShowHideWithScale(RectTransform rectTransform)
        {
            rectTransform.localScale = rectTransform.localScale == Vector3.one ? Vector3.zero : Vector3.one;
        }
    }
}