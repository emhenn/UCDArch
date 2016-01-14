using System;
using System.IO;
using System.Web.Mvc;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;
using Xunit;

namespace UCDArch.Tests.UCDArch.Web.Controllers
{
    public class ControllerTransactionTests : IDisposable
    {
        private IDbContext _dbContext;
        private TestControllerBuilder _builder;
        private SampleController _controller;
        private static int _beginTransactionCount;
        private static int _commitTransactionCount;
        private static int _closeSessionCount;

        public ControllerTransactionTests()
        {
            _builder = new TestControllerBuilder();

            _controller = _builder.CreateController<SampleController>();

            //Required by .NET4.5+ to invoke actions
            System.Web.HttpContext.Current =
                new System.Web.HttpContext(new System.Web.HttpRequest("foo", "http://tempuri.org/foo", ""),
                                           new System.Web.HttpResponse(new StringWriter()));
            
            ServiceLocatorInitializer.InitWithFakeDBContext();

            _dbContext = SmartServiceLocator<IDbContext>.GetService();

            _dbContext.Stub(x => x.IsActive).Repeat.Any().Return(true);
            _dbContext.Stub(x => x.BeginTransaction()).Repeat.Any().WhenCalled(x => _beginTransactionCount++);
            _dbContext.Stub(x => x.CommitTransaction()).Repeat.Any().WhenCalled(x => _commitTransactionCount++);
            _dbContext.Stub(x => x.CloseSession()).Repeat.Any().WhenCalled(x=> _closeSessionCount++);
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
        // ~ControllerTransactionTests() {
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

        public void TearDown()
        {
            _beginTransactionCount = 0;
            _commitTransactionCount = 0;
            _closeSessionCount = 0;
        }
        #endregion

        /// <summary>
        /// Controller closes session when calling method without manual transaction attribute.
        /// </summary>
        [Fact]
        public void ControllerCloseSessionWhenCallingMethodWithoutManualTransactionAttribute()
        {
            _controller.ActionInvoker.InvokeAction(_controller.ControllerContext,
                                                   "MethodWithoutManualTransactionAttribute");

            Assert.Equal(1, _closeSessionCount);
            //_dbContext.AssertWasCalled(x => x.CloseSession(), x => x.Repeat.Once());
        }

        /// <summary>
        /// Controller DOESN'T close session when calling method with manual transaction attribute.
        /// </summary>
        [Fact]
        public void ControllerDoesNotCloseSessionWhenCallingMethodWithManualTransactionAttribute()
        {
            _controller.ActionInvoker.InvokeAction(_controller.ControllerContext,
                                                   "MethodWithManualTransactionAttribute");

            Assert.Equal(0, _closeSessionCount);
            //_dbContext.AssertWasNotCalled(a => a.CloseSession());
        }

        /// <summary>
        /// Controller begins the transaction when calling method without manual transaction attribute.
        /// </summary>
        [Fact]
        public void ControllerBeginsTransactionWhenCallingMethodWithoutManualTransactionAttribute()
        {
            _controller.ActionInvoker.InvokeAction(_controller.ControllerContext,
                                                   "MethodWithoutManualTransactionAttribute");

            Assert.Equal(1, _beginTransactionCount);
            //_dbContext.AssertWasCalled(a=>a.BeginTransaction(), a=>a.Repeat.Once());
        }

        /// <summary>
        /// Controller commits the transaction when calling method without manual transaction attribute.
        /// </summary>
        [Fact]
        public void ControllerCommitsTransactionWhenCallingMethodWithoutManualTransactionAttribute()
        {
            //Assume the transaction has been opened correctly
            _dbContext.Stub(a => a.IsActive).Return(true);

            _controller.ActionInvoker.InvokeAction(_controller.ControllerContext,
                                                   "MethodWithoutManualTransactionAttribute");

            Assert.Equal(1, _commitTransactionCount);
            //_dbContext.AssertWasCalled(a => a.CommitTransaction(), a => a.Repeat.Once());
        }

        /// <summary>
        /// Controller does not begin the transaction when calling method with manual transaction attribute.
        /// This is a case where the begin and commit/rollback would be handeled manually.
        /// </summary>
        [Fact]
        public void ControllerDoesNotBeginTransactionWhenCallingMethodWithManualTransactionAttribute()
        {
            _controller.ActionInvoker.InvokeAction(_controller.ControllerContext,
                                                   "MethodWithManualTransactionAttribute");

            Assert.Equal(0, _beginTransactionCount);
            //_dbContext.AssertWasNotCalled(a => a.BeginTransaction());
        }

        /// <summary>
        /// Controller does not commit the transaction when calling method with manual transaction attribute.
        /// This is a case where the begin and commit/rollback would be handeled manually.
        /// </summary>
        [Fact]
        public void ControllerDoesNotCommitTransactionWhenCallingMethodWithManualTransactionAttribute()
        {
            //Assume the transaction has been opened correctly
            _dbContext.Expect(a => a.IsActive).Return(true);

            _controller.ActionInvoker.InvokeAction(_controller.ControllerContext,
                                                   "MethodWithManualTransactionAttribute");

            Assert.Equal(0, _commitTransactionCount);
            //_dbContext.AssertWasNotCalled(a=>a.CommitTransaction());
        }

        /// <summary>
        /// Controller calls begin transaction only once when calling method with transaction attribute.
        /// This has the [Transaction] Attribute, but we still want the begin/commit to only happen once.
        /// </summary>
        [Fact]
        public void ControllerCallsBeginTransactionOnlyOnceWhenCallingMethodWithTransactionAttribute()
        {
            _controller.ActionInvoker.InvokeAction(_controller.ControllerContext,
                                                   "MethodWithTransactionAttribute");

            Assert.Equal(1, _beginTransactionCount);
            //_dbContext.AssertWasCalled(a => a.BeginTransaction(), a=>a.Repeat.Once());
        }

        /// <summary>
        /// Controller calls commit transaction only once when calling method with transaction attribute.
        /// This has the [Transaction] Attribute, but we still want the begin/commit to only happen once.
        /// </summary>
        [Fact]
        public void ControllerCallsCommitTransactionOnlyOnceWhenCallingMethodWithTransactionAttribute()
        {
            //Assume the transaction has been opened correctly
            _dbContext.Expect(a => a.IsActive).Return(true);

            _controller.ActionInvoker.InvokeAction(_controller.ControllerContext,
                                                   "MethodWithTransactionAttribute");

            Assert.Equal(1, _commitTransactionCount);
            //_dbContext.AssertWasCalled(a => a.CommitTransaction(), a=>a.Repeat.Once());
        }

        /// <summary>
        /// Controller calls begin transaction only once when calling method with manual transaction attribute and transaction scope.
        /// </summary>
        [Fact]
        public void ControllerCallsBeginTransactionOnlyOnceWhenCallingMethodWithManualTransactionAttributeAndTransactionScope()
        {
            _controller.ActionInvoker.InvokeAction(_controller.ControllerContext,
                                                   "MethodWithManualTransactionAttributeAndTransactionScope");

            Assert.Equal(1, _beginTransactionCount);
            //_dbContext.AssertWasCalled(a => a.BeginTransaction(), a => a.Repeat.Once());
        }

        /// <summary>
        /// Controller calls commit transaction only once when calling method with manual transaction attribute and transaction scope.
        /// </summary>
        [Fact]
        public void ControllerCallsCommitTransactionOnlyOnceWhenCallingMethodWithManualTransactionAttributeAndTransactionScope()
        {
            _controller.ActionInvoker.InvokeAction(_controller.ControllerContext,
                                                   "MethodWithManualTransactionAttributeAndTransactionScope");

            Assert.Equal(1, _commitTransactionCount);
            //_dbContext.AssertWasCalled(a => a.CommitTransaction(), a => a.Repeat.Once());
        }


        /// <summary>
        /// Controller calls begin transaction twice when calling method without manual transaction attribute and transaction scope.
        /// Assuming that this is the correct behaviour.
        /// </summary>
        [Fact]
        public void ControllerCallsBeginTransactionTwiceWhenCallingMethodWithoutManualTransactionAttributeAndTransactionScope()
        {
            _controller.ActionInvoker.InvokeAction(_controller.ControllerContext,
                                                   "MethodWithoutManualTransactionAttributeAndTransactionScope");

            Assert.Equal(2, _beginTransactionCount);
            //_dbContext.AssertWasCalled(a => a.BeginTransaction(), a => a.Repeat.Twice());
        }


        internal class SampleController : TestSuperController
        {

            public ActionResult MethodWithoutManualTransactionAttribute()
            {
                return Content("String");
            }

            [HandleTransactionsManually]
            public ActionResult MethodWithManualTransactionAttribute()
            {
                return Content("String");
            }

            [Transaction]
            public ActionResult MethodWithTransactionAttribute()
            {
                return Content("String");
            }

            [HandleTransactionsManually]
            public ActionResult MethodWithManualTransactionAttributeAndTransactionScope()
            {
                using (var ts = new TransactionScope())
                {
                    ts.CommitTransaction();
                }
                return Content("String");
            }


            public ActionResult MethodWithoutManualTransactionAttributeAndTransactionScope()
            {
                using (var ts = new TransactionScope())
                {
                    ts.CommitTransaction();
                }
                return Content("String");
            }
        }

        [UseTransactionsByDefault]
        internal class TestSuperController : Controller
        {

        }
    }
}