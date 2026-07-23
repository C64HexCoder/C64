using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace C64.Graphics;

public class KMeansQuant
{
    const uint NOT_CLASTERED = 0xffffffff;
    static Dictionary<uint, int> colorHistogram = new Dictionary<uint, int>();
    static uint[,] ColorsTable;

    static Color[] palette;
    static uint[] K;

    static List<uint>[] Clusters = new List<uint>[15];

    static bool FoundBackground = false;
    static Color BackgroundColor;
    public static bool IgnoreBackground = false;
    public static ToolStripProgressBar toolStripProgressBar;

    public class ProgressBarEventArgs : EventArgs
    {
        public int Min { get; set; }
        public int Max { get; set; }
        public int Position { get; set; }
    }

    public static event EventHandler<ProgressBarEventArgs> ProgressBarUpdated;

    protected static void OnProgressBarUpdated(ProgressBarEventArgs args)
    {
        EventHandler<ProgressBarEventArgs> handler = ProgressBarUpdated;

        if (handler != null)
        {
            handler(null, args);
        }


    }


    /// <summary>
    /// Reduced the colors in bitmap to the number of colors requested in NumOfColors
    /// </summary>
    /// <param name="bitmap"></param>
    /// <param name="NumOfColors"></param>
    /// <returns>returns a bitmap with the reduced colors</returns>
    public static Bitmap ReduceColors(Bitmap bitmap, int NumOfColors)
    {
        palette = new Color[NumOfColors];

        CreateColorHistogram(bitmap);

        // if NumOfColors in Bitmap is less or equal to desired number of colors then exit
        if (colorHistogram.Count <= NumOfColors)
        {
            return bitmap;
        }
        NumOfColors = FoundBackground ? NumOfColors - 1 : NumOfColors;


        K = GetInitialKs(colorHistogram, NumOfColors);


        ColorsTable = new uint[colorHistogram.Count, 2];
        CopyColorsToTable();

        bool ChangedClaster;



        do
        {
            ChangedClaster = false;

            for (int i = 0; i < ColorsTable.GetLength(0); i++)
            {
                uint Cluster = ClosestCluster(ColorsTable[i, 0], NumOfColors);
                if (ColorsTable[i, 1] != Cluster)
                {
                    ColorsTable[i, 1] = Cluster;
                    ChangedClaster = true;
                }
            }

            AveragingKs();

        } while (ChangedClaster == true);

        CopyKsToPalette(NumOfColors);

        colorHistogram.Clear();
        return RecreateBitmap(bitmap);

    }
    /// <summary>
    /// Reduced the colors in bitmap to the number of colors requested in NumOfColors
    /// </summary>
    /// <param name="bitmap"></param>
    /// <param name="NumOfColors"></param>
    /// <returns>returns a bitmap with the reduced colors</returns>
    public static Bitmap C64ReduceColors(Bitmap bitmap)
    {
        int NumOfColors = 16;
        CreateColorHistogram(bitmap);

        // if NumOfColors in Bitmap is less or equal to desired number of colors then exit
        if (colorHistogram.Count <= 16)
        {
            return bitmap;
        }
        NumOfColors = FoundBackground ? NumOfColors - 1 : NumOfColors;


        PopulateC64Ks();


        ColorsTable = new uint[colorHistogram.Count, 2];
        CopyColorsToTable();

        bool ChangedClaster;



        do
        {
            ChangedClaster = false;

            for (int i = 0; i < ColorsTable.GetLength(0); i++)
            {
                uint Cluster = ClosestCluster(ColorsTable[i, 0], NumOfColors);
                if (ColorsTable[i, 1] != Cluster)
                {
                    ColorsTable[i, 1] = Cluster;
                    ChangedClaster = true;
                }
            }


        } while (ChangedClaster == true);


        colorHistogram.Clear();
        return RecreateBitmap(bitmap);

    }

    /// <summary>
    /// Reduced the colors in a colors List to the number of colors requested in NumOfColors
    /// </summary>
    /// <param name="colors"></param>
    /// <param name="NumOfColors"></param>
    /// <returns>returns a List with the number of colors requested</returns>
    public List<Color> ReduceColors(List<Color> colors, int NumOfColors)
    {
        palette = new Color[NumOfColors];

        CreateColorHistogram(colors);

        // if NumOfColors in Bitmap is less or equal to desired number of colors then exit
        if (colorHistogram.Count <= NumOfColors)
        {
            return colors;
        }

        NumOfColors = FoundBackground ? NumOfColors - 1 : NumOfColors;
        K = GetInitialKs(colorHistogram, NumOfColors);

        ColorsTable = new uint[colorHistogram.Count, 2];
        CopyColorsToTable();

        bool ChangedClaster;


        do
        {
            ChangedClaster = false;

            for (int i = 0; i < ColorsTable.GetLength(0); i++)
            {
                uint Cluster = ClosestCluster(ColorsTable[i, 0], NumOfColors);
                if (ColorsTable[i, 1] != Cluster)
                {
                    ColorsTable[i, 1] = Cluster;
                    ChangedClaster = true;
                }
            }

            AveragingKs();

        } while (ChangedClaster == true);

        CopyKsToPalette(NumOfColors);

        colorHistogram.Clear();
        return colors;

    }


    /// <summary>
    /// Reduces the colors in bitmap to the requested colors in NumOfColors
    /// This function is the same as ReduceColors, the only difference is that this function is Async
    /// </summary>
    /// <param name="bitmap"></param>
    /// <param name="NumOfColors"></param>
    /// <returns>returns a Task<Bitmap> with the number of colors requested.</returns>
    public static async Task<Bitmap> ReduceColorsAsync(Bitmap bitmap, int NumOfColors)
    {
        palette = new Color[NumOfColors];
        ProgressBarEventArgs args = new ProgressBarEventArgs();

        Stopwatch stopwatch = Stopwatch.StartNew();
        CreateColorHistogram(bitmap);

        // if NumOfColors in Bitmap is less or equal to desired number of colors then exit
        if (colorHistogram.Count <= NumOfColors)
        {
            return bitmap;
        }
        NumOfColors = FoundBackground ? NumOfColors - 1 : NumOfColors;


        K = GetInitialKs(colorHistogram, NumOfColors);


        ColorsTable = new uint[colorHistogram.Count, 2];
        CopyColorsToTable();

        int p = 0;
        bool ChangedClaster;



        do
        {
            ChangedClaster = false;

            args.Min = 0;
            args.Max = ColorsTable.GetLength(0);

            for (int i = 0; i < ColorsTable.GetLength(0); i++)
            {

                args.Position = i;

                //ProgressBarUpdated?.Invoke(null, args); // need to decide between the two methods
                OnProgressBarUpdated(args);


                uint Cluster = ClosestCluster(ColorsTable[i, 0], NumOfColors);
                if (ColorsTable[i, 1] != Cluster)
                {
                    ColorsTable[i, 1] = Cluster;
                    ChangedClaster = true;
                }
            }

            AveragingKs();

        } while (ChangedClaster == true);

        CopyKsToPalette(NumOfColors);

        colorHistogram.Clear();
        return RecreateBitmap(bitmap);

    }
    static void RandomKs(Bitmap bitmap, int NumOfColors)
    {
        Random random = new Random();
        K = new uint[NumOfColors];
        uint color;
        bool Transparent;

        for (int i = 0; i < NumOfColors; i++)
        {
            do
            {
                Transparent = false;

                int x = random.Next(0, bitmap.Width);
                int y = random.Next(0, bitmap.Height);

                color = (uint)bitmap.GetPixel(x, y).ToArgb();
                Transparent = (color >> 24) == 0 ? true : false;

            } while (IsColorExistInKs(color) || Transparent);

            K[i] = color;
        }
    }

    private static void CreateKsFromPalette(Color[] palette, int NumOfColors)
    {
        K = new uint[NumOfColors];
        for (int i = 0; i < NumOfColors; i++)
        {
            K[i] = (uint)palette[i].ToArgb();
        }
    }

    static void RandomKs12bitRGB(Bitmap bitmap, int NumOfColors)
    {
        Random random = new Random();
        K = new uint[NumOfColors];
        uint color;
        bool Transparent;

        for (int i = 0; i < NumOfColors; i++)
        {
            do
            {
                Transparent = false;

                int x = random.Next(0, bitmap.Width);
                int y = random.Next(0, bitmap.Height);

                Color tmpC = bitmap.GetPixel(x, y);
                color = (uint)ConvertRGBFrom24To12Bit(tmpC).ToArgb();
                Transparent = (color >> 24) == 0 ? true : false;

            } while (IsColorExistInKs(color) || Transparent);

            K[i] = color;
        }
    }
    /// <summary>
    /// בוחרת את ה-Ks ההתחלתיים המיטביים באופן דטרמיניסטי ושומרת על גיוון צבעים (K-Means++)
    /// </summary>
    public static uint[] GetInitialKs(Dictionary<uint, int> histogram, int NumOfColors)
    {
        uint[] initialKs = new uint[NumOfColors];
        List<uint> uniqueColors = histogram.Keys.ToList();

        if (uniqueColors.Count <= NumOfColors)
        {
            return uniqueColors.ToArray();
        }

        // 1. הצבע הראשון: הצבע השכיח ביותר בתמונה (למשל צבע הגוף/הרקע)
        uint firstColor = histogram.OrderByDescending(p => p.Value).First().Key;
        initialKs[0] = firstColor;

        // מערך ששומר לכל צבע בתמונה את המרחק המינימלי שלו מה-Ks שכבר נבחרו
        double[] minDistances = new double[uniqueColors.Count];
        for (int i = 0; i < minDistances.Length; i++)
        {
            minDistances[i] = double.MaxValue;
        }

        // 2. לולאה לבחירת שאר ה-Ks (מ-1 עד NumOfColors - 1)
        for (int k = 1; k < NumOfColors; k++)
        {
            uint lastSelectedK = initialKs[k - 1];
            int maxDistanceIndex = -1;
            double maxWeightedDistance = -1;

            for (int i = 0; i < uniqueColors.Count; i++)
            {
                uint currentColor = uniqueColors[i];

                // אם הצבע הזה כבר נבחר כ-K, נתעלם ממנו
                if (initialKs.Contains(currentColor))
                    continue;

                // מחשבים מרחק בין הצבע הנוכחי ל-K האחרון שנבחר
                double dist = GetDistance(currentColor, lastSelectedK);

                // עדכון המרחק הקרוב ביותר עבור הצבע הזה
                if (dist < minDistances[i])
                {
                    minDistances[i] = dist;
                }

                // --- הוקסם מתרחש כאן ---
                // משקללים את המרחק גוון עם נורמליזציה קלה של כמות הפיקסלים (Math.Sqrt)
                // זה מונע מפיקסל רעש יחיד לשבש, אך נותן עדיפות רבה לצבעים שונים גוונית (כמו העין)
                double countWeight = Math.Sqrt(histogram[currentColor]);
                double weightedDistance = minDistances[i] * countWeight;

                if (weightedDistance > maxWeightedDistance)
                {
                    maxWeightedDistance = weightedDistance;
                    maxDistanceIndex = i;
                }
            }

            if (maxDistanceIndex != -1)
            {
                initialKs[k] = uniqueColors[maxDistanceIndex];
            }
        }

        return initialKs;
    }
    static void RandomKs(List<Color> colors, int NumOfColors)
    {
        Random random = new Random();
        K = new uint[NumOfColors];
        uint color;
        bool Transparent;

        for (int i = 0; i < NumOfColors; i++)
        {
            do
            {
                Transparent = false;
                color = (uint)colors[random.Next(0, colors.Count)].ToArgb();
                Transparent = (color >> 24) == 0 ? true : false;

            } while (IsColorExistInKs(color) || Transparent);

            K[i] = color;
        }
    }

    static void RandomKsFromColorTable(Bitmap bitmap, int NumOfColors)
    {
        Random random = new Random();
        K = new uint[NumOfColors];
        uint color;

        for (int i = 0; i < NumOfColors; i++)
        {
            do
            {
                int idx = random.Next(0, ColorsTable.GetLength(0));
                color = ColorsTable[idx, 0];

            } while (IsColorExistInKs(color));

            K[i] = color;
        }

    }

    static void PopulateC64Ks()
    {
        K = new uint[16];

        for (int i = 0; i < 16; i++)
        {
            K[i] = (uint)C64Palette.ActivePalette[i].ToArgb();
        }
    }

    static bool IsColorExistInKs(uint color)
    {
        for (int i = 0; i < K.Length; i++)
        {
            if (K[i] == color)
                return true;
        }
        return false;
    }

    private static void CopyKsToPalette(int NumOfColors)
    {

        if (!FoundBackground)
        {
            for (int i = 0; i < NumOfColors; i++)
                palette[i] = Color.FromArgb((int)K[i]);
        }
        else
        {
            for (int i = 0; i < NumOfColors; i++)
                palette[i + 1] = Color.FromArgb((int)K[i]);
        }
    }

    static void AveragingKs()
    {
        List<uint> Sum = new List<uint>();

        for (int j = 0; j < K.Length; j++)

        {
            Sum.Clear();
            for (int i = 0; i < ColorsTable.GetLength(0); i++)
            {
                if (ColorsTable[i, 1] == j)
                    Sum.Add(ColorsTable[i, 0]);
            }

            if (Sum.Count != 0)
                K[j] = AvaragingList(Sum);
        }
    }
    static unsafe void AveragingKsOpt()
    {
        //List<uint> Sum = new List<uint>();
        uint Sum = 0;
        uint Count;
        uint red, green, blue;
        fixed (uint* ptr = ColorsTable)
        {
            for (int j = 0; j < K.Length; j++)

            {
                //Sum.Clear();
                Sum = 0;
                Count = 0;
                red = green = blue = 0;
                for (int i = 0; i < ColorsTable.GetLength(0); i++)
                {


                    if (ptr[i * 2 + 1] == j)
                    {
                        red += ((ptr[i * 2 + 0] >> 16) & 0xff);
                        green += ((ptr[i * 2 + 0] >> 8) & 0xff);
                        blue += (ptr[i * 2 + 0] & 0xff);

                        Count++;
                    }

                }

                red = red / Count;
                green = green / Count;
                blue = blue / Count;
                K[j] = 0xff000000 | red << 16 | green << 8 | blue;
            }
        }
    }
    static uint AvaragingList(List<uint> list)
    {
        uint Rsum = 0, Gsum = 0, Bsum = 0;
        int R, G, B;
        uint color = 0, Amount = 0, count = 0;
        for (int i = 0; i < list.Count; i++)
        {
            color = list[i];

            int R1 = (int)(color & 0x00ff0000) >> 16;
            int G1 = (int)(color & 0x0000ff00) >> 8;
            int B1 = (int)(color & 0x000000ff);
            Amount = (uint)colorHistogram[color];

            Rsum += (uint)(R1 * Amount);
            Gsum += (uint)(G1 * Amount);
            Bsum += (uint)(B1 * Amount);

            count += Amount;
        }
        Rsum = (uint)Math.Round((double)(Rsum / count));
        Gsum = (uint)Math.Round((double)(Gsum / count));
        Bsum = (uint)Math.Round((double)(Bsum / count));

        return 0xff000000 | Rsum << 16 | Gsum << 8 | Bsum;
    }

    static uint ClosestCluster(uint color, int NumOfColors)
    {
        uint SelectedCluster = 0;
        double distance = GetDistance(color, K[0]);

        for (int i = 1; i < NumOfColors; i++)
        {
            double TempDist = GetDistance(color, K[i]);
            if (TempDist < distance)
            {
                distance = TempDist;
                SelectedCluster = (uint)i;
            }
        }

        return SelectedCluster;
    }

    static void CopyColorsToTable()
    {
        for (int i = 0; i < colorHistogram.Count; i++)
        {
            ColorsTable[i, 0] = (uint)colorHistogram.Keys.ElementAt(i);
            ColorsTable[i, 1] = 0xff;
        }
    }

    /// <summary>
    /// בוחרת את ה-Ks ההתחלתיים המיטביים מתוך היסטוגרמת ה-12Bit שכבר נוצרה (K-Means++)
    /// </summary>
    public static uint[] GetInitialKs12BitAmiga(Dictionary<uint, int> histogram, int NumOfColors)
    {
        List<uint> uniqueColors = histogram.Keys.ToList();

        // אם כמות הצבעים הייחודיים בתמונה קטנה או שווה למספר הצבעים המבוקש - מחזירים אותם
        if (uniqueColors.Count <= NumOfColors)
        {
            return uniqueColors.ToArray();
        }

        uint[] initialKs = new uint[NumOfColors];

        // 1. הצבע הראשון: הצבע השכיח ביותר בהיסטוגרמת ה-12bit
        uint firstColor = histogram.OrderByDescending(p => p.Value).First().Key;
        initialKs[0] = firstColor;

        double[] minDistances = new double[uniqueColors.Count];
        for (int i = 0; i < minDistances.Length; i++)
        {
            minDistances[i] = double.MaxValue;
        }

        // 2. לולאה לבחירת שאר ה-Ks
        for (int k = 1; k < NumOfColors; k++)
        {
            uint lastSelectedK = initialKs[k - 1];
            int maxDistanceIndex = -1;
            double maxWeightedDistance = -1;

            for (int i = 0; i < uniqueColors.Count; i++)
            {
                uint currentColor = uniqueColors[i];

                if (initialKs.Contains(currentColor))
                    continue;

                double dist = GetDistance(currentColor, lastSelectedK);

                if (dist < minDistances[i])
                {
                    minDistances[i] = dist;
                }

                double countWeight = Math.Sqrt(histogram[currentColor]);
                double weightedDistance = minDistances[i] * countWeight;

                if (weightedDistance > maxWeightedDistance)
                {
                    maxWeightedDistance = weightedDistance;
                    maxDistanceIndex = i;
                }
            }

            if (maxDistanceIndex != -1)
            {
                initialKs[k] = uniqueColors[maxDistanceIndex];
            }
        }

        return initialKs;
    }
    public static Dictionary<uint, int> CreateColorHistogram(Bitmap bitmap)
    {
        // reset histogram for this run
        colorHistogram.Clear();

        for (int y = 0; y < bitmap.Height; y++)
            for (int x = 0; x < bitmap.Width; x++)
            {
                Color color = bitmap.GetPixel(x, y);

                // If a color is partially transparent, make a new color based on the transperancy
                if (color.A < 0xff && color.A != 0)
                {
                    float transperence = (float)color.A / 255;
                    color = Color.FromArgb((int)Math.Round(color.R * transperence), (int)Math.Round(color.G * transperence), (int)Math.Round(color.B * transperence));
                    bitmap.SetPixel(x, y, color);
                }

                if (color.A == 0xff)
                {
                    //color = Color.FromArgb(color.R, color.G, color.B);

                    if (!colorHistogram.ContainsKey((uint)color.ToArgb()))
                        colorHistogram.Add((uint)color.ToArgb(), 1);
                    else
                        colorHistogram[(uint)color.ToArgb()]++;
                }
                else if (color.A == 0) // added to prevent a bug the from some reason FoundBackground == true;
                    FoundBackground = true;

                //bitmap.SetPixel(x, y, Color.FromArgb(0, 0, 0, 0));         // replace it with background color
            }

        return colorHistogram;
    }

    public static Dictionary<uint, int> CreateColorHistogram12bitRGB(Bitmap bitmap)
    {
        colorHistogram.Clear();

        for (int y = 0; y < bitmap.Height; y++)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                Color color = bitmap.GetPixel(x, y);

                // 1. אם יש שקפיות חלקית - מחשבים Alpha Blending ב-24-bit מלא
                if (color.A < 0xff && color.A != 0)
                {
                    float transperence = (float)color.A / 255f;
                    color = Color.FromArgb(
                        255,
                        (int)Math.Round(color.R * transperence),
                        (int)Math.Round(color.G * transperence),
                        (int)Math.Round(color.B * transperence)
                    );
                }

                // 2. עכשיו ממירים את הצבע הסופי ל-12-bit של האמיגה
                color = ConvertRGBFrom24To12Bit(color);

                // 3. הוספה להיסטוגרמה
                if (color.A == 0xff)
                {
                    uint colorARGB = (uint)color.ToArgb();
                    if (!colorHistogram.ContainsKey(colorARGB))
                        colorHistogram.Add(colorARGB, 1);
                    else
                        colorHistogram[colorARGB]++;
                }
                else if (color.A == 0)
                {
                    FoundBackground = true;
                }
            }
        }

        return colorHistogram;
    }

    private static Color ConvertRGBFrom24To12Bit(Color color)
    {
        //return new Color.FromArgb(color.R & 0x0f, color.G & 0x0f, color.B & 0x0f );
        return Color.FromArgb(color.A << 24 | (color.R & 0xF0) << 16 | (color.G & 0xf0) << 8 | color.B & 0xF0);
    }

    private static uint ConvertRGBFrom24To12Bit(uint color)
    {
        return color & 0xfff0f0f0;
    }

    // Create color histogram for a list of Colors
    public static Dictionary<uint, int> CreateColorHistogram(List<Color> colors)
    {
        // Dictionary<Color, int> colorHistogram = new Dictionary<Color, int>();

        colorHistogram.Clear();

        for (int i = 0; i < colors.Count(); i++)
        {
            Color color = colors[i];

            if (color.A < 0xff && color.A != 0)
            {
                float transperence = (float)color.A / 255;
                color = Color.FromArgb((int)Math.Round(color.R * transperence), (int)Math.Round(color.G * transperence), (int)Math.Round(color.B * transperence));
                colors[i] = color;
            }

            if (color.A == 0xff)
            {
                //color = Color.FromArgb(color.R, color.G, color.B);

                if (!colorHistogram.ContainsKey((uint)color.ToArgb()))
                    colorHistogram.Add((uint)color.ToArgb(), 1);
                else
                    colorHistogram[(uint)color.ToArgb()]++;
            }
            else if (color.A == 0) // added to prevent a bug the from some reason FoundBackground == true;
                FoundBackground = true;
        }

        return colorHistogram;
    }

    public static void SortHistogram()
    {
        colorHistogram = colorHistogram.OrderByDescending(P => P.Value).ToDictionary(p => p.Key, p => p.Value);
    }

    private static double GetDistance(uint v1, uint v2)
    {
        int R1 = (int)(v1 & 0x00ff0000) >> 16;
        int G1 = (int)(v1 & 0x0000ff00) >> 8;
        int B1 = (int)(v1 & 0x000000ff);

        int R2 = (int)(v2 & 0x00ff0000) >> 16;
        int G2 = (int)(v2 & 0x0000ff00) >> 8;
        int B2 = (int)(v2 & 0x000000ff);


        double x = Math.Pow(B1 - B2, 2);

        Color test1 = Color.FromArgb((int)v1);

        return Math.Sqrt(Math.Pow(R1 - R2, 2) + Math.Pow(G1 - G2, 2) + Math.Pow(B1 - B2, 2));

    }

    private static float GetNormDistance(uint v1, uint v2)
    {

        float R1 = (float)((v1 & 0x00ff0000) >> 16) / 255f;
        float G1 = (float)((v1 & 0x0000ff00) >> 8) / 255f;
        float B1 = (float)(v1 & 0x000000ff);

        float R2 = (float)((v2 & 0x00ff0000) >> 16) / 255f;
        float G2 = (float)((v2 & 0x0000ff00) >> 8) / 255f;
        float B2 = (float)(v2 & 0x000000ff) / 255f;


        double x = Math.Pow(B1 - B2, 2);

        Color test1 = Color.FromArgb((int)v1);

        return (float)Math.Sqrt(Math.Pow(R1 - R2, 2) + Math.Pow(G1 - G2, 2) + Math.Pow(B1 - B2, 2));


    }

    // Replacing all pixel in bitmap with the new colors (K means) in the ColorTable.
    static public Bitmap RecreateBitmap(Bitmap bitmap)
    {
        Bitmap NewBitmap = new Bitmap(bitmap.Width, bitmap.Height);

        for (int y = 0; y < bitmap.Height; y++)
            for (int x = 0; x < bitmap.Width; x++)
            {
                Color color = bitmap.GetPixel(x, y);
                if (color.A != 0)
                {
                    //color = Color.FromArgb(color.R, color.G, color.B);

                    uint WhichK = FindColorInColorTable(color);
                    Color replacementColor = Color.FromArgb((int)K[WhichK]);
                    NewBitmap.SetPixel(x, y, replacementColor);
                }

            }
        return NewBitmap;
    }

    static uint FindColorInColorTable(Color color)
    {
        //uint colorRgb = (uint)color.ToKnownColor(); AI Told to remove this line because it does not work properly, for example if the color is Color.FromArgb(255, 0, 0) it will return 0 instead of 0xff0000
        uint colorARGB = (uint)color.ToArgb();

        for (int i = 0; i < ColorsTable.GetLength(0); i++)
        {
            if (colorARGB == ColorsTable[i, 0])
                return ColorsTable[i, 1];
        }
        return 0xff; // fail

        //int i = 0;

    }
}
