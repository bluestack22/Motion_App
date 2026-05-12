using OpenCvSharp;
using System;

namespace Motion_App.Logic
{
    public class Vision_BLL
    {
        /// <summary>
        /// Apply Gaussian Blur to the image.
        /// </summary>
        public static Mat ApplyBlur(Mat input, int blurValue)
        {
            if (blurValue <= 0) return input.Clone();
            
            // Kernel size must be odd and positive
            int kSize = blurValue;
            if (kSize % 2 == 0) kSize++;
            
            Mat output = new Mat();
            Cv2.GaussianBlur(input, output, new Size(kSize, kSize), 0);
            return output;
        }

        /// <summary>
        /// Apply Canny Edge Detection.
        /// </summary>
        public static Mat ApplyCanny(Mat input, int lowThreshold, int highThreshold)
        {
            Mat gray = new Mat();
            if (input.Channels() > 1)
                Cv2.CvtColor(input, gray, ColorConversionCodes.BGR2GRAY);
            else
                gray = input.Clone();

            Mat edges = new Mat();
            Cv2.Canny(gray, edges, lowThreshold, highThreshold);
            gray.Dispose();
            
            // Convert back to BGR so it can be displayed consistently if needed, 
            // or just return the single channel mask. 
            // Usually, WPF Image likes BGR or Gray.
            return edges;
        }

        /// <summary>
        /// Apply Binary Threshold.
        /// </summary>
        public static Mat ApplyThreshold(Mat input, int thresholdValue)
        {
            Mat gray = new Mat();
            if (input.Channels() > 1)
                Cv2.CvtColor(input, gray, ColorConversionCodes.BGR2GRAY);
            else
                gray = input.Clone();

            Mat binary = new Mat();
            Cv2.Threshold(gray, binary, thresholdValue, 255, ThresholdTypes.Binary);
            gray.Dispose();
            return binary;
        }

        /// <summary>
        /// Full processing pipeline based on selected mode and parameters.
        /// </summary>
        public static Mat Process(Mat input, string mode, int threshold, int blur, int cannyLow, int cannyHigh, int kernelSize)
        {
            if (input == null || input.Empty()) return new Mat();

            switch (mode)
            {
                case "Blur":
                    return ApplyBlur(input, blur > 0 ? blur : kernelSize);
                case "Edges":
                    return ApplyCanny(input, cannyLow, cannyHigh);
                case "Threshold":
                    return ApplyThreshold(input, threshold);
                default:
                    return input.Clone();
            }
        }
    }
}
