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

        public static int DetectDigit(Image<Bgr, byte> img, List<Image<Bgr, Byte>> digits, int[] sign_coords)
        {
            unsafe
            {
                int x, y;
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image

                int width = sign_coords[2] - sign_coords[0];
                int height = sign_coords[3] - sign_coords[1];
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width;
                int[] digits_similarity = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                int maximum_similarity = 0;
                int index_max_similarity = -1;
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
                Console.Out.WriteLine(index_max_similarity);
                Console.Out.WriteLine(digits_similarity);
                return index_max_similarity;
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
    }
}
