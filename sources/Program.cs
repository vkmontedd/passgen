/*
 * =============================================================================
 *  Project:       Passgen Tool
 *  File:          Program.cs
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
using System.Windows.Forms;

namespace PassgenTool
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Forms.MainForm());
        }
    }
}
