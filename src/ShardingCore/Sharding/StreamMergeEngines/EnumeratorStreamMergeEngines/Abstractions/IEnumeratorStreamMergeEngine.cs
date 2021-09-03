﻿using System;
using System.Collections.Generic;

namespace ShardingCore.Sharding.StreamMergeEngines.EnumeratorStreamMergeEngines.Abstractions
{
    /*
    * @Author: xjm
    * @Description:
    * @Date: 2021/9/2 15:15:34
    * @Ver: 1.0
    * @Email: 326308290@qq.com
    */
    public interface IEnumeratorStreamMergeEngine<TEntity> : IAsyncEnumerable<TEntity>, IEnumerable<TEntity>, IDisposable
    {
    }
}
