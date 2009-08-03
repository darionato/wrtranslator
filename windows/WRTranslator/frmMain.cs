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
        }
    }
}
