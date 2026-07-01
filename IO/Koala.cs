using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace C64.IO
{
    internal static class Koala
    {
        public static bool LoadKoalaFile(string filePath,out string error)
        {
            try
            {
                error = null;
                // Implement loading of Koala Painter files here
                if (string.IsNullOrEmpty(filePath))
                {
                    error = "Invalid file path.";
                    return false;
                }

                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);

                byte[] koalaData = br.ReadBytes((int)fs.Length);

                return true;

            } catch (FileLoadException ex)
            {
                error = $"Error loading Koala file: {ex.Message}";
                return false;
            }
        }
    }
}
