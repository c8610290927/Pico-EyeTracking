using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace LabVisualization.EyeTracing
{
    public class EyeTracingVisualController : VisualControllerBase
    {
        private EyeTracingVisualConfig _config;

        public GameObject EyeTracinGameObject;

        private IEyeTracingPos _iEyeTracingPos;

        public override void VisualInit()
        {
            _iEyeTracingPos = FindObjectsOfType<MonoBehaviour>().OfType<IEyeTracingPos>().First();
            //此处初始化可以通过配置表和读取资源动态生成
            _config = new EyeTracingVisualConfig(EyeTracinGameObject, 0.2f);
        }

        public override void VisualShow()
        {
            StartCoroutine(EyeTracingVisualIEnumerator());
        }

        private IEnumerator EyeTracingVisualIEnumerator()
        {
            Debug.Log(Screen.height+"/////////////"+ Screen.width);
            while (_iEyeTracingPos.GetEyeTracingPos() != null)
            {
                
                var pos = _iEyeTracingPos.GetEyeTracingPos().Invoke();
                float x = (Mathf.Clamp(pos.x,0.2f,0.8f) - 0.2f) / (0.8f - 0.2f);
                float y = (Mathf.Clamp(pos.y, 0.2f, 0.8f) - 0.2f) / (0.8f - 0.2f);
                _config.EyetracingTextureObject.transform.localPosition =
                    new Vector2(Mathf.Lerp(_config.EyetracingTextureObject.transform.position.x, x * Screen.width, _config.EyetracingSmooth)
                        , Mathf.Lerp(_config.EyetracingTextureObject.transform.position.y, (1 - y) * Screen.height, _config.EyetracingSmooth));

                yield return new WaitForFixedUpdate();
            }
        }

        public override void VisualDispose()
        {
            StopAllCoroutines();
            _iEyeTracingPos = null;
            _config = null;
        }
    }
}

