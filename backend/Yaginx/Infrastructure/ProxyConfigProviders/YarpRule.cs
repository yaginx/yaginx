using System;
using System.Collections.Generic;

namespace Yaginx.Infrastructure.ProxyConfigProviders
{
	public class YarpRule
	{
		public string RequestPattern { get; set; }
		public Dictionary<string, string> Clusters { get; set; }

		public override bool Equals(object obj)
		{
			if (obj is YarpRule other)
			{
				bool isClusterSame = true;
				foreach (var item in other.Clusters.Keys)
				{
					if (!isClusterSame)
						break;

					if (!Clusters.ContainsKey(item) || Clusters[item] != other.Clusters[item])
					{
						isClusterSame = false;
					}
				}

				return RequestPattern == other.RequestPattern && isClusterSame;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(RequestPattern, Clusters);
		}
	}
}
