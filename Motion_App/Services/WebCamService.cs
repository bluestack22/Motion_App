using OpenCvSharp;
using System;
using System.Threading;
using System.Threading.Tasks;
using Motion_App.ViewModels;

namespace Motion_App.Service
{
    public class WebCamService : IDataSource
    {
        private VideoCapture? _capture;
        private CancellationTokenSource? _cts;
        private Task? _captureTask;

        private int _cameraIndex = 0;

        public bool IsRunning { get; private set; }

        public event Action<Mat>? FrameReady;

        // Singleton
        private static readonly Lazy<WebCamService> _instance =
            new(() => new WebCamService());

        public static WebCamService Instance => _instance.Value;

        private WebCamService()
        {
        }
        // =========================
        // SCAN CAMERAS
        public List<SelectionItem<int>> ScanCameras(int maxTest = 10)
        {
            List<SelectionItem<int>> cameras = new();

            for (int i = 0; i < maxTest; i++)
            {
                try
                {
                    using var cap = new VideoCapture(i);

                    if (cap.IsOpened())
                    {
                        cameras.Add(new SelectionItem<int>
                        {
                            Value = i,
                            Display = $"Camera {i}"
                        });

                        cap.Release();
                    }
                }
                catch
                {
                }
            }

            return cameras;
        }

        // =========================
        // OPEN
        // =========================
        public bool Open()
        {
            return Open(0);
        }

        public bool Open(int index)
        {
            try
            {
                _cameraIndex = index;

                _capture?.Release();
                _capture?.Dispose();

                _capture = new VideoCapture(index);

                if (!_capture.IsOpened())
                    return false;

                // OPTIONAL:
                // giảm latency
                _capture.Set(VideoCaptureProperties.BufferSize, 1);

                return true;
            }
            catch
            {
                return false;
            }
        }

        // =========================
        // START
        // =========================
        public void Start()
        {
            if (IsRunning)
                return;

            if (_capture == null || !_capture.IsOpened())
            {
                if (!Open(_cameraIndex))
                    return;
            }

            _cts = new CancellationTokenSource();

            IsRunning = true;

            _captureTask = Task.Run(() => CaptureLoop(_cts.Token));
        }

        public void Start(int index)
        {
            Stop();

            if (!Open(index))
                return;

            Start();
        }

        // =========================
        // LOOP
        // =========================
        private void CaptureLoop(CancellationToken token)
        {
            try
            {
                using var frame = new Mat();

                while (!token.IsCancellationRequested &&
                       _capture != null &&
                       _capture.IsOpened())
                {
                    bool success = _capture.Read(frame);

                    if (!success || frame.Empty())
                    {
                        Thread.Sleep(5);
                        continue;
                    }

                    try
                    {
                        // Clone cực kỳ quan trọng
                        // tránh race condition
                        FrameReady?.Invoke(frame.Clone());
                    }
                    catch
                    {
                    }

                    // giảm CPU usage
                    Thread.Sleep(1);
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

                _captureTask?.Wait(500);
            }
            catch
            {
            }

            IsRunning = false;

            _cts?.Dispose();
            _cts = null;

            try
            {
                if (_capture != null)
                {
                    if (_capture.IsOpened())
                        _capture.Release();

                    _capture.Dispose();
                    _capture = null;
                }
            }
            catch
            {
            }
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