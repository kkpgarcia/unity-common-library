using UnityEngine;

namespace Common.ObjectPointer.Math
{
    public static class Vector3Extentions
    {
        public static float MagnitudeInDirection(Vector3 vector, Vector3 direction, bool normalizeParameters = true)
        {
            if(normalizeParameters) direction.Normalize();
            return Vector3.Dot(vector, direction);
        }

        public static bool IsInDirection(Vector3 direction, Vector3 otherDirection)
        {
            return Vector3.Dot(direction, otherDirection) > 0f;
        }
    }
    
}