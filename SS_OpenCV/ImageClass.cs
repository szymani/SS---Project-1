using System;
using System.Collections.Generic;
using System.Text;
using Emgu.CV.Structure;
using Emgu.CV;

namespace SS_OpenCV
{
    class PixelOrder
    {
        public int[][] center;

        public int[][] upperLeft;
        public int[][] upperRight;
        public int[][] lowerLeft;
        public int[][] lowerRight;

        public int[][] top;
        public int[][] bottom;
        public int[][] left;
        public int[][] right;

        public PixelOrder()
        {
            center = new int[][] { new int[] { -1, -1 }, new int[] { 0, -1 }, new int[] { 1, -1 }, new int[] { -1, 0 }, new int[] { 0, 0 }, new int[] { 1, 0 }, new int[] { -1, 1 }, new int[] { 0, 1 }, new int[] { 1, 1 } };

            upperLeft = new int[][] { new int[] { 0, 0 }, new int[] { 0, 0 }, new int[] { 0, 0 }, new int[] { 0, 0 }, new int[] { 0, 1 }, new int[] { 0, 1 }, new int[] { 1, 0 }, new int[] { 1, 0 }, new int[] { 0, 0 } };
            upperRight = new int[][] { new int[] { 0, 0 }, new int[] { 0, 0 }, new int[] { 0, 1 }, new int[] { 0, 1 }, new int[] { 1, 0 }, new int[] { 1, 0 }, new int[] { 1, 0 }, new int[] { 1, 0 }, new int[] { 1, 1 } };
            lowerLeft = new int[][] { new int[] { 0, 0 }, new int[] { 0, 0 }, new int[] { 1, 0 }, new int[] { 0, 1 }, new int[] { 0, 1 }, new int[] { 0, 1 }, new int[] { 0, 1 }, new int[] { 1, 1 }, new int[] { 1, 1 } };
            lowerRight = new int[][] { new int[] { 0, 0 }, new int[] { 1, 0 }, new int[] { 1, 0 }, new int[] { 0, 1 }, new int[] { 0, 1 }, new int[] { 1, 1 }, new int[] { 1, 1 }, new int[] { 1, 1 }, new int[] { 1, 1 } };

            top = new int[][] { new int[] { -1, 0 }, new int[] { 0, 0 }, new int[] { 1, 0 }, new int[] { -1, 0 }, new int[] { 0, 0 }, new int[] { 1, 0 }, new int[] { -1, 1 }, new int[] { 0, 1 }, new int[] { 1, 1 } };
            bottom = new int[][] { new int[] { -1, -1 }, new int[] { 0, -1 }, new int[] { 1, -1 }, new int[] { -1, 0 }, new int[] { -1, 0 }, new int[] { 0, 0 }, new int[] { 0, 0 }, new int[] { 1, 0 }, new int[] { 1, 0 } };
            left = new int[][] { new int[] { 0, -1 }, new int[] { 0, -1 }, new int[] { 1, -1 }, new int[] { 0, 0 }, new int[] { 0, 0 }, new int[] { 0, 1 }, new int[] { 1, 0 }, new int[] { 1, 0 }, new int[] { 1, 1 } };
            right = new int[][] { new int[] { -1, -1 }, new int[] { 0, -1 }, new int[] { 0, -1 }, new int[] { -1, 0 }, new int[] { 0, 0 }, new int[] { 0, 0 }, new int[] { -1, 1 }, new int[] { 0, 1 }, new int[] { 0, 1 } };
        }
    }

    class ImageClass
    {

        /// <summary>
        /// Image Negative using EmguCV library
        /// Slower method
        /// </summary>
        /// <param name="img">Image</param>
        public static void Negative(Image<Bgr, byte> img)
        {
            int x, y;

            Bgr aux;
            for (y = 0; y < img.Height; y++)
            {
                for (x = 0; x < img.Width; x++)
                {
                    // acesso directo : mais lento 
                    aux = img[y, x];
                    img[y, x] = new Bgr(255 - aux.Blue, 255 - aux.Green, 255 - aux.Red);
                }
            }
        }

        /// <summary>
        /// Convert to gray
        /// Direct access to memory - faster method
        /// </summary>
        /// <param name="img">image</param>
        public static void ConvertToGray(Image<Bgr, byte> img)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
                byte blue, green, red, gray;

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;

                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            //retrive 3 colour components
                            blue = dataPtr[0];
                            green = dataPtr[1];
                            red = dataPtr[2];

                            // convert to gray
                            gray = (byte)Math.Round(((int)blue + green + red) / 3.0);

                            // store in the image
                            dataPtr[0] = gray;
                            dataPtr[1] = gray;
                            dataPtr[2] = gray;

                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                        }

                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtr += padding;
                    }
                }
            }
        }

        /// <summary>
        /// Convert to gray
        /// Direct access to memory - faster method
        /// </summary>
        /// <param name="img">image</param>
        public static void Translation(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, int dx, int dy)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image

                MIplImage m2 = imgCopy.MIplImage;
                byte* dataPtr2 = (byte*)m2.imageData.ToPointer(); // Pointer to the image

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;

                //dataPtr2 -= dx;

                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            var x0 = x - dx;
                            var y0 = y - dy;

                            if(x0>=0&& y0>=0 &&x0<width && y0<height)
                            {
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
        public static void Rotation(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float angle)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image

                MIplImage m2 = imgCopy.MIplImage;
                byte* dataPtr2 = (byte*)m2.imageData.ToPointer(); // Pointer to the image

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;

                //dataPtr2 -= dx;

                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            var x0 = (byte)Math.Round((x - width / 2) * Math.Cos(angle) - (height / 2 - y) * Math.Sin(angle) + width / 2);
                            var y0 = (byte)Math.Round(height /2 - (x - width / 2) * Math.Sin(angle) - (height / 2 - y) * Math.Cos(angle));

                            if (Math.Abs(x0-x)<=height && Math.Abs(y0 - y) <= height)
                            {
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

        public static void Mean(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            unsafe
            {
                // direct access to the image memory(sequencial
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image

                MIplImage m2 = imgCopy.MIplImage;
                byte* dataPtr2 = (byte*)m2.imageData.ToPointer(); // Pointer to the image

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y, ct;
                var meanB = 0;
                var meanG = 0;
                var meanR = 0;

                PixelOrder order = new PixelOrder();
                float[] filter = {1,1,1,1,1,1,1,1,1 };

                for (x = 1; x < width - 1; x++)
                {
                    for (y = 1; y < height - 1; y++)
                    {
                        ct = 0;
                        meanB = 0;
                        meanG = 0;
                        meanR = 0;

                        foreach (int[] coor in order.center)
                        {
                            meanB += (byte)(filter[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[0]);
                            meanG += (byte)(filter[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[1]);
                            meanR += (byte)(filter[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[2]);
                            ct++;
                        }

                        (dataPtr + y * m.widthStep + x * nChan)[0] = (byte)(int)Math.Round(meanB / 9.0);
                        (dataPtr + y * m.widthStep + x * nChan)[1] = (byte)(int)Math.Round(meanG / 9.0);
                        (dataPtr + y * m.widthStep + x * nChan)[2] = (byte)(int)Math.Round(meanR / 9.0);
                    }               
                }

                int[][] pixOrd = new int[][] { };
                for (int k = 0; k < 4; k++)
                {
                    x = 0;
                    y = 0;
                    switch (k)
                    {
                        case 0:     //Upper left corner
                            pixOrd = order.upperLeft;
                            x = 0;
                            y = 0;
                            break;
                        case 1:     //Upper right
                            pixOrd = order.upperRight;
                            x = width - 2;
                            y = 0;
                            break;
                        case 2:     //Lower left
                            pixOrd = order.lowerLeft;
                            x = 0;
                            y = height - 2;
                            break;
                        case 3:     //Lower right
                            pixOrd = order.lowerRight;
                            x = width - 2;
                            y = height - 2;
                            break;
                        default:
                            break;
                    }

                    ct = 0;
                    meanB = 0;
                    meanG = 0;
                    meanR = 0;

                    foreach (int[] coor in pixOrd)
                    {
                        meanB += (byte)(filter[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[0]);
                        meanG += (byte)(filter[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[1]);
                        meanR += (byte)(filter[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[2]);
                        ct++;
                    }

                    (dataPtr + y * m.widthStep + x * nChan)[0] = (byte)(int)Math.Round(meanB / 9.0);
                    (dataPtr + y * m.widthStep + x * nChan)[1] = (byte)(int)Math.Round(meanG / 9.0);
                    (dataPtr + y * m.widthStep + x * nChan)[2] = (byte)(int)Math.Round(meanR / 9.0);
                }


                //first and last row, without corners
                for (x = 1; x < width - 1; x++)
                {
                    y = 0;      //First row
                    ct = 0;
                    meanB = 0;
                    meanG = 0;
                    meanR = 0;

                    foreach (int[] coor in order.top)
                    {
                        meanB += (byte)(filter[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[0]);
                        meanG += (byte)(filter[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[1]);
                        meanR += (byte)(filter[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[2]);
                        ct++;
                    }

                    (dataPtr + y * m.widthStep + x * nChan)[0] = (byte)(int)Math.Round(meanB / 9.0);
                    (dataPtr + y * m.widthStep + x * nChan)[1] = (byte)(int)Math.Round(meanG / 9.0);
                    (dataPtr + y * m.widthStep + x * nChan)[2] = (byte)(int)Math.Round(meanR / 9.0);

                    y = height - 2;     //Last row
                    ct = 0;
                    meanB = 0;
                    meanG = 0;
                    meanR = 0;

                    foreach (int[] coor in order.bottom)
                    {
                        meanB += (byte)(filter[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[0]);
                        meanG += (byte)(filter[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[1]);
                        meanR += (byte)(filter[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[2]);
                        ct++;
                    }

                    (dataPtr + y * m.widthStep + x * nChan)[0] = (byte)(int)Math.Round(meanB / 9.0);
                    (dataPtr + y * m.widthStep + x * nChan)[1] = (byte)(int)Math.Round(meanG / 9.0);
                    (dataPtr + y * m.widthStep + x * nChan)[2] = (byte)(int)Math.Round(meanR / 9.0);
                }

                //first and last column, without corners
                for (y = 1; y < height - 1; y++)
                {
                    x = 0;      //First column
                    ct = 0;
                    meanB = 0;
                    meanG = 0;
                    meanR = 0;

                    foreach (int[] coor in order.left)
                    {
                        meanB += (byte)(filter[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[0]);
                        meanG += (byte)(filter[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[1]);
                        meanR += (byte)(filter[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[2]);
                        ct++;
                    }

                    (dataPtr + y * m.widthStep + x * nChan)[0] = (byte)(int)Math.Round(meanB / 9.0);
                    (dataPtr + y * m.widthStep + x * nChan)[1] = (byte)(int)Math.Round(meanG / 9.0);
                    (dataPtr + y * m.widthStep + x * nChan)[2] = (byte)(int)Math.Round(meanR / 9.0);

                    x = width - 1;     //Last column
                    ct = 0;
                    meanB = 0;
                    meanG = 0;
                    meanR = 0;

                    foreach (int[] coor in order.right)
                    {
                        meanB += (byte)(filter[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[0]);
                        meanG += (byte)(filter[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[1]);
                        meanR += (byte)(filter[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[2]);
                        ct++;
                    }

                    (dataPtr + y * m.widthStep + x * nChan)[0] = (byte)(int)Math.Round(meanB / 9.0);
                    (dataPtr + y * m.widthStep + x * nChan)[1] = (byte)(int)Math.Round(meanG / 9.0);
                    (dataPtr + y * m.widthStep + x * nChan)[2] = (byte)(int)Math.Round(meanR / 9.0);
                }
            }
        }


        public static void NonUniform(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float[,] matrix, float matrixWeight)
        {
            try
            {
                unsafe
                {
                    // direct access to the image memory(sequencial
                    // direcion top left -> bottom right

                    MIplImage m = img.MIplImage;
                    byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image

                    MIplImage m2 = imgCopy.MIplImage;
                    byte* dataPtr2 = (byte*)m2.imageData.ToPointer(); // Pointer to the image

                    int width = img.Width;
                    int height = img.Height;
                    int nChan = m.nChannels; // number of channels - 3
                    int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                    int x, y;
                    var meanB = 0;
                    var meanG = 0;
                    var meanR = 0;

                    for (y = 1; y < height - 1; y++)
                    {
                        for (x = 1; x < width - 1; x++)
                        {
                            meanB = 0;
                            meanG = 0;
                            meanR = 0;
                            for (int i = -1; i < 2; i++)
                            {
                                for (int j = -1; j < 2; j++)
                                {
                                    meanB += (byte)(int)Math.Round(matrix[i, j] * matrixWeight * (dataPtr2 + (y + j) * m.widthStep + (x + i) * nChan)[0]);
                                    meanG += (byte)(int)Math.Round(matrix[i, j] * matrixWeight * (dataPtr2 + (y + j) * m.widthStep + (x + i) * nChan)[1]);
                                    meanR += (byte)(int)Math.Round(matrix[i, j] * matrixWeight * (dataPtr2 + (y + j) * m.widthStep + (x + i) * nChan)[2]);
                                }
                            }
                            (dataPtr + y * m.widthStep + x * nChan)[0] = (byte)(int)Math.Round(meanB / 9.0);
                            (dataPtr + y * m.widthStep + x * nChan)[1] = (byte)(int)Math.Round(meanG / 9.0);
                            (dataPtr + y * m.widthStep + x * nChan)[2] = (byte)(int)Math.Round(meanR / 9.0);
                        }
                    }

                }
            }
            catch { }
        
        }
       public static void Sobel(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
       {
            unsafe
            {
                // direct access to the image memory(sequencial
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image

                MIplImage m2 = imgCopy.MIplImage;
                byte* dataPtr2 = (byte*)m2.imageData.ToPointer(); // Pointer to the image

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;
                var meanB = 0;
                var meanG = 0;
                var meanR = 0;
                int meanBx = 0;
                int meanGx = 0;
                int meanRx = 0;
                int meanBy = 0;
                int meanGy = 0;
                int meanRy = 0;

                var tablex = new[] { 1, 0, -1, 2, 0, -2, 1, 0, -1 };
                var tabley = new[] { -1, -2, -1, 0, 0, 0, 1, 2, 1 };

                for (y = 1; y < height - 1; y++)
                {
                    for (x = 1; x < width - 1; x++)
                    {
                        meanB = 0;
                        meanG = 0;
                        meanR = 0;
                        meanBx = 0;
                        meanGx = 0;
                        meanRx = 0;
                        meanBy = 0;
                        meanGy = 0;
                        meanRy = 0;

                        int index = 0;
                        for (int i = -1; i < 2; i++)
                        {
                            for (int j = -1; j < 2; j++, index++)
                            {
                                meanBx += tablex[index] * ((dataPtr2 + (y + j) * m.widthStep + (x + i) * nChan))[0];
                                meanGx += tablex[index] * ((dataPtr2 + (y + j) * m.widthStep + (x + i) * nChan))[1];
                                meanRx += tablex[index] * ((dataPtr2 + (y + j) * m.widthStep + (x + i) * nChan))[2];

                                meanBy += tabley[index] * ((dataPtr2 + (y + j) * m.widthStep + (x + i) * nChan))[0];
                                meanGy += tabley[index] * ((dataPtr2 + (y + j) * m.widthStep + (x + i) * nChan))[1];
                                meanRy += tabley[index] * ((dataPtr2 + (y + j) * m.widthStep + (x + i) * nChan))[2];
                            }
                        }
                        meanB = Math.Min(Math.Abs(meanBx), 255) + Math.Min(Math.Abs(meanBy), 255);
                        meanG = Math.Min(Math.Abs(meanGx), 255) + Math.Min(Math.Abs(meanGy), 255);
                        meanR = Math.Min(Math.Abs(meanRx), 255) + Math.Min(Math.Abs(meanRy), 255);

                        (dataPtr + y * m.widthStep + x * nChan)[0] = (byte)meanB;
                        (dataPtr + y * m.widthStep + x * nChan)[1] = (byte)meanG;
                        (dataPtr + y * m.widthStep + x * nChan)[2] = (byte)meanR;
                    }
                }
            }
        }
        public static void Diferentiation(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            unsafe
            {
                // direct access to the image memory(sequencial
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image

                MIplImage m2 = imgCopy.MIplImage;
                byte* dataPtr2 = (byte*)m2.imageData.ToPointer(); // Pointer to the image

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;
                var meanB = 0;
                var meanG = 0;
                var meanR = 0;
                int meanBx = 0;
                int meanGx = 0;
                int meanRx = 0;
                int meanBy = 0;
                int meanGy = 0;
                int meanRy = 0;

                var tablex = new[] { 1, 0, 0, 0, -1, 0, 0, 0, 0 };
                var tabley = new[] { 0, 1, 0, -1, 0, 0, 0, 0, 0 };

                for (y = 1; y < height - 1; y++)
                {
                    for (x = 1; x < width - 1; x++)
                    {
                        meanB = 0;
                        meanG = 0;
                        meanR = 0;
                        meanBx = 0;
                        meanGx = 0;
                        meanRx = 0;
                        meanBy = 0;
                        meanGy = 0;
                        meanRy = 0;

                        int index = 0;
                        for (int i = -1; i < 2; i++)
                        {
                            for (int j = -1; j < 2; j++, index++)
                            {
                                meanBx += tablex[index] *((dataPtr2 + (y + j) * m.widthStep + (x + i) * nChan))[0];
                                meanGx += tablex[index] * ((dataPtr2 + (y + j) * m.widthStep + (x + i) * nChan))[1];
                                meanRx += tablex[index] * ((dataPtr2 + (y + j) * m.widthStep + (x + i) * nChan))[2];

                                meanBy += tabley[index] * ((dataPtr2 + (y + j) * m.widthStep + (x + i) * nChan))[0];
                                meanGy += tabley[index] * ((dataPtr2 + (y + j) * m.widthStep + (x + i) * nChan))[1];
                                meanRy += tabley[index] * ((dataPtr2 + (y + j) * m.widthStep + (x + i) * nChan))[2];
                            }
                        }
                        meanB = Math.Min(Math.Abs(meanBx), 255) + Math.Min(Math.Abs(meanBy), 255);
                        meanG = Math.Min(Math.Abs(meanGx), 255) + Math.Min(Math.Abs(meanGy), 255);
                        meanR = Math.Min(Math.Abs(meanRx), 255) + Math.Min(Math.Abs(meanRy), 255);

                        (dataPtr + y * m.widthStep + x * nChan)[0] = (byte)meanB;
                        (dataPtr + y * m.widthStep + x * nChan)[1] = (byte)meanG;
                        (dataPtr + y * m.widthStep + x * nChan)[2] = (byte)meanR;
                    }
                }
            }
        }
        public static void Median(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            img = imgCopy.SmoothMedian(3);
        }
        public static int[] Histogram_Gray(Emgu.CV.Image<Bgr, byte> img)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
                byte blue, green, red, gray;

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;
                int []histArray = new int[256];

                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            //retrive 3 colour components
                            blue = dataPtr[0];
                            green = dataPtr[1];
                            red = dataPtr[2];

                            // convert to gray
                            gray = (byte)Math.Round(((int)blue + green + red) / 3.0);

                            histArray[gray]++;

                            // store in the image
                            dataPtr[0] = gray;
                            dataPtr[1] = gray;
                            dataPtr[2] = gray;
                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                        }
                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtr += padding;
                    }
                }
                return histArray;
            }         
        }
        public static int[,] Histogram_RGB(Emgu.CV.Image<Bgr, byte> img)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
                byte blue, green, red, gray;

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;
                int[,] histArray = new int[3,256];

                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            //retrive 3 colour components
                            blue = dataPtr[0];
                            green = dataPtr[1];
                            red = dataPtr[2];

                            histArray[0, red]++;
                            histArray[1, green]++;
                            histArray[2, blue]++;

                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                        }
                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtr += padding;
                    }
                }
                return histArray;
            }
        }
    }
}



