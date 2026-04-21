using Project.Scripts.Shared;
using UnityEngine;

namespace Project.Scripts.Utils
{
    public static class SharedVectorUtils
    {
        public static SharedVector3 ToSharedVector3(this Vector3 vector)
        {
            return new SharedVector3(vector.x, vector.y, vector.z);
        }

        public static Vector3 ToUnityVector3(this SharedVector3 vector)
        {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }
    }
}