using UnityEngine;
using Windows.Kinect;

using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System;
using System.IO;

public enum BodySlice
{
	HEIGHT = 0,

	TORSO_1 = 1,
	TORSO_2 = 2,
	TORSO_3 = 3,
	TORSO_4 = 4,

	COUNT = 5
}

public struct BodySliceData
{
	public bool isSliceValid;
	public float diameter;
	public int depthsLength;
//	public ushort[] depths;
	public Vector2 startDepthPoint;
	public Vector2 endDepthPoint;
	public Vector3 startKinectPoint;
	public Vector3 endKinectPoint;
}


public class BodySlicer : MonoBehaviour
{
	[Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
	public int playerIndex = 0;

	[Tooltip("Whether the slicing should be done on all updates, or only after the user calibration.")]
	public bool continuousSlicing = false;

	[Tooltip("Whether the detected body slices should be displayed on the screen.")]
	public bool displayBodySlices = false;

//	// background image texture, if any
//	public GUITexture bgImage;

	//Is Kinect initialized and user calibrated?
	private bool kinectConnected = false;

	private long calibratedUserId;
	private byte userBodyIndex;


	// The singleton instance of KinectController
	private static BodySlicer instance = null;
	private KinectManager manager;
	private KinectInterop.SensorData sensorData;
	private long lastDepthFrameTime;

	private BodySliceData[] bodySlices = new BodySliceData[(int)BodySlice.COUNT];
	private Texture2D depthImage;

	
	/// <summary>
	/// Gets the singleton BodySlicer instance.
	/// </summary>
	/// <value>The singleton BodySlicer instance.</value>
	public static BodySlicer Instance
	{
		get
		{
			return instance;
		}
	}


	/// <summary>
	/// Gets the height of the user.
	/// </summary>
	/// <returns>The user height.</returns>
	public float getUserHeight()
	{
		return getSliceWidth (BodySlice.HEIGHT);
	}


	/// <summary>
	/// Gets the slice width.
	/// </summary>
	/// <returns>The slice width.</returns>
	/// <param name="slice">Slice.</param>
	public float getSliceWidth(BodySlice slice)
	{
		int iSlice = (int)slice;

		if (bodySlices[iSlice].isSliceValid) 
		{
			return bodySlices[iSlice].diameter;
		}

		return 0f;
	}


	/// <summary>
	/// Gets the body slice count.
	/// </summary>
	/// <returns>The body slice count.</returns>
	public int getBodySliceCount()
	{
		return bodySlices != null ? bodySlices.Length : 0;
	}


	/// <summary>
	/// Gets the body slice data.
	/// </summary>
	/// <returns>The body slice data.</returns>
	/// <param name="slice">Slice.</param>
	public BodySliceData getBodySliceData(BodySlice slice)
	{
		return bodySlices[(int)slice];
	}


	/// <summary>
	/// Gets the calibrated user ID.
	/// </summary>
	/// <returns>The calibrated user ID.</returns>
	public int getCalibratedUserId() 
	{
		return (int)calibratedUserId;				
	}


	/// <summary>
	/// Gets the last frame time.
	/// </summary>
	/// <returns>The last frame time.</returns>
	public long getLastFrameTime()
	{
		return lastDepthFrameTime;
	}


	////////////////////////////////////////////////////////////////////////


	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		manager = KinectManager.Instance;
		kinectConnected = manager ? manager.IsInitialized() : false;
		sensorData = manager.GetSensorData();
	}

	void Update()
	{
		if(!manager || !kinectConnected)
			return;

		// get required player
		long userId = manager.GetUserIdByIndex (playerIndex);

		if(calibratedUserId == 0)
		{
			if(userId != 0)
			{
				OnCalibrationSuccess(userId);
			}
		}
		else
		{
			if (calibratedUserId != userId) 
			{
				OnUserLost(calibratedUserId);
			} 
			else if(continuousSlicing)
			{
				EstimateBodySlices(calibratedUserId);
			}
		}

//		// update color image
//		if (bgImage && !bgImage.texture) 
//		{
//			bgImage.texture = manager.GetUsersClrTex();
//		}
	}

	void OnGUI() 
	{
		if(displayBodySlices && depthImage)
		{
			Rect depthImageRect = new Rect(0, Screen.height, 256, -212);
			GUI.DrawTexture(depthImageRect, depthImage);
		}
	}

    void OnCalibrationSuccess(long userId)
    {
		calibratedUserId = userId;

		// estimate body slices
		EstimateBodySlices(calibratedUserId);
    }

    void OnUserLost(long UserId)
    {
		calibratedUserId = 0;
    }
	
	public bool EstimateBodySlices(long userId)
	{
		if (userId <= 0) 
			userId = calibratedUserId;

		if(!manager || userId == 0)
			return false;

		userBodyIndex = (byte)manager.GetBodyIndexByUserId(userId);
		if (userBodyIndex == 255)
			return false;

		bool bSliceSuccess = false;

		if (sensorData.bodyIndexImage != null && sensorData.depthImage != null &&
		    sensorData.lastDepthFrameTime != lastDepthFrameTime) 
		{
			lastDepthFrameTime = sensorData.lastDepthFrameTime;

			bSliceSuccess = true;

			Vector2 pointSpineBase = manager.MapSpacePointToDepthCoords(manager.GetJointKinectPosition(userId, (int)JointType.SpineBase));
			bodySlices[(int)BodySlice.HEIGHT] = GetUserHeightParams(pointSpineBase);

			if(manager.IsJointTracked(userId, (int)JointType.SpineBase) && manager.IsJointTracked(userId, (int)JointType.Neck))
			{
				Vector2 point1 = pointSpineBase;
				Vector2 point2 = manager.MapSpacePointToDepthCoords(manager.GetJointKinectPosition(userId, (int)JointType.Neck));
				Vector2 sliceDir = (point2 - point1) / 4f;

				Vector2 vSlicePoint = point1;
				bodySlices[(int)BodySlice.TORSO_1] = GetBodySliceParams(vSlicePoint, true, false, -1);

				vSlicePoint += sliceDir;
				bodySlices[(int)BodySlice.TORSO_2] = GetBodySliceParams(vSlicePoint, true, false, -1);

				vSlicePoint += sliceDir;
				bodySlices[(int)BodySlice.TORSO_3] = GetBodySliceParams(vSlicePoint, true, false, -1);

				vSlicePoint += sliceDir;
				bodySlices[(int)BodySlice.TORSO_4] = GetBodySliceParams(vSlicePoint, true, false, -1);
			}

			// display body slices
			if(displayBodySlices)
			{
				depthImage = manager.GetUsersLblTex();

				if(depthImage)
				{
					depthImage = GameObject.Instantiate(depthImage) as Texture2D;

					DrawBodySlice(bodySlices[(int)BodySlice.HEIGHT]);

					DrawBodySlice(bodySlices[(int)BodySlice.TORSO_1]);
					DrawBodySlice(bodySlices[(int)BodySlice.TORSO_2]);
					DrawBodySlice(bodySlices[(int)BodySlice.TORSO_3]);
					DrawBodySlice(bodySlices[(int)BodySlice.TORSO_4]);

					depthImage.Apply();
				}
			}
		}

		return bSliceSuccess;
	}


	private void DrawBodySlice(BodySliceData bodySlice)
	{
		if(depthImage && bodySlice.isSliceValid && 
		   bodySlice.startDepthPoint != Vector2.zero && bodySlice.endDepthPoint != Vector2.zero)
		{
			KinectInterop.DrawLine(depthImage, (int)bodySlice.startDepthPoint.x, (int)bodySlice.startDepthPoint.y, 
			         (int)bodySlice.endDepthPoint.x, (int)bodySlice.endDepthPoint.y, Color.red);
		}
	}

	private BodySliceData GetUserHeightParams(Vector2 pointSpineBase)
	{
		int depthLength = sensorData.depthImage.Length;
		int depthWidth = sensorData.depthImageWidth;
		int depthHeight = sensorData.depthImageHeight;

		Vector2 posTop = new Vector2 (0, depthHeight);
		for (int i = 0, x = 0, y = 0; i < depthLength; i++) 
		{
			if (sensorData.bodyIndexImage [i] == userBodyIndex) 
			{
				//if (posTop.y > y)
					posTop = new Vector2(x, y);
				break;
			}

			x++;
			if (x >= depthWidth) 
			{
				x = 0;
				y++;
			}
		}

		Vector2 posBottom = new Vector2 (0, -1);
		for (int i = depthLength - 1, x = depthWidth - 1, y = depthHeight - 1; i >= 0; i--) 
		{
			if (sensorData.bodyIndexImage [i] == userBodyIndex) 
			{
				//if (posBottom.y < y)
					posBottom = new Vector2(x, y);
				break;
			}

			x--;
			if (x < 0) 
			{
				x = depthWidth - 1;
				y--;
			}
		}

		BodySliceData sliceData = new BodySliceData();
		sliceData.isSliceValid = false;

		if (posBottom.y >= 0) 
		{
			sliceData.startDepthPoint = posTop;
			sliceData.endDepthPoint = posBottom;
			sliceData.depthsLength = (int)posBottom.y - (int)posTop.y + 1;

			int index1 = (int)posTop.y * depthWidth + (int)posTop.x;
			ushort depth1 = sensorData.depthImage[index1];
			sliceData.startKinectPoint = manager.MapDepthPointToSpaceCoords(sliceData.startDepthPoint, depth1, true);

			int index2 = (int)posBottom.y * depthWidth + (int)posBottom.x;
			ushort depth2 = sensorData.depthImage[index2];
			sliceData.endKinectPoint = manager.MapDepthPointToSpaceCoords(sliceData.endDepthPoint, depth2, true);

			// correct x-positions of depth points
			sliceData.startDepthPoint.x = pointSpineBase.x;
			sliceData.endDepthPoint.x = pointSpineBase.x;

			sliceData.diameter = (sliceData.endKinectPoint - sliceData.startKinectPoint).magnitude;
			sliceData.isSliceValid = true;
		} 

		return sliceData;
	}

	private BodySliceData GetBodySliceParams(Vector2 middlePoint, bool bSliceOnX, bool bSliceOnY, int maxDepthLength)
	{
		BodySliceData sliceData = new BodySliceData();
		sliceData.isSliceValid = false;
		sliceData.depthsLength  = 0;

		if(!manager || middlePoint == Vector2.zero)
			return sliceData;
		if(!bSliceOnX && !bSliceOnY)
			return sliceData;

		middlePoint.x = Mathf.FloorToInt(middlePoint.x + 0.5f);
		middlePoint.y = Mathf.FloorToInt(middlePoint.y + 0.5f);

		int depthWidth = sensorData.depthImageWidth;
		int depthHeight = sensorData.depthImageHeight;

		int indexMid = (int)middlePoint.y * depthWidth + (int)middlePoint.x;
		byte userIndex = sensorData.bodyIndexImage[indexMid];

		if(userIndex != userBodyIndex)
			return sliceData;

		sliceData.startDepthPoint = middlePoint;
		sliceData.endDepthPoint = middlePoint;

		int indexDiff1 = 0;
		int indexDiff2 = 0;

		if(bSliceOnX)
		{
			// min-max
			int minIndex = (int)middlePoint.y * depthWidth;
			int maxIndex = (int)(middlePoint.y + 1) * depthWidth;

			// horizontal left
			int stepIndex = -1;
			indexDiff1 = TrackSliceInDirection(indexMid, stepIndex, minIndex, maxIndex, userIndex);

			// horizontal right
			stepIndex = 1;
			indexDiff2 = TrackSliceInDirection(indexMid, stepIndex, minIndex, maxIndex, userIndex);
		}
		else if(bSliceOnY)
		{
			// min-max
			int minIndex = 0;
			int maxIndex = depthHeight * depthWidth;

			// vertical up
			int stepIndex = -depthWidth;
			indexDiff1 = TrackSliceInDirection(indexMid, stepIndex, minIndex, maxIndex, userIndex);

			// vertical down
			stepIndex = depthWidth;
			indexDiff2 = TrackSliceInDirection(indexMid, stepIndex, minIndex, maxIndex, userIndex);
		}

		// calculate depth length
		sliceData.depthsLength = indexDiff1 + indexDiff2 + 1;

		// check for max length (used by upper legs)
		if(maxDepthLength > 0 && sliceData.depthsLength > maxDepthLength)
		{
//			indexDiff1 = (int)((float)indexDiff1 * maxDepthLength / sliceData.depthsLength);
//			indexDiff2 = (int)((float)indexDiff2 * maxDepthLength / sliceData.depthsLength);

			if(indexDiff1 > indexDiff2)
				indexDiff1 = indexDiff2;
			else
				indexDiff2 = indexDiff1;

			sliceData.depthsLength = indexDiff1 + indexDiff2 + 1;
		}

		// set start and end depth points
		if(bSliceOnX)
		{
			sliceData.startDepthPoint.x -= indexDiff1;
			sliceData.endDepthPoint.x += indexDiff2;
		}
		else if(bSliceOnY)
		{
			sliceData.startDepthPoint.y -= indexDiff1;
			sliceData.endDepthPoint.y += indexDiff2;
		}

		// start point
		int index1 = (int)sliceData.startDepthPoint.y * depthWidth + (int)sliceData.startDepthPoint.x;
		ushort depth1 = sensorData.depthImage[index1];
		sliceData.startKinectPoint = manager.MapDepthPointToSpaceCoords(sliceData.startDepthPoint, depth1, true);

		// end point
		int index2 = (int)sliceData.endDepthPoint.y * depthWidth + (int)sliceData.endDepthPoint.x;
		ushort depth2 = sensorData.depthImage[index2];
		sliceData.endKinectPoint = manager.MapDepthPointToSpaceCoords(sliceData.endDepthPoint, depth2, true);

		// diameter
		sliceData.diameter = (sliceData.endKinectPoint - sliceData.startKinectPoint).magnitude;
		sliceData.isSliceValid = true;

//		// get depths
//		sliceData.depths = new ushort[sliceData.depthsLength];
//		int stepDepthIndex = 1;
//
//		if(bSliceOnX)
//		{
//			stepDepthIndex = 1;
//		}
//		else if(bSliceOnY)
//		{
//			stepDepthIndex = depthWidth;
//		}
//		
//		for(int i = index1, d = 0; i <= index2; i+= stepDepthIndex, d++)
//		{
//			sliceData.depths[d] = sensorData.depthImage[i];
//		}

		return sliceData;
	}

	private int TrackSliceInDirection(int index, int stepIndex, int minIndex, int maxIndex, byte userIndex)
	{
		int indexDiff = 0;
		int errCount = 0;

		index += stepIndex;
		while(index >= minIndex && index < maxIndex)
		{
			if(sensorData.bodyIndexImage[index] != userIndex)
			{
				errCount++;
				if(errCount > 0) // allow 0 error(s)
					break;
			}
			else
			{
				errCount = 0;
			}
			
			index += stepIndex;
			indexDiff++;
		}

		return indexDiff;
	}

}

