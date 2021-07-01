using UnityEngine;
using Vectrosity;
using System.Collections.Generic;

public class MaskLine2 : MonoBehaviour {

    //С������
    public int numberOfPoints = 100;
    //������ɫ
	public Color lineColor = Color.yellow;
    //�������
	public GameObject mask;
    //�߿�
	public float lineWidth = 9.0f;
    //�߸�
	public float lineHeight = 17.0f;
    //������
	private VectorLine spikeLine;

	private float t = 0f;
    //��ʼ��λ��
	private Vector3 startPos;
	
	void Start () {
        //�����ǻ���
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
        // ��һ��Բ���ƶ�����任�����Ҹ���ʹ����ͬ���ƶ�����Ϊ��ʹ������任��
        t = Mathf.Repeat (t + Time.deltaTime, 360.0f);
		transform.position = new Vector2(startPos.x, startPos.y + Mathf.Cos (t) * 4);
		spikeLine.Draw();
	}
}