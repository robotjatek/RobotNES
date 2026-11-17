using System;
using NAudio.Wave;

namespace RobotNES;

public class AudioPlayer : IDisposable
{
    private readonly WaveOut  _waveOut;
    private readonly BufferedWaveProvider _buffer;
    private byte[] _byteBuffer = [];

    public AudioPlayer()
    {
        var format = WaveFormat.CreateIeeeFloatWaveFormat(44100, 1);
        _buffer = new BufferedWaveProvider(format)
        {
            DiscardOnBufferOverflow = true, 
        };

        _waveOut = new WaveOut
        {
            BufferMilliseconds = 35
        };
        _waveOut.Init(_buffer);
        _waveOut.Play();
    }

    public void AddSamples(float[] samples, int count)
    {
        if (_buffer.BufferedDuration.TotalMilliseconds > 80)
        {
            return;
        }
        
        var byteCount = count * sizeof(float);
        if (_byteBuffer.Length < byteCount)
        {
            _byteBuffer = new byte[byteCount];
        }
        
        Buffer.BlockCopy(samples, 0, _byteBuffer, 0, byteCount);
        _buffer.AddSamples(_byteBuffer, 0, byteCount);
    }
    
    public void Clear() => _buffer.ClearBuffer();

    public void Dispose()
    {
        _waveOut.Stop();
        _waveOut.Dispose();
    }
}