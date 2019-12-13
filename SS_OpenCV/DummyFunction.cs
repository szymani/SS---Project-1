/// <summary>
/// 
/// </summary>
/// <param name="img">Input image</param>
/// <param name="imgCopy">Image Copy</param>
/// <param name="limitSign">List of positions (Left-x,Top-y,Right-x,Bottom-y) of all detected limit signs</param>
/// <param name="warningSign">List of positions (Left-x,Top-y,Right-x,Bottom-y) of all detected warning signs</param>
/// <param name="prohibitionSign">List of positions (Left-x,Top-y,Right-x,Bottom-y) of all detected prohibition signs</param>
/// <param name="level">Level of image</param>
/// <returns>image</returns>
public static Image<Bgr, byte> Signs(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, out List<string[]> limitSign, out List<string[]> warningSign, out List<string[]> prohibitionSign, int level)
{
    limitSign = new List<string[]>();
    warningSign = new List<string[]>();
    prohibitionSign = new List<string[]>();


    string[] dummy_vector1 = new string[5];
    dummy_vector1[0] = "70";
    dummy_vector1[1] = "1160";
    dummy_vector1[2] = "330";
    dummy_vector1[3] = "1200";
    dummy_vector1[4] = "350";

    string[] dummy_vector2 = new string[5];
    dummy_vector2[0] = "-1";
    dummy_vector2[1] = "669";
    dummy_vector2[2] = "469";
    dummy_vector2[3] = "680";
    dummy_vector2[4] = "480";

    string[] dummy_vector3 = new string[5];
    dummy_vector3[0] = "-1";
    dummy_vector3[1] = "669";
    dummy_vector3[2] = "469";
    dummy_vector3[3] = "680";
    dummy_vector3[4] = "480";

    limitSign.Add(dummy_vector1);
    warningSign.Add(dummy_vector2);
    prohibitionSign.Add(dummy_vector3);


    return img;
}