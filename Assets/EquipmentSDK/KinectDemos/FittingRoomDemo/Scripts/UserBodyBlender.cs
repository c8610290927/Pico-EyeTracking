﻿using UnityEngine;
using System.Collections;


public class UserBodyBlender : MonoBehaviour 
{
	[Tooltip("Allowed depth distance between the user and the cloth model.")]
	[Range(0f, 0.5f)]
	public float depthThreshold = 0.1f;

	private Material userBlendMat;
	private KinectManager kinectManager;
	private long lastDepthFrameTime;

	private Vector2[] color2DepthCoords;
	private ComputeBuffer color2DepthBuffer;

	private float[] depthImageBufferData;
	private ComputeBuffer depthImageBuffer;


	void Start () 
	{
		GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;

		kinectManager = KinectManager.Instance;

		if(kinectManager && kinectManager.IsInitialized() &&
			kinectManager.GetSensorData().sensorIntPlatform == KinectInterop.DepthSensorPlatform.KinectSDKv2)
		{
			Shader userBlendShader = Shader.Find("Custom/UserBlendShader");
			KinectInterop.SensorData sensorData = kinectManager.GetSensorData();

			if(userBlendShader != null && sensorData != null)
			{
				userBlendMat = new Material(userBlendShader);
				
				userBlendMat.SetFloat("_ColorResX", (float)sensorData.colorImageWidth);
				userBlendMat.SetFloat("_ColorResY", (float)sensorData.colorImageHeight);
				userBlendMat.SetFloat("_DepthResX", (float)sensorData.depthImageWidth);
				userBlendMat.SetFloat("_DepthResY", (float)sensorData.depthImageHeight);

				color2DepthCoords = new Vector2[sensorData.colorImageWidth * sensorData.colorImageHeight];

				color2DepthBuffer = new ComputeBuffer(sensorData.colorImageWidth * sensorData.colorImageHeight, sizeof(float) * 2);
				userBlendMat.SetBuffer("_DepthCoords", color2DepthBuffer);

				depthImageBufferData = new float[sensorData.depthImage.Length];

				depthImageBuffer = new ComputeBuffer(sensorData.depthImage.Length, sizeof(float));
				userBlendMat.SetBuffer("_DepthBuffer", depthImageBuffer);
			}
		}
	}

	void OnDestroy()
	{
		if(color2DepthBuffer != null)
		{
			color2DepthBuffer.Release();
			color2DepthBuffer = null;
		}
		
		if(depthImageBuffer != null)
		{
			depthImageBuffer.Release();
			depthImageBuffer = null;
		}

		color2DepthCoords = null;
		depthImageBufferData = null;
	}

	void Update()
	{
		if(kinectManager && kinectManager.IsInitialized())
		{
			KinectInterop.SensorData sensorData = kinectManager.GetSensorData();
			
			if(sensorData != null && sensorData.depthImage != null && sensorData.colorImageTexture &&
				userBlendMat != null && lastDepthFrameTime != sensorData.lastDepthFrameTime)
			{
				lastDepthFrameTime = sensorData.lastDepthFrameTime;
				userBlendMat.SetTexture("_ColorTex", sensorData.colorImageTexture);
				
				if(KinectInterop.MapColorFrameToDepthCoords(sensorData, ref color2DepthCoords))
				{
					color2DepthBuffer.SetData(color2DepthCoords);
				}
				
				for (int i = 0; i < sensorData.depthImage.Length; i++)
				{
					int depth = sensorData.depthImage[i];
					depthImageBufferData[i] = (float)depth;
				}
				
				depthImageBuffer.SetData(depthImageBufferData);
			}
		}
	}
	
	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		if(userBlendMat != null)
		{
			userBlendMat.SetFloat("_Threshold", depthThreshold);
			Graphics.Blit(source, destination, userBlendMat);
		}
	}
}
