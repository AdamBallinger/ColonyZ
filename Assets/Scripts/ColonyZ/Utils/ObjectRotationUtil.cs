using ColonyZ.Models.Map.Tiles.Objects;
using UnityEngine;

namespace ColonyZ.Utils
{
    public static class ObjectRotationUtil
    {
        public static Quaternion GetQuaternion(ObjectRotation _rotation)
        {
            switch (_rotation)
            {
                case ObjectRotation.Default:
                case ObjectRotation.Clock_Wise_180:
                    return Quaternion.identity;
                case ObjectRotation.Clock_Wise_90:
                    return Quaternion.Euler(new Vector3(0, 0, 90));
                case ObjectRotation.Clock_Wise_270:
                    return Quaternion.Euler(new Vector3(0, 0, 270));
            }

            return Quaternion.identity;
        }

        public static Vector2 GetObjectRotationPositionOffset(TileObject _object, ObjectRotation _rotation)
        {
            if (!_object.Rotatable) return Vector2.zero;
            
            var offset = Vector2.zero;
            // Switch offsets when rotating between 90 and 270 degrees.
            offset.x = 0.5f * (_rotation == ObjectRotation.Default || _rotation == ObjectRotation.Clock_Wise_180
                ? _object.Width - 1
                : _object.Height - 1);
            offset.y = -0.5f * (_rotation == ObjectRotation.Default || _rotation == ObjectRotation.Clock_Wise_180 
                ? _object.Height - 1
                : _object.Width - 1);

            return offset;
        }
    }
}