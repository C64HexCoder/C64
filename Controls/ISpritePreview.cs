using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace C64.Controls
{
    public interface ISpritePreview
    {
        // מחזיר את אינדקס הצבע עבור מיקום לוגי (x, y) בספרייט
        Color GetPreviewPixel(int x, int y);
        event EventHandler SpriteChanged;

        int PreviewWidth { get; }
        int PreviewHeight { get; }
        bool IsMultiColor { get; }
    }
}
