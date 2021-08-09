using System;
using Newtonsoft.Json.Serialization;

namespace Suigetsu.Core.Serialization
{
    /// <summary>
    ///     Parameters for quick initialization of the <see cref="JsonSerializerSettingsWrapper" /> class.
    /// </summary>
    public class JsonSerializerSettingsWrapperParameters
    {
        /// <summary>
        ///     Determines if the json will be indented according to the
        ///     <see cref="T:Newtonsoft.Json.JsonTextWriter" /> default settings.
        /// </summary>
        public bool Indented = false;

        /// <summary>
        ///     Event that fires on every contract created.
        /// </summary>
        public Action<JsonContract> OnCreateContract = null;

        /// <summary>
        ///     Event that fires on every property created.
        /// </summary>
        public Action<JsonProperty> OnCreateProperty = null;

        /// <summary>
        ///     Determines if the built-in contracts of the library will be registered.
        /// </summary>
        public bool RegisterCustomContracts = true;
    }
}
