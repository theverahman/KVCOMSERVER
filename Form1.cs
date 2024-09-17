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


namespace KVCOMSERVER
{
    
    public partial class Form1 : Form 
    {
        private WORKFLOWHANDLER _WorkflowHandler;
        public KVPROTOCOL connPLC;
        public string settingIpv4;
        public int settingPortIp;
        public string msgToBeSent;

        //public Form1(KVPROTOCOL connEst)
        public Form1()
        {
            InitializeComponent();
            _WorkflowHandler = new WORKFLOWHANDLER(this);
            textBox1.Text = settingIpv4;
            textBox2.Text = settingPortIp.ToString();

            Thread backgroundThread_1 = new Thread(_WorkflowHandler.BackgroundWork_1);
            backgroundThread_1.Start();

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
            _WorkflowHandler.SetConnection();
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
