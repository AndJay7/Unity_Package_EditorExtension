using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace And.Editor
{
    public class SerializedGridProperty
    {
        private SerializedProperty property;
        public float LabelWidth { get; private set; }
        public string DisplayName => property.displayName;

        public SerializedGridProperty(SerializedProperty property, float labelWidth)
        {
            this.property = property;
            this.LabelWidth = labelWidth;
        }

        public void SetExpand(bool isExpanded)
        {
            property.isExpanded = isExpanded;
        }
        
        public void DrawProperty(Rect fieldRect)
        {
            EditorGUI.PropertyField(fieldRect, property, GUIContent.none, true);
        }

        public void DrawEnum<T>(Rect fieldRect, List<T> disabledValues = null)
            where T : Enum
        {
            var values = Enum.GetValues(typeof(T));
            T val = (T)values.GetValue(property.enumValueIndex);
            Func<Enum, bool> disableFunc = null;
            if (disabledValues != null)
                disableFunc = (e) => !disabledValues.Contains((T)e);
            val = (T)EditorGUI.EnumPopup(fieldRect, GUIContent.none, val, checkEnabled: disableFunc);
            property.enumValueIndex = (int)Convert.ChangeType(val, typeof(int));
        }
    }
}