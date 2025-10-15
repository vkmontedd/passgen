/*
 * =============================================================================
 *  Project:       Passgen Tool
 *  File:          MainForm.cs
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
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;
using PassgenTool.Classes;

namespace PassgenTool.Forms
{
    public partial class MainForm : Form
    {
        private Label lblTitle = null!;
        private NumericUpDown nudLength = null!;
        private CheckBox cbLower = null!;
        private CheckBox cbUpper = null!;
        private CheckBox cbDigits = null!;
        private CheckBox cbSymbols = null!;
        private CheckBox cbExcludeSimilar = null!;
        private Button btnGenerate = null!;
        private Button btnCopy = null!;
        private TextBox txtPassword = null!;
        private Button btnAbout = null!;
        private Label lblLength = null!;

        private readonly string appTitle = "PassgenTool";

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

    private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        public MainForm()
        {
            InitializeComponent();
            ApplyTheme();
            SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            ApplyTitleBarTheme();
        }

        private void OnUserPreferenceChanged(object? sender, UserPreferenceChangedEventArgs e)
        {
            if (e.Category == UserPreferenceCategory.General)
            {
                ApplyTheme();
                ApplyTitleBarTheme();
            }
        }

        private void ApplyTheme()
        {
            bool dark = IsSystemInDarkMode();
            
            BackColor = dark ? Color.FromArgb(32, 32, 32) : Color.WhiteSmoke;
            ForeColor = dark ? Color.White : Color.Black;
            foreach (Control c in Controls)
            {
                c.BackColor = dark ? Color.FromArgb(32, 32, 32) : Color.WhiteSmoke;
                c.ForeColor = dark ? Color.White : Color.Black;
            }
        }

        private void ApplyTitleBarTheme()
        {
            if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 17763))
            {
                int useDark = IsSystemInDarkMode() ? 1 : 0;
                DwmSetWindowAttribute(Handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref useDark, sizeof(int));
            }
        }

        private bool IsSystemInDarkMode()
        {
            const string key = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
            using var regKey = Registry.CurrentUser.OpenSubKey(key);
            var value = regKey?.GetValue("AppsUseLightTheme");
            return value is int i && i == 0;
        }

        private void InitializeComponent()
        {
            Text = appTitle;
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(520, 400);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            lblTitle = new Label { Text = appTitle, Font = new Font("Segoe UI", 14, FontStyle.Bold), AutoSize = true, Location = new Point(20, 20) };
            Controls.Add(lblTitle);

            lblLength = new Label { Text = "Длина пароля:", Location = new Point(20, 70), AutoSize = true };
            Controls.Add(lblLength);

            nudLength = new NumericUpDown { Location = new Point(150, 70), Minimum = 4, Maximum = 64, Value = 16, Width = 80 };
            Controls.Add(nudLength);

            cbLower = new CheckBox { Text = "Строчные буквы (a-z)", Location = new Point(20, 110), Checked = true, AutoSize = true };
            cbUpper = new CheckBox { Text = "Заглавные буквы (A-Z)", Location = new Point(20, 140), Checked = true, AutoSize = true };
            cbDigits = new CheckBox { Text = "Цифры (0-9)", Location = new Point(20, 170), Checked = true, AutoSize = true };
            cbSymbols = new CheckBox { Text = "Символы (!@#$%)", Location = new Point(20, 200), Checked = true, AutoSize = true };
            cbExcludeSimilar = new CheckBox { Text = "Исключить похожие (O,0,l,I)", Location = new Point(20, 230), AutoSize = true };
            Controls.AddRange(new Control[] { cbLower, cbUpper, cbDigits, cbSymbols, cbExcludeSimilar });

            btnGenerate = new Button { Text = "Сгенерировать", Location = new Point(20, 270), Width = 150, Height = 30 };
            btnGenerate.Click += BtnGenerate_Click;
            Controls.Add(btnGenerate);

            btnCopy = new Button { Text = "Копировать", Location = new Point(190, 270), Width = 100, Height = 30 };
            btnCopy.Click += BtnCopy_Click;
            Controls.Add(btnCopy);

            btnAbout = new Button { Text = "О программе", Location = new Point(310, 270), Width = 130, Height = 30 };
            btnAbout.Click += (s, e) => MessageBox.Show("PassgenTool v1.1\nРазработчик: vkmontedd\nGitHub: https://github.com/vkmontedd/passgen", "О программе", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Controls.Add(btnAbout);

            txtPassword = new TextBox { Location = new Point(20, 320), Width = 420, Height = 30, ReadOnly = true, Font = new Font("Consolas", 12, FontStyle.Regular) };
            Controls.Add(txtPassword);
        }

        private void BtnGenerate_Click(object? sender, EventArgs e)
        {
            var result = PasswordGenerator.Generate(
                (int)nudLength.Value,
                cbLower.Checked,
                cbUpper.Checked,
                cbDigits.Checked,
                cbSymbols.Checked,
                cbExcludeSimilar.Checked);
            if (string.IsNullOrEmpty(result))
            {
                result = PasswordGenerator.Generate((int)nudLength.Value, true, false, false, false, false);
            }

            txtPassword.Text = result;
        }

        private void BtnCopy_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtPassword.Text))
                return;

            Clipboard.SetText(txtPassword.Text);
            Text = "Скопировано!";

            var t = new System.Windows.Forms.Timer { Interval = 1200 };
            t.Tick += (s, ev) =>
            {
                t.Stop();
                t.Dispose();
                Text = appTitle;
            };
            t.Start();
        }
        
    }
}
