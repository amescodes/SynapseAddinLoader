using System;
using Newtonsoft.Json;

namespace SynapseAddinLoader.Core.Json
{
    public class SystemTypeConverter : JsonConverter<Type>
    {
        public override void WriteJson(JsonWriter writer, Type value, JsonSerializer serializer)
        {
            Type type = value;
            writer.WriteValue(type.AssemblyQualifiedName);
        }

        public override Type ReadJson(JsonReader reader, Type objectType, Type existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            string typeName = (string)reader.Value;
            return Type.GetType(typeName);
        }
    }
}