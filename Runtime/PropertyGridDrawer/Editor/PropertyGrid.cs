using UnityEngine;

namespace And.Editor
{
    public struct PropertyGrid
    {
        public Vector2 pivot;
        public Vector2 size;
        public Vector2 margin;
        public Vector2 spacing;
        public Vector2Int count;
        public float prefixIndent;
        public float indentValue;
        public int indentLevel;

        public Vector2 GetPos(GridPosition gridPos)
        {
            Vector2 cellSize = GetCellSize(gridPos.IsFirstRow);

            Vector2 fieldPos = pivot + GetSpanSize(cellSize, spacing, gridPos.index);

            if (gridPos.ColumnIndex > 0)
                fieldPos.x += spacing.x;
            if (gridPos.RowIndex > 0)
                fieldPos.y += spacing.y;

            fieldPos.x += GetIndent(gridPos.IsFirstRow);

            int rowIndentLevel = indentLevel;
            fieldPos.x += rowIndentLevel * indentValue;

            return fieldPos;
        }


        public Vector2 GetSize(GridPosition gridPos)
        {
            Vector2 cellSize = GetCellSize(gridPos.IsFirstRow);

            Vector2 spanSize = GetSpanSize(cellSize, spacing, gridPos.span);

            return spanSize;
        }

        private Vector2 GetCellSize(bool isFirstRow)
        {
            Vector2 gridSize = size;
            gridSize.x -= GetIndent(isFirstRow);

            Vector2 cellSize = gridSize - spacing * (count - Vector2.one);
            cellSize.x /= count.x;
            cellSize.y /= count.y;

            return cellSize;
        }

        private float GetIndent(bool isFirstRow)
        {
            float indent = isFirstRow ? 0 : indentLevel * indentValue;

            if (isFirstRow)
                indent += prefixIndent;
            else
                indent += indentValue;

            return indent;
        }

        public static Vector2 GetSpanSize(Vector2 cellSize, Vector2 marginSize, Vector2Int count)
        {
            return cellSize * count + marginSize * Vector2Int.Max(count - Vector2Int.one, Vector2Int.zero);
        }

        public static float GetSpanSize(float cellSize, float spacingSize, int count)
        {
            return cellSize * count + spacingSize * Mathf.Max(count - 1, 0);
        }
    }
}