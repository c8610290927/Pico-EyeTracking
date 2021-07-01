using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LabData;

public class UIController:MonoSingleton<UIController>
{
    private Transform canvasTransform;
    private Transform CanvasTransform
    {
        get
        {
            if (canvasTransform == null)
            {
                canvasTransform = GameObject.Find("Canvas").transform;
            }
            return canvasTransform;
        }
    }

    private Dictionary<string, BasePanel> panelDict;

    private Stack<BasePanel> panelStack;

    private Dictionary<string, string> panelPathDict = new Dictionary<string, string>();

    private BasePanel _basePanel=new BasePanel();

    public string MainPanle=> "Menu";
    private void Awake()
    {
        PanleDatas panleDatas = LabTools.GetData<PanleDatas>("Panle", "/StreamingAssets/GameData/");
        panelPathDict = panleDatas.PanelValue;
        GetBasePanelData();
    }

    private void Start()
    {
        OpenPanle(MainPanle);
    }

    public void OpenPanle(string name)
    {
        if (_basePanel == null)
        {
            foreach (var item in panelDict)
            {
                if (item.Key == name)
                {
                    _basePanel = item.Value as BasePanel;
                    Debug.Log(_basePanel);
                    _basePanel.OnEnter();
                }
            }
        }
        else
        {
            _basePanel.OnExit();
            foreach (var item in panelDict)
            {
                if (item.Key == name)
                {
                    _basePanel = item.Value;
                    item.Value.OnEnter();
                }
            }
        }
    }


    private void GetBasePanelData()
    {
        panelDict = new Dictionary<string, BasePanel>();
        foreach (var item in panelPathDict)
        {
            
            GameObject instPanel = GameObject.Instantiate(Resources.Load(item.Value)) as GameObject;
            instPanel.transform.SetParent(CanvasTransform, false);
            panelDict.Add(item.Key, instPanel.GetComponent<BasePanel>() as BasePanel);
            instPanel.SetActive(false);
            
        }
    }
}
