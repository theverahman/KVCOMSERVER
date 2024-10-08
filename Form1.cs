﻿using System;
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
using System.Threading.Tasks;
using System.Diagnostics;
using ScottPlot;
using static OpenTK.Graphics.OpenGL.GL;

namespace KVCOMSERVER
{

    public partial class Form1 : Form
    {
        private WORKFLOWHANDLER _WorkflowHandler;
        private CancellationTokenSource _cts;

        public string settingIpv4;
        public int settingPortIp;
        public string msgToBeSent;
        public int _connStat;
        public int _beaconn;

        Thread backgroundThread_1;
        Thread backgroundThread_2;
        Thread backgroundThread_3;

        Thread uibackgroundThread_0;

        //public Form1(KVPROTOCOL connEst)



        public Form1()
        {
            InitializeComponent();
            _WorkflowHandler = new WORKFLOWHANDLER(this);
            textBox1.Text = settingIpv4;
            textBox2.Text = settingPortIp.ToString();


            //backgroundThread_1 = new Thread(_WorkflowHandler.BackgroundWork_1);
            //backgroundThread_2 = new Thread(_WorkflowHandler.BackgroundWork_2);
            //backgroundThread_3 = new Thread(_WorkflowHandler.BackgroundWork_3);

            //uibackgroundThread_0 = new Thread(this._uiBackgroundWork_0);
            //uibackgroundThread_0.Start();
            //Debug.Write(uibackgroundThread_0.ManagedThreadId);
            //Debug.Write((char)'\n');

            _cts = new CancellationTokenSource();

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
            //start_background_task();
            //start_async_background_task();
            _WorkflowHandler.SetConnection();
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
            _WorkflowHandler.CloseConnection();
            _connStat = _WorkflowHandler.GetConnState();
            Thread.Sleep(100);
            if (_connStat == 0)
            {
                _WorkflowHandler.abortTasks();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            msgToBeSent = richTextBox1.Text + "\r";
            _WorkflowHandler.SendMessage(msgToBeSent);
            Thread.Sleep(100);
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
            button5.ForeColor = System.Drawing.Color.Black;
            button5.BackColor = System.Drawing.Color.LimeGreen;
        }

        public void connStatLampOff()
        {
            button5.Text = "Disconnected";
            button5.ForeColor = System.Drawing.Color.Black;
            button5.BackColor = System.Drawing.Color.Red;
        }

        public void beaconnStatLampOn()
        {
            button6.Text = "ON";
            button6.ForeColor = System.Drawing.Color.Black;
            button6.BackColor = System.Drawing.Color.LimeGreen;
        }

        public void beaconnStatLampOff()
        {
            button6.Text = "OFF";
            button6.ForeColor = System.Drawing.Color.Black;
            button6.BackColor = System.Drawing.Color.BlueViolet;
        }

        public void Plot1Update(double[] xd, double[] yd)
        {
            formsPlot1.Reset();
            var sp1 = formsPlot1.Plot.Add.Scatter(xd, yd);
            formsPlot1.Plot.Axes.AntiAlias(true);
            formsPlot1.Refresh();
        }

        public void Plot2Update(double[] xd, double[] yd)
        {
            formsPlot2.Reset();
            var sp2 = formsPlot2.Plot.Add.Scatter(xd, yd);
            formsPlot2.Plot.Axes.AntiAlias(true);
            formsPlot2.Refresh();
        }

        public void Plot3Update(double[] xd, double[] yd)
        {
            formsPlot3.Reset();
            var sp3 = formsPlot3.Plot.Add.Scatter(xd, yd);
            formsPlot3.Plot.Axes.AntiAlias(true);
            formsPlot3.Refresh();
        }

        public void Plot4Update(double[] xd, double[] yd)
        {
            formsPlot4.Reset();
            var sp4 = formsPlot4.Plot.Add.Scatter(xd, yd);
            formsPlot4.Plot.Axes.AntiAlias(true);
            formsPlot4.Refresh();
        }

        private void tabPage4_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {

        }

        private void formsPlot1_Load(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }
    }

}
