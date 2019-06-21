using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.UI;

[CustomEditor(typeof(TextPlus), true)]
[CanEditMultipleObjects]
public class TextPlusEditor : GraphicEditor
{
    private static bool m_TextSpacingPanelOpen = false;
    private static bool m_VertexColorPanelOpen = false;
    private static bool m_TextShadowPanelOpen = false;
    private static bool m_TextOutlinePanelOpen = false;
    private static bool m_LocalizationTextPanelOpen = false;

    SerializedProperty m_Text;
    SerializedProperty m_FontData;

    //text spacing
    SerializedProperty m_UseTextSpacing;
    SerializedProperty m_TextSpacing;

    // Ver Color
    SerializedProperty m_UseVertexColor;
    SerializedProperty m_VertexColorFilter;
    SerializedProperty m_VertexColorOffset;
    SerializedProperty m_VertexTopLeft;
    SerializedProperty m_VertexTopRight;
    SerializedProperty m_VertexBottomLeft;
    SerializedProperty m_VertexBottomRight;

    //Shadow
    SerializedProperty m_UseShadow;
    SerializedProperty m_ShadowColorTopLeft;
    SerializedProperty m_ShadowColorTopRight;
    SerializedProperty m_ShadowColorBottomLeft;
    SerializedProperty m_ShadowColorBottomRight;
    SerializedProperty m_ShadowEffectDistance;

    //Outline
    SerializedProperty m_UseOutline;
    SerializedProperty m_OutlineEffectColor;
    SerializedProperty m_OutlineEffectDistance;

    //Localization
    SerializedProperty m_UseLocalization;
    SerializedProperty m_TextStyle;
 


    protected override void OnEnable()
    {
        base.OnEnable();
        m_Text = serializedObject.FindProperty("m_Text");
        m_FontData = serializedObject.FindProperty("m_FontData");

        //text spacing
        m_UseTextSpacing = serializedObject.FindProperty("m_FontSpacingHandler.m_UseTextSpacing");
        m_TextSpacing = serializedObject.FindProperty("m_FontSpacingHandler.m_TextSpacing");

        // VertexColor
        m_UseVertexColor = serializedObject.FindProperty("m_VertexColorHandler.m_UseVertexColor");
        m_VertexColorFilter = serializedObject.FindProperty("m_VertexColorHandler.m_VertexColorFilter");
        m_VertexTopLeft = serializedObject.FindProperty("m_VertexColorHandler.m_VertexTopLeft");
        m_VertexTopRight = serializedObject.FindProperty("m_VertexColorHandler.m_VertexTopRight");
        m_VertexBottomLeft = serializedObject.FindProperty("m_VertexColorHandler.m_VertexBottomLeft");
        m_VertexBottomRight = serializedObject.FindProperty("m_VertexColorHandler.m_VertexBottomRight");
        m_VertexColorOffset = serializedObject.FindProperty("m_VertexColorHandler.m_VertexColorOffset");

        //Shadow
        m_UseShadow = serializedObject.FindProperty("m_TextShadowHandler.m_UseShadow");
        m_ShadowColorTopLeft = serializedObject.FindProperty("m_TextShadowHandler.m_ShadowColorTopLeft");
        m_ShadowColorTopRight = serializedObject.FindProperty("m_TextShadowHandler.m_ShadowColorTopRight");
        m_ShadowColorBottomLeft = serializedObject.FindProperty("m_TextShadowHandler.m_ShadowColorBottomLeft");
        m_ShadowColorBottomRight = serializedObject.FindProperty("m_TextShadowHandler.m_ShadowColorBottomRight");
        m_ShadowEffectDistance = serializedObject.FindProperty("m_TextShadowHandler.m_EffectDistance");

        //Outline
        m_UseOutline = serializedObject.FindProperty("m_TextOutlineHandler.m_UseOutline");
        m_OutlineEffectColor = serializedObject.FindProperty("m_TextOutlineHandler.m_EffectColor");
        m_OutlineEffectDistance = serializedObject.FindProperty("m_TextOutlineHandler.m_EffectDistance");

        //Localization
        m_UseLocalization = serializedObject.FindProperty("m_LocalizationTextHandler.m_UseLocalization");
        m_TextStyle = serializedObject.FindProperty("m_LocalizationTextHandler.m_TextStyleCN");     

        // Panel Open
        m_TextSpacingPanelOpen = EditorPrefs.GetBool("UGUIPlus.m_TextSpacingPanelOpen", m_TextSpacingPanelOpen);
        m_VertexColorPanelOpen = EditorPrefs.GetBool("UGUIPlus.m_VertexColorPanelOpen", m_VertexColorPanelOpen);
        m_TextShadowPanelOpen = EditorPrefs.GetBool("UGUIPlus.m_TextShadowPanelOpen", m_TextShadowPanelOpen);
        m_TextOutlinePanelOpen = EditorPrefs.GetBool("UGUIPlus.m_TextOutlinePanelOpen", m_TextOutlinePanelOpen);
        m_LocalizationTextPanelOpen = EditorPrefs.GetBool("UGUIPlus.m_LocalizationTextPanelOpen", m_LocalizationTextPanelOpen);
    }




    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        EditorGUILayout.PropertyField(m_Text);
        EditorGUILayout.PropertyField(m_FontData);
        AppearanceControlsGUI();
        RaycastControlsGUI();
        PlusGUI();
        serializedObject.ApplyModifiedProperties();
    }

    private void PlusGUI()
    {
        EditorUtil.TextSpacingGUI(m_UseTextSpacing, m_TextSpacing, ref m_TextSpacingPanelOpen);

        EditorUtil.VertexColorGUI(
              m_UseVertexColor,
              m_VertexTopLeft,
              m_VertexTopRight,
              m_VertexBottomLeft,
              m_VertexBottomRight,
              m_VertexColorFilter,
              m_VertexColorOffset,
              ref m_VertexColorPanelOpen
          );

        EditorUtil.TextShadowGUI(
               m_UseShadow,
               m_ShadowColorTopLeft,
               m_ShadowColorTopRight,
               m_ShadowColorBottomLeft,
               m_ShadowColorBottomRight,
               m_ShadowEffectDistance,
               ref m_TextShadowPanelOpen
               );

        EditorUtil.SimpleUseGUI(
               "Outline 描边",
               ref m_TextOutlinePanelOpen,
               0f,
               m_UseOutline,
               m_OutlineEffectColor,
               m_OutlineEffectDistance
               );

        EditorUtil.SimpleUseGUI(
               "文字格式",
               ref m_LocalizationTextPanelOpen,
               0f,
               m_UseLocalization,
               m_TextStyle           
               );

        if (GUI.changed)
        {
            EditorPrefs.SetBool("UGUIPlus.m_TextSpacingPanelOpen", m_TextSpacingPanelOpen);
            EditorPrefs.SetBool("UGUIPlus.m_VertexColorPanelOpen", m_VertexColorPanelOpen);
            EditorPrefs.SetBool("UGUIPlus.m_TextShadowPanelOpen", m_TextShadowPanelOpen);
            EditorPrefs.SetBool("UGUIPlus.m_TextOutlinePanelOpen", m_TextOutlinePanelOpen);
            EditorPrefs.SetBool("UGUIPlus.m_LocalizationTextPanelOpen", m_LocalizationTextPanelOpen);
        }
    }
}
