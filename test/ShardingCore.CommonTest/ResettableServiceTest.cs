using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ShardingCore.EFCores;
using ShardingCore.Sharding;
using Xunit;

namespace ShardingCore.CommonTest
{
    public class ResettableServiceTest
    {
        [Fact]
        public void ResetState_ClearsExecutorFlag()
        {
            var options = new DbContextOptionsBuilder<TestResettableDbContext>().Options;
            using var context = new TestResettableDbContext(options);
            context.MarkExecutorCreated();

            var resettableService = new ShardingDbContextResettableService(new TestCurrentDbContext(context));
            resettableService.ResetState();

            Assert.False(context.ExecutorCreated);
        }

        [Fact]
        public async Task ResetStateAsync_ClearsExecutorFlag()
        {
            var options = new DbContextOptionsBuilder<TestResettableDbContext>().Options;
            using var context = new TestResettableDbContext(options);
            context.MarkExecutorCreated();

            var resettableService = new ShardingDbContextResettableService(new TestCurrentDbContext(context));
            await resettableService.ResetStateAsync();

            Assert.False(context.ExecutorCreated);
        }

        private sealed class TestResettableDbContext : AbstractShardingDbContext
        {
            public TestResettableDbContext(DbContextOptions options) : base(options)
            {
            }

            public bool ExecutorCreated => base.ExecutorCreated;

            public void MarkExecutorCreated()
            {
                SetExecutorCreated(true);
            }
        }

        private sealed class TestCurrentDbContext : ICurrentDbContext
        {
            public TestCurrentDbContext(DbContext context)
            {
                Context = context;
            }

            public DbContext Context { get; }
        }
    }
}
