using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vectrosity;
using UnityEngine.UI;

namespace LabVisualization.Cardiogram
{
    public class CardiogramVisualController : VisualControllerBase
    {
        private CardiogramVisualConfig _config;

        public int EnergyLineWidth;

        public int PointsInEnergyLine;

        private CardiogramPos cardiogramPos;

        //皮肤电
        private VectorLine energyLine;
        //心电
        private VectorLine heartLine;

        public Text Heartrate;

        public override void VisualInit()
        {
            //拿到心电图二位坐标
            cardiogramPos = FindObjectsOfType<MonoBehaviour>().OfType<CardiogramPos>().First();
            //实例化config
            _config = new CardiogramVisualConfig(EnergyLineWidth, PointsInEnergyLine);
            //实例化出来划线
            Line();
        }

        public override void VisualShow()
        {
            StartCoroutine(LineShow());
        }

        public override void VisualDispose()
        {
            cardiogramPos = null;
            _config = null;
        }

        private void Line()
        {
            //实例化皮肤电
            energyLine = new VectorLine("Hand", new List<Vector2>(1000), null, 4, LineType.Continuous);
            energyLine.color = Color.green;
            //实例化心电
            heartLine = new VectorLine("Haerts", new List<Vector2>(1000), null, 4, LineType.Continuous);
            heartLine.color = Color.green;
            //初始化皮肤电数值
            SetEnergyLinePoints();
            //初始化心电数值
            SetHeartLinePoints();

            //找到自动生成的心电
            GameObject Hand = GameObject.Find("Hand");
            Hand.transform.parent = this.gameObject.transform;
            Hand.GetComponent<RectTransform>().anchoredPosition = new Vector2(34, 113);
            Hand.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);


            //找到自动生成的皮肤电
            GameObject Heart = GameObject.Find("Haerts");
            Heart.transform.parent = this.gameObject.transform;
            Heart.GetComponent<RectTransform>().anchoredPosition = new Vector2(1231, 111);
            Heart.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);

        }

       

        private IEnumerator LineShow()
        {
            while (cardiogramPos.GetCardiogramPos()!=null)
            {
                var pos = cardiogramPos.GetCardiogramPos().Invoke();
                HandUpdate(pos.x/10);
                Heartrate.text = pos.y.ToString();
                HeartUpdate(pos.z/10);
                energyLine.Draw();
                heartLine.Draw();
                yield return new WaitForFixedUpdate();
            }
        }


        /// <summary>
        /// 初始化皮肤电数值
        /// </summary>
        private void SetEnergyLinePoints()
        {
            for (int i = 0; i < energyLine.points2.Count; i++)
            {
                var xPoint = Mathf.Lerp(70, Screen.width - 20, (i + 0.0f) / energyLine.points2.Count);
                energyLine.points2[i] = new Vector2(xPoint, 0);
            }
        }
        /// <summary>
        /// 初始化心电数值
        /// </summary>
        private void SetHeartLinePoints()
        {

            for (int i = 0; i < heartLine.points2.Count; i++)
            {
                var xPoint = Mathf.Lerp(70, Screen.width - 20, (i + 0.0f) / heartLine.points2.Count);
                heartLine.points2[i] = new Vector2(xPoint, 0);
            }
        }

        private void HandUpdate(float y)
        {
            int i;
            for (i = 0; i < energyLine.points2.Count - 1; i++)
            {
                energyLine.points2[i] = new Vector2(energyLine.points2[i].x, energyLine.points2[i + 1].y);
            }
            energyLine.points2[i] = new Vector2(energyLine.points2[i].x, y);
        }

        private void HeartUpdate(float x)
        {
            int i;
            for (i = 0; i < heartLine.points2.Count - 1; i++)
            {
                heartLine.points2[i] = new Vector2(heartLine.points2[i].x, heartLine.points2[i + 1].y);
            }
            heartLine.points2[i] = new Vector2(heartLine.points2[i].x, x);//Heartline / 30/*Heart/30*/ );
        }
    }
}
