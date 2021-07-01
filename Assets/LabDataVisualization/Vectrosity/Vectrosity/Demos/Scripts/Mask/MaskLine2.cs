using UnityEngine;
using Vectrosity;
using System.Collections.Generic;

public class MaskLine2 : MonoBehaviour {

    //小数点数
    public int numberOfPoints = 100;
    //画线颜色
	public Color lineColor = Color.yellow;
    //画线面板
	public GameObject mask;
    //线宽
	public float lineWidth = 9.0f;
    //线高
	public float lineHeight = 17.0f;
    //向量线
	private VectorLine spikeLine;

	private float t = 0f;
    //开始的位置
	private Vector3 startPos;
	
	void Start () {
        //这里是画线
		spikeLine = new VectorLine("SpikeLine", new List<Vector3>(numberOfPoints), 2.0f, LineType.Continuous);
		float y = lineHeight / 2;
		for (int i = 0; i < numberOfPoints; i++) {
			spikeLine.points3[i] = new Vector2(Random.Range(-lineWidth/2, lineWidth/2), y);
			y -= lineHeight / numberOfPoints;
		}
		spikeLine.color = lineColor;
		spikeLine.drawTransform = transform;
		spikeLine.SetMask (mask);
		
		startPos = transform.position;
	}
	
	void Update () {
        // 在一个圆中移动这个变换，并且该行使用相同的移动，因为它使用这个变换。
        t = Mathf.Repeat (t + Time.deltaTime, 360.0f);
		transform.position = new Vector2(startPos.x, startPos.y + Mathf.Cos (t) * 4);
		spikeLine.Draw();
	}
}