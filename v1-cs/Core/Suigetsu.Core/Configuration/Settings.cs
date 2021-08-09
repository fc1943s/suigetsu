using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Suigetsu.Core.Configuration
{
    //TODO: testar com GET em tipos de instancias diferentes
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class Settings
    {
        private static readonly Dictionary<Type, Settings> InstanceList = new Dictionary<Type, Settings>();

        public static T Get<T>() where T : Settings, new()
        {
            lock(InstanceList)
            {
                if(!InstanceList.ContainsKey(typeof(T)))
                {
                    Settings obj = new T();

                    foreach(var v in typeof(T).GetProperties())
                    {
                        var strValue = ConfigurationManager.AppSettings[v.Name];

                        if(strValue == null)
                        {
                            foreach(var v2 in
                                v.GetCustomAttributes(true).Where(v2 => v2.GetType() == typeof(DefaultValueAttribute)))
                            {
                                strValue = ((DefaultValueAttribute)v2).Value.ToString();
                            }
                        }

                        if(strValue == null)
                        {
                            throw new Exception($"Unable to find value of parameter {v.Name}.");
                        }

                        if(!v.GetGetMethod(true).IsPublic)
                        {
                            throw new Exception
                                ($"The get method must be public. Parameter: {v.Name}. Class: {obj.GetType().FullName}");
                        }

                        if(!v.GetSetMethod(true).IsFamily)
                        {
                            throw new Exception
                                ($"The set method must be protected. Parameter: {v.Name}. Class: {obj.GetType().FullName}");
                        }

                        v.SetValue(obj, Convert.ChangeType(strValue, v.PropertyType, CultureInfo.InvariantCulture), null);
                    }

                    InstanceList.Add(typeof(T), obj);
                }

                return (T)InstanceList[typeof(T)];
            }
        }

        #region Parameters

        [DefaultValue(true)]
        public bool FileLog { get; protected set; }

        [DefaultValue(10000)]
        public int SocketTimeout { get; protected set; }

        [DefaultValue("dd/MM/yyyy HH:mm:ss")]
        public string DefaultDateFormat { get; protected set; }

        #endregion
    }
}
