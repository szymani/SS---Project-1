using System;
using System.Collections.Generic;
using System.Text;
using Emgu.CV.Structure;
using Emgu.CV;
using System.Linq;


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
    }
}
