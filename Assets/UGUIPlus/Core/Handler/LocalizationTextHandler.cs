using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class LocalizationTextHandler
{

    [SerializeField]
    private bool m_UseLocalization;
    public bool UseLocalization
    {
        get
        {
            return m_UseLocalization;
        }
        set
        {
            m_UseLocalization = value;
        }
    }

    //中文显示用
    public enum TextStyleCN
    {
        默认,
        系统标题,
        二级弹框,
        小标题,
    }

    //实际代码用
    public enum TextStyle
    {
        NULL,   //默认
        SystemTitle,//系统标题
        SecondTipTitle, //二级弹框
        SmallTipTitle,  //小标题  
    }

    public class TextStyleFont
    {
        public Font font;
        public int fontSize;
    }


    [SerializeField]
    private TextStyleCN m_TextStyleCN = TextStyleCN.默认;

    [SerializeField]
    private TextStyle m_TextStyle = TextStyle.NULL;

    private Text txt_Target;

    public TextStyle TextStyle_
    {
        get
        {
            return m_TextStyle;
        }

        set
        {
            m_TextStyle = value;
            m_TextStyleCN = (TextStyleCN)((int)m_TextStyle);
        }
    }

    public void UpdateText(Text target)
    {
        txt_Target = target;
        if (m_UseLocalization == false)
            return;
        m_TextStyle = (TextStyle)(int)m_TextStyleCN;

        //通过类型选择字体和颜色
        var color_ = Color.black;
        TextStyleFont fontStyle = new TextStyleFont()
        {
            font = Resources.Load<Font>(""),
            fontSize = 20
        };

        switch (m_TextStyle)
        {
            case TextStyle.SystemTitle:
                color_ = Color.blue;
                txt_Target.font = Resources.Load<Font>("Font/FZZDHJW");
                txt_Target.fontSize = 35;
                break;
            case TextStyle.SecondTipTitle:
                color_ = Color.yellow;
                txt_Target.font = Resources.Load<Font>("Font/FZZDHJW");
                txt_Target.fontSize = 25;
                break;
            case TextStyle.SmallTipTitle:
                color_ = Color.yellow;
                txt_Target.font = Resources.Load<Font>("Font/MSYHTTF");
                txt_Target.fontSize = 25;
                break;
        }
                
        txt_Target.color = color_;
    }
}
