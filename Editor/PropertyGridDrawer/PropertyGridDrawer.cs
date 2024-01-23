using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace And.Editor
{
    public abstract class PropertyGridDrawer : PropertyDrawer
    {
        protected bool IsWrapping => INDENT_ACTIVATION_WIDTH >= Screen.width;

        //passing height is necessary, because List Drawer is changing rect after GetPropertyHeight()
        private const float COLUMN_SPACING = 4f;
        protected const float ROW_SPACING = 2f;
        private const float MIN_NAME_WIDTH = 120f;
        private const float INDENT_ACTIVATION_WIDTH = 331f;
        private const float INDENT_OFFSET_X = 16f;

        public override sealed void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
        {
            int indentCount = EditorGUI.indentLevel;

            Vector2Int gridCount = new Vector2Int(GetColumnCount(property, label), GetRowCount(property, label));
            float targetHeight = GetPropertyHeight(property, label);
            float prefixIndent = GetPrefixWidth(pos);
            float indentValue = INDENT_OFFSET_X;

            if (IsFullyNested(property, label))
            {
                prefixIndent = indentValue = 0;
            }

            PropertyGrid grid = GetGrid(pos, targetHeight, gridCount, prefixIndent, indentValue, indentCount);

            label = EditorGUI.BeginProperty(pos, label, property);

            EditorGUI.PrefixLabel(pos, label);
            EditorGUI.indentLevel = 0;
            DrawGUI(pos, property, label, grid);

            EditorGUI.EndProperty();
            EditorGUI.indentLevel = indentCount;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float rowHeight = EditorGUIUtility.singleLineHeight;
            int rowCount = GetRowCount(property, label);
            float height = PropertyGrid.GetSpanSize(rowHeight, ROW_SPACING, rowCount);
            return height;
        }

        protected abstract int GetRowCount(SerializedProperty property, GUIContent label);
        protected abstract int GetColumnCount(SerializedProperty property, GUIContent label);
        protected abstract void DrawGUI(Rect pos, SerializedProperty property, GUIContent label, PropertyGrid grid);

        protected bool IsFullyNested(SerializedProperty property, GUIContent label)
        {
            bool isNested = property.depth > 0;
            bool noLabel = string.IsNullOrEmpty(label.text);
            return isNested && noLabel;
        }

        private PropertyGrid GetGrid(Rect pos, float height, Vector2Int gridCount, float prefixIndent, float indentValue, int indentCount)
        {
            float marginY = (pos.height - height) / 2;
            float posX = pos.x - indentValue * indentCount;
            float width = pos.width;// + indentValue * indentCount;

            PropertyGrid grid = new PropertyGrid()
            {
                pivot = new Vector2(posX, pos.y),
                size = new Vector2(width, height),
                margin = new Vector2(0, marginY),
                spacing = new Vector2(COLUMN_SPACING, ROW_SPACING),
                count = gridCount,
                prefixIndent = prefixIndent,
                indentValue = indentValue,
                indentLevel = indentCount
            };

            return grid;
        }

        private float GetPrefixWidth(Rect pos)
        {
            return Mathf.Max(MIN_NAME_WIDTH, pos.width * 0.45f - 28);
        }
    }

    public class SerializedGridProperty
    {
        private SerializedProperty property;
        private float labelWidth;

        public SerializedGridProperty(SerializedProperty property, float labelWidth)
        {
            this.property = property;
            this.labelWidth = labelWidth;
        }

        public void SetExpand(bool isExpanded)
        {
            property.isExpanded = isExpanded;
        }

        public void DrawFloat(PropertyGrid grid, GridPosition gridPos, string label = null, bool showLabel = true)
        {
            Action<Rect> action = (fieldRect) =>
                property.floatValue = EditorGUI.FloatField(fieldRect, property.floatValue);

            DrawFieldWithLabel(action, grid, gridPos, label, showLabel);
        }

        public void DrawCurve(PropertyGrid grid, GridPosition gridPos, string label = null, bool showLabel = true)
        {
            Action<Rect> action = (fieldRect) =>
                property.animationCurveValue = EditorGUI.CurveField(fieldRect, property.animationCurveValue);

            DrawFieldWithLabel(action, grid, gridPos, label, showLabel);
        }

        public void DrawProperty(PropertyGrid grid, GridPosition gridPos, string label = null, bool showLabel = true)
        {
            Action<Rect> action = (fieldRect) => EditorGUI.PropertyField(fieldRect, property, GUIContent.none, true);

            DrawFieldWithLabel(action, grid, gridPos, label, showLabel);
        }

        public void DrawEnum<T>(PropertyGrid grid, GridPosition gridPos, List<T> disabledValues = null, string label = null, bool showLabel = true)
        where T : Enum
        {
            Action<Rect> action = (fieldRect) =>
            {
                var values = Enum.GetValues(typeof(T));
                T val = (T)values.GetValue(property.enumValueIndex);
                Func<Enum, bool> disableFunc = null;
                if (disabledValues != null)
                    disableFunc = (e) => !disabledValues.Contains((T)e);
                val = (T)EditorGUI.EnumPopup(fieldRect, GUIContent.none, val, checkEnabled: disableFunc);
                property.enumValueIndex = (int)Convert.ChangeType(val, typeof(int));
            };

            DrawFieldWithLabel(action, grid, gridPos, label, showLabel);
        }

        private void DrawFieldWithLabel(Action<Rect> drawFieldAction, PropertyGrid grid, GridPosition gridPos, string label = null, bool showLabel = true)
        {
            Vector2 pos = grid.GetPos(gridPos);
            Vector2 size = grid.GetSize(gridPos);

            Rect labelRect = new Rect()
            {
                x = pos.x,
                y = pos.y,
                width = labelWidth,
                height = size.y
            };

            Rect fieldRect = new Rect()
            {
                x = pos.x,
                y = pos.y,
                width = size.x,
                height = size.y
            };

            if (showLabel)
            {
                fieldRect.x += labelWidth;
                fieldRect.width -= labelWidth;
            }

            drawFieldAction(fieldRect);

            if (showLabel)
                DrawLabel(labelRect, label);
        }

        private void DrawLabel(Rect rect, string label)
        {
            if (label == null)
                label = property.displayName;

            EditorGUI.LabelField(rect, label);
        }
    }
}