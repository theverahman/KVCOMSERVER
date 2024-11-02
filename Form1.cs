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
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

using ScottPlot;
using static OpenTK.Graphics.OpenGL.GL;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.Office.Interop;
using Excel = Microsoft.Office.Interop.Excel;

using LIBKVPROTOCOL;
using WORKFLOW;

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

            try
            {
                RealtimeUpdateList();
                MasteringUpdateList();
                if (_connStat != 1)
                {
                    _WorkflowHandler.SetConnection();
                    _connStat = _WorkflowHandler.GetConnState();
                }
            }
            catch
            {

            }

            _cts = new CancellationTokenSource();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Connect

            try
            {
                if (_connStat != 1)
                {
                    _WorkflowHandler.SetConnection();
                    _connStat = _WorkflowHandler.GetConnState();
                }
            }
            catch
            {

            }
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
            //msgToBeSent = richTextBox1.Text + "\r";
            //_WorkflowHandler.SendMessage(msgToBeSent);
            //Thread.Sleep(100);
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

        public void AllPlotReset()
        {
            formsPlot1.Reset();
            formsPlot2.Reset();
            formsPlot3.Reset();
            formsPlot4.Reset();

            formsPlot1.Refresh();
            formsPlot2.Refresh();
            formsPlot3.Refresh();
            formsPlot4.Refresh();
        }


        public void Plot1Update(double[] xd, double[] yd, System.Drawing.Color linecolor)
        {
            formsPlot1.Reset();
            //formsPlot1.BackColor = System.Drawing.Color.Black;
            var sp1 = formsPlot1.Plot.Add.SignalXY(xd, yd);
            sp1.Color = ScottPlot.Color.FromColor(linecolor);
            sp1.LineWidth = 3;
            formsPlot1.Plot.Axes.AntiAlias(true);
            formsPlot1.Refresh();
        }

        public void Plot1AddPlot(double[] xd, double[] yd, System.Drawing.Color linecolor)
        {
            var sp1 = formsPlot1.Plot.Add.SignalXY(xd, yd);
            sp1.Color = ScottPlot.Color.FromColor(linecolor);
            sp1.LineWidth = 3;
            formsPlot1.Plot.Axes.AntiAlias(true);
            formsPlot1.Refresh();
        }

        public void Plot2Update(double[] xd, double[] yd, System.Drawing.Color linecolor)
        {
            formsPlot2.Reset();
            //formsPlot2.BackColor = System.Drawing.Color.Black;
            var sp2 = formsPlot2.Plot.Add.SignalXY(xd, yd);
            sp2.Color = ScottPlot.Color.FromColor(linecolor);
            sp2.LineWidth = 3;
            formsPlot2.Plot.Axes.AntiAlias(true);
            formsPlot2.Refresh();
        }

        public void Plot2AddPlot(double[] xd, double[] yd, System.Drawing.Color linecolor)
        {
            var sp2 = formsPlot2.Plot.Add.SignalXY(xd, yd);
            sp2.Color = ScottPlot.Color.FromColor(linecolor);
            sp2.LineWidth = 3;
            formsPlot2.Plot.Axes.AntiAlias(true);
            formsPlot2.Refresh();
        }

        public void Plot3Update(double[] xd, double[] yd, System.Drawing.Color linecolor)
        {
            formsPlot3.Reset();
            //formsPlot3.BackColor = System.Drawing.Color.Black;
            var sp3 = formsPlot3.Plot.Add.SignalXY(xd, yd);
            sp3.Color = ScottPlot.Color.FromColor(linecolor);
            sp3.LineWidth = 3;
            formsPlot3.Plot.Axes.AntiAlias(true);
            formsPlot3.Refresh();
        }

        public void Plot3AddPlot(double[] xd, double[] yd, System.Drawing.Color linecolor)
        {
            var sp3 = formsPlot3.Plot.Add.SignalXY(xd, yd);
            sp3.Color = ScottPlot.Color.FromColor(linecolor);
            sp3.LineWidth = 3;
            formsPlot3.Plot.Axes.AntiAlias(true);
            formsPlot3.Refresh();
        }

        public void Plot4Update(double[] xd, double[] yd, System.Drawing.Color linecolor)
        {
            formsPlot4.Reset();
            //formsPlot4.BackColor = System.Drawing.Color.Black;
            var sp4 = formsPlot4.Plot.Add.SignalXY(xd, yd);
            sp4.Color = ScottPlot.Color.FromColor(linecolor);
            sp4.LineWidth = 3;
            formsPlot4.Plot.Axes.AntiAlias(true);
            formsPlot4.Refresh();
        }

        public void Plot4AddPlot(double[] xd, double[] yd, System.Drawing.Color linecolor)
        {
            var sp4 = formsPlot4.Plot.Add.SignalXY(xd, yd);
            sp4.Color = ScottPlot.Color.FromColor(linecolor);
            sp4.LineWidth = 3;
            formsPlot4.Plot.Axes.AntiAlias(true);
            formsPlot4.Refresh();
        }

        public void Plot5Update(double[] xd, double[] yd, System.Drawing.Color linecolor)
        {
            formsPlot5.Reset();
            var sp5 = formsPlot5.Plot.Add.SignalXY(xd, yd);
            sp5.Color = ScottPlot.Color.FromColor(linecolor);
            formsPlot5.Plot.Axes.AntiAlias(true);
            formsPlot5.Refresh();
        }
        public void Plot5UAddPlot(double[] xd, double[] yd, System.Drawing.Color linecolor)
        {
            var sp5 = formsPlot5.Plot.Add.SignalXY(xd, yd);
            sp5.Color = ScottPlot.Color.FromColor(linecolor);
            formsPlot5.Plot.Axes.AntiAlias(true);
            formsPlot5.Refresh();
        }

        public void Plot6Update(double[] xd, double[] yd, System.Drawing.Color linecolor)
        {
            formsPlot6.Reset();
            var sp6 = formsPlot6.Plot.Add.SignalXY(xd, yd);
            sp6.Color = ScottPlot.Color.FromColor(linecolor);
            formsPlot6.Plot.Axes.AntiAlias(true);
            formsPlot6.Refresh();
        }

        public void Plot6AddPlot(double[] xd, double[] yd, System.Drawing.Color linecolor)
        {
            var sp6 = formsPlot6.Plot.Add.SignalXY(xd, yd);
            sp6.Color = ScottPlot.Color.FromColor(linecolor);
            formsPlot6.Plot.Axes.AntiAlias(true);
            formsPlot6.Refresh();
        }

        public void Plot7Update(double[] xd, double[] yd, System.Drawing.Color linecolor)
        {
            formsPlot7.Reset();
            var sp7 = formsPlot7.Plot.Add.SignalXY(xd, yd);
            sp7.Color = ScottPlot.Color.FromColor(linecolor);
            formsPlot7.Plot.Axes.AntiAlias(true);
            formsPlot7.Refresh();
        }

        public void Plot7AddPlot(double[] xd, double[] yd, System.Drawing.Color linecolor)
        {
            var sp7 = formsPlot7.Plot.Add.SignalXY(xd, yd);
            sp7.Color = ScottPlot.Color.FromColor(linecolor);
            formsPlot7.Plot.Axes.AntiAlias(true);
            formsPlot7.Refresh();
        }

        public void Plot8Update(double[] xd, double[] yd, System.Drawing.Color linecolor)
        {
            formsPlot8.Reset();
            var sp8 = formsPlot8.Plot.Add.SignalXY(xd, yd);
            sp8.Color = ScottPlot.Color.FromColor(linecolor);
            formsPlot8.Plot.Axes.AntiAlias(true);
            formsPlot8.Refresh();
        }

        public void Plot8AddPlot(double[] xd, double[] yd, System.Drawing.Color linecolor)
        {
            var sp8 = formsPlot8.Plot.Add.SignalXY(xd, yd);
            sp8.Color = ScottPlot.Color.FromColor(linecolor);
            formsPlot8.Plot.Axes.AntiAlias(true);
            formsPlot8.Refresh();
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

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button12_Click(object sender, EventArgs e)
        {

        }

        private void button15_Click(object sender, EventArgs e)
        {

        }

        void CheckFolderPath(string pathblazer) { if (!Directory.Exists(pathblazer)) { Directory.CreateDirectory(pathblazer); } }

        public void RealtimeUpdateList()
        {
            string DirRealtime = $"C:\\FTP_DB_FUNCTION_TESTER\\LOG_REALTIME\\YEAR_{dateTimePicker1.Value.Year}\\MONTH_{dateTimePicker1.Value.Month}\\DAY_{dateTimePicker1.Value.Day}";
            CheckFolderPath(DirRealtime);

            string[] listfiles = new string[] { };
            string[] listfiles_date = new string[] { };
            int idx1 = new int();

            foreach (string files in Directory.GetFiles(DirRealtime))
            {
                idx1 += 1;
                Array.Resize(ref listfiles, idx1);
                Array.Resize(ref listfiles_date, idx1);
                listfiles[idx1 - 1] = System.IO.Path.GetFileName(files);
                listfiles_date[idx1 - 1] = File.GetCreationTime(files).ToLongTimeString();
            }
            DataTable listtablefile = new DataTable();
            listtablefile.Columns.Add("Time");
            listtablefile.Columns.Add("File Name");

            for (int i = 0; i < idx1; i++)
            {
                DataRow newRow = listtablefile.NewRow();
                newRow["Time"] = listfiles_date[i];
                newRow["File Name"] = listfiles[i];
                listtablefile.Rows.Add(newRow);
            }

            dataGridView1.DataSource = listtablefile;
            dataGridView1.Update();
        }

        public void MasteringUpdateList()
        {
            string DirMaster = $"C:\\FTP_DB_FUNCTION_TESTER\\MASTERING";
            CheckFolderPath(DirMaster);

            string[] listfiles = new string[] { };
            string[] listfiles_date = new string[] { };
            int idx2 = new int();

            foreach (string files in Directory.GetFiles(DirMaster))
            {
                idx2 += 1;
                Array.Resize(ref listfiles, idx2);
                Array.Resize(ref listfiles_date, idx2);
                listfiles[idx2 - 1] = System.IO.Path.GetFileName(files);
                listfiles_date[idx2 - 1] = File.GetCreationTime(files).ToLongTimeString();
            }

            DataTable listtablefile = new DataTable();
            listtablefile.Columns.Add("Time");
            listtablefile.Columns.Add("File Name");

            for (int i = 0; i < idx2; i++)
            {
                DataRow newRow = listtablefile.NewRow();
                newRow["Time"] = listfiles_date[i];
                newRow["File Name"] = listfiles[i];
                listtablefile.Rows.Add(newRow);
            }
            dataGridView2.DataSource = listtablefile;
            dataGridView2.Update();
        }

        public void RealtimeList_SetDate(DateTime daten)
        {
            dateTimePicker1.Value = daten;
        }

        public DateTime RealtimeList_GetDate()
        {
            return dateTimePicker1.Value;
        }

        private void dateTimePicker1_ValueChanged_1(object sender, EventArgs e)
        {
            RealtimeUpdateList();
        }

        private void button19_Click(object sender, EventArgs e)
        {
            string DirRealtime = $"C:\\FTP_DB_FUNCTION_TESTER\\LOG_REALTIME\\YEAR_{dateTimePicker1.Value.Year}\\MONTH_{dateTimePicker1.Value.Month}\\DAY_{dateTimePicker1.Value.Day}";
            CheckFolderPath(DirRealtime);
            DataGridViewRow test1 = new DataGridViewRow();
            test1 = dataGridView1.CurrentRow;
            string selectedfile = new string(test1.Cells[1].FormattedValue.ToString());

            Excel.Application objExcel = new Excel.Application();
            Excel.Workbook excelWorkbook = objExcel.Workbooks.Open($"{DirRealtime}\\{selectedfile}");
            objExcel.Visible = true;
        }

        private void button20_Click(object sender, EventArgs e)
        {
            string DirMaster = $"C:\\FTP_DB_FUNCTION_TESTER\\MASTERING";
            CheckFolderPath(DirMaster);
            DataGridViewRow test2 = new DataGridViewRow();
            test2 = dataGridView2.CurrentRow;
            string selectedfile = new string(test2.Cells[1].FormattedValue.ToString());

            Excel.Application objExcel = new Excel.Application();
            Excel.Workbook excelWorkbook = objExcel.Workbooks.Open($"{DirMaster}\\{selectedfile}");
            objExcel.Visible = true;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string DirRealtime = $"C:\\FTP_DB_FUNCTION_TESTER\\LOG_REALTIME\\YEAR_{dateTimePicker1.Value.Year}\\MONTH_{dateTimePicker1.Value.Month}\\DAY_{dateTimePicker1.Value.Day}";
            CheckFolderPath(DirRealtime);
            var psi = new ProcessStartInfo();
            psi.FileName = @"c:\windows\explorer.exe";
            psi.Arguments = DirRealtime;
            Process.Start(psi);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            string DirMaster = $"C:\\FTP_DB_FUNCTION_TESTER\\MASTERING";
            CheckFolderPath(DirMaster);
            var psi = new ProcessStartInfo();
            psi.FileName = @"c:\windows\explorer.exe";
            psi.Arguments = DirMaster;
            Process.Start(psi);
        }
    }

}
