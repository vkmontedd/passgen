using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;

namespace PassgenTool.Forms
{
    public partial class MainForm : Form
    {
        private Label lblTitle = null!;
        private NumericUpDown nudLength = null!;
        private TrackBar tbLength = null!;
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
        private Label lblStrength = null!;

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
            Text = "PassgenTool";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(520, 400);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            lblTitle = new Label { Text = "PassgenTool", Font = new Font("Segoe UI", 14, FontStyle.Bold), AutoSize = true, Location = new Point(20, 20) };
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
            btnAbout.Click += (s, e) => MessageBox.Show("PassgenTool v1.0\nРазработчик: killpassed", "О программе", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Controls.Add(btnAbout);

            txtPassword = new TextBox { Location = new Point(20, 320), Width = 420, Height = 30, ReadOnly = true, Font = new Font("Consolas", 12, FontStyle.Regular) };
            Controls.Add(txtPassword);
        }

        private void BtnGenerate_Click(object? sender, EventArgs e)
        {
            txtPassword.Text = GeneratePassword((int)nudLength.Value);
        }

        private void BtnCopy_Click(object? sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPassword.Text))
            {
                Clipboard.SetText(txtPassword.Text);
                var t = new System.Windows.Forms.Timer { Interval = 1200 };
                t.Tick += (s, ev) =>
                {
                    t.Stop();
                    Text = "PassgenTool";
                };
                Text = "Скопировано!";
                t.Start();
            }
        }

        private string GeneratePassword(int length)
        {
            string lower = "abcdefghijklmnopqrstuvwxyz";
            string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string digits = "0123456789";
            string symbols = "!@#$%^&*()_-+=[]{};:,.<>?";
            string similar = "O0lI";

            string chars = "";
            if (cbLower.Checked) chars += lower;
            if (cbUpper.Checked) chars += upper;
            if (cbDigits.Checked) chars += digits;
            if (cbSymbols.Checked) chars += symbols;
            if (cbExcludeSimilar.Checked)
                foreach (char c in similar) chars = chars.Replace(c.ToString(), "");

            if (chars.Length == 0) chars = lower;

            var rnd = new Random();
            var pass = new char[length];
            for (int i = 0; i < length; i++)
                pass[i] = chars[rnd.Next(chars.Length)];
            return new string(pass);
        }
    }
}
