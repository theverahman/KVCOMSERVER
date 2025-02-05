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

using Color = System.Drawing.Color;
using SColor = ScottPlot.Color;

using FontStyle = System.Drawing.FontStyle;
using SFontStyle = ScottPlot.FontStyle;

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

        public Form1()
        {
            InitializeComponent();
            _WorkflowHandler = new WORKFLOWHANDLER(this);
            textBox1.Text = settingIpv4;
            textBox2.Text = settingPortIp.ToString();
            _cts = new CancellationTokenSource();
            try
            {
                if (_connStat != 1)
                {
                    //_WorkflowHandler.SetConnection();
                    _connStat = _WorkflowHandler.GetConnState();
                }
                //RealtimeUpdateList();
                //MasteringUpdateList();   
            }
            catch
            { }
            InitializeUI();
        }

        private void InitializeUI()
        {
            // Set form properties
            this.Text = "Damping Force Function Tester";
            this.BackColor = Color.White;
            this.Font = new Font("Arial", 10);

            // Initialize buttons
            InitializeModernButton(button1, "Connect");
            InitializeModernButton(button2, "Close");
            InitializeModernButton(button4, "Save Setting");
            InitializeModernButton(button3, "Display to Graph");
            InitializeModernButton(button19, "Open Selected");
            InitializeModernButton(button8, "Open Folder");

            // Initialize TextBoxes
            textBox1.Font = new Font("Arial", 12);
            textBox2.Font = new Font("Arial", 12);
            textBox1.TextChanged += textBox1_TextChanged;
            textBox2.TextChanged += textBox2_TextChanged;

            // Initialize DataGridViews
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Set up DateTimePicker
            dateTimePicker1.Font = new Font("Arial", 12);
            dateTimePicker1.ValueChanged += dateTimePicker1_ValueChanged_1;
        }

        private void InitializeModernButton(Button button, string text)
        {
            button.Text = text;
            button.BackColor = Color.LimeGreen;
            button.FlatStyle = FlatStyle.Flat;
            button.ForeColor = Color.White;
            button.Font = new Font("Arial", 12, FontStyle.Bold);
            button.Size = new Size(150, 50);
            button.MouseEnter += (s, e) => button.BackColor = Color.DarkGreen;
            button.MouseLeave += (s, e) => button.BackColor = Color.LimeGreen;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
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
                MessageBox.Show("Connection failed. Please check your settings.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            settingIpv4 = textBox1.Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(textBox2.Text, out int port))
            {
                settingPortIp = port;
            }
            else
            {
                MessageBox.Show("Please enter a valid port number.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string DirRealtime = _WorkflowHandler.RealLogDir + $"YEAR_{dateTimePicker1.Value.Year}\\MONTH_{dateTimePicker1.Value.Month}\\DAY_{dateTimePicker1.Value.Day}";
            CheckFolderPath(DirRealtime);
            var psi = new ProcessStartInfo();
            psi.FileName = @"c:\windows\explorer.exe";
            psi.Arguments = DirRealtime;
            Process.Start(psi);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            string DirMaster = _WorkflowHandler.MasterDir;
            CheckFolderPath(DirMaster);
            var psi = new ProcessStartInfo();
            psi.FileName = @"c:\windows\explorer.exe";
            psi.Arguments = DirMaster;
            Process.Start(psi);
        }

        private void button19_Click(object sender, EventArgs e)
        {
            string DirRealtime = _WorkflowHandler.RealLogDir + $"YEAR_{dateTimePicker1.Value.Year}\\MONTH_{dateTimePicker1.Value.Month}\\DAY_{dateTimePicker1.Value.Day}";
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
            string DirMaster = _WorkflowHandler.MasterDir;
            CheckFolderPath(DirMaster);
            DataGridViewRow test2 = new DataGridViewRow();
            test2 = dataGridView2.CurrentRow;
            string selectedfile = new string(test2.Cells[1].FormattedValue.ToString());

            Excel.Application objExcel = new Excel.Application();
            Excel.Workbook excelWorkbook = objExcel.Workbooks.Open($"{DirMaster}\\{selectedfile}");
            objExcel.Visible = true;
        }

        private void dateTimePicker1_ValueChanged_1(object sender, EventArgs e)
        {
            RealtimeUpdateList();
        }

        public void RealtimeUpdateList()
        {
            string DirRealtime = _WorkflowHandler.RealLogDir + $"YEAR_{dateTimePicker1.Value.Year}\\MONTH_{dateTimePicker1.Value.Month}\\DAY_{dateTimePicker1.Value.Day}";
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

            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            dataGridView1.DataSource = listtablefile;
            dataGridView1.Update();
        }

        public void MasteringUpdateList()
        {
            string DirMaster = _WorkflowHandler.MasterDir;
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

        public void RealtimeList_SetDate(DateTime datentime)
        {
            dateTimePicker1.Value = datentime;
        }

        public DateTime RealtimeList_GetDate()
        {
            return dateTimePicker1.Value;
        }

        void CheckFolderPath(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
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

        public void Plot5AddPlot(double[] xd, double[] yd, System.Drawing.Color linecolor)
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

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void tabPage4_Click(object sender, EventArgs e)
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

        private void button28_Click(object sender, EventArgs e)
        {

        }
    }
}
