using Castle.Windsor;
using Microsoft.Practices.ServiceLocation;
using System;
using UCDArch.Core;
using UCDArch.Core.CommonValidator;
using UCDArch.Core.DataAnnotationsValidator.CommonValidatorAdapter;
using UCDArch.Web.IoC;
using Castle.MicroKernel.Registration;
using Xunit;

namespace UCDArch.Tests.UCDArch.Core
{
    public class SmartServiceLocatorTests
    {
        public SmartServiceLocatorTests()
        {
            ServiceLocator.SetLocatorProvider(null);
        }

        [Fact]
        public void WillBeInformedIfServiceLocatorNotInitialized()
        {
            bool exceptionThrown = false;

            try
            {
                SmartServiceLocator<IValidator>.GetService();
            }
            catch (NullReferenceException e)
            {
                exceptionThrown = true;
                Assert.True(e.Message.Contains("ServiceLocator has not been initialized"));
            }

            Assert.True(exceptionThrown);
        }

        [Fact]
        public void WillBeInformedIfServiceNotRegistered()
        {
            bool exceptionThrown = false;

            IWindsorContainer container = new WindsorContainer();
            ServiceLocator.SetLocatorProvider(() => new WindsorServiceLocator(container));

            try
            {
                SmartServiceLocator<IValidator>.GetService();
            }
            catch (ActivationException e)
            {
                exceptionThrown = true;
                Assert.True(e.Message.Contains("IValidator could not be located"));
            }

            Assert.True(exceptionThrown);
        }

        [Fact]
        public void CanReturnServiceIfInitializedAndRegistered()
        {
            IWindsorContainer container = new WindsorContainer();
            container.Register(Component.For<IValidator>().ImplementedBy<Validator>().Named("validator"));
            
            ServiceLocator.SetLocatorProvider(() => new WindsorServiceLocator(container));

            IValidator validatorService = SmartServiceLocator<IValidator>.GetService();

            Assert.NotNull(validatorService);
        }
    }
}