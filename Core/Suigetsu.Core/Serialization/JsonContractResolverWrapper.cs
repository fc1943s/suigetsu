using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Suigetsu.Core.Serialization
{
    /// <summary>
    ///     Wrapper for the <see cref="DefaultContractResolver" /> events, allowing them to be
    ///     defined as properties, making them optional and removing the need to create a new class for every rule.
    /// </summary>
    public class JsonContractResolverWrapper : DefaultContractResolver
    {
        /// <summary>
        ///     Event that fires on every property created.
        /// </summary>
        public Action<JsonProperty> OnCreateProperty { private get; set; }

        /// <summary>
        ///     Event that fires on every type contract created.
        /// </summary>
        public Action<JsonContract> OnCreateContract { private get; set; }

        /// <summary>
        ///     Creates a <see cref="T:Newtonsoft.Json.Serialization.JsonProperty" /> for the given
        ///     <see cref="T:System.Reflection.MemberInfo" />.
        /// </summary>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            OnCreateProperty?.Invoke(property);
            return property;
        }

        /// <summary>
        ///     Determines which contract type is created for the given type.
        /// </summary>
        protected override JsonContract CreateContract(Type objectType)
        {
            var contract = base.CreateContract(objectType);
            OnCreateContract?.Invoke(contract);
            return contract;
        }
    }
}
