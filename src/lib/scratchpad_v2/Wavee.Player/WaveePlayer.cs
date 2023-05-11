﻿using System.Threading.Channels;
using LanguageExt;
using Wavee.Infrastructure.Live;
using Wavee.Player.Commanding;
using Wavee.Player.States;
using Wavee.Player.Sys;

namespace Wavee.Player;

public sealed class WaveePlayer : IWaveePlayer
{
    private readonly ChannelWriter<IInternalPlayerCommand> _commandWriter;
    private static readonly WaveePlayer _instance = new WaveePlayer();
    public static IWaveePlayer Instance => _instance;

    private readonly Ref<IWaveePlayerState> _state = Ref((IWaveePlayerState)InvalidState.Default);

    private WaveePlayer()
    {
        var commands = Channel.CreateUnbounded<IInternalPlayerCommand>();
        _commandWriter = commands.Writer;

        _ = WaveePlayerRuntime<WaveeRuntime>.Start(commands.Reader, _state)
            .Run(WaveeCore.Runtime)
            .Result;
    }

    public async ValueTask<bool> Pause()
    {
        if (_state.Value is not WaveePlayingState)
            return false;

        await _commandWriter.WriteAsync(new InternalPauseCommand<WaveeRuntime>(WaveeCore.Runtime));
        return true;
    }

    public async ValueTask<Unit> Play(IAudioStream stream)
    {
        await _commandWriter.WriteAsync(new InternalPlayCommand<WaveeRuntime>(WaveeCore.Runtime, stream));
        return unit;
    }

    public IWaveePlayerState State => _state.Value;
    public IObservable<IWaveePlayerState> StateObservable => _state.OnChange();
}

public interface IWaveePlayer
{
    ValueTask<bool> Pause();
    ValueTask<Unit> Play(IAudioStream stream);
    IWaveePlayerState State { get; }
    IObservable<IWaveePlayerState> StateObservable { get; }
}