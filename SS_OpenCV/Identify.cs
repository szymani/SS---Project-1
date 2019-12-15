using System;
using System.Collections.Generic;
using System.Text;
using Emgu.CV.Structure;
using Emgu.CV;
using System.Linq;

namespace SS_OpenCV
{
    struct Coords
    {

        private int code;
        private int begX;
        private int begY;
        private int endX;
        private int endY;
        private int frameHeight;
        private int frameWidth;

        public Coords(int code, int begX, int begY, int endX, int endY, int frameHeight, int frameWidth) : this()
        {
            this.code = code;
            this.begX = begX;
            this.begY = begY;
            this.endX = endX;
            this.endY = endY;
            this.frameHeight = frameHeight;
            this.frameWidth = frameWidth;
        }
    }

    class Identify
    {
        public static void BgrToHsv(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
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

                        double r = red / 255.0;
                        double g = green / 255.0;
                        double b = blue / 255.0;
                        double hue, sat, val;

                        double cmax = Math.Max(r, b);
                        cmax = Math.Max(cmax, g);
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

                        (dataPtr2 + y * m.widthStep + x * nChan)[0] = (byte)(Math.Round(val * 255));
                        (dataPtr2 + y * m.widthStep + x * nChan)[1] = (byte)(Math.Round(sat * 255));
                        (dataPtr2 + y * m.widthStep + x * nChan)[2] = (byte)(Math.Round(hue * 255 / 360));

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

        public static void connectedComponents(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, int hueLimit  =  20, int satLimit  =  50, int valLimit =  50)
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

                int hue = 0;
                int value = 0;
                int saturation = 0;

                int[, ] indexTable = new int[width, height];
                int currentMax = 0;
                int toChange = -1;
                List<int> objects = new List<int>();
                List<Coords> result = new List<Coords>();

                for (y = 1; y < height-1; y++)
                {
                    for (x = 1; x < width-1; x++)
                    {
                        value = ((dataPtr2 + y * m.widthStep + x * nChan)[0] * 100) / 255;
                        saturation = ((dataPtr2 + y * m.widthStep + x * nChan)[1] * 100) / 255;
                        hue = ((dataPtr2 + y * m.widthStep + x * nChan)[2] * 360 ) / 255;

                        if (hue >= 0 && hue <=hueLimit)         //if red
                        {
                            //8 - connectivity
                            if (indexTable[x + 1, y - 1] != 0)
                                indexTable[x, y] = indexTable[x + 1, y - 1];

                            if (indexTable[x, y - 1] != 0 && indexTable[x, y] != indexTable[x, y - 1])
                            {
                                if(indexTable[x, y] == 0)
                                    indexTable[x, y] = indexTable[x, y - 1];
                                else                  //Find lower index,  change  higher  to lower
                                {
                                    if (indexTable[x, y] < indexTable[x, y - 1])
                                        toChange = indexTable[x, y - 1];

                                    else
                                    {
                                        indexTable[x, y] = indexTable[x, y - 1];
                                        toChange = indexTable[x, y];
                                    }
                                    for (int i = 0; i < height; i++)
                                    {
                                        for (int j = 0; j < width; j++)
                                        {
                                            if (indexTable[i, j] == toChange)
                                                indexTable[i, j] = toChange;
                                        }
                                    }
                                    objects.Remove(objects.Single(r => r == toChange));
                                }
                            }
                            if (indexTable[x - 1,  y  - 1] != 0 && indexTable[x, y] != indexTable[x - 1,  y  - 1])
                            {
                                if (indexTable[x, y] == 0)
                                    indexTable[x, y] = indexTable[x - 1,  y  - 1];
                                else
                                {
                                    if (indexTable[x, y] < indexTable[x - 1,  y  - 1])
                                        toChange = indexTable[x - 1,  y  - 1];

                                    else
                                    {
                                        indexTable[x, y] = indexTable[x - 1,  y  - 1];
                                        toChange = indexTable[x, y];
                                    }
                                    for (int i = 0; i < height; i++)
                                    {
                                        for (int j = 0; j < width; j++)
                                        {
                                            if (indexTable[i, j] == toChange)
                                                indexTable[i, j] = toChange;
                                        }
                                    }
                                    objects.Remove(objects.Single(r => r == toChange));
                                }
                            }
                            if (indexTable[x - 1,  y ] != 0 && indexTable[x, y] != indexTable[x - 1,  y ])
                            {
                                if (indexTable[x, y] == 0)
                                    indexTable[x, y] = indexTable[x - 1,  y ];
                                else
                                {
                                    if (indexTable[x, y] < indexTable[x - 1,  y ])
                                        toChange = indexTable[x - 1,  y ];

                                    else
                                    {
                                        indexTable[x, y] = indexTable[x - 1,  y ];
                                        toChange = indexTable[x, y];
                                    }
                                    for (int i = 0; i < height; i++)
                                    {
                                        for (int j = 0; j < width; j++)
                                        {
                                            if (indexTable[i, j] == toChange)
                                                indexTable[i, j] = toChange;
                                        }
                                    }
                                    objects.Remove(objects.Single(r => r == toChange));
                                }
                            }
                            else
                            {
                                currentMax = currentMax++;
                                indexTable[x, y] = currentMax;
                                objects.Add(currentMax);
                            }
                        }
                        else
                        {
                            indexTable[x, y] = 0;
                        }                     
                    }
                }
                foreach (int obj in objects)
                {
                    int code = obj;
                    int begX = 10000000;
                    int begY = 10000000;
                    int endX = -1;
                    int endY = -1;
                    int frameHeight;
                    int frameWidth;
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            if(indexTable[i, j] == code)
                            {
                                if (i < begX)
                                    begX = i;
                                if (j < begY)
                                    begY = i;
                                if (i > endX)
                                    endX = i;
                                if (j > endY)
                                    endY = j;
                            }
                        }
                    }

                    frameWidth = endX - begX;
                    frameHeight = endY - begY;

                    result.Add(new Coords(code, begX, begY, endX, endY, frameHeight, frameWidth));
                }
            }
        }

        public static List<Image<Bgr, Byte>> Scale(List<Image<Bgr, Byte>> digits, int[] sign_coords)
        {
            unsafe
            {
                for (int i=0; i<10; i++)
                {
                    int x, y;
                    MIplImage m = digits[i].MIplImage;
                    byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image

                    int width = digits[i].Width;
                    int height = digits[i].Height;
                    int nChan = m.nChannels; // number of channels - 3
                    int padding = m.widthStep - m.nChannels * m.width;
                    /*
                    sign_coords[0] = Left-x
                    sign_coords[1] = Top-y
                    sign_coords[2] = Right-x
                    sign_coords[3] = Bottom-y
                    */
                    double sign_size = Math.Sqrt((sign_coords[2] - sign_coords[0]) * (sign_coords[3] - sign_coords[1]));
                    double digit_size = Math.Sqrt(width * height);
                    double scaleFactor = sign_size / digit_size;

                    Console.Out.WriteLine(scaleFactor);

                    height = (int)Math.Round(height * scaleFactor);
                    width = (int)Math.Round(width * scaleFactor);

                    Image<Bgr, byte> resizedImage = digits[i].Resize(width, height, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);

                    digits[i] = resizedImage;
  
                }
                return digits;
            }
        }
    }
}
