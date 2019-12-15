/// <summary>
/// Traffic Signs Detection
/// </summary>
/// <param name="img">Input image</param>
/// <param name="imgCopy">Image Copy</param>
/// <param name="limitSign">List of speed limit value and positions (speed limit value, Left-x,Top-y,Right-x,Bottom-y) of all detected limit signs</param>
/// <param name="warningSign">List of value (-1) and positions (-1, Left-x,Top-y,Right-x,Bottom-y) of all detected warning signs</param>
/// <param name="prohibitionSign">List of value (-1) and positions (-1, Left-x,Top-y,Right-x,Bottom-y) of all detected prohibition signs</param>
/// <param name="level">Image Level</param>
/// <returns>image with traffic signs detected</returns>
public static Image<Bgr, byte> Signs(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, out List<string[]> limitSign, out List<string[]> warningSign, out List<string[]> prohibitionSign, int level)
{
    limitSign = new List<string[]>();
    warningSign = new List<string[]>();
    prohibitionSign = new List<string[]>();


    string[] dummy_vector1 = new string[5];
    dummy_vector1[0] = "70";   // Speed limit
    dummy_vector1[1] = "1160"; // Left-x
    dummy_vector1[2] = "330";  // Top-y
    dummy_vector1[3] = "1200"; // Right-x
    dummy_vector1[4] = "350";  // Bottom-y

    string[] dummy_vector2 = new string[5];
    dummy_vector2[0] = "-1";  // value -1
    dummy_vector2[1] = "669"; // Left-x
    dummy_vector2[2] = "469"; // Top-y
    dummy_vector2[3] = "680"; // Right-x
    dummy_vector2[4] = "480"; // Bottom-y

    string[] dummy_vector3 = new string[5];
    dummy_vector3[0] = "-1";  // value -1
    dummy_vector3[1] = "669"; // Left-x
    dummy_vector3[2] = "469"; // Top-y
    dummy_vector3[3] = "680"; // Right-x
    dummy_vector3[4] = "480"; // Bottom-y

    limitSign.Add(dummy_vector1);
    warningSign.Add(dummy_vector2);
    prohibitionSign.Add(dummy_vector3);


    return img;
}