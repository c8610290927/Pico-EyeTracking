using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlowDataTest : MonoBehaviour
{
    public Text Text;

    private void Start()
    {
        string str = "";
        str += GameDataManager.FlowData.UserId;
        Text.text = str;
    }
}
