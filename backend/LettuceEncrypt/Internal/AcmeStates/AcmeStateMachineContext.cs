// Copyright (c) Nate McMaster.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using LettuceEncrypt.Acme;

namespace LettuceEncrypt.Internal.AcmeStates;

internal class AcmeStateMachineContext
{
    public IServiceProvider Services { get; }
    public string? CurrentDomain { get; set; }
    public string? EmailAddress { get; set; }
    public KeyAlgorithm KeyAlgorithm { get; set; } = KeyAlgorithm.ES256;
    public ChallengeType AllowedChallengeTypes { get; set; } = ChallengeType.Http01;
    public string[] AdditionalIssuers { get; set; } = Array.Empty<string>();

    /// <summary>
    /// How long before certificate expiration will be renewal attempted.
    /// Set to <c>null</c> to disable automatic renewal.
    /// </summary>
    public TimeSpan? RenewDaysInAdvance { get; set; } = TimeSpan.FromDays(30);

    /// <summary>
    /// How often will be certificates checked for renewal
    /// </summary>
    public TimeSpan? RenewalCheckPeriod { get; set; } = TimeSpan.FromDays(1);

    public AcmeStateMachineContext(IServiceProvider services)
    {
        Services = services;
    }
}
