using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vlc_arduino_fw_main {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private List<string> port = new List<string>();
        private void button1_Click(object sender, EventArgs e) {
            string[] arrport;
            ManagementObjectSearcher objOSDetails2 = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Caption like '%(COM%'");
            ManagementObjectCollection osDetailsCollection2 = objOSDetails2.Get();
            foreach (ManagementObject usblist in osDetailsCollection2) {
                if (usblist["Description"].ToString() != "USB-SERIAL CH340") continue;
                arrport = usblist.GetPropertyValue("NAME").ToString().Split('(', ')');
                port.Add(arrport[1]);
            }

            foreach (string portX in port) {
                File.WriteAllText("vlc_arduino_fw_port.txt", portX);
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.CreateNoWindow = false;
                startInfo.UseShellExecute = false;
                startInfo.FileName = "vlc_arduino_fw.exe";
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                File.Delete("call_exe_tric.txt");
                try { Process.Start(startInfo); } catch {
                    MessageBox.Show("call exe err " + portX);
                    return;
                }
                while (true) {
                    try { File.ReadAllText("call_exe_tric.txt"); break; } catch { }
                    DelaymS(50);
                }
            }
        }

        public static void DelaymS(int mS) {
            Stopwatch stopwatchDelaymS = new Stopwatch();
            stopwatchDelaymS.Restart();
            while (mS > stopwatchDelaymS.ElapsedMilliseconds) {
                if (!stopwatchDelaymS.IsRunning) stopwatchDelaymS.Start();
                Application.DoEvents();
            }
            stopwatchDelaymS.Stop();
        }
        private void Form1_Load(object sender, EventArgs e) {
            try { textBox1.Text = File.ReadAllText("vlc_arduino_fw_hex.txt"); } catch {
                File.WriteAllText("vlc_arduino_fw_hex.txt", "Design.hex");
                textBox1.Text = "Design.hex";
            }
        }
        private void textBox1_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode != Keys.Enter) return;
            File.WriteAllText("vlc_arduino_fw_hex.txt", textBox1.Text);
        }
    }
}
