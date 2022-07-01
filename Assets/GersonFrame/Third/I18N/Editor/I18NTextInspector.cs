using System.Collections;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;

namespace Localization
{
    [CustomEditor(typeof(I18NText))]
    public class I18NTextInspector : Editor
    {
        I18NText i18NText;

        private SerializedProperty lanKeyProp;
        private ReorderableList valueListProp;
        void OnEnable()
        {
            i18NText = (I18NText)target;

            lanKeyProp = serializedObject.FindProperty("lanKey");
            valueListProp = new ReorderableList(serializedObject, serializedObject.FindProperty("valueList"), true, true, false, false);

            valueListProp.drawHeaderCallback = (Rect rect) =>
            {
                GUI.Label(rect, "列表");
            };

            valueListProp.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                SerializedProperty item = valueListProp.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, item, new GUIContent(LocalizationManager.Instance.languages[index].ToString()));
            };
            valueListProp.onSelectCallback = (ReorderableList list) => {
                var str = list.serializedProperty.GetArrayElementAtIndex(list.index).stringValue;
                i18NText.SetText(str);
            };
        }
        public override void OnInspectorGUI()
        {
            string beforeKey = lanKeyProp.stringValue;

            serializedObject.Update();
            EditorGUILayout.PropertyField(lanKeyProp, true);
            valueListProp.DoLayoutList();
            serializedObject.ApplyModifiedProperties();

            if (!beforeKey.Equals(lanKeyProp.stringValue))
            {
                i18NText.valueList = LocalizationManager.Instance.GetStringListFromKey(lanKeyProp.stringValue);
            }
        }
    }
}
