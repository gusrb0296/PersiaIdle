using UnityEngine;

namespace Utils
{
    public class EaseEquation
    {
        public static float InQuad(float x)
        {
            return x * x;
        }

        public static float OutQuad(float x)
        {
            return 1 - (1 - x) * (1 - x);
        }

        public static float InOutQuad(float x)
        {
            if (x < 0.5)
                return 2 * x * x;
            return 1 - 2 * (x - 1) * (x - 1);
        }

    }

    public class Vector
    {
        public static float BoxDistance(Vector2 from, Vector2 to, float xScale, float yScale)
        {
            Vector2 newFrom = new Vector2(from.x * xScale, from.y * yScale);
            Vector2 newTo = new Vector2(to.x * xScale, to.y * yScale);

            return Vector2.Distance(newFrom, newTo);
        }
    }
}
