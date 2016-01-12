using System;

namespace UCDArch.Core
{
    public static class ServiceLocator
    {
        private static IServiceProvider currentProvider;

        public static IServiceProvider Current
        {
            get
            {
                if (!IsLocationProviderSet) throw new InvalidOperationException("ServiceProvider has not been initialized");

                return currentProvider;
            }
        }

        public static void SetLocatorProvider(IServiceProvider newProvider)
        {
            currentProvider = newProvider;
        }

        public static bool IsLocationProviderSet
        {
            get
            {
                return currentProvider != null;
            }
        }
    }
}
