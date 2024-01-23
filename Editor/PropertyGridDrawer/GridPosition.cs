using UnityEngine;

namespace And.Editor
{
    public struct GridPosition
    {
        public Vector2Int index;
        public Vector2Int span;

        public int ColumnIndex => index.x;
        public int RowIndex => index.y;
        public bool IsFirstRow => RowIndex == 0;

        public GridPosition(Vector2Int index)
        {
            this.index = index;
            span = Vector2Int.one;
        }

        public GridPosition(Vector2Int index, Vector2Int span)
        {
            this.index = index;
            this.span = span;
        }
    }
}