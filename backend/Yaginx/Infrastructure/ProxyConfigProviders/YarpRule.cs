using System;
using System.Collections.Generic;

namespace Yaginx.Infrastructure.ProxyConfigProviders
{
	public class YarpRule
	{
		public string RequestPattern { get; set; }

        public List<ClusterItem> Clusters { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is YarpRule other)
            {
                bool isClusterSame = true;
                foreach (var item in other.Clusters)
                {
                    if (!isClusterSame)
                        break;

                    //if (!this.Clusters.ContainsKey(item) || this.Clusters[item] != other.Clusters[item])
                    //{
                    //    isClusterSame = false;
                    //}
                    var clusterItem = Clusters.FirstOrDefault(x => x.ClusterId == item.ClusterId);
                    if (clusterItem == null || clusterItem.Address != item.Address)
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
    public class ClusterItem
    {
        public string ClusterId { get; set; }
        public string Address { get; set; }
    }
}
