using Newtonsoft.Json;

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
    }
}
