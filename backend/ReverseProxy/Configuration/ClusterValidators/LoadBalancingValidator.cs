﻿using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yarp.ReverseProxy.LoadBalancing;
using Yarp.ReverseProxy.Utilities;

namespace Yarp.ReverseProxy.Configuration.ClusterValidators;

internal sealed class LoadBalancingValidator : IClusterValidator
{
    private readonly FrozenDictionary<string, ILoadBalancingPolicy> _loadBalancingPolicies;
    public LoadBalancingValidator(IEnumerable<ILoadBalancingPolicy> loadBalancingPolicies)
    {
        _loadBalancingPolicies = loadBalancingPolicies?.ToDictionaryByUniqueId(p => p.Name) ?? throw new ArgumentNullException(nameof(loadBalancingPolicies));
    }

    public ValueTask ValidateAsync(ClusterConfig cluster, IList<Exception> errors)
    {
        var loadBalancingPolicy = cluster.LoadBalancingPolicy;
        if (string.IsNullOrEmpty(loadBalancingPolicy))
        {
            // The default.
            loadBalancingPolicy = LoadBalancingPolicies.PowerOfTwoChoices;
        }

        if (!_loadBalancingPolicies.ContainsKey(loadBalancingPolicy))
        {
            errors.Add(new ArgumentException($"No matching {nameof(ILoadBalancingPolicy)} found for the load balancing policy '{loadBalancingPolicy}' set on the cluster '{cluster.ClusterId}'."));
        }

        return ValueTask.CompletedTask;
    }
}
