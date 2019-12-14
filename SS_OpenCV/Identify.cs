using System;
using System.Collections.Generic;
using System.Text;
using Emgu.CV.Structure;
using Emgu.CV;


namespace SS_OpenCV
{
    class Identify
    {
        public static void SignIdentify(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            unsafe
            {
                int x, y;
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image

                MIplImage m2 = imgCopy.MIplImage;
                byte* dataPtr2 = (byte*)m2.imageData.ToPointer(); // Pointer to the image

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width;
                int[] sign_coords = new int[4];
                /*
                sign_coords[0] = Left-x
                sign_coords[1] = Top-y
                sign_coords[2] = Right-x
                sign_coords[3] = Bottom-y
                */

                sign_coords[0] = 200;
                sign_coords[1] = 300;
                sign_coords[2] = 400;
                sign_coords[3] = 500;

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        var x0 = x;
                        var y0 = y;

                        if (
                            //x0 > 200 && x0 < 400 && (y0 == 300 || y0 == 500) || //vertical lines
                            //y0 > 300 && y0 < 500 && (x0 == 200 || x0 == 400)    //horizontal lines
                            x0 > sign_coords[0] && x0 < sign_coords[2] && (y0 == sign_coords[1] || y0 == sign_coords[3]) || //vertical lines
                            y0 > sign_coords[1] && y0 < sign_coords[3] && (x0 == sign_coords[0] || x0 == sign_coords[2])    //horizontal lines
                            )
                        {
                            // get pixel address
                            (dataPtr + y * m.widthStep + x * nChan)[0] = 0;
                            (dataPtr + y * m.widthStep + x * nChan)[1] = 0;
                            (dataPtr + y * m.widthStep + x * nChan)[2] = 255;
                        }
                    }
                }
            }
        }

    }
}
