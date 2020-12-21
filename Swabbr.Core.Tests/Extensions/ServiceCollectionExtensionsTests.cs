using Microsoft.Extensions.DependencyInjection;
using Swabbr.Core.Extensions;
using Swabbr.Testing.DummyServices;
using System.Linq;
using Xunit;

namespace Swabbr.Core.Tests.Extensions
{
    /// <summary>
    ///     Testsing <see cref="ServiceCollectionExtensions"/> functionality.
    /// </summary>
    public class ServiceCollectionExtensionsTests
    {
        private readonly IServiceCollection services;

        /// <summary>
        ///     Before each tests.
        /// </summary>
        public ServiceCollectionExtensionsTests() 
            => services = new ServiceCollection();

        [Fact]
        public void AddOrReplaceAddsService()
        {
            // Act.
            services.AddOrReplace<IDummyService, DummyServiceTwo>(ServiceLifetime.Transient);
            
            var dummyServices = services.Where(x => x.ServiceType == typeof(IDummyService));

            // Assert.
            Assert.Single(dummyServices);
            Assert.Equal(typeof(IDummyService), dummyServices.First().ServiceType);
            Assert.Equal(typeof(DummyServiceTwo), dummyServices.First().ImplementationType);
            Assert.Equal(ServiceLifetime.Transient, dummyServices.First().Lifetime);
        }

        [Fact]
        public void AddOrReplaceReplacesService()
        {
            // Arrange.
            services.AddTransient<IDummyService, DummyServiceOne>();

            // Act.
            services.AddOrReplace<IDummyService, DummyServiceTwo>(ServiceLifetime.Transient);
            var dummyServices = services.Where(x => x.ServiceType == typeof(IDummyService));

            // Assert.
            Assert.Single(dummyServices);
            Assert.Equal(typeof(IDummyService), dummyServices.First().ServiceType);
            Assert.Equal(typeof(DummyServiceTwo), dummyServices.First().ImplementationType);
            Assert.Equal(ServiceLifetime.Transient, dummyServices.First().Lifetime);
        }

        [Fact]
        public void AddOrReplacePreservesLifetime()
        {
            // Arrange.
            services.AddTransient<IDummyService, DummyServiceOne>();

            // Act.
            services.AddOrReplace<IDummyService, DummyServiceOne>(ServiceLifetime.Singleton);
            var dummyServices = services.Where(x => x.ServiceType == typeof(IDummyService));

            // Assert.
            Assert.Single(dummyServices);
            Assert.Equal(typeof(IDummyService), dummyServices.First().ServiceType);
            Assert.Equal(typeof(DummyServiceOne), dummyServices.First().ImplementationType);
            Assert.Equal(ServiceLifetime.Transient, dummyServices.First().Lifetime);
        }

        [Fact]
        public void AddOrReplaceReplacesServicePreservesLifetime()
        {
            // Arrange.
            services.AddTransient<IDummyService, DummyServiceOne>();

            // Act.
            services.AddOrReplace<IDummyService, DummyServiceTwo>(ServiceLifetime.Singleton);
            var dummyServices = services.Where(x => x.ServiceType == typeof(IDummyService));

            // Assert.
            Assert.Single(dummyServices);
            Assert.Equal(typeof(IDummyService), dummyServices.First().ServiceType);
            Assert.Equal(typeof(DummyServiceTwo), dummyServices.First().ImplementationType);
            Assert.Equal(ServiceLifetime.Transient, dummyServices.First().Lifetime);
        }
    }
}
