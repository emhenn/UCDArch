using UCDArch.Core.DomainModel;
using UCDArch.Testing;
using Newtonsoft.Json;
using Xunit;

namespace UCDArch.Tests.UCDArch.Core.JsonNet
{
    public class JsonSerializerTests
    {
        [Fact]
        public void DomainObjectReturnsJustId()
        {
            var obj = new SimpleDomainObject();

            obj.SetIdTo(42);

            var result = JsonConvert.SerializeObject(obj);

            const string jsonString = "{\"Id\":42}";

            Assert.Equal(jsonString, result);
        }

        [Fact]
        public void AddingSimplePropertyReturnsJustId()
        {
            var obj = new NamedDomainObject {Name = "TestName"};

            obj.SetIdTo(42);

            var result = JsonConvert.SerializeObject(obj);

            const string jsonString = "{\"Id\":42}";

            Assert.Equal(jsonString, result);
        }

        [Fact]
        public void AddingSimplePropertyWithJsonAttributeReturnsIdAndName()
        {
            var obj = new NamedDomainObjectWithPropertyIncluded { Name = "TestName" };

            obj.SetIdTo(42);

            var result = JsonConvert.SerializeObject(obj);

            const string jsonString = "{\"Name\":\"TestName\",\"Id\":42}";

            Assert.Equal(jsonString, result);
        }
    }

    public class SimpleDomainObject : DomainObject{}

    public class NamedDomainObject : DomainObject
    {
        public virtual string Name { get; set; }
    }

    public class NamedDomainObjectWithPropertyIncluded : DomainObject
    {
        [JsonProperty]
        public virtual string Name { get; set; }
    }
}