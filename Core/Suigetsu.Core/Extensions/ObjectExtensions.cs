using Newtonsoft.Json;
using Suigetsu.Core.Serialization;

namespace Suigetsu.Core.Extensions
{
    /// <summary>
    ///     Extensions for the <see cref="T:System.Object" /> class.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        ///     Serializes the <paramref name="obj" /> to a json string using the given <paramref name="settings" />.
        /// </summary>
        public static string ToJson(this object obj,
                                    JsonSerializerSettingsWrapper.JsonSerializerSettingsWrapperParameters settings =
                                        null)
            => JsonConvert.SerializeObject(obj, new JsonSerializerSettingsWrapper(settings));
    }
}
