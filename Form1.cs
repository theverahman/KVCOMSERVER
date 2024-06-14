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

namespace WindowsFormsApp1
{
    
    public partial class Form1 : Form 
    { 
        public ConnectPLC connPLC;

        public string settingIpv4;
        public int settingPortIp;
        public string msgToBeSent;
        
        public Form1(ConnectPLC connEst)
        {
            this.connPLC = connEst;
            InitializeComponent();
            Thread backgroundThread = new Thread(BackgroundWork);
            backgroundThread.Start();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Connect
            connPLC.SetConnection(settingIpv4, settingPortIp);
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
            connPLC.CloseConnection();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            msgToBeSent = richTextBox1.Text + "\r";
            //Execute
            connPLC.connSend
                (
                    Encoding.ASCII.GetBytes(msgToBeSent)
                );

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

        private void BackgroundWork()
        {
            int counter = 0;
            while (counter < 5)
            {
                counter++;
                Thread.Sleep(50);
            }
            DoWorkOnUI();
        }

        private void DoWorkOnUI()
        {
            while (true)
            {
                MethodInvoker methodInvokerDelegate = delegate ()
                {
                    if (connPLC.getState())
                    {
                        if (connPLC.getAvail() > 0)
                        {
                            connPLC.connRecv();
                            richTextBox2.Text = Encoding.ASCII.GetString
                                (
                                    connPLC.getMsgRecv(), 0, connPLC.getByteRecv()
                                );
                        }
                    }
                };
                //This will be true if Current thread is not UI thread.
                this.Invoke(methodInvokerDelegate);
            }
            
        }


        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
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
