using System.Collections.Generic;
using System.Linq;
using ColonyZ.Models.Map.Tiles.Objects.Data;

namespace ColonyZ.Models.Map.Tiles.Objects
{
    public static class TileObjectDataCache
    {
        public static List<TileObjectData> ObjectDatas { get; } = new List<TileObjectData>();

        private static TileObjectData FoundationObject { get; set; }

        public static void Add(TileObjectData _data)
        {
            if (_data == null)
            {
                return;
            }

            if (ObjectDatas.Contains(_data))
            {
                return;
            }

            if (_data.ObjectName == "Foundation")
            {
                FoundationObject = _data;
            }

            ObjectDatas.Add(_data);
        }

        /// <summary>
        ///     Returns the object data for the given object name.
        /// </summary>
        /// <param name="_objectName"></param>
        /// <returns></returns>
        public static T GetData<T>(string _objectName) where T : TileObjectData
        {
            return (T)ObjectDatas.FirstOrDefault(obj => obj.ObjectName.Equals(_objectName));
        }

        public static TileObjectData GetData(string _objectName)
        {
            return GetData<TileObjectData>(_objectName);
        }

        /// <summary>
        ///     Returns data for the foundation object.
        /// </summary>
        /// <returns></returns>
        public static TileObjectData GetFoundation()
        {
            return FoundationObject;
        }
    }
}