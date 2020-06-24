﻿using System.Collections.Concurrent;
using UnityEngine;

namespace QFSW.QC.Utilities
{
    public static class ColorExtensions
    {
        /// <summary>Colors a string using rich formatting.</summary>
        /// <returns>The formatted text.</returns>
        /// <param name="text">The text to color.</param>
        /// <param name="color">The color to add to the text.</param>
        public static string ColorText(this string text, Color color)
        {
            if (string.IsNullOrWhiteSpace(text)) { return text; }
            string hexColor = Color32ToStringNonAlloc(color);
            return $"<color=#{hexColor}>{text}</color>";
        }

        private static readonly ConcurrentDictionary<int, string> _colorLookupTable = new ConcurrentDictionary<int, string>();
        private static unsafe string Color32ToStringNonAlloc(Color32 color)
        {
            int colorKey = color.r << 24 | color.g << 16 | color.b << 8 | color.a;
            if (_colorLookupTable.ContainsKey(colorKey))
            {
                return _colorLookupTable[colorKey];
            }

            char* buffer = stackalloc char[8];
            Color32ToHexNonAlloc(color, buffer);

            int bufferLength = color.a < 0xFF ? 8 : 6;
            string colorText = new string(buffer, 0, bufferLength);

            _colorLookupTable[colorKey] = colorText;
            return colorText;
        }

        private static unsafe void Color32ToHexNonAlloc(Color32 color, char* buffer)
        {
            ByteToHex(color.r, out buffer[0], out buffer[1]);
            ByteToHex(color.g, out buffer[2], out buffer[3]);
            ByteToHex(color.b, out buffer[4], out buffer[5]);
            ByteToHex(color.a, out buffer[6], out buffer[7]);
        }

        private static void ByteToHex(byte value, out char dig1, out char dig2)
        {
            dig1 = NibbleToHex((byte)(value >> 4));
            dig2 = NibbleToHex((byte)(value & 0xF));
        }

        private static char NibbleToHex(byte nibble)
        {
            if (nibble < 10) { return (char)('0' + nibble); }
            else { return (char)('A' + nibble - 10); }
        }
    }
}