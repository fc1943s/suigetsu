using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Suigetsu.Core.Serialization
{
    /// <summary>
    ///     Wrapper for the <see cref="JsonSerializerSettings" /> class initialization, automatically registering
    ///     custom contracts and making customization easier through optional parameters.
    /// </summary>
    public class JsonSerializerSettingsWrapper : JsonSerializerSettings
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:JsonSerializerSettingsWrapper" /> class.
        /// </summary>
        public JsonSerializerSettingsWrapper(JsonSerializerSettingsWrapperParameters parameters)
        {
            parameters = parameters ?? new JsonSerializerSettingsWrapperParameters();

            NullValueHandling = NullValueHandling.Ignore;
            Formatting = parameters.Indented ? Formatting.Indented : Formatting.None;
            ContractResolver = new JsonContractResolverWrapper
            {
                OnCreateContract = contract =>
                {
                    parameters.OnCreateContract?.Invoke(contract);
                    if(parameters.RegisterCustomContracts)
                    {
                        if(contract.UnderlyingType.IsEnum)
                        {
                            contract.Converter = new JsonEnumConverter();
                        }
                    }
                },
                OnCreateProperty = property => parameters.OnCreateProperty?.Invoke(property)
            };
        }

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
}
