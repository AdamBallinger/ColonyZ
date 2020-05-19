using ColonyZ.Models.Saving;
using ColonyZ.Models.TimeSystem;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ColonyZ.Models.Map
{
    public class WorldDataProvider : ISaveable
    {
        public int WorldWidth { get; private set; }
        public int WorldHeight { get; private set; }

        public WorldDataProvider(int _width, int _height)
        {
            WorldWidth = _width;
            WorldHeight = _height;
        }

        public bool CanSave()
        {
            return true;
        }

        public void OnSave(JsonTextWriter _writer)
        {
            _writer.WritePropertyName("width");
            _writer.WriteValue(WorldWidth);
            _writer.WritePropertyName("height");
            _writer.WriteValue(WorldWidth);
            _writer.WritePropertyName("Time");
            _writer.WriteStartArray();
            _writer.WriteValue(TimeManager.Instance.Day);
            _writer.WriteValue(TimeManager.Instance.Hour);
            _writer.WriteValue(TimeManager.Instance.Minute);
            _writer.WriteEnd();
        }

        public void OnLoad(JToken _dataToken)
        {
            WorldWidth = _dataToken["width"].Value<int>();
            WorldHeight = _dataToken["height"].Value<int>();
            var day = _dataToken["Time"][0].Value<int>();
            var hour = _dataToken["Time"][1].Value<int>();
            var minute = _dataToken["Time"][2].Value<int>();

            TimeManager.Instance.SetTime(hour, minute, day);
        }
    }
}