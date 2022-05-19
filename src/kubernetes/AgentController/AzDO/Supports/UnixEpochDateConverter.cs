

using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AgentController.AzDO.Supports
{
    sealed class UnixEpochDateConverter : JsonConverter<DateTimeOffset>
    {
        static readonly DateTimeOffset s_epoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
        
        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Very special parsing for undocumented Azure DevOps dates            
            var formatted = new string(reader.GetString().Where(ch => Char.IsDigit(ch)).ToArray());
            if(Int64.TryParse(formatted, out var unixEpochValue))
            {
                var doffset = DateTimeOffset.FromUnixTimeMilliseconds(unixEpochValue);
                return doffset;
            }

            throw new Exception("Unexpected value format, unable to parse DateTimeOffset.");
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            long unixTime = Convert.ToInt64((value - s_epoch).TotalMilliseconds);
            TimeSpan utcOffset = value.Offset;

            string formatted = FormattableString.Invariant($"/Date({unixTime}{(utcOffset >= TimeSpan.Zero ? "+" : "-")}{utcOffset:hhmm})/");
            writer.WriteStringValue(formatted);
        }
    }
}
