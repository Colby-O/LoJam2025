using Unity.VisualScripting;
using UnityEngine;

namespace LoJam.Core
{
    public static class VectorHelpers
    {
        public static Vector2 SetX(this Vector2 vec, float val)
        {
            return new Vector2(val, vec.y);
        }

        public static Vector2 SetY(this Vector2 vec, float val)
        {
            return new Vector2(vec.x, val);
        }

        public static Vector3 SetX(this Vector3 vec, float val)
        {
            return new Vector3(val, vec.y, vec.z);
        }

        public static Vector3 SetY(this Vector3 vec, float val)
        {
            return new Vector3(vec.x, val, vec.z);
        }

        public static Vector3 SetZ(this Vector3 vec, float val)
        {
            return new Vector3(vec.x, vec.y, val);
        }
    }
}
