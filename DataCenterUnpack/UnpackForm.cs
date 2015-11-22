using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using SharpDisasm;
using SharpDisasm.Udis86;

namespace DataCenterUnpack
{
    public partial class UnpackForm : Form
    {
        public UnpackForm()
        {
            InitializeComponent();
        }

        private void BrowseInput_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                InputFile.Text = openFileDialog1.FileName;
            }
        }

        private void Go_Click(object sender, EventArgs e)
        {
            try
            {
                var keyString = Key.Text.Replace(" ", "");
                var ivString = IV.Text.Replace(" ", "");

                var key = Unpacker.StringToByteArray(keyString);
                var iv = Unpacker.StringToByteArray(ivString);
                Unpacker.Unpack(InputFile.Text, InputFile.Text, key, iv);

                GC.Collect();

                MessageBox.Show("done");
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FindKeyButton_Click(object sender, EventArgs e)
        {
            try
            {
                var candidates = KeyScanner.Find();
                MessageBox.Show("Results:\r\n" + String.Join("\r\n", candidates.Select(x => x.Item1 + "    " + x.Item2)));
                if (candidates.Any())
                {
                    Key.Text = candidates.First().Item1;
                    IV.Text = candidates.First().Item2;
                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
