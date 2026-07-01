using System;
using System.Collections.Generic;
using System.Text;

namespace C64
{
    internal class CharData
    {
        public byte[] Data { get; }       // במקום C64CharData
        public byte[] ScreenMap { get; }  // מפת המסך הלינארית
        public byte[] ColorRam { get; }   // זיכרון הצבעים הלינארי

        public byte BackgroundColor { get; set; }
        public byte Multicolor1 { get; set; }
        public byte Multicolor2 { get; set; }
    }
}
