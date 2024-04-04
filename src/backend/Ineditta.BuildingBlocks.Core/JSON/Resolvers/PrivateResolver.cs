using System.Reflection;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Ineditta.BuildingBlocks.Core.JSON.Resolvers
{
    public class PrivateResolver : DefaultContractResolver
    {
        public PrivateResolver()
        {
                
        }

        public PrivateResolver(NamingStrategy namingStrategy)
        {
            NamingStrategy = namingStrategy;
        }
        
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);
            if (!prop.Writable)
            {
                var property = member as PropertyInfo;
                var hasPrivateSetter = property?.GetSetMethod(true) != null;
                prop.Writable = hasPrivateSetter;
            }
            return prop;
        }
    }
}
