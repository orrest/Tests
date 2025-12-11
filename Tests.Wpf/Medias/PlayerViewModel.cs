using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Components;
using SoundFlow.Enums;
using SoundFlow.Providers;
using SoundFlow.Structs;
using System.IO;
using Tests.Wpf.Constants;

namespace Tests.Wpf.Medias;

public sealed partial class PlayerViewModel : ObservableRecipient
{
    [RelayCommand]
    public async Task Play()
    {
        // Initialize the audio engine with the MiniAudio backend.
        using var audioEngine = new MiniAudioEngine();

        // Find the default playback device.
        var defaultPlaybackDevice = audioEngine
            .PlaybackDevices
            .FirstOrDefault(d => d.IsDefault);

        if (defaultPlaybackDevice.Id == IntPtr.Zero)
        {
            Messenger.Send("No default playback device found.", Channels.TOAST);
            return;
        }

        // The audio format for processing. We'll use 32-bit float, which is standard for processing.
        // The data provider will handle decoding the source file to this format.
        var audioFormat = new AudioFormat
        {
            Format = SampleFormat.F32,
            SampleRate = 48000,
            Channels = 2,
        };

        // Initialize the playback device. This manages the connection to the physical audio hardware.
        // The 'using' statement ensures it's properly disposed of.
        using var device = audioEngine
            .InitializePlaybackDevice(defaultPlaybackDevice, audioFormat);

        // Create a data provider for the audio file.
        // Replace "path/to/your/audiofile.wav" with the actual path to your audio file.
        using var dataProvider = new StreamDataProvider(
            audioEngine,
            audioFormat,
            File.OpenRead("Files/file_example_MP3_1MG.mp3")
        );

        // Create a SoundPlayer, linking the engine, format, and data provider.
        // The player is also IDisposable.
        using var player = new SoundPlayer(audioEngine, audioFormat, dataProvider);

        // Add the player to the device's master mixer to route its audio for playback.
        device.MasterMixer.AddComponent(player);

        // Start the device. This opens the audio stream to the hardware.
        device.Start();

        var tcs = new TaskCompletionSource<bool>();
        player.PlaybackEnded += (s, e) => tcs.SetResult(true);

        // Start playback.
        player.Play();

        await tcs.Task;
    }
}
