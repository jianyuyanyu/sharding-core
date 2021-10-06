﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShardingCore.Sharding.Abstractions;

namespace ShardingCore.DIExtensions
{
    /*
    * @Author: xjm
    * @Description:
    * @Date: 2021/9/19 20:49:03
    * @Ver: 1.0
    * @Email: 326308290@qq.com
    */
    public class ShardingCoreConfigBuilder<TShardingDbContext, TActualDbContext>
        where TActualDbContext : DbContext
        where TShardingDbContext : DbContext, IShardingDbContext<TActualDbContext>
    {
        public IServiceCollection Services { get; }


        public ShardingConfigOption<TShardingDbContext> ShardingConfigOption { get; }



        public ShardingCoreConfigBuilder(IServiceCollection services)
        {
            Services = services;
            ShardingConfigOption = new ShardingConfigOption<TShardingDbContext>();
        }


        public ShardingQueryBuilder<TShardingDbContext, TActualDbContext> Begin(Action<ShardingCoreBeginOptions> shardingCoreBeginOptionsConfigure)
        {
            var shardingCoreBeginOptions = new ShardingCoreBeginOptions();
            shardingCoreBeginOptionsConfigure?.Invoke(shardingCoreBeginOptions);
            if (shardingCoreBeginOptions.ParallelQueryMaxThreadCount <= 0)
                throw new ArgumentException(
                    $"{nameof(shardingCoreBeginOptions.ParallelQueryMaxThreadCount)} should greater than zero thread count");
            if (shardingCoreBeginOptions.ParallelQueryTimeOut.TotalMilliseconds <= 0)
                throw new ArgumentException(
                    $"{nameof(shardingCoreBeginOptions.ParallelQueryTimeOut)} should greater than zero milliseconds");
            ShardingConfigOption.EnsureCreatedWithOutShardingTable = shardingCoreBeginOptions.EnsureCreatedWithOutShardingTable;
            ShardingConfigOption.AutoTrackEntity = shardingCoreBeginOptions.AutoTrackEntity;
            ShardingConfigOption.ParallelQueryMaxThreadCount = shardingCoreBeginOptions.ParallelQueryMaxThreadCount;
            ShardingConfigOption.ParallelQueryTimeOut = shardingCoreBeginOptions.ParallelQueryTimeOut;
            ShardingConfigOption.CreateShardingTableOnStart = shardingCoreBeginOptions.CreateShardingTableOnStart;
            ShardingConfigOption.IgnoreCreateTableError = shardingCoreBeginOptions.IgnoreCreateTableError;
            return new ShardingQueryBuilder<TShardingDbContext, TActualDbContext>(this);
        }
        //public ShardingCoreConfigBuilder<TShardingDbContext, TActualDbContext> AddDefaultDataSource(string dataSourceName, string connectionString)
        //{
        //    if (!string.IsNullOrWhiteSpace(defaultDataSourceName) || !string.IsNullOrWhiteSpace(defaultConnectionString))
        //        throw new InvalidOperationException($"{nameof(AddDefaultDataSource)}-{dataSourceName}");
        //    this.defaultDataSourceName = dataSourceName;
        //    this.defaultConnectionString = connectionString;
        //    return this;
        //}

        //public ShardingCoreConfigBuilder<TShardingDbContext, TActualDbContext> AddDataSource(string dataSourceName, string connectionString)
        //{
        //    if (_dataSources.ContainsKey(dataSourceName))
        //        throw new InvalidOperationException($"{nameof(AddDataSource)}-{dataSourceName} repeat");
        //    _dataSources.Add(dataSourceName, connectionString);
        //    return this;
        //}
    }

    public class ShardingCoreBeginOptions
    {
        /// <summary>
        /// 如果数据库不存在就创建并且创建表除了分表的
        /// </summary>
        public bool EnsureCreatedWithOutShardingTable { get; set; }

        /// <summary>
        /// 是否需要在启动时创建分表
        /// </summary>
        public bool? CreateShardingTableOnStart { get; set; }
        /// <summary>
        /// 是否自动追踪实体
        /// </summary>
        public bool AutoTrackEntity { get; set; }

        /// <summary>
        /// 单次查询并发线程数目(最小1)
        /// </summary>
        public int ParallelQueryMaxThreadCount { get; set; } = 65536;
        /// <summary>
        /// 默认30秒超时
        /// </summary>
        public TimeSpan ParallelQueryTimeOut { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>
        /// 忽略建表时的错误
        /// </summary>
        public bool? IgnoreCreateTableError { get; set; }
    }
}