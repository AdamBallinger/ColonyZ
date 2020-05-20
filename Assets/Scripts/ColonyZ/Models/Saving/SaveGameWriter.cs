using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace ColonyZ.Models.Saving
{
    public class SaveGameWriter
    {
        private JsonTextWriter writer;
        private StringBuilder stringBuilder;
        private StringWriter stringWriter;

        public void StartNew()
        {
            writer?.Flush();
            stringBuilder = new StringBuilder();
            stringWriter = new StringWriter(stringBuilder);
            writer = new JsonTextWriter(stringWriter);
            writer.Formatting = Formatting.Indented;
        }

        public void BeginObject()
        {
            writer.WriteStartObject();
        }

        public void EndObject()
        {
            writer.WriteEndObject();
        }

        public void BeginArray()
        {
            writer.WriteStartArray();
        }

        public void EndArray()
        {
            writer.WriteEnd();
        }

        public void WriteProperty<T>(string _name, T _value)
        {
            writer.WritePropertyName(_name);
            writer.WriteValue(_value);
        }

        public void WriteProperty(string _name)
        {
            writer.WritePropertyName(_name);
        }

        public void WriteSet<T>(string _name, params T[] _values)
        {
            writer.WritePropertyName(_name);
            writer.WriteStartArray();
            foreach (var val in _values)
            {
                writer.WriteValue(val);
            }

            writer.WriteEnd();
        }

        public string GetJson()
        {
            return stringBuilder.ToString();
        }
    }
}