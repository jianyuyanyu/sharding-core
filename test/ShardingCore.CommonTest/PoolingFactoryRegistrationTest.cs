using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using ShardingCore;
using ShardingCore.Core.RuntimeContexts;
using ShardingCore.Sharding;
using Xunit;

namespace ShardingCore.CommonTest
{
    public class PoolingFactoryRegistrationTest
    {
        [Fact]
        public void AddDbContextPool_ResolvesResettableService()
        {
            var services = new ServiceCollection();
            var runtimeContext = new ShardingRuntimeContext<TestPoolDbContext>();
            services.AddDbContextPool<TestPoolDbContext>(options =>
                options.UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .UseShardingOptions(runtimeContext));

            using var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TestPoolDbContext>();
            context.MarkExecutorCreated();

            var resettable = context.GetService<IResettableService>();
            Assert.NotNull(resettable);
            resettable!.ResetState();

            Assert.False(context.ExecutorCreated);
        }

        [Fact]
        public void AddDbContextFactory_ResolvesResettableService()
        {
            var services = new ServiceCollection();
            var runtimeContext = new ShardingRuntimeContext<TestPoolDbContext>();
            services.AddDbContextFactory<TestPoolDbContext>(options =>
                options.UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .UseShardingOptions(runtimeContext));

            using var provider = services.BuildServiceProvider();
            var factory = provider.GetRequiredService<IDbContextFactory<TestPoolDbContext>>();
            using var context = factory.CreateDbContext();
            context.MarkExecutorCreated();

            var resettable = context.GetService<IResettableService>();
            Assert.NotNull(resettable);
            resettable!.ResetState();

            Assert.False(context.ExecutorCreated);
        }

        [Fact]
        public void AddPooledDbContextFactory_ResolvesResettableService()
        {
            var services = new ServiceCollection();
            var runtimeContext = new ShardingRuntimeContext<TestPoolDbContext>();
            services.AddPooledDbContextFactory<TestPoolDbContext>(options =>
                options.UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .UseShardingOptions(runtimeContext));

            using var provider = services.BuildServiceProvider();
            var factory = provider.GetRequiredService<IDbContextFactory<TestPoolDbContext>>();
            using var context = factory.CreateDbContext();
            context.MarkExecutorCreated();

            var resettable = context.GetService<IResettableService>();
            Assert.NotNull(resettable);
            resettable!.ResetState();

            Assert.False(context.ExecutorCreated);
        }

        private sealed class TestPoolDbContext : AbstractShardingDbContext
        {
            public TestPoolDbContext(DbContextOptions options) : base(options)
            {
            }

            public bool ExecutorCreated => base.ExecutorCreated;

            public void MarkExecutorCreated()
            {
                SetExecutorCreated(true);
            }
        }
    }
}
