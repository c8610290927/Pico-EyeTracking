using System.Reflection;
using LabData;
using UnityEngine;
using UnityEngine.UI;

public class Test_FileTest : MonoBehaviour
{
    public Button SaveBtn, EditorBtn,GoBtn;

    public Dropdown FileDropdown;

    public InputField FilenameField,TestFloatField,TestIntField,TestStringField;

    public Text FileInfoText;


    // Start is called before the first frame update
    void Start()
    {
       
        //必须先创建对应数据的文件夹
        LabTools.CreateDataFolder<Test_FileTestData>();
       
        #region 初始化界面信息

        if (LabTools.GetDataName<Test_FileTestData>() != null)
        {
            FileDropdown.AddOptions(LabTools.GetDataName<Test_FileTestData>());
        }
        //通过DropDown后去当前选择的数据，如果存在
        var testData = LabTools.GetData<Test_FileTestData>(FileDropdown.captionText.text);
        //判断数据是否存在，如果存在就打印在Text上
        if (testData != null)
        {
            FileInfoText.text = string.Join("\n", FileDropdown.captionText.text, testData.TestFloat, testData.TestInt,
                testData.TestString);
        }

        #endregion


        FileDropdown.onValueChanged.AddListener((a) =>
        {
            testData = LabTools.GetData<Test_FileTestData>(FileDropdown.captionText.text);
            FileInfoText.text = string.Join("\n", FileDropdown.captionText.text, testData.TestFloat, testData.TestInt,
                testData.TestString);
        });

        SaveBtn.onClick.AddListener(() =>
        {
            testData = new Test_FileTestData(float.Parse(TestFloatField.text),int.Parse(TestIntField.text),TestStringField.text);
            LabTools.WriteData(testData, FilenameField.text);
            FileDropdown.ClearOptions();
            FileDropdown.AddOptions(LabTools.GetDataName<Test_FileTestData>());
        });
        EditorBtn.onClick.AddListener(() =>
        {
            testData = new Test_FileTestData(float.Parse(TestFloatField.text), int.Parse(TestIntField.text), TestStringField.text);
            //修改数据，所以传入true
            LabTools.WriteData(testData, FileDropdown.captionText.text, true);
            FileInfoText.text = "";
            FileInfoText.text = string.Join("\n", FileDropdown.captionText.text, testData.TestFloat, testData.TestInt,
                testData.TestString);

        });

        GoBtn.onClick.AddListener(() =>
            {
               // GameApplication.GameDataManager.FlowData.TestData = LabTools.GetData<Test_FileTestData>(FileDropdown.captionText.text);
            });
    }

    

    
}
