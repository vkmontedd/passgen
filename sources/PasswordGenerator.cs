/*
 * =============================================================================
 *  Project:       Passgen Tool
 *  File:          PasswordGenerator.cs
 *  Author:        vkmontedd
 *  License:       GNU General Public License v3.0
 * 
 *  Description:
 *  ---------------------------------------------------------------------------
 *  Passgen Tool — это генератор паролей с открытым исходным кодом на GitHub.
 * 
 *  Разрешается копирование, распространение и модификация в рамках GPLv3.
 *  Программное обеспечение распространяется «как есть» без каких-либо гарантий.
 * 
 *  Copyright (c) 2025 vkmontedd
 * =============================================================================
 */

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace PassgenTool.Classes
{
    public static class PasswordGenerator
    {
        private const string Lowers = "abcdefghijklmnopqrstuvwxyz";
        private const string Uppers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string Digits = "0123456789";
        private const string Symbols = "!@#$%^&*()-_=+[]{}|;:,.<>?";

        public static string Generate(int length, bool useLower, bool useUpper, bool useDigits, bool useSymbols, bool excludeSimilar)
        {
            if (length <= 0) return string.Empty;

            var sets = new List<string>();
            if (useLower) sets.Add(Lowers);
            if (useUpper) sets.Add(Uppers);
            if (useDigits) sets.Add(Digits);
            if (useSymbols) sets.Add(Symbols);

            if (sets.Count == 0) return string.Empty;

            if (excludeSimilar)
            {
                for (int i = 0; i < sets.Count; i++)
                {
                    sets[i] = RemoveSimilar(sets[i]);
                }
            }

            var all = string.Concat(sets);

            var chars = new List<char>(length);
            foreach (var s in sets)
            {
                if (!string.IsNullOrEmpty(s))
                    chars.Add(GetRandomCharFrom(s));
            }

            for (int i = chars.Count; i < length; i++)
            {
                chars.Add(GetRandomCharFrom(all));
            }

            Shuffle(chars);
            return new string(chars.ToArray());
        }

        private static char GetRandomCharFrom(string s)
        {
            if (string.IsNullOrEmpty(s)) throw new ArgumentException("Character set must not be empty", nameof(s));
            int idx = RandomNumberGenerator.GetInt32(s.Length);
            return s[idx];
        }

        private static string RemoveSimilar(string s)
        {
            const string remove = "lI10Oo";
            var sb = new StringBuilder(s.Length);
            foreach (var c in s)
            {
                if (remove.IndexOf(c) < 0)
                    sb.Append(c);
            }
            return sb.ToString();
        }

        private static void Shuffle(List<char> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = RandomNumberGenerator.GetInt32(i + 1);
                var tmp = list[i];
                list[i] = list[j];
                list[j] = tmp;
            }
        }
    }
}
