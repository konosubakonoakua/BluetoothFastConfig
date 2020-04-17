using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace BluetoothFastConfig
{
    public partial class Form1 : Form
    {
        //int TrueBAUD = 0;
        public Form1()
        {
            InitializeComponent();
            comboBox1.Items.Add("1200");
            comboBox1.Items.Add("2400");
            comboBox1.Items.Add("4800");
            comboBox1.Items.Add("9600");
            comboBox1.Items.Add("19200");
            comboBox1.Items.Add("38400");
            comboBox1.Items.Add("57600");
            comboBox1.Items.Add("115200");
            comboBox1.Items.Add("230400");
            comboBox1.Items.Add("460800");
            comboBox1.Items.Add("921600");
            comboBox1.Items.Add("1382400");
            comboBox1.SelectedIndex = 3;

            comboBox2.Items.Add("请选择");
            comboBox2.SelectedIndex = 0;

            comboBox3.Items.Add("0");
            comboBox3.Items.Add("1");
            comboBox3.SelectedIndex = 0;
            IsPortOpen();
        }

        private void L(String s,int color)
        {
            if (color == 0) richTextBox1.SelectionColor = Color.Black;
            if (color == 1) richTextBox1.SelectionColor = Color.Blue;
            if (color == 2) richTextBox1.SelectionColor = Color.Green;
            if (color == 3) richTextBox1.SelectionColor = Color.Red;
            richTextBox1.AppendText("[" + DateTime.Now.ToString("hh:mm:ss") + "] " + s + "\r\n");
            this.richTextBox1.SelectionStart = this.richTextBox1.TextLength;
            this.richTextBox1.ScrollToCaret(); 
        }

        public bool IsPortOpen()
        {
            bool _available = false;
            SerialPort _tempPort;
            String[] Portname = SerialPort.GetPortNames();
            comboBox2.Items.Clear();

            foreach (string str in Portname)
            {
                try
                {
                    _tempPort = new SerialPort(str);
                    _tempPort.Open();

                    if (_tempPort.IsOpen)
                    {
                        comboBox2.Items.Add(str);
                        _tempPort.Close();
                        _available = true;
                    }
                }
                catch (Exception ex)
                {
                    L("串口枚举失败，因为"+ex.Message,3);
                    _available = false;
                }
            }
            comboBox2.SelectedItem = 1;
            if (_available==false) L("没有找到串口设备或其他程序正在使用串口",3);
            L("串口枚举完成", 1);
            return _available;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            IsPortOpen();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            //int c = '\0';
            string input = "";
            if (comboBox2.SelectedItem.ToString() == "请选择")
            {
                L("你还没有选择串口",3);
                return;
            }
            if (serialPort1.IsOpen) 
                serialPort1.Close();
            try
            {
                serialPort1.PortName = comboBox2.SelectedItem.ToString();
                serialPort1.BaudRate = int.Parse(textBox1.Text);

                serialPort1.DataBits = 8;
                serialPort1.Parity = System.IO.Ports.Parity.None;
                serialPort1.StopBits = System.IO.Ports.StopBits.One;
                serialPort1.ReadTimeout = 1000;
                serialPort1.NewLine = "\r\n";
                
                serialPort1.Open();
            }
            catch (Exception ex) { L(ex.Message,3); return; }
            L("串口打开成功",0);
            button6.BackColor = Color.RoyalBlue;

            L("发送:AT",1);
            try{serialPort1.WriteLine("AT");}
            catch (Exception ex){L(ex.Message,3);return;}
            
            try
            {
                input = "";
                input = serialPort1.ReadLine();
                L("收到:" + input, 2);
                if (input.StartsWith("OK"))
                {
                    L("连接正常", 0);
                }
                else
                {
                    L("连接失败，请检查波特率", 3);
                    return;
                }
            }
            catch (Exception ex)
            {
                L(ex.Message, 3); return;
            }


            L("发送:AT+NAME="+textBox3.Text,1);
            try { serialPort1.WriteLine("AT+NAME=" + textBox3.Text); }
            catch (Exception ex) { L(ex.Message,3); return; }
            try
            {
                input = "";
                input = serialPort1.ReadLine();
                L("收到:" + input, 2);
                if (input.StartsWith("OK"))
                {
                    L("蓝牙名字设置完成", 0);
                }
                else
                {
                    L("蓝牙名字设置失败", 3);
                    return;
                }
            }
            catch (Exception ex)
            {
                L(ex.Message, 3); return;
            }



            L("发送:AT+PSWD="+textBox4.Text,1);
            try { serialPort1.WriteLine("AT+PSWD=" + textBox4.Text); }
            catch (Exception ex) { L(ex.Message,3); return; }
            try
            {
                input = "";
                input = serialPort1.ReadLine();
                L("收到:" + input, 2);
                if (input.StartsWith("OK"))
                {
                    L("配对密码设置完成", 0);
                }
                else
                {
                    L("配对密码设置失败", 3);
                    return;
                }
            }
            catch (Exception ex)
            {
                L(ex.Message, 3); return;
            }


            try 
            {
                string s = "AT+UART=" + comboBox1.SelectedItem.ToString() + ",0,0";
                serialPort1.WriteLine(s);
                L("发送:" + s, 1);
            }
            catch (Exception ex) { L(ex.Message, 3); return; }

            try
            {
                input = "";
                input = serialPort1.ReadLine();
                L("收到:" + input, 2);
                if (input.StartsWith("OK"))
                {
                    L("波特率设置完成", 0);
                }
                else
                {
                    L("波特率设置失败", 3);
                    return;
                }
            }
            catch (Exception ex)
            {
                L(ex.Message, 3); return;
            }



            try
            {
                string s = "AT+ROLE=" + comboBox3.SelectedItem.ToString();
                serialPort1.WriteLine(s);
                L("发送:" + s, 1);
            }
            catch (Exception ex) { L(ex.Message, 3); return; }

            try
            {
                input = "";
                input = serialPort1.ReadLine();
                L("收到:" + input, 2);
                if (input.StartsWith("OK"))
                {
                    L("主从机设置完成", 0);
                }
                else
                {
                    L("主从机设置失败", 3);
                    return;
                }
            }
            catch (Exception ex)
            {
                L(ex.Message, 3); return;
            }


            L("全部操作完成",0);
            L("蓝牙重启", 0);

            try
            {
                serialPort1.WriteLine("AT+RESET");
            }
            catch (Exception ex)
            {
            
                L(ex.Message, 3); return;
            }

            serialPort1.Close();
            button6.BackColor = Color.Coral;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
            L("HC05蓝牙快速配置助手",1);
            L("", 0);
            L("帮助：", 1);
            L("上电之前长按按钮，等到指示灯慢闪时松开按钮",3);
            L("只需要将主从模块的密码和波特率配置相同，再次上电即可自动配对。", 0);
            L("主模块不能配置名字（配置主模块名字会失败）但不影响模块正常使用。",0);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
            //L("源代码:https://github.com/konosubakonoakua/BluetoothFastConfig", 0);
            L("QQ:1356781673", 0);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            string input = "";
            if (comboBox2.SelectedItem.ToString() == "请选择")
            {
                L("你还没有选择串口", 3);
                return;
            }
            if (serialPort1.IsOpen)
                serialPort1.Close();
            try
            {
                serialPort1.PortName = comboBox2.SelectedItem.ToString();
                serialPort1.BaudRate = int.Parse(textBox1.Text);

                serialPort1.DataBits = 8;
                serialPort1.Parity = System.IO.Ports.Parity.None;
                serialPort1.StopBits = System.IO.Ports.StopBits.One;
                serialPort1.ReadTimeout = 1000;
                serialPort1.NewLine = "\r\n";
                serialPort1.Open();
            }
            catch (Exception ex) { L(ex.Message, 3); return; }
            L("串口打开成功", 0);
            button6.BackColor = Color.RoyalBlue;
            L("发送:AT", 1);
            try { serialPort1.Write("AT\r\n"); }
            catch (Exception ex) { L(ex.Message, 3); return; }

            try
            {
                input = "";
                input = serialPort1.ReadLine();
                L("收到:" + input, 2);
                if (input.StartsWith("OK"))
                {
                    L("连接正常", 0);
                }
                else
                {
                    L("连接失败，请检查波特率", 3);
                    return;
                }
            }
            catch (Exception ex)
            {
                L(ex.Message, 3); return;
            }
            L("蓝牙恢复出厂设置", 0);
            try
            {
                string s = "AT+ORGL";
                serialPort1.WriteLine(s);
                L("发送:" + s, 1);
            }
            catch (Exception ex) { L(ex.Message, 3); return; }

            try
            {
                input = "";
                input = serialPort1.ReadLine();
                L("收到:" + input, 2);
                if (input.StartsWith("OK"))
                {
                    L("蓝牙恢复出厂设置完成", 0);
                }
                else
                {
                    L("蓝牙恢复出厂设置失败", 3);
                    return;
                }
            }
            catch (Exception ex)
            {
                L(ex.Message, 3); return;
            }
            serialPort1.Close();
            button6.BackColor = Color.Coral;
        }
    }

}
