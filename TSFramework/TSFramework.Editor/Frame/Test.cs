using TSFrame.UI;
using UnityEngine;
using UnityEngine.UI;

public partial class TestPanel : UIPanel
{
    public override string UIPath => "";

    private GameObject _gameobj;
    private Transform _tran;
    private Button raw_uibutton;
    private UIButton uibutton;


    protected override void InitializeElement()
    {
        //初始化组件
        _gameobj = this.transform.Find("").gameObject;
        _tran = this.transform.Find("");
        raw_uibutton = this.transform.Find("").GetComponent<Button>();
        uibutton = new UIButton(raw_uibutton);
    }
}