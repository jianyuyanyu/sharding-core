using ShardingCore.Sharding.Abstractions;
using ShardingCore.Sharding.Parsers.Abstractions;
using ShardingCore.Sharding.Parsers.Visitors;
using ShardingCore.Sharding.Visitors;
using System.Linq.Expressions;

namespace ShardingCore.Sharding.Parsers
{
    /// <summary>
    /// 
    /// </summary>
    /// Author: xjm
    /// Created: 2022/5/1 21:37:25
    /// Email: 326308290@qq.com
    public class DefaultPrepareParser : IPrepareParser
    {
        private static readonly MemoryExtensionsToEnumerableReplacingExpressionVisitor MemoryExtensionsReplacer = new();

        public IPrepareParseResult Parse(IShardingDbContext shardingDbContext, Expression query)
        {
            // .NET 6+ / C# 10+: The compiler may optimize array.Contains() to use MemoryExtensions.Contains
            // with ReadOnlySpan<T>. EF Core cannot evaluate this because ReadOnlySpan<T> is a ref struct.
            // We replace these calls with Enumerable.Contains before processing.
            var rewrittenQuery = MemoryExtensionsReplacer.Visit(query);

            var shardingQueryPrepareVisitor = new ShardingQueryPrepareVisitor(shardingDbContext);
            var expression = shardingQueryPrepareVisitor.Visit(rewrittenQuery);
            var shardingPrepareResult = shardingQueryPrepareVisitor.GetShardingPrepareResult();
            return new PrepareParseResult(shardingDbContext, expression, shardingPrepareResult);
        }
    }
}
