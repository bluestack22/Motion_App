using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace Motion_App.Service
{
    public interface IDataSource
    {
        bool Open();

        void Start();

        void Stop();

        bool IsRunning { get; }

        event Action<Mat> FrameReady;
    }
}
