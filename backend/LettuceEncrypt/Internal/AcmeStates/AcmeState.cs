// Copyright (c) Nate McMaster.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.DependencyInjection;

namespace LettuceEncrypt.Internal.AcmeStates;

internal enum AcmeStateContinueStatus
{
    Continue,
    End
}
internal interface IAcmeState
{
    AcmeStateContinueStatus ContinueState { get; }
    Task<IAcmeState> MoveNextAsync(CancellationToken cancellationToken);
}

internal class TerminalState : IAcmeState
{
    public static TerminalState Singleton { get; } = new();
    public AcmeStateContinueStatus ContinueState => AcmeStateContinueStatus.End;
    private TerminalState() { }

    public Task<IAcmeState> MoveNextAsync(CancellationToken cancellationToken)
    {
        throw new OperationCanceledException();
    }
}

internal abstract class AcmeState : IAcmeState
{
    public AcmeStateMachineContext Context { get; }
    public AcmeStateContinueStatus ContinueState => AcmeStateContinueStatus.Continue;

    public AcmeState(AcmeStateMachineContext context)
    {
        Context = context;
    }

    public abstract Task<IAcmeState> MoveNextAsync(CancellationToken cancellationToken);

    protected T MoveTo<T>() where T : IAcmeState
    {
        return Context.Services.GetRequiredService<T>();
    }
}

internal abstract class SyncAcmeState : AcmeState
{
    protected SyncAcmeState(AcmeStateMachineContext context) : base(context)
    {
    }

    public override Task<IAcmeState> MoveNextAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var next = MoveNext();

        return Task.FromResult(next);
    }

    public abstract IAcmeState MoveNext();
}
