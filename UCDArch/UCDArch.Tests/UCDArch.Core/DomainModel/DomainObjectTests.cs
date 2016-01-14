using System;
using UCDArch.Core.DomainModel;
using UCDArch.Testing;
using Xunit;

namespace UCDArch.Tests.UCDArch.Core.DomainModel
{
    public class DomainObjectTests : IDisposable
    {
        public DomainObjectTests()
        {
            _obj = new MockEntityObjectWithDefaultId
            {
                FirstName = "FName1",
                LastName = "LName1",
                Email = "testus...@mail.com"
            };
            _sameObj = new MockEntityObjectWithDefaultId
            {
                FirstName = "FName1",
                LastName = "LName1",
                Email = "testus...@mail.com"
            };
            _diffObj = new MockEntityObjectWithDefaultId
            {
                FirstName = "FName2",
                LastName = "LName2",
                Email = "testuse...@mail.com"
            };
            _objWithId = new MockEntityObjectWithSetId
            {
                FirstName = "FName1",
                LastName = "LName1",
                Email = "testus...@mail.com"
            };
            _sameObjWithId = new MockEntityObjectWithSetId
            {
                FirstName = "FName1",
                LastName = "LName1",
                Email = "testus...@mail.com"
            };
            _diffObjWithId = new MockEntityObjectWithSetId
            {
                FirstName = "FName2",
                LastName = "LName2",
                Email = "testuse...@mail.com"
            };
        }

        [Fact]
        public void CanHaveEntityWithoutDomainSignatureProperties()
        {
            var invalidEntity =
                new ObjectWithNoDomainSignatureProperties();

            invalidEntity.GetSignatureProperties();
        }

        [Fact]
        public void TransientEntityWithoutDomainSignatureShouldReturnConsistentHashcode()
        {
            var sut = new ObjectWithNoDomainSignatureProperties();

            Assert.Equal(sut.GetHashCode(), sut.GetHashCode());
        }

        [Fact]
        public void TwoTransientEntitiesWithoutSignaturePropertiesGenerateDifferentHashcodes()
        {
            var sut1 = new ObjectWithNoDomainSignatureProperties();
            var sut2 = new ObjectWithNoDomainSignatureProperties();

            Assert.NotEqual(sut1.GetHashCode(), sut2.GetHashCode());
        }

        [Fact]
        public void
            EntityWithNoSignaturePropertiesPreservesHashcodeWhenTransitioningFromTransientToPersistent()
        {
            var sut = new ObjectWithNoDomainSignatureProperties();

            Assert.True(sut.IsTransient());

            int hashcodeWhenTransient = sut.GetHashCode();

            sut.SetIdTo(1);

            Assert.False(sut.IsTransient());
            Assert.Equal(sut.GetHashCode(), hashcodeWhenTransient);
        }

        [Fact]
        public void TwoPersistentEntitiesWithNoSignaturePropertiesAndDifferentIdsGenerateDifferentHashcodes()
        {
            IDomainObjectWithTypedId<int> sut1 = new ObjectWithNoDomainSignatureProperties().SetIdTo(1);
            IDomainObjectWithTypedId<int> sut2 = new ObjectWithNoDomainSignatureProperties().SetIdTo(2);

            Assert.NotEqual(sut1.GetHashCode(), sut2.GetHashCode());
        }

        [Fact]
        public void TwoPersistentEntitiesWithNoSignaturePropertiesAndEqualIdsGenerateEqualHashcodes()
        {
            IDomainObjectWithTypedId<int> sut1 = new ObjectWithNoDomainSignatureProperties().SetIdTo(1);
            IDomainObjectWithTypedId<int> sut2 = new ObjectWithNoDomainSignatureProperties().SetIdTo(1);

            Assert.Equal(sut1.GetHashCode(), sut2.GetHashCode());
        }

        [Fact]
        public void TransientEntityWithDomainSignatureShouldReturnConsistentHashcode()
        {
            var sut = new ObjectWithOneDomainSignatureProperty {Age = 1};

            Assert.Equal(sut.GetHashCode(), sut.GetHashCode());
        }

        [Fact]
        public void TwoTransientEntitiesWithDifferentValuesOfDomainSignatureGenerateDifferentHashcodes()
        {
            var sut1 = new ObjectWithOneDomainSignatureProperty {Age = 1};
            var sut2 = new ObjectWithOneDomainSignatureProperty {Age = 2};

            Assert.NotEqual(sut1.GetHashCode(), sut2.GetHashCode());
        }

        [Fact]
        public void TwoTransientEntititesWithEqualValuesOfDomainSignatureGenerateEqualHashcodes()
        {
            var sut1 = new ObjectWithOneDomainSignatureProperty {Age = 1};
            var sut2 = new ObjectWithOneDomainSignatureProperty {Age = 1};

            Assert.Equal(sut1.GetHashCode(), sut2.GetHashCode());
        }

        [Fact]
        public void
            TransientEntityWithDomainSignaturePreservesHashcodeTemporarilyWhenItsDomainSignatureChanges()
        {
            var sut = new ObjectWithOneDomainSignatureProperty {Age = 1};

            int initialHashcode = sut.GetHashCode();

            sut.Age = 2;

            Assert.Equal(sut.GetHashCode(), initialHashcode);
        }

        [Fact]
        public void EntityWithDomainSignaturePreservesHashcodeWhenTransitioningFromTransientToPersistent()
        {
            var sut = new ObjectWithOneDomainSignatureProperty {Age = 1};

            Assert.True(sut.IsTransient());

            int hashcodeWhenTransient = sut.GetHashCode();

            sut.SetIdTo(1);

            Assert.False(sut.IsTransient());
            Assert.Equal(sut.GetHashCode(), hashcodeWhenTransient);
        }

        [Fact]
        public void TwoPersistentEntitiesWithEqualDomainSignatureAndDifferentIdsGenerateDifferentHashcodes()
        {
            IDomainObjectWithTypedId<int> sut1 = new ObjectWithOneDomainSignatureProperty {Age = 1}.SetIdTo(1);
            IDomainObjectWithTypedId<int> sut2 = new ObjectWithOneDomainSignatureProperty {Age = 1}.SetIdTo(2);

            Assert.NotEqual(sut1.GetHashCode(), sut2.GetHashCode());
        }

        [Fact]
        public void TwoPersistentEntitiesWithDifferentDomainSignatureAndEqualIdsGenerateEqualHashcodes()
        {
            IDomainObjectWithTypedId<int> sut1 = new ObjectWithOneDomainSignatureProperty {Age = 1}.SetIdTo(1);
            IDomainObjectWithTypedId<int> sut2 = new ObjectWithOneDomainSignatureProperty {Age = 2}.SetIdTo(1);

            Assert.Equal(sut1.GetHashCode(), sut2.GetHashCode());
        }

        [Fact]
        public void KeepsConsistentHashThroughLifetimeOfTransientObject()
        {
            var object1 = new ObjectWithOneDomainSignatureProperty();
            int initialHash = object1.GetHashCode();

            object1.Age = 13;
            object1.Name = "Foo";

            Assert.Equal(initialHash, object1.GetHashCode());

            object1.Age = 14;
            Assert.Equal(initialHash, object1.GetHashCode());
        }

        [Fact]
        public void KeepsConsistentHashThroughLifetimeOfPersistentObject()
        {
            var object1 = new ObjectWithOneDomainSignatureProperty();
            EntityIdSetter.SetIdOf(object1, 1);
            int initialHash = object1.GetHashCode();

            object1.Age = 13;
            object1.Name = "Foo";

            Assert.Equal(initialHash, object1.GetHashCode());

            object1.Age = 14;
            Assert.Equal(initialHash, object1.GetHashCode());
        }

        [Fact]
        public void CanCompareDomainObjectsWithOnlySomePropertiesBeingPartOfDomainSignature()
        {
            var object1 = new ObjectWithOneDomainSignatureProperty();
            var object2 = new ObjectWithOneDomainSignatureProperty();
            Assert.Equal(object1, object2);

            object1.Age = 13;
            object2.Age = 13;
            // Name property isn't included in comparison
            object1.Name = "Foo";
            object2.Name = "Bar";
            Assert.Equal(object1, object2);

            object1.Age = 14;
            Assert.NotEqual(object1, object2);
        }

        [Fact]
        public void CanCompareDomainObjectsWithAllPropertiesBeingPartOfDomainSignature()
        {
            var object1 = new ObjectWithAllDomainSignatureProperty();
            var object2 = new ObjectWithAllDomainSignatureProperty();
            Assert.Equal(object1, object2);

            object1.Age = 13;
            object2.Age = 13;
            object1.Name = "Foo";
            object2.Name = "Foo";
            Assert.Equal(object1, object2);

            object1.Name = "Bar";
            Assert.NotEqual(object1, object2);

            object1.Name = null;
            Assert.NotEqual(object1, object2);

            object2.Name = null;
            Assert.Equal(object1, object2);
        }

        [Fact]
        public void CanCompareInheritedDomainObjects()
        {
            var object1 =
                new InheritedObjectWithExtraDomainSignatureProperty();
            var object2 =
                new InheritedObjectWithExtraDomainSignatureProperty();
            Assert.Equal(object1, object2);

            object1.Age = 13;
            object1.IsLiving = true;
            object2.Age = 13;
            object2.IsLiving = true;
            // Address property isn't included in comparison
            object1.Address = "123 Oak Ln.";
            object2.Address = "Nightmare on Elm St.";
            Assert.Equal(object1, object2);

            object1.IsLiving = false;
            Assert.NotEqual(object1, object2);
        }

        [Fact]
        public void WontGetConfusedWithOutsideCases()
        {
            var object1 =
                new ObjectWithIdenticalTypedProperties();
            var object2 =
                new ObjectWithIdenticalTypedProperties();

            object1.Address = "Henry";
            object1.Name = "123 Lane St.";
            object2.Address = "123 Lane St.";
            object2.Name = "Henry";
            Assert.NotEqual(object1, object2);

            object1.Address = "Henry";
            object1.Name = null;
            object2.Address = "Henri";
            object2.Name = null;
            Assert.NotEqual(object1, object2);

            object1.Address = null;
            object1.Name = "Supercalifragilisticexpialidocious";
            object2.Address = null;
            object2.Name = "Supercalifragilisticexpialidocious";
            Assert.Equal(object1, object2);

            object1.Name = "Supercalifragilisticexpialidocious";
            object2.Name = "Supercalifragilisticexpialidociouz";
            Assert.NotEqual(object1, object2);
        }

        [Fact]
        public void CanCompareObjectsWithComplexProperties()
        {
            var object1 = new ObjectWithComplexProperties();
            var object2 = new ObjectWithComplexProperties();

            Assert.Equal(object1, object2);

            object1.Address = new AddressBeingDomainSignatureComparble
                                  {
                                      Address1 = "123 Smith Ln.",
                                      Address2 = "Suite 201",
                                      ZipCode = 12345
                                  };
            Assert.NotEqual(object1, object2);

            // Set the address of the 2nd to be different to the address of the first
            object2.Address = new AddressBeingDomainSignatureComparble
                                  {
                                      Address1 = "123 Smith Ln.",
                                      // Address2 isn't marked as being part of the domain signature; 
                                      // therefore, it WON'T be used in the equality comparison
                                      Address2 = "Suite 402",
                                      ZipCode = 98765
                                  };
            Assert.NotEqual(object1, object2);

            // Set the address of the 2nd to be the same as the first
            object2.Address.ZipCode = 12345;
            Assert.Equal(object1, object2);

            object1.Phone = new PhoneBeingNotDomainObject
                                {
                                    PhoneNumber = "(555) 555-5555"
                                };
            Assert.NotEqual(object1, object2);

            // IMPORTANT: Note that even though the phone number below has the same value as the 
            // phone number on object1, they're not domain signature comparable; therefore, the
            // "out of the box" equality will be used which shows them as being different objects.
            object2.Phone = new PhoneBeingNotDomainObject
                                {
                                    PhoneNumber = "(555) 555-5555"
                                };
            Assert.NotEqual(object1, object2);

            // Observe as we replace the object1.Phone with an object that isn't domain-signature
            // comparable, but DOES have an overridden Equals which will return true if the phone
            // number properties are equal.
            object1.Phone = new PhoneBeingNotDomainObjectButWithOverriddenEquals
                                {
                                    PhoneNumber = "(555) 555-5555"
                                };
            Assert.Equal(object1, object2);
        }

        #region Nested type: InheritedObjectWithExtraDomainSignatureProperty

        private class InheritedObjectWithExtraDomainSignatureProperty : ObjectWithOneDomainSignatureProperty
        {
            public string Address { get; set; }

            [DomainSignature]
            public bool IsLiving { get; set; }
        }

        #endregion

        #region Nested type: ObjectWithAllDomainSignatureProperty

        private class ObjectWithAllDomainSignatureProperty : DomainObject
        {
            [DomainSignature]
            public string Name { get; set; }

            [DomainSignature]
            public int Age { get; set; }
        }

        #endregion

        #region Nested type: ObjectWithIdenticalTypedProperties

        private class ObjectWithIdenticalTypedProperties : DomainObject
        {
            [DomainSignature]
            public string Name { get; set; }

            [DomainSignature]
            public string Address { get; set; }
        }

        #endregion

        #region ObjectWithComplexProperties

        #region Nested type: AddressBeingDomainSignatureComparble

        private class AddressBeingDomainSignatureComparble : DomainObject
        {
            [DomainSignature]
            public string Address1 { get; set; }

            public string Address2 { get; set; }

            [DomainSignature]
            public int ZipCode { get; set; }
        }

        #endregion

        #region Nested type: ObjectWithComplexProperties

        private class ObjectWithComplexProperties : DomainObject
        {
            [DomainSignature]
            public string Name { get; set; }

            [DomainSignature]
            public AddressBeingDomainSignatureComparble Address { get; set; }

            [DomainSignature]
            public PhoneBeingNotDomainObject Phone { get; set; }
        }

        #endregion

        #region Nested type: PhoneBeingNotDomainObject

        private class PhoneBeingNotDomainObject
        {
            public string PhoneNumber { get; set; }
            public string Extension { get; set; }
        }

        #endregion

        #region Nested type: PhoneBeingNotDomainObjectButWithOverriddenEquals

        private class PhoneBeingNotDomainObjectButWithOverriddenEquals : PhoneBeingNotDomainObject
        {
            public override bool Equals(object obj)
            {
                var compareTo =
                    obj as PhoneBeingNotDomainObject;

                return (compareTo != null &&
                        PhoneNumber.Equals(compareTo.PhoneNumber));
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        #endregion

        #endregion

        #region Carry-Over tests from when Entity was split from an object called PersistentObject

        [Fact]
        public void CanCompareEntitys()
        {
            var object1 = new ObjectWithIntId {Name = "Acme"};
            var object2 = new ObjectWithIntId {Name = "Anvil"};

            Assert.NotEqual(object1, null);
            Assert.NotEqual(object1, object2);

            EntityIdSetter.SetIdOf(object1, 10);
            EntityIdSetter.SetIdOf(object2, 10);

            // Even though the "business value signatures" are different, the persistent Ids 
            // were the same.  Call me crazy, but I put that much trust into persisted Ids.
            Assert.Equal(object1, object2);
            Assert.Equal(object1.GetHashCode(), object2.GetHashCode());

            var object3 = new ObjectWithIntId {Name = "Acme"};

            // Since object1 has an Id but object3 doesn't, they won't be equal
            // even though their signatures are the same
            Assert.NotEqual(object1, object3);

            var object4 = new ObjectWithIntId {Name = "Acme"};

            // object3 and object4 are both transient and share the same signature
            Assert.Equal(object3, object4);
        }

        [Fact]
        public void CanCompareEntitiesWithAssignedIds()
        {
            var object1 = new ObjectWithAssignedId {Name = "Acme"};
            var object2 = new ObjectWithAssignedId {Name = "Anvil"};

            Assert.NotEqual(object1, null);
            Assert.NotEqual(object1, object2);

            object1.SetAssignedIdTo("AAAAA");
            object2.SetAssignedIdTo("AAAAA");

            // Even though the "business value signatures" are different, the persistent Ids 
            // were the same.  Call me crazy, but I put that much trust into persisted Ids.
            Assert.Equal(object1, object2);

            var object3 = new ObjectWithAssignedId {Name = "Acme"};

            // Since object1 has an Id but object3 doesn't, they won't be equal
            // even though their signatures are the same
            Assert.NotEqual(object1, object3);

            var object4 = new ObjectWithAssignedId {Name = "Acme"};

            // object3 and object4 are both transient and share the same signature
            Assert.Equal(object3, object4);
        }

        [Fact]
        public void CannotEquateObjectsWithSameIdButDifferentTypes()
        {
            var object1Type = new Object1();
            var object2Type = new Object2();

            EntityIdSetter.SetIdOf(object1Type, 1);
            EntityIdSetter.SetIdOf(object2Type, 1);

            Assert.NotEqual((DomainObject)object1Type, (DomainObject)object2Type);
        }

        #region Nested type: Object1

        private class Object1 : DomainObject
        {
        }

        #endregion

        #region Nested type: Object2

        private class Object2 : DomainObject
        {
        }

        #endregion

        #region Comprehensive unit tests provided by Brian Nicoloff

        private MockEntityObjectWithDefaultId _diffObj;
        private MockEntityObjectWithSetId _diffObjWithId;
        private MockEntityObjectWithDefaultId _obj;
        private MockEntityObjectWithSetId _objWithId;
        private MockEntityObjectWithDefaultId _sameObj;
        private MockEntityObjectWithSetId _sameObjWithId;

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    Teardown();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~DomainObjectTests() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        public void Teardown()
        {
            _obj = null;
            _sameObj = null;
            _diffObj = null;
        }
        #endregion

        [Fact]
        public void DoesDefaultEntityEqualsOverrideWorkWhenNoIdIsAssigned()
        {
            Assert.True(_obj.Equals(_sameObj));
            Assert.True(!_obj.Equals(_diffObj));
            Assert.True(!_obj.Equals(new MockEntityObjectWithDefaultId()));
        }

        [Fact]
        public void DoEqualDefaultEntitiesWithNoIdsGenerateSameHashCodes()
        {
            Assert.True(_obj.GetHashCode().Equals(_sameObj.GetHashCode()));
        }

        [Fact]
        public void DoEqualDefaultEntitiesWithMatchingIdsGenerateDifferentHashCodes()
        {
            Assert.True(!_obj.GetHashCode().Equals(_diffObj.GetHashCode()));
        }

        [Fact]
        public void DoDefaultEntityEqualsOverrideWorkWhenIdIsAssigned()
        {
            _obj.SetAssignedIdTo(1);
            _diffObj.SetAssignedIdTo(1);
            Assert.True(_obj.Equals(_diffObj));
        }

        [Fact]
        public void DoesEntityEqualsOverrideWorkWhenNoIdIsAssigned()
        {
            Assert.True(_objWithId.Equals(_sameObjWithId));
            Assert.True(!_objWithId.Equals(_diffObjWithId));
            Assert.True(!_objWithId.Equals(new MockEntityObjectWithSetId()));
        }

        [Fact]
        public void DoEqualEntitiesWithNoIdsGenerateSameHashCodes()
        {
            Assert.True(_objWithId.GetHashCode().Equals(_sameObjWithId.GetHashCode()));
        }

        [Fact]
        public void DoEqualEntitiesWithMatchingIdsGenerateDifferentHashCodes()
        {
            Assert.True(!_objWithId.GetHashCode().Equals(_diffObjWithId.GetHashCode()));
        }

        [Fact]
        public void DoEntityEqualsOverrideWorkWhenIdIsAssigned()
        {
            _objWithId.SetAssignedIdTo("1");
            _diffObjWithId.SetAssignedIdTo("1");
            Assert.True(_objWithId.Equals(_diffObjWithId));
        }

        #region Nested type: MockEntityObjectBase

        private class MockEntityObjectBase :
            MockEntityObjectBase<int>
        {
        }

        public class MockEntityObjectBase<T> :
            DomainObjectWithTypedId<T>
        {
            [DomainSignature]
            public string FirstName { get; set; }

            [DomainSignature]
            public string LastName { get; set; }

            public string Email { get; set; }
        }

        #endregion

        #region Nested type: MockEntityObjectWithDefaultId

        private class MockEntityObjectWithDefaultId :
            MockEntityObjectBase, IHasAssignedId<int>
        {
            #region IHasAssignedId<int> Members

            public void SetAssignedIdTo(int assignedId)
            {
                Id = assignedId;
            }

            #endregion
        }

        #endregion

        #region Nested type: MockEntityObjectWithSetId

        private class MockEntityObjectWithSetId :
            MockEntityObjectBase<string>, IHasAssignedId<string>
        {
            #region IHasAssignedId<string> Members

            public void SetAssignedIdTo(string assignedId)
            {
                Id = assignedId;
            }

            #endregion
        }

        #endregion

        #endregion

        #region Nested type: ObjectWithAssignedId

        private class ObjectWithAssignedId : DomainObjectWithTypedId<string>, IHasAssignedId<string>
        {
            [DomainSignature]
            public string Name { get; set; }

            #region IHasAssignedId<string> Members

            public void SetAssignedIdTo(string assignedId)
            {
                Id = assignedId;
            }

            #endregion
        }

        #endregion

        #region Nested type: ObjectWithIntId

        private class ObjectWithIntId : DomainObject
        {
            [DomainSignature]
            public string Name { get; set; }
        }

        #endregion

        #endregion

        #region Nested type: ObjectWithNoDomainSignatureProperties

        /// <summary>
        /// This is a nonsense object; i.e., it doesn't make sense to have 
        /// an entity without a domain signature.
        /// </summary>
        private class ObjectWithNoDomainSignatureProperties : DomainObject
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        #endregion

        #region Nested type: ObjectWithOneDomainSignatureProperty

        public class ObjectWithOneDomainSignatureProperty : DomainObject
        {
            public string Name { get; set; }

            [DomainSignature]
            public int Age { get; set; }
        }
        #endregion
    }
}