using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UCDArch.Core.DomainModel;
using UCDArch.Testing;
using Xunit;

namespace UCDArch.Tests.UCDArch.Core.DomainModel
{
    public class BaseObjectEqualityComparerTests
    {
        [Fact]
        public void CanCompareNulls()
        {
            var comparer = new BaseObjectEqualityComparer<BaseObject>();

            Assert.True(comparer.Equals(null, null));
            Assert.False(comparer.Equals(null, new ConcreteBaseObject()));
            Assert.False(comparer.Equals(new ConcreteBaseObject(), null));
        }

        [Fact]
        public void CanCompareBaseObjects()
        {
            var comparer = new BaseObjectEqualityComparer<BaseObject>();

            var object1 = new ConcreteBaseObject
                              {
                                  Name = "Whatever"
                              };
            var object2 = new ConcreteBaseObject
                              {
                                  Name = "Whatever"
                              };
            Assert.True(comparer.Equals(object1, object2));

            object2.Name = "Mismatch";
            Assert.False(comparer.Equals(object1, object2));
        }

        [Fact]
        public void CanCompareEntitiesWithNoDomainSignatureProperties()
        {
            var comparer = new BaseObjectEqualityComparer<BaseObject>();

            var object1 = new ConcreteEntityWithNoDomainSignatureProperties
                              {
                                  Name = "Whatever"
                              };
            var object2 = new ConcreteEntityWithNoDomainSignatureProperties
                              {
                                  Name = "asdf"
                              };
            Assert.False(comparer.Equals(object1, object2));

            EntityIdSetter.SetIdOf(object1, 1);
            EntityIdSetter.SetIdOf(object2, 1);
            Assert.True(comparer.Equals(object1, object2));
        }

        [Fact]
        public void CanCompareEntitiesWithDomainSignatureProperties()
        {
            var comparer = new BaseObjectEqualityComparer<DomainObject>();

            var object1 = new ConcreteEntityWithDomainSignatureProperties
                              {
                                  Name = "Whatever"
                              };
            var object2 = new ConcreteEntityWithDomainSignatureProperties
                              {
                                  Name = "Whatever"
                              };
            Assert.True(comparer.Equals(object1, object2));

            object2.Name = "Mismatch";
            Assert.False(comparer.Equals(object1, object2));

            EntityIdSetter.SetIdOf(object1, 1);
            EntityIdSetter.SetIdOf(object2, 1);
            Assert.True(comparer.Equals(object1, object2));
        }

        [Fact]
        public void CanBeUsedByLinqSetOperatorsSuchAsIntersect()
        {
            IList<ConcreteEntityWithDomainSignatureProperties> objects1 =
                new List<ConcreteEntityWithDomainSignatureProperties>();
            var object1 = new ConcreteEntityWithDomainSignatureProperties
                              {
                                  Name = "Billy McCafferty",
                              };
            EntityIdSetter.SetIdOf(object1, 2);
            objects1.Add(object1);

            IList<ConcreteEntityWithDomainSignatureProperties> objects2 =
                new List<ConcreteEntityWithDomainSignatureProperties>();
            var object2 = new ConcreteEntityWithDomainSignatureProperties
                              {
                                  Name = "Jimi Hendrix",
                              };
            EntityIdSetter.SetIdOf(object2, 1);
            objects2.Add(object2);
            var object3 = new ConcreteEntityWithDomainSignatureProperties
                              {
                                  Name =
                                      "Doesn't Matter since the Id will match and the presedence of the domain signature will go overridden",
                              };
            EntityIdSetter.SetIdOf(object3, 2);
            objects2.Add(object3);

            Assert.Equal(1, objects1.Intersect(objects2,
                                                  new BaseObjectEqualityComparer
                                                      <ConcreteEntityWithDomainSignatureProperties>()).Count());
            Assert.Equal(objects1.Intersect(objects2,
                                               new BaseObjectEqualityComparer
                                                   <ConcreteEntityWithDomainSignatureProperties>()).First(), object1);
            Assert.Equal(objects1.Intersect(objects2,
                                               new BaseObjectEqualityComparer
                                                   <ConcreteEntityWithDomainSignatureProperties>()).First(), object3);
        }

        #region Nested type: ConcreteBaseObject

        private class ConcreteBaseObject : BaseObject
        {
            public string Name { get; set; }

            protected override IEnumerable<PropertyInfo> GetTypeSpecificSignatureProperties()
            {
                return GetType().GetProperties();
            }
        }

        #endregion

        #region Nested type: ConcreteEntityWithDomainSignatureProperties

        private class ConcreteEntityWithDomainSignatureProperties : DomainObject
        {
            [DomainSignature]
            public string Name { get; set; }
        }

        #endregion

        #region Nested type: ConcreteEntityWithNoDomainSignatureProperties

        private class ConcreteEntityWithNoDomainSignatureProperties : DomainObject
        {
            public string Name { get; set; }
        }

        #endregion
    }
}