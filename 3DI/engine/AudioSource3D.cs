using System;
using System.Runtime.CompilerServices;
using NAudio.Wave;

namespace Simple3DEngine
{
    public class AudioSource3D
    {
        public Vector3D Position { get; set; }
        public float Volume { get; set; }
        public float MaxDistance { get; set; }
        public float MinDistance { get; set; }
        private WaveOutEvent waveOut;
        private StereoSampleProvider stereoProvider;
        private AudioFileReader audioFile;

        public AudioSource3D(Vector3D position, float volume, float maxDistance, float minDistance, string audioFilePath)
        {
            if (System.IO.File.Exists(audioFilePath)) { 
                Position = position;
                Volume = volume;
                MaxDistance = maxDistance;
                MinDistance = minDistance;

                waveOut = new WaveOutEvent();
                audioFile = new AudioFileReader(audioFilePath);
                stereoProvider = new StereoSampleProvider(audioFile);
                waveOut.Init(stereoProvider);
            } else
            {
                Console.WriteLine("AudioSource3D not initialized due to missing audio file \"" + audioFilePath  + "\"");
            }
        }

        public void Play()
        {
            audioFile.Position = 0;
            waveOut.Play();
        }

        public void Stop()
        {
            waveOut.Stop();
        }

        public void Restart()
        {
            Stop(); // Stop the current playback
            audioFile.Position = 0; // Reset audio file position to the beginning
            stereoProvider = new StereoSampleProvider(audioFile); // Reinitialize the sample provider
            waveOut.Init(stereoProvider); // Reinitialize WaveOut with the new sample provider
            Play(); // Start playback again
        }

        public void Update(Vector3D listenerPosition, Vector3D listenerDirection)
        {
            if (listenerPosition == null || listenerDirection == null)
            {
                throw new ArgumentNullException("Listener position and listener direction must be non-null.");
            }

            float distance = Vector3D.Distance(Position, listenerPosition);
            float calculatedVolume = CalculateVolume(distance);

            // Update stereo panning based on listener direction
            Vector3D toSource = Position - listenerPosition;
            toSource = toSource.Normalize();
            Vector3D listenerDir = listenerDirection.Normalize();
            float dotProduct = Vector3D.Dot(toSource, listenerDir);

            // Calculate pan (-1.0 for full left, 1.0 for full right)
            float pan = MathHelper.Clamp(dotProduct, -1f, 1f);

            stereoProvider.SetVolume(calculatedVolume);
            stereoProvider.SetPan(pan);
        }

        private float CalculateVolume(float distance)
        {
            // Ensure distance is within range
            distance = Math.Max(MinDistance, Math.Min(MaxDistance, distance));

            // Apply a logarithmic scale to the volume for smoother transitions
            float normalizedDistance = (distance - MinDistance) / (MaxDistance - MinDistance);
            float volumeScale = 1 - MathHelper.Clamp(normalizedDistance, 0, 1);
            return Volume * volumeScale;
        }
    }

    public static class MathHelper
    {
        public static float Clamp(float value, float min, float max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }
    }
}
