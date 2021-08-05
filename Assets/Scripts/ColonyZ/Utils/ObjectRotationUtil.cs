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
            offset.x = 0.5f * (GetRotatedObjectWidth(_object, _rotation) - 1);
            offset.y = -0.5f * (GetRotatedObjectHeight(_object, _rotation) - 1);

            return offset;
        }

        /// <summary>
        ///     Returns the width of an object using its current rotation.
        /// </summary>
        /// <param name="_object"></param>
        /// <returns></returns>
        public static int GetRotatedObjectWidth(TileObject _object)
        {
            return GetRotatedObjectWidth(_object, _object.ObjectRotation);
        }

        /// <summary>
        ///     Returns the height of an object using its current rotation.
        /// </summary>
        /// <param name="_object"></param>
        /// <returns></returns>
        public static int GetRotatedObjectHeight(TileObject _object)
        {
            return GetRotatedObjectHeight(_object, _object.ObjectRotation);
        }

        /// <summary>
        ///     Returns the width of the object using the provided rotation.
        /// </summary>
        /// <param name="_object"></param>
        /// <param name="_rot"></param>
        /// <returns></returns>
        public static int GetRotatedObjectWidth(TileObject _object, ObjectRotation _rot)
        {
            if (_rot == ObjectRotation.Default || _rot == ObjectRotation.Clock_Wise_180)
            {
                return _object.Width;
            }

            return _object.Height;
        }
        
        /// <summary>
        ///     Returns the height of an object using the provided rotation.
        /// </summary>
        /// <param name="_object"></param>
        /// <param name="_rot"></param>
        /// <returns></returns>
        public static int GetRotatedObjectHeight(TileObject _object, ObjectRotation _rot)
        {
            if (_rot == ObjectRotation.Default || _rot == ObjectRotation.Clock_Wise_180)
            {
                return _object.Height;
            }

            return _object.Width;
        }
    }
}