using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Runtime.InteropServices;

namespace ConsoleAppDll_client
{
    internal class Program
    {
        const int Width = 640;
        const int Height = 480;

        [DllImport("TensorFlowLiteDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Initialize();

        [DllImport("TensorFlowLiteDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CaptureFrameAndSegment(byte[] buffer, int width, int height);

        [DllImport("TensorFlowLiteDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Release();

        static void Main(string[] args)
        {
            int result = Initialize();
            if (result != 0)
            {
                Console.WriteLine($"Error initializing: {result}");
                return;
            }

            byte[] buffer = new byte[Width * Height];

            while (true)
            {
                result = CaptureFrameAndSegment(buffer, Width, Height);
                if (result == 0)
                {
                    using (Mat mat = new Mat(Height, Width, DepthType.Cv8U, 1))
                    {
                        Marshal.Copy(buffer, 0, mat.DataPointer, buffer.Length);

                        Image<Bgr, byte> image = mat.ToImage<Bgr, byte>();

                        CvInvoke.Imshow("Live Segmentation Stream", image);
                        if (CvInvoke.WaitKey(1) == 27)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Error capturing frame: {result}");
                }
            }

            Release();
            
        }

    }
}
