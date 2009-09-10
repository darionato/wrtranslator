using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Badlydone.WRTranslator;

namespace WRTranslator
{
    public partial class frmMain : Form
    {

        private bool m_Close = false;
        private NotifyIcon m_Tray;

        public frmMain()
        {
            InitializeComponent();

            this.txtFrom.KeyDown += new KeyEventHandler(txtFrom_KeyDown);
        }

        void txtFrom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                Cursor = Cursors.WaitCursor;
                DoTranslation();
                Cursor = Cursors.Default;
            }
        }

        private void DoTranslation()
        {

            txtTo.Text = "";

            lib_wrtranslation transl = new lib_wrtranslation();
            transl.LanguageFromTo = (enumLangFromTo)Enum.Parse(typeof(enumLangFromTo), Convert.ToString(this.comboLangs.SelectedIndex + 1));
            transl.WordFrom = txtFrom.Text;

            transl.TranslateAsynch();
            transl.WaitDone();

            if (transl.WordTo.Length == 0) return;

            foreach (string w in transl.WordTo)
            {
                txtTo.Text += w + " - ";
            }

            if (txtTo.Text.EndsWith(" - "))
            {
                txtTo.Text = txtTo.Text.Substring(0, txtTo.Text.Length - 3);
            }

        }

        private void btnTranslate_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            DoTranslation();
            Cursor = Cursors.Default;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            this.comboLangs.SelectedIndex = 0;
            this.txtFrom.Text = Clipboard.GetText();

            m_Tray = new NotifyIcon();
            m_Tray.DoubleClick += new EventHandler(m_Tray_DoubleClick);
            m_Tray.ContextMenuStrip = this.contextMenuStrip1;
            m_Tray.Icon = this.Icon;
            m_Tray.Visible = true;

        }

        void m_Tray_DoubleClick(object sender, EventArgs e)
        {
            ShowForm();
        }

        private void ShowForm()
        {
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
            this.Visible = true;
            this.Show();
        }

        private void HideForm()
        {
            this.ShowInTaskbar = false;
            this.Hide();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_Close == false)
            {
                e.Cancel = true;
                this.HideForm();
                return;
            }

            m_Tray.Visible = false;

        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_Close = true;
            this.Close();
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            HideForm();
        }

    }
}
