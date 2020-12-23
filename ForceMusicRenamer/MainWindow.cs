using System;
using System.Windows.Forms;

namespace ForceMusicRenamer {
	public partial class MainWindow : Form {
		public MainWindow() {
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e) {
			if (textBox1.Text != string.Empty) {
				label3.Text = Business.RenameAllMusicByPath(textBox1.Text, checkBox1.Checked);
			}
		}

		private void textBox1_TextChanged(object sender, EventArgs e) {
			label3.Text = "Status:";
		}
	}
}