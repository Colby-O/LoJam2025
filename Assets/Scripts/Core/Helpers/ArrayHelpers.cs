using System;
using Unity.Burst.CompilerServices;
using UnityEngine;

namespace LoJam.Core
{
    public static class ArrayHelpers
    {
        public static void Fill<T>(T[,] arr, Func<int, int, T> generator)
        {
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    arr[i, j] = generator(j, i);
                }
            }
        }

        public static void CreateAndFill<T>(out T[,] arr, Vector2Int size, Func<int, int, T> generator)
        {
            arr = new T[size.x, size.y];
            Fill(arr, generator);
        }

        public static T[,] ExtractRegion<T>(T[,] arr, Vector2Int center, int size)
        {
            T[,] subArr = new T[size, size];

            for (int i = -Mathf.FloorToInt(size / 2); i <= Mathf.CeilToInt(size / 2); i++)
            {
                for (int j = -Mathf.FloorToInt(size / 2); j <= Mathf.CeilToInt(size / 2); j++)
                {
                    if 
                    (
                        center.x + i >= 0 &&
                        center.x + i < arr.GetLength(0) &&
                        center.y + j >= 0 &&
                        center.y + j < arr.GetLength(1)
                    )
                    {
                        subArr[i + Mathf.FloorToInt(size / 2), j + Mathf.FloorToInt(size / 2)] = arr[center.x + i, center.y + j];
                    }
                    else
                    {
                        subArr[i + Mathf.FloorToInt(size / 2), j + Mathf.FloorToInt(size / 2)] = default;

                    }
                }
            }

            return subArr;
        }
    }
}
