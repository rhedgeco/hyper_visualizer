using System;
using System.Collections.Generic;
using System.IO;
using HyperScripts.Managers;
using NAudio.Wave;
using UnityEngine.Events;

namespace HyperScripts.Threading
{
    public class AudioDecodeWorker : GeneralThreadWorker
    {
        private string path;
        private string mp3ConvertPath;
        private string audioName;
        private float[] samples = new float[0];
        private int channels = 0;
        private int sampleRate = 0;

        private AudioDecodedEvent decodedEvent = new AudioDecodedEvent();

        public class AudioDecodedEvent : UnityEvent<float[], int, int>
        {
        }

        public AudioDecodeWorker(string path, string mp3ConvertPath, UnityAction<float[], int, int> sampleCallback)
        {
            this.path = path;
            this.mp3ConvertPath = mp3ConvertPath;
            decodedEvent.AddListener(sampleCallback);

            audioName = Path.GetFileNameWithoutExtension(path);
        }

        #region ThreadCallbacks

        internal override void ThreadCallbackStart()
        {
            AudioManager.PurgeFftCache();
            OverlayManager.Loading.StartLoading($"Loading Audio File\n{audioName}");
        }

        protected override void ThreadCallbackUpdate()
        {
            OverlayManager.Loading.UpdateLoading(StateMessage, StateProgress);
        }

        protected override void ThreadCallbackError()
        {
            OverlayManager.Loading.EndLoading();
        }

        protected override void ThreadCallbackClosed()
        {
            OverlayManager.Loading.EndLoading();
            decodedEvent.Invoke(samples, channels, sampleRate);
        }

        #endregion

        #region ThreadDecoding

        protected override void ThreadBody()
        {
            if (!File.Exists(path))
            {
                throw new Exception($"File '{path}' could not be found.");
            }
            
            WaveFileReader reader;
            if (Path.HasExtension(path) &&
                // ReSharper disable once PossibleNullReferenceException
                Path.GetExtension(path).Equals(".mp3", StringComparison.InvariantCultureIgnoreCase))
            {
                StateMessage = "Converting mp3 to wav...";
                reader = GetWavReaderFromMp3();
                if (reader == null)
                {
                    throw new Exception("Error converting file");
                }
            }
            else if (Path.HasExtension(path) &&
                     // ReSharper disable once PossibleNullReferenceException
                     Path.GetExtension(path).Equals(".wav", StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {
                    reader = new WaveFileReader(path);
                }
                catch (Exception e)
                {
                    throw new Exception($"Error loading file: {e.Message}");
                }
            }
            else
            {
                throw new Exception("Unsupported file extension.");
            }

            if (!reader.CanRead)
            {
                throw new Exception("Cannot read audio file.");
            }

            StateMessage = $"Reading wav file\n{audioName}";

            List<float> sampleList = new List<float>();
            float[] sampleFrame;
            while ((sampleFrame = reader.ReadNextSampleFrame()) != null)
            {
                sampleList.AddRange(sampleFrame);
                float percent = (float) reader.Position / reader.Length;
                StateProgress = percent;
            }

            float[] sampleArray = sampleList.ToArray();

            samples = sampleArray;
            channels = reader.WaveFormat.Channels;
            sampleRate = reader.WaveFormat.SampleRate;
        }

        private WaveFileReader GetWavReaderFromMp3()
        {
            try
            {
                if (!Directory.Exists(mp3ConvertPath)) Directory.CreateDirectory(mp3ConvertPath);
                string outPath = Path.Combine(mp3ConvertPath,
                    Path.GetFileName(path) ?? throw new NullReferenceException());
                if (File.Exists(outPath)) File.Delete(outPath);

                StateMessage = "Creating wav file from mp3...";
                using (var reader = new Mp3FileReader(path))
                {
                    WaveFileWriter.CreateWaveFile(outPath, reader);
                }

                WaveFileReader wavReader = new WaveFileReader(outPath);
                StateMessage = "Converted mp3 to wav...";
                return wavReader;
            }
            catch (Exception e)
            {
                throw new Exception($"Error converting mp3 file: {e.Message}");
            }
        }

        #endregion
    }
}