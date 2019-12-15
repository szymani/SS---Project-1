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

        public static void DrawRectangles(Image<Bgr, byte> img, List<int[]> sign_coords)
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
                        foreach(int[] sign in  sign_coords)
                        {
                            //Drawing rectangle
                            if (
                                x > sign[0] && x < sign[2] && (y == sign[1] || y == sign[3]) || //vertical lines
                                y > sign[1] && y < sign[3] && (x == sign[0] || x == sign[2])    //horizontal lines
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

        public static IDictionary<int, List<int>> getAliases(List<(int, int)> lista)
        {
            lista.Sort((first, second) => first.Item1.CompareTo(second.Item1));
            IDictionary<int, List<int>> dict = new Dictionary<int, List<int>>();

            foreach ((int, int) x in lista)
            {
                if (dict.ContainsKey(x.Item1))
                    dict[x.Item1].Add(x.Item2);
                else
                    dict.Add(new KeyValuePair<int, List<int>>(x.Item1, new List<int>() { x.Item2 }));
            }
             dict.OrderByDescending(pair => pair.Key);

            List<int> toRemove = new List<int>();
            foreach (int key in dict.Keys.OrderByDescending(y => y))
            {
                foreach (int key2 in dict.Keys.OrderByDescending(x=> x))
                {
                    if(!key.Equals(key2) && !toRemove.Contains(key))
                    {
                        if(dict[key2].Contains(key))
                        {
                            dict[key2].AddRange(dict[key]);
                            toRemove.Add(key);
                        }
                    }
                }
            }
            foreach(int key in toRemove)
            {
                dict.Remove(key);
            }

            foreach(int key in dict.Keys.ToList())
            {
                dict[key] = dict[key].Distinct().ToList();
            }

            return dict;
        }

        public static List<int[]> connectedComponents(Image<Hsv, byte> imgHsv, Image<Bgr, byte> img, int hueLimit  =  40, int satLimit  =  50, int valLimit = 30)
        {
            unsafe
            {
                int x, y;
                MIplImage m = imgHsv.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image

                int width = imgHsv.Width;
                int height = imgHsv.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width;

                int hue = 0;
                int value = 0;
                int saturation = 0;

                int[,] indexTable = new int[width, height];
                int[,] indexTableBlack = new int[width, height];
                int currentMax = 0;
                List<int> objects = new List<int>();
                List<(int, int)> aliases = new List<(int, int)>();
                List<int[]> result = new List<int[]>();

                List<int> objectsBlack = new List<int>();
                List<(int, int)> aliasesBlack = new List<(int, int)>();
                List<int[]> resultBlack = new List<int[]>();

                for (y = 1; y < height - 1; y++)
                {
                    for (x = 1; x < width - 1; x++)
                    {
                        value = ((dataPtr + y * m.widthStep + x * nChan)[0] * 100) / 255;
                        saturation = ((dataPtr + y * m.widthStep + x * nChan)[1] * 100) / 255;
                        hue = ((dataPtr + y * m.widthStep + x * nChan)[2] * 360) / 255;

                        if (((hue >= 0 && hue <= hueLimit)||(hue >= 360 - hueLimit && hue <= 360)) && (saturation <= 100 && saturation > satLimit) && (value <= 100 && value > valLimit))         //if red
                        {
                            //8 - connectivity
                            if (indexTable[x + 1, y - 1] != 0)
                                indexTable[x, y] = indexTable[x + 1, y - 1];

                            if (indexTable[x, y - 1] != 0 && indexTable[x, y] != indexTable[x, y - 1])
                            {
                                if (indexTable[x, y] == 0)
                                    indexTable[x, y] = indexTable[x, y - 1];
                                else                  //Find lower index,  change  higher  to lower
                                {
                                    if (indexTable[x, y] < indexTable[x, y - 1])
                                        aliases.Add((indexTable[x, y], indexTable[x, y - 1]));

                                    else
                                    {
                                        indexTable[x, y] = indexTable[x, y - 1];
                                        aliases.Add((indexTable[x, y - 1], indexTable[x, y]));
                                    }
                                }
                            }
                            else if (indexTable[x - 1, y - 1] != 0 && indexTable[x, y] != indexTable[x - 1, y - 1])
                            {
                                if (indexTable[x, y] == 0)
                                    indexTable[x, y] = indexTable[x - 1, y - 1];
                                else
                                {
                                    if (indexTable[x, y] < indexTable[x - 1, y - 1])
                                        aliases.Add((indexTable[x, y], indexTable[x - 1, y - 1]));
                                    else
                                    {
                                        indexTable[x, y] = indexTable[x - 1, y - 1];
                                        aliases.Add((indexTable[x - 1, y - 1], indexTable[x, y]));
                                    }
                                }
                            }
                            if (indexTable[x - 1, y] != 0 && indexTable[x, y] != indexTable[x - 1, y])
                            {
                                if (indexTable[x, y] == 0)
                                    indexTable[x, y] = indexTable[x - 1, y];
                                else
                                {
                                    if (indexTable[x, y] < indexTable[x - 1, y])
                                        aliases.Add((indexTable[x, y], indexTable[x - 1, y]));

                                    else
                                    {
                                        indexTable[x, y] = indexTable[x - 1, y];
                                        aliases.Add((indexTable[x - 1, y], indexTable[x, y]));
                                    }
                                }
                            }
                            if (indexTable[x, y] == 0)
                            {
                                currentMax = currentMax + 1;
                                indexTable[x, y] = currentMax;
                                objects.Add(currentMax);
                            }
                        }
                        else if (value <= 35 && value >= 0)         //if black
                        {
                            //8 - connectivity
                            if (indexTableBlack[x + 1, y - 1] != 0)
                                indexTableBlack[x, y] = indexTableBlack[x + 1, y - 1];

                            if (indexTableBlack[x, y - 1] != 0 && indexTableBlack[x, y] != indexTableBlack[x, y - 1])
                            {
                                if (indexTableBlack[x, y] == 0)
                                    indexTableBlack[x, y] = indexTableBlack[x, y - 1];
                                else                  //Find lower index,  change  higher  to lower
                                {
                                    if (indexTableBlack[x, y] < indexTableBlack[x, y - 1])
                                        aliasesBlack.Add((indexTableBlack[x, y], indexTableBlack[x, y - 1]));

                                    else
                                    {
                                        indexTableBlack[x, y] = indexTableBlack[x, y - 1];
                                        aliasesBlack.Add((indexTableBlack[x, y - 1], indexTableBlack[x, y]));
                                    }
                                }
                            }
                            if (indexTableBlack[x - 1, y - 1] != 0 && indexTableBlack[x, y] != indexTableBlack[x - 1, y - 1])
                            {
                                if (indexTableBlack[x, y] == 0)
                                    indexTableBlack[x, y] = indexTableBlack[x - 1, y - 1];
                                else
                                {
                                    if (indexTableBlack[x, y] < indexTableBlack[x - 1, y - 1])
                                        aliasesBlack.Add((indexTableBlack[x, y], indexTableBlack[x - 1, y - 1]));
                                    else
                                    {
                                        indexTableBlack[x, y] = indexTableBlack[x - 1, y - 1];
                                        aliasesBlack.Add((indexTableBlack[x - 1, y - 1], indexTableBlack[x, y]));
                                    }
                                }
                            }
                            if (indexTableBlack[x - 1, y] != 0 && indexTableBlack[x, y] != indexTableBlack[x - 1, y])
                            {
                                if (indexTableBlack[x, y] == 0)
                                    indexTableBlack[x, y] = indexTableBlack[x - 1, y];
                                else
                                {
                                    if (indexTableBlack[x, y] < indexTableBlack[x - 1, y])
                                        aliasesBlack.Add((indexTableBlack[x, y], indexTableBlack[x - 1, y]));

                                    else
                                    {
                                        indexTableBlack[x, y] = indexTableBlack[x - 1, y];
                                        aliasesBlack.Add((indexTableBlack[x - 1, y], indexTableBlack[x, y]));
                                    }
                                }
                            }
                            if (indexTableBlack[x, y] == 0)
                            {
                                currentMax = currentMax + 1;
                                indexTableBlack[x, y] = currentMax;
                                objectsBlack.Add(currentMax);
                            }
                        }
                        else
                        {
                            indexTable[x, y] = 0;
                            indexTableBlack[x, y] = 0;
                        }
                    }
                }

                //------------Anotated pixels Check--------------------
                MIplImage m2 = img.MIplImage;
                byte* dataPtr2 = (byte*)m2.imageData.ToPointer(); // Pointer to the image

                int width2 = img.Width;
                int height2 = img.Height;
                int nChan2 = m.nChannels; // number of channels - 3

                int changedPixels = 0;
                int changedPixelsBlack = 0;
                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        if (!indexTable[x, y].Equals(0))
                        {
                            // get pixel address
                            (dataPtr2 + y * m2.widthStep + x * nChan2)[0] = 255;
                            (dataPtr2 + y * m2.widthStep + x * nChan2)[1] = 0;
                            (dataPtr2 + y * m2.widthStep + x * nChan2)[2] = 255;
                            changedPixels += 1;
                        }
                    }
                }

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        if (!indexTableBlack[x, y].Equals(0))
                        {
                            // get pixel address
                            (dataPtr2 + y * m2.widthStep + x * nChan2)[0] = 255;
                            (dataPtr2 + y * m2.widthStep + x * nChan2)[1] = 255;
                            (dataPtr2 + y * m2.widthStep + x * nChan2)[2] = 0;
                            changedPixelsBlack += 1;
                        }
                    }
                }
                //-----------------------------------------------



                //IDictionary<int, List<int>> dict = getAliases(aliases);

                //for (y = 0; y < height; y++)
                //{
                //    for (x = 0; x < width; x++)
                //    { 
                //        foreach(int index in dict.Keys)
                //        {
                //            if (dict[index].Contains(indexTable[x, y]))
                //            {
                //                indexTable[x, y] = index;
                //            }
                //        }
                //    }
                //}

                //List<int> toRemove = new List<int>();
                //foreach (int obj in objects)
                //{
                //    foreach (int index in dict.Keys)
                //    {
                //        if (dict[index].Contains(obj))
                //        {
                //            toRemove.Add(obj);
                //        }
                //    }
                //}
                //foreach (int key in toRemove)
                //{
                //    objects.Remove(key);
                //}

                //foreach (int obj in objects)
                //{
                //    int count = 0;
                //    foreach (int num in indexTable)
                //    {
                //        if (num.Equals(obj))
                //            count += 1;
                //    }

                //    if (count>500)
                //    {
                //        int code = obj;
                //        int begX = 10000000;
                //        int begY = 10000000;
                //        int endX = -1;
                //        int endY = -1;

                //        for (int i = 0; i < height; i++)
                //        {
                //            for (int j = 0; j < width; j++)
                //            {
                //                if (indexTable[j, i] == code)
                //                {
                //                    if (i < begX)
                //                        begX = i;
                //                    if (j < begY)
                //                        begY = j;
                //                    if (i > endX)
                //                        endX = i;
                //                    if (j > endY)
                //                        endY = j;
                //                }
                //            }
                //        }
                //        result.Add(new int[] { begY, begX, endY, endX });
                //    }
                //}
                return result;
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
