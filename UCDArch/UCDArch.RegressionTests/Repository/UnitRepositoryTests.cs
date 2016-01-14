using System.Collections.Generic;
using UCDArch.Core;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.RegressionTests.SampleMappings;
using UCDArch.Testing;
using System;
using UCDArch.Testing.Extensions;
using Xunit;

namespace UCDArch.RegressionTests.Repository
{
    public class UnitRepositoryTests : FluentRepositoryTestBase<UnitMap>
    {
        private readonly IRepository<Unit> _repository = new Repository<Unit>();

        protected override void LoadData()
        {
            CreateUnits();
        }

        #region Repository Tests
        /// <summary>
        /// Determines whether this instance [can save valid unit using generic repository].
        /// </summary>
        [Fact]
        public void CanSaveValidUnitUsingGenericRepository()
        {
            var unit = CreateValidUnit();

            Assert.Equal(true, unit.IsTransient());

            _repository.EnsurePersistent(unit);

            Assert.Equal(false, unit.IsTransient());
        }

        /// <summary>
        /// Determines whether this instance [can save valid unit using non generic repository].
        /// </summary>
        [Fact]
        public void CanSaveValidUnitUsingNonGenericRepository()
        {
            var unit = CreateValidUnit();

            Assert.Equal(true, unit.IsTransient());

            Repository.OfType<Unit>().EnsurePersistent(unit);

            Assert.Equal(false, unit.IsTransient());
        }
        #endregion Repository Tests

        #region Validation Tests
        #region FullName Tests
        /// <summary>
        /// Units does not save with null full name.
        /// </summary>
        [Fact]
        public void UnitDoesNotSaveWithNullFullName()
        {
            Unit unit = null;
            try
            {
                unit = CreateValidUnit();
                unit.FullName = null;
                _repository.EnsurePersistent(unit);
                Assert.True(false, "Expected Application Exception");
            }
            catch (ApplicationException)
            {
                Assert.NotNull(unit);
                unit.ValidationResults().AsMessageList().AssertErrorsAre("The FullName field is required.");
            }
        }
        /// <summary>
        /// Units does not save with empty full name.
        /// </summary>
        [Fact]
        public void UnitDoesNotSaveWithEmptyFullName()
        {
            Unit unit = null;
            try
            {
                unit = CreateValidUnit();
                unit.FullName = "";
                _repository.EnsurePersistent(unit);
                Assert.True(false, "Expected Application Exception");
            }
            catch (ApplicationException)
            {
                Assert.NotNull(unit);
                unit.ValidationResults().AsMessageList().AssertErrorsAre("The FullName field is required.");
            }
        }
        /// <summary>
        /// Units does not save with Spaces Only full name.
        /// </summary>
        [Fact]
        public void UnitDoesNotSaveWithSpacesOnlyFullName()
        {
            Unit unit = null;
            try
            {
                unit = CreateValidUnit();
                unit.FullName = " ";
                _repository.EnsurePersistent(unit);
                Assert.True(false, "Expected Application Exception");
            }
            catch (ApplicationException)
            {
                Assert.NotNull(unit);
                unit.ValidationResults().AsMessageList().AssertErrorsAre("The FullName field is required.");
            }
        }
        /// <summary>
        /// Units does not save with Spaces Only full name.
        /// </summary>
        [Fact]
        public void UnitDoesNotSaveWithTooLongFullName()
        {
            Unit unit = null;
            try
            {
                unit = CreateValidUnit();
                unit.FullName = "123456789 123456789 123456789 123456789 12345678901"; //51 characters long, max 50 allowed
                _repository.EnsurePersistent(unit);
                Assert.True(false, "Expected Application Exception");
            }
            catch (ApplicationException)
            {
                Assert.NotNull(unit);
                unit.ValidationResults().AsMessageList().AssertErrorsAre("The field FullName must be a string with a maximum length of 50.");
            }
        }
        #endregion FullName Tests

        #region ShortName Tests TODO
        #endregion ShortName Tests

        #region FISCode Tests TODO
        #endregion FISCode Tests

        #region PPSCode Tests TODO
        #endregion PPSCode Tests

        #endregion Validation Tests

        #region CRUD Tests
        /// <summary>
        /// Determines whether this instance [can get all units].
        /// </summary>
        [Fact]
        public void CanGetAllUnits()
        {
            IList<Unit> allUnits = _repository.GetAll();
            Assert.Equal(10, allUnits.Count);
        }

        /// <summary>
        /// Determines whether this instance [can save new unit].
        /// </summary>
        [Fact]
        public void CanSaveNewUnit()
        {
            var unit = CreateValidUnit();
            unit.FullName = "Full1234Name"; //Just a random name. Used lower in code to check that we get it.

            Assert.True(unit.IsTransient());

            NHibernateSessionManager.Instance.BeginTransaction();
            _repository.EnsurePersistent(unit);
            NHibernateSessionManager.Instance.CommitTransaction();

            Assert.False(unit.IsTransient()); //Make sure the unit is saved

            NHibernateSessionManager.Instance.GetSession().Evict(unit);//get the unit out of the local cache

            //Now get the user back out
            var unitId = unit.Id;

            var retrievedUser = _repository.GetNullableById(unitId);

            Assert.NotNull(retrievedUser);
            Assert.Equal("Full1234Name", retrievedUser.FullName); //Make sure it is the correct user
            Assert.Equal(11, _repository.GetAll().Count);
        }
        /// <summary>
        /// Determines whether this instance [can modify unit].
        /// </summary>
        [Fact]
        public void CanModifyUnit()
        {
            Unit firstUnit = _repository.GetNullableById(1); //Just get the first user

            Assert.Equal("FullName1", firstUnit.FullName); //First user is scott

            NHibernateSessionManager.Instance.BeginTransaction();
            firstUnit.FullName = "NewFullName";
            _repository.EnsurePersistent(firstUnit);
            NHibernateSessionManager.Instance.CommitTransaction();

            NHibernateSessionManager.Instance.GetSession().Evict(firstUnit); //Evict the user from the cache so we can retrieve it

            firstUnit = _repository.GetNullableById(1); //Get the user back out

            Assert.Equal("NewFullName", firstUnit.FullName); //the name should now be tiny
        }

        /// <summary>
        /// Determines whether this instance [can remove unit].
        /// </summary>
        [Fact]
        public void CanRemoveUnit()
        {
            Unit firstUnit = _repository.GetNullableById(1); //Just get the first user

            Assert.Equal("FullName1", firstUnit.FullName); //First user is scott

            NHibernateSessionManager.Instance.BeginTransaction();
            _repository.Remove(firstUnit);
            NHibernateSessionManager.Instance.CommitTransaction();

            Assert.Equal(9, _repository.GetAll().Count); //There should be 9 users now

            Unit shouldNotExistUnit = _repository.GetNullableById(1);

            Assert.Null(shouldNotExistUnit);
        }
        #endregion CRUD Tests

        #region Helper methods
        /// <summary>
        /// Creates the valid unit.
        /// </summary>
        /// <returns></returns>
        private static Unit CreateValidUnit()
        {
            return new Unit
            {
                FullName = "FullName",
                ShortName = "ShortName",
                FISCode = "1234",
                PPSCode = "123456"
            };
        }

        /// <summary>
        /// Creates 10 units.
        /// </summary>
        private void CreateUnits()
        {
            NHibernateSessionManager.Instance.BeginTransaction();
            for (int i = 1; i <= 10; i++)
            {
                var unit = CreateValidUnit();
                unit.FullName += i.ToString();
                unit.ShortName += i.ToString();

                _repository.EnsurePersistent(unit); //Save
            }
            NHibernateSessionManager.Instance.CommitTransaction();
        }

        #endregion Helper methods
    }
}
