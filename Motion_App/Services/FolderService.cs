using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Motion_App.Service
{
    public class FolderService : IDataSource
    {
        private readonly List<string> _imagePaths;

        private int _index = 0;

        private CancellationTokenSource? _cts;

        private Task? _playTask;

        public bool IsRunning { get; private set; }

        public event Action<Mat>? FrameReady;

        // Delay giữa các frame (ms)
        public int FrameDelay { get; set; } = 100;

        public FolderService(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                _imagePaths = new List<string>();
                return;
            }

            string[] validExtensions =
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".bmp",
                ".tif",
                ".tiff"
            };

            _imagePaths = Directory
                .GetFiles(folderPath)
                .Where(f => validExtensions.Contains(
                    Path.GetExtension(f).ToLower()))
                .ToList();
        }

        // =========================
        // OPEN
        // =========================
        public bool Open()
        {
            return _imagePaths.Count > 0;
        }

        // =========================
        // START
        // =========================
        public void Start()
        {
            if (IsRunning)
                return;

            if (!Open())
                return;

            _cts = new CancellationTokenSource();

            IsRunning = true;

            _playTask = Task.Run(() => PlaybackLoop(_cts.Token));
        }

        // =========================
        // PLAYBACK LOOP
        // =========================
        private void PlaybackLoop(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    if (_imagePaths.Count == 0)
                    {
                        Thread.Sleep(100);
                        continue;
                    }

                    string path = _imagePaths[_index];

                    _index++;

                    if (_index >= _imagePaths.Count)
                        _index = 0;

                    try
                    {
                        using var mat = Cv2.ImRead(path);

                        if (mat.Empty())
                            continue;

                        // Clone trước khi emit
                        FrameReady?.Invoke(mat.Clone());
                    }
                    catch
                    {
                    }

                    Thread.Sleep(FrameDelay);
                }
            }
            catch
            {
            }
            finally
            {
                IsRunning = false;
            }
        }

        // =========================
        // STOP
        // =========================
        public void Stop()
        {
            try
            {
                _cts?.Cancel();

                _playTask?.Wait(500);
            }
            catch
            {
            }

            IsRunning = false;

            _cts?.Dispose();
            _cts = null;
        }

        // =========================
        // DISPOSE
        // =========================
        public void Dispose()
        {
            Stop();
        }
    }
}