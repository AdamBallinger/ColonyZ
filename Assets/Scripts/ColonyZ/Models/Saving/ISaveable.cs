using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ColonyZ.Models.Saving
{
    /// <summary>
    /// Implementing classes can specify to the save system which data for the object should be saved to
    /// disk.
    /// </summary>
    public interface ISaveable
    {
        bool CanSave();

        void OnSave(JsonTextWriter _writer);

        void OnLoad(JToken _dataToken);
    }
}