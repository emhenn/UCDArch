using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Castle.Windsor;
using FluentNHibernate.Cfg;
using NHibernate.Cfg;
using NHibernate.Stat;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Data.NHibernate.Fluent;

namespace UCDArch.Testing
{
    public abstract class FluentRepositoryTestBase<TMappingClass> : FluentRepositoryTestBase<TMappingClass, PrimaryKeyConvention> {}
    
    public abstract class FluentRepositoryTestBase<TMappingClass, TConventionClass> : IDisposable
    {
        public List<Guid> UserIds { get; set; }
        public IRepository Repository { get; set; }

        /// <summary>
        /// Get the statistics for the associated session factory
        /// </summary>
        /// <remarks>
        /// Must have <property name="generate_statistics">true</property>
        /// </remarks>
        /// <see cref="http://nhforge.org/blogs/nhibernate/archive/2008/10/26/exploring-nhibernate-statistics-part-1-simple-data-fetching.aspx"/>
        public IStatistics FactoryStatistics
        {
            get
            {
                return NHibernateSessionManager.Instance.FactoryStatistics;
            }
        }

        private static readonly Configuration FluentConfiguration = BuildConfiguration();
            
        private static Configuration BuildConfiguration()
        {
            var mappingAssembly = typeof (TMappingClass).Assembly;
            var conventionAssembly = typeof (TConventionClass).Assembly;

            NHibernateSessionConfiguration.Mappings.UseFluentMappings(mappingAssembly, conventionAssembly);

            var config = Fluently.Configure().Mappings(
                x =>
                x.FluentMappings.AddFromAssembly(mappingAssembly).Conventions.AddAssembly(conventionAssembly)).
                BuildConfiguration();

            return config;
        }

        protected FluentRepositoryTestBase()
        {
            UserIds = new List<Guid>();
            Repository = new Repository();
        }

        /// <summary>
        /// Creates the DB.  Call from parameterless constructor of subclasses - TestInitialize not supported in XUnit
        /// </summary>
        public void CreateDB()
        {
            new NHibernate.Tool.hbm2ddl.SchemaExport(FluentConfiguration).Execute(false, true, false,
                                                                     NHibernateSessionManager.
                                                                         Instance.GetSession().Connection, null);
            InitServiceLocator();

            LoadData();
        }

        /// <summary>
        /// Used to cleanup something that resharper 6 currently leaves behind.
        /// </summary>
        public void TearDown()
        {
            NHibernateSessionManager.Instance.CloseSession();
        }

        protected virtual void LoadData()
        {

        }

        protected virtual void InitServiceLocator()
        {
            var container = ServiceLocatorInitializer.Init();

            RegisterAdditionalServices(container);
        }

        /// <summary>
        /// Instead of overriding InitServiceLocator, you can jump in here to register additional services for testing
        /// </summary>
        protected virtual void RegisterAdditionalServices(IWindsorContainer container)
        {
            
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    TearDown();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~FluentRepositoryTestBase() {
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
        #endregion
    }
}
