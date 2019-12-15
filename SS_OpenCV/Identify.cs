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

        public static void DrawRectangles(Image<Bgr, byte> img, List<int[]> sign_coords, int type = 0)
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


                byte[] colors = new byte[3];
                if (type.Equals(0))
                    colors = new byte[]{0 ,0 ,255 };
                else if (type.Equals(1))
                    colors = new byte[] { 255, 0, 0 };
                else if (type.Equals(2))
                    colors = new byte[] { 0, 255, 0 };

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
                                (dataPtr + y * m.widthStep + x * nChan)[0] = colors[0];
                                (dataPtr + y * m.widthStep + x * nChan)[1] = colors[1];
                                (dataPtr + y * m.widthStep + x * nChan)[2] = colors[2];
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

        public static List<List<int[]>> connectedComponents(Image<Bgr, byte> imgHsv, Image<Bgr, byte> img, int hueLimit  =  20, int satLimit  =  50, int valLimit = 30)
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
                int currentMax = 0;
                List<int> objects = new List<int>();
                List<(int, int)> aliases = new List<(int, int)>();
                List<int[]> result = new List<int[]>();

                int[,] indexTableBlack = new int[width, height];
                int currentMaxBlack = 0;
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
                        else
                        {
                            indexTable[x, y] = 0;
                        }
                    }
                }

                IDictionary<int, List<int>> dict = getAliases(aliases);

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        foreach (int index in dict.Keys)
                        {
                            if (dict[index].Contains(indexTable[x, y]))
                            {
                                indexTable[x, y] = index;
                            }
                        }
                    }
                }

                List<int> toRemove = new List<int>();
                foreach (int obj in objects)
                {
                    foreach (int index in dict.Keys)
                    {
                        if (dict[index].Contains(obj) && !index.Equals(obj))
                        {
                            toRemove.Add(obj);
                        }
                    }
                }
                foreach (int key in toRemove)
                {
                    objects.Remove(key);
                }
                foreach (int obj in objects)
                {
                    int count = 0;
                    foreach (int num in indexTable)
                    {
                        if (num.Equals(obj))
                            count += 1;
                    }

                    if (count > 500)
                    {
                        int code = obj;
                        int begX = 10000000;
                        int begY = 10000000;
                        int endX = -1;
                        int endY = -1;

                        for (int i = 0; i < height; i++)
                        {
                            for (int j = 0; j < width; j++)
                            {
                                if (indexTable[j, i] == code)
                                {
                                    if (i < begX)
                                        begX = i;
                                    if (j < begY)
                                        begY = j;
                                    if (i > endX)
                                        endX = i;
                                    if (j > endY)
                                        endY = j;
                                }
                            }
                        }
                        result.Add(new int[] { begY, begX, endY, endX });
                    }
                }

                Image<Bgr, Byte> otsu = img.Copy();

                foreach(int[] res in result)
                {
                    Identify.ConvertToBW_Otsu_coords(otsu, res);
                    MIplImage mOtsu = otsu.MIplImage;
                    byte* dataPtrOtsu = (byte*)mOtsu.imageData.ToPointer(); // Pointer to the image

                    for (y = res[1]; y <= res[3]; y++)
                    {
                        for (x = res[0]; x <= res[2]; x++)
                        {
                            //value = (dataPtrOtsu + y * m.widthStep + x * nChan)[0];

                            value = ((dataPtr + y * m.widthStep + x * nChan)[0] * 100) / 255;

                            if (value<35)         //if black
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
                                    currentMaxBlack = currentMaxBlack + 1;
                                    indexTableBlack[x, y] = currentMaxBlack;
                                    objectsBlack.Add(currentMaxBlack);
                                }
                            }
                            else
                            {
                                indexTableBlack[x, y] = 0;
                            }
                        }
                    }
                }

                dict = getAliases(aliasesBlack);

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        foreach (int index in dict.Keys)
                        {
                            if (dict[index].Contains(indexTableBlack[x, y]))
                            {
                                indexTableBlack[x, y] = index;
                            }
                        }
                    }
                }

                toRemove = new List<int>();
                foreach (int obj in objectsBlack)
                {
                    foreach (int index in dict.Keys)
                    {
                        if (dict[index].Contains(obj) && !index.Equals(obj))
                        {
                            toRemove.Add(obj);
                        }
                    }
                }

                foreach (int key in toRemove)
                {
                    objectsBlack.Remove(key);
                }

                foreach (int obj in objectsBlack)
                {
                    int count = 0;
                    foreach (int num in indexTableBlack)
                    {
                        if (num.Equals(obj))
                            count += 1;
                    }
                    if (count > 400)
                    {
                        int code = obj;
                        int begX = 10000000;
                        int begY = 10000000;
                        int endX = -1;
                        int endY = -1;

                        for (int i = 0; i < height; i++)
                        {
                            for (int j = 0; j < width; j++)
                            {
                                if (indexTableBlack[j, i] == code)
                                {
                                    if (i < begX)
                                        begX = i;
                                    if (j < begY)
                                        begY = j;
                                    if (i > endX)
                                        endX = i;
                                    if (j > endY)
                                        endY = j;
                                }
                            }
                        }
                            resultBlack.Add(new int[] { begY, begX, endY, endX });
                    }
                }

                List<int[]> newBlack = new List<int[]>();
                foreach (int[] red in result)
                {
                    foreach (int[] black in resultBlack)
                    {
                        if ((black[0] - red[0] > 20) && (black[1] - red[1] > 20) && (red[2] - black[2] > 20) && (red[3] - black[3] > 20))
                        {
                            newBlack.Add(black);
                        }
                    }
                }

                //Drawing rectangle
                Identify.DrawRectangles(img, result);
                Identify.DrawRectangles(img, resultBlack, 1);
                Identify.DrawRectangles(img, newBlack, 2);

                return new List<List<int[]>> { result, newBlack };
            }
        }

        public static List<Image<Bgr, Byte>> Scale(List<Image<Bgr, Byte>> digits, int[] sign_coords)
        {
            unsafe
            {
                for (int i = 0; i < 10; i++)
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

                    /*
                    //Scaling without changed aspect ratio
                    double sign_size = Math.Sqrt((sign_coords[2] - sign_coords[0]) * (sign_coords[3] - sign_coords[1]));
                    double digit_size = Math.Sqrt(width * height);
                    double scaleFactor = sign_size / digit_size;
                    height = (int)Math.Round(height * scaleFactor);
                    width = (int)Math.Round(width * scaleFactor);
                    */

                    //Scaling with changed aspect ratio
                    width = sign_coords[2] - sign_coords[0];
                    height = sign_coords[3] - sign_coords[1];

                    //Creating new image with scaled dimensions
                    Image<Bgr, byte> resizedImage = digits[i].Resize(width, height, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);

                    //Save resized image
                    digits[i] = resizedImage;

                    //Thresholding
                    ImageClass.ConvertToBW_Otsu(digits[i]);

                }
                return digits;
            }
        }

        public static int DetectDigit(Image<Bgr, byte> img, List<Image<Bgr, Byte>> digits, int[] sign_coords, double tolerance = 0.8)
        {
            unsafe
            {
                int x, y;
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image

                int width = sign_coords[2] - sign_coords[0];
                int height = sign_coords[3] - sign_coords[1];
                int area = width * height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width;
                int[] digits_similarity = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                int maximum_similarity = 0;
                int index_max_similarity = -1;
                double similarity_factor;
                ConvertToBW_Otsu_coords(img, sign_coords);

                for (int i = 0; i < 10; i++)
                {
                    MIplImage m2 = digits[i].MIplImage;
                    byte* dataPtr2 = (byte*)m2.imageData.ToPointer(); // Pointer to the image

                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            if ((dataPtr + (y + sign_coords[1]) * m.widthStep + (x + sign_coords[0]) * nChan)[0] == (dataPtr2 + y * m2.widthStep + x * nChan)[0])
                            {
                                digits_similarity[i]++;
                            }
                        }
                    }
                    if (digits_similarity[i] > maximum_similarity)
                    {
                        index_max_similarity = i;
                        maximum_similarity = digits_similarity[i];
                    }
                }
                //Console.Out.WriteLine(index_max_similarity);
                //Console.Out.WriteLine(digits_similarity);
                similarity_factor = (double)maximum_similarity / area;
                if (similarity_factor > tolerance)
                    return index_max_similarity;
                else
                    return -1;
            }
        }

        public static void ConvertToBW_coords(Image<Bgr, byte> img, int threshold, int[] sign_coords)
        {
            unsafe
            {
                int x, y;
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image

                int nChan = m.nChannels; // number of channels - 3

                //MAIN MEAN
                for (y = sign_coords[1]; y <= sign_coords[3]; y++)
                {
                    for (x = sign_coords[0]; x < sign_coords[2]; x++)
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

        public static void ConvertToBW_Otsu_coords(Image<Bgr, byte> img, int[] sign_coords)
        {
            unsafe
            {
                int x, y;
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image

                int width = sign_coords[2] - sign_coords[0];
                int height = sign_coords[3] - sign_coords[1];
                int nChan = m.nChannels; // number of channels - 3
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
                for (y = sign_coords[1]; y < sign_coords[3]; y++)
                {
                    for (x = sign_coords[0]; x < sign_coords[2]; x++)
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
                ConvertToBW_coords(img, threshold, sign_coords);
            }
        }

        public static List<string[]> CreateFinalList(List<int> classification, List<int[]> signsObjects, List<int[]> numberObjects)
        {
            unsafe
            {
                List<string[]> signs = new List<string[]>();
                List<int[]> foundedDigits = new List<int[]>();
                string sign_value = "";
                int index = 0;

                //Sorting digits founded inside sign
                foreach (int classifier in classification)
                {
                    if (classifier >= 0)
                    {
                        foundedDigits.Add(new int[] { classifier, numberObjects[index][0], numberObjects[index][1], numberObjects[index][2], numberObjects[index][3] });
                    }
                    index += 1;
                }

                //Merging sorted digits as one number
                foundedDigits = foundedDigits.OrderBy(x => x[1]).ToList();
                foreach (int[] number in foundedDigits)
                {
                    sign_value += number[0].ToString();
                }

                //Adding founded signs to list
                foreach (int[] sign in signsObjects)
                {
                    string[] dummy_vector = new string[5];

                    //Checking digits position in order to sign position - if digit is inside sign then sign is speed limit type
                    if (sign[0] - foundedDigits[0][1] < 0 && sign[1] - foundedDigits[0][2] < 0 && sign[2] - foundedDigits[0][3] > 0 && sign[3] - foundedDigits[0][4] > 0)
                        dummy_vector[0] = sign_value;   // Speed limit
                    else
                        dummy_vector[0] = "-1";     // Another sign

                    dummy_vector[1] = sign[0].ToString(); // Left-x
                    dummy_vector[2] = sign[1].ToString();  // Top-y
                    dummy_vector[3] = sign[2].ToString(); // Right-x
                    dummy_vector[4] = sign[3].ToString();  // Bottom-y
                    signs.Add(dummy_vector);
                }
                return signs;
            }
        }

    }
}
