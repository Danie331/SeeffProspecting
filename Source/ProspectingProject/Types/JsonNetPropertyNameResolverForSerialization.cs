using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

/// <summary>
/// Enfore usage of the original (not json) property names when serializing an object to json.
/// </summary>
public class JsonNetPropertyNameResolverForSerialization : DefaultContractResolver
{
    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        IList<JsonProperty> list = base.CreateProperties(type, memberSerialization);
        foreach (JsonProperty prop in list)
        {
            prop.PropertyName = prop.UnderlyingName;
        }

        return list;
    }
}