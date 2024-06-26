﻿using Microsoft.Extensions.Primitives;
using System.Runtime.CompilerServices;

namespace Yaginx.SimpleProcessors.ConfigProviders;

public interface ISimpleProcessorConfig
{
    private static readonly ConditionalWeakTable<ISimpleProcessorConfig, string> _revisionIdsTable = new();

    /// <summary>
    /// A unique identifier for this revision of the configuration.
    /// </summary>
    string RevisionId => _revisionIdsTable.GetValue(this, static _ => Guid.NewGuid().ToString());

    /// <summary>
    /// Routes matching requests to clusters.
    /// </summary>
    IReadOnlyList<WebSiteMetadataConfig> WebSites { get; }

    /// <summary>
    /// A notification that triggers when this snapshot expires.
    /// </summary>
    IChangeToken ChangeToken { get; }
}   /// <summary>
    /// 网关配置实现类
    /// </summary>
public class SimpleProcessConfig : ISimpleProcessorConfig
{
    /// <summary>
    /// 当前路由
    /// </summary>
    public List<WebSiteMetadataConfig> WebSites { get; internal set; } = new List<WebSiteMetadataConfig>();

    /// <summary>
    /// 热更新Token
    /// </summary>
    public IChangeToken ChangeToken { get; internal set; } = default!;
    /// <summary>
    /// 实现接口
    /// </summary>
    IReadOnlyList<WebSiteMetadataConfig> ISimpleProcessorConfig.WebSites => WebSites;
}
