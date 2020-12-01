using Microsoft.Extensions.DependencyInjection;
using System;
using WordCounter.Business;

namespace WordCounter
{
    class Program
    {
        private static IServiceProvider _serviceProvider;
        static void Main()
        {
            RegisterServices();
            IServiceScope scope = _serviceProvider.CreateScope();
            scope.ServiceProvider.GetRequiredService<CounterApplication>().Run();
            DisposeServices();

        }
        private static void RegisterServices()
        {
            var services = new ServiceCollection();
            services.AddTransient<ITextManager, TextManager>();
            services.AddTransient<ICounterManager, CounterManager>();
            services.AddSingleton<CounterApplication>();
            _serviceProvider = services.BuildServiceProvider(true);
        }

        private static void DisposeServices()
        {
            if (_serviceProvider == null)
            {
                return;
            }
            if (_serviceProvider is IDisposable)
            {
                ((IDisposable)_serviceProvider).Dispose();
            }
        }

    }
}
