using Microsoft.Extensions.Primitives;
using System;
using System.Threading;

namespace Yaginx.Infrastructure.ProxyConfigProviders
{
	/// <summary>
	/// 路由变更Token，实现路由热更新
	/// </summary>
	public class RouteChangeToken : IChangeToken, IDisposable
	{
		private CancellationTokenSource _TokenSource = new CancellationTokenSource();
		public bool HasChanged => _TokenSource.IsCancellationRequested;
		public bool ActiveChangeCallbacks => true;
		public IDisposable RegisterChangeCallback(Action<object> callback, object state)
			=> _TokenSource.Token.Register(callback, state);
		/// <summary>
		/// 调用之后，更新路由
		/// </summary>
		public void OnReload()
			=> _TokenSource.Cancel();

		public void Dispose()
			=> _TokenSource?.Dispose();
	}
}
