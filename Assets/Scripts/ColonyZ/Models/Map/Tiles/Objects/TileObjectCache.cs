using System.Collections.Generic;
using UnityEngine;

namespace ColonyZ.Models.Map.Tiles.Objects
{
    public static class TileObjectCache
    {
        public static List<TileObject> TileObjects { get; } = new List<TileObject>();

        private static TileObject FoundationObject { get; set; }

        public static void Add(TileObject _object)
        {
            if (_object == null)
            {
                return;
            }

            if (TileObjects.Contains(_object))
            {
                return;
            }

            if (_object is FoundationObject)
            {
                FoundationObject = _object;
            }

            TileObjects.Add(_object);
        }

        /// <summary>
        ///     Returns a new instance for the tile object with provided name.
        /// </summary>
        /// <param name="_objectName"></param>
        /// <returns></returns>
        public static TileObject GetObject(string _objectName)
        {
            foreach (var obj in TileObjects)
            {
                if (obj.ObjectName.Equals(_objectName))
                {
                    return Object.Instantiate(obj);
                }
            }

            return null;
        }

        /// <summary>
        ///     Returns a copy of the provided object.
        /// </summary>
        /// <param name="_object"></param>
        /// <returns></returns>
        public static TileObject GetObject(TileObject _object)
        {
            return GetObject(_object.ObjectName);
        }

        /// <summary>
        ///     Returns a new instance of the foundation object.
        /// </summary>
        /// <returns></returns>
        public static TileObject GetFoundation()
        {
            return Object.Instantiate(FoundationObject);
        }
    }
}