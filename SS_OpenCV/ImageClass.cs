using System;
using System.Collections.Generic;
using System.Text;
using Emgu.CV.Structure;
using Emgu.CV;
using System.Linq;

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

                            if (x0 >= 0 && y0 >= 0 && x0 < width && y0 < height)
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
                            var x0 = (int)Math.Round((x - width / 2) * Math.Cos(angle) - (height / 2 - y) * Math.Sin(angle) + width / 2);
                            var y0 = (int)Math.Round(height / 2 - (x - width / 2) * Math.Sin(angle) - (height / 2 - y) * Math.Cos(angle));

                            if (x0 >= 0 && y0 >= 0 && x0 < width && y0 < height)
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
            float[] filter = { 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            genericFilter(img, imgCopy, filter);
        }

        public static void genericFilter(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float[] filter, float weight = 9)
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
                            meanB += (int)(filter[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[0]);
                            meanG += (int)(filter[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[1]);
                            meanR += (int)(filter[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[2]);
                            ct++;
                        }

                        (dataPtr + y * m.widthStep + x * nChan)[0] = (byte)(int)Math.Round(meanB / weight);
                        (dataPtr + y * m.widthStep + x * nChan)[1] = (byte)(int)Math.Round(meanG / weight);
                        (dataPtr + y * m.widthStep + x * nChan)[2] = (byte)(int)Math.Round(meanR / weight);
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
                        meanB += (int)(filter[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[0]);
                        meanG += (int)(filter[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[1]);
                        meanR += (int)(filter[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[2]);
                        ct++;
                    }

                    (dataPtr + y * m.widthStep + x * nChan)[0] = (byte)(int)Math.Round(meanB / weight);
                    (dataPtr + y * m.widthStep + x * nChan)[1] = (byte)(int)Math.Round(meanG / weight);
                    (dataPtr + y * m.widthStep + x * nChan)[2] = (byte)(int)Math.Round(meanR / weight);
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
                        meanB += (int)(filter[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[0]);
                        meanG += (int)(filter[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[1]);
                        meanR += (int)(filter[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[2]);
                        ct++;
                    }

                    (dataPtr + y * m.widthStep + x * nChan)[0] = (byte)(int)Math.Round(meanB / weight);
                    (dataPtr + y * m.widthStep + x * nChan)[1] = (byte)(int)Math.Round(meanG / weight);
                    (dataPtr + y * m.widthStep + x * nChan)[2] = (byte)(int)Math.Round(meanR / weight);

                    y = height - 1;     //Last row
                    ct = 0;
                    meanB = 0;
                    meanG = 0;
                    meanR = 0;

                    foreach (int[] coor in order.bottom)
                    {
                        meanB += (int)(filter[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[0]);
                        meanG += (int)(filter[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[1]);
                        meanR += (int)(filter[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[2]);
                        ct++;
                    }

                    (dataPtr + y * m.widthStep + x * nChan)[0] = (byte)(int)Math.Round(meanB / weight);
                    (dataPtr + y * m.widthStep + x * nChan)[1] = (byte)(int)Math.Round(meanG / weight);
                    (dataPtr + y * m.widthStep + x * nChan)[2] = (byte)(int)Math.Round(meanR / weight);
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
                        meanB += (int)(filter[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[0]);
                        meanG += (int)(filter[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[1]);
                        meanR += (int)(filter[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[2]);
                        ct++;
                    }

                    (dataPtr + y * m.widthStep + x * nChan)[0] = (byte)(int)Math.Round(meanB / weight);
                    (dataPtr + y * m.widthStep + x * nChan)[1] = (byte)(int)Math.Round(meanG / weight);
                    (dataPtr + y * m.widthStep + x * nChan)[2] = (byte)(int)Math.Round(meanR / weight);

                    x = width - 1;     //Last column
                    ct = 0;
                    meanB = 0;
                    meanG = 0;
                    meanR = 0;

                    foreach (int[] coor in order.right)
                    {
                        meanB += (int)(filter[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[0]);
                        meanG += (int)(filter[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[1]);
                        meanR += (int)(filter[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[2]);
                        ct++;
                    }

                    (dataPtr + y * m.widthStep + x * nChan)[0] = (byte)(int)Math.Round(meanB / weight);
                    (dataPtr + y * m.widthStep + x * nChan)[1] = (byte)(int)Math.Round(meanG / weight);
                    (dataPtr + y * m.widthStep + x * nChan)[2] = (byte)(int)Math.Round(meanR / weight);
                }
            }
        }

        public static void NonUniform(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float[,] matrix, float matrixWeight)
        {
            //float[] filter = matrix.Cast<float>().ToArray();
            //genericFilter(img, imgCopy, filter, (float)(matrixWeight));

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
                                    meanB += (int)(int)Math.Round(matrix[i + 1, j + 1] * (dataPtr2 + (y + j) * m.widthStep + (x + i) * nChan)[0]);
                                    meanG += (int)(int)Math.Round(matrix[i + 1, j + 1] * (dataPtr2 + (y + j) * m.widthStep + (x + i) * nChan)[1]);
                                    meanR += (int)(int)Math.Round(matrix[i + 1, j + 1] * (dataPtr2 + (y + j) * m.widthStep + (x + i) * nChan)[2]);
                                }
                            }
                            (dataPtr + y * m.widthStep + x * nChan)[0] = (byte)(int)Math.Round(meanB / matrixWeight);
                            (dataPtr + y * m.widthStep + x * nChan)[1] = (byte)(int)Math.Round(meanG / matrixWeight);
                            (dataPtr + y * m.widthStep + x * nChan)[2] = (byte)(int)Math.Round(meanR / matrixWeight);
                        }
                    }

                }
            }
            catch { }
        }

        public static void genericForTwo(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, int[] filterx, int[] filtery, float weight = 1)
        {
            unsafe
            {
                // direct access to the image memory(sequencial
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image

                MIplImage m2 = imgCopy.MIplImage;
                byte* dataPtr2 = (byte*)m2.imageData.ToPointer(); // Pointer to the image

                PixelOrder order = new PixelOrder();

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y, ct;
                var meanB = 0;
                var meanG = 0;
                var meanR = 0;
                int meanBx = 0;
                int meanGx = 0;
                int meanRx = 0;
                int meanBy = 0;
                int meanGy = 0;
                int meanRy = 0;

                for (y = 1; y < height - 1; y++)
                {
                    for (x = 1; x < width - 1; x++)
                    {
                        ct = 0;
                        meanB = 0;
                        meanG = 0;
                        meanR = 0;
                        meanBx = 0;
                        meanGx = 0;
                        meanRx = 0;
                        meanBy = 0;
                        meanGy = 0;
                        meanRy = 0;

                        foreach (int[] coor in order.center)
                        {
                            meanBx += (filterx[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[0]);
                            meanGx += (filterx[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[1]);
                            meanRx += (filterx[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[2]);

                            meanBy += (filtery[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[0]);
                            meanGy += (filtery[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[1]);
                            meanRy += (filtery[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[2]);
                            ct++;
                        }
                        meanB = Math.Min(Math.Abs(meanBx), 255) + Math.Min(Math.Abs(meanBy), 255);
                        meanG = Math.Min(Math.Abs(meanGx), 255) + Math.Min(Math.Abs(meanGy), 255);
                        meanR = Math.Min(Math.Abs(meanRx), 255) + Math.Min(Math.Abs(meanRy), 255);

                        (dataPtr + y * m.widthStep + x * nChan)[0] = (byte)meanB;
                        (dataPtr + y * m.widthStep + x * nChan)[1] = (byte)meanG;
                        (dataPtr + y * m.widthStep + x * nChan)[2] = (byte)meanR;
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
                    meanBx = 0;
                    meanGx = 0;
                    meanRx = 0;
                    meanBy = 0;
                    meanGy = 0;
                    meanRy = 0;

                    foreach (int[] coor in pixOrd)
                    {
                        meanBx += (filterx[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[0]);
                        meanGx += (filterx[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[1]);
                        meanRx += (filterx[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[2]);

                        meanBy += (filtery[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[0]);
                        meanGy += (filtery[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[1]);
                        meanRy += (filtery[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[2]);
                        ct++;
                    }
                    meanB = Math.Min(Math.Abs(meanBx), 255) + Math.Min(Math.Abs(meanBy), 255);
                    meanG = Math.Min(Math.Abs(meanGx), 255) + Math.Min(Math.Abs(meanGy), 255);
                    meanR = Math.Min(Math.Abs(meanRx), 255) + Math.Min(Math.Abs(meanRy), 255);

                    (dataPtr + y * m.widthStep + x * nChan)[0] = (byte)meanB;
                    (dataPtr + y * m.widthStep + x * nChan)[1] = (byte)meanG;
                    (dataPtr + y * m.widthStep + x * nChan)[2] = (byte)meanR;
                }


                //first and last row, without corners
                for (x = 1; x < width - 1; x++)
                {
                    y = 0;      //First row
                    ct = 0;
                    meanB = 0;
                    meanG = 0;
                    meanR = 0;
                    meanBx = 0;
                    meanGx = 0;
                    meanRx = 0;
                    meanBy = 0;
                    meanGy = 0;
                    meanRy = 0;

                    foreach (int[] coor in order.top)
                    {
                        meanBx += (filterx[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[0]);
                        meanGx += (filterx[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[1]);
                        meanRx += (filterx[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[2]);

                        meanBy += (filtery[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[0]);
                        meanGy += (filtery[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[1]);
                        meanRy += (filtery[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[2]);
                        ct++;
                    }
                    meanB = Math.Min(Math.Abs(meanBx), 255) + Math.Min(Math.Abs(meanBy), 255);
                    meanG = Math.Min(Math.Abs(meanGx), 255) + Math.Min(Math.Abs(meanGy), 255);
                    meanR = Math.Min(Math.Abs(meanRx), 255) + Math.Min(Math.Abs(meanRy), 255);

                    (dataPtr + y * m.widthStep + x * nChan)[0] = (byte)meanB;
                    (dataPtr + y * m.widthStep + x * nChan)[1] = (byte)meanG;

                    y = height - 1;     //Last row
                    ct = 0;
                    meanB = 0;
                    meanG = 0;
                    meanR = 0;
                    meanBx = 0;
                    meanGx = 0;
                    meanRx = 0;
                    meanBy = 0;
                    meanGy = 0;
                    meanRy = 0;

                    foreach (int[] coor in order.bottom)
                    {
                        meanBx += (filterx[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[0]);
                        meanGx += (filterx[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[1]);
                        meanRx += (filterx[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[2]);

                        meanBy += (filtery[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[0]);
                        meanGy += (filtery[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[1]);
                        meanRy += (filtery[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[2]);
                        ct++;
                    }
                    meanB = Math.Min(Math.Abs(meanBx), 255) + Math.Min(Math.Abs(meanBy), 255);
                    meanG = Math.Min(Math.Abs(meanGx), 255) + Math.Min(Math.Abs(meanGy), 255);
                    meanR = Math.Min(Math.Abs(meanRx), 255) + Math.Min(Math.Abs(meanRy), 255);

                    (dataPtr + y * m.widthStep + x * nChan)[0] = (byte)meanB;
                    (dataPtr + y * m.widthStep + x * nChan)[1] = (byte)meanG;
                }

                //first and last column, without corners
                for (y = 1; y < height - 1; y++)
                {
                    x = 0;      //First column
                    ct = 0;
                    meanB = 0;
                    meanG = 0;
                    meanR = 0;
                    meanBx = 0;
                    meanGx = 0;
                    meanRx = 0;
                    meanBy = 0;
                    meanGy = 0;
                    meanRy = 0;

                    foreach (int[] coor in order.left)
                    {
                        meanBx += (filterx[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[0]);
                        meanGx += (filterx[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[1]);
                        meanRx += (filterx[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[2]);

                        meanBy += (filtery[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[0]);
                        meanGy += (filtery[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[1]);
                        meanRy += (filtery[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[2]);
                        ct++;
                    }
                    meanB = Math.Min(Math.Abs(meanBx), 255) + Math.Min(Math.Abs(meanBy), 255);
                    meanG = Math.Min(Math.Abs(meanGx), 255) + Math.Min(Math.Abs(meanGy), 255);
                    meanR = Math.Min(Math.Abs(meanRx), 255) + Math.Min(Math.Abs(meanRy), 255);

                    (dataPtr + y * m.widthStep + x * nChan)[0] = (byte)meanB;
                    (dataPtr + y * m.widthStep + x * nChan)[1] = (byte)meanG;

                    x = width - 1;     //Last column
                    ct = 0;
                    meanB = 0;
                    meanG = 0;
                    meanR = 0;
                    meanBx = 0;
                    meanGx = 0;
                    meanRx = 0;
                    meanBy = 0;
                    meanGy = 0;
                    meanRy = 0;

                    foreach (int[] coor in order.right)
                    {
                        meanBx += (filterx[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[0]);
                        meanGx += (filterx[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[1]);
                        meanRx += (filterx[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[2]);

                        meanBy += (filtery[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[0]);
                        meanGy += (filtery[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[1]);
                        meanRy += (filtery[ct] * (dataPtr2 + (y + coor[1]) * m.widthStep + (x + coor[0]) * nChan)[2]);
                        ct++;
                    }
                    meanB = Math.Min(Math.Abs(meanBx), 255) + Math.Min(Math.Abs(meanBy), 255);
                    meanG = Math.Min(Math.Abs(meanGx), 255) + Math.Min(Math.Abs(meanGy), 255);
                    meanR = Math.Min(Math.Abs(meanRx), 255) + Math.Min(Math.Abs(meanRy), 255);

                    (dataPtr + y * m.widthStep + x * nChan)[0] = (byte)meanB;
                    (dataPtr + y * m.widthStep + x * nChan)[1] = (byte)meanG;
                }
            }
        }

        public static void Sobel(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            int[] tablex = { 1, 0, -1, 2, 0, -2, 1, 0, -1 };
            int[] tabley = { -1, -2, -1, 0, 0, 0, 1, 2, 1 };
            genericForTwo(img, imgCopy, tablex, tabley);
        }
        public static void Diferentiation(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            int[] tablex = { 1, 0, 0, 0, -1, 0, 0, 0, 0 };
            int[] tabley = { 0, 1, 0, -1, 0, 0, 0, 0, 0 };
            genericForTwo(img, imgCopy, tablex, tabley);
        }
        public static void Median(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            img = imgCopy.SmoothMedian(3);
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

        public static void Scale_point_xy(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float scaleFactor, int centerX, int centerY)
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
                        //var x0 = (int)Math.Round((x+width/2.0)/scaleFactor);
                        //var y0 = (int)Math.Round((y+height/2.0)/scaleFactor);
                        var x0 = (int)Math.Round((x + centerX / scaleFactor) / scaleFactor);
                        var y0 = (int)Math.Round((y + centerY / scaleFactor) / scaleFactor);

                        if (x0 >= 0 && y0 >= 0 && x0 <= width && y0 <= height)
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


        public static void ConvertToBW(Emgu.CV.Image<Bgr, byte> img, int threshold)
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

                //MAIN MEAN
                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        int blue = (dataPtr + y * m.widthStep + x * nChan)[0];
                        int green = (dataPtr + y * m.widthStep + x * nChan)[1];
                        int red = (dataPtr + y * m.widthStep + x * nChan)[2];
                        int ave = (byte)Math.Round((blue + green + red) / 3.0);

                        if (ave <= threshold)
                        {
                            for (int i = 0; i < 3; i++)
                                (dataPtr + y * m.widthStep + x * nChan)[i] = 0;
                        }
                        else
                        {
                            for (int i = 0; i < 3; i++)
                                (dataPtr + y * m.widthStep + x * nChan)[i] = 255;
                        }
                    }
                }
            }
        }

        public static void ConvertToBW_Otsu(Emgu.CV.Image<Bgr, byte> img)
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
                int[] histogram = new int[256];
                int threshold = 0;

                double weight_bg = 0.0;
                double mean_bg = 0.0;
                double variance_bg = 0.0;
                double weight_fg = 0.0;
                double mean_fg = 0.0;
                double variance_fg = 0.0;
                int hist_elements = 0;
                double class_variance = double.MaxValue;
                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        int blue = (dataPtr + y * m.widthStep + x * nChan)[0];
                        int green = (dataPtr + y * m.widthStep + x * nChan)[1];
                        int red = (dataPtr + y * m.widthStep + x * nChan)[2];
                        int ave = (byte)Math.Round((blue + green + red) / 3.0);
                        histogram[ave] += 1;
                    }
                }

                for (int current_threshold = 1; current_threshold < 255; current_threshold++)
                {
                    hist_elements = 0;
                    weight_bg = 0.0;
                    mean_bg = 0.0;
                    variance_bg = 0.0;
                    weight_fg = 0.0;
                    mean_fg = 0.0;
                    variance_fg = 0.0;
                    double current_class_variance = 0.0;

                    //background
                    for (int i = 0; i < current_threshold; i++)
                    {
                        weight_bg += histogram[i];
                        mean_bg += i * histogram[i];
                        hist_elements += histogram[i];
                    }
                    weight_bg /= width * height;
                    mean_bg /= hist_elements;
                    for (int i = 0; i < current_threshold; i++)
                    {
                        variance_bg += Math.Pow((i - mean_bg), 2) * histogram[i];
                    }
                    variance_bg /= hist_elements;
                    hist_elements = 0;

                    //foreground
                    for (int i = current_threshold + 1; i < 256; i++)
                    {
                        weight_fg += histogram[i];
                        mean_fg += i * histogram[i];
                        hist_elements += histogram[i];
                    }
                    weight_fg /= width * height;
                    mean_fg /= hist_elements;
                    for (int i = current_threshold + 1; i < 256; i++)
                    {
                        variance_fg += Math.Pow((i - mean_fg), 2) * histogram[i];
                    }
                    variance_fg /= hist_elements;

                    //result and compare
                    current_class_variance = weight_bg * variance_bg + weight_fg * variance_fg;
                    if (current_class_variance < class_variance)
                    {
                        class_variance = current_class_variance;
                        threshold = current_threshold;
                    }
                }
                ConvertToBW(img, threshold);
            }
        }

        public static void RedChannel(Image<Bgr, byte> img)
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
                // acesso directo : mais lento 
                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        // store in the image
                        dataPtr[0] = (dataPtr[2]);
                        dataPtr[1] = (dataPtr[2]);

                        // advance the pointer to the next pixel
                        dataPtr += nChan;
                    }
                    //at the end of the line advance the pointer by the aligment bytes (padding)
                    dataPtr += padding;
                }
            }
        }

        public static void BrightContrast(Image<Bgr, byte> img, int bright, double contrast)
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
                // acesso directo : mais lento 
                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        // store in the image
                        dataPtr[0] = (byte)(Math.Round(contrast * dataPtr[0] + bright));
                        dataPtr[1] = (byte)(Math.Round(contrast * dataPtr[1] + bright));
                        dataPtr[2] = (byte)(Math.Round(contrast * dataPtr[2] + bright));

                        // advance the pointer to the next pixel
                        dataPtr += nChan;
                    }
                    //at the end of the line advance the pointer by the aligment bytes (padding)
                    dataPtr += padding;
                }
            }
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

<<<<<<< HEAD
        
=======
                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;
                int[] histArray = new int[256];

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
>>>>>>> 8c66b2e10ff4869d468cf3aa60a96a7d21efcc3c

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
                byte blue, green, red;

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;
                int[,] histArray = new int[3, 256];

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

                            histArray[0, blue]++;
                            histArray[1, green]++;
                            histArray[2, red]++;

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
        public static int[,] Histogram_All(Emgu.CV.Image<Bgr, byte> img)
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
                int[,] histArray = new int[4, 256];

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

                            gray = (byte)Math.Round(((int)blue + green + red) / 3.0);

                            histArray[1, blue]++;
                            histArray[2, green]++;
                            histArray[3, red]++;
                            histArray[0, gray]++;

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



