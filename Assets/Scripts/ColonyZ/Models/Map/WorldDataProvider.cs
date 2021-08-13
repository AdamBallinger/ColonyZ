using System.IO;
using ColonyZ.Models.Saving;
using ColonyZ.Models.TimeSystem;
using Newtonsoft.Json.Linq;

namespace ColonyZ.Models.Map
{
    public class WorldDataProvider : ISaveable
    {
        public int WorldWidth => Size.Width;
        public int WorldHeight => Size.Height;

        public bool GodMode { get; private set; }

        public WorldSizeTypes.WorldSize Size { get; private set; }

        public WorldDataProvider(WorldSizeTypes.WorldSize _worldSize)
        {
            Size = _worldSize;
            TimeManager.Create(8, 0, 1);
        }

        public bool CanSave()
        {
            return true;
        }

        public void OnSave(SaveGameWriter _writer)
        {
            _writer.Comment("Modifying this value will likely corrupt your save game. This is not Budz proof.");
            _writer.WriteProperty("size", Size.Name);
            _writer.WriteProperty("god_mode", World.Instance.WorldActionProcessor.GodMode);
            _writer.WriteSet("Time",
                TimeManager.Instance.Day,
                TimeManager.Instance.Hour,
                TimeManager.Instance.Minute);
        }

        public void OnLoad(JToken _dataToken)
        {
            var size = _dataToken["size"].Value<string>();
            var day = _dataToken["Time"][0].Value<int>();
            var hour = _dataToken["Time"][1].Value<int>();
            var minute = _dataToken["Time"][2].Value<int>();
            GodMode = _dataToken["god_mode"].Value<bool>();

            Size = WorldSizeTypes.SIZES.Find(s => s.Name == size);

            if (Size.Name == null) throw new InvalidDataException("Failed to load world size.");

            TimeManager.Instance.SetTime(hour, minute, day);
        }
    }
}