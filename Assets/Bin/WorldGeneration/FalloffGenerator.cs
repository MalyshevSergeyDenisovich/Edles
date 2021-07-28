using UnityEngine;

namespace Bin.WorldGeneration
{
    public static class FalloffGenerator
    {
        public static float[,] GenerateFallofMap(int size)
        {
            var map = new float[size, size];

            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    var x = i / (float) size * 2 - 1;
                    var y = j / (float) size * 2 - 1;

                    var value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                    map[i, j] = Evaluate(value);
                }
            }
            return map;
        }

        static float Evaluate(float value)
        {
            const float a = 3f;
            const float b = 2.2f;

            var valuesPowA = Mathf.Pow(value, a);
            var bOnValuePowA = Mathf.Pow(b - b * value, a);
            
            return valuesPowA / (valuesPowA + bOnValuePowA);
        }
    }
}