using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace EbiSoft.EbIRC
{
    /// <summary>
    /// �e�L�X�g�̓��͂��s�����
    /// </summary>
    public partial class InputBoxForm : Form
    {
        public InputBoxForm()
        {
            InitializeComponent();
        }

        private void okMenuItem1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void cancelMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// ���͂��ꂽ�e�L�X�g
        /// </summary>
        public string Value
        {
            get { return textBox.Text; }
            set { textBox.Text = value; }
        }

        /// <summary>
        /// ���͍��ڂ̐���
        /// </summary>
        public string Description
        {
            get { return label.Text; }
            set { label.Text = value; }
        }
    }
}