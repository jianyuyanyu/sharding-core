using Microsoft.EntityFrameworkCore;
using ShardingCore.Core.VirtualRoutes.TableRoutes.RouteTails.Abstractions;
using ShardingCore.Sharding;
using ShardingCore.Sharding.Abstractions;

namespace Sample.MySql.multi;

public class OtherDbContext:AbstractShardingDbContext,IShardingTableDbContext
{
    public DbSet<MyUser> MyUsers { get; set; }
    public OtherDbContext(DbContextOptions<OtherDbContext> options) : base(options)
    {
    }

    public IRouteTail RouteTail { get; set; }
}