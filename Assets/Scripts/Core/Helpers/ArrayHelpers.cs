using System;
using Unity.Burst.CompilerServices;
using UnityEngine;

namespace LoJam.Core
{
    public static class ArrayHelpers
    {
        public static void Fill<T>(T[,] arr, Func<int, int, T> generator)
        {
            for (int y = 0; y < arr.GetLength(0); y++)
            {
                for (int x = 0; x < arr.GetLength(1); x++)
                {
                    arr[y, x] = generator(x, y);
                }
            }
        }

        public static void CreateAndFill<T>(out T[,] arr, Vector2Int size, Func<int, int, T> generator)
        {
            arr = new T[size.y, size.x];
            Fill(arr, generator);
        }

        public static T[,] ExtractRegion<T>(T[,] arr, Vector2Int center, int size)
        {
            T[,] subArr = new T[size, size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    if 
                    (
                        center.y + y - Mathf.FloorToInt(size / 2f) >= 0               &&
                        center.y + y - Mathf.FloorToInt(size / 2f) < arr.GetLength(0) &&
                        center.x + x - Mathf.FloorToInt(size / 2f) >= 0               &&
                        center.x + x - Mathf.FloorToInt(size / 2f) < arr.GetLength(1)
                    )
                    {
                        subArr[y, x] = arr[center.y + y - Mathf.FloorToInt(size / 2f), center.x + x - Mathf.FloorToInt(size / 2f)];
                    }
                    else
                    {
                        subArr[y, x] = default;

                    }
                }
            }

            return subArr;
        }
    }
}
