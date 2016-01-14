using System.Collections.Generic;
using UCDArch.Testing;
using UCDArch.RegressionTests.SampleMappings;
using UCDArch.Data.NHibernate;
using Xunit;

namespace UCDArch.RegressionTests.Repository
{
    /// <summary>
    /// Tests to check object flushing/dirty checking behavior
    /// </summary>
    public class ObjectFlushTests : FluentRepositoryTestBase<UnitMap>
    {
        protected override void LoadData()
        {
            var unitRepository = Repository.OfType<Unit>();

            unitRepository.DbContext.BeginTransaction();

            //Let's create a unit
            var unit = new Unit
            {
                FullName = "FullName",
                ShortName = "ShortName",
                FISCode = "1234",
                PPSCode = "123456"
            };

            unitRepository.EnsurePersistent(unit);

            unitRepository.DbContext.CommitTransaction();

            NHibernateSessionManager.Instance.GetSession().Clear(); 
            //Have to clear the session to get objects out of cache

            base.LoadData();
        }

        [Fact]
        public void ChangingLazyObjectDoesNotAutomaticallyFlushChanges()
        {
            var stats = NHibernateSessionManager.Instance.FactoryStatistics;
            stats.Clear();

            var unitRepository = Repository.OfType<Unit>();

            unitRepository.DbContext.BeginTransaction();

            var existingUnit = unitRepository.GetById(1); //Lazy load

            Assert.False(existingUnit.IsTransient(), "The unit with id 1 should already exist");

            Assert.Equal("FullName", existingUnit.FullName);

            existingUnit.FullName = "ChangedName";

            unitRepository.DbContext.CommitTransaction();

            Assert.Equal(0, stats.EntityUpdateCount);
        }

        [Fact]
        public void ChangingObjectDoesNotAutomaticallyFlushChanges()
        {
            var stats = NHibernateSessionManager.Instance.FactoryStatistics;
            stats.Clear();

            var unitRepository = Repository.OfType<Unit>();

            unitRepository.DbContext.BeginTransaction();

            var existingUnit = unitRepository.GetNullableById(1);

            Assert.False(existingUnit.IsTransient(), "The unit with id 1 should already exist");

            Assert.Equal("FullName", existingUnit.FullName);

            existingUnit.FullName = "ChangedName";

            unitRepository.DbContext.CommitTransaction();
            
            Assert.Equal(0, stats.EntityUpdateCount);
        }


        [Fact]
        public void EnsurePersistSavesChangesWithLazyObject()
        {
            var stats = NHibernateSessionManager.Instance.FactoryStatistics;
            stats.Clear();

            var unitRepository = Repository.OfType<Unit>();

            unitRepository.DbContext.BeginTransaction();

            var existingUnit = unitRepository.GetById(1); //Lazy load

            Assert.False(existingUnit.IsTransient(), "The unit with id 1 should already exist");

            Assert.Equal("FullName", existingUnit.FullName);

            existingUnit.FullName = "ChangedName";

            unitRepository.EnsurePersistent(existingUnit); //Explicit Save
            
            unitRepository.DbContext.CommitTransaction();

            Assert.Equal(1, stats.EntityUpdateCount);
        }

        [Fact]
        public void EnsurePersistSavesChanges()
        {
            var stats = NHibernateSessionManager.Instance.FactoryStatistics;
            stats.Clear();

            var unitRepository = Repository.OfType<Unit>();

            unitRepository.DbContext.BeginTransaction();

            var existingUnit = unitRepository.GetNullableById(1);

            Assert.False(existingUnit.IsTransient(), "The unit with id 1 should already exist");

            Assert.Equal("FullName", existingUnit.FullName);

            existingUnit.FullName = "ChangedName";

            unitRepository.EnsurePersistent(existingUnit); //Explicit Save
            
            unitRepository.DbContext.CommitTransaction();

            Assert.Equal(1, stats.EntityUpdateCount);
        }
    }
}