using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Threading;

namespace ParaPROG_29_39_49_5V
{
    public partial class Form1 : Form
    {
        public int step;
        public int alowsend = 0;
        public int memsize = 0;
        public byte[] box = new byte[5];
        public byte[] buf; //temp buf for data
        public byte[] verifybuf;
        public int adr = 0; //temp buf for data
        public double time = 0;
        int maxtime = 3000;

        public Form1()
        {
            InitializeComponent();
            buf = new byte[0];
            backgroundWorker1.DoWork += Shaw_DoWork;
            backgroundWorker1.WorkerSupportsCancellation = true;
        }

        private void Detectprog_button_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
            serialPort1.PortName = comboBox2.Text;
            try
            {
                serialPort1.Open();
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message);
            }
            if (serialPort1.IsOpen)
            {
                System.Threading.Thread.Sleep(1000);
                Detectchip_button.Enabled = true;
                textBox8.Text = "Programmer detected";
                textBox8.BackColor = System.Drawing.Color.Green;
                Detectprog_button.Enabled = false;
                comboBox2.Enabled = false;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            serialPort1.Close();
            serialPort1.PortName = comboBox2.Text;
            Detectprog_button.Enabled = true;
        }

        private void comboBox2_MouseClick(object sender, MouseEventArgs e)
        {
            string[] portnames = SerialPort.GetPortNames();
            comboBox2.Items.Clear();
            comboBox2.Items.AddRange(portnames);
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            switch (step)
            {
                case 1:     //is it you?
                    this.Invoke(new EventHandler(IDrequest));
                    break;
                case 2:    //read Atmel signature
                    this.Invoke(new EventHandler(Step2));
                    break;
                case 3:
                    this.Invoke(new EventHandler(Step3));
                    break;
                case 4:
                    this.Invoke(new EventHandler(Step4));
                    break;
            }
        }

        private void IDrequest(object s, EventArgs e)
        {
            Open_file_button.Enabled = true;
            Read_button.Enabled = true;
            Save_file_button.Enabled = true;
            Erase_button.Enabled = true;
            Write_button.Enabled = true;
            Verify_button.Enabled = true;

            textBox7.Text = "Unknown or No Chip";
            while (serialPort1.BytesToRead < 2) { }
            ;
            int bytes = serialPort1.BytesToRead; //quantity of bytes in the buffer
            byte[] buffer = new byte[bytes]; //create an array of buffer size
            serialPort1.Read(buffer, 0, bytes); //read bytes to array
            var h = Convert.ToString(buffer[0], 16);
            while (h.Length < 2) { h = "0" + h; }
            textBox7.Text = h;
            h = Convert.ToString(buffer[1], 16);
            while (h.Length < 2) { h = "0" + h; }
            textBox6.Text = h;
            /*manufacturer IDs*/
            if (textBox7.Text == "00" || textBox7.Text == "ff") { textBox7.Text = "Unknown or No Chip"; }
            else if (textBox7.Text == "da") { textBox7.Text = "Winbond"; }
            else if (textBox7.Text == "bf") { textBox7.Text = "SST"; }
            else if (textBox7.Text == "1f") { textBox7.Text = "Atmel"; }
            else if (textBox7.Text == "01") { textBox7.Text = "AMD"; }
            else if (textBox7.Text == "ad") { textBox7.Text = "Hynix"; }
            else if (textBox7.Text == "37") { textBox7.Text = "Amic"; }
            else if (textBox7.Text == "c2") { textBox7.Text = "MXIC"; }
            else if (textBox7.Text == "9d") { textBox7.Text = "PMC"; }
            else if (textBox7.Text == "89") { textBox7.Text = "Intel"; }
            else if (textBox7.Text == "20") { textBox7.Text = "STMicroelectronics"; }
            else if (textBox7.Text == "04") { textBox7.Text = "Fujitsu"; }
            else if (textBox7.Text == "1c") { textBox7.Text = "EON"; }
            else if (textBox7.Text == "7f") { textBox7.Text = "EON"; } //it's for EN29F002 / EN29F002N
            /*chip IDs*/
            if (textBox7.Text == "Unknown or No Chip")
            {
                textBox6.Text = "Unknown or No Chip";
                textBox5.Text = "Unknown or No Chip";
                memsize = 0;
            }
            else if (textBox7.Text == "Winbond")
            {
                if (textBox6.Text == "0b") { textBox6.Text = "W49F002U/N"; textBox5.Text = "262144 bytes (2Mbit)"; memsize = 262144; }
                else if (textBox6.Text == "45") { textBox6.Text = "W29C020"; textBox5.Text = "262144 bytes (2Mbit)"; memsize = 262144; }
                else if (textBox6.Text == "46") { textBox6.Text = "W29C040"; textBox5.Text = "524288 bytes (4Mbit)"; memsize = 524288; }
                else if (textBox6.Text == "c8") { textBox6.Text = "W29EE512"; textBox5.Text = "65536 bytes (0.5Mbit)"; memsize = 65536; }
                else if (textBox6.Text == "c1") { textBox6.Text = "W29EE011"; textBox5.Text = "131072 bytes (1Mbit)"; memsize = 131072; }
                else if (textBox6.Text == "1a") { textBox6.Text = "W39F010"; textBox5.Text = "131072 bytes (1Mbit)"; memsize = 131072; }
            }
            else if (textBox7.Text == "SST")
            {
                if (textBox6.Text == "b5") { textBox6.Text = "SST39SF010"; textBox5.Text = "131072 bytes (1Mbit)"; memsize = 131072; }
                else if (textBox6.Text == "b6") { textBox6.Text = "SST39SF020"; textBox5.Text = "262144 bytes (2Mbit)"; memsize = 262144; }
                else if (textBox6.Text == "b7") { textBox6.Text = "SST39SF040"; textBox5.Text = "524288 bytes (4Mbit)"; memsize = 524288; }
                else if (textBox6.Text == "07") { textBox6.Text = "SST29EE010"; textBox5.Text = "131072 bytes (1Mbit)"; memsize = 131072; }
                else if (textBox6.Text == "10") { textBox6.Text = "SST29EE020"; textBox5.Text = "262144 bytes (2Mbit)"; memsize = 262144; }
            }
            else if (textBox7.Text == "Atmel")
            {
                if (textBox6.Text == "dc") { textBox6.Text = "AT29C257"; textBox5.Text = "32768 bytes (0.25Mbit)"; memsize = 32768; }
                else if (textBox6.Text == "5d") { textBox6.Text = "AT29C512"; textBox5.Text = "65536 bytes (0.5Mbit)"; memsize = 65536; }
                else if (textBox6.Text == "d5") { textBox6.Text = "AT29C010"; textBox5.Text = "131072 bytes (1Mbit)"; memsize = 131072; }
                else if (textBox6.Text == "da") { textBox6.Text = "AT29C020"; textBox5.Text = "262144 bytes (2Mbit)"; memsize = 262144; }
                else if (textBox6.Text == "a4") { textBox6.Text = "AT29C040A"; textBox5.Text = "524288 bytes (4Mbit)"; memsize = 524288; }
                else if (textBox6.Text == "03") { textBox6.Text = "AT49F512"; textBox5.Text = "65536 bytes (0.5Mbit)"; memsize = 65536; }
                else if (textBox6.Text == "05") { textBox6.Text = "AT49F001(N)"; textBox5.Text = "131072 bytes (1Mbit)"; memsize = 131072; }
                else if (textBox6.Text == "04") { textBox6.Text = "AT49F001(N)T"; textBox5.Text = "131072 bytes (1Mbit)"; memsize = 131072; }
                else if (textBox6.Text == "07") { textBox6.Text = "AT49F002(N)"; textBox5.Text = "262144 bytes (2Mbit)"; memsize = 262144; }
                else if (textBox6.Text == "08") { textBox6.Text = "AT49F002(N)T"; textBox5.Text = "262144 bytes (2Mbit)"; memsize = 262144; }
                else if (textBox6.Text == "17") { textBox6.Text = "AT49F/HF010"; textBox5.Text = "131072 bytes (1Mbit)"; memsize = 131072; }
                else if (textBox6.Text == "0b") { textBox6.Text = "AT49F020"; textBox5.Text = "262144 bytes (2Mbit)"; memsize = 262144; }
                else if (textBox6.Text == "13") { textBox6.Text = "AT49F040"; textBox5.Text = "524288 bytes (4Mbit)"; memsize = 524288; }
                else if (textBox6.Text == "0a") { textBox6.Text = "AT49F002(N)T"; textBox5.Text = "262144 bytes (2Mbit)"; memsize = 262144; }
            }
            else if (textBox7.Text == "AMD")
            {
                if (textBox6.Text == "20") { textBox6.Text = "AM29F010"; textBox5.Text = "131072 bytes (1Mbit)"; memsize = 131072; }
                else if (textBox6.Text == "a4") { textBox6.Text = "AM29F040"; textBox5.Text = "524288 bytes (4Mbit)"; memsize = 524288; }
            }
            else if (textBox7.Text == "Hynix")
            {
                if (textBox6.Text == "b0") { textBox6.Text = "HY29F002T"; textBox5.Text = "262144 bytes (2Mbit)"; memsize = 262144; }
            }
            else if (textBox7.Text == "Amic")
            {
                if (textBox6.Text == "a1") { textBox6.Text = "A29001"; textBox5.Text = "131072 bytes (1Mbit)"; memsize = 131072; }
                else if (textBox6.Text == "a4") { textBox6.Text = "A29010"; textBox5.Text = "131072 bytes (1Mbit)"; memsize = 131072; }
                else if (textBox6.Text == "8c") { textBox6.Text = "A290020(21)"; textBox5.Text = "262144 bytes (2Mbit)"; memsize = 262144; }
                else if (textBox6.Text == "86") { textBox6.Text = "A29040"; textBox5.Text = "524288 bytes (4Mbit)"; memsize = 524288; }
            }
            else if (textBox7.Text == "MXIC")
            {
                if (textBox6.Text == "1a") { textBox6.Text = "MX28F1000P"; textBox5.Text = "131072 bytes (1Mbit)"; memsize = 131072; }
                else if (textBox6.Text == "18") { textBox6.Text = "MX29F001T"; textBox5.Text = "131072 bytes (1Mbit)"; memsize = 131072; }
                else if (textBox6.Text == "19") { textBox6.Text = "MX29F001B"; textBox5.Text = "131072 bytes (1Mbit)"; memsize = 131072; }
                else if (textBox6.Text == "b0") { textBox6.Text = "MX29F002T"; textBox5.Text = "262144 bytes (2Mbit)"; memsize = 262144; }
                else if (textBox6.Text == "34") { textBox6.Text = "MX29F002B"; textBox5.Text = "262144 bytes (2Mbit)"; memsize = 262144; }
                else if (textBox6.Text == "a4") { textBox6.Text = "MX29F040"; textBox5.Text = "524288 bytes (4Mbit)"; memsize = 524288; }
                else if (textBox6.Text == "a6") { textBox6.Text = "MX29F040C"; textBox5.Text = "524288 bytes (4Mbit)"; memsize = 524288; }
                else if (textBox6.Text == "2a") { textBox6.Text = "MX28F2000P"; textBox5.Text = "262144 bytes (2Mbit)"; memsize = 262144; }
            }
            else if (textBox7.Text == "PMC")
            {
                if (textBox6.Text == "1c") { textBox6.Text = "Pm39F010"; textBox5.Text = "131072 bytes (1Mbit)"; memsize = 131072; }
                else if (textBox6.Text == "4d") { textBox6.Text = "Pm39F020"; textBox5.Text = "262144 bytes (2Mbit)"; memsize = 262144; }
                else if (textBox6.Text == "4e") { textBox6.Text = "Pm39F040"; textBox5.Text = "524288 bytes (4Mbit)"; memsize = 524288; }
            }
            else if (textBox7.Text == "Fujitsu")
            {
                if (textBox6.Text == "34") { textBox6.Text = "MBM29F002B"; textBox5.Text = "262144 bytes (2Mbit)"; memsize = 262144; }
                else if (textBox6.Text == "b0") { textBox6.Text = "MBM29F002T"; textBox5.Text = "262144 bytes (2Mbit)"; memsize = 262144; }
                else if (textBox6.Text == "dc") { textBox6.Text = "MBM29F002ST"; textBox5.Text = "262144 bytes (2Mbit)"; memsize = 262144; }
                else if (textBox6.Text == "5b") { textBox6.Text = "MBM29F002SB"; textBox5.Text = "262144 bytes (2Mbit)"; memsize = 262144; }
                else if (textBox6.Text == "a4") { textBox6.Text = "MBM29F040"; textBox5.Text = "524288 bytes (4Mbit)"; memsize = 524288; }
            }
            else if (textBox7.Text == "EON")
            {
                if (textBox6.Text == "04") { textBox6.Text = "EN29F040"; textBox5.Text = "524288 bytes (4Mbit)"; memsize = 524288; }
                else if (textBox6.Text == "20") { textBox6.Text = "EN29F010"; textBox5.Text = "131072 bytes (1Mbit)"; memsize = 131072; }
                else if (textBox6.Text == "7f") { textBox6.Text = "EN29F002/EN29F002N"; textBox5.Text = "262144 bytes (2Mbit)"; memsize = 262144; }
            }

            //0- no reset pin (A18 address instead); 1-reset pin
            if (memsize > 262144) { box[0] = 0x00; } else { box[0] = 0x01; }
            ; box[1] = 0x00; box[2] = 0x00; box[3] = 0x00; box[4] = 0x03;
            try { serialPort1.Write(box, 0, 1); }
            catch { }
            try { serialPort1.Write(box, 1, 1); }
            catch { }
            try { serialPort1.Write(box, 2, 1); }
            catch { }
            try { serialPort1.Write(box, 3, 1); }
            catch { }
            try { serialPort1.Write(box, 4, 1); }
            catch { }
        }

        private void Step2(object s, EventArgs e)
        {
            for (int i = 0; i < serialPort1.BytesToRead; i++)
            {
                if (adr == (memsize - 1)) //writing done
                {
                    maxtime = 0; buf[adr] = Convert.ToByte(serialPort1.ReadByte());
                    textBox9.Text = Convert.ToString(memsize) + " bytes";
                    progressBar1.Value = 100; textBox11.Text = "100%";
                    Open_file_button.Enabled = true;
                    Read_button.Enabled = true;
                    Save_file_button.Enabled = true;
                    Erase_button.Enabled = true;
                    Write_button.Enabled = true;
                    Verify_button.Enabled = true;
                }
                else
                {
                    buf[adr] = Convert.ToByte(serialPort1.ReadByte());
                    adr++;
                }
            }
        }

        private void Step3(object s, EventArgs e)
        {
            for (int i = 0; i < serialPort1.BytesToRead; i++)
            {
                try
                {
                    if (serialPort1.BytesToWrite == 0)
                    {
                        serialPort1.ReadByte();
                        adr++;
                        box[0] = buf[adr];
                        serialPort1.Write(box, 0, 1);
                    }
                }
                catch { }
                if (adr == buf.Length - 1)  //reading done
                {
                    maxtime = 0; textBox9.Text = Convert.ToString(buf.Length) + " bytes";
                    progressBar1.Value = 100; textBox11.Text = "100%";
                    Open_file_button.Enabled = true;
                    Read_button.Enabled = true;
                    Save_file_button.Enabled = true;
                    Erase_button.Enabled = true;
                    Write_button.Enabled = true;
                    Verify_button.Enabled = true;
                }
            }
        }

        private void Step4(object s, EventArgs e)
        {
            for (int i = 0; i < serialPort1.BytesToRead; i++)
            {
                if (adr == (memsize - 1))//verifying done
                {
                    Open_file_button.Enabled = true;
                    Read_button.Enabled = true;
                    Save_file_button.Enabled = true;
                    Erase_button.Enabled = true;
                    Write_button.Enabled = true;
                    Verify_button.Enabled = true;
                    maxtime = 0; verifybuf[adr] = Convert.ToByte(serialPort1.ReadByte());
                    textBox9.Text = Convert.ToString(memsize) + " bytes"; progressBar1.Value = 100;
                    textBox11.Text = "100%";
                    string message;
                    if (buf.Length == 0)
                    {
                        message = "Bad!" + "\n";
                        message += "Bytes in buffer: 0. Bytes read: " + Convert.ToString(verifybuf.Length) + ".";
                        MessageBox.Show(message, "Verification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        if (buf.SequenceEqual(verifybuf) == true) { message = "Identical!" + "\n"; } else { message = "Bad!" + "\n"; }
                        message += "Bytes in buffer: " + Convert.ToString(buf.Length) + ". Bytes read: " + Convert.ToString(verifybuf.Length) + ".";
                        MessageBox.Show(message, "Verification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    verifybuf[adr] = Convert.ToByte(serialPort1.ReadByte());
                    adr++;
                }
            }
        }

        private void Detectchip_button_Click(object sender, EventArgs e)
        {
            step = 1;
            box[0] = 0x00; box[1] = 0x00; box[2] = 0x00; box[3] = 0x00; box[4] = 0x02;
            try { serialPort1.Write(box, 0, 1); }
            catch { }
            try { serialPort1.Write(box, 1, 1); }
            catch { }
            try { serialPort1.Write(box, 2, 1); }
            catch { }
            try { serialPort1.Write(box, 3, 1); }
            catch { }
            try { serialPort1.Write(box, 4, 1); }
            catch { }
        }

        private void Readall(object sender, EventArgs e)
        {
            buf = new byte[memsize]; //create an array of memory size
            step = 2; //read mode
            adr = 0;
            int lastadr = memsize;
            box[1] = (byte)(lastadr >> 16);
            box[2] = (byte)(lastadr >> 8);
            box[3] = (byte)lastadr;
            box[0] = 0x00; box[4] = 0x07;
            try { serialPort1.Write(box, 0, 1); }
            catch { }
            try { serialPort1.Write(box, 1, 1); }
            catch { }
            try { serialPort1.Write(box, 2, 1); }
            catch { }
            try { serialPort1.Write(box, 3, 1); }
            catch { }
            try { serialPort1.Write(box, 4, 1); }
            catch { }
        }

        private void Verify(object sender, EventArgs e)
        {
            verifybuf = new byte[memsize]; //create an array of memory size
            step = 4; //data exchange in read mode
            adr = 0;
            int lastadr = memsize;
            box[1] = (byte)(lastadr >> 16);
            box[2] = (byte)(lastadr >> 8);
            box[3] = (byte)lastadr;
            box[0] = 0x00; box[4] = 0x07;
            try { serialPort1.Write(box, 0, 1); }
            catch { }
            try { serialPort1.Write(box, 1, 1); }
            catch { }
            try { serialPort1.Write(box, 2, 1); }
            catch { }
            try { serialPort1.Write(box, 3, 1); }
            catch { }
            try { serialPort1.Write(box, 4, 1); }
            catch { }
        }

        private void Writeall(object sender, EventArgs e)
        {
            if (buf.Length == 0) { MessageBox.Show("No data in buffer!"); }
            else
            {
                step = 3; //write mode
                adr = buf.Length;
                box[1] = (byte)(adr >> 16);
                box[2] = (byte)(adr >> 8);
                box[3] = (byte)adr;
                box[0] = 0x00; box[4] = 0x08;
                try { serialPort1.Write(box, 0, 1); }
                catch { }
                try { serialPort1.Write(box, 1, 1); }
                catch { }
                try { serialPort1.Write(box, 2, 1); }
                catch { }
                try { serialPort1.Write(box, 3, 1); }
                catch { }
                try { serialPort1.Write(box, 4, 1); }
                catch { }
                System.Threading.Thread.Sleep(5);
                adr = 0;
                box[0] = buf[adr];
                try { serialPort1.Write(box, 0, 1); }
                catch { }
                this.Invoke(new EventHandler(Step3));
            }
        }

        private void Read_button_Click(object sender, EventArgs e)
        {
            Open_file_button.Enabled = false;
            Read_button.Enabled = false;
            Save_file_button.Enabled = false;
            Erase_button.Enabled = false;
            Write_button.Enabled = false;
            Verify_button.Enabled = false;
            maxtime = 3000;
            time = 0;
            backgroundWorker1.RunWorkerAsync();
            this.Invoke(new EventHandler(Readall));
        }

        private void Save_file_button_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "bin files (*.bin)|*.bin|All files (*.*)|*.*";
            saveFileDialog1.Title = "Save buffer data to a file";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileStream fileStream = File.Open(saveFileDialog1.FileName, FileMode.Create);
                fileStream.SetLength(0);
                fileStream.Close();
                File.WriteAllBytes(saveFileDialog1.FileName, buf); // Requires System.IO
            }
        }

        private void Erase_button_Click(object sender, EventArgs e)
        {
            box[0] = 0x00; box[1] = 0x00; box[2] = 0x00; box[3] = 0x00; box[4] = 0x06;
            try { serialPort1.Write(box, 0, 1); }
            catch { }
            try { serialPort1.Write(box, 1, 1); }
            catch { }
            try { serialPort1.Write(box, 2, 1); }
            catch { }
            try { serialPort1.Write(box, 3, 1); }
            catch { }
            try { serialPort1.Write(box, 4, 1); }
            catch { }
            System.Threading.Thread.Sleep(500);
            box[0] = 0x00; box[1] = 0x00; box[2] = 0x00; box[3] = 0x00; box[4] = 0x09;
            try { serialPort1.Write(box, 0, 1); }
            catch { }
            try { serialPort1.Write(box, 1, 1); }
            catch { }
            try { serialPort1.Write(box, 2, 1); }
            catch { }
            try { serialPort1.Write(box, 3, 1); }
            catch { }
            try { serialPort1.Write(box, 4, 1); }
            catch { }
            MessageBox.Show("The chip is erased!");
        }

        private void Open_file_button_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog(); //open file dialog
            openFileDialog1.Filter = "(*.bin)|*.bin|All files (*.*)|*.*"; //filter for hex files
            openFileDialog1.Title = "Select a FLASH file"; //window's name
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK) //if hex file has been choosen
            {
                FileStream stream = File.OpenRead(openFileDialog1.FileName);
                buf = new byte[stream.Length];
                stream.Read(buf, 0, buf.Length);
                stream.Close();
                textBox9.Text = Convert.ToString(buf.Length) + " bytes";
            }
        }

        private void Write_button_Click(object sender, EventArgs e)
        {
            Open_file_button.Enabled = false;
            Read_button.Enabled = false;
            Save_file_button.Enabled = false;
            Erase_button.Enabled = false;
            Write_button.Enabled = false;
            Verify_button.Enabled = false;
            maxtime = 3000;
            time = 0;
            backgroundWorker1.RunWorkerAsync();
            this.Invoke(new EventHandler(Writeall));
        }

        private void Shaw_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            for (int i = 0; i < maxtime; i++)
            {
                Thread.Sleep(1000);
                time += 1;
                if (serialPort1.BytesToRead > 0)
                {
                    if (step == 2) { this.Invoke(new EventHandler(Step2)); }
                    else if (step == 3) { this.Invoke(new EventHandler(Step3)); }
                    else if (step == 4) { this.Invoke(new EventHandler(Step4)); }
                }
                //progress bar
                if (step == 2) { progressBar1.Invoke((MethodInvoker)delegate { progressBar1.Value = adr * 100 / (memsize - 1); }); }
                else if (step == 3) { progressBar1.Invoke((MethodInvoker)delegate { progressBar1.Value = adr * 100 / (buf.Length); }); }
                else if (step == 4) { progressBar1.Invoke((MethodInvoker)delegate { progressBar1.Value = adr * 100 / (memsize - 1); }); }
                //persentage
                if (step == 2) { textBox11.Invoke((MethodInvoker)delegate { textBox11.Text = Convert.ToString(adr * 100 / (memsize - 1)) + "%"; }); }
                else if (step == 3) { textBox11.Invoke((MethodInvoker)delegate { textBox11.Text = Convert.ToString(adr * 100 / (buf.Length - 1)) + "%"; }); }
                else if (step == 4) { textBox11.Invoke((MethodInvoker)delegate { textBox11.Text = Convert.ToString(adr * 100 / (memsize - 1)) + "%"; }); }
                //timer
                textBox13.Invoke((MethodInvoker)delegate { textBox13.Text = time.ToString("N0") + " sec"; });
                if (step == 3) { textBox9.Invoke((MethodInvoker)delegate { textBox9.Text = Convert.ToString(adr) + " bytes"; }); }
                else if (step == 2 || step == 4) { textBox9.Invoke((MethodInvoker)delegate { textBox9.Text = Convert.ToString(adr + 1) + " bytes"; }); }
            }
        }

        private void Verify_button_Click(object sender, EventArgs e)
        {
            Open_file_button.Enabled = false;
            Read_button.Enabled = false;
            Save_file_button.Enabled = false;
            Erase_button.Enabled = false;
            Write_button.Enabled = false;
            Verify_button.Enabled = false;
            maxtime = 3000;
            time = 0;
            backgroundWorker1.RunWorkerAsync();
            this.Invoke(new EventHandler(Verify));
        }
    }
}
