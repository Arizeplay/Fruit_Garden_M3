

namespace AS.SafeArea
{
    [System.Serializable]
    public class FloatRectOffset
    {

        public float left = 0f;
        public float right = 0f;
        public float top = 0f;
        public float bottom = 0f;

        public float horizontal
        {
            get
            {
                return left + right;
            }
        }
        public float vertical
        {
            get
            {
                return top + bottom;
            }
        }

        public FloatRectOffset(float left, float right, float top, float bottom)
        {
            this.left = left;
            this.right = right;
            this.top = top;
            this.bottom = bottom;
        }

        public FloatRectOffset(FloatRectOffset floatRectOffset)
        {
            left = floatRectOffset.left;
            right = floatRectOffset.right;
            top = floatRectOffset.top;
            bottom = floatRectOffset.bottom;
        }

        public override bool Equals(object obj)
        {
            return obj is FloatRectOffset offset &&
                   left == offset.left &&
                   right == offset.right &&
                   top == offset.top &&
                   bottom == offset.bottom;
        }

        public override int GetHashCode()
        {
            return System.Tuple.Create(left, right, top, bottom).GetHashCode();
        }
    }
}