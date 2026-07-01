using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.PerformanceData;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace C64.Controls
{
    public partial class CharScreen : UserControl
    {
        byte[,] charData = new byte[256, 8];
        byte[] screenMap = new byte[1000];
        byte[] colorRam = new byte[1000];
        int CellsOnX = 40;
        int CellsOnY = 25;

        public CharScreen()
        {
            InitializeComponent();
        }

        public CharScreen(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        private bool _showGrid = false;
        [Category("Grid"), Description("Define if the grid lines are visible or not")]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowGrid
        {
            get => _showGrid;
            set
            {
                _showGrid = value;
                Refresh();
            }
        }

        public void ClearScreen()
        {
            Array.Clear(charData, 0, charData.Length);
            Array.Clear(screenMap, 0, screenMap.Length);
            Array.Clear(colorRam, 0, colorRam.Length);
            Refresh();
        }
        public void SetChar(int x, int y, byte charIndex, byte color)
        {
            if (x < 0 || x >= CellsOnX || y < 0 || y >= CellsOnY)
                throw new ArgumentOutOfRangeException("Coordinates out of bounds");
            int screenIndex = y * CellsOnX + x;
            screenMap[screenIndex] = charIndex;
            colorRam[screenIndex] = color;
            Refresh();

        }
        public void SetCharData(byte charIndex, byte[] charBytes)
        {
            if (charIndex < 0 || charIndex >= 256)
                throw new ArgumentOutOfRangeException("Character index out of bounds");
            if (charBytes.Length != 8)
                throw new ArgumentException("Character data must be exactly 8 bytes long");
            for (int i = 0; i < 8; i++)
            {
                charData[charIndex, i] = charBytes[i];
            }
            Refresh();
        }
        public void SetCharData(int charIndex, byte row, byte value)
        {
            if (charIndex < 0 || charIndex >= 256)
                throw new ArgumentOutOfRangeException("Character index out of bounds");
            if (row < 0 || row >= 8)
                throw new ArgumentOutOfRangeException("Row index out of bounds");
            charData[charIndex, row] = value;
            Refresh();
        }

        public void SetData(byte[,] charData, byte[] screenMap, byte[] colorRam)
        {
            if (charData.GetLength(0) != 256 || charData.GetLength(1) != 8)
                throw new ArgumentException("Character data must be a 256x8 array");
            if (screenMap.Length != 1000)
                throw new ArgumentException("Screen map must be exactly 1000 bytes long");
            if (colorRam.Length != 1000)
                throw new ArgumentException("Color RAM must be exactly 1000 bytes long");
            this.charData = charData;
            this.screenMap = screenMap;
            this.colorRam = colorRam;
            Refresh();
        }

        Bitmap screeDisplay = new Bitmap(320, 200); // 40 תווים לרוחב * 8 פיקסלים לתו = 320 פיקסלים, ו-25 תווים לגובה * 8 פיקסלים לתו = 200 פיקסלים


        public void DrawScreen()
        {
            // 1. נעילת ה-Bitmap לקריאה וכתיבה
            var rect = new Rectangle(0, 0, screeDisplay.Width, screeDisplay.Height);
            BitmapData bmpData = screeDisplay.LockBits(rect, ImageLockMode.ReadWrite, screeDisplay.PixelFormat);

            int bytesCount = bmpData.Stride * screeDisplay.Height;
            byte[] screenBytes = new byte[bytesCount];

            // העתקת הזיכרון הגולמי מה-Bitmap למערך המקומי
            Marshal.Copy(bmpData.Scan0, screenBytes, 0, screenBytes.Length);

            int stride = bmpData.Stride;

            // 2. לולאת הציור הפסיבית (40x25 תווים)
            for (int y = 0; y < CellsOnY; y++)
            {
                for (int x = 0; x < CellsOnX; x++)
                {
                    int screenIndex = y * CellsOnX + x;
                    byte charIndex = screenMap[screenIndex];
                    byte colorIndex = colorRam[screenIndex];

                    // שליפת הצבע מהפאלטה (הבקר רק קורא צבע, הוא לא מחשב לוגיקה)
                    Color charColor = C64.Graphics.C64Palette.ActivePalette[colorIndex];

                    for (int row = 0; row < 8; row++)
                    {
                        byte rowData = charData[charIndex, row];
                        int pixelY = (y * 8) + row;

                        if (pixelY >= screeDisplay.Height) continue;

                        // רינון Hi-Res טהור ופשוט: ביט דולק = צבע התו, ביט כבוי = רקע (שחור/צבע רקע)
                        for (int bit = 0; bit < 8; bit++)
                        {
                            bool pixelOn = (rowData & (1 << (7 - bit))) != 0;
                            int pixelX = (x * 8) + bit;

                            if (pixelX >= screeDisplay.Width) continue;

                            int byteIndex = (pixelY * stride) + (pixelX * 4);

                            if (pixelOn)
                            {
                                screenBytes[byteIndex] = charColor.B;
                                screenBytes[byteIndex + 1] = charColor.G;
                                screenBytes[byteIndex + 2] = charColor.R;
                                screenBytes[byteIndex + 3] = charColor.A;
                            }
                            else
                            {
                                // צבע רקע פסיבי (למשל שחור)
                                screenBytes[byteIndex] = 0;
                                screenBytes[byteIndex + 1] = 0;
                                screenBytes[byteIndex + 2] = 0;
                                screenBytes[byteIndex + 3] = 255;
                            }
                        }
                    }
                }
            }

            // 3. תיקון הבאגים: העתקה חזרה ל-Bitmap ושחרור הנעילה
            Marshal.Copy(screenBytes, 0, bmpData.Scan0, screenBytes.Length);
            screeDisplay.UnlockBits(bmpData);

            // 4. רענון וויזואלי
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            int LineWidth = ShowGrid ? 1 : 0;
            int CellWidth = Width / CellsOnX;
            int CellHeight = CellWidth;

            base.OnPaint(e);
            if (!DesignMode)
            {
                DrawScreen();
                e.Graphics.DrawImage(screeDisplay, 0, 0, screeDisplay.Width, screeDisplay.Height);
            }


            Pen LinePen = new Pen(Color.Black, LineWidth);

            for (int y = 0; y < (Height); y += CellHeight)
                e.Graphics.DrawLine(LinePen, 0, y, CellWidth * CellsOnX, y);

            for (int x = 0; x < (Width); x += CellWidth)
                e.Graphics.DrawLine(LinePen, x, 0, x, CellHeight * CellsOnY);

        }
    }
}
