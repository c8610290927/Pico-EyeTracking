
using UnityEngine;
using System.Collections;

public class moveBehaviour : MonoBehaviour
{



    public ILeapInterface LeapInterface;
    public bool IsFindHand;

    private void Start()
    {
        LeapInterface = new LeapInput();
    }
    void Update()
    {
        transform.rotation = LeapInterface.GetQuaternion();

    }


    public void ForEachHand()
    {
        foreach (var finger in LeapInterface.GetFrame().Fingers)
        {
            if (finger.Type == Leap.Finger.FingerType.TYPE_INDEX)
            {
                Debug.Log(finger.bones[3].NextJoint.x/1000);
            }
           
        }
    }
}
