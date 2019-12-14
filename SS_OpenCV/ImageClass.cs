using System;
using System.Collections.Generic;
using System.Text;
using Emgu.CV.Structure;
using Emgu.CV;

namespace SS_OpenCV
{
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
                            var x0 = (int)Math.Round((x - width / 2) * Math.Cos(angle) - (height / 2 - y) * Math.Sin(angle) + width / 2);
                            var y0 = (int)Math.Round(height /2 - (x - width / 2) * Math.Sin(angle) - (height / 2 - y) * Math.Cos(angle));

                            //if (Math.Abs(x0-x)<=height && Math.Abs(y0 - y) <= height)
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
                                meanB += (dataPtr2 + (y + j) * m.widthStep + (x + i) * nChan)[0];
                                meanG += (dataPtr2 + (y + j) * m.widthStep + (x + i) * nChan)[1];
                                meanR += (dataPtr2 + (y + j) * m.widthStep + (x + i) * nChan)[2];
                            }                       
                        }
                        (dataPtr + y * m.widthStep + x * nChan)[0] = (byte)(int)Math.Round(meanB / 9.0);
                        (dataPtr + y * m.widthStep + x * nChan)[1] = (byte)(int)Math.Round(meanG / 9.0);
                        (dataPtr + y * m.widthStep + x * nChan)[2] = (byte)(int)Math.Round(meanR / 9.0);
                    }
                }

                var tableS = new[] { 0,0,0,0 };
                for (int k = 0; k < 4; k++)
                {
                    x = 0;
                    y = 0;
                    switch (k)
                    {
                        case 0:     //Upper left corner
                            tableS = new[] { 4, 2, 2, 1 };
                            x = 0;
                            y = 0;
                            break;
                        case 1:     //Upper right
                            tableS = new[] { 2, 4, 2, 1 };
                            x = width-2;
                            y = 0;
                            break;
                        case 2:     //Lower left
                            tableS = new[] { 2, 1, 4, 2 };
                            x = 0;
                            y = height-2;
                            break;
                        case 3:     //Lower right
                            tableS = new[] { 1, 2, 2, 4 };
                            x = width-2;
                            y = height-2;
                            break;
                        default:
                            break;
                    }

                    meanB = 0;
                    meanG = 0;
                    meanR = 0;
                    for (int i = 0; i < 2; i++)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            meanB += tableS[k] * (dataPtr2 + (y + j) * m.widthStep + (x + i) * nChan)[0];
                            meanG += tableS[k] * (dataPtr2 + (y + j) * m.widthStep + (x + i) * nChan)[1];
                            meanR += tableS[k] * (dataPtr2 + (y + j) * m.widthStep + (x + i) * nChan)[2];
                        }
                    }
                    (dataPtr + y * m.widthStep + x * nChan)[0] = (byte)(int)Math.Round(meanB / 9.0);
                    (dataPtr + y * m.widthStep + x * nChan)[1] = (byte)(int)Math.Round(meanG / 9.0);
                    (dataPtr + y * m.widthStep + x * nChan)[2] = (byte)(int)Math.Round(meanR / 9.0);
                }

                var table = new[] { (-1, 0), (-1, 0), (-1, 1), (0, 0), (0, 0), (0, 1), (1, 0), (1, 0), (1, 1) };

                //first and last row, without corners
                for (x = 1; x < width - 1; x++)
                {
                    y = 0;      //First row
                    table = new[] { (0, -1), (0, -1), (0, 0), (0, 0), (0, 1), (0, 1), (1, -1), (1, 0), (1, 1) }; //y first

                    meanB = 0;
                    meanG = 0;
                    meanR = 0;
                    for (int i = 0; i < 9; i++)
                    {
                        meanB += (dataPtr2 + (y + table[i].Item1) * m.widthStep + (x + table[i].Item2) * nChan)[0];
                        meanG += (dataPtr2 + (y + table[i].Item1) * m.widthStep + (x + table[i].Item2) * nChan)[1];
                        meanR += (dataPtr2 + (y + table[i].Item1) * m.widthStep + (x + table[i].Item2) * nChan)[2];
                    }
                    (dataPtr + y * m.widthStep + x * nChan)[0] = (byte)(int)Math.Round(meanB / 9.0);
                    (dataPtr + y * m.widthStep + x * nChan)[1] = (byte)(int)Math.Round(meanG / 9.0);
                    (dataPtr + y * m.widthStep + x * nChan)[2] = (byte)(int)Math.Round(meanR / 9.0);

                    y = height - 1;     //Last row
                    table = new[] { (-1, 1), (-1, 0), (-1, 1), (0, -1), (0, -1), (0, 0), (0, 0), (0, 1), (0, 1) }; //y first

                    meanB = 0;
                    meanG = 0;
                    meanR = 0;
                    for (int i = 0; i < 9; i++)
                    {
                        meanB += (dataPtr2 + (y + table[i].Item1) * m.widthStep + (x + table[i].Item2) * nChan)[0];
                        meanG += (dataPtr2 + (y + table[i].Item1) * m.widthStep + (x + table[i].Item2) * nChan)[1];
                        meanR += (dataPtr2 + (y + table[i].Item1) * m.widthStep + (x + table[i].Item2) * nChan)[2];
                    }
                    (dataPtr + y * m.widthStep + x * nChan)[0] = (byte)(int)Math.Round(meanB / 9.0);
                    (dataPtr + y * m.widthStep + x * nChan)[1] = (byte)(int)Math.Round(meanG / 9.0);
                    (dataPtr + y * m.widthStep + x * nChan)[2] = (byte)(int)Math.Round(meanR / 9.0);
                }

                //first and last collumn, without corners
                for (y = 1; y < height - 1; y++)
                {
                    x = 0;      //first collumn
                    table = new[] { (-1, 0), (-1, 0), (-1, 1), (0, 0), (0, 0), (0, 1), (1, 0), (1, 0), (1, 1) }; //y first

                    meanB = 0;
                    meanG = 0;
                    meanR = 0;
                    for (int i = 0; i < 9; i++)
                    {
                        meanB += (dataPtr2 + (y + table[i].Item1) * m.widthStep + (x + table[i].Item2) * nChan)[0];
                        meanG += (dataPtr2 + (y + table[i].Item1) * m.widthStep + (x + table[i].Item2) * nChan)[1];
                        meanR += (dataPtr2 + (y + table[i].Item1) * m.widthStep + (x + table[i].Item2) * nChan)[2];
                    }
                    (dataPtr + y * m.widthStep + x * nChan)[0] = (byte)(int)Math.Round(meanB / 9.0);
                    (dataPtr + y * m.widthStep + x * nChan)[1] = (byte)(int)Math.Round(meanG / 9.0);
                    (dataPtr + y * m.widthStep + x * nChan)[2] = (byte)(int)Math.Round(meanR / 9.0);

                    x = width-1;      //last collumn
                    table = new[] { (-1, -1), (-1, 0), (-1, 0), (0, -1), (0, 0), (0, 0), (1, -1), (1, 0), (1, 0) }; //y first

                    meanB = 0;
                    meanG = 0;
                    meanR = 0;
                    for (int i = 0; i < 9; i++)
                    {
                        meanB += (dataPtr2 + (y + table[i].Item1) * m.widthStep + (x + table[i].Item2) * nChan)[0];
                        meanG += (dataPtr2 + (y + table[i].Item1) * m.widthStep + (x + table[i].Item2) * nChan)[1];
                        meanR += (dataPtr2 + (y + table[i].Item1) * m.widthStep + (x + table[i].Item2) * nChan)[2];
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


        


    }
}


