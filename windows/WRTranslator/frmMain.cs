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
        }

        private void btnTranslate_Click(object sender, EventArgs e)
        {

            txtTo.Text = "";

            lib_wrtranslation transl = new lib_wrtranslation();
            transl.LanguageFromTo = enumLangFromTo.English_Italian;
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
    }
}
