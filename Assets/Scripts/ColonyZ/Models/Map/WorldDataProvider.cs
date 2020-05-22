using ColonyZ.Controllers;
using ColonyZ.Models.Saving;
using ColonyZ.Models.TimeSystem;
using Newtonsoft.Json.Linq;

namespace ColonyZ.Models.Map
{
    public class WorldDataProvider : ISaveable
    {
        public int WorldWidth { get; private set; }
        public int WorldHeight { get; private set; }

        public bool GodMode { get; private set; }

        public WorldDataProvider(WorldSizeTypes.WorldSize _worldSize)
        {
            WorldWidth = _worldSize.Width;
            WorldHeight = _worldSize.Height;
            TimeManager.Create(6, 0, 1);
        }

        public bool CanSave()
        {
            return true;
        }

        public void OnSave(SaveGameWriter _writer)
        {
            _writer.WriteProperty("width", WorldWidth);
            _writer.WriteProperty("height", WorldHeight);
            // TODO: Move god mode into a settings class, and save/load settings into world.json
            _writer.WriteProperty("god_mode", MouseController.Instance.BuildModeController.GodMode);
            _writer.WriteSet("Time",
                TimeManager.Instance.Day,
                TimeManager.Instance.Hour,
                TimeManager.Instance.Minute);
        }

        public void OnLoad(JToken _dataToken)
        {
            WorldWidth = _dataToken["width"].Value<int>();
            WorldHeight = _dataToken["height"].Value<int>();
            var day = _dataToken["Time"][0].Value<int>();
            var hour = _dataToken["Time"][1].Value<int>();
            var minute = _dataToken["Time"][2].Value<int>();
            GodMode = _dataToken["god_mode"].Value<bool>();

            TimeManager.Instance.SetTime(hour, minute, day);
        }
    }
}