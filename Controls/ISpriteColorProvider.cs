using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C64.Controls
{
    public interface ISpriteColorProvider
    {
        // הפלטה החיצונית מקבלת את מספר הסלוט (0-3) ומחזירה צבע מוכן לציור ברשת!
        Color GetColorForSlot(int slotIndex);

        // מחזיר את הסלוט שהמשתמש בחר כרגע כדי לצייר איתו (0, 1, 2 או 3)
        byte SelectedSlotIndex { get; }

        event EventHandler ColorChanged;
    }
}
