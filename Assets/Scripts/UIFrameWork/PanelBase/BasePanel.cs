using UnityEngine;

public enum LanguageType
{

}

public class BasePanel : MonoBehaviour
{
    /// <summary>
    /// 显示界面,相当于Start
    /// </summary>
    public virtual void OnEnter()
    {
        
        this.gameObject.SetActive(true);
    }
    /// <summary>
    /// 关闭界面
    /// </summary>
    public virtual void OnExit()
    {
        this.gameObject.SetActive(false);
    }
    /// <summary>
    /// 多语言
    /// </summary>
    /// <param name="language"></param>
    public virtual void ChangeLanguages(Language language)
    {

    }
}
