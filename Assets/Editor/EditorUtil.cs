using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class EditorUtil
{

    [MenuItem("GameObject/UI/UGUI Plus/Text Plus")]
    public static void CreateTextPlus()
    {
        GameObject root = new GameObject("Text", typeof(RectTransform), typeof(TextPlus));
        ResetInCanvasFor((RectTransform)root.transform);
        root.GetComponent<TextPlus>().text = "Text Plus";
        var text = root.GetComponent<TextPlus>();
        text.text = "Text Plus";
        text.color = Color.black;
        text.alignment = TextAnchor.MiddleCenter;
        root.transform.localPosition = Vector3.zero;
    }

    public static void TextSpacingGUI(SerializedProperty m_UseTextSpacing, SerializedProperty m_TextSpacing, ref bool m_TextSpacingPanelOpen)
    {
        LayoutF(() =>
        {            
            EditorGUILayout.PropertyField(m_UseTextSpacing);
            if (m_UseTextSpacing.boolValue)
            {
                Space();
                LayoutH(() => {
                    EditorGUI.PropertyField(GUIRect(0, 18), m_TextSpacing, new GUIContent());
                });
            }
        }, "Text Spacing", ref m_TextSpacingPanelOpen, true);
    }

    public static void VertexColorGUI(SerializedProperty m_UseVertexColor, SerializedProperty m_VertexTopLeft, SerializedProperty m_VertexTopRight, SerializedProperty m_VertexBottomLeft, SerializedProperty m_VertexBottomRight, SerializedProperty m_VertexColorFilter, SerializedProperty m_VertexColorOffset, ref bool m_VertexColorPanelOpen)
    {
        LayoutF(() => {
            EditorGUILayout.PropertyField(m_UseVertexColor);
            if (m_UseVertexColor.boolValue)
            {
                Space();
                LayoutH(() => {
                    EditorGUI.PropertyField(GUIRect(0, 18), m_VertexTopLeft, new GUIContent());
                    Space();
                    EditorGUI.PropertyField(GUIRect(0, 18), m_VertexTopRight, new GUIContent());
                });
                Space();
                LayoutH(() => {
                    EditorGUI.PropertyField(GUIRect(0, 18), m_VertexBottomLeft, new GUIContent());
                    Space();
                    EditorGUI.PropertyField(GUIRect(0, 18), m_VertexBottomRight, new GUIContent());
                });
                Space();
                m_VertexColorFilter.enumValueIndex = (int)(VertexColorHandler.ColorFilterType)EditorGUILayout.EnumPopup(
                    new GUIContent("Filter"), (VertexColorHandler.ColorFilterType)m_VertexColorFilter.enumValueIndex
                );
                Vector2 newOffset = EditorGUILayout.Vector2Field("Offset", m_VertexColorOffset.vector2Value);
                newOffset.x = Mathf.Clamp(newOffset.x, -1f, 1f);
                newOffset.y = Mathf.Clamp(newOffset.y, -1f, 1f);
                m_VertexColorOffset.vector2Value = newOffset;
                Space();
            }
        }, "Vertex Color", ref m_VertexColorPanelOpen, true);
    }

    public static void TextShadowGUI(SerializedProperty m_UseShadow, SerializedProperty m_ShadowColorTopLeft, SerializedProperty m_ShadowColorTopRight,
           SerializedProperty m_ShadowColorBottomLeft, SerializedProperty m_ShadowColorBottomRight, SerializedProperty m_ShadowEffectDistance, ref bool m_TextShadowPanelOpen)
    {
        LayoutF(() =>
        {
            EditorGUILayout.PropertyField(m_UseShadow);
            if (m_UseShadow.boolValue)
            {
                Space();
                LayoutH(() => {
                    EditorGUI.PropertyField(GUIRect(0, 18), m_ShadowColorTopLeft, new GUIContent());
                    Space();
                    EditorGUI.PropertyField(GUIRect(0, 18), m_ShadowColorTopRight, new GUIContent());
                });
                Space();
                LayoutH(() => {
                    EditorGUI.PropertyField(GUIRect(0, 18), m_ShadowColorBottomLeft, new GUIContent());
                    Space();
                    EditorGUI.PropertyField(GUIRect(0, 18), m_ShadowColorBottomRight, new GUIContent());
                });
                Space();
                EditorGUILayout.PropertyField(m_ShadowEffectDistance);
            }
        }, "Shadow", ref m_TextShadowPanelOpen, true);
    }


    public static void SimpleUseGUI(string title, ref bool m_PanelOpen, float space, SerializedProperty useThis, params SerializedProperty[] sps)
    {
        LayoutF(() => {
            EditorGUILayout.PropertyField(useThis);
            if (useThis.boolValue)
            {
                foreach (var s in sps)
                {
                    if (s != null)
                    {
                        EditorGUILayout.PropertyField(s);
                    }
                }
            }
        }, title, ref m_PanelOpen, true);
    }

    private static void ResetInCanvasFor(RectTransform root)
    {
        root.SetParent(Selection.activeTransform);
        if (!InCanvas(root))
        {
            Transform canvasTF = GetCreateCanvas();
            root.SetParent(canvasTF);
        }
        if (!Transform.FindObjectOfType<UnityEngine.EventSystems.EventSystem>())
        {
            GameObject eg = new GameObject("EventSystem");
            eg.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eg.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
        root.localScale = Vector3.one;
        root.localPosition = new Vector3(root.localPosition.x, root.localPosition.y, 0f);
        Selection.activeGameObject = root.gameObject;
    }


    private static bool InCanvas(Transform tf)
    {
        while (tf.parent)
        {
            tf = tf.parent;
            if (tf.GetComponent<Canvas>())
            {
                return true;
            }
        }
        return false;
    }

    private static Transform GetCreateCanvas()
    {
        Canvas c = Object.FindObjectOfType<Canvas>();
        if (c)
        {
            return c.transform;
        }
        else
        {
            GameObject g = new GameObject("Canvas");
            c = g.AddComponent<Canvas>();
            c.renderMode = RenderMode.ScreenSpaceOverlay;
            g.AddComponent<CanvasScaler>();
            g.AddComponent<GraphicRaycaster>();
            return g.transform;
        }
    }

    private static void LayoutF(System.Action action, string label, ref bool open, bool box = false)
    {
        bool _open = open;
        LayoutV(() => {
            _open = GUILayout.Toggle(
                _open,
                label,
                GUI.skin.GetStyle("foldout"),
                GUILayout.ExpandWidth(true),
                GUILayout.Height(18)
            );
            if (_open)
            {
                action();
            }
        }, box);
        open = _open;
    }

    private static Rect GUIRect(float width, float height)
    {
        return GUILayoutUtility.GetRect(width, height, GUILayout.ExpandWidth(width <= 0), GUILayout.ExpandHeight(height <= 0));
    }


    private static void Space(float space = 4f)
    {
        GUILayout.Space(space);
    }

    private static void LayoutH(System.Action action, bool box = false)
    {
        if (box)
        {
            GUIStyle style = new GUIStyle(GUI.skin.box);
            GUILayout.BeginHorizontal(style);
        }
        else
        {
            GUILayout.BeginHorizontal();
        }
        action();
        GUILayout.EndHorizontal();
    }


    private static void LayoutV(System.Action action, bool box = false)
    {
        if (box)
        {
            GUIStyle style = new GUIStyle(GUI.skin.box)
            {
                padding = new RectOffset(6, 6, 2, 2)
            };
            GUILayout.BeginVertical(style);
        }
        else
        {
            GUILayout.BeginVertical();
        }
        action();
        GUILayout.EndVertical();
    }

}
