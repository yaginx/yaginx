// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Model;

namespace Yarp.ReverseProxy.SessionAffinity;

internal abstract class BaseHashCookieSessionAffinityPolicy : ISessionAffinityPolicy
{
    private static readonly object AffinityKeyId = new();
    private readonly ILogger _logger;
    private readonly TimeProvider _timeProvider;

    public BaseHashCookieSessionAffinityPolicy(TimeProvider timeProvider, ILogger logger)
    {
        _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public abstract string Name { get; }

    public void AffinitizeResponse(HttpContext context, ClusterState cluster, SessionAffinityConfig config, DestinationState destination)
    {
        if (!config.Enabled.GetValueOrDefault())
        {
            throw new InvalidOperationException("Session affinity is disabled for cluster.");
        }

        // Affinity key is set on the response only if it's a new affinity.
        if (!context.Items.ContainsKey(AffinityKeyId))
        {
            var affinityKey = GetDestinationHash(destination);
            var affinityCookieOptions = AffinityHelpers.CreateCookieOptions(config.Cookie, context.Request.IsHttps, _timeProvider);
            context.Response.Cookies.Append(config.AffinityKeyName, affinityKey, affinityCookieOptions);
        }
    }

    public AffinityResult FindAffinitizedDestinations(HttpContext context, ClusterState cluster, SessionAffinityConfig config, IReadOnlyList<DestinationState> destinations)
    {
        if (!config.Enabled.GetValueOrDefault())
        {
            throw new InvalidOperationException($"Session affinity is disabled for cluster {cluster.ClusterId}.");
        }

        var affinityHash = context.Request.Cookies[config.AffinityKeyName];
        if (affinityHash is null)
        {
            return new(null, AffinityStatus.AffinityKeyNotSet);
        }

        foreach (var d in destinations)
        {
            var hashValue = GetDestinationHash(d);

            if (affinityHash == hashValue)
            {
                context.Items[AffinityKeyId] = affinityHash;
                return new(d, AffinityStatus.OK);
            }
        }

        if (destinations.Count == 0)
        {
            Log.AffinityCannotBeEstablishedBecauseNoDestinationsFound(_logger, cluster.ClusterId);
        }
        else
        {
            Log.DestinationMatchingToAffinityKeyNotFound(_logger, cluster.ClusterId);
        }

        return new(null, AffinityStatus.DestinationNotFound);
    }

    protected abstract string GetDestinationHash(DestinationState d);
}
