using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Drawing;

using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.IO;

using Tasks = System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

using DataTable = System.Data.DataTable;
using Label = System.Windows.Forms.Label;
using Control = System.Windows.Forms.Control;
using TextRenderer = System.Windows.Forms.TextRenderer;

using DRW = System.Drawing;
using Font = System.Drawing.Font;
using Size = System.Drawing.Size;
using Point = System.Drawing.Point;
using Color = System.Drawing.Color;
using FontStyle = System.Drawing.FontStyle;

using ScottPlot;
using Excel = Microsoft.Office.Interop.Excel;

using LIBKVPROTOCOL;
using WORKFLOW;
using ScottPlot.DataSources;
using ScottPlot.WinForms;
using System.Reflection.Emit;
using ClosedXML.Report.Utils;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Drawing.Text;
using ScottPlot.Interactivity;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using ScottPlot.Plottables;
using SixLabors.Fonts;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using ScottPlot.Interactivity;
using ScottPlot.Interactivity.UserActions;
using ScottPlot.Interactivity.UserActionResponses;


namespace KVCOMSERVER
{
    public partial class Form1 : Form
    {
        private WORKFLOWHANDLER _WorkflowHandler;
        private Panel drawingPanel;
        private Panel drawingBorderLeft;
        private Panel drawingBorderRight;
        private Panel drawingBorderUpper;
        private Panel drawingBorderLower;

        private SaveFileDialog saveFileDialog;
        private OpenFileDialog openFileDialog;
        private FolderBrowserDialog folderBrowserDialog;
        private PrivateFontCollection privateFontCollection;

        private CancellationTokenSource _cts;

        Text JudgePlot1;
        Text JudgePlot2;
        Text JudgePlot3;
        Text JudgePlot4;
        Text JudgePlot9;
        Text JudgePlot10;

        bool masterConfirmEdit;

        public string settingIpv4;
        public int settingPortIp;
        public string msgToBeSent;
        public int _connStat;
        public int _beaconn;

        public bool updateMasterProcess;
        public bool validationMasterProcess;

        int tabIdxRealSideL = 0;
        int tabIdxRealSideR = 0;

        int tabIdxMasterSideL = 0;
        int tabIdxMasterSideR = 0;

        int ActiveRealTableLeftData = 0;
        int ActiveRealTableRightData = 0;

        int ActiveMasterTableLeftData = 0;
        int ActiveMasterTableRightData = 0;

        float MasterTeachOffsetBatchL = 0;
        float MasterTeachOffsetBatchR = 0;

        float MasterTeachDiffOffsetBatchL = 0;
        float MasterTeachDiffOffsetBatchR = 0;

        bool MasterTeachSetConfirm = false;
        public void MasterTeachSetConfirmSet()
        {
            MasterTeachSetConfirm = true;
        }
        public void MasterTeachSetConfirmReset()
        {
            MasterTeachSetConfirm = false;
        }

        public void SetModelName(string mod)
        {
            label8.Text = ($"SELECTED MODEL: {mod}");
        }
        public void UpdateTime()
        {
            timelabel0.Text = DateTime.Now.ToString("HH:mm:ss");
        }
        public void UpdateDate()
        {
            string day = DateTime.Now.ToString("ddd");
            string date = DateTime.Now.ToString("dd");
            string month = DateTime.Now.ToString("MMM");
            string year = DateTime.Now.ToString("yyyy");
            datelabel0.Text = ($"{day}, {date} {month} {year} ");
        }

        #region Table Data Def 

        #region Real Data
        CustomTableLayoutPanel tabRealSideL;
        List<TextBox> tabRealSideLStroke = new List<TextBox>();
        List<TextBox> tabRealSideLMaster = new List<TextBox>();
        List<TextBox> tabRealSideLLower = new List<TextBox>();
        List<TextBox> tabRealSideLReal = new List<TextBox>();
        List<TextBox> tabRealSideLUpper = new List<TextBox>();

        CustomTableLayoutPanel tabRealSideR;
        List<TextBox> tabRealSideRStroke = new List<TextBox>();
        List<TextBox> tabRealSideRMaster = new List<TextBox>();
        List<TextBox> tabRealSideRLower = new List<TextBox>();
        List<TextBox> tabRealSideRReal = new List<TextBox>();
        List<TextBox> tabRealSideRUpper = new List<TextBox>();

        bool[] dataRealCompSideLJudge = new bool[200];
        bool[] dataRealCompSideRJudge = new bool[200];

        bool[] dataRealExtnSideLJudge = new bool[200];
        bool[] dataRealExtnSideRJudge = new bool[200];

        float[] _dataRealCompSideLStroke = new float[200];
        float[] _dataRealCompSideLLoad = new float[200];
        float[] _dataRealCompSideLMaster = new float[200];
        float[] _dataRealCompSideLUpper = new float[200];
        float[] _dataRealCompSideLLower = new float[200];

        float[] _dataRealExtnSideLStroke = new float[200];
        float[] _dataRealExtnSideLLoad = new float[200];
        float[] _dataRealExtnSideLMaster = new float[200];
        float[] _dataRealExtnSideLUpper = new float[200];
        float[] _dataRealExtnSideLLower = new float[200];

        float[] _dataRealSideLDiffStroke = new float[200];
        float[] _dataRealSideLDiffLoad = new float[200];
        float[] _dataRealSideLDiffMaster = new float[200];
        float[] _dataRealSideLDiffUpper = new float[200];
        float[] _dataRealSideLDiffLower = new float[200];

        float[] _dataRealCompSideRStroke = new float[200];
        float[] _dataRealCompSideRLoad = new float[200];
        float[] _dataRealCompSideRMaster = new float[200];
        float[] _dataRealCompSideRUpper = new float[200];
        float[] _dataRealCompSideRLower = new float[200];

        float[] _dataRealExtnSideRStroke = new float[200];
        float[] _dataRealExtnSideRLoad = new float[200];
        float[] _dataRealExtnSideRMaster = new float[200];
        float[] _dataRealExtnSideRUpper = new float[200];
        float[] _dataRealExtnSideRLower = new float[200];

        float[] _dataRealSideRDiffStroke = new float[200];
        float[] _dataRealSideRDiffLoad = new float[200];
        float[] _dataRealSideRDiffMaster = new float[200];
        float[] _dataRealSideRDiffUpper = new float[200];
        float[] _dataRealSideRDiffLower = new float[200];

        public float[] DataRealCompSideLStroke
        {
            get { return _dataRealCompSideLStroke; }
            set { _dataRealCompSideLStroke = value; }
        }

        public float[] DataRealCompSideLLoad
        {
            get { return _dataRealCompSideLLoad; }
            set { _dataRealCompSideLLoad = value; }
        }

        public float[] DataRealCompSideLMaster
        {
            get { return _dataRealCompSideLMaster; }
            set { _dataRealCompSideLMaster = value; }
        }

        public float[] DataRealCompSideLUpper
        {
            get { return _dataRealCompSideLUpper; }
            set { _dataRealCompSideLUpper = value; }
        }

        public float[] DataRealCompSideLLower
        {
            get { return _dataRealCompSideLLower; }
            set { _dataRealCompSideLLower = value; }
        }

        public float[] DataRealExtnSideLStroke
        {
            get { return _dataRealExtnSideLStroke; }
            set { _dataRealExtnSideLStroke = value; }
        }

        public float[] DataRealExtnSideLLoad
        {
            get { return _dataRealExtnSideLLoad; }
            set { _dataRealExtnSideLLoad = value; }
        }

        public float[] DataRealExtnSideLMaster
        {
            get { return _dataRealExtnSideLMaster; }
            set { _dataRealExtnSideLMaster = value; }
        }

        public float[] DataRealExtnSideLUpper
        {
            get { return _dataRealExtnSideLUpper; }
            set { _dataRealExtnSideLUpper = value; }
        }

        public float[] DataRealExtnSideLLower
        {
            get { return _dataRealExtnSideLLower; }
            set { _dataRealExtnSideLLower = value; }
        }

        public float[] DataRealCompSideRStroke
        {
            get { return _dataRealCompSideRStroke; }
            set { _dataRealCompSideRStroke = value; }
        }

        public float[] DataRealCompSideRLoad
        {
            get { return _dataRealCompSideRLoad; }
            set { _dataRealCompSideRLoad = value; }
        }

        public float[] DataRealCompSideRMaster
        {
            get { return _dataRealCompSideRMaster; }
            set { _dataRealCompSideRMaster = value; }
        }

        public float[] DataRealCompSideRUpper
        {
            get { return _dataRealCompSideRUpper; }
            set { _dataRealCompSideRUpper = value; }
        }

        public float[] DataRealCompSideRLower
        {
            get { return _dataRealCompSideRLower; }
            set { _dataRealCompSideRLower = value; }
        }

        public float[] DataRealExtnSideRStroke
        {
            get { return _dataRealExtnSideRStroke; }
            set { _dataRealExtnSideRStroke = value; }
        }

        public float[] DataRealExtnSideRLoad
        {
            get { return _dataRealExtnSideRLoad; }
            set { _dataRealExtnSideRLoad = value; }
        }

        public float[] DataRealExtnSideRMaster
        {
            get { return _dataRealExtnSideRMaster; }
            set { _dataRealExtnSideRMaster = value; }
        }

        public float[] DataRealExtnSideRUpper
        {
            get { return _dataRealExtnSideRUpper; }
            set { _dataRealExtnSideRUpper = value; }
        }

        public float[] DataRealExtnSideRLower
        {
            get { return _dataRealExtnSideRLower; }
            set { _dataRealExtnSideRLower = value; }
        }
        public float[] DataRealSideLDiffStroke
        {
            get { return _dataRealSideLDiffStroke; }
            set { _dataRealSideLDiffStroke = value; }
        }

        public float[] DataRealSideLDiffLoad
        {
            get { return _dataRealSideLDiffLoad; }
            set { _dataRealSideLDiffLoad = value; }
        }

        public float[] DataRealSideLDiffMaster
        {
            get { return _dataRealSideLDiffMaster; }
            set { _dataRealSideLDiffMaster = value; }
        }

        public float[] DataRealSideLDiffUpper
        {
            get { return _dataRealSideLDiffUpper; }
            set { _dataRealSideLDiffUpper = value; }
        }

        public float[] DataRealSideLDiffLower
        {
            get { return _dataRealSideLDiffLower; }
            set { _dataRealSideLDiffLower = value; }
        }

        public float[] DataRealSideRDiffStroke
        {
            get { return _dataRealSideRDiffStroke; }
            set { _dataRealSideRDiffStroke = value; }
        }

        public float[] DataRealSideRDiffLoad
        {
            get { return _dataRealSideRDiffLoad; }
            set { _dataRealSideRDiffLoad = value; }
        }

        public float[] DataRealSideRDiffMaster
        {
            get { return _dataRealSideRDiffMaster; }
            set { _dataRealSideRDiffMaster = value; }
        }

        public float[] DataRealSideRDiffUpper
        {
            get { return _dataRealSideRDiffUpper; }
            set { _dataRealSideRDiffUpper = value; }
        }

        public float[] DataRealSideRDiffLower
        {
            get { return _dataRealSideRDiffLower; }
            set { _dataRealSideRDiffLower = value; }
        }

        #endregion

        #region Master Data
        CustomTableLayoutPanel tabMasterSideL;
        List<TextBox> tabMasterSideLStroke = new List<TextBox>();
        List<TextBox> tabMasterSideLMaster = new List<TextBox>();
        List<TextBox> tabMasterSideLAccMaster = new List<TextBox>();
        List<TextBox> tabMasterSideLLower = new List<TextBox>();
        List<TextBox> tabMasterSideLUpper = new List<TextBox>();

        CustomTableLayoutPanel tabMasterSideR;
        List<TextBox> tabMasterSideRStroke = new List<TextBox>();
        List<TextBox> tabMasterSideRMaster = new List<TextBox>();
        List<TextBox> tabMasterSideRAccMaster = new List<TextBox>();
        List<TextBox> tabMasterSideRLower = new List<TextBox>();
        List<TextBox> tabMasterSideRUpper = new List<TextBox>();

        float[] _dataMasterCompSideLStroke = new float[200];
        float[] _dataMasterCompSideLMaster = new float[200];
        float[] _dataMasterCompSideLAccMaster = new float[200];
        float[] _dataMasterCompSideLUpper = new float[200];
        float[] _dataMasterCompSideLLower = new float[200];

        float[] _dataMasterExtnSideLStroke = new float[200];
        float[] _dataMasterExtnSideLMaster = new float[200];
        float[] _dataMasterExtnSideLAccMaster = new float[200];
        float[] _dataMasterExtnSideLUpper = new float[200];
        float[] _dataMasterExtnSideLLower = new float[200];

        float[] _dataMasterSideLDiffStroke = new float[200];
        float[] _dataMasterSideLDiffMaster = new float[200];
        float[] _dataMasterSideLDiffAccMaster = new float[200];
        float[] _dataMasterSideLDiffUpper = new float[200];
        float[] _dataMasterSideLDiffLower = new float[200];

        float[] _dataMasterCompSideRStroke = new float[200];
        float[] _dataMasterCompSideRMaster = new float[200];
        float[] _dataMasterCompSideRAccMaster = new float[200];
        float[] _dataMasterCompSideRUpper = new float[200];
        float[] _dataMasterCompSideRLower = new float[200];

        float[] _dataMasterExtnSideRStroke = new float[200];
        float[] _dataMasterExtnSideRMaster = new float[200];
        float[] _dataMasterExtnSideRAccMaster = new float[200];
        float[] _dataMasterExtnSideRUpper = new float[200];
        float[] _dataMasterExtnSideRLower = new float[200];

        float[] _dataMasterSideRDiffStroke = new float[200];
        float[] _dataMasterSideRDiffMaster = new float[200];
        float[] _dataMasterSideRDiffAccMaster = new float[200];
        float[] _dataMasterSideRDiffUpper = new float[200];
        float[] _dataMasterSideRDiffLower = new float[200];


        public float[] DataMasterCompSideLStroke
        {
            get { return _dataMasterCompSideLStroke; }
            set { _dataMasterCompSideLStroke = value; }
        }

        public float[] DataMasterCompSideLMaster
        {
            get { return _dataMasterCompSideLMaster; }
            set { _dataMasterCompSideLMaster = value; }
        }

        public float[] DataMasterCompSideLAccMaster
        {
            get { return _dataMasterCompSideLAccMaster; }
            set { _dataMasterCompSideLAccMaster = value; }
        }

        public float[] DataMasterCompSideLUpper
        {
            get { return _dataMasterCompSideLUpper; }
            set { _dataMasterCompSideLUpper = value; }
        }

        public float[] DataMasterCompSideLLower
        {
            get { return _dataMasterCompSideLLower; }
            set { _dataMasterCompSideLLower = value; }
        }

        public float[] DataMasterExtnSideLStroke
        {
            get { return _dataMasterExtnSideLStroke; }
            set { _dataMasterExtnSideLStroke = value; }
        }

        public float[] DataMasterExtnSideLMaster
        {
            get { return _dataMasterExtnSideLMaster; }
            set { _dataMasterExtnSideLMaster = value; }
        }

        public float[] DataMasterExtnSideLAccMaster
        {
            get { return _dataMasterExtnSideLAccMaster; }
            set { _dataMasterExtnSideLAccMaster = value; }
        }

        public float[] DataMasterExtnSideLUpper
        {
            get { return _dataMasterExtnSideLUpper; }
            set { _dataMasterExtnSideLUpper = value; }
        }

        public float[] DataMasterExtnSideLLower
        {
            get { return _dataMasterExtnSideLLower; }
            set { _dataMasterExtnSideLLower = value; }
        }

        public float[] DataMasterCompSideRStroke
        {
            get { return _dataMasterCompSideRStroke; }
            set { _dataMasterCompSideRStroke = value; }
        }

        public float[] DataMasterCompSideRMaster
        {
            get { return _dataMasterCompSideRMaster; }
            set { _dataMasterCompSideRMaster = value; }
        }

        public float[] DataMasterCompSideRAccMaster
        {
            get { return _dataMasterCompSideRAccMaster; }
            set { _dataMasterCompSideRAccMaster = value; }
        }

        public float[] DataMasterCompSideRUpper
        {
            get { return _dataMasterCompSideRUpper; }
            set { _dataMasterCompSideRUpper = value; }
        }

        public float[] DataMasterCompSideRLower
        {
            get { return _dataMasterCompSideRLower; }
            set { _dataMasterCompSideRLower = value; }
        }

        public float[] DataMasterExtnSideRStroke
        {
            get { return _dataMasterExtnSideRStroke; }
            set { _dataMasterExtnSideRStroke = value; }
        }

        public float[] DataMasterExtnSideRMaster
        {
            get { return _dataMasterExtnSideRMaster; }
            set { _dataMasterExtnSideRMaster = value; }
        }

        public float[] DataMasterExtnSideRAccMaster
        {
            get { return _dataMasterExtnSideRAccMaster; }
            set { _dataMasterExtnSideRAccMaster = value; }
        }

        public float[] DataMasterExtnSideRUpper
        {
            get { return _dataMasterExtnSideRUpper; }
            set { _dataMasterExtnSideRUpper = value; }
        }

        public float[] DataMasterExtnSideRLower
        {
            get { return _dataMasterExtnSideRLower; }
            set { _dataMasterExtnSideRLower = value; }
        }

        public float[] DataMasterSideLDiffStroke
        {
            get { return _dataMasterSideLDiffStroke; }
            set { _dataMasterSideLDiffStroke = value; }
        }
        public float[] DataMasterSideLDiffMaster
        {
            get { return _dataMasterSideLDiffMaster; }
            set { _dataMasterSideLDiffMaster = value; }
        }

        public float[] DataMasterSideLDiffLower
        {
            get { return _dataMasterSideLDiffLower; }
            set { _dataMasterSideLDiffLower = value; }
        }

        public float[] DataMasterSideLDiffUpper
        {
            get { return _dataMasterSideLDiffUpper; }
            set { _dataMasterSideLDiffUpper = value; }
        }

        public float[] DataMasterSideRDiffStroke
        {
            get { return _dataMasterSideRDiffStroke; }
            set { _dataMasterSideRDiffStroke = value; }
        }
        public float[] DataMasterSideRDiffMaster
        {
            get { return _dataMasterSideRDiffMaster; }
            set { _dataMasterSideRDiffMaster = value; }
        }

        public float[] DataMasterSideRDiffLower
        {
            get { return _dataMasterSideRDiffLower; }
            set { _dataMasterSideRDiffLower = value; }
        }

        public float[] DataMasterSideRDiffUpper
        {
            get { return _dataMasterSideRDiffUpper; }
            set { _dataMasterSideRDiffUpper = value; }
        }

        public float[] DataMasterSideRDiffAccMaster
        {
            get { return _dataMasterSideRDiffAccMaster; }
            set { _dataMasterSideRDiffAccMaster = value; }
        }
        public float[] DataMasterSideLDiffAccMaster
        {
            get { return _dataMasterSideLDiffAccMaster; }
            set { _dataMasterSideLDiffAccMaster = value; }
        }

        #endregion

        #endregion

        #region UIComponents
        private void InitializeCustomComponents() //tab cover blank space
        {
            // Create a new Panel to contain the drawing
            drawingPanel = new Panel
            {
                BackColor = Color.Transparent,
                Location = new DRW.Point(985, 1025),
                Size = new Size(910, 24)
            };
            // Add the Paint event handler for custom drawing
            drawingPanel.Paint += DrawingPanel_Paint;
            // Add the drawing panel to the form
            this.Controls.Add(drawingPanel);
        }
        private void InitializeBorderComponent()//border
        {
            drawingBorderUpper = new Panel
            {
                BackColor = Color.Transparent,
                Location = new DRW.Point(10, 5),
                Size = new Size(1878, 4)
            };

            drawingBorderLower = new Panel
            {
                BackColor = Color.Transparent,
                Location = new DRW.Point(10, 1020),
                Size = new Size(1878, 4)
            };

            drawingBorderLeft = new Panel
            {
                BackColor = Color.Transparent,
                Location = new DRW.Point(10, 5),
                Size = new Size(4, 1020)
            };

            drawingBorderRight = new Panel
            {
                BackColor = Color.Transparent,
                Location = new DRW.Point(1883, 5),
                Size = new Size(5, 1020)
            };

            drawingBorderUpper.Paint += DrawingBorderUpper_Paint;
            drawingBorderLower.Paint += DrawingBorderLower_Paint;
            drawingBorderLeft.Paint += DrawingBorderLeft_Paint;
            drawingBorderRight.Paint += DrawingBorderRight_Paint;

            this.Controls.Add(drawingBorderUpper);
            this.Controls.Add(drawingBorderLower);
            this.Controls.Add(drawingBorderLeft);
            this.Controls.Add(drawingBorderRight);
        }

        private void TabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabControl tabControl = sender as TabControl;
            Graphics g = e.Graphics;
            DRW.Rectangle bounds;
            DRW.Rectangle nokoribounds;
            Color textColor;
            Color backgroundColor;


            for (int i = 0; i < tabControl.TabCount; i++)
            {
                bounds = tabControl.GetTabRect(i);
                if (i == tabControl.SelectedIndex)
                {
                    string hexColor = "#037B7B";
                    Color colorconvert = ColorTranslator.FromHtml(hexColor);
                    backgroundColor = colorconvert;
                    textColor = Color.Ivory;
                    using (Brush backgroundBrush = new SolidBrush(backgroundColor))
                    {
                        g.FillRectangle(backgroundBrush, bounds);
                    }
                    string tabText = tabControl.TabPages[i].Text;
                    TextRenderer.DrawText(g, tabText, tabControl.Font, bounds, textColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }
                else
                {
                    backgroundColor = Color.DarkSlateGray;
                    textColor = Color.Ivory;
                    using (Brush backgroundBrush = new SolidBrush(backgroundColor))
                    {
                        g.FillRectangle(backgroundBrush, bounds);
                    }
                    string tabText = tabControl.TabPages[i].Text;
                    TextRenderer.DrawText(g, tabText, tabControl.Font, bounds, textColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }
            }
        }
        private void DrawingPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            // Draw a rectangle
            g.FillRectangle(Brushes.DarkSlateGray, new DRW.Rectangle(0, 0, 1878, 24));
        }
        private void DrawingBorderUpper_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.FillRectangle(Brushes.DarkCyan, new DRW.Rectangle(0, 0, 1878, 4));
        }
        private void DrawingBorderLower_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.FillRectangle(Brushes.DarkCyan, new DRW.Rectangle(0, 0, 1878, 4));
        }
        private void DrawingBorderLeft_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.FillRectangle(Brushes.DarkCyan, new DRW.Rectangle(0, 0, 4, 1020));
        }
        private void DrawingBorderRight_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.FillRectangle(Brushes.DarkCyan, new DRW.Rectangle(0, 0, 5, 1020));
        }

        public void defLayoutPanelRealSideR()
        {
            tabRealSideR = new CustomTableLayoutPanel()
            {
                ColumnCount = 5,
                RowCount = 41,
                Location = new Point(1163, 3),
                Size = new Size(700, 1000),
                AutoSize = false,
                CellBorderColor = ColorTranslator.FromHtml("#037B7B") // Set the desired border color
            };

            tabRealSideR.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16F));
            for (int i = 0; i < 4; i++)
            {
                tabRealSideR.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 21F));
            }

            tabRealSideR.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
            for (int i = 0; i < 40; i++)
            {
                tabRealSideR.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
            }

            Label lb1 = new Label
            {
                Text = $"STROKE[mm]",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle,
                AutoSize = false,
                Font = new System.Drawing.Font("Work Sans", 10, FontStyle.Bold),
                Size = new Size(100, 20),
                BackColor = Color.Cyan
            };
            tabRealSideR.Controls.Add(lb1, 0, 0);

            Label lb2 = new Label
            {
                Text = $"MASTER [N]",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle,
                AutoSize = false,
                Font = new System.Drawing.Font("Work Sans", 10, FontStyle.Bold),
                Size = new Size(80, 20),
                BackColor = Color.Cyan
            };
            tabRealSideR.Controls.Add(lb2, 1, 0);

            Label lb3 = new Label
            {
                Text = $"LOWER [N]",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle,
                AutoSize = false,
                Font = new System.Drawing.Font("Work Sans", 10, FontStyle.Bold),
                Size = new Size(80, 20),
                BackColor = Color.Cyan
            };
            tabRealSideR.Controls.Add(lb3, 2, 0);

            Label lb4 = new Label
            {
                Text = $"REALTIME [N]",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle,
                AutoSize = false,
                Font = new System.Drawing.Font("Work Sans", 10, FontStyle.Bold),
                Size = new Size(80, 20),
                BackColor = Color.Cyan
            };
            tabRealSideR.Controls.Add(lb4, 3, 0);

            Label lb5 = new Label
            {
                Text = $"UPPER [N]",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle,
                AutoSize = false,
                Font = new System.Drawing.Font("Work Sans", 10, FontStyle.Bold),
                Size = new Size(80, 20),
                BackColor = Color.Cyan
            };
            tabRealSideR.Controls.Add(lb5, 4, 0);

            // Add controls to CustomTableLayoutPanel
            for (int col = 0; col < 1; col++)
            {
                for (int row = 1; row < 41; row++)
                {
                    TextBox tbx = new TextBox
                    {
                        Text = $"{row}",
                        Dock = DockStyle.Fill,
                        TextAlign = System.Windows.Forms.HorizontalAlignment.Center,
                        Margin = new Padding(0),
                        Font = new System.Drawing.Font("Work Sans", 10, FontStyle.Bold),
                        Size = new Size(80, 24),
                        ReadOnly = true,
                        BackColor = Color.LightGray,

                    };
                    tabRealSideRStroke.Add(tbx);
                    tabRealSideR.Controls.Add(tbx, col, row);
                }
            }
            for (int row = 1; row < 41; row++)
            {
                TextBox tbx = new TextBox
                {
                    Dock = DockStyle.Fill,
                    TextAlign = System.Windows.Forms.HorizontalAlignment.Center,
                    Margin = new Padding(0),
                    Font = new System.Drawing.Font("Work Sans", 10, FontStyle.Bold),
                    Size = new Size(80, 24),
                    ReadOnly = true,

                };
                tbx.TextChanged += new System.EventHandler(this.DecimalTextBox_TextChanged);
                tabRealSideRMaster.Add(tbx);
                tabRealSideR.Controls.Add(tbx, 1, row);
            }
            for (int row = 1; row < 41; row++)
            {
                TextBox tbx = new TextBox
                {
                    Dock = DockStyle.Fill,
                    TextAlign = System.Windows.Forms.HorizontalAlignment.Center,
                    Margin = new Padding(0),
                    Font = new System.Drawing.Font("Work Sans", 10, FontStyle.Bold),
                    Size = new Size(80, 24),
                    ReadOnly = true,

                };
                tbx.TextChanged += new System.EventHandler(this.DecimalTextBox_TextChanged);
                tabRealSideRLower.Add(tbx);
                tabRealSideR.Controls.Add(tbx, 2, row);
            }
            for (int row = 1; row < 41; row++)
            {
                TextBox tbx = new TextBox
                {
                    Dock = DockStyle.Fill,
                    TextAlign = System.Windows.Forms.HorizontalAlignment.Center,
                    Margin = new Padding(0),
                    Font = new System.Drawing.Font("Work Sans", 10, FontStyle.Bold),
                    Size = new Size(80, 24),
                    ReadOnly = true,

                };
                tbx.TextChanged += new System.EventHandler(this.DecimalTextBox_TextChanged);
                tabRealSideRReal.Add(tbx);
                tabRealSideR.Controls.Add(tbx, 3, row);
            }
            for (int row = 1; row < 41; row++)
            {
                TextBox tbx = new TextBox
                {
                    Dock = DockStyle.Fill,
                    TextAlign = System.Windows.Forms.HorizontalAlignment.Center,
                    Margin = new Padding(0),
                    Font = new System.Drawing.Font("Work Sans", 10, FontStyle.Bold),
                    Size = new Size(80, 24),
                    ReadOnly = true,

                };
                tbx.TextChanged += new System.EventHandler(this.DecimalTextBox_TextChanged);
                tabRealSideRUpper.Add(tbx);
                tabRealSideR.Controls.Add(tbx, 4, row);
            }
            // Add the CustomTableLayoutPanel to the form
            tabPage7.Controls.Add(tabRealSideR);
            tabRealSideR.BringToFront();
        }
        public void defLayoutPanelRealSideL()
        {
            tabRealSideL = new CustomTableLayoutPanel()
            {
                ColumnCount = 5,
                RowCount = 41,
                Location = new Point(3, 3),
                Size = new Size(700, 1000),
                AutoSize = false,
                CellBorderColor = ColorTranslator.FromHtml("#037B7B") // Set the desired border color
            };

            tabRealSideL.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16F));
            for (int i = 0; i < 4; i++)
            {
                tabRealSideL.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 21F));
            }

            tabRealSideL.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
            for (int i = 0; i < 40; i++)
            {
                tabRealSideL.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
            }

            Label lb1 = new Label
            {
                Text = $"STROKE[mm]",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle,
                AutoSize = false,
                Font = new System.Drawing.Font("Work Sans", 10, FontStyle.Bold),
                Size = new Size(100, 20),
                BackColor = Color.Cyan
            };
            tabRealSideL.Controls.Add(lb1, 0, 0);

            Label lb2 = new Label
            {
                Text = $"MASTER [N]",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle,
                AutoSize = false,
                Font = new System.Drawing.Font("Work Sans", 10, FontStyle.Bold),
                Size = new Size(80, 20),
                BackColor = Color.Cyan
            };
            tabRealSideL.Controls.Add(lb2, 1, 0);

            Label lb3 = new Label
            {
                Text = $"LOWER [N]",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle,
                AutoSize = false,
                Font = new System.Drawing.Font("Work Sans", 10, FontStyle.Bold),
                Size = new Size(80, 20),
                BackColor = Color.Cyan
            };
            tabRealSideL.Controls.Add(lb3, 2, 0);

            Label lb4 = new Label
            {
                Text = $"REALTIME [N]",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle,
                AutoSize = false,
                Font = new System.Drawing.Font("Work Sans", 10, FontStyle.Bold),
                Size = new Size(80, 20),
                BackColor = Color.Cyan
            };
            tabRealSideL.Controls.Add(lb4, 3, 0);

            Label lb5 = new Label
            {
                Text = $"UPPER [N]",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle,
                AutoSize = false,
                Font = new System.Drawing.Font("Work Sans", 10, FontStyle.Bold),
                Size = new Size(80, 20),
                BackColor = Color.Cyan
            };
            tabRealSideL.Controls.Add(lb5, 4, 0);

            // Add controls to CustomTableLayoutPanel
            for (int col = 0; col < 1; col++)
            {
                for (int row = 1; row < 41; row++)
                {
                    TextBox tbx = new TextBox
                    {
                        Text = $"{row}",
                        Dock = DockStyle.Fill,
                        TextAlign = System.Windows.Forms.HorizontalAlignment.Center,
                        Margin = new Padding(0),
                        Font = new System.Drawing.Font("Work Sans", 10, FontStyle.Bold),
                        Size = new Size(80, 24),
                        ReadOnly = true,
                        BackColor = Color.LightGray

                    };
                    tabRealSideLStroke.Add(tbx);
                    tabRealSideL.Controls.Add(tbx, col, row);
                }
            }
            for (int row = 1; row < 41; row++)
            {
                TextBox tbx = new TextBox
                {
                    Dock = DockStyle.Fill,
                    TextAlign = System.Windows.Forms.HorizontalAlignment.Center,
                    Margin = new Padding(0),
                    Font = new System.Drawing.Font("Work Sans", 10, FontStyle.Bold),
                    Size = new Size(80, 24),
                    ReadOnly = true,


                };
                tbx.TextChanged += new System.EventHandler(this.DecimalTextBox_TextChanged);
                tabRealSideLMaster.Add(tbx);
                tabRealSideL.Controls.Add(tbx, 1, row);
            }
            for (int row = 1; row < 41; row++)
            {
                TextBox tbx = new TextBox
                {
                    Dock = DockStyle.Fill,
                    TextAlign = System.Windows.Forms.HorizontalAlignment.Center,
                    Margin = new Padding(0),
                    Font = new System.Drawing.Font("Work Sans", 10, FontStyle.Bold),
                    Size = new Size(80, 24),
                    ReadOnly = true,

                };
                tbx.TextChanged += new System.EventHandler(this.DecimalTextBox_TextChanged);
                tabRealSideLLower.Add(tbx);
                tabRealSideL.Controls.Add(tbx, 2, row);
            }
            for (int row = 1; row < 41; row++)
            {
                TextBox tbx = new TextBox
                {
                    Dock = DockStyle.Fill,
                    TextAlign = System.Windows.Forms.HorizontalAlignment.Center,
                    Margin = new Padding(0),
                    Font = new System.Drawing.Font("Work Sans", 10, FontStyle.Bold),
                    Size = new Size(80, 24),
                    ReadOnly = true,

                };
                tbx.TextChanged += new System.EventHandler(this.DecimalTextBox_TextChanged);
                tabRealSideLReal.Add(tbx);
                tabRealSideL.Controls.Add(tbx, 3, row);
            }
            for (int row = 1; row < 41; row++)
            {
                TextBox tbx = new TextBox
                {
                    Dock = DockStyle.Fill,
                    TextAlign = System.Windows.Forms.HorizontalAlignment.Center,
                    Margin = new Padding(0),
                    Font = new System.Drawing.Font("Work Sans", 10, FontStyle.Bold),
                    Size = new Size(80, 24),
                    ReadOnly = true,

                };
                tbx.TextChanged += new System.EventHandler(this.DecimalTextBox_TextChanged);
                tabRealSideLUpper.Add(tbx);
                tabRealSideL.Controls.Add(tbx, 4, row);
            }
            // Add the CustomTableLayoutPanel to the form
            tabPage7.Controls.Add(tabRealSideL);
            tabRealSideL.BringToFront();
        }
        public void defLayoutPanelMasterSideR()
        {
            tabMasterSideR = new CustomTableLayoutPanel()
            {
                ColumnCount = 5,
                RowCount = 41,
                Location = new Point(1163, 3),
                Size = new Size(700, 1000),
                AutoSize = false,
                CellBorderColor = ColorTranslator.FromHtml("#037B7B") // Set the desired border color
            };

            tabMasterSideR.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16F));
            for (int i = 0; i < 4; i++)
            {
                tabMasterSideR.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 21F));
            }

            tabMasterSideR.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
            for (int i = 0; i < 40; i++)
            {
                tabMasterSideR.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
            }

            Label lb1 = new Label
            {
                Text = $"STROKE[mm]",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle,
                AutoSize = false,
                Font = new System.Drawing.Font("Work Sans", 10, FontStyle.Bold),
                Size = new Size(100, 20),
                BackColor = Color.Cyan
            };
            tabMasterSideR.Controls.Add(lb1, 0, 0);

            Label lb2 = new Label
            {
                Text = $"MASTER [N]",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle,
                AutoSize = false,
                Font = new System.Drawing.Font("Work Sans", 10, FontStyle.Bold),
                Size = new Size(80, 20),
                BackColor = Color.Cyan
            };
            tabMasterSideR.Controls.Add(lb2, 1, 0);

            Label lb3 = new Label
            {
                Text = $"ACC MASTER [N]",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle,
                AutoSize = false,
                Font = new System.Drawing.Font("Work Sans", 10, FontStyle.Bold),
                Size = new Size(80, 20),
                BackColor = Color.Cyan
            };
            tabMasterSideR.Controls.Add(lb3, 2, 0);

            Label lb4 = new Label
            {
                Text = $"LOWER [N]",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle,
                AutoSize = false,
                Font = new System.Drawing.Font("Work Sans", 10, FontStyle.Bold),
                Size = new Size(80, 20),
                BackColor = Color.Cyan
            };
            tabMasterSideR.Controls.Add(lb4, 3, 0);

            Label lb5 = new Label
            {
                Text = $"UPPER [N]",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle,
                AutoSize = false,
                Font = new System.Drawing.Font("Work Sans", 10, FontStyle.Bold),
                Size = new Size(80, 20),
                BackColor = Color.Cyan
            };
            tabMasterSideR.Controls.Add(lb5, 4, 0);

            // Add controls to CustomTableLayoutPanel
            for (int col = 0; col < 1; col++)
            {
                for (int row = 1; row < 41; row++)
                {
                    TextBox tbx = new TextBox
                    {
                        Text = $"{row}",
                        Dock = DockStyle.Fill,
                        TextAlign = System.Windows.Forms.HorizontalAlignment.Center,
                        Margin = new Padding(0),
                        Font = new System.Drawing.Font("Work Sans", 10, FontStyle.Bold),
                        Size = new Size(80, 24),
                        ReadOnly = true,
                        BackColor = Color.LightGray

                    };
                    tabMasterSideRStroke.Add(tbx);
                    tabMasterSideR.Controls.Add(tbx, col, row);
                }
            }
            for (int row = 1; row < 41; row++)
            {
                TextBox tbx = new TextBox
                {
                    Dock = DockStyle.Fill,
                    TextAlign = System.Windows.Forms.HorizontalAlignment.Center,
                    Margin = new Padding(0),
                    Font = new System.Drawing.Font("Work Sans", 10, FontStyle.Bold),
                    Size = new Size(80, 24),

                };
                tbx.TextChanged += new System.EventHandler(this.DecimalTextBox_TextChanged);
                tbx.Click += new EventHandler(InputTextBoxMasterTable_Click);
                tabMasterSideRMaster.Add(tbx);
                tabMasterSideR.Controls.Add(tbx, 1, row);
            }
            for (int row = 1; row < 41; row++)
            {
                TextBox tbx = new TextBox
                {
                    Dock = DockStyle.Fill,
                    TextAlign = System.Windows.Forms.HorizontalAlignment.Center,
                    Margin = new Padding(0),
                    Font = new System.Drawing.Font("Work Sans", 10, FontStyle.Bold),
                    Size = new Size(80, 24),

                };
                tbx.TextChanged += new System.EventHandler(this.DecimalTextBox_TextChanged);
                tbx.Click += new EventHandler(InputTextBoxMasterTable_Click);
                tabMasterSideRAccMaster.Add(tbx);
                tabMasterSideR.Controls.Add(tbx, 2, row);
            }
            for (int row = 1; row < 41; row++)
            {
                TextBox tbx = new TextBox
                {
                    Dock = DockStyle.Fill,
                    TextAlign = System.Windows.Forms.HorizontalAlignment.Center,
                    Margin = new Padding(0),
                    Font = new System.Drawing.Font("Work Sans", 10, FontStyle.Bold),
                    Size = new Size(80, 24),

                };
                tbx.TextChanged += new System.EventHandler(this.DecimalTextBox_TextChanged);
                tbx.Click += new EventHandler(InputTextBoxMasterTable_Click);
                tabMasterSideRLower.Add(tbx);
                tabMasterSideR.Controls.Add(tbx, 3, row);
            }
            for (int row = 1; row < 41; row++)
            {
                TextBox tbx = new TextBox
                {
                    Dock = DockStyle.Fill,
                    TextAlign = System.Windows.Forms.HorizontalAlignment.Center,
                    Margin = new Padding(0),
                    Font = new System.Drawing.Font("Work Sans", 10, FontStyle.Bold),
                    Size = new Size(80, 24),

                };
                tbx.TextChanged += new System.EventHandler(this.DecimalTextBox_TextChanged);
                tbx.Click += new EventHandler(InputTextBoxMasterTable_Click);
                tabMasterSideRUpper.Add(tbx);
                tabMasterSideR.Controls.Add(tbx, 4, row);
            }
            // Add the CustomTableLayoutPanel to the form
            tabPage8.Controls.Add(tabMasterSideR);
            tabMasterSideR.BringToFront();
        }
        public void defLayoutPanelMasterSideL()
        {
            tabMasterSideL = new CustomTableLayoutPanel()
            {
                ColumnCount = 5,
                RowCount = 41,
                Location = new Point(3, 3),
                Size = new Size(700, 1000),
                AutoSize = false,
                CellBorderColor = ColorTranslator.FromHtml("#037B7B") // Set the desired border color
            };

            tabMasterSideL.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16F));
            for (int i = 0; i < 4; i++)
            {
                tabMasterSideL.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 21F));
            }

            tabMasterSideL.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
            for (int i = 0; i < 40; i++)
            {
                tabMasterSideL.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
            }

            Label lb1 = new Label
            {
                Text = $"STROKE[mm]",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle,
                AutoSize = false,
                Font = new System.Drawing.Font("Work Sans", 10, FontStyle.Bold),
                Size = new Size(100, 20),
                BackColor = Color.Cyan
            };
            tabMasterSideL.Controls.Add(lb1, 0, 0);

            Label lb2 = new Label
            {
                Text = $"MASTER [N]",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle,
                AutoSize = false,
                Font = new System.Drawing.Font("Work Sans", 10, FontStyle.Bold),
                Size = new Size(80, 20),
                BackColor = Color.Cyan
            };
            tabMasterSideL.Controls.Add(lb2, 1, 0);

            Label lb3 = new Label
            {
                Text = $"ACC MASTER [N]",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle,
                AutoSize = false,
                Font = new System.Drawing.Font("Work Sans", 10, FontStyle.Bold),
                Size = new Size(80, 20),
                BackColor = Color.Cyan
            };
            tabMasterSideL.Controls.Add(lb3, 2, 0);

            Label lb4 = new Label
            {
                Text = $"LOWER [N]",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle,
                AutoSize = false,
                Font = new System.Drawing.Font("Work Sans", 10, FontStyle.Bold),
                Size = new Size(80, 20),
                BackColor = Color.Cyan
            };
            tabMasterSideL.Controls.Add(lb4, 3, 0);

            Label lb5 = new Label
            {
                Text = $"UPPER [N]",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle,
                AutoSize = false,
                Font = new System.Drawing.Font("Work Sans", 10, FontStyle.Bold),
                Size = new Size(80, 20),
                BackColor = Color.Cyan
            };
            tabMasterSideL.Controls.Add(lb5, 4, 0);

            // Add controls to CustomTableLayoutPanel
            for (int col = 0; col < 1; col++)
            {
                for (int row = 1; row < 41; row++)
                {
                    TextBox tbx = new TextBox
                    {
                        Text = $"{row}",
                        Dock = DockStyle.Fill,
                        TextAlign = System.Windows.Forms.HorizontalAlignment.Center,
                        Margin = new Padding(0),
                        Font = new System.Drawing.Font("Work Sans", 10, FontStyle.Bold),
                        Size = new Size(80, 24),
                        ReadOnly = true,
                        BackColor = Color.LightGray

                    };
                    tabMasterSideLStroke.Add(tbx);
                    tabMasterSideL.Controls.Add(tbx, col, row);
                }
            }
            for (int row = 1; row < 41; row++)
            {
                TextBox tbx = new TextBox
                {
                    Dock = DockStyle.Fill,
                    TextAlign = System.Windows.Forms.HorizontalAlignment.Center,
                    Margin = new Padding(0),
                    Font = new System.Drawing.Font("Work Sans", 10, FontStyle.Bold),
                    Size = new Size(80, 24),
                    ReadOnly = true,


                };
                tbx.TextChanged += new System.EventHandler(this.DecimalTextBox_TextChanged);
                tbx.Click += new EventHandler(InputTextBoxMasterTable_Click);
                tabMasterSideLMaster.Add(tbx);
                tabMasterSideL.Controls.Add(tbx, 1, row);
            }
            for (int row = 1; row < 41; row++)
            {
                TextBox tbx = new TextBox
                {
                    Dock = DockStyle.Fill,
                    TextAlign = System.Windows.Forms.HorizontalAlignment.Center,
                    Margin = new Padding(0),
                    Font = new System.Drawing.Font("Work Sans", 10, FontStyle.Bold),
                    Size = new Size(80, 24),
                    ReadOnly = true,

                };
                tbx.TextChanged += new System.EventHandler(this.DecimalTextBox_TextChanged);
                tbx.Click += new EventHandler(InputTextBoxMasterTable_Click);
                tabMasterSideLAccMaster.Add(tbx);
                tabMasterSideL.Controls.Add(tbx, 2, row);
            }
            for (int row = 1; row < 41; row++)
            {
                TextBox tbx = new TextBox
                {
                    Dock = DockStyle.Fill,
                    TextAlign = System.Windows.Forms.HorizontalAlignment.Center,
                    Margin = new Padding(0),
                    Font = new System.Drawing.Font("Work Sans", 10, FontStyle.Bold),
                    Size = new Size(80, 24),
                    ReadOnly = true,

                };
                tbx.TextChanged += new System.EventHandler(this.DecimalTextBox_TextChanged);
                tbx.Click += new EventHandler(InputTextBoxMasterTable_Click);
                tabMasterSideLLower.Add(tbx);
                tabMasterSideL.Controls.Add(tbx, 3, row);
            }
            for (int row = 1; row < 41; row++)
            {
                TextBox tbx = new TextBox
                {
                    Dock = DockStyle.Fill,
                    TextAlign = System.Windows.Forms.HorizontalAlignment.Center,
                    Margin = new Padding(0),
                    Font = new System.Drawing.Font("Work Sans", 10, FontStyle.Bold),
                    Size = new Size(80, 24),
                    ReadOnly = true,

                };
                tbx.TextChanged += new System.EventHandler(this.DecimalTextBox_TextChanged);
                tbx.Click += new EventHandler(InputTextBoxMasterTable_Click);
                tabMasterSideLUpper.Add(tbx);
                tabMasterSideL.Controls.Add(tbx, 4, row);
            }
            // Add the CustomTableLayoutPanel to the form
            tabPage8.Controls.Add(tabMasterSideL);
            tabMasterSideL.BringToFront();
        }

        #endregion

        #region Table Handler

        #region Real Data
        public void tabdataRealSideInit()
        {
            tabIdxRealSideL = 0;
            tabIdxRealSideR = 0;
            ActiveRealTableLeftData = 0;
            ActiveRealTableRightData = 0;
        }
        public void tabdataRealCompStep2L()
        {
            int tabIndex = tabIdxRealSideL * 40;
            for (int i = 0; i < 40; i++)
            {
                #region Stroke
                tabRealSideLStroke[i].Text = _dataRealCompSideLStroke[i + tabIndex].ToString();
                #endregion

                #region Master 
                tabRealSideLMaster[i].Text = RoundingEdge(_dataRealCompSideLMaster[i + tabIndex], 0.10f).ToString();
                #endregion

                #region Lower
                tabRealSideLLower[i].Text = RoundingEdge(_dataRealCompSideLLower[i + tabIndex], 0.10f).ToString();
                #endregion

                #region Realtime
                tabRealSideLReal[i].Text = RoundingEdge(_dataRealCompSideLLoad[i + tabIndex], 0.10f).ToString();
                if ((_dataRealCompSideLLoad[i + tabIndex] > _dataRealCompSideLUpper[i + tabIndex]) || (_dataRealCompSideLLoad[i + tabIndex] < _dataRealCompSideLLower[i + tabIndex]))
                {
                    tabRealSideLReal[i].BackColor = Color.Red;
                    tabRealSideLReal[i].ForeColor = Color.Ivory;
                }
                else
                {
                    tabRealSideLReal[i].BackColor = Color.White;
                    tabRealSideLReal[i].ForeColor = Color.Black;
                }
                #endregion

                #region Upper
                tabRealSideLUpper[i].Text = RoundingEdge(_dataRealCompSideLUpper[i + tabIndex], 0.10f).ToString();
                #endregion
            }
            ActiveRealTableLeftData = 1;
        }
        public void tabdataRealExtnStep2L()
        {
            int tabIndex = tabIdxRealSideL * 40;
            for (int i = 0; i < 40; i++)
            {
                #region Stroke
                tabRealSideLStroke[i].Text = _dataRealExtnSideLStroke[i + tabIndex].ToString();
                #endregion

                #region Master 
                tabRealSideLMaster[i].Text = RoundingEdge(_dataRealExtnSideLMaster[i + tabIndex], 0.10f).ToString();
                #endregion

                #region Lower
                tabRealSideLLower[i].Text = RoundingEdge(_dataRealExtnSideLLower[i + tabIndex], 0.10f).ToString();
                #endregion

                #region Realtime
                tabRealSideLReal[i].Text = RoundingEdge(_dataRealExtnSideLLoad[i + tabIndex], 0.10f).ToString();
                if ((_dataRealExtnSideLLoad[i + tabIndex] > _dataRealExtnSideLUpper[i + tabIndex]) || (_dataRealExtnSideLLoad[i + tabIndex] < _dataRealExtnSideLLower[i + tabIndex]))
                {
                    tabRealSideLReal[i].BackColor = Color.Red;
                    tabRealSideLReal[i].ForeColor = Color.Ivory;
                }
                else
                {
                    tabRealSideLReal[i].BackColor = Color.White;
                    tabRealSideLReal[i].ForeColor = Color.Black;
                }
                #endregion

                #region Upper
                tabRealSideLUpper[i].Text = RoundingEdge(_dataRealExtnSideLUpper[i + tabIndex], 0.10f).ToString();
                #endregion
            }
            ActiveRealTableLeftData = 2;
        }
        public void tabdataRealDiffStep2L()
        {
            int tabIndex = tabIdxRealSideL * 40;
            for (int i = 0; i < 40; i++)
            {
                #region Stroke
                tabRealSideLStroke[i].Text = _dataRealSideLDiffStroke[i + tabIndex].ToString();
                #endregion

                #region Master 
                tabRealSideLMaster[i].Text = RoundingEdge(_dataRealSideLDiffMaster[i + tabIndex], 0.10f).ToString();
                #endregion

                #region Lower
                tabRealSideLLower[i].Text = RoundingEdge(_dataRealSideLDiffLower[i + tabIndex], 0.10f).ToString();
                #endregion

                #region Realtime
                tabRealSideLReal[i].Text = RoundingEdge(_dataRealSideLDiffLoad[i + tabIndex], 0.10f).ToString();
                if ((_dataRealSideLDiffLoad[i + tabIndex] > _dataRealSideLDiffUpper[i + tabIndex]) || (_dataRealSideLDiffLoad[i + tabIndex] < _dataRealSideLDiffLower[i + tabIndex]))
                {
                    tabRealSideLReal[i].BackColor = Color.Red;
                    tabRealSideLReal[i].ForeColor = Color.Ivory;
                }
                else
                {
                    tabRealSideLReal[i].BackColor = Color.White;
                    tabRealSideLReal[i].ForeColor = Color.Black;
                }
                #endregion

                #region Upper
                tabRealSideLUpper[i].Text = RoundingEdge(_dataRealSideLDiffUpper[i + tabIndex], 0.10f).ToString();
                #endregion
            }
            ActiveRealTableLeftData = 3;
        }
        public void tabdataRealCompStep2R()
        {
            int tabIndex = tabIdxRealSideR * 40;
            for (int i = 0; i < 40; i++)
            {
                #region Stroke
                tabRealSideRStroke[i].Text = _dataRealCompSideRStroke[i + tabIndex].ToString();
                #endregion

                #region Master 
                tabRealSideRMaster[i].Text = RoundingEdge(_dataRealCompSideRMaster[i + tabIndex], 0.10f).ToString();
                #endregion

                #region Lower
                tabRealSideRLower[i].Text = RoundingEdge(_dataRealCompSideRLower[i + tabIndex], 0.10f).ToString();
                #endregion

                #region Realtime
                tabRealSideRReal[i].Text = RoundingEdge(_dataRealCompSideRLoad[i + tabIndex], 0.10f).ToString();
                if (_dataRealCompSideRLoad[i + tabIndex] > _dataRealCompSideRUpper[i + tabIndex] || _dataRealCompSideRLoad[i + tabIndex] < _dataRealCompSideRLower[i + tabIndex])
                {
                    tabRealSideRReal[i].BackColor = Color.Red;
                    tabRealSideRReal[i].ForeColor = Color.Ivory;
                }
                else
                {
                    tabRealSideRReal[i].BackColor = Color.White;
                    tabRealSideRReal[i].ForeColor = Color.Black;
                }
                #endregion

                #region Upper
                tabRealSideRUpper[i].Text = RoundingEdge(_dataRealCompSideRUpper[i + tabIndex], 0.10f).ToString();
                #endregion
            }
            ActiveRealTableRightData = 1;
        }
        public void tabdataRealExtnStep2R()
        {
            int tabIndex = tabIdxRealSideR * 40;
            for (int i = 0; i < 40; i++)
            {
                #region Stroke
                tabRealSideRStroke[i].Text = _dataRealExtnSideRStroke[i + tabIndex].ToString();
                #endregion

                #region Master 
                tabRealSideRMaster[i].Text = RoundingEdge(_dataRealExtnSideRMaster[i + tabIndex], 0.10f).ToString();
                #endregion

                #region Lower
                tabRealSideRLower[i].Text = RoundingEdge(_dataRealExtnSideRLower[i + tabIndex], 0.10f).ToString();
                #endregion

                #region Realtime
                tabRealSideRReal[i].Text = RoundingEdge(_dataRealExtnSideRLoad[i + tabIndex], 0.10f).ToString();
                if (_dataRealExtnSideRLoad[i + tabIndex] > _dataRealExtnSideRUpper[i + tabIndex] || _dataRealExtnSideRLoad[i + tabIndex] < _dataRealExtnSideRLower[i + tabIndex])
                {
                    tabRealSideRReal[i].BackColor = Color.Red;
                    tabRealSideRReal[i].ForeColor = Color.Ivory;
                }
                else
                {
                    tabRealSideRReal[i].BackColor = Color.White;
                    tabRealSideRReal[i].ForeColor = Color.Black;
                }
                #endregion

                #region Upper
                tabRealSideRUpper[i].Text = RoundingEdge(_dataRealExtnSideRUpper[i + tabIndex], 0.10f).ToString();
                #endregion
            }
            ActiveRealTableRightData = 2;
        }
        public void tabdataRealDiffStep2R()
        {
            int tabIndex = tabIdxRealSideR * 40;
            for (int i = 0; i < 40; i++)
            {
                #region Stroke
                tabRealSideRStroke[i].Text = _dataRealSideRDiffStroke[i + tabIndex].ToString();
                #endregion

                #region Master 
                tabRealSideRMaster[i].Text = RoundingEdge(_dataRealSideRDiffMaster[i + tabIndex], 0.10f).ToString();
                #endregion

                #region Lower
                tabRealSideRLower[i].Text = RoundingEdge(_dataRealSideRDiffLower[i + tabIndex], 0.10f).ToString();
                #endregion

                #region Realtime
                tabRealSideRReal[i].Text = RoundingEdge(_dataRealSideRDiffLoad[i + tabIndex], 0.10f).ToString();
                if (_dataRealSideRDiffLoad[i + tabIndex] > _dataRealSideRDiffUpper[i + tabIndex] || _dataRealSideRDiffLoad[i + tabIndex] < _dataRealSideRDiffLower[i + tabIndex])
                {
                    tabRealSideRReal[i].BackColor = Color.Red;
                    tabRealSideRReal[i].ForeColor = Color.Ivory;
                }
                else
                {
                    tabRealSideRReal[i].BackColor = Color.White;
                    tabRealSideRReal[i].ForeColor = Color.Black;
                }
                #endregion

                #region Upper
                tabRealSideRUpper[i].Text = RoundingEdge(_dataRealSideRDiffUpper[i + tabIndex], 0.10f).ToString();
                #endregion
            }
            ActiveRealTableRightData = 3;
        }
        #endregion

        #region Master Data

        public void tabdataMasterSideInit()
        {
            tabIdxMasterSideL = 0;
            tabIdxMasterSideR = 0;
            ActiveMasterTableLeftData = 0;
            ActiveMasterTableRightData = 0;
        }
        public void tabdataMasterCompStep2L()
        {
            int tabIndex = tabIdxMasterSideL * 40;
            for (int i = 0; i < 40; i++)
            {
                #region Stroke
                tabMasterSideLStroke[i].Text = _dataMasterCompSideLStroke[i + tabIndex].ToString();
                #endregion

                #region Master 
                tabMasterSideLMaster[i].Text = RoundingEdge(_dataMasterCompSideLMaster[i + tabIndex], 0.10f).ToString();
                #endregion

                #region Master Acc
                tabMasterSideLAccMaster[i].Text = RoundingEdge(_dataMasterCompSideLAccMaster[i + tabIndex], 0.10f).ToString();
                #endregion

                #region Lower
                tabMasterSideLLower[i].Text = RoundingEdge(_dataMasterCompSideLMaster[i + tabIndex] - _dataMasterCompSideLLower[i + tabIndex], 0.10f).ToString();
                #endregion

                #region Upper
                tabMasterSideLUpper[i].Text = RoundingEdge(_dataMasterCompSideLUpper[i + tabIndex] - _dataMasterCompSideLMaster[i + tabIndex], 0.10f).ToString();
                #endregion
            }
            ActiveMasterTableLeftData = 1;
        }
        public void tabdataMasterExtnStep2L()
        {
            int tabIndex = tabIdxMasterSideL * 40;
            for (int i = 0; i < 40; i++)
            {
                #region Stroke
                tabMasterSideLStroke[i].Text = _dataMasterExtnSideLStroke[i + tabIndex].ToString();
                #endregion

                #region Master 
                tabMasterSideLMaster[i].Text = RoundingEdge(_dataMasterExtnSideLMaster[i + tabIndex], 0.10f).ToString();
                #endregion

                #region Master Acc
                tabMasterSideLAccMaster[i].Text = RoundingEdge(_dataMasterExtnSideLAccMaster[i + tabIndex], 0.10f).ToString();
                #endregion

                #region Lower
                tabMasterSideLLower[i].Text = RoundingEdge(_dataMasterExtnSideLMaster[i + tabIndex] - _dataMasterExtnSideLLower[i + tabIndex], 0.10f).ToString();
                #endregion

                #region Upper
                tabMasterSideLUpper[i].Text = RoundingEdge(_dataMasterExtnSideLUpper[i + tabIndex] - _dataMasterExtnSideLMaster[i + tabIndex], 0.10f).ToString();
                #endregion
            }
            ActiveMasterTableLeftData = 2;
        }
        public void tabdataMasterDiffStep2L()
        {
            int tabIndex = tabIdxMasterSideL * 40;
            for (int i = 0; i < 40; i++)
            {
                #region Stroke
                tabMasterSideLStroke[i].Text = _dataMasterSideLDiffStroke[i + tabIndex].ToString();
                #endregion

                #region Master 
                tabMasterSideLMaster[i].Text = RoundingEdge(_dataMasterSideLDiffMaster[i + tabIndex], 0.10f).ToString();
                #endregion

                #region Master Acc
                tabMasterSideLAccMaster[i].Text = RoundingEdge(_dataMasterSideLDiffAccMaster[i + tabIndex], 0.10f).ToString();
                #endregion

                #region Lower
                tabMasterSideLLower[i].Text = RoundingEdge(_dataMasterSideLDiffMaster[i + tabIndex] - _dataMasterSideLDiffLower[i + tabIndex], 0.10f).ToString();
                #endregion

                #region Upper
                tabMasterSideLUpper[i].Text = RoundingEdge(_dataMasterSideLDiffUpper[i + tabIndex] - _dataMasterSideLDiffMaster[i + tabIndex], 0.10f).ToString();
                #endregion
            }
            ActiveMasterTableLeftData = 3;
        }
        public void tabdataMasterCompStep2R()
        {
            int tabIndex = tabIdxMasterSideR * 40;
            for (int i = 0; i < 40; i++)
            {
                #region Stroke
                tabMasterSideRStroke[i].Text = _dataMasterCompSideRStroke[i + tabIndex].ToString();
                #endregion

                #region Master 
                tabMasterSideRMaster[i].Text = RoundingEdge(_dataMasterCompSideRMaster[i + tabIndex], 0.10f).ToString();
                #endregion

                #region Master Acc
                tabMasterSideRAccMaster[i].Text = RoundingEdge(_dataMasterCompSideRAccMaster[i + tabIndex], 0.10f).ToString();
                #endregion

                #region Lower
                tabMasterSideRLower[i].Text = RoundingEdge(_dataMasterCompSideRMaster[i + tabIndex] - _dataMasterCompSideRLower[i + tabIndex], 0.10f).ToString();
                #endregion

                #region Upper
                tabMasterSideRUpper[i].Text = RoundingEdge(_dataMasterCompSideRUpper[i + tabIndex] - _dataMasterCompSideRMaster[i + tabIndex], 0.10f).ToString();
                #endregion
            }
            ActiveMasterTableRightData = 1;
        }
        public void tabdataMasterExtnStep2R()
        {
            int tabIndex = tabIdxMasterSideR * 40;
            for (int i = 0; i < 40; i++)
            {
                #region Stroke
                tabMasterSideRStroke[i].Text = _dataMasterExtnSideRStroke[i + tabIndex].ToString();
                #endregion

                #region Master 
                tabMasterSideRMaster[i].Text = RoundingEdge(_dataMasterExtnSideRMaster[i + tabIndex], 0.10f).ToString();
                #endregion

                #region Master Acc
                tabMasterSideRAccMaster[i].Text = RoundingEdge(_dataMasterExtnSideRAccMaster[i + tabIndex], 0.10f).ToString();
                #endregion

                #region Lower
                tabMasterSideRLower[i].Text = RoundingEdge(_dataMasterExtnSideRMaster[i + tabIndex] - _dataMasterExtnSideRLower[i + tabIndex], 0.10f).ToString();
                #endregion

                #region Upper
                tabMasterSideRUpper[i].Text = RoundingEdge(_dataMasterExtnSideRUpper[i + tabIndex] - _dataMasterExtnSideRMaster[i + tabIndex], 0.10f).ToString();
                #endregion
            }
            ActiveMasterTableRightData = 2;
        }
        public void tabdataMasterDiffStep2R()
        {
            int tabIndex = tabIdxMasterSideR * 40;
            for (int i = 0; i < 40; i++)
            {
                #region Stroke
                tabMasterSideRStroke[i].Text = _dataMasterSideRDiffStroke[i + tabIndex].ToString();
                #endregion

                #region Master 
                tabMasterSideRMaster[i].Text = RoundingEdge(_dataMasterSideRDiffMaster[i + tabIndex], 0.10f).ToString();
                #endregion

                #region Master Acc
                tabMasterSideRAccMaster[i].Text = RoundingEdge(_dataMasterSideRDiffAccMaster[i + tabIndex], 0.10f).ToString();
                #endregion

                #region Lower
                tabMasterSideRLower[i].Text = RoundingEdge(_dataMasterSideRDiffMaster[i + tabIndex] - _dataMasterSideRDiffLower[i + tabIndex], 0.10f).ToString();
                #endregion

                #region Upper
                tabMasterSideRUpper[i].Text = RoundingEdge(_dataMasterSideRDiffUpper[i + tabIndex] - _dataMasterSideRDiffMaster[i + tabIndex], 0.10f).ToString();
                #endregion
            }
            ActiveMasterTableRightData = 3;
        }

        public void tabdataMasterCompStep2LW()
        {
            int tabIndex = tabIdxMasterSideL * 40;
            for (int i = 0; i < 40; i++)
            {
                #region Stroke
                float.TryParse(tabMasterSideLStroke[i].Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float num1);
                _dataMasterCompSideLStroke[i + tabIndex] = num1;
                #endregion

                #region Master
                float.TryParse(tabMasterSideLMaster[i].Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float num2);
                _dataMasterCompSideLMaster[i + tabIndex] = RoundingEdge(num2, 0.10f);
                #endregion

                #region Master Acc
                float.TryParse(tabMasterSideLAccMaster[i].Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float num3);
                _dataMasterCompSideLAccMaster[i + tabIndex] = RoundingEdge(num3, 0.10f);
                #endregion

                #region Lower
                float.TryParse(tabMasterSideLLower[i].Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float num4);
                _dataMasterCompSideLLower[i + tabIndex] = RoundingEdge((num2 - num4), 0.10f);
                #endregion

                #region Upper
                float.TryParse(tabMasterSideLUpper[i].Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float num5);
                _dataMasterCompSideLUpper[i + tabIndex] = RoundingEdge((num2 + num5), 0.10f);
                #endregion
            }
        }
        public void tabdataMasterExtnStep2LW()
        {
            int tabIndex = tabIdxMasterSideL * 40;
            for (int i = 0; i < 40; i++)
            {
                #region Stroke
                float.TryParse(tabMasterSideLStroke[i].Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float num1);
                _dataMasterExtnSideLStroke[i + tabIndex] = num1;
                #endregion

                #region Master
                float.TryParse(tabMasterSideLMaster[i].Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float num2);
                _dataMasterExtnSideLMaster[i + tabIndex] = RoundingEdge(num2, 0.10f);
                #endregion

                #region Master Acc
                float.TryParse(tabMasterSideLAccMaster[i].Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float num3);
                _dataMasterExtnSideLAccMaster[i + tabIndex] = RoundingEdge(num3, 0.10f);
                #endregion

                #region Lower
                float.TryParse(tabMasterSideLLower[i].Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float num4);
                _dataMasterExtnSideLLower[i + tabIndex] = RoundingEdge((num2 - num4), 0.10f);
                #endregion

                #region Upper
                float.TryParse(tabMasterSideLUpper[i].Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float num5);
                _dataMasterExtnSideLUpper[i + tabIndex] = RoundingEdge((num2 + num5), 0.10f);
                #endregion
            }
        }
        public void tabdataMasterDiffStep2LW()
        {
            int tabIndex = tabIdxMasterSideL * 40;
            for (int i = 0; i < 40; i++)
            {
                #region Stroke
                float.TryParse(tabMasterSideLStroke[i].Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float num1);
                _dataMasterSideLDiffStroke[i + tabIndex] = num1;
                #endregion

                #region Master
                float.TryParse(tabMasterSideLMaster[i].Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float num2);
                _dataMasterSideLDiffMaster[i + tabIndex] = RoundingEdge(num2, 0.10f);
                #endregion

                #region Master Acc
                float.TryParse(tabMasterSideLAccMaster[i].Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float num3);
                _dataMasterSideLDiffAccMaster[i + tabIndex] = RoundingEdge(num3, 0.10f);
                #endregion

                #region Lower
                float.TryParse(tabMasterSideLLower[i].Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float num4);
                _dataMasterSideLDiffLower[i + tabIndex] = RoundingEdge((num2 - num4), 0.10f);
                #endregion

                #region Upper
                float.TryParse(tabMasterSideLUpper[i].Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float num5);
                _dataMasterSideLDiffUpper[i + tabIndex] = RoundingEdge((num2 + num5), 0.10f);
                #endregion
            }
        }
        public void tabdataMasterCompStep2RW()
        {
            int tabIndex = tabIdxMasterSideR * 40;
            for (int i = 0; i < 40; i++)
            {
                #region Stroke
                float.TryParse(tabMasterSideRStroke[i].Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float num1);
                _dataMasterCompSideRStroke[i + tabIndex] = num1;
                #endregion

                #region Master
                float.TryParse(tabMasterSideRMaster[i].Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float num2);
                _dataMasterCompSideRMaster[i + tabIndex] = RoundingEdge(num2, 0.10f);
                #endregion

                #region Lower
                float.TryParse(tabMasterSideRAccMaster[i].Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float num3);
                _dataMasterCompSideRAccMaster[i + tabIndex] = RoundingEdge(num3, 0.10f);
                #endregion

                #region Realtime
                float.TryParse(tabMasterSideRLower[i].Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float num4);
                _dataMasterCompSideRLower[i + tabIndex] = RoundingEdge((num2 - num4), 0.10f);
                #endregion

                #region Upper
                float.TryParse(tabMasterSideRUpper[i].Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float num5);
                _dataMasterCompSideRUpper[i + tabIndex] = RoundingEdge((num2 + num5), 0.10f);
                #endregion
            }
        }
        public void tabdataMasterExtnStep2RW()
        {
            int tabIndex = tabIdxMasterSideR * 40;
            for (int i = 0; i < 40; i++)
            {
                #region Stroke
                float.TryParse(tabMasterSideRStroke[i].Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float num1);
                _dataMasterExtnSideRStroke[i + tabIndex] = num1;
                #endregion

                #region Master
                float.TryParse(tabMasterSideRMaster[i].Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float num2);
                _dataMasterExtnSideRMaster[i + tabIndex] = RoundingEdge(num2, 0.10f);
                #endregion

                #region Lower
                float.TryParse(tabMasterSideRAccMaster[i].Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float num3);
                _dataMasterExtnSideRAccMaster[i + tabIndex] = RoundingEdge(num3, 0.10f);
                #endregion

                #region Realtime
                float.TryParse(tabMasterSideRLower[i].Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float num4);
                _dataMasterExtnSideRLower[i + tabIndex] = RoundingEdge((num2 - num4), 0.10f);
                #endregion

                #region Upper
                float.TryParse(tabMasterSideRUpper[i].Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float num5);
                _dataMasterExtnSideRUpper[i + tabIndex] = RoundingEdge((num2 + num5), 0.10f);
                #endregion
            }
        }
        public void tabdataMasterDiffStep2RW()
        {
            int tabIndex = tabIdxMasterSideR * 40;
            for (int i = 0; i < 40; i++)
            {
                #region Stroke
                float.TryParse(tabMasterSideRStroke[i].Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float num1);
                _dataMasterSideRDiffStroke[i + tabIndex] = num1;
                #endregion

                #region Master
                float.TryParse(tabMasterSideRMaster[i].Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float num2);
                _dataMasterSideRDiffMaster[i + tabIndex] = RoundingEdge(num2, 0.10f);
                #endregion

                #region Master Acc
                float.TryParse(tabMasterSideRAccMaster[i].Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float num3);
                _dataMasterSideRDiffAccMaster[i + tabIndex] = RoundingEdge(num3, 0.10f);
                #endregion

                #region Lower
                float.TryParse(tabMasterSideRLower[i].Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float num4);
                _dataMasterSideRDiffLower[i + tabIndex] = RoundingEdge((num2 - num4), 0.10f);
                #endregion

                #region Upper
                float.TryParse(tabMasterSideRUpper[i].Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float num5);
                _dataMasterSideRDiffUpper[i + tabIndex] = RoundingEdge((num2 + num5), 0.10f);
                #endregion
            }
        }


        public void tabdataMasterTeachSet()
        {
            _dataMasterCompSideLAccMaster.ForEach((item, index) => _dataMasterCompSideLMaster[index] = item);
            tabdataSetMasterTeachLower(ref _dataMasterCompSideLMaster, ref _dataMasterCompSideLLower, MasterTeachOffsetBatchL);
            tabdataSetMasterTeachUpper(ref _dataMasterCompSideLMaster, ref _dataMasterCompSideLUpper, MasterTeachOffsetBatchL);
            _dataMasterExtnSideLAccMaster.ForEach((item, index) => _dataMasterExtnSideLMaster[index] = item);
            tabdataSetMasterTeachLower(ref _dataMasterExtnSideLMaster, ref _dataMasterExtnSideLLower, MasterTeachOffsetBatchL);
            tabdataSetMasterTeachUpper(ref _dataMasterExtnSideLMaster, ref _dataMasterExtnSideLUpper, MasterTeachOffsetBatchL);
            _dataMasterSideLDiffAccMaster.ForEach((item, index) => _dataMasterSideLDiffMaster[index] = item);
            tabdataSetMasterTeachLower(ref _dataMasterSideLDiffMaster, ref _dataMasterSideLDiffLower, MasterTeachDiffOffsetBatchL);
            tabdataSetMasterTeachUpper(ref _dataMasterSideLDiffMaster, ref _dataMasterSideLDiffUpper, MasterTeachDiffOffsetBatchL);

            _dataMasterCompSideRAccMaster.ForEach((item, index) => _dataMasterCompSideRMaster[index] = item);
            tabdataSetMasterTeachLower(ref _dataMasterCompSideRMaster, ref _dataMasterCompSideRLower, MasterTeachOffsetBatchR);
            tabdataSetMasterTeachUpper(ref _dataMasterCompSideRMaster, ref _dataMasterCompSideRUpper, MasterTeachOffsetBatchR);
            _dataMasterExtnSideRAccMaster.ForEach((item, index) => _dataMasterExtnSideRMaster[index] = item);
            tabdataSetMasterTeachLower(ref _dataMasterExtnSideRMaster, ref _dataMasterExtnSideRLower, MasterTeachOffsetBatchR);
            tabdataSetMasterTeachUpper(ref _dataMasterExtnSideRMaster, ref _dataMasterExtnSideRUpper, MasterTeachOffsetBatchR);
            _dataMasterSideRDiffAccMaster.ForEach((item, index) => _dataMasterSideRDiffMaster[index] = item);
            tabdataSetMasterTeachLower(ref _dataMasterSideRDiffMaster, ref _dataMasterSideRDiffLower, MasterTeachDiffOffsetBatchR);
            tabdataSetMasterTeachUpper(ref _dataMasterSideRDiffMaster, ref _dataMasterSideRDiffUpper, MasterTeachDiffOffsetBatchR);
        }
        void tabdataSetMasterTeachLower(ref float[] master, ref float[] lower, float batchoffset)
        {
            float[] lobuff = new float[lower.Length];
            master.ForEach((item, index) => lobuff[index] = item - batchoffset);
            Array.Copy(lobuff, lower, lobuff.Length);
        }
        void tabdataSetMasterTeachUpper(ref float[] master, ref float[] upper, float batchoffset)
        {
            float[] hibuff = new float[upper.Length];
            master.ForEach((item, index) => hibuff[index] = item + batchoffset);
            Array.Copy(hibuff, upper, hibuff.Length);
        }

        #endregion

        #endregion

        #region FormComponents

        public Form1()
        {
            //LoadCustomFont("IBMPlexSans-VariableFont.ttf");
            //LoadCustomFont("WorkSans-VariableFont.ttf");
            //LoadCustomFont("SarasaGothicJ-Regular.ttf");
            //LoadCustomFont("SarasaFixedJ-Regular.ttf");
            //LoadCustomFont("SarasaMonoJ-Regular.ttf");
            //LoadCustomFont("SarasaTermJ-Regular.ttf");

            saveFileDialog = new SaveFileDialog();
            openFileDialog = new OpenFileDialog();
            folderBrowserDialog = new FolderBrowserDialog();

            InitializeComponent();
            InitializePlotStyle();
            InitializeCustomComponents();
            InitializeBorderComponent();
            InitializeUI();

            drawingBorderUpper.BringToFront();
            drawingBorderLower.BringToFront();
            drawingBorderLeft.BringToFront();
            drawingBorderRight.BringToFront();
            drawingPanel.BringToFront();


            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            _WorkflowHandler = new WORKFLOWHANDLER(this);
            _WorkflowHandler._kvMasterConfirm();
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
            { } //init connection
        }
        private void InitializePlotStyle()
        {
            formsPlot1.Plot.XLabel("Stroke");
            formsPlot1.Plot.YLabel("Load");
            formsPlot1.Plot.Axes.Bottom.Label.FontSize = 12;
            formsPlot1.Plot.Axes.Left.Label.FontSize = 12;
            PixelPadding pad1 = new(60, 1, 60, 10);
            formsPlot1.Plot.Layout.Fixed(pad1);




            formsPlot1.Plot.HideLegend();
            formsPlot1.Plot.Axes.AntiAlias(true);
            formsPlot1.Plot.Axes.Hairline(true);
            formsPlot1.Plot.Axes.AutoScale(true);
            formsPlot1.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#708090");
            formsPlot1.Plot.DataBackground.Color = ScottPlot.Color.FromHex("#343c43");
            formsPlot1.Plot.Axes.Color(ScottPlot.Color.FromColor(System.Drawing.Color.Ivory));
            formsPlot1.Plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#5a6773");
            formsPlot1.Plot.Legend.BackgroundColor = ScottPlot.Color.FromHex("#708090");
            formsPlot1.Plot.Legend.FontColor = ScottPlot.Color.FromColor(System.Drawing.Color.Ivory);
            formsPlot1.Plot.Legend.OutlineColor = ScottPlot.Color.FromHex("#5a6773");
            formsPlot1.Plot.Legend.FontSize = 8;
            formsPlot1.Plot.Legend.SymbolWidth = 3;
            formsPlot1.Plot.Legend.OutlineWidth = 3;
            formsPlot1.Plot.Legend.Padding = new PixelPadding(1, 1);
            JudgePlot1 = formsPlot1.Plot.Add.Text("", 0, 0);
            JudgePlot1.LabelFontSize = 12;
            JudgePlot1.LabelBold = true;
            JudgePlot1.LabelBackgroundColor = Colors.LimeGreen;
            JudgePlot1.LabelFontColor = Colors.Black;
            JudgePlot1.LabelBorderColor = Colors.White;
            JudgePlot1.LabelBorderWidth = 1;
            JudgePlot1.LabelPadding = 2;
            //JudgePlot1.IsVisible = false;
            //JudgePlot1.OffsetY = -10f * 5f;
            //JudgePlot1.LabelText = "NG";
            //JudgePlot1.LabelFontColor = Colors.White;
            //JudgePlot1.LabelBackgroundColor = Colors.Crimson;


            //formsPlot1.MouseDown += OnTouchDown;
            //formsPlot1.MouseMove += OnTouchMove;
            //formsPlot1.MouseUp += OnTouchUp;


            formsPlot2.Plot.XLabel("Stroke");
            formsPlot2.Plot.YLabel("Load");
            formsPlot2.Plot.Axes.Bottom.Label.FontSize = 12;
            formsPlot2.Plot.Axes.Left.Label.FontSize = 12;
            PixelPadding pad2 = new(60, 1, 60, 10);
            formsPlot2.Plot.Layout.Fixed(pad2);
            //formsPlot2.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.MouseWheelZoom>();
            //formsPlot2.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.MouseDragZoom>();
            //formsPlot2.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.MouseDragZoomRectangle>();
            //formsPlot2.Plot.ShowLegend(Edge.Right);
            formsPlot2.Plot.HideLegend();
            formsPlot2.Plot.Axes.AntiAlias(true);
            formsPlot2.Plot.Axes.Hairline(true);
            formsPlot2.Plot.Axes.AutoScale(true);
            formsPlot2.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#708090");
            formsPlot2.Plot.DataBackground.Color = ScottPlot.Color.FromHex("#343c43");
            formsPlot2.Plot.Axes.Color(ScottPlot.Color.FromColor(System.Drawing.Color.Ivory));
            formsPlot2.Plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#5a6773");
            formsPlot2.Plot.Legend.BackgroundColor = ScottPlot.Color.FromHex("#708090");
            formsPlot2.Plot.Legend.FontColor = ScottPlot.Color.FromColor(System.Drawing.Color.Ivory);
            formsPlot2.Plot.Legend.OutlineColor = ScottPlot.Color.FromHex("#5a6773");
            formsPlot2.Plot.Legend.FontSize = 8;
            formsPlot2.Plot.Legend.SymbolWidth = 3;
            formsPlot2.Plot.Legend.OutlineWidth = 3;
            formsPlot2.Plot.Legend.Padding = new PixelPadding(1, 1);
            JudgePlot2 = formsPlot2.Plot.Add.Text("", 0, 0);
            JudgePlot2.LabelFontSize = 12;
            JudgePlot2.LabelBold = true;
            JudgePlot2.LabelBackgroundColor = Colors.LimeGreen;
            JudgePlot2.LabelFontColor = Colors.Black;
            JudgePlot2.LabelBorderColor = Colors.White;
            JudgePlot2.LabelBorderWidth = 1;
            JudgePlot2.LabelPadding = 2;
            //formsPlot2.MouseDown += OnTouchDown;
            //formsPlot2.MouseMove += OnTouchMove;
            //formsPlot2.MouseUp += OnTouchUp;

            var sig3 = formsPlot3.Plot.Add.SignalXY(new float[] { 0.0f }, new float[] { 0.0f });
            sig3.Axes.YAxis = formsPlot3.Plot.Axes.Right;
            formsPlot3.Plot.XLabel("Stroke");
            formsPlot3.Plot.YLabel("Load");
            formsPlot3.Plot.Axes.Bottom.Label.FontSize = 12;
            formsPlot3.Plot.Axes.Left.Label.FontSize = 12;
            // Hide axis label and tick
            formsPlot3.Plot.Axes.Left.TickLabelStyle.IsVisible = false;
            formsPlot3.Plot.Axes.Left.Label.IsVisible = false;
            formsPlot3.Plot.Axes.Left.FrameLineStyle.Width = 0;
            formsPlot3.Plot.Axes.Left.MajorTickStyle.Length = 0;
            formsPlot3.Plot.Axes.Left.MinorTickStyle.Length = 0;
            //formsPlot3.Plot.Axes.AddRightAxis();
            formsPlot3.Plot.Axes.Right.Label.Text = "Load";
            formsPlot3.Plot.Axes.Right.Label.FontSize = 12;
            PixelPadding pad3 = new(1, 60, 60, 10);
            formsPlot3.Plot.Layout.Fixed(pad3);
            //formsPlot3.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.MouseWheelZoom>();
            //formsPlot3.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.MouseDragZoom>();
            //formsPlot3.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.MouseDragZoomRectangle>();
            //formsPlot3.Plot.ShowLegend(Edge.Left);
            formsPlot3.Plot.HideLegend();
            formsPlot3.Plot.Axes.AntiAlias(true);
            formsPlot3.Plot.Axes.Hairline(true);
            formsPlot3.Plot.Axes.AutoScale(true);
            formsPlot3.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#708090");
            formsPlot3.Plot.DataBackground.Color = ScottPlot.Color.FromHex("#343c43");
            formsPlot3.Plot.Axes.Color(ScottPlot.Color.FromColor(System.Drawing.Color.Ivory));
            formsPlot3.Plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#5a6773");
            formsPlot3.Plot.Legend.BackgroundColor = ScottPlot.Color.FromHex("#708090");
            formsPlot3.Plot.Legend.FontColor = ScottPlot.Color.FromColor(System.Drawing.Color.Ivory);
            formsPlot3.Plot.Legend.OutlineColor = ScottPlot.Color.FromHex("#5a6773");
            formsPlot3.Plot.Legend.FontSize = 8;
            formsPlot3.Plot.Legend.SymbolWidth = 3;
            formsPlot3.Plot.Legend.OutlineWidth = 3;
            formsPlot3.Plot.Legend.Padding = new PixelPadding(1, 1);
            JudgePlot3 = formsPlot3.Plot.Add.Text("", 0, 0);
            JudgePlot3.LabelFontSize = 12;
            JudgePlot3.LabelBold = true;
            JudgePlot3.LabelBackgroundColor = Colors.LimeGreen;
            JudgePlot3.LabelFontColor = Colors.Black;
            JudgePlot3.LabelBorderColor = Colors.White;
            JudgePlot3.LabelBorderWidth = 1;
            JudgePlot3.LabelPadding = 2;
            //formsPlot3.MouseDown += OnTouchDown;
            //formsPlot3.MouseMove += OnTouchMove;
            //formsPlot3.MouseUp += OnTouchUp;

            var sig4 = formsPlot4.Plot.Add.SignalXY(new float[] { 0.0f }, new float[] { 0.0f });
            sig4.Axes.YAxis = formsPlot4.Plot.Axes.Right;
            formsPlot4.Plot.XLabel("Stroke");
            formsPlot4.Plot.YLabel("Load");
            formsPlot4.Plot.Axes.Bottom.Label.FontSize = 12;
            formsPlot4.Plot.Axes.Left.Label.FontSize = 12;
            // Hide axis label and tick
            formsPlot4.Plot.Axes.Left.TickLabelStyle.IsVisible = false;
            formsPlot4.Plot.Axes.Left.Label.IsVisible = false;
            formsPlot4.Plot.Axes.Left.FrameLineStyle.Width = 0;
            formsPlot4.Plot.Axes.Left.MajorTickStyle.Length = 0;
            formsPlot4.Plot.Axes.Left.MinorTickStyle.Length = 0;
            //formsPlot4.Plot.Axes.AddRightAxis();
            formsPlot4.Plot.Axes.Right.Label.Text = "Load";
            formsPlot4.Plot.Axes.Right.Label.FontSize = 12;
            PixelPadding pad4 = new(1, 60, 60, 10);
            formsPlot4.Plot.Layout.Fixed(pad4);
            //formsPlot4.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.MouseWheelZoom>();
            //formsPlot4.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.MouseDragZoom>();
            //formsPlot4.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.MouseDragZoomRectangle>();
            //formsPlot4.Plot.ShowLegend(Edge.Left);
            formsPlot4.Plot.HideLegend();
            formsPlot4.Plot.Axes.AntiAlias(true);
            formsPlot4.Plot.Axes.Hairline(true);
            formsPlot4.Plot.Axes.AutoScale(true);
            formsPlot4.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#708090");
            formsPlot4.Plot.DataBackground.Color = ScottPlot.Color.FromHex("#343c43");
            formsPlot4.Plot.Axes.Color(ScottPlot.Color.FromColor(System.Drawing.Color.Ivory));
            formsPlot4.Plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#5a6773");
            formsPlot4.Plot.Legend.BackgroundColor = ScottPlot.Color.FromHex("#708090");
            formsPlot4.Plot.Legend.FontColor = ScottPlot.Color.FromColor(System.Drawing.Color.Ivory);
            formsPlot4.Plot.Legend.OutlineColor = ScottPlot.Color.FromHex("#5a6773");
            formsPlot4.Plot.Legend.FontSize = 8;
            formsPlot4.Plot.Legend.SymbolWidth = 3;
            formsPlot4.Plot.Legend.OutlineWidth = 3;
            formsPlot4.Plot.Legend.Padding = new PixelPadding(1, 1);
            JudgePlot4 = formsPlot4.Plot.Add.Text("", 0, 0);
            JudgePlot4.LabelFontSize = 12;
            JudgePlot4.LabelBold = true;
            JudgePlot4.LabelBackgroundColor = Colors.LimeGreen;
            JudgePlot4.LabelFontColor = Colors.Black;
            JudgePlot4.LabelBorderColor = Colors.White;
            JudgePlot4.LabelBorderWidth = 1;
            JudgePlot4.LabelPadding = 2;
            //formsPlot4.MouseDown += OnTouchDown;
            //formsPlot4.MouseMove += OnTouchMove;
            //formsPlot4.MouseUp += OnTouchUp;

            formsPlot5.Plot.XLabel("Stroke");
            formsPlot5.Plot.YLabel("Load");
            formsPlot5.Plot.Axes.Bottom.Label.FontSize = 12;
            formsPlot5.Plot.Axes.Left.Label.FontSize = 12;
            PixelPadding pad5 = new(60, 1, 60, 10);
            formsPlot5.Plot.Layout.Fixed(pad5);
            //formsPlot5.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.MouseWheelZoom>();
            //formsPlot5.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.MouseDragZoom>();
            //formsPlot5.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.MouseDragZoomRectangle>();
            //formsPlot5.Plot.ShowLegend(Edge.Right);
            formsPlot5.Plot.HideLegend();
            formsPlot5.Plot.Axes.AntiAlias(true);
            formsPlot5.Plot.Axes.Hairline(true);
            formsPlot5.Plot.Axes.AutoScale(true);
            formsPlot5.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#708090");
            formsPlot5.Plot.DataBackground.Color = ScottPlot.Color.FromHex("#343c43");
            formsPlot5.Plot.Axes.Color(ScottPlot.Color.FromColor(System.Drawing.Color.Ivory));
            formsPlot5.Plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#5a6773");
            formsPlot5.Plot.Legend.BackgroundColor = ScottPlot.Color.FromHex("#708090");
            formsPlot5.Plot.Legend.FontColor = ScottPlot.Color.FromColor(System.Drawing.Color.Ivory);
            formsPlot5.Plot.Legend.OutlineColor = ScottPlot.Color.FromHex("#5a6773");
            formsPlot5.Plot.Legend.FontSize = 8;
            formsPlot5.Plot.Legend.SymbolWidth = 3;
            formsPlot5.Plot.Legend.OutlineWidth = 3;
            formsPlot5.Plot.Legend.Padding = new PixelPadding(1, 1);
            //formsPlot5.MouseDown += OnTouchDown;
            //formsPlot5.MouseMove += OnTouchMove;
            //formsPlot5.MouseUp += OnTouchUp;

            formsPlot6.Plot.XLabel("Stroke");
            formsPlot6.Plot.YLabel("Load");
            formsPlot6.Plot.Axes.Bottom.Label.FontSize = 12;
            formsPlot6.Plot.Axes.Left.Label.FontSize = 12;
            PixelPadding pad6 = new(60, 1, 60, 10);
            formsPlot6.Plot.Layout.Fixed(pad6);
            //formsPlot6.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.MouseWheelZoom>();
            //formsPlot6.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.MouseDragZoom>();
            //formsPlot6.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.MouseDragZoomRectangle>();
            //formsPlot6.Plot.ShowLegend(Edge.Right);
            formsPlot6.Plot.HideLegend();
            formsPlot6.Plot.Axes.AntiAlias(true);
            formsPlot6.Plot.Axes.Hairline(true);
            formsPlot6.Plot.Axes.AutoScale(true);
            formsPlot6.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#708090");
            formsPlot6.Plot.DataBackground.Color = ScottPlot.Color.FromHex("#343c43");
            formsPlot6.Plot.Axes.Color(ScottPlot.Color.FromColor(System.Drawing.Color.Ivory));
            formsPlot6.Plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#5a6773");
            formsPlot6.Plot.Legend.BackgroundColor = ScottPlot.Color.FromHex("#708090");
            formsPlot6.Plot.Legend.FontColor = ScottPlot.Color.FromColor(System.Drawing.Color.Ivory);
            formsPlot6.Plot.Legend.OutlineColor = ScottPlot.Color.FromHex("#5a6773");
            formsPlot6.Plot.Legend.FontSize = 8;
            formsPlot6.Plot.Legend.SymbolWidth = 3;
            formsPlot6.Plot.Legend.OutlineWidth = 3;
            formsPlot6.Plot.Legend.Padding = new PixelPadding(1, 1);
            //formsPlot6.MouseDown += OnTouchDown;
            //formsPlot6.MouseMove += OnTouchMove;
            //formsPlot6.MouseUp += OnTouchUp;

            var sig7 = formsPlot7.Plot.Add.SignalXY(new float[] { 0.0f }, new float[] { 0.0f });
            sig7.Axes.YAxis = formsPlot7.Plot.Axes.Right;
            formsPlot7.Plot.XLabel("Stroke");
            formsPlot7.Plot.YLabel("Load");
            formsPlot7.Plot.Axes.Bottom.Label.FontSize = 12;
            formsPlot7.Plot.Axes.Left.Label.FontSize = 12;
            // Hide axis label and tick
            formsPlot7.Plot.Axes.Left.TickLabelStyle.IsVisible = false;
            formsPlot7.Plot.Axes.Left.Label.IsVisible = false;
            formsPlot7.Plot.Axes.Left.FrameLineStyle.Width = 0;
            formsPlot7.Plot.Axes.Left.MajorTickStyle.Length = 0;
            formsPlot7.Plot.Axes.Left.MinorTickStyle.Length = 0;
            //formsPlot4.Plot.Axes.AddRightAxis();
            formsPlot7.Plot.Axes.Right.Label.Text = "Load";
            formsPlot7.Plot.Axes.Right.Label.FontSize = 12;
            PixelPadding pad7 = new(1, 60, 60, 10);
            formsPlot7.Plot.Layout.Fixed(pad7);
            //formsPlot7.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.MouseWheelZoom>();
            //formsPlot7.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.MouseDragZoom>();
            //formsPlot7.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.MouseDragZoomRectangle>();
            //formsPlot7.Plot.ShowLegend(Edge.Left);
            formsPlot7.Plot.HideLegend();
            formsPlot7.Plot.Axes.AntiAlias(true);
            formsPlot7.Plot.Axes.Hairline(true);
            formsPlot7.Plot.Axes.AutoScale(true);
            formsPlot7.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#708090");
            formsPlot7.Plot.DataBackground.Color = ScottPlot.Color.FromHex("#343c43");
            formsPlot7.Plot.Axes.Color(ScottPlot.Color.FromColor(System.Drawing.Color.Ivory));
            formsPlot7.Plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#5a6773");
            formsPlot7.Plot.Legend.BackgroundColor = ScottPlot.Color.FromHex("#708090");
            formsPlot7.Plot.Legend.FontColor = ScottPlot.Color.FromColor(System.Drawing.Color.Ivory);
            formsPlot7.Plot.Legend.OutlineColor = ScottPlot.Color.FromHex("#5a6773");
            formsPlot7.Plot.Legend.FontSize = 8;
            formsPlot7.Plot.Legend.SymbolWidth = 3;
            formsPlot7.Plot.Legend.OutlineWidth = 3;
            formsPlot7.Plot.Legend.Padding = new PixelPadding(1, 1);
            //formsPlot7.MouseDown += OnTouchDown;
            //formsPlot7.MouseMove += OnTouchMove;
            //formsPlot7.MouseUp += OnTouchUp;

            var sig8 = formsPlot8.Plot.Add.SignalXY(new float[] { 0.0f }, new float[] { 0.0f });
            sig8.Axes.YAxis = formsPlot8.Plot.Axes.Right;
            formsPlot8.Plot.XLabel("Stroke");
            formsPlot8.Plot.YLabel("Load");
            formsPlot8.Plot.Axes.Bottom.Label.FontSize = 12;
            formsPlot8.Plot.Axes.Left.Label.FontSize = 12;
            // Hide axis label and tick
            formsPlot8.Plot.Axes.Left.TickLabelStyle.IsVisible = false;
            formsPlot8.Plot.Axes.Left.Label.IsVisible = false;
            formsPlot8.Plot.Axes.Left.FrameLineStyle.Width = 0;
            formsPlot8.Plot.Axes.Left.MajorTickStyle.Length = 0;
            formsPlot8.Plot.Axes.Left.MinorTickStyle.Length = 0;
            //formsPlot4.Plot.Axes.AddRightAxis();
            formsPlot8.Plot.Axes.Right.Label.Text = "Load";
            formsPlot8.Plot.Axes.Right.Label.FontSize = 12;
            PixelPadding pad8 = new(1, 60, 60, 10);
            formsPlot8.Plot.Layout.Fixed(pad8);
            //formsPlot8.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.MouseWheelZoom>();
            //formsPlot8.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.MouseDragZoom>();
            //formsPlot8.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.MouseDragZoomRectangle>();
            //formsPlot8.Plot.ShowLegend(Edge.Left);
            formsPlot8.Plot.HideLegend();
            formsPlot8.Plot.Axes.AntiAlias(true);
            formsPlot8.Plot.Axes.Hairline(true);
            formsPlot8.Plot.Axes.AutoScale(true);
            formsPlot8.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#708090");
            formsPlot8.Plot.DataBackground.Color = ScottPlot.Color.FromHex("#343c43");
            formsPlot8.Plot.Axes.Color(ScottPlot.Color.FromColor(System.Drawing.Color.Ivory));
            formsPlot8.Plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#5a6773");
            formsPlot8.Plot.Legend.BackgroundColor = ScottPlot.Color.FromHex("#708090");
            formsPlot8.Plot.Legend.FontColor = ScottPlot.Color.FromColor(System.Drawing.Color.Ivory);
            formsPlot8.Plot.Legend.OutlineColor = ScottPlot.Color.FromHex("#5a6773");
            formsPlot8.Plot.Legend.FontSize = 8;
            formsPlot8.Plot.Legend.SymbolWidth = 3;
            formsPlot8.Plot.Legend.OutlineWidth = 3;
            formsPlot8.Plot.Legend.Padding = new PixelPadding(1, 1);
            //formsPlot8.MouseDown += OnTouchDown;
            //formsPlot8.MouseMove += OnTouchMove;
            //formsPlot8.MouseUp += OnTouchUp;

            formsPlot9.Plot.XLabel("Stroke");
            formsPlot9.Plot.YLabel("Load");
            formsPlot9.Plot.Axes.Bottom.Label.FontSize = 12;
            formsPlot9.Plot.Axes.Left.Label.FontSize = 12;
            formsPlot9.Plot.Axes.AntiAlias(true);
            formsPlot9.Plot.Axes.Hairline(true);
            formsPlot9.Plot.Axes.AutoScale(true);
            formsPlot9.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#708090");
            formsPlot9.Plot.DataBackground.Color = ScottPlot.Color.FromHex("#343c43");
            formsPlot9.Plot.Axes.Color(ScottPlot.Color.FromColor(System.Drawing.Color.Ivory));
            formsPlot9.Plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#5a6773");
            PixelPadding pad9 = new(60, 1, 60, 10);
            formsPlot9.Plot.Layout.Fixed(pad9);
            //formsPlot9.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.MouseWheelZoom>();
            //formsPlot9.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.MouseDragZoom>();
            //formsPlot9.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.MouseDragZoomRectangle>();
            formsPlot9.Plot.Legend.Orientation = ScottPlot.Orientation.Horizontal;
            formsPlot9.Plot.Legend.BackgroundColor = ScottPlot.Color.FromHex("#708090");
            formsPlot9.Plot.Legend.FontColor = ScottPlot.Color.FromColor(System.Drawing.Color.Ivory);
            formsPlot9.Plot.Legend.OutlineColor = ScottPlot.Color.FromHex("#5a6773");
            formsPlot9.Plot.Legend.FontSize = 9;
            formsPlot9.Plot.Legend.SymbolWidth = 5;
            formsPlot9.Plot.Legend.OutlineWidth = 5;
            formsPlot9.Plot.Legend.Padding = new PixelPadding(1, 1);
            ScottPlot.Panels.LegendPanel legendPanel9 = new(formsPlot9.Plot.Legend)
            {
                Edge = Edge.Bottom,
                Alignment = Alignment.UpperCenter,
            };
            formsPlot9.Plot.Axes.AddPanel(legendPanel9);
            formsPlot9.Plot.Legend.IsVisible = false;
            //formsPlot9.Plot.ShowLegend(Edge.Bottom);
            JudgePlot9 = formsPlot9.Plot.Add.Text("", 0, 0);
            JudgePlot9.LabelFontSize = 12;
            JudgePlot9.LabelBold = true;
            JudgePlot9.LabelBackgroundColor = Colors.LimeGreen;
            JudgePlot9.LabelFontColor = Colors.Black;
            JudgePlot9.LabelBorderColor = Colors.White;
            JudgePlot9.LabelBorderWidth = 1;
            JudgePlot9.LabelPadding = 2;
            //formsPlot9.MouseDown += OnTouchDown;
            //formsPlot9.MouseMove += OnTouchMove;
            //formsPlot9.MouseUp += OnTouchUp;

            var sig10 = formsPlot10.Plot.Add.SignalXY(new float[] { 0.0f }, new float[] { 0.0f });
            sig10.Axes.YAxis = formsPlot10.Plot.Axes.Right;
            formsPlot10.Plot.XLabel("Stroke");
            formsPlot10.Plot.YLabel("Load");
            formsPlot10.Plot.Axes.Bottom.Label.FontSize = 12;
            formsPlot10.Plot.Axes.Left.Label.FontSize = 12;
            // Hide axis label and tick
            formsPlot10.Plot.Axes.Left.TickLabelStyle.IsVisible = false;
            formsPlot10.Plot.Axes.Left.Label.IsVisible = false;
            formsPlot10.Plot.Axes.Left.FrameLineStyle.Width = 0;
            formsPlot10.Plot.Axes.Left.MajorTickStyle.Length = 0;
            formsPlot10.Plot.Axes.Left.MinorTickStyle.Length = 0;
            //formsPlot10.Plot.Axes.AddRightAxis();
            formsPlot10.Plot.Axes.Right.Label.Text = "Load";
            formsPlot10.Plot.Axes.Right.Label.FontSize = 12;
            formsPlot10.Plot.Axes.AntiAlias(true);
            formsPlot10.Plot.Axes.Hairline(true);
            formsPlot10.Plot.Axes.AutoScale(true);
            formsPlot10.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#708090");
            formsPlot10.Plot.DataBackground.Color = ScottPlot.Color.FromHex("#343c43");
            formsPlot10.Plot.Axes.Color(ScottPlot.Color.FromColor(System.Drawing.Color.Ivory));
            formsPlot10.Plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#5a6773");
            PixelPadding pad10 = new(1, 60, 60, 10);
            formsPlot10.Plot.Layout.Fixed(pad10);
            //formsPlot10.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.MouseWheelZoom>();
            //formsPlot10.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.MouseDragZoom>();
            //formsPlot10.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.MouseDragZoomRectangle>();
            formsPlot10.Plot.Legend.Orientation = ScottPlot.Orientation.Horizontal;
            formsPlot10.Plot.Legend.BackgroundColor = ScottPlot.Color.FromHex("#708090");
            formsPlot10.Plot.Legend.FontColor = ScottPlot.Color.FromColor(System.Drawing.Color.Ivory);
            formsPlot10.Plot.Legend.OutlineColor = ScottPlot.Color.FromHex("#5a6773");
            formsPlot10.Plot.Legend.FontSize = 9;
            formsPlot10.Plot.Legend.SymbolWidth = 5;
            formsPlot10.Plot.Legend.OutlineWidth = 5;
            formsPlot10.Plot.Legend.Padding = new PixelPadding(1, 1);
            ScottPlot.Panels.LegendPanel legendPanel10 = new(formsPlot10.Plot.Legend)
            {
                Edge = Edge.Bottom,
                Alignment = Alignment.UpperCenter,
            };
            formsPlot10.Plot.Axes.AddPanel(legendPanel10);
            formsPlot10.Plot.Legend.IsVisible = false;
            //formsPlot10.Plot.ShowLegend(Edge.Bottom);
            JudgePlot10 = formsPlot10.Plot.Add.Text("", 0, 0);
            JudgePlot10.LabelFontSize = 12;
            JudgePlot10.LabelBold = true;
            JudgePlot10.LabelBackgroundColor = Colors.LimeGreen;
            JudgePlot10.LabelFontColor = Colors.Black;
            JudgePlot10.LabelBorderColor = Colors.White;
            JudgePlot10.LabelBorderWidth = 1;
            JudgePlot10.LabelPadding = 2;
            //formsPlot10.MouseDown += OnTouchDown;
            //formsPlot10.MouseMove += OnTouchMove;
            //formsPlot10.MouseUp += OnTouchUp;

            formsPlot11.Plot.XLabel("Stroke");
            formsPlot11.Plot.YLabel("Load");
            formsPlot11.Plot.Axes.Bottom.Label.FontSize = 12;
            formsPlot11.Plot.Axes.Left.Label.FontSize = 12;
            formsPlot11.Plot.Axes.AntiAlias(true);
            formsPlot11.Plot.Axes.Hairline(true);
            formsPlot11.Plot.Axes.AutoScale(true);
            formsPlot11.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#708090");
            formsPlot11.Plot.DataBackground.Color = ScottPlot.Color.FromHex("#343c43");
            formsPlot11.Plot.Axes.Color(ScottPlot.Color.FromColor(System.Drawing.Color.Ivory));
            formsPlot11.Plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#5a6773");
            PixelPadding pad11 = new(60, 1, 60, 10);
            formsPlot11.Plot.Layout.Fixed(pad11);
            //formsPlot11.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.MouseWheelZoom>();
            //formsPlot11.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.MouseDragZoom>();
            //formsPlot11.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.MouseDragZoomRectangle>();
            formsPlot11.Plot.Legend.Orientation = ScottPlot.Orientation.Horizontal;
            formsPlot11.Plot.Legend.BackgroundColor = ScottPlot.Color.FromHex("#708090");
            formsPlot11.Plot.Legend.FontColor = ScottPlot.Color.FromColor(System.Drawing.Color.Ivory);
            formsPlot11.Plot.Legend.OutlineColor = ScottPlot.Color.FromHex("#5a6773");
            formsPlot11.Plot.Legend.FontSize = 9;
            formsPlot11.Plot.Legend.SymbolWidth = 5;
            formsPlot11.Plot.Legend.OutlineWidth = 5;
            formsPlot11.Plot.Legend.Padding = new PixelPadding(1, 1);
            ScottPlot.Panels.LegendPanel legendPanel11 = new(formsPlot11.Plot.Legend)
            {
                Edge = Edge.Bottom,
                Alignment = Alignment.UpperCenter,
            };
            formsPlot11.Plot.Axes.AddPanel(legendPanel11);
            formsPlot11.Plot.Legend.IsVisible = false;
            //formsPlot10.Plot.ShowLegend(Edge.Bottom);
            //formsPlot11.MouseDown += OnTouchDown;
            //formsPlot11.MouseMove += OnTouchMove;
            //formsPlot11.MouseUp += OnTouchUp;

            var sig12 = formsPlot12.Plot.Add.SignalXY(new float[] { 0.0f }, new float[] { 0.0f });
            sig12.Axes.YAxis = formsPlot12.Plot.Axes.Right;
            formsPlot12.Plot.XLabel("Stroke");
            formsPlot12.Plot.YLabel("Load");
            formsPlot12.Plot.Axes.Bottom.Label.FontSize = 12;
            formsPlot12.Plot.Axes.Left.Label.FontSize = 12;
            // Hide axis label and tick
            formsPlot12.Plot.Axes.Left.TickLabelStyle.IsVisible = false;
            formsPlot12.Plot.Axes.Left.Label.IsVisible = false;
            formsPlot12.Plot.Axes.Left.FrameLineStyle.Width = 0;
            formsPlot12.Plot.Axes.Left.MajorTickStyle.Length = 0;
            formsPlot12.Plot.Axes.Left.MinorTickStyle.Length = 0;
            //formsPlot10.Plot.Axes.AddRightAxis();
            formsPlot12.Plot.Axes.Right.Label.Text = "Load";
            formsPlot12.Plot.Axes.Right.Label.FontSize = 12;
            formsPlot12.Plot.Axes.AntiAlias(true);
            formsPlot12.Plot.Axes.Hairline(true);
            formsPlot12.Plot.Axes.AutoScale(true);
            formsPlot12.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#708090");
            formsPlot12.Plot.DataBackground.Color = ScottPlot.Color.FromHex("#343c43");
            formsPlot12.Plot.Axes.Color(ScottPlot.Color.FromColor(System.Drawing.Color.Ivory));
            formsPlot12.Plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#5a6773");
            PixelPadding pad12 = new(1, 60, 60, 10);
            formsPlot12.Plot.Layout.Fixed(pad12);
            //formsPlot12.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.MouseWheelZoom>();
            //formsPlot12.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.MouseDragZoom>();
            //formsPlot12.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.MouseDragZoomRectangle>();
            formsPlot12.Plot.Legend.Orientation = ScottPlot.Orientation.Horizontal;
            formsPlot12.Plot.Legend.BackgroundColor = ScottPlot.Color.FromHex("#708090");
            formsPlot12.Plot.Legend.FontColor = ScottPlot.Color.FromColor(System.Drawing.Color.Ivory);
            formsPlot12.Plot.Legend.OutlineColor = ScottPlot.Color.FromHex("#5a6773");
            formsPlot12.Plot.Legend.FontSize = 9;
            formsPlot12.Plot.Legend.SymbolWidth = 5;
            formsPlot12.Plot.Legend.OutlineWidth = 5;
            formsPlot12.Plot.Legend.Padding = new PixelPadding(1, 1);
            ScottPlot.Panels.LegendPanel legendPanel12 = new(formsPlot12.Plot.Legend)
            {
                Edge = Edge.Bottom,
                Alignment = Alignment.UpperCenter,
            };
            formsPlot12.Plot.Axes.AddPanel(legendPanel12);
            formsPlot12.Plot.Legend.IsVisible = false;
            //formsPlot10.Plot.ShowLegend(Edge.Bottom);
            //formsPlot12.MouseDown += OnTouchDown;
            //formsPlot12.MouseMove += OnTouchMove;
            //formsPlot12.MouseUp += OnTouchUp;

            DisableZoomActions(ref formsPlot1);
            DisableZoomActions(ref formsPlot2);
            DisableZoomActions(ref formsPlot3);
            DisableZoomActions(ref formsPlot4);
            DisableZoomActions(ref formsPlot5);
            DisableZoomActions(ref formsPlot6);
            DisableZoomActions(ref formsPlot7);
            DisableZoomActions(ref formsPlot8);
            DisableZoomActions(ref formsPlot9);
            DisableZoomActions(ref formsPlot10);
            DisableZoomActions(ref formsPlot11);
            DisableZoomActions(ref formsPlot12);

        }
        private void InitializeUI()
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            // Set form properties
            this.Text = " Front Fork Damper Function Tester";
            //this.BackColor = Color.White;
            //this.Font = new Font("Arial", 10);

            // Initialize TextBoxes
            //textBox1.Font = new Font("Arial", 12);
            //textBox2.Font = new Font("Arial", 12);
            textBox1.TextChanged += textBox1_TextChanged;
            textBox2.TextChanged += textBox2_TextChanged;
            // Initialize DataGridViews
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            // Set up DateTimePicker
            //dateTimePicker1.Font = new Font("Arial", 12);
            dateTimePicker1.ValueChanged += dateTimePicker1_ValueChanged_1;

            EnableDoubleBuffering(tabControl1);
            tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl1.DrawItem += TabControl_DrawItem;
            tabControl1.Selected += TabControl_Selected;
            tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);

            textBox3.Click += InputTextBox_Click;
            textBox4.Click += InputTextBox_Click;
            textBox5.Click += InputTextBox_Click;
            textBox6.Click += InputTextBox_Click;

            masterValidReset();
            masterSetupReset();
            setupPlotCrosshair();

            PreloadTabPages();
        }
        public void TabPageSelect(int ivy)
        {
            tabControl1.SelectedIndex = ivy;
        }
        private void TabControl_Selected(object sender, TabControlEventArgs e)
        {
            // Refresh the TabControl to apply the drawing changes
            tabControl1.Invalidate();
        }
        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 7)
            {
                tabdataMasterSideInit();
                tabdataMasterCompStep2L();
                button59.BackColor = Color.Cyan;
                button58.BackColor = Color.LightSteelBlue;
                button65.BackColor = Color.LightSteelBlue;
                tabdataMasterCompStep2R();
                button57.BackColor = Color.Cyan;
                button56.BackColor = Color.LightSteelBlue;
                button64.BackColor = Color.LightSteelBlue;
            }
            if (tabControl1.SelectedIndex == 5)
            {
                MasteringUpdateList();
            }
            if (tabControl1.SelectedIndex == 4)
            {
                tabdataRealSideInit();
                tabdataRealCompStep2L();
                button55.BackColor = Color.Cyan;
                button54.BackColor = Color.LightSteelBlue;
                button71.BackColor = Color.LightSteelBlue;
                tabdataRealCompStep2R();
                button53.BackColor = Color.Cyan;
                button52.BackColor = Color.LightSteelBlue;
                button70.BackColor = Color.LightSteelBlue;
            }
            if (tabControl1.SelectedIndex == 2)
            {
                RealtimeUpdateList();
            }
        }
        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            // This will hide the tabs by not drawing them
        }
        private async void PreloadTabPages()
        {
            await Task.Run(() => defLayoutPanelRealSideR());
            await Task.Run(() => defLayoutPanelRealSideL());
            await Task.Run(() => defLayoutPanelMasterSideR());
            await Task.Run(() => defLayoutPanelMasterSideL());
        }
        private void EnableDoubleBuffering(Control control)
        {
            // Enable double buffering for the specified control and its children
            control.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                .SetValue(control, true, null);

            foreach (Control child in control.Controls)
            {
                EnableDoubleBuffering(child);
            }
        }
        public string ShowKeypad(string initialValue)
        {
            using (KeypadForm keypadForm = new KeypadForm(initialValue))
            {
                if (keypadForm.ShowDialog() == DialogResult.OK)
                {
                    return keypadForm.EnteredValue;
                }
                else if (keypadForm.DialogResult == DialogResult.Cancel)
                {
                    return "Cancelled";
                }
            }
            return string.Empty;
        }
        private void InputTextBox_Click(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string initialValue = textBox.Text;
            string enteredValue = ShowKeypad(initialValue);
            if (enteredValue != "Cancelled")
            {
                textBox.Text = enteredValue;
            }
        }

        private void InputTextBoxMasterTable_Click(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string initialValue = textBox.Text;
            string enteredValue = ShowKeypad(initialValue);
            if (enteredValue != "Cancelled")
            {
                confirmEditReset();
                textBox.Text = enteredValue;
            }
        }

        private void DecimalTextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                string text = textBox.Text;
                if (decimal.TryParse(text, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out decimal value))
                {
                    value = Math.Ceiling(value * 100) / 100;
                    textBox.TextChanged -= DecimalTextBox_TextChanged;
                    textBox.Text = value.ToString("F2", CultureInfo.InvariantCulture);
                    textBox.SelectionStart = textBox.Text.Length;
                    textBox.TextChanged += DecimalTextBox_TextChanged;
                }
                else
                {
                    textBox.TextChanged -= DecimalTextBox_TextChanged;
                    textBox.Text = "";
                    textBox.TextChanged += DecimalTextBox_TextChanged;
                }
            }
        }
        private void LoadCustomFont(string sourceFont)
        {
            privateFontCollection = new PrivateFontCollection();

            // Load the font from the embedded resource
            string namespaceName = Assembly.GetExecutingAssembly().GetName().Name;
            string resource = $"{namespaceName}.{sourceFont}";
            using (Stream fontStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
            {
                if (fontStream == null)
                {
                    throw new Exception("Font resource not found.");
                }

                int fontLength = (int)fontStream.Length;
                byte[] fontData = new byte[fontLength];
                fontStream.Read(fontData, 0, fontLength);

                // Create a temporary file to store the font
                string tempFilePath = Path.Combine(Path.GetTempPath(), sourceFont);
                File.WriteAllBytes(tempFilePath, fontData);

                // Load the font into the PrivateFontCollection
                privateFontCollection.AddFontFile(tempFilePath);

                // Apply the font to the form controls
                Font customFont = new Font(privateFontCollection.Families[0], 12F);
                foreach (Control control in this.Controls)
                {
                    control.Font = customFont;
                }

                // Delete the temporary font file
                File.Delete(tempFilePath);
            }
        }
        static string GetVname<T>(ref T myvar, string varName)
        {
            return varName;
        }
        static float RoundingEdge(float value, float threshold)
        {
            float touchthegrass = value - (float)Math.Floor(value);
            if (touchthegrass > 0.5f)
            {
                if (touchthegrass > (1.0f - threshold))
                {
                    return (float)Math.Ceiling(value);
                }
                else
                {
                    return (float)value;
                }
            }
            else
            {
                if (touchthegrass < threshold)
                {
                    return (float)Math.Floor(value);
                }
                else
                {
                    return (float)value;
                }
            }

            return value;
        }

        private void DisableZoomActions(ref FormsPlot formplot)
        {
            // Remove the MouseDragZoomRectangle action
            var zoomRectangleAction = formplot.UserInputProcessor.UserActionResponses
                .FirstOrDefault(response => response is MouseDragZoomRectangle);
            if (zoomRectangleAction != null)
            {
                formplot.UserInputProcessor.UserActionResponses.Remove(zoomRectangleAction);
            }

            // Remove the MouseDragZoom action
            var zoomAction = formplot.UserInputProcessor.UserActionResponses
                .FirstOrDefault(response => response is MouseDragZoom);
            if (zoomAction != null)
            {
                formplot.UserInputProcessor.UserActionResponses.Remove(zoomAction);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            // Check the number of monitors
            if (Screen.AllScreens.Length > 1)
            {
                // Get the second monitor
                Screen secondScreen = Screen.AllScreens[1];

                // Set the form's location to the second monitor
                this.Location = secondScreen.Bounds.Location;
                // Optional: Maximize the form to fill the second monitor
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                // If no second monitor is detected, run on the primary monitor
                Screen primaryScreen = Screen.PrimaryScreen;
                this.Location = primaryScreen.Bounds.Location;
                this.WindowState = FormWindowState.Maximized;
            }
        }

        #endregion

        //-----------------------------------------------------------------LaMurallaVerde-----------------------------------------------

        #region Plotting Graphs

        #region PlotInterface
        public ref ScottPlot.WinForms.FormsPlot FormPlot1()
        {
            return ref this.formsPlot1;
        }
        public ref ScottPlot.WinForms.FormsPlot FormPlot2()
        {
            return ref this.formsPlot2;
        }
        public ref ScottPlot.WinForms.FormsPlot FormPlot3()
        {
            return ref this.formsPlot3;
        }
        public ref ScottPlot.WinForms.FormsPlot FormPlot4()
        {
            return ref this.formsPlot4;
        }
        public ref ScottPlot.WinForms.FormsPlot FormPlot5()
        {
            return ref this.formsPlot5;
        }
        public ref ScottPlot.WinForms.FormsPlot FormPlot6()
        {
            return ref this.formsPlot6;
        }
        public ref ScottPlot.WinForms.FormsPlot FormPlot7()
        {
            return ref this.formsPlot7;
        }
        public ref ScottPlot.WinForms.FormsPlot FormPlot8()
        {
            return ref this.formsPlot8;
        }
        public ref ScottPlot.WinForms.FormsPlot FormPlot9()
        {
            return ref this.formsPlot9;
        }
        public ref ScottPlot.WinForms.FormsPlot FormPlot10()
        {
            return ref this.formsPlot10;
        }
        public ref ScottPlot.WinForms.FormsPlot FormPlot11()
        {
            return ref this.formsPlot11;
        }
        public ref ScottPlot.WinForms.FormsPlot FormPlot12()
        {
            return ref this.formsPlot12;
        }

        #endregion

        #region PlotComponents

        public ScottPlot.Plottables.SignalXY Plot1_PRESENT;
        public ScottPlot.Plottables.SignalXY Plot1_MASTER;
        public ScottPlot.Plottables.SignalXY Plot1_LOWER;
        public ScottPlot.Plottables.SignalXY Plot1_UPPER;
        ScottPlot.Plottables.Crosshair Plot1_Crosshair;

        public ScottPlot.Plottables.SignalXY Plot2_PRESENT;
        public ScottPlot.Plottables.SignalXY Plot2_MASTER;
        public ScottPlot.Plottables.SignalXY Plot2_LOWER;
        public ScottPlot.Plottables.SignalXY Plot2_UPPER;
        ScottPlot.Plottables.Crosshair Plot2_Crosshair;

        public ScottPlot.Plottables.SignalXY Plot3_PRESENT;
        public ScottPlot.Plottables.SignalXY Plot3_MASTER;
        public ScottPlot.Plottables.SignalXY Plot3_LOWER;
        public ScottPlot.Plottables.SignalXY Plot3_UPPER;
        ScottPlot.Plottables.Crosshair Plot3_Crosshair;

        public ScottPlot.Plottables.SignalXY Plot4_PRESENT;
        public ScottPlot.Plottables.SignalXY Plot4_MASTER;
        public ScottPlot.Plottables.SignalXY Plot4_LOWER;
        public ScottPlot.Plottables.SignalXY Plot4_UPPER;
        ScottPlot.Plottables.Crosshair Plot4_Crosshair;

        public ScottPlot.Plottables.SignalXY Plot5_PRESENT;
        public ScottPlot.Plottables.SignalXY Plot5_MASTER;
        public ScottPlot.Plottables.SignalXY Plot5_LOWER;
        public ScottPlot.Plottables.SignalXY Plot5_UPPER;
        ScottPlot.Plottables.Crosshair Plot5_Crosshair;

        public ScottPlot.Plottables.SignalXY Plot6_PRESENT;
        public ScottPlot.Plottables.SignalXY Plot6_MASTER;
        public ScottPlot.Plottables.SignalXY Plot6_LOWER;
        public ScottPlot.Plottables.SignalXY Plot6_UPPER;
        ScottPlot.Plottables.Crosshair Plot6_Crosshair;

        public ScottPlot.Plottables.SignalXY Plot7_PRESENT;
        public ScottPlot.Plottables.SignalXY Plot7_MASTER;
        public ScottPlot.Plottables.SignalXY Plot7_LOWER;
        public ScottPlot.Plottables.SignalXY Plot7_UPPER;
        ScottPlot.Plottables.Crosshair Plot7_Crosshair;

        public ScottPlot.Plottables.SignalXY Plot8_PRESENT;
        public ScottPlot.Plottables.SignalXY Plot8_MASTER;
        public ScottPlot.Plottables.SignalXY Plot8_LOWER;
        public ScottPlot.Plottables.SignalXY Plot8_UPPER;
        ScottPlot.Plottables.Crosshair Plot8_Crosshair;

        public ScottPlot.Plottables.SignalXY Plot9_PRESENT;
        public ScottPlot.Plottables.SignalXY Plot9_MASTER;
        public ScottPlot.Plottables.SignalXY Plot9_LOWER;
        public ScottPlot.Plottables.SignalXY Plot9_UPPER;
        ScottPlot.Plottables.Crosshair Plot9_Crosshair;

        public ScottPlot.Plottables.SignalXY Plot10_PRESENT;
        public ScottPlot.Plottables.SignalXY Plot10_MASTER;
        public ScottPlot.Plottables.SignalXY Plot10_LOWER;
        public ScottPlot.Plottables.SignalXY Plot10_UPPER;
        ScottPlot.Plottables.Crosshair Plot10_Crosshair;

        public ScottPlot.Plottables.SignalXY Plot11_PRESENT;
        public ScottPlot.Plottables.SignalXY Plot11_MASTER;
        public ScottPlot.Plottables.SignalXY Plot11_LOWER;
        public ScottPlot.Plottables.SignalXY Plot11_UPPER;
        ScottPlot.Plottables.Crosshair Plot11_Crosshair;

        public ScottPlot.Plottables.SignalXY Plot12_PRESENT;
        public ScottPlot.Plottables.SignalXY Plot12_MASTER;
        public ScottPlot.Plottables.SignalXY Plot12_LOWER;
        public ScottPlot.Plottables.SignalXY Plot12_UPPER;
        ScottPlot.Plottables.Crosshair Plot12_Crosshair;

        #endregion

        void setupPlotCrosshair()
        {
            settingPlotCrosshair(ref FormPlot1(), ref Plot1_Crosshair, ref Plot1Coord);
            settingPlotCrosshair(ref FormPlot2(), ref Plot2_Crosshair, ref Plot2Coord);
            settingPlotCrosshair(ref FormPlot3(), ref Plot3_Crosshair, ref Plot3Coord);
            settingPlotCrosshair(ref FormPlot4(), ref Plot4_Crosshair, ref Plot4Coord);
            settingPlotCrosshair(ref FormPlot5(), ref Plot5_Crosshair, ref Plot5Coord);
            settingPlotCrosshair(ref FormPlot6(), ref Plot6_Crosshair, ref Plot6Coord);
            settingPlotCrosshair(ref FormPlot7(), ref Plot7_Crosshair, ref Plot7Coord);
            settingPlotCrosshair(ref FormPlot8(), ref Plot8_Crosshair, ref Plot8Coord);
            settingPlotCrosshair(ref FormPlot9(), ref Plot9_Crosshair, ref Plot9Coord);
            settingPlotCrosshair(ref FormPlot10(), ref Plot10_Crosshair, ref Plot10Coord);
            settingPlotCrosshair(ref FormPlot11(), ref Plot11_Crosshair, ref Plot11Coord);
            settingPlotCrosshair(ref FormPlot12(), ref Plot12_Crosshair, ref Plot12Coord);
        }
        void settingPlotCrosshair(ref ScottPlot.WinForms.FormsPlot plotbase, ref ScottPlot.Plottables.Crosshair plotcross, ref Label labeltextout)
        {
            plotcross = plotbase.Plot.Add.Crosshair(0, 0);
            plotcross.IsVisible = false;
            plotcross.LineColor = ScottPlot.Color.FromColor(System.Drawing.Color.White);
            plotcross.MarkerShape = MarkerShape.OpenCircle;
            plotcross.MarkerColor = ScottPlot.Color.FromColor(System.Drawing.Color.White);
            plotcross.MarkerSize = 5;
            functionPlotCrosshair(ref plotbase, ref plotcross, ref labeltextout);
        }
        void functionPlotCrosshair(ref ScottPlot.WinForms.FormsPlot plotbase, ref ScottPlot.Plottables.Crosshair plotcross, ref Label textout)
        {
            var eventHandler = new MouseMoveEventHandler(plotcross, textout);
            plotbase.MouseMove += eventHandler.OnMouseMove;
        }

        public void AllPlotReset()
        {
            formsPlot1.Reset();
            formsPlot2.Reset();
            formsPlot3.Reset();
            formsPlot4.Reset();
            formsPlot9.Reset();
            formsPlot10.Reset();

            formsPlot1.Refresh();
            formsPlot2.Refresh();
            formsPlot3.Refresh();
            formsPlot4.Refresh();
            formsPlot9.Refresh();
            formsPlot10.Refresh();
        }
        public void MasterPlotReset()
        {
            formsPlot5.Reset();
            formsPlot6.Reset();
            formsPlot7.Reset();
            formsPlot8.Reset();
            formsPlot11.Reset();
            formsPlot12.Reset();

            formsPlot5.Refresh();
            formsPlot6.Refresh();
            formsPlot7.Refresh();
            formsPlot8.Refresh();
            formsPlot11.Refresh();
            formsPlot12.Refresh();
        }

        public void PlotSignalPlotting(ref ScottPlot.WinForms.FormsPlot plotbase, ref ScottPlot.Plottables.SignalXY plotsig, double[] xd, double[] yd, [CallerArgumentExpression("plotsig")] string varName = null)
        {
            if (plotsig == null)
            {
                plotsig = plotbase.Plot.Add.SignalXY(xd, yd);
            }
            else
            {
                ISignalXYSource src = new SignalXYSourceDoubleArray(xd, yd);
                plotsig.Data = src;
            }
            plotsig.LineWidth = 2;

            string varname = GetVname(ref plotsig, varName);
            if (varname.Contains("PRESENT"))
            {
                plotsig.LegendText = "PRESENT";
            }
            else if (varname.Contains("MASTER"))
            {
                plotsig.LegendText = "MASTER";
            }
            else if (varname.Contains("LOWER"))
            {
                plotsig.LegendText = "LOWER";
            }
            else if (varname.Contains("UPPER"))
            {
                plotsig.LegendText = "UPPER";
            }

            if (varname.Contains("Plot3") || varname.Contains("Plot4") || varname.Contains("Plot7") || varname.Contains("Plot8") || varname.Contains("Plot10") || varname.Contains("Plot12"))
            {
                plotsig.Axes.YAxis = plotbase.Plot.Axes.Right;
            }
        }
        public void PlotChangeColor(ref ScottPlot.Plottables.SignalXY plotsig, System.Drawing.Color linecolor)
        {
            plotsig.Color = ScottPlot.Color.FromColor(linecolor);
        }
        public void PlotBringToFront(ref ScottPlot.WinForms.FormsPlot plotbase, ref ScottPlot.Plottables.SignalXY plotsig)
        {
            plotbase.Plot.MoveToFront(plotsig);
        }
        public void PlotSignalLineShow(ref ScottPlot.Plottables.SignalXY plotsig)
        {
            plotsig.IsVisible = true;
        }
        public void PlotSignalLineHide(ref ScottPlot.Plottables.SignalXY plotsig)
        {
            plotsig.IsVisible = false;
        }
        public bool PlotSignalLineVisible(ref ScottPlot.Plottables.SignalXY plotsig)
        {
            return plotsig.IsVisible;
        }
        public void PlotShowLegend(ref ScottPlot.WinForms.FormsPlot plotbase)
        {
            plotbase.Plot.ShowLegend();
        }
        public void PlotCheckInverted(ref ScottPlot.WinForms.FormsPlot plotbase)
        {
            if (plotbase.Plot.Axes.Bottom.IsInverted())
            {
                plotbase.Plot.Axes.RectifyX();
            }
        }
        public void PlotCheckAutoScale(ref ScottPlot.WinForms.FormsPlot plotbase)
        {
            plotbase.Plot.Axes.AutoScale();
        }
        public void PlotZoomin(ref ScottPlot.WinForms.FormsPlot plotbase, double fX = 1.0, double fY = 1.0)
        {
            plotbase.Plot.Axes.ZoomIn(fX, fY);
        }
        public void PlotZoomout(ref ScottPlot.WinForms.FormsPlot plotbase, double fX = 1.0, double fY = 1.0)
        {
            plotbase.Plot.Axes.ZoomOut(fX, fY);
        }
        public void workSumPlotCheck(ref ScottPlot.WinForms.FormsPlot plotbase)
        {
            plotbase.Plot.Axes.AutoScale();
            if (plotbase.Plot.Axes.Bottom.IsInverted())
            {
                plotbase.Plot.Axes.RectifyX();
            }
            //plotbase.Plot.ShowLegend();
            plotbase.Refresh();
        }


        void RealtimeMasterLineShowHide()
        {
            if (Plot1_MASTER != null)
            {
                if (!Plot1_MASTER.IsVisible)
                {
                    PlotSignalLineShow(ref Plot1_MASTER);
                }
                else
                {
                    PlotSignalLineHide(ref Plot1_MASTER);
                }
            }
            if (Plot2_MASTER != null)
            {
                if (!Plot2_MASTER.IsVisible)
                {
                    PlotSignalLineShow(ref Plot2_MASTER);
                }
                else
                {
                    PlotSignalLineHide(ref Plot2_MASTER);
                }
            }
            if (Plot3_MASTER != null)
            {
                if (!Plot3_MASTER.IsVisible)
                {
                    PlotSignalLineShow(ref Plot3_MASTER);
                }
                else
                {
                    PlotSignalLineHide(ref Plot3_MASTER);
                }
            }
            if (Plot4_MASTER != null)
            {
                if (!Plot4_MASTER.IsVisible)
                {
                    PlotSignalLineShow(ref Plot4_MASTER);
                }
                else
                {
                    PlotSignalLineHide(ref Plot4_MASTER);
                }
            }
            if (Plot9_MASTER != null)
            {
                if (!Plot9_MASTER.IsVisible)
                {
                    PlotSignalLineShow(ref Plot9_MASTER);
                }
                else
                {
                    PlotSignalLineHide(ref Plot9_MASTER);
                }
            }
            if (Plot10_MASTER != null)
            {
                if (!Plot10_MASTER.IsVisible)
                {
                    PlotSignalLineShow(ref Plot10_MASTER);
                }
                else
                {
                    PlotSignalLineHide(ref Plot10_MASTER);
                }
            }
            if (Plot1_MASTER != null && Plot2_MASTER != null && Plot3_MASTER != null && Plot4_MASTER != null && Plot9_MASTER != null && Plot10_MASTER != null)
            {
                if (Plot1_MASTER.IsVisible && Plot2_MASTER.IsVisible && Plot3_MASTER.IsVisible && Plot4_MASTER.IsVisible && Plot9_MASTER.IsVisible && Plot10_MASTER.IsVisible)
                {

                }
                else if (Plot1_MASTER.IsVisible || Plot2_MASTER.IsVisible || Plot3_MASTER.IsVisible || Plot4_MASTER.IsVisible || Plot9_MASTER.IsVisible || Plot10_MASTER.IsVisible)
                {
                    PlotSignalLineHide(ref Plot1_MASTER);
                    PlotSignalLineHide(ref Plot2_MASTER);
                    PlotSignalLineHide(ref Plot3_MASTER);
                    PlotSignalLineHide(ref Plot4_MASTER);
                    PlotSignalLineHide(ref Plot9_MASTER);
                    PlotSignalLineHide(ref Plot10_MASTER);
                }
            }

            formsPlot1.Refresh();
            formsPlot2.Refresh();
            formsPlot3.Refresh();
            formsPlot4.Refresh();
            formsPlot9.Refresh();
            formsPlot10.Refresh();
        }
        void RealtimeMasterLimitShowHide()
        {
            if (Plot1_LOWER != null)
            {
                if (!Plot1_LOWER.IsVisible)
                {
                    PlotSignalLineShow(ref Plot1_LOWER);
                }
                else
                {
                    PlotSignalLineHide(ref Plot1_LOWER);
                }
            }
            if (Plot2_LOWER != null)
            {
                if (!Plot2_LOWER.IsVisible)
                {
                    PlotSignalLineShow(ref Plot2_LOWER);
                }
                else
                {
                    PlotSignalLineHide(ref Plot2_LOWER);
                }
            }
            if (Plot3_LOWER != null)
            {
                if (!Plot3_LOWER.IsVisible)
                {
                    PlotSignalLineShow(ref Plot3_LOWER);
                }
                else
                {
                    PlotSignalLineHide(ref Plot3_LOWER);
                }
            }
            if (Plot4_LOWER != null)
            {
                if (!Plot4_LOWER.IsVisible)
                {
                    PlotSignalLineShow(ref Plot4_LOWER);
                }
                else
                {
                    PlotSignalLineHide(ref Plot4_LOWER);
                }
            }
            if (Plot9_LOWER != null)
            {
                if (!Plot9_LOWER.IsVisible)
                {
                    PlotSignalLineShow(ref Plot9_LOWER);
                }
                else
                {
                    PlotSignalLineHide(ref Plot9_LOWER);
                }
            }
            if (Plot10_LOWER != null)
            {
                if (!Plot10_LOWER.IsVisible)
                {
                    PlotSignalLineShow(ref Plot10_LOWER);
                }
                else
                {
                    PlotSignalLineHide(ref Plot10_LOWER);
                }
            }

            if (Plot1_UPPER != null)
            {
                if (!Plot1_UPPER.IsVisible)
                {
                    PlotSignalLineShow(ref Plot1_UPPER);
                }
                else
                {
                    PlotSignalLineHide(ref Plot1_UPPER);
                }
            }
            if (Plot2_UPPER != null)
            {
                if (!Plot2_UPPER.IsVisible)
                {
                    PlotSignalLineShow(ref Plot2_UPPER);
                }
                else
                {
                    PlotSignalLineHide(ref Plot2_UPPER);
                }
            }
            if (Plot3_UPPER != null)
            {
                if (!Plot3_UPPER.IsVisible)
                {
                    PlotSignalLineShow(ref Plot3_UPPER);
                }
                else
                {
                    PlotSignalLineHide(ref Plot3_UPPER);
                }
            }
            if (Plot4_UPPER != null)
            {
                if (!Plot4_UPPER.IsVisible)
                {
                    PlotSignalLineShow(ref Plot4_UPPER);
                }
                else
                {
                    PlotSignalLineHide(ref Plot4_UPPER);
                }
            }
            if (Plot9_UPPER != null)
            {
                if (!Plot9_UPPER.IsVisible)
                {
                    PlotSignalLineShow(ref Plot9_UPPER);
                }
                else
                {
                    PlotSignalLineHide(ref Plot9_UPPER);
                }
            }
            if (Plot10_UPPER != null)
            {
                if (!Plot10_UPPER.IsVisible)
                {
                    PlotSignalLineShow(ref Plot10_UPPER);
                }
                else
                {
                    PlotSignalLineHide(ref Plot10_UPPER);
                }
            }
            if (Plot1_UPPER != null && Plot2_UPPER != null && Plot3_UPPER != null && Plot4_UPPER != null && Plot9_UPPER != null && Plot10_UPPER != null && Plot1_LOWER != null && Plot2_LOWER != null && Plot3_LOWER != null && Plot4_LOWER != null && Plot9_LOWER != null && Plot10_LOWER != null)
            {
                if (Plot1_UPPER.IsVisible && Plot2_UPPER.IsVisible && Plot3_UPPER.IsVisible && Plot4_UPPER.IsVisible && Plot9_UPPER.IsVisible && Plot10_UPPER.IsVisible && Plot1_LOWER.IsVisible && Plot2_LOWER.IsVisible && Plot3_LOWER.IsVisible && Plot4_LOWER.IsVisible && Plot9_LOWER.IsVisible && Plot10_LOWER.IsVisible)
                {

                }
                else if (Plot1_UPPER.IsVisible || Plot2_UPPER.IsVisible || Plot3_UPPER.IsVisible || Plot4_UPPER.IsVisible || Plot9_UPPER.IsVisible || Plot10_UPPER.IsVisible || Plot1_LOWER.IsVisible || Plot2_LOWER.IsVisible || Plot3_LOWER.IsVisible || Plot4_LOWER.IsVisible || Plot9_LOWER.IsVisible || Plot10_LOWER.IsVisible)
                {
                    PlotSignalLineHide(ref Plot1_LOWER);
                    PlotSignalLineHide(ref Plot2_LOWER);
                    PlotSignalLineHide(ref Plot3_LOWER);
                    PlotSignalLineHide(ref Plot4_LOWER);
                    PlotSignalLineHide(ref Plot9_LOWER);
                    PlotSignalLineHide(ref Plot10_LOWER);

                    PlotSignalLineHide(ref Plot1_UPPER);
                    PlotSignalLineHide(ref Plot2_UPPER);
                    PlotSignalLineHide(ref Plot3_UPPER);
                    PlotSignalLineHide(ref Plot4_UPPER);
                    PlotSignalLineHide(ref Plot9_UPPER);
                    PlotSignalLineHide(ref Plot10_UPPER);

                }
            }

            formsPlot1.Refresh();
            formsPlot2.Refresh();
            formsPlot3.Refresh();
            formsPlot4.Refresh();
            formsPlot9.Refresh();
            formsPlot10.Refresh();
        }

        void TeachingMasterLineShowHide()
        {
            if (Plot5_MASTER != null)
            {
                if (!Plot5_MASTER.IsVisible)
                {
                    PlotSignalLineShow(ref Plot5_MASTER);
                }
                else
                {
                    PlotSignalLineHide(ref Plot5_MASTER);
                }
            }
            if (Plot6_MASTER != null)
            {
                if (!Plot6_MASTER.IsVisible)
                {
                    PlotSignalLineShow(ref Plot6_MASTER);
                }
                else
                {
                    PlotSignalLineHide(ref Plot6_MASTER);
                }
            }
            if (Plot7_MASTER != null)
            {
                if (!Plot7_MASTER.IsVisible)
                {
                    PlotSignalLineShow(ref Plot7_MASTER);
                }
                else
                {
                    PlotSignalLineHide(ref Plot7_MASTER);
                }
            }
            if (Plot8_MASTER != null)
            {
                if (!Plot8_MASTER.IsVisible)
                {
                    PlotSignalLineShow(ref Plot8_MASTER);
                }
                else
                {
                    PlotSignalLineHide(ref Plot8_MASTER);
                }
            }
            if (Plot11_MASTER != null)
            {
                if (!Plot11_MASTER.IsVisible)
                {
                    PlotSignalLineShow(ref Plot11_MASTER);
                }
                else
                {
                    PlotSignalLineHide(ref Plot11_MASTER);
                }
            }
            if (Plot12_MASTER != null)
            {
                if (!Plot12_MASTER.IsVisible)
                {
                    PlotSignalLineShow(ref Plot12_MASTER);
                }
                else
                {
                    PlotSignalLineHide(ref Plot12_MASTER);
                }
            }
            if (Plot5_MASTER != null && Plot6_MASTER != null && Plot7_MASTER != null && Plot8_MASTER != null && Plot11_MASTER != null && Plot12_MASTER != null)
            {
                if (Plot5_MASTER.IsVisible && Plot6_MASTER.IsVisible && Plot7_MASTER.IsVisible && Plot8_MASTER.IsVisible && Plot11_MASTER.IsVisible && Plot12_MASTER.IsVisible)
                {

                }
                else if (Plot5_MASTER.IsVisible || Plot6_MASTER.IsVisible || Plot7_MASTER.IsVisible || Plot8_MASTER.IsVisible || Plot11_MASTER.IsVisible || Plot12_MASTER.IsVisible)
                {
                    PlotSignalLineHide(ref Plot5_MASTER);
                    PlotSignalLineHide(ref Plot6_MASTER);
                    PlotSignalLineHide(ref Plot7_MASTER);
                    PlotSignalLineHide(ref Plot8_MASTER);
                    PlotSignalLineHide(ref Plot11_MASTER);
                    PlotSignalLineHide(ref Plot12_MASTER);
                }
            }

            formsPlot5.Refresh();
            formsPlot6.Refresh();
            formsPlot7.Refresh();
            formsPlot8.Refresh();
            formsPlot11.Refresh();
            formsPlot12.Refresh();
        }
        void TeachingMasterLimitShowHide()
        {
            if (Plot5_LOWER != null)
            {
                if (!Plot5_LOWER.IsVisible)
                {
                    PlotSignalLineShow(ref Plot5_LOWER);
                }
                else
                {
                    PlotSignalLineHide(ref Plot5_LOWER);
                }
            }
            if (Plot6_LOWER != null)
            {
                if (!Plot6_LOWER.IsVisible)
                {
                    PlotSignalLineShow(ref Plot6_LOWER);
                }
                else
                {
                    PlotSignalLineHide(ref Plot6_LOWER);
                }
            }
            if (Plot7_LOWER != null)
            {
                if (!Plot7_LOWER.IsVisible)
                {
                    PlotSignalLineShow(ref Plot7_LOWER);
                }
                else
                {
                    PlotSignalLineHide(ref Plot7_LOWER);
                }
            }
            if (Plot8_LOWER != null)
            {
                if (!Plot8_LOWER.IsVisible)
                {
                    PlotSignalLineShow(ref Plot8_LOWER);
                }
                else
                {
                    PlotSignalLineHide(ref Plot8_LOWER);
                }
            }
            if (Plot11_LOWER != null)
            {
                if (!Plot11_LOWER.IsVisible)
                {
                    PlotSignalLineShow(ref Plot11_LOWER);
                }
                else
                {
                    PlotSignalLineHide(ref Plot11_LOWER);
                }
            }
            if (Plot12_LOWER != null)
            {
                if (!Plot12_LOWER.IsVisible)
                {
                    PlotSignalLineShow(ref Plot12_LOWER);
                }
                else
                {
                    PlotSignalLineHide(ref Plot12_LOWER);
                }
            }

            if (Plot5_UPPER != null)
            {
                if (!Plot5_UPPER.IsVisible)
                {
                    PlotSignalLineShow(ref Plot5_UPPER);
                }
                else
                {
                    PlotSignalLineHide(ref Plot5_UPPER);
                }
            }
            if (Plot6_UPPER != null)
            {
                if (!Plot6_UPPER.IsVisible)
                {
                    PlotSignalLineShow(ref Plot6_UPPER);
                }
                else
                {
                    PlotSignalLineHide(ref Plot6_UPPER);
                }
            }
            if (Plot7_UPPER != null)
            {
                if (!Plot7_UPPER.IsVisible)
                {
                    PlotSignalLineShow(ref Plot7_UPPER);
                }
                else
                {
                    PlotSignalLineHide(ref Plot7_UPPER);
                }
            }
            if (Plot8_UPPER != null)
            {
                if (!Plot8_UPPER.IsVisible)
                {
                    PlotSignalLineShow(ref Plot8_UPPER);
                }
                else
                {
                    PlotSignalLineHide(ref Plot8_UPPER);
                }
            }
            if (Plot11_UPPER != null)
            {
                if (!Plot11_UPPER.IsVisible)
                {
                    PlotSignalLineShow(ref Plot11_UPPER);
                }
                else
                {
                    PlotSignalLineHide(ref Plot11_UPPER);
                }
            }
            if (Plot12_UPPER != null)
            {
                if (!Plot12_UPPER.IsVisible)
                {
                    PlotSignalLineShow(ref Plot12_UPPER);
                }
                else
                {
                    PlotSignalLineHide(ref Plot12_UPPER);
                }
            }
            if (Plot5_UPPER != null && Plot6_UPPER != null && Plot7_UPPER != null && Plot8_UPPER != null && Plot11_UPPER != null && Plot12_UPPER != null && Plot5_LOWER != null && Plot6_LOWER != null && Plot7_LOWER != null && Plot8_LOWER != null && Plot11_LOWER != null && Plot12_LOWER != null)
            {
                if (Plot5_UPPER.IsVisible && Plot6_UPPER.IsVisible && Plot7_UPPER.IsVisible && Plot8_UPPER.IsVisible && Plot11_UPPER.IsVisible && Plot12_UPPER.IsVisible && Plot5_LOWER.IsVisible && Plot6_LOWER.IsVisible && Plot7_LOWER.IsVisible && Plot8_LOWER.IsVisible && Plot11_LOWER.IsVisible && Plot12_LOWER.IsVisible)
                {

                }
                else if (Plot5_UPPER.IsVisible || Plot6_UPPER.IsVisible || Plot7_UPPER.IsVisible || Plot8_UPPER.IsVisible || Plot11_UPPER.IsVisible || Plot12_UPPER.IsVisible || Plot5_LOWER.IsVisible || Plot6_LOWER.IsVisible || Plot7_LOWER.IsVisible || Plot8_LOWER.IsVisible || Plot11_LOWER.IsVisible || Plot12_LOWER.IsVisible)
                {
                    PlotSignalLineHide(ref Plot5_LOWER);
                    PlotSignalLineHide(ref Plot6_LOWER);
                    PlotSignalLineHide(ref Plot7_LOWER);
                    PlotSignalLineHide(ref Plot8_LOWER);
                    PlotSignalLineHide(ref Plot11_LOWER);
                    PlotSignalLineHide(ref Plot12_LOWER);

                    PlotSignalLineHide(ref Plot5_UPPER);
                    PlotSignalLineHide(ref Plot6_UPPER);
                    PlotSignalLineHide(ref Plot7_UPPER);
                    PlotSignalLineHide(ref Plot8_UPPER);
                    PlotSignalLineHide(ref Plot11_UPPER);
                    PlotSignalLineHide(ref Plot12_UPPER);

                }
            }

            formsPlot5.Refresh();
            formsPlot6.Refresh();
            formsPlot7.Refresh();
            formsPlot8.Refresh();
            formsPlot11.Refresh();
            formsPlot12.Refresh();
        }

        #endregion

        #region UIMethod
        private void CopyDirectory(string sourceDir, string destDir)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDir);
            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            // Get the files in the source directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDir, file.Name);
                file.CopyTo(tempPath, false);
            }

            // Copy subdirectories and their contents to the new location.
            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(destDir, subdir.Name);
                CopyDirectory(subdir.FullName, tempPath);
            }
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
            //listtablefile.Columns.Add("Time");
            listtablefile.Columns.Add("File Name");

            for (int i = 0; i < idx2; i++)
            {
                DataRow newRow = listtablefile.NewRow();
                //newRow["Time"] = listfiles_date[i];
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

            button48.Text = "Connected";
            button48.ForeColor = System.Drawing.Color.Black;
            button48.BackColor = System.Drawing.Color.LimeGreen;
        }
        public void connStatLampOff()
        {
            button5.Text = "Disconnected";
            button5.ForeColor = System.Drawing.Color.Black;
            button5.BackColor = System.Drawing.Color.Red;

            button48.Text = "Disconnected";
            button48.ForeColor = System.Drawing.Color.Black;
            button48.BackColor = System.Drawing.Color.Red;
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

        public void masterSetupSet()
        {
            button83.Text = "SETUP OK";
            button83.ForeColor = System.Drawing.Color.Black;
            button83.BackColor = System.Drawing.Color.Cyan;
        }
        public void masterSetupReset()
        {
            button83.Text = "SETUP NG";
            button83.ForeColor = System.Drawing.Color.Ivory;
            button83.BackColor = System.Drawing.Color.Maroon;
        }

        public void masterValidSet()
        {
            button84.Text = "MASTER VALID";
            button84.ForeColor = System.Drawing.Color.Black;
            button84.BackColor = System.Drawing.Color.DodgerBlue;
            button80.BackColor = Color.DodgerBlue;
        }


        public void masterValidReset()
        {
            button84.Text = "MASTER INVALID";
            button84.ForeColor = System.Drawing.Color.Ivory;
            button84.BackColor = System.Drawing.Color.Indigo;
            button80.BackColor = Color.LightSteelBlue;
        }

        public void updateMasterSet()
        {
            Thread.Sleep(1000);
            button30.BackColor = Color.DodgerBlue;
        }

        public void updateMasterReset()
        {
            button30.BackColor = Color.LightSteelBlue;
        }

        public void updateTeachingSet()
        {
            Thread.Sleep(1000);
            button39.BackColor = Color.DodgerBlue;
        }

        public void updateTeachingReset()
        {
            button39.BackColor = Color.LightSteelBlue;
        }

        public void confirmEditSet()
        {
            Thread.Sleep(1000);
            button31.BackColor = Color.DodgerBlue;
        }

        public void confirmEditReset()
        {
            button31.BackColor = Color.LightSteelBlue;
        }

        public void uiLoadPosMonitor(float lload, float lpos, float rload, float rpos)
        {
            TXLoadL1.Text = $"Load: {lload:0.##} N";
            TXLoadL2.Text = $"Load: {lload:0.##} N";
            TXPosL1.Text = $"Pos: {lpos:0.##} mm";
            TXPosL2.Text = $"Pos: {lpos:0.##} mm";

            TXLoadR1.Text = $"Load: {rload:0.##} N";
            TXLoadR1.TextAlign = ContentAlignment.MiddleRight;
            TXLoadR2.Text = $"Load: {rload:0.##} N";
            TXLoadR2.TextAlign = ContentAlignment.MiddleRight;
            TXPosR1.Text = $"Pos: {rpos:0.##} mm";
            TXPosR1.TextAlign = ContentAlignment.MiddleRight;
            TXPosR2.Text = $"Pos: {rpos:0.##} mm";
            TXPosR2.TextAlign = ContentAlignment.MiddleRight;
        }

        #endregion

        #region UI Action

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
            DataGridViewRow viewRow = new DataGridViewRow();
            viewRow = dataGridView1.CurrentRow;
            string selectedfile = new string(viewRow.Cells[1].FormattedValue.ToString());

            if (selectedfile != null && viewRow != null)
            {
                Excel.Application objExcel = new Excel.Application();
                Excel.Workbook excelWorkbook = objExcel.Workbooks.Open($"{DirRealtime}\\{selectedfile}");
                objExcel.Visible = true;
            }
        }

        private void button20_Click(object sender, EventArgs e)
        {
            string DirMaster = _WorkflowHandler.MasterDir;
            CheckFolderPath(DirMaster);
            DataGridViewRow viewRow = new DataGridViewRow();
            viewRow = dataGridView2.CurrentRow;
            string selectedfile = new string(viewRow.Cells[1].FormattedValue.ToString());

            if (selectedfile != null && viewRow != null)
            {
                Excel.Application objExcel = new Excel.Application();
                Excel.Workbook excelWorkbook = objExcel.Workbooks.Open($"{DirMaster}\\{selectedfile}");
                objExcel.Visible = true;
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            //upload data from machine to excel master database
            _WorkflowHandler.MasterUpdatingDatabaseSet();
            _WorkflowHandler.workUpdateMasterDatabase();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            //download to machine from excel master database
            if (!_WorkflowHandler.MasterIsUpdatingDatabase())
            {
                _WorkflowHandler.workMasterValidation();
            }
        }

        private void button28_Click(object sender, EventArgs e)
        {

        }

        private void button47_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
        }

        private void button41_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 2;
        }

        private void button42_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 3;
        }

        private void button43_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 4;
        }

        private void button46_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 5;
        }

        private void button45_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 6;
        }

        private void button44_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 7;
        }

        private void button40_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
        }

        private void button35_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
        }

        private void button36_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
        }

        private void button34_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
        }

        private void button37_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
        }

        private void button38_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
        }

        private void button39_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
        }

        private void button32_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 3;
        }

        private void button29_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 6;
        }

        private void button33_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 4;
        }

        private void button28_Click_1(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 7;
        }

        private void button72_Click(object sender, EventArgs e)
        {
            if (tabIdxRealSideL > 0)
            {
                tabIdxRealSideL -= 1;
                if (ActiveRealTableLeftData == 1)
                {
                    tabdataRealCompStep2L();
                }
                if (ActiveRealTableLeftData == 2)
                {
                    tabdataRealExtnStep2L();
                }
                if (ActiveRealTableLeftData == 3)
                {
                    tabdataRealDiffStep2L();
                }
            }
        }

        private void button73_Click(object sender, EventArgs e)
        {
            if (tabIdxRealSideL < 4)
            {
                tabIdxRealSideL += 1;
                if (ActiveRealTableLeftData == 1)
                {
                    tabdataRealCompStep2L();
                }
                if (ActiveRealTableLeftData == 2)
                {
                    tabdataRealExtnStep2L();
                }
                if (ActiveRealTableLeftData == 3)
                {
                    tabdataRealDiffStep2L();
                }
            }
        }

        private void button75_Click(object sender, EventArgs e)
        {
            if (tabIdxRealSideR > 0)
            {
                tabIdxRealSideR -= 1;
                if (ActiveRealTableRightData == 1)
                {
                    tabdataRealCompStep2R();
                }
                if (ActiveRealTableRightData == 2)
                {
                    tabdataRealExtnStep2R();
                }
                if (ActiveRealTableRightData == 3)
                {
                    tabdataRealDiffStep2R();
                }
            }
        }

        private void button74_Click(object sender, EventArgs e)
        {
            if (tabIdxRealSideR < 4)
            {
                tabIdxRealSideR += 1;
                if (ActiveRealTableRightData == 1)
                {
                    tabdataRealCompStep2R();
                }
                if (ActiveRealTableRightData == 2)
                {
                    tabdataRealExtnStep2R();
                }
                if (ActiveRealTableRightData == 3)
                {
                    tabdataRealDiffStep2R();
                }
            }
        }

        private void button79_Click(object sender, EventArgs e)
        {
            if (tabIdxMasterSideL > 0)
            {
                tabIdxMasterSideL -= 1;
                if (ActiveMasterTableLeftData == 1)
                {
                    tabdataMasterCompStep2L();
                }
                if (ActiveMasterTableLeftData == 2)
                {
                    tabdataMasterExtnStep2L();
                }
                if (ActiveMasterTableLeftData == 3)
                {
                    tabdataMasterDiffStep2L();
                }
            }
        }

        private void button78_Click(object sender, EventArgs e)
        {
            if (tabIdxMasterSideL < 4)
            {
                tabIdxMasterSideL += 1;
                if (ActiveMasterTableLeftData == 1)
                {
                    tabdataMasterCompStep2L();
                }
                if (ActiveMasterTableLeftData == 2)
                {
                    tabdataMasterExtnStep2L();
                }
                if (ActiveMasterTableLeftData == 3)
                {
                    tabdataMasterDiffStep2L();
                }
            }
        }

        private void button77_Click(object sender, EventArgs e)
        {
            if (tabIdxMasterSideR > 0)
            {
                tabIdxMasterSideR -= 1;
                if (ActiveMasterTableRightData == 1)
                {
                    tabdataMasterCompStep2R();
                }
                if (ActiveMasterTableRightData == 2)
                {
                    tabdataMasterExtnStep2R();
                }
                if (ActiveMasterTableRightData == 3)
                {
                    tabdataMasterDiffStep2R();
                }
            }
        }

        private void button76_Click(object sender, EventArgs e)
        {
            if (tabIdxMasterSideR < 4)
            {
                tabIdxMasterSideR += 1;
                if (ActiveMasterTableRightData == 1)
                {
                    tabdataMasterCompStep2R();
                }
                if (ActiveMasterTableRightData == 2)
                {
                    tabdataMasterExtnStep2R();
                }
                if (ActiveMasterTableRightData == 3)
                {
                    tabdataMasterDiffStep2R();
                }
            }
        }

        private void button55_Click(object sender, EventArgs e)
        {
            tabdataRealCompStep2L();
            button55.BackColor = Color.Cyan;
            button54.BackColor = Color.LightSteelBlue;
            button71.BackColor = Color.LightSteelBlue;
        }

        private void button54_Click(object sender, EventArgs e)
        {
            tabdataRealExtnStep2L();
            button55.BackColor = Color.LightSteelBlue;
            button54.BackColor = Color.Cyan;
            button71.BackColor = Color.LightSteelBlue;
        }

        private void button71_Click(object sender, EventArgs e)
        {
            tabdataRealDiffStep2L();
            button55.BackColor = Color.LightSteelBlue;
            button54.BackColor = Color.LightSteelBlue;
            button71.BackColor = Color.Cyan;
        }

        private void button53_Click(object sender, EventArgs e)
        {
            tabdataRealCompStep2R();
            button53.BackColor = Color.Cyan;
            button52.BackColor = Color.LightSteelBlue;
            button70.BackColor = Color.LightSteelBlue;
        }

        private void button52_Click(object sender, EventArgs e)
        {
            tabdataRealExtnStep2R();
            button53.BackColor = Color.LightSteelBlue;
            button52.BackColor = Color.Cyan;
            button70.BackColor = Color.LightSteelBlue;
        }

        private void button70_Click(object sender, EventArgs e)
        {
            tabdataRealDiffStep2R();
            button53.BackColor = Color.LightSteelBlue;
            button52.BackColor = Color.LightSteelBlue;
            button70.BackColor = Color.Cyan;
        }

        private void button59_Click(object sender, EventArgs e)
        {
            tabdataMasterCompStep2L();
            button59.BackColor = Color.Cyan;
            button58.BackColor = Color.LightSteelBlue;
            button65.BackColor = Color.LightSteelBlue;
        }

        private void button58_Click(object sender, EventArgs e)
        {
            tabdataMasterExtnStep2L();
            button59.BackColor = Color.LightSteelBlue;
            button58.BackColor = Color.Cyan;
            button65.BackColor = Color.LightSteelBlue;
        }

        private void button65_Click(object sender, EventArgs e)
        {
            tabdataMasterDiffStep2L();
            button59.BackColor = Color.LightSteelBlue;
            button58.BackColor = Color.LightSteelBlue;
            button65.BackColor = Color.Cyan;
        }

        private void button57_Click(object sender, EventArgs e)
        {
            tabdataMasterCompStep2R();
            button57.BackColor = Color.Cyan;
            button56.BackColor = Color.LightSteelBlue;
            button64.BackColor = Color.LightSteelBlue;
        }

        private void button56_Click(object sender, EventArgs e)
        {
            tabdataMasterExtnStep2R();
            button57.BackColor = Color.LightSteelBlue;
            button56.BackColor = Color.Cyan;
            button64.BackColor = Color.LightSteelBlue;
        }

        private void button64_Click(object sender, EventArgs e)
        {
            tabdataMasterDiffStep2R();
            button57.BackColor = Color.LightSteelBlue;
            button56.BackColor = Color.LightSteelBlue;
            button64.BackColor = Color.Cyan;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            float.TryParse(textBox3.Text, out float value);
            MasterTeachOffsetBatchL = value;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            float.TryParse(textBox4.Text, out float value);
            MasterTeachOffsetBatchR = value;
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            float.TryParse(textBox6.Text, out float value);
            MasterTeachDiffOffsetBatchL = value;
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            float.TryParse(textBox5.Text, out float value);
            MasterTeachDiffOffsetBatchR = value;
        }

        private void button39_Click_1(object sender, EventArgs e)
        {
            tabdataMasterTeachSet();

            tabIdxMasterSideL = 0;
            if (ActiveMasterTableLeftData == 1)
            {
                tabdataMasterCompStep2L();
            }
            if (ActiveMasterTableLeftData == 2)
            {
                tabdataMasterExtnStep2L();
            }
            if (ActiveMasterTableLeftData == 3)
            {
                tabdataMasterDiffStep2L();
            }
            tabIdxMasterSideR = 0;
            if (ActiveMasterTableRightData == 1)
            {
                tabdataMasterCompStep2R();
            }
            if (ActiveMasterTableRightData == 2)
            {
                tabdataMasterExtnStep2R();
            }
            if (ActiveMasterTableRightData == 3)
            {
                tabdataMasterDiffStep2R();
            }

            MasterTeachSetConfirm = true;
            updateMasterReset();
            updateTeachingSet();
        }

        private void button31_Click(object sender, EventArgs e)
        {
            if (ActiveMasterTableLeftData == 1)
            {
                tabdataMasterCompStep2LW();
                tabdataMasterCompStep2L();
            }
            if (ActiveMasterTableLeftData == 2)
            {
                tabdataMasterExtnStep2LW();
                tabdataMasterExtnStep2L();
            }
            if (ActiveMasterTableLeftData == 3)
            {
                tabdataMasterDiffStep2LW();
                tabdataMasterDiffStep2L();
            }

            if (ActiveMasterTableRightData == 1)
            {
                tabdataMasterCompStep2RW();
                tabdataMasterCompStep2R();
            }
            if (ActiveMasterTableRightData == 2)
            {
                tabdataMasterExtnStep2RW();
                tabdataMasterExtnStep2R();
            }
            if (ActiveMasterTableRightData == 3)
            {
                tabdataMasterDiffStep2RW();
                tabdataMasterDiffStep2R();
            }

            updateMasterReset();
            confirmEditSet();
        }



        private void button30_Click(object sender, EventArgs e)
        {
            updateMasterProcess = true;
            if (updateMasterProcess)
            {
                button30.BackColor = Color.Tomato;
            }
            //UPDATE MASTER DATA
            if (MasterTeachSetConfirm)
            {

                //MasterTeachSetConfirm = false;
            }

            _WorkflowHandler.MasterUpdatingDatabaseSet();
            _WorkflowHandler.workUpdateMasterData();
            _WorkflowHandler.workUpdateMasterDatabase();

            updateMasterProcess = false;

            if (!updateMasterProcess)
            {

            }

            Thread.Sleep(1000);

            button30.BackColor = Color.DodgerBlue;

        }


        private void button80_Click(object sender, EventArgs e)
        {
            validationMasterProcess = true;
            if (validationMasterProcess)
            {
                button80.BackColor = Color.Tomato;
            }

            //VALIDATE MASTER DATA
            if (!_WorkflowHandler.MasterIsUpdatingDatabase())
            {
                _WorkflowHandler.workMasterValidation();
                MasterTeachSetConfirm = false;
            }
            validationMasterProcess = false;

            Thread.Sleep(1000);

            if (!validationMasterProcess)
            {
                button80.BackColor = Color.LightSteelBlue;
            }
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }
        private void button15_Click(object sender, EventArgs e)
        {
            RealtimeMasterLineShowHide();
        }
        private void button16_Click(object sender, EventArgs e)
        {
            RealtimeMasterLimitShowHide();
        }
        private void button27_Click(object sender, EventArgs e)
        {
            TeachingMasterLineShowHide();
        }
        private void button26_Click(object sender, EventArgs e)
        {
            TeachingMasterLimitShowHide();
        }
        private void button81_Click(object sender, EventArgs e)
        {
            _WorkflowHandler.uiReloadTeachingData();
        }
        private void button82_Click(object sender, EventArgs e)
        {
            _WorkflowHandler.uiReloadRealtimeData();
        }
        private void button21_Click_1(object sender, EventArgs e)
        {
            string DirRealtime = _WorkflowHandler.RealLogDir + $"YEAR_{dateTimePicker1.Value.Year}\\MONTH_{dateTimePicker1.Value.Month}\\DAY_{dateTimePicker1.Value.Day}";
            CheckFolderPath(DirRealtime);
            string sourceDir = DirRealtime;

            if (string.IsNullOrWhiteSpace(sourceDir))
            {
                MessageBox.Show("Please enter a source directory.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Directory.Exists(sourceDir))
            {
                MessageBox.Show("The source directory does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Show the FolderBrowserDialog to select the target directory
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string destDir = this.folderBrowserDialog.SelectedPath;

                try
                {
                    CopyDirectory(sourceDir, destDir);
                    MessageBox.Show($"Directory copied successfully from {sourceDir} to {destDir}.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while copying the directory: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button22_Click(object sender, EventArgs e)
        {
            string DirMaster = _WorkflowHandler.MasterDir;
            CheckFolderPath(DirMaster);
            string sourceDir = DirMaster;

            if (string.IsNullOrWhiteSpace(sourceDir))
            {
                MessageBox.Show("Please enter a source directory.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Directory.Exists(sourceDir))
            {
                MessageBox.Show("The source directory does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Show the FolderBrowserDialog to select the target directory
            if (this.folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string destDir = this.folderBrowserDialog.SelectedPath;

                try
                {
                    CopyDirectory(sourceDir, destDir);
                    MessageBox.Show($"Directory copied successfully from {sourceDir} to {destDir}.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while copying the directory: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button23_Click(object sender, EventArgs e)
        {
            string selectedFile;
            string DirMaster = _WorkflowHandler.MasterDir;
            CheckFolderPath(DirMaster);
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                selectedFile = openFileDialog.FileName;
                if (string.IsNullOrWhiteSpace(selectedFile))
                {
                    MessageBox.Show("Please select a file to save.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!File.Exists(selectedFile))
                {
                    MessageBox.Show("The selected file does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                try
                {
                    string fileName = Path.GetFileName(selectedFile);
                    string destFilePath = Path.Combine(DirMaster, fileName);

                    // Ensure the destination directory exists
                    if (!Directory.Exists(DirMaster))
                    {
                        Directory.CreateDirectory(DirMaster);
                    }

                    // Copy the file to the destination directory, overwriting if necessary
                    File.Copy(selectedFile, destFilePath, true);

                    MessageBox.Show($"File saved successfully to {destFilePath}.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while saving the file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button83_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void button86_Click(object sender, EventArgs e)
        {
            try
            {
                Pixel point = new Pixel(600, 120);
                MouseAxisManipulation.MouseWheelZoom(FormPlot1().Plot, 1.10, 1.10, point, false);
                FormPlot1().Refresh();
            }
            catch { }
        }

        private void button85_Click(object sender, EventArgs e)
        {
            try
            {
                Pixel point = new Pixel(600, 120);
                MouseAxisManipulation.MouseWheelZoom(FormPlot1().Plot, (1 / 1.10), (1 / 1.10), point, false);
                FormPlot1().Refresh();
            }
            catch { }

        }

        private void button88_Click(object sender, EventArgs e)
        {
            try
            {
                Pixel point = new Pixel(600, 120);
                MouseAxisManipulation.MouseWheelZoom(FormPlot2().Plot, 1.10, 1.10, point, false);
                FormPlot2().Refresh();
            }
            catch { }

        }

        private void button87_Click(object sender, EventArgs e)
        {
            try
            {
                Pixel point = new Pixel(600, 120);
                MouseAxisManipulation.MouseWheelZoom(FormPlot2().Plot, (1 / 1.10), (1 / 1.10), point, false);
                FormPlot2().Refresh();
            }
            catch { }

        }

        private void button90_Click(object sender, EventArgs e)
        {
            try
            {
                Pixel point = new Pixel(600, 120);
                MouseAxisManipulation.MouseWheelZoom(FormPlot9().Plot, 1.10, 1.10, point, false);
                FormPlot9().Refresh();
            }
            catch { }

        }

        private void button89_Click(object sender, EventArgs e)
        {
            try
            {
                Pixel point = new Pixel(600, 120);
                MouseAxisManipulation.MouseWheelZoom(FormPlot9().Plot, (1 / 1.10), (1 / 1.10), point, false);
                FormPlot9().Refresh();
            }
            catch { }

        }

        private void button92_Click(object sender, EventArgs e)
        {
            try
            {
                Pixel point = new Pixel(600, 120);
                MouseAxisManipulation.MouseWheelZoom(FormPlot3().Plot, 1.10, 1.10, point, false);
                FormPlot3().Refresh();
            }
            catch { }

        }

        private void button91_Click(object sender, EventArgs e)
        {
            try
            {
                Pixel point = new Pixel(600, 120);
                MouseAxisManipulation.MouseWheelZoom(FormPlot3().Plot, (1 / 1.10), (1 / 1.10), point, false);
                FormPlot3().Refresh();
            }
            catch { }

        }

        private void button94_Click(object sender, EventArgs e)
        {
            try
            {
                Pixel point = new Pixel(600, 120);
                MouseAxisManipulation.MouseWheelZoom(FormPlot4().Plot, 1.10, 1.10, point, false);
                FormPlot4().Refresh();
            }
            catch { }

        }

        private void button93_Click(object sender, EventArgs e)
        {
            try
            {
                Pixel point = new Pixel(600, 120);
                MouseAxisManipulation.MouseWheelZoom(FormPlot4().Plot, (1 / 1.10), (1 / 1.10), point, false);
                FormPlot4().Refresh();
            }
            catch { }

        }

        private void button96_Click(object sender, EventArgs e)
        {
            try
            {
                Pixel point = new Pixel(600, 120);
                MouseAxisManipulation.MouseWheelZoom(FormPlot10().Plot, 1.10, 1.10, point, false);
                FormPlot10().Refresh();
            }
            catch { }

        }

        private void button95_Click(object sender, EventArgs e)
        {
            try
            {
                Pixel point = new Pixel(600, 120);
                MouseAxisManipulation.MouseWheelZoom(FormPlot10().Plot, (1 / 1.10), (1 / 1.10), point, false);
                FormPlot10().Refresh();
            }
            catch { }

        }

        private void button98_Click(object sender, EventArgs e)
        {
            try
            {
                Pixel point = new Pixel(600, 120);
                MouseAxisManipulation.MouseWheelZoom(FormPlot5().Plot, 1.10, 1.10, point, false);
                FormPlot5().Refresh();
            }
            catch { }

        }

        private void button97_Click(object sender, EventArgs e)
        {
            try
            {
                Pixel point = new Pixel(600, 120);
                MouseAxisManipulation.MouseWheelZoom(FormPlot5().Plot, (1 / 1.10), (1 / 1.10), point, false);
                FormPlot5().Refresh();
            }
            catch { }

        }

        private void button100_Click(object sender, EventArgs e)
        {
            try
            {
                Pixel point = new Pixel(600, 120);
                MouseAxisManipulation.MouseWheelZoom(FormPlot6().Plot, 1.10, 1.10, point, false);
                FormPlot6().Refresh();
            }
            catch { }

        }

        private void button99_Click(object sender, EventArgs e)
        {
            try
            {
                Pixel point = new Pixel(600, 120);
                MouseAxisManipulation.MouseWheelZoom(FormPlot6().Plot, (1 / 1.10), (1 / 1.10), point, false);
                FormPlot6().Refresh();
            }
            catch { }

        }

        private void button102_Click(object sender, EventArgs e)
        {
            try
            {
                Pixel point = new Pixel(600, 120);
                MouseAxisManipulation.MouseWheelZoom(FormPlot11().Plot, 1.10, 1.10, point, false);
                FormPlot11().Refresh();
            }
            catch { }

        }

        private void button101_Click(object sender, EventArgs e)
        {
            try
            {
                Pixel point = new Pixel(600, 120);
                MouseAxisManipulation.MouseWheelZoom(FormPlot11().Plot, (1 / 1.10), (1 / 1.10), point, false);
                FormPlot11().Refresh();
            }
            catch { }

        }

        private void button104_Click(object sender, EventArgs e)
        {
            try
            {
                Pixel point = new Pixel(600, 120);
                MouseAxisManipulation.MouseWheelZoom(FormPlot7().Plot, 1.10, 1.10, point, false);
                FormPlot7().Refresh();
            }
            catch { }

        }

        private void button103_Click(object sender, EventArgs e)
        {
            try
            {
                Pixel point = new Pixel(600, 120);
                MouseAxisManipulation.MouseWheelZoom(FormPlot7().Plot, (1 / 1.10), (1 / 1.10), point, false);
                FormPlot7().Refresh();
            }
            catch { }

        }

        private void button106_Click(object sender, EventArgs e)
        {
            try
            {
                Pixel point = new Pixel(600, 120);
                MouseAxisManipulation.MouseWheelZoom(FormPlot8().Plot, 1.10, 1.10, point, false);
                FormPlot8().Refresh();
            }
            catch { }

        }

        private void button105_Click(object sender, EventArgs e)
        {
            try
            {
                Pixel point = new Pixel(600, 120);
                MouseAxisManipulation.MouseWheelZoom(FormPlot8().Plot, (1 / 1.10), (1 / 1.10), point, false);
                FormPlot8().Refresh();
            }
            catch { }

        }

        private void button108_Click(object sender, EventArgs e)
        {
            try
            {
                Pixel point = new Pixel(600, 120);
                MouseAxisManipulation.MouseWheelZoom(FormPlot12().Plot, 1.10, 1.10, point, false);
                FormPlot12().Refresh();
            }
            catch { }

        }

        private void button107_Click(object sender, EventArgs e)
        {
            try
            {
                Pixel point = new Pixel(600, 120);
                MouseAxisManipulation.MouseWheelZoom(FormPlot12().Plot, (1 / 1.10), (1 / 1.10), point, false);
                FormPlot12().Refresh();
            }
            catch { }

        }

        #endregion

        private void tabPage5_Click(object sender, EventArgs e)
        {

        }

        #region UI JUDGEMENT STATUS

        public void Judge_Left_Lamp(int value)
        {
            P_NG_L.Visible = true;
            if (value < 1)
            {
                P_NG_L.Text = "OK";
                P_NG_L.BackColor = Color.LimeGreen;
                P_NG_L.ForeColor = Color.Black;
            }
            else
            {
                P_NG_L.Text = "NG";
                P_NG_L.BackColor = Color.Red;
                P_NG_L.ForeColor = Color.Black;
            }

        }

        public void PreStroke_Left_Value(float value, int judge)
        {
            PRE_LLOAD.Visible = true;
            PRE_LLOAD.Text = $"Load: {value:0.##} N";
            if (judge < 1)
            {
                PRE_LLOAD.BackColor = Color.ForestGreen;
            }
            else
            {
                PRE_LLOAD.BackColor = Color.Crimson;
            }
        }

        public void Step2_Left_CompressValue(float value, int judge)
        {
            STEP2_CMPLLOAD.Visible = true;
            STEP2_CMPLLOAD.Text = $"Load: {value:0.##} N";
            if (judge < 1)
            {
                STEP2_CMPLLOAD.BackColor = Color.ForestGreen;
            }
            else
            {
                STEP2_CMPLLOAD.BackColor = Color.Crimson;
            }
        }

        public void Step2_Left_CompressLowLimit(float value)
        {
            STEP2_CMPLLOAD_LO.Visible = true;
            STEP2_CMPLLOAD_LO.Text = $"> {value:0.##} N";
        }

        public void Step2_Left_CompressHiLimit(float value)
        {
            STEP2_CMPLLOAD_HI.Visible = true;
            STEP2_CMPLLOAD_HI.Text = $"< {value:0.##} N";
        }

        public void Step2_Left_ExtensValue(float value, int judge)
        {
            STEP2_EXTLLOAD.Visible = true;
            STEP2_EXTLLOAD.Text = $"Load: {value:0.##} N";
            if (judge < 1)
            {
                STEP2_EXTLLOAD.BackColor = Color.ForestGreen;
            }
            else
            {
                STEP2_EXTLLOAD.BackColor = Color.Crimson;
            }
        }

        public void Step2_Left_ExtensLowLimit(float value)
        {
            STEP2_EXTLLOAD_LO.Visible = true;
            STEP2_EXTLLOAD_LO.Text = $"> {value:0.##} N";
        }

        public void Step2_Left_ExtensHiLimit(float value)
        {
            STEP2_EXTLLOAD_HI.Visible = true;
            STEP2_EXTLLOAD_HI.Text = $"< {value:0.##} N";
        }

        public void Plot_LeftComp_GraphJudge(int judge, float yoffset)
        {
            JudgePlot1.OffsetY = -10f * yoffset;
            JudgePlot1.IsVisible = true;
            if (judge < 1)
            {
                JudgePlot1.LabelText = "OK";
                JudgePlot1.LabelFontColor = Colors.Black;
                JudgePlot1.LabelBackgroundColor = Colors.LimeGreen;
            }
            else
            {
                JudgePlot1.LabelText = "NG";
                JudgePlot1.LabelFontColor = Colors.White;
                JudgePlot1.LabelBackgroundColor = Colors.Crimson;
            }
        }
        public void Plot_LeftExt_GraphJudge(int judge, float yoffset)
        {
            JudgePlot2.OffsetY = -10f * yoffset;
            JudgePlot2.IsVisible = true;
            if (judge < 1)
            {
                JudgePlot2.LabelText = "OK";
                JudgePlot2.LabelFontColor = Colors.Black;
                JudgePlot2.LabelBackgroundColor = Colors.LimeGreen;
            }
            else
            {
                JudgePlot2.LabelText = "NG";
                JudgePlot2.LabelFontColor = Colors.White;
                JudgePlot2.LabelBackgroundColor = Colors.Crimson;
            }
        }
        public void Plot_LeftDiff_GraphJudge(int judge, float yoffset)
        {
            JudgePlot9.OffsetY = -10f * yoffset;
            JudgePlot9.IsVisible = true;
            if (judge < 1)
            {
                JudgePlot9.LabelText = "OK";
                JudgePlot9.LabelFontColor = Colors.Black;
                JudgePlot9.LabelBackgroundColor = Colors.LimeGreen;
            }
            else
            {
                JudgePlot9.LabelText = "NG";
                JudgePlot9.LabelFontColor = Colors.White;
                JudgePlot9.LabelBackgroundColor = Colors.Crimson;
            }
        }

        //-------------------------------------------------------------

        public void Judge_Right_Lamp(int value)
        {
            P_NG_R.Visible = true;
            if (value < 1)
            {
                P_NG_R.Text = "OK";
                P_NG_R.BackColor = Color.LimeGreen;
                P_NG_R.ForeColor = Color.Black;
            }
            else
            {
                P_NG_R.Text = "NG";
                P_NG_R.BackColor = Color.Red;
                P_NG_R.ForeColor = Color.Black;
            }

        }

        public void PreStroke_Right_Value(float value, int judge)
        {
            PRE_RLOAD.Visible = true;
            PRE_RLOAD.Text = $"Load: {value:0.##} N";
            if (judge < 1)
            {
                PRE_RLOAD.BackColor = Color.ForestGreen;
            }
            else
            {
                PRE_RLOAD.BackColor = Color.Crimson;
            }
        }

        public void Step2_Right_CompressValue(float value, int judge)
        {
            STEP2_CMPRLOAD.Visible = true;
            STEP2_CMPRLOAD.Text = $"Load: {value:0.##} N";
            if (judge < 1)
            {
                STEP2_CMPRLOAD.BackColor = Color.ForestGreen;
            }
            else
            {
                STEP2_CMPRLOAD.BackColor = Color.Crimson;
            }
        }

        public void Step2_Right_CompressLowLimit(float value)
        {
            STEP2_CMPRLOAD_LO.Visible = true;
            STEP2_CMPRLOAD_LO.Text = $"> {value:0.##} N";
        }

        public void Step2_Right_CompressHiLimit(float value)
        {
            STEP2_CMPRLOAD_HI.Visible = true;
            STEP2_CMPRLOAD_HI.Text = $"< {value:0.##} N";
        }

        public void Step2_Right_ExtensValue(float value, int judge)
        {
            STEP2_EXTRLOAD.Visible = true;
            STEP2_EXTRLOAD.Text = $"Load: {value:0.##} N";
            if (judge < 1)
            {
                STEP2_EXTRLOAD.BackColor = Color.ForestGreen;
            }
            else
            {
                STEP2_EXTRLOAD.BackColor = Color.Crimson;
            }
        }

        public void Step2_Right_ExtensLowLimit(float value)
        {
            STEP2_EXTRLOAD_LO.Visible = true;
            STEP2_EXTRLOAD_LO.Text = $"> {value:0.##} N";
        }

        public void Step2_Right_ExtensHiLimit(float value)
        {
            STEP2_EXTRLOAD_HI.Visible = true;
            STEP2_EXTRLOAD_HI.Text = $"< {value:0.##} N";
        }

        public void Plot_RightComp_GraphJudge(int judge, float yoffset)
        {
            JudgePlot3.OffsetY = -10f * yoffset;
            JudgePlot3.IsVisible = true;
            if (judge < 1)
            {
                JudgePlot3.LabelText = "OK";
                JudgePlot3.LabelFontColor = Colors.Black;
                JudgePlot3.LabelBackgroundColor = Colors.LimeGreen;
            }
            else
            {
                JudgePlot3.LabelText = "NG";
                JudgePlot3.LabelFontColor = Colors.White;
                JudgePlot3.LabelBackgroundColor = Colors.Crimson;
            }
        }
        public void Plot_RightExt_GraphJudge(int judge, float yoffset)
        {
            JudgePlot4.OffsetY = -10f * yoffset;
            JudgePlot4.IsVisible = true;
            if (judge < 1)
            {
                JudgePlot4.LabelText = "OK";
                JudgePlot4.LabelFontColor = Colors.Black;
                JudgePlot4.LabelBackgroundColor = Colors.LimeGreen;
            }
            else
            {
                JudgePlot4.LabelText = "NG";
                JudgePlot4.LabelFontColor = Colors.White;
                JudgePlot4.LabelBackgroundColor = Colors.Crimson;
            }
        }
        public void Plot_RightDiff_GraphJudge(int judge, float yoffset)
        {
            JudgePlot10.OffsetY = -10f * yoffset;
            JudgePlot10.IsVisible = true;
            if (judge < 1)
            {
                JudgePlot10.LabelText = "OK";
                JudgePlot10.LabelFontColor = Colors.Black;
                JudgePlot10.LabelBackgroundColor = Colors.LimeGreen;
            }
            else
            {
                JudgePlot10.LabelText = "NG";
                JudgePlot10.LabelFontColor = Colors.White;
                JudgePlot10.LabelBackgroundColor = Colors.Crimson;
            }
        }

        #endregion
    }

    #region supporting classes
    public class CustomTableLayoutPanel : TableLayoutPanel
    {
        public Color CellBorderColor { get; set; }

        public CustomTableLayoutPanel()
        {
            CellBorderColor = Color.Black; // Default border color
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            using (Pen pen = new Pen(CellBorderColor))
            {
                for (int row = 0; row < RowCount; row++)
                {
                    for (int col = 0; col < ColumnCount; col++)
                    {
                        DRW.Rectangle cellBounds = GetCellBounds(row, col);
                        e.Graphics.DrawRectangle(pen, cellBounds);
                    }
                }
            }
        }

        private DRW.Rectangle GetCellBounds(int row, int col)
        {
            DRW.Rectangle result = new DRW.Rectangle();

            if (ColumnStyles.Count > col && RowStyles.Count > row)
            {
                for (int i = 0; i < col; i++)
                {
                    result.X += (int)(ColumnStyles[i].Width / 100F * ClientSize.Width);
                }

                for (int j = 0; j < row; j++)
                {
                    result.Y += (int)(RowStyles[j].Height / 100F * ClientSize.Height);
                }

                result.Width = (int)(ColumnStyles[col].Width / 100F * ClientSize.Width);
                result.Height = (int)(RowStyles[row].Height / 100F * ClientSize.Height);
            }

            return result;
        }


    }
    public static class ArrayExtensions
    {
        // Extension method to support lambda with index
        public static void ForEach<T>(this T[] array, Action<T, int> action)
        {
            for (int i = 0; i < array.Length; i++)
            {
                action(array[i], i);
            }
        }

        // Extension method to convert array to list using a lambda expression
        public static List<T> ToList<T>(this T[] array, Func<T, T> converter)
        {
            List<T> list = new List<T>(array.Length);
            array.ForEach((item, index) => list.Add(converter(item)));
            return list;
        }
    }
    public static class MouseEventSimulator
    {
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_LBUTTONUP = 0x0202;
        private const int WM_MOUSEMOVE = 0x0200;
        private const int WM_MOUSEWHEEL = 0x020A;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        public static void SimulateMouseClick(Control control, int x, int y)
        {
            IntPtr lParam = (IntPtr)((y << 16) | x);
            SendMessage(control.Handle, WM_LBUTTONDOWN, IntPtr.Zero, lParam);
            SendMessage(control.Handle, WM_LBUTTONUP, IntPtr.Zero, lParam);
        }

        public static void SimulateMouseMove(Control control, int x, int y)
        {
            IntPtr lParam = (IntPtr)((y << 16) | x);
            SendMessage(control.Handle, WM_MOUSEMOVE, IntPtr.Zero, lParam);
        }

        public static void SimulateMouseWheel(Control control, int delta, int x, int y)
        {
            IntPtr wParam = (IntPtr)((delta << 16) & 0xFFFF0000);
            IntPtr lParam = (IntPtr)((y << 16) | x);
            SendMessage(control.Handle, WM_MOUSEWHEEL, wParam, lParam);
        }

        public static void SimulateMouseDrag(Control control, int startX, int startY, int endX, int endY)
        {
            IntPtr lParamStart = (IntPtr)((startY << 16) | startX);
            IntPtr lParamEnd = (IntPtr)((endY << 16) | endX);

            // Send the WM_LBUTTONDOWN message to simulate pressing the left mouse button
            SendMessage(control.Handle, WM_LBUTTONDOWN, IntPtr.Zero, lParamStart);

            // Send the WM_MOUSEMOVE message to simulate moving the mouse to the end position
            SendMessage(control.Handle, WM_MOUSEMOVE, IntPtr.Zero, lParamEnd);

            // Send the WM_LBUTTONUP message to simulate releasing the left mouse button
            SendMessage(control.Handle, WM_LBUTTONUP, IntPtr.Zero, lParamEnd);
        }

        public static void SetMousePosition(int x, int y)
        {
            SetCursorPos(x, y);
        }
    }
    class MouseMoveEventHandler
    {
        private readonly ScottPlot.Plottables.Crosshair _plotCross;
        private readonly Label _label;

        public MouseMoveEventHandler(ScottPlot.Plottables.Crosshair plotCross, Label label)
        {
            _plotCross = plotCross;
            _label = label;
        }

        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            var plotBase = sender as FormsPlot;
            if (plotBase == null) return;

            // Determine where the mouse is and get the nearest point
            Pixel mousePixel = new(e.Location.X, e.Location.Y);
            Coordinates mouseLocation = plotBase.Plot.GetCoordinates(mousePixel);
            AxisLimits limits = plotBase.Plot.Axes.GetLimits();
            // Place the crosshair over the highlighted point
            if (mouseLocation.X >= limits.Left && mouseLocation.X <= limits.Right && mouseLocation.Y >= limits.Bottom && mouseLocation.Y <= limits.Top)
            {
                _plotCross.IsVisible = true;
                _plotCross.Position = mouseLocation;
                plotBase.Refresh();
                _label.Text = $"X = {mouseLocation.X:0000.##}, Y = {mouseLocation.Y:0000.##}";
            }
            else
            {
                _plotCross.IsVisible = false;
                plotBase.Refresh();
                _label.Text = $"X = 0000, Y = 0000.00";
            }
        }
    }

    #endregion
}


namespace ScottPlot.WinForms
{


}

