using NAudio.Wave;
using System;

namespace Simple3DEngine
{
    public class StereoSampleProvider : ISampleProvider
    {
        private readonly ISampleProvider source;
        private float volume = 1f;
        private float pan = 0f; // -1.0 for left, 1.0 for right

        public StereoSampleProvider(ISampleProvider source)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public void SetVolume(float volume)
        {
            this.volume = volume;
        }

        public void SetPan(float pan)
        {
            this.pan = pan;
        }

        public WaveFormat WaveFormat => source.WaveFormat;

        public int Read(float[] buffer, int offset, int count)
        {
            int samplesRead = source.Read(buffer, offset, count);

            // Apply volume and panning
            for (int i = offset; i < offset + samplesRead; i += 2)
            {
                // Apply volume
                buffer[i] *= volume;
                if (i + 1 < buffer.Length)
                {
                    buffer[i + 1] *= volume;
                }

                // Apply panning
                float leftPan = 1f - Math.Max(0, pan);
                float rightPan = 1f + Math.Min(0, pan);

                buffer[i] *= leftPan; // Left channel
                if (i + 1 < buffer.Length)
                {
                    buffer[i + 1] *= rightPan; // Right channel
                }
            }
            return samplesRead;
        }
    }
}
