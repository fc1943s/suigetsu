using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Suigetsu.Core.Enums;

namespace Suigetsu.Core.Serialization
{
    /// <summary>
    ///     Json converter for <see langword="enum" /> types that uses the
    ///     <see cref="T:Suigetsu.Core.Enums.EnumRepo" />'s Id value for conversion.
    /// </summary>
    public class JsonEnumConverter : JsonConverter
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            => writer.WriteValue(EnumRepo.Get((Enum)value).Id);

        public override object ReadJson
            (JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            => EnumRepo.EnumById(objectType, (string)reader.Value);

        [ExcludeFromCodeCoverage]
        public override bool CanConvert(Type objectType) => objectType.IsEnum;

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
