using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using ShardingCore.Sharding;

namespace ShardingCore.EFCores
{
    public sealed class ShardingDbContextResettableService : IResettableService
    {
        private readonly ICurrentDbContext _currentDbContext;

        public ShardingDbContextResettableService(ICurrentDbContext currentDbContext)
        {
            _currentDbContext = currentDbContext;
        }

        public void ResetState()
        {
            if (_currentDbContext.Context is AbstractShardingDbContext shardingDbContext)
            {
                shardingDbContext.ResetShardingDbContextExecutor();
            }
        }

        public Task ResetStateAsync(CancellationToken cancellationToken = default)
        {
            ResetState();
            return Task.CompletedTask;
        }
    }
}
