using LabData;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EditorFrame
{
    public class CreateUIRootWindow : EditorWindow
    {
        private static UIFormConfig formConfig;
        private static CancellationTokenSource _sayHelloTokenSource;
        private static Dictionary<string, string> Path;

        /// <summary>
        /// 绑定脚本
        /// </summary>
        [MenuItem("UiFrameWork/1.UiSetup  %e")]
        private static void UiPanelAddScript()
        {
            
            UiPanelAddScripts();
            ResetUnity();

            var window = EditorWindow.CreateInstance<CreateUIRootWindow>();
            window.minSize = new Vector2(300, 100);
            window.maxSize = new Vector2(300, 100);
            window.Show();
           
        }

        private void OnGUI()
        {
            GUIStyle bb = new GUIStyle {normal = {background = null, textColor = new Color(1, 0, 0)}, fontSize = 20};
            //这是设置背景填充的
            //设置字体颜色的
            GUI.Label(new Rect(40, 5, 200, 200), "在资源加载完成后点击OK", bb);
            if (GUI.Button(new Rect(110, 35, 100, 30), "O K"))
            {
                Setup();
                Close();
            }
        }

        /// <summary>
        /// 创建脚本
        /// </summary>
        private static void UiPanelAddScripts()
        {
            formConfig = LabTools.GetConfig<UIFormConfig>(false, "/StreamingAssets/GameData/");
            foreach (var item in formConfig.PanleKeys)
            {
                BuildClass(item.Key);
            }
        }
        /// <summary>
        /// 重载资源
        /// </summary>
        private static void ResetUnity()
        {
           AssetDatabase.Refresh();
        }
        /// <summary>
        /// 动态生成脚本
        /// </summary>
        /// <param name="className">脚本名称</param>
        private static void BuildClass(string className)
        {
            //申请一个备用单元
            CodeCompileUnit unit = new CodeCompileUnit();
            //设置一个命名空间
            CodeNamespace codeNamespace = new CodeNamespace("UIFrameWork");

            //导入必要的命名空间
            codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
            codeNamespace.Imports.Add(new CodeNamespaceImport("UnityEngine"));
            codeNamespace.Imports.Add(new CodeNamespaceImport("UnityEngine.UI"));

            //Code:代码体
            CodeTypeDeclaration MyClass = new CodeTypeDeclaration(className + ":BasePanel")
            {
                IsClass = true, TypeAttributes = TypeAttributes.Public
            };
            //为Class
            //设置类型
            //放入命名空间内
            codeNamespace.Types.Add(MyClass);

            unit.Namespaces.Add(codeNamespace);

            //生成C#脚本("VisualBasic"：VB脚本)

            var provider = CodeDomProvider.CreateProvider("CSharp");

            var options = new CodeGeneratorOptions {BracingStyle = "C", BlankLinesBetweenMembers = true};



            var path = Application.dataPath + "/Scripts/" + "UiPanle/";
            WriteFolder(path);

            var outputFile = Application.dataPath + "/Scripts/" + "UiPanle/" + className + ".cs";

            using (var sw = new StreamWriter(outputFile))
            {
                provider.GenerateCodeFromCompileUnit(unit, sw, options);
            }
        }

        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="name"></param>
        private static void WriteFolder(string name)
        {
            var path = name;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private static void Setup()
        {
            formConfig = LabTools.GetConfig<UIFormConfig>(false,"/StreamingAssets/GameData/");
            Debug.Log("生成UI");
                Path = new Dictionary<string, string>();
                #region 生成UIRoot
                var UIRoot = new GameObject("UiRoot");
                UIRoot.AddComponent<UIController>();
                UIRoot.layer = LayerMask.NameToLayer("UI");
                #endregion

                #region 生成Canvas
                var UICanvas = new GameObject("Canvas");
                UICanvas.transform.SetParent(UIRoot.transform);
                UICanvas.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                var canvasScaler = UICanvas.AddComponent<CanvasScaler>();
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.referenceResolution = new Vector2(1920, 1080);
                UICanvas.AddComponent<GraphicRaycaster>();
                UICanvas.layer = LayerMask.NameToLayer("UI");
                #endregion

                #region 生成EventSystem
                var eventSystem = new GameObject("EventSystem");
                eventSystem.transform.SetParent(UIRoot.transform);
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
                eventSystem.layer = LayerMask.NameToLayer("UI");
                #endregion

                #region 生成Panle 根据配置表来，每生成一个都需要做成预设体
                foreach (var item in formConfig.PanleKeys)
                {
                    GameObject PanleObject = Instantiate(Resources.Load("UiFrameWorkPrefabs/Panel")) as GameObject;

                    PanleObject.gameObject.name = item.Key;
                    PanleObject.transform.SetParent(UICanvas.transform, false);
                    PanleObject.gameObject.AddComponent(LabTools.GetScriptType(item.Key));
                    foreach (PanleData.Data Data in item.Value)
                    {
                        GameObject UiObject = Instantiate(Resources.Load("UiFrameWorkPrefabs/" + Data.UIFormName)) as GameObject;
                        UiObject.name = Data.UIObjectName;
                        UiObject.transform.SetParent(PanleObject.transform);
                        UiObject.transform.localPosition = new Vector3(0, 0, 0);
                    }


                    WriteFolder(Application.dataPath + "/Resources/" + "UiPanlePrefabs/");

                    var Folder = Application.dataPath + "/Resources/" + "UiPanlePrefabs/";

                    string FindPrefabsPath = "UiPanlePrefabs/" + item.Key;

                    Path.Add(item.Key, FindPrefabsPath);

                    var FilePath = Folder + item.Key + ".prefab";

                    PrefabUtility.SaveAsPrefabAsset(PanleObject, FilePath);

                    DestroyImmediate(PanleObject, false);
                }
                #endregion

                WriteFolder(Application.dataPath + "/StreamingAssets/GameData/" + "PanleDatas/");
                PanleDatas panleDatas = new PanleDatas();
                panleDatas.PanelValue = Path;
                LabTools.WriteData(panleDatas, "Panle", true, "/StreamingAssets/GameData/");
                WriteFolder(Application.dataPath + "/Resources/" + "UiRoot/");

                var savedFilePath = Application.dataPath + "/Resources/" + "UiRoot/" + "UIRoot.prefab";

                PrefabUtility.SaveAsPrefabAssetAndConnect(UIRoot, savedFilePath, InteractionMode.AutomatedAction);
        }
    }
}
