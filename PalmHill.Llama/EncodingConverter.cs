using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text;

namespace PalmHill.Llama
{

    public class EncodingConverter : JsonConverter<Encoding>
    {
        public override Encoding Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Return a default Encoding or null, since we don't expect to deserialize this type
            var encodingString = reader.GetString();

            if (encodingString != null)
            {
                return Encoding.GetEncoding(encodingString);
            }
            else
            {
                return Encoding.UTF8;
            }
        }

        public override void Write(Utf8JsonWriter writer, Encoding value, JsonSerializerOptions options)
        {
            // Do nothing to effectively skip serialization of Encoding type properties
            writer.WriteStringValue(value.EncodingName);
        }
    }

}
