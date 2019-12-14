using System;
using System.Collections.Generic;
using System.Text;
using Emgu.CV.Structure;
using Emgu.CV;


namespace SS_OpenCV
{
    class Identify
    {
        public static void BgrToHsv(Image<Bgr, byte> img, Image<Hsv, byte> imgCopy)
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

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        var x0 = x;
                        var y0 = y;

                        int blue = (dataPtr + y * m.widthStep + x * nChan)[0];
                        int green = (dataPtr + y * m.widthStep + x * nChan)[1];
                        int red = (dataPtr + y * m.widthStep + x * nChan)[2];

                        System.Drawing.Color intermediate = System.Drawing.Color.FromArgb(red, green, blue);                        
                        Hsv hsvPixel = new Hsv(intermediate.GetHue(), intermediate.GetSaturation(), intermediate.GetBrightness());

                        double r = red/255.0;
                        double g = green/255.0;
                        double b = blue/255.0;
                        double hue, sat, val;

                        double cmax = Math.Max(r,b);
                        cmax = Math.Max(cmax,g);
                        double cmin = Math.Min(r, b);
                        cmin = Math.Min(cmin, g);
                        double delta = cmax - cmin;

                        //Hue
                        hue = hsvPixel.Hue;

                        //Saturation
                        if (cmax == 0)
                        {
                            sat = 0;
                        }
                        else
                        {
                            sat = delta / cmax;
                        }

                        //Value
                        val = cmax;
                        
                        (dataPtr2 + y * m.widthStep + x * nChan)[0] = (byte)(Math.Round(val*255));
                        (dataPtr2 + y * m.widthStep + x * nChan)[1] = (byte)(Math.Round(sat*255));
                        (dataPtr2 + y * m.widthStep + x * nChan)[2] = (byte)(Math.Round(hue*255/360));
                        
                    }
                }
            }
        }

        public static void DrawRectangle(Image<Bgr, byte> img, int[] sign_coords)
        {
            unsafe
            {
                int x, y;
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width;
                //int[] sign_coords = new int[4];
                /*
                sign_coords[0] = Left-x
                sign_coords[1] = Top-y
                sign_coords[2] = Right-x
                sign_coords[3] = Bottom-y
                */
                /*
                sign_coords[0] = 200;
                sign_coords[1] = 300;
                sign_coords[2] = 400;
                sign_coords[3] = 500;
                */
                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        //Drawing rectangle
                        if (
                            x > sign_coords[0] && x < sign_coords[2] && (y == sign_coords[1] || y == sign_coords[3]) || //vertical lines
                            y > sign_coords[1] && y < sign_coords[3] && (x == sign_coords[0] || x == sign_coords[2])    //horizontal lines
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


        public static void Scale(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float scaleFactor)
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
                // acesso directo : mais lento 
                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        // get pixel address
                        //blue = (byte)(dataPtr + y * widthstep + x * nC)[0];
                        var x0 = (int)Math.Round(x / scaleFactor);
                        var y0 = (int)Math.Round(y / scaleFactor);

                        if (x0 >= 0 && y0 >= 0 && x0 < width && y0 < height)
                        {
                            // get pixel address
                            (dataPtr + y * m.widthStep + x * nChan)[0] = (dataPtr2 + y0 * m.widthStep + x0 * nChan)[0];
                            (dataPtr + y * m.widthStep + x * nChan)[1] = (dataPtr2 + y0 * m.widthStep + x0 * nChan)[1];
                            (dataPtr + y * m.widthStep + x * nChan)[2] = (dataPtr2 + y0 * m.widthStep + x0 * nChan)[2];
                        }
                        else
                        {
                            (dataPtr + y * m.widthStep + x * nChan)[0] = 0;
                            (dataPtr + y * m.widthStep + x * nChan)[1] = 0;
                            (dataPtr + y * m.widthStep + x * nChan)[2] = 0;
                        }
                    }
                }
            }
        }

    }
}
