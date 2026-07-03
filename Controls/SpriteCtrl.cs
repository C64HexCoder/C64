using C64.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinGraphics = System.Drawing.Graphics; // יצירת כינוי מקוצר


namespace C64.Controls
{
    public enum DrawingState
    {
        Pen,
        Line,
        Rectangle,
        Circle,
        Fill
    }

    public enum ShiftDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    public partial class SpriteCtrl : UserControl
    {
        Bitmap Buffer = new Bitmap (24,21);

        public SpriteCtrl()
        {
            InitializeComponent();
            DoubleBuffered = true; // להפחית ריצודים בעת הציור
                                   //ShowGrid = true;


        }

        public SpriteCtrl (Sprite sprite) : this()
        {
            ActiveSprite = sprite;
        }

        public Sprite ActiveSprite { get; set; } = new Sprite();

        public ISpriteColorProvider colorProvider;

        public ISpriteColorProvider ColorProvider
        {
            get { return colorProvider; }
            set
            {
                colorProvider = value;
                if (colorProvider != null)
                {
                    //colorProvider.ColorChanged += (s, e) => Invalidate(); // Redraw the control to reflect changes in color provider
                    colorProvider.ColorChanged += HandleOnColorChanged; // Subscribe to the ColorChanged event of the color provider
                }
                Invalidate();
            }
        }

        [Category("Grid")]
        [Description("Determines whether the pixel grid lines are visible.")]
        public bool ShowGrid
        {
            get => _showGrid;
            set { _showGrid = value; Invalidate(); }
        }
        private bool _showGrid = true; // ברירת מחדל דולקת, כי זה עורך
        public void HandleOnColorChanged(object sender, EventArgs e)
        {
            Invalidate(); // Redraw the control to reflect changes in color provider
        }

        Pen GridPen = new Pen(Color.Red);

        [Category("Grid"), Description("Determines the color of the Grid")]
        public Color GridColor {

            get
            {
                return GridPen.Color;
            }

            set
            {
                GridPen.Color = value;
                Invalidate();
            }
        }
    

        [Category("Sprite"),Description("Determine the drawing state of the Sprite Control")]
        public DrawingState CurrentDrawingState { get; set; }

        public byte MainColor = 4;
        public byte MultiColor1 = 2;
        public byte MainColor2 = 3;

        private byte SelectedColor
        {
            get
            {
                if (ColorProvider != null)
                {
                    return (byte)ColorProvider.SelectedSlotIndex;
                }
                else
                {
                    return MainColor; // ברירת מחדל לצבע הראשי אם אין ספק צבעים
                }
            }
        }

        internal Color GetColorFromIndex(byte colorIndex)
        {
            if (colorProvider != null)
            {
                return colorProvider.GetColorForSlot(colorIndex);
            }
            else
            {
                switch (colorIndex)
                {
                    case 0:
                        return Color.Transparent;
                        break;
                    case 1:
                        return Color.Green;
                        break;
                    case 2:
                        return Color.Blue;
                        break;
                    default:
                        return Color.Magenta; // צבע ברירת מחדל עבור אינדקסים לא מוכרים

                }
            }
        }
        // 2. החיבור האוטומטי: קורה מעצמו בזמן שהתוכנה עולה
        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            // אם ה-ColorProvider עדיין לא קושר ידנית ב-Properties, והתוכנה רצה כרגע (לא ב-Designer)
            if (colorProvider == null && !DesignMode && FindForm() != null)
            {
                // סורקים את כל הפקדים בטופס ומחפשים מישהו שמממש את ה-Interface שלנו
                foreach (Control ctrl in FindForm().Controls)
                {
                    if (ctrl is ISpriteColorProvider palette)
                    {
                        // מצאנו! משדכים אוטומטית
                        ColorProvider = palette;
                        break; // מצאנו אחד, מספיק טוב
                    }
                }
            }
        }
        public void SetDrawingMode(DrawingState mode)
        {
            CurrentDrawingState = mode;
        }



        [Browsable(false)] // מונע מהמאפיין להופיע בכלל בחלון ה-Properties
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] // אומר ל-Designer לא לכתוב את המערך הזה ב-Designer.cs

        private byte cellWidthHeight = 20;
        private byte MultiColorCellWidth;

        [Category("Grid"), Description("The width and height of each cell in pixels.")]
        public byte CellWidthHeight
        {
            get
            {
                return cellWidthHeight;
            }
            set
            {
                cellWidthHeight = value;
                Invalidate();
            }
        }

        private bool isMulticolor = false;
        [Category("Sprite"), Description("Determines whether the sprite is multicolor or not.")]
        public bool IsMulticolor
        {
            get
            {
                return ActiveSprite.IsMulticolor;
            }
            set
            {
                ActiveSprite.IsMulticolor = value;
                Invalidate();
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

          //  WinGraphics BufferGfx = WinGraphics.FromImage(Buffer);
         //   BufferGfx.Clear(Color.Transparent);
        
            Pen borderPen = new Pen(Color.White, 2);
            int ShrinkCell = 0;
            //e.Graphics.FillRectangle(Brushes.White, 0, 0, 24 * CellWidthHeight, 21 * CellWidthHeight);
            //e.Graphics.DrawRectangle(borderPen, 0 , 0 , 24 * CellWidthHeight, 21 * CellWidthHeight);
            if (ShowGrid)
            {
                ShrinkCell = 1;
                if (ActiveSprite.IsMulticolor)
                {
                    for (int x = 0; x < 24; x++)
                    {
                        e.Graphics.DrawLine(GridPen, x * CellWidthHeight, 0, x * CellWidthHeight, 21 * CellWidthHeight);
                    }
                    e.Graphics.DrawLine(GridPen, 24 * CellWidthHeight, 0, 24 * CellWidthHeight, 21 * CellWidthHeight);

                    for (int y = 0; y < 21; y++)
                    {
                        e.Graphics.DrawLine(GridPen, 0, y * CellWidthHeight, 24 * CellWidthHeight, y * CellWidthHeight);
                    }
                    e.Graphics.DrawLine(GridPen, 0, 21 * CellWidthHeight, 24 * CellWidthHeight, 21 * CellWidthHeight);
                }
                else
                {
                    MultiColorCellWidth = (byte)(CellWidthHeight * 2);

                    for (int x = 0; x < 12; x++)
                    {

                        e.Graphics.DrawLine(GridPen, x * CellWidthHeight * 2, 0, x * CellWidthHeight * 2, 21 * CellWidthHeight);
                    }
                    e.Graphics.DrawLine(GridPen, 12 * CellWidthHeight * 2, 0, 12 * CellWidthHeight * 2, 21 * CellWidthHeight);

                    for (int y = 0; y < 21; y++)
                    {
                        e.Graphics.DrawLine(GridPen, 0, y * CellWidthHeight, 24 * CellWidthHeight, y * CellWidthHeight);
                    }
                    e.Graphics.DrawLine(GridPen, 0, 21 * CellWidthHeight, 24 * CellWidthHeight, 21 * CellWidthHeight);
                }
            }
            Width = 24 * CellWidthHeight + 1;
            Height = 21 * CellWidthHeight + 1;

            Brush brush;

            for (int y = 0; y < ActiveSprite.Height; y++)
            {
                for (int x = 0; x < ActiveSprite.Width; x++)
                {
                    // קבלת אינדקס הצבע מהספרייט
                    int colorIndex = ActiveSprite[x, y];

                    // המרה לצבע ממשי (דרך ה-ColorProvider)
                    Color pixelColor = ColorProvider != null ?
                                       ColorProvider.GetColorForSlot(colorIndex) : Color.Black;

                    // ציור הפיקסל על הפקד
                    using (Brush b = new SolidBrush(pixelColor))
                    {
                        if (ActiveSprite.IsMulticolor)
                            e.Graphics.FillRectangle(b, x * MultiColorCellWidth, y * CellWidthHeight, MultiColorCellWidth-ShrinkCell, CellWidthHeight-ShrinkCell);
                        else
                            e.Graphics.FillRectangle(b, x * CellWidthHeight, y * CellWidthHeight, CellWidthHeight-ShrinkCell, CellWidthHeight-ShrinkCell);
                    }
                }
            }

        
            
            int SpriteWidth = ActiveSprite.IsMulticolor ? 12 : 24;
            if (ActiveSprite.IsMulticolor)
            {
                for (int y = 0; y < 21; y++)
                    for (int x = 0; x < SpriteWidth; x++)
                    {
                        Color color = Buffer.GetPixel(x, y);
                        if (color.A != 0x00)
                        {
                            brush = new SolidBrush(ColorProvider.GetColorForSlot(ColorProvider.SelectedSlotIndex));
                            if (IsMulticolor)
                            {
                                e.Graphics.FillRectangle(brush, x * MultiColorCellWidth + ShrinkCell, y * CellWidthHeight + ShrinkCell, MultiColorCellWidth - ShrinkCell, CellWidthHeight - ShrinkCell);

                            }
                            else
                                e.Graphics.FillRectangle(brush, x * CellWidthHeight + ShrinkCell, y * CellWidthHeight + ShrinkCell, CellWidthHeight - ShrinkCell, CellWidthHeight - ShrinkCell);
                        }
                    }

            }
                   
                

        }


        bool IsLineStarted = false;

        private PointF lineStartCell;
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            int cellX = e.X / (this.Width / ActiveSprite.Width);
            int cellY = e.Y / (this.Height / ActiveSprite.Height);

            switch (CurrentDrawingState)
            {
                case DrawingState.Pen:
     

                    SetCell(cellX, cellY);
                    //Invalidate();
                    break;
                case DrawingState.Line:

                    lineStartCell = new Point (cellX, cellY);
                    IsLineStarted = true;
                    break;
                case DrawingState.Rectangle:

                    lineStartCell = new Point (cellX, cellY);
                    IsLineStarted = true;
                    break;
                    
                case DrawingState.Fill:

                    FloodFill(cellX, cellY, SelectedColor);
                    break;
            }
         
  
        }

        private void SetCell (int cellX, int cellY)
        {
   
                ActiveSprite[cellX, cellY] = (byte)SelectedColor; // Update the ActiveSprite with the selected color
                Invalidate(); // Redraw the control to reflect changes

        }

        PointF LineEndCell;
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.X >= this.Width || e.Y >= this.Height || e.X < 0 || e.Y < 0)
                return; // Prevent drawing outside the control bounds
            Pen drawingPen = new Pen(ColorProvider.GetColorForSlot(SelectedColor), 1);


            System.Drawing.Graphics g = CreateGraphics();

            if (e.Button == MouseButtons.Left)
            {
                int cellX = e.X / (this.Width / ActiveSprite.Width);
                int cellY = e.Y / (this.Height / ActiveSprite.Height);

                WinGraphics BufferGfx = WinGraphics.FromImage(Buffer);

                if ((CurrentDrawingState != DrawingState.Pen))
                {
                    switch (CurrentDrawingState)
                    {
                        case DrawingState.Line:

                            if (IsLineStarted)
                            {
                             

                                LineEndCell = new Point(cellX, cellY);
                                BufferGfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                                BufferGfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                                BufferGfx.Clear(Color.Transparent);
                                BufferGfx.DrawLine(drawingPen,lineStartCell, LineEndCell);
                                BufferGfx.Dispose();
                                Invalidate();
                                // Implement line drawing logic based on lineStartPoint and current mouse position
                                //g.DrawLine(Pens.Red, lineStartPoint.Value.X * CellWidthHeight, lineStartPoint.Value.Y * CellWidthHeight, cellX * CellWidthHeight, cellY * CellWidthHeight);
                            }
                            break;
                        case DrawingState.Rectangle:

                            LineEndCell = new Point(cellX, cellY);
                            BufferGfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                            BufferGfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                            BufferGfx.Clear(Color.Transparent);
                            BufferGfx.DrawRectangle(drawingPen,lineStartCell.X,lineStartCell.Y,Math.Abs(LineEndCell.X-lineStartCell.X),Math.Abs(LineEndCell.Y-lineStartCell.Y));
                    
                            BufferGfx.Dispose();
                            Invalidate();
                            break;
                    }
                }
                else {
                    if (ActiveSprite.IsMulticolor)
                    {
                        ActiveSprite[cellX, cellY] = SelectedColor;             
                        Invalidate(); // Redraw the control to reflect changes
                    }
                    else
                    {
                        
                        if (ColorProvider.SelectedSlotIndex == 0)
                            ActiveSprite[cellX, cellY] = 0; // Set the pixel to 0 (transparent)  
                        else
                            ActiveSprite[cellX, cellY] = 1; // Set the pixel to 1 (opaque)

                        Invalidate(); // Redraw the control to reflect changes
                        
                    }
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (CurrentDrawingState == DrawingState.Line || CurrentDrawingState == DrawingState.Rectangle)
            {
                // Draw the final line based on the starting point and the current mouse position into the SpriteData
                int X = e.X / CellWidthHeight;
                int Y = e.Y / CellWidthHeight;
                InjectBufferToSpriteDate();

                IsLineStarted = false;
            }
        }

        private void InjectBufferToSpriteDate ()
        {
            int SpriteWIdth = ActiveSprite.IsMulticolor ? 12 : 24;

            for (int y=0; y < 21; y++)
                for (int x=0; x < SpriteWIdth; x++)
                {
                    if (Buffer.GetPixel(x,y).A != 0x00)
                    {
                        SetCell (x,y);
                    }
                }

            System.Drawing.Graphics BufferGfx = System.Drawing.Graphics.FromImage(Buffer);
            BufferGfx.Clear(Color.Transparent);
            BufferGfx.Dispose();

            Invalidate();
        }

        private void UpdateSpriteDataFromGrid()
        {
            // This method can be used to update the SpriteData array based on the current state of the grid
            // It can be called after drawing operations to ensure SpriteData is in sync with the visual representation


        }

        public void ClearSprite()
        {
            ActiveSprite.Clear();
            Invalidate(); // Redraw the control to reflect changes
        }

        class SpriteSelectorEventArgs : EventArgs
        {
            public byte ColorIndex { get; }
            public SpriteSelectorEventArgs(byte colorIndex)
            {
                ColorIndex = colorIndex;
            }
        }


        public byte GetCellColorIndex(int x, int y)
        {
            if (ActiveSprite.IsMulticolor)
            {
                if (x >= 12 || y >= 21)
                    throw new ArgumentException("X,Y Out of range");

                return (byte)ActiveSprite[x, y];

            }
            else
            {
                if (x >= 24 || y >= 21) throw new ArgumentException("X,Y Out of rage");

                return (byte)ActiveSprite[x, y];


            }
        }

        public void SetCellColorIndex(byte x, byte y, byte colorIndex)
        {
            if (ActiveSprite.IsMulticolor)
            {
                if (x >= 12 || y >= 21) return;

                ActiveSprite[x, y] = colorIndex;
            }
            else
            {
                if (x >= 24 || y >= 21) return;

                ActiveSprite[x, y] = colorIndex;
            }
        }

        public void FloodFill(int startX, int startY, byte targetColorIndex)
        {
            int currentWidth = ActiveSprite.IsMulticolor ? 12 : 24;

            if (startX < 0 || startX >= currentWidth || startY < 0 || startY >= 21) return;

            // קריאת אינדקס הצבע המקורי של המשבצת שנלחצה
            byte originalColorIndex = GetCellColorIndex(startX, startY);

            // אם הצבע המקורי כבר שווה לצבע החדש, אין צורך לבצע מילוי
            if (originalColorIndex == targetColorIndex) return;

            // יצירת תור של נקודות לסריקת השטח המחובר
            Queue<Point> queue = new Queue<Point>();
            queue.Enqueue(new Point(startX, startY));

            while (queue.Count > 0)
            {
                Point p = queue.Dequeue();

                // בדיקה שהנקודה הנוכחית בתוך גבולות הגריד
                if (p.X < 0 || p.X >= currentWidth || p.Y < 0 || p.Y >= 21) continue;

                // אם המשבצת הנוכחית היא בצבע המקורי שאותו אנו מחליפים
                if (GetCellColorIndex((byte)p.X, (byte)p.Y) == originalColorIndex)
                {
                    // צביעת המשבצת לצבע החדש
                    SetCellColorIndex((byte)p.X, (byte)p.Y, targetColorIndex);

                    // הוספת 4 השכנים הישירים (ימין, שמאל, למטה, למעלה) אל התור להמשך סריקה
                    queue.Enqueue(new Point(p.X + 1, p.Y));
                    queue.Enqueue(new Point(p.X - 1, p.Y));
                    queue.Enqueue(new Point(p.X, p.Y + 1));
                    queue.Enqueue(new Point(p.X, p.Y - 1));
                }
            }

            // מרעננים את הפקד באופן אקטיבי כדי שהשינוי הגרפי יוצג מיידית על המסך
            Invalidate();
        }
        public void Shift(ShiftDirection direction)
        {
            int currentWidth = ActiveSprite.IsMulticolor ? 12 : 24;
            int currentHeight = 21;

            // 1. יוצרים מטריצה זמנית בגודל הספרייט ושומרים בה את אינדקסי הצבעים הנוכחיים
            byte[,] tempGrid = new byte[currentWidth, currentHeight];
            for (byte y = 0; y < currentHeight; y++)
            {
                for (byte x = 0; x < currentWidth; x++)
                {
                    tempGrid[x, y] = GetCellColorIndex(x, y);
                }
            }

            // 2. מנקים את הספרייט הנוכחי לאפסים (כדי שהשוליים החדשים שנוצרים יהיו ריקים/שקופים)
            ClearSprite();

            // 3. מעתיקים חזרה עם ההיסט (Offset) המתאים לפי הכיוון
            for (byte y = 0; y < currentHeight; y++)
            {
                for (byte x = 0; x < currentWidth; x++)
                {
                    int targetX = x;
                    int targetY = y;

                    switch (direction)
                    {
                        case ShiftDirection.Up: targetY = y - 1; break;
                        case ShiftDirection.Down: targetY = y + 1; break;
                        case ShiftDirection.Left: targetX = x - 1; break;
                        case ShiftDirection.Right: targetX = x + 1; break;
                    }

                    // כותבים חזרה רק אם הפיקסל המוזז עדיין נמצא בתוך גבולות הספרייט
                    if (targetX >= 0 && targetX < currentWidth && targetY >= 0 && targetY < currentHeight)
                    {
                        SetCellColorIndex((byte)targetX, (byte)targetY, tempGrid[x, y]);
                    }
                }
            }

            // 4. מרעננים את התצוגה הויזואלית ב-Live
            Invalidate();
        }

       

    }

}
