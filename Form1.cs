using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using LIBKVPROTOCOL;
using WORKFLOW;
using DocumentFormat.OpenXml.Vml.Spreadsheet;


namespace KVCOMSERVER
{

    public partial class Form1 : Form
    {
        private WORKFLOWHANDLER _WorkflowHandler;
        public string settingIpv4;
        public int settingPortIp;
        public string msgToBeSent;
        public int _connStat;
        public int _beaconn;

        Thread backgroundThread_1;
        Thread backgroundThread_2;
        Thread backgroundThread_3;
        Thread backgroundThread1_0;

        Thread uibackgroundThread_0;

        //public Form1(KVPROTOCOL connEst)
        public Form1()
        {
            InitializeComponent();
            _WorkflowHandler = new WORKFLOWHANDLER(this);
            textBox1.Text = settingIpv4;
            textBox2.Text = settingPortIp.ToString();

            backgroundThread_1 = new Thread(_WorkflowHandler.BackgroundWork_1);
            backgroundThread_2 = new Thread(_WorkflowHandler.BackgroundWork_2);
            backgroundThread_3 = new Thread(_WorkflowHandler.BackgroundWork_3);
            backgroundThread1_0 = new Thread(_WorkflowHandler.BackgroundWork1_0);

            uibackgroundThread_0 = new Thread(this._uiBackgroundWork_0);
            uibackgroundThread_0.Start();
        }

        public void setTextBox2(string text)
        {
            richTextBox2.Text = text;
        }

        public string getTextBox2()
        {
            return richTextBox2.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Connect
            start_background_task();
            _connStat = _WorkflowHandler.GetConnState();
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        //IPV4Address Input
        {
            settingIpv4 = textBox1.Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            settingPortIp = int.Parse(textBox2.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Close
            _WorkflowHandler.CloseConnection();
            backgroundThread_1.Abort();
            //backgroundThread_2.Abort();
            //backgroundThread_3.Abort();
            //backgroundThread1_0.Abort();
            _connStat = _WorkflowHandler.GetConnState();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            msgToBeSent = richTextBox1.Text + "\r";
            //Execute
            _WorkflowHandler.SendMessage(msgToBeSent);

            Thread.Sleep(1);


            //Read
            //    if (connPLC.getState())
            //    {
            //        connPLC.connRecv();
            //        richTextBox2.Text = Encoding.ASCII.GetString
            //            (
            //                connPLC.getMsgRecv(), 0, connPLC.getByteRecv()
            //            );
            //    }

        }


        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        public void connStatLampOn()
        {
            button5.Text = "Connected";
            button5.ForeColor = Color.Black;
            button5.BackColor = Color.LimeGreen;
        }

        public void connStatLampOff()
        {
            button5.Text = "Disconnected";
            button5.ForeColor = Color.Black;
            button5.BackColor = Color.Red;
        }

        public void beaconnStatLampOn()
        {
            button6.Text = "ON";
            button6.ForeColor = Color.Black;
            button6.BackColor = Color.LimeGreen;
        }

        public void beaconnStatLampOff()
        {
            button6.Text = "OFF";
            button6.ForeColor = Color.Black;
            button6.BackColor = Color.BlueViolet;
        }

        public void _uiBackgroundWork_0()
        {
            int counter = 0;
            while (counter < 5)
            {
                counter++;
                Thread.Sleep(5);
            }
            DoWork_0();
        }

        private void DoWork_0()
        {
            while (true)
            {
                MethodInvoker methodInvokerDelegate = delegate ()
                {
                    if (this._connStat == 1)
                    {
                        this.connStatLampOn();
                    }
                    else
                    {
                        this.connStatLampOff();
                    }

                    if (this._beaconn == 1)
                    {
                        this.beaconnStatLampOn();
                    }
                    else
                    {
                        this.beaconnStatLampOff();
                    }
                };
                //This will be true if Current thread is not UI thread.
                this.Invoke(methodInvokerDelegate);
            }
        }

        public void start_background_task()
        {
            _WorkflowHandler.SetConnection();
            backgroundThread_1.Start();
            //backgroundThread_2.Start();
            //backgroundThread_3.Start();
            //backgroundThread1_0.Start();
        }

        private void tabPage4_Click(object sender, EventArgs e)
        {

        }




        /*
        // Creation of message that
        // we will send to Server
        byte[] messageSent = Encoding.ASCII.GetBytes("Test Client<EOF>");
        //int byteSent = connPLC.ConnObj().Send(messageSent);
        */
        /*
        // Data buffer
        byte[] messageReceived = new byte[1024];
        */
        /*
        // We receive the message using
        // the method Receive(). This
        // method returns number of bytes
        // received, that we'll use to
        // convert them to string
        //int byteRecv = connPLC.ConnObj().Receive(messageReceived);
        Console.WriteLine("Message from Server -> {0}",
                  Encoding.ASCII.GetString(messageReceived,
                                             0, byteRecv));
        */
    }



}
