using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using Yarp.ReverseProxy.Configuration;

namespace Yaginx.Infrastructure.ProxyConfigProviders
{
	/// <summary>
	/// 网关配置实现类
	/// </summary>
	public class ProxyConfig : IProxyConfig
	{
		/// <summary>
		/// 当前路由
		/// </summary>
		public List<RouteConfig> Routes { get; internal set; } = new List<RouteConfig>();
		/// <summary>
		/// 当前集群
		/// </summary>
		public List<ClusterConfig> Clusters { get; internal set; } = new List<ClusterConfig>();
		/// <summary>
		/// 热更新Token
		/// </summary>
		public IChangeToken ChangeToken { get; internal set; } = default!;
		/// <summary>
		/// 实现接口
		/// </summary>
		IReadOnlyList<RouteConfig> IProxyConfig.Routes => Routes;
		/// <summary>
		/// 实现接口
		/// </summary>
		IReadOnlyList<ClusterConfig> IProxyConfig.Clusters => Clusters;
	}
}
