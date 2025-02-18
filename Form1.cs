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

using Control = System.Windows.Forms.Control;

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

using DRW = System.Drawing;
using Size = System.Drawing.Size;
using Point = System.Drawing.Point;
using Label = System.Windows.Forms.Label;
using DataTable = System.Data.DataTable;

using LIBKVPROTOCOL;
using WORKFLOW;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using ScottPlot.DataSources;
using DocumentFormat.OpenXml.Drawing.Charts;
using ScottPlot.WinForms;
using System.Reflection.Emit;
using ClosedXML.Report.Utils;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

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
        private CancellationTokenSource _cts;

        public string settingIpv4;
        public int settingPortIp;
        public string msgToBeSent;
        public int _connStat;
        public int _beaconn;

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
        private void InitializeCustomComponents()
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
        private void InitializeBorderComponent()
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
                Font = new System.Drawing.Font("Sarasa Fixed J", 10, FontStyle.Bold),
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
                Font = new System.Drawing.Font("Sarasa Fixed J", 10, FontStyle.Bold),
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
                Font = new System.Drawing.Font("Sarasa Fixed J", 10, FontStyle.Bold),
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
                Font = new System.Drawing.Font("Sarasa Fixed J", 10, FontStyle.Bold),
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
                Font = new System.Drawing.Font("Sarasa Fixed J", 10, FontStyle.Bold),
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
                        Font = new System.Drawing.Font("Sarasa Fixed J", 10, FontStyle.Bold),
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
                    Font = new System.Drawing.Font("Sarasa Fixed J", 10, FontStyle.Bold),
                    Size = new Size(80, 24),
                    ReadOnly = true,

                };
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
                    Font = new System.Drawing.Font("Sarasa Fixed J", 10, FontStyle.Bold),
                    Size = new Size(80, 24),
                    ReadOnly = true,

                };
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
                    Font = new System.Drawing.Font("Sarasa Fixed J", 10, FontStyle.Bold),
                    Size = new Size(80, 24),
                    ReadOnly = true,

                };
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
                    Font = new System.Drawing.Font("Sarasa Fixed J", 10, FontStyle.Bold),
                    Size = new Size(80, 24),
                    ReadOnly = true,

                };
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
                Font = new System.Drawing.Font("Sarasa Fixed J", 10, FontStyle.Bold),
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
                Font = new System.Drawing.Font("Sarasa Fixed J", 10, FontStyle.Bold),
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
                Font = new System.Drawing.Font("Sarasa Fixed J", 10, FontStyle.Bold),
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
                Font = new System.Drawing.Font("Sarasa Fixed J", 10, FontStyle.Bold),
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
                Font = new System.Drawing.Font("Sarasa Fixed J", 10, FontStyle.Bold),
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
                        Font = new System.Drawing.Font("Sarasa Fixed J", 10, FontStyle.Bold),
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
                    Font = new System.Drawing.Font("Sarasa Fixed J", 10, FontStyle.Bold),
                    Size = new Size(80, 24),
                    ReadOnly = true,


                };
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
                    Font = new System.Drawing.Font("Sarasa Fixed J", 10, FontStyle.Bold),
                    Size = new Size(80, 24),
                    ReadOnly = true,

                };
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
                    Font = new System.Drawing.Font("Sarasa Fixed J", 10, FontStyle.Bold),
                    Size = new Size(80, 24),
                    ReadOnly = true,

                };
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
                    Font = new System.Drawing.Font("Sarasa Fixed J", 10, FontStyle.Bold),
                    Size = new Size(80, 24),
                    ReadOnly = true,

                };
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
                Font = new System.Drawing.Font("Sarasa Fixed J", 10, FontStyle.Bold),
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
                Font = new System.Drawing.Font("Sarasa Fixed J", 10, FontStyle.Bold),
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
                Font = new System.Drawing.Font("Sarasa Fixed J", 10, FontStyle.Bold),
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
                Font = new System.Drawing.Font("Sarasa Fixed J", 10, FontStyle.Bold),
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
                Font = new System.Drawing.Font("Sarasa Fixed J", 10, FontStyle.Bold),
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
                        Font = new System.Drawing.Font("Sarasa Fixed J", 10, FontStyle.Bold),
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
                    Font = new System.Drawing.Font("Sarasa Fixed J", 10, FontStyle.Bold),
                    Size = new Size(80, 24),

                };
                tbx.Click += new EventHandler(InputTextBox_Click);
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
                    Font = new System.Drawing.Font("Sarasa Fixed J", 10, FontStyle.Bold),
                    Size = new Size(80, 24),

                };
                tbx.Click += new EventHandler(InputTextBox_Click);
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
                    Font = new System.Drawing.Font("Sarasa Fixed J", 10, FontStyle.Bold),
                    Size = new Size(80, 24),

                };
                tbx.Click += new EventHandler(InputTextBox_Click);
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
                    Font = new System.Drawing.Font("Sarasa Fixed J", 10, FontStyle.Bold),
                    Size = new Size(80, 24),

                };
                tbx.Click += new EventHandler(InputTextBox_Click);
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
                Font = new System.Drawing.Font("Sarasa Fixed J", 10, FontStyle.Bold),
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
                Font = new System.Drawing.Font("Sarasa Fixed J", 10, FontStyle.Bold),
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
                Font = new System.Drawing.Font("Sarasa Fixed J", 10, FontStyle.Bold),
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
                Font = new System.Drawing.Font("Sarasa Fixed J", 10, FontStyle.Bold),
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
                Font = new System.Drawing.Font("Sarasa Fixed J", 10, FontStyle.Bold),
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
                        Font = new System.Drawing.Font("Sarasa Fixed J", 10, FontStyle.Bold),
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
                    Font = new System.Drawing.Font("Sarasa Fixed J", 10, FontStyle.Bold),
                    Size = new Size(80, 24),
                    ReadOnly = true,


                };
                tbx.Click += new EventHandler(InputTextBox_Click);
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
                    Font = new System.Drawing.Font("Sarasa Fixed J", 10, FontStyle.Bold),
                    Size = new Size(80, 24),
                    ReadOnly = true,

                };
                tbx.Click += new EventHandler(InputTextBox_Click);
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
                    Font = new System.Drawing.Font("Sarasa Fixed J", 10, FontStyle.Bold),
                    Size = new Size(80, 24),
                    ReadOnly = true,

                };
                tbx.Click += new EventHandler(InputTextBox_Click);
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
                    Font = new System.Drawing.Font("Sarasa Fixed J", 10, FontStyle.Bold),
                    Size = new Size(80, 24),
                    ReadOnly = true,

                };
                tbx.Click += new EventHandler(InputTextBox_Click);
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
                tabRealSideLMaster[i].Text = _dataRealCompSideLMaster[i + tabIndex].ToString();
                #endregion

                #region Lower
                tabRealSideLLower[i].Text = (_dataRealCompSideLLower[i + tabIndex]).ToString();
                #endregion

                #region Realtime
                tabRealSideLReal[i].Text = _dataRealCompSideLLoad[i + tabIndex].ToString();
                if (_dataRealCompSideLLoad[i + tabIndex] > _dataRealCompSideLUpper[i + tabIndex] | _dataRealCompSideLLoad[i + tabIndex] < _dataRealCompSideLLower[i + tabIndex])
                {
                    tabRealSideLReal[i].BackColor = Color.Red;
                }
                else
                {
                    tabRealSideLReal[i].BackColor = Color.White;
                }
                #endregion

                #region Upper
                tabRealSideLUpper[i].Text = (_dataRealCompSideLUpper[i + tabIndex]).ToString();
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
                tabRealSideLMaster[i].Text = _dataRealExtnSideLMaster[i + tabIndex].ToString();
                #endregion

                #region Lower
                tabRealSideLLower[i].Text = (_dataRealExtnSideLLower[i + tabIndex]).ToString();
                #endregion

                #region Realtime
                tabRealSideLReal[i].Text = _dataRealExtnSideLLoad[i + tabIndex].ToString();
                if (_dataRealExtnSideLLoad[i + tabIndex] > _dataRealExtnSideLUpper[i + tabIndex] || _dataRealExtnSideLLoad[i + tabIndex] < _dataRealExtnSideLLower[i + tabIndex])
                {
                    tabRealSideLReal[i].BackColor = Color.Red;
                }
                else
                {
                    tabRealSideLReal[i].BackColor = Color.White;
                }
                #endregion

                #region Upper
                tabRealSideLUpper[i].Text = (_dataRealExtnSideLUpper[i + tabIndex]).ToString();
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
                tabRealSideLMaster[i].Text = _dataRealSideLDiffMaster[i + tabIndex].ToString();
                #endregion

                #region Lower
                tabRealSideLLower[i].Text = (_dataRealSideLDiffLower[i + tabIndex]).ToString();
                #endregion

                #region Realtime
                tabRealSideLReal[i].Text = _dataRealSideLDiffLoad[i + tabIndex].ToString();
                if (_dataRealSideLDiffLoad[i + tabIndex] > _dataRealSideLDiffUpper[i + tabIndex] || _dataRealSideLDiffLoad[i + tabIndex] < _dataRealSideLDiffLower[i + tabIndex])
                {
                    tabRealSideLReal[i].BackColor = Color.Red;
                }
                else
                {
                    tabRealSideLReal[i].BackColor = Color.White;
                }
                #endregion

                #region Upper
                tabRealSideLUpper[i].Text = (_dataRealSideLDiffUpper[i + tabIndex]).ToString();
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
                tabRealSideRMaster[i].Text = _dataRealCompSideRMaster[i + tabIndex].ToString();
                #endregion

                #region Lower
                tabRealSideRLower[i].Text = (_dataRealCompSideRLower[i + tabIndex]).ToString();
                #endregion

                #region Realtime
                tabRealSideRReal[i].Text = _dataRealCompSideRLoad[i + tabIndex].ToString();
                if (_dataRealCompSideRLoad[i + tabIndex] > _dataRealCompSideRUpper[i + tabIndex] || _dataRealCompSideRLoad[i + tabIndex] < _dataRealCompSideRLower[i + tabIndex])
                {
                    tabRealSideRReal[i].BackColor = Color.Red;
                }
                else
                {
                    tabRealSideRReal[i].BackColor = Color.White;
                }
                #endregion

                #region Upper
                tabRealSideRUpper[i].Text = (_dataRealCompSideRUpper[i + tabIndex]).ToString();
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
                tabRealSideRMaster[i].Text = _dataRealExtnSideRMaster[i + tabIndex].ToString();
                #endregion

                #region Lower
                tabRealSideRLower[i].Text = (_dataRealExtnSideRLower[i + tabIndex]).ToString();
                #endregion

                #region Realtime
                tabRealSideRReal[i].Text = _dataRealExtnSideRLoad[i + tabIndex].ToString();
                if (_dataRealExtnSideRLoad[i + tabIndex] > _dataRealExtnSideRUpper[i + tabIndex] || _dataRealExtnSideRLoad[i + tabIndex] < _dataRealExtnSideRLower[i + tabIndex])
                {
                    tabRealSideRReal[i].BackColor = Color.Red;
                }
                else
                {
                    tabRealSideRReal[i].BackColor = Color.White;
                }
                #endregion

                #region Upper
                tabRealSideRUpper[i].Text = (_dataRealExtnSideRUpper[i + tabIndex]).ToString();
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
                tabRealSideRMaster[i].Text = _dataRealSideRDiffMaster[i + tabIndex].ToString();
                #endregion

                #region Lower
                tabRealSideRLower[i].Text = (_dataRealSideRDiffLower[i + tabIndex]).ToString();
                #endregion

                #region Realtime
                tabRealSideRReal[i].Text = _dataRealSideRDiffLoad[i + tabIndex].ToString();
                if (_dataRealSideRDiffLoad[i + tabIndex] > _dataRealSideRDiffUpper[i + tabIndex] || _dataRealSideRDiffLoad[i + tabIndex] < _dataRealSideRDiffLower[i + tabIndex])
                {
                    tabRealSideRReal[i].BackColor = Color.Red;
                }
                else
                {
                    tabRealSideRReal[i].BackColor = Color.White;
                }
                #endregion

                #region Upper
                tabRealSideRUpper[i].Text = (_dataRealSideRDiffUpper[i + tabIndex]).ToString();
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
                tabMasterSideLMaster[i].Text = _dataMasterCompSideLMaster[i + tabIndex].ToString();
                #endregion

                #region Master Acc
                tabMasterSideLAccMaster[i].Text = _dataMasterCompSideLAccMaster[i + tabIndex].ToString();
                #endregion

                #region Lower
                tabMasterSideLLower[i].Text = (_dataMasterCompSideLMaster[i + tabIndex] - _dataMasterCompSideLLower[i + tabIndex]).ToString();
                #endregion

                #region Upper
                tabMasterSideLUpper[i].Text = (_dataMasterCompSideLUpper[i + tabIndex] - _dataMasterCompSideLMaster[i + tabIndex]).ToString();
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
                tabMasterSideLMaster[i].Text = _dataMasterExtnSideLMaster[i + tabIndex].ToString();
                #endregion

                #region Master Acc
                tabMasterSideLAccMaster[i].Text = _dataMasterExtnSideLAccMaster[i + tabIndex].ToString();
                #endregion

                #region Lower
                tabMasterSideLLower[i].Text = (_dataMasterExtnSideLMaster[i + tabIndex] - _dataMasterExtnSideLLower[i + tabIndex]).ToString();
                #endregion

                #region Upper
                tabMasterSideLUpper[i].Text = (_dataMasterExtnSideLUpper[i + tabIndex] - _dataMasterExtnSideLMaster[i + tabIndex]).ToString();
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
                tabMasterSideLMaster[i].Text = _dataMasterSideLDiffMaster[i + tabIndex].ToString();
                #endregion

                #region Master Acc
                tabMasterSideLAccMaster[i].Text = _dataMasterSideLDiffAccMaster[i + tabIndex].ToString();
                #endregion

                #region Lower
                tabMasterSideLLower[i].Text = (_dataMasterSideLDiffMaster[i + tabIndex] - _dataMasterSideLDiffLower[i + tabIndex]).ToString();
                #endregion

                #region Upper
                tabMasterSideLUpper[i].Text = (_dataMasterSideLDiffUpper[i + tabIndex] - _dataMasterSideLDiffMaster[i + tabIndex]).ToString();
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
                tabMasterSideRMaster[i].Text = _dataMasterCompSideRMaster[i + tabIndex].ToString();
                #endregion

                #region Master Acc
                tabMasterSideRAccMaster[i].Text = _dataMasterCompSideRAccMaster[i + tabIndex].ToString();
                #endregion

                #region Lower
                tabMasterSideRLower[i].Text = (_dataMasterCompSideRMaster[i + tabIndex] - _dataMasterCompSideRLower[i + tabIndex]).ToString();
                #endregion

                #region Upper
                tabMasterSideRUpper[i].Text = (_dataMasterCompSideRUpper[i + tabIndex] - _dataMasterCompSideRMaster[i + tabIndex]).ToString();
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
                tabMasterSideRMaster[i].Text = _dataMasterExtnSideRMaster[i + tabIndex].ToString();
                #endregion

                #region Master Acc
                tabMasterSideRAccMaster[i].Text = _dataMasterExtnSideRAccMaster[i + tabIndex].ToString();
                #endregion

                #region Lower
                tabMasterSideRLower[i].Text = (_dataMasterExtnSideRMaster[i + tabIndex] - _dataMasterExtnSideRLower[i + tabIndex]).ToString();
                #endregion

                #region Upper
                tabMasterSideRUpper[i].Text = (_dataMasterExtnSideRUpper[i + tabIndex] - _dataMasterExtnSideRMaster[i + tabIndex]).ToString();
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
                tabMasterSideRMaster[i].Text = _dataMasterSideRDiffMaster[i + tabIndex].ToString();
                #endregion

                #region Master Acc
                tabMasterSideRAccMaster[i].Text = _dataMasterSideRDiffAccMaster[i + tabIndex].ToString();
                #endregion

                #region Lower
                tabMasterSideRLower[i].Text = (_dataMasterSideRDiffMaster[i + tabIndex] - _dataMasterSideRDiffLower[i + tabIndex]).ToString();
                #endregion

                #region Upper
                tabMasterSideRUpper[i].Text = (_dataMasterSideRDiffUpper[i + tabIndex] - _dataMasterSideRDiffMaster[i + tabIndex]).ToString();
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
                float.TryParse(tabMasterSideLStroke[i].Text, out float num1);
                _dataMasterCompSideLStroke[i + tabIndex] = num1;
                #endregion

                #region Master
                float.TryParse(tabMasterSideLMaster[i].Text, out float num2);
                _dataMasterCompSideLMaster[i + tabIndex] = num2;
                #endregion

                #region Master Acc
                float.TryParse(tabMasterSideLAccMaster[i].Text, out float num3);
                _dataMasterCompSideLAccMaster[i + tabIndex] = num3;
                #endregion

                #region Lower
                float.TryParse(tabMasterSideLLower[i].Text, out float num4);
                _dataMasterCompSideLLower[i + tabIndex] = num2 - num4;
                #endregion

                #region Upper
                float.TryParse(tabMasterSideLUpper[i].Text, out float num5);
                _dataMasterCompSideLUpper[i + tabIndex] = num2 + num5;
                #endregion
            }
        }
        public void tabdataMasterExtnStep2LW()
        {
            int tabIndex = tabIdxMasterSideL * 40;
            for (int i = 0; i < 40; i++)
            {
                #region Stroke
                float.TryParse(tabMasterSideLStroke[i].Text, out float num1);
                _dataMasterExtnSideLStroke[i + tabIndex] = num1;
                #endregion

                #region Master
                float.TryParse(tabMasterSideLMaster[i].Text, out float num2);
                _dataMasterExtnSideLMaster[i + tabIndex] = num2;
                #endregion

                #region Master Acc
                float.TryParse(tabMasterSideLAccMaster[i].Text, out float num3);
                _dataMasterExtnSideLAccMaster[i + tabIndex] = num3;
                #endregion

                #region Lower
                float.TryParse(tabMasterSideLLower[i].Text, out float num4);
                _dataMasterExtnSideLLower[i + tabIndex] = num2 - num4;
                #endregion

                #region Upper
                float.TryParse(tabMasterSideLUpper[i].Text, out float num5);
                _dataRealExtnSideLUpper[i + tabIndex] = num2 + num5;
                #endregion
            }
        }
        public void tabdataMasterDiffStep2LW()
        {
            int tabIndex = tabIdxMasterSideL * 40;
            for (int i = 0; i < 40; i++)
            {
                #region Stroke
                float.TryParse(tabMasterSideLStroke[i].Text, out float num1);
                _dataMasterSideLDiffStroke[i + tabIndex] = num1;
                #endregion

                #region Master
                float.TryParse(tabMasterSideLMaster[i].Text, out float num2);
                _dataMasterSideLDiffMaster[i + tabIndex] = num2;
                #endregion

                #region Master Acc
                float.TryParse(tabMasterSideLAccMaster[i].Text, out float num3);
                _dataMasterSideLDiffAccMaster[i + tabIndex] = num3;
                #endregion

                #region Lower
                float.TryParse(tabMasterSideLLower[i].Text, out float num4);
                _dataMasterSideLDiffLower[i + tabIndex] = num2 - num4;
                #endregion

                #region Upper
                float.TryParse(tabMasterSideLUpper[i].Text, out float num5);
                _dataRealSideLDiffUpper[i + tabIndex] = num2 + num5;
                #endregion
            }
        }
        public void tabdataMasterCompStep2RW()
        {
            int tabIndex = tabIdxMasterSideR * 40;
            for (int i = 0; i < 40; i++)
            {
                #region Stroke
                float.TryParse(tabMasterSideRStroke[i].Text, out float num1);
                _dataMasterCompSideRStroke[i + tabIndex] = num1;
                #endregion

                #region Master
                float.TryParse(tabMasterSideRMaster[i].Text, out float num2);
                _dataMasterCompSideRMaster[i + tabIndex] = num2;
                #endregion

                #region Lower
                float.TryParse(tabMasterSideRAccMaster[i].Text, out float num3);
                _dataMasterCompSideRAccMaster[i + tabIndex] = num3;
                #endregion

                #region Realtime
                float.TryParse(tabMasterSideRLower[i].Text, out float num4);
                _dataMasterCompSideRLower[i + tabIndex] = num2 - num4;
                #endregion

                #region Upper
                float.TryParse(tabMasterSideRUpper[i].Text, out float num5);
                _dataRealCompSideRUpper[i + tabIndex] = num2 + num5;
                #endregion
            }
        }
        public void tabdataMasterExtnStep2RW()
        {
            int tabIndex = tabIdxMasterSideR * 40;
            for (int i = 0; i < 40; i++)
            {
                #region Stroke
                float.TryParse(tabMasterSideRStroke[i].Text, out float num1);
                _dataMasterExtnSideRStroke[i + tabIndex] = num1;
                #endregion

                #region Master
                float.TryParse(tabMasterSideRMaster[i].Text, out float num2);
                _dataMasterExtnSideRMaster[i + tabIndex] = num2;
                #endregion

                #region Lower
                float.TryParse(tabMasterSideRAccMaster[i].Text, out float num3);
                _dataMasterExtnSideRAccMaster[i + tabIndex] = num3;
                #endregion

                #region Realtime
                float.TryParse(tabMasterSideRLower[i].Text, out float num4);
                _dataMasterExtnSideRLower[i + tabIndex] = num2 - num4;
                #endregion

                #region Upper
                float.TryParse(tabMasterSideRUpper[i].Text, out float num5);
                _dataMasterExtnSideRUpper[i + tabIndex] = num2 + num5;
                #endregion
            }
        }
        public void tabdataMasterDiffStep2RW()
        {
            int tabIndex = tabIdxMasterSideR * 40;
            for (int i = 0; i < 40; i++)
            {
                #region Stroke
                float.TryParse(tabMasterSideRStroke[i].Text, out float num1);
                _dataMasterSideRDiffStroke[i + tabIndex] = num1;
                #endregion

                #region Master
                float.TryParse(tabMasterSideRMaster[i].Text, out float num2);
                _dataMasterSideRDiffMaster[i + tabIndex] = num2;
                #endregion

                #region Master Acc
                float.TryParse(tabMasterSideRAccMaster[i].Text, out float num3);
                _dataMasterSideRDiffAccMaster[i + tabIndex] = num3;
                #endregion

                #region Lower
                float.TryParse(tabMasterSideRLower[i].Text, out float num4);
                _dataMasterSideRDiffLower[i + tabIndex] = num2 - num4;
                #endregion

                #region Upper
                float.TryParse(tabMasterSideRUpper[i].Text, out float num5);
                _dataRealSideRDiffUpper[i + tabIndex] = num2 + num5;
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
            InitializeComponent();
            InitializePlotStyle();
            InitializeCustomComponents();
            InitializeBorderComponent();

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
            { } //init connection

            InitializeUI();
            drawingBorderUpper.BringToFront();
            drawingBorderLower.BringToFront();
            drawingBorderLeft.BringToFront();
            drawingBorderRight.BringToFront();
            drawingPanel.BringToFront();

        }
        private void InitializePlotStyle()
        {
            formsPlot1.Plot.XLabel("Stroke");
            formsPlot1.Plot.YLabel("Load");
            formsPlot1.Plot.ShowLegend();
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

            formsPlot2.Plot.XLabel("Stroke");
            formsPlot2.Plot.YLabel("Load");
            formsPlot2.Plot.ShowLegend();
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

            formsPlot3.Plot.XLabel("Stroke");
            formsPlot3.Plot.YLabel("Load");
            formsPlot3.Plot.ShowLegend();
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

            formsPlot4.Plot.XLabel("Stroke");
            formsPlot4.Plot.YLabel("Load");
            formsPlot4.Plot.ShowLegend();
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

            formsPlot5.Plot.XLabel("Stroke");
            formsPlot5.Plot.YLabel("Load");
            formsPlot5.Plot.ShowLegend();
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

            formsPlot6.Plot.XLabel("Stroke");
            formsPlot6.Plot.YLabel("Load");
            formsPlot6.Plot.ShowLegend();
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

            formsPlot7.Plot.XLabel("Stroke");
            formsPlot7.Plot.YLabel("Load");
            formsPlot7.Plot.ShowLegend();
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

            formsPlot8.Plot.XLabel("Stroke");
            formsPlot8.Plot.YLabel("Load");
            formsPlot8.Plot.ShowLegend();
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

            formsPlot9.Plot.XLabel("Stroke");
            formsPlot9.Plot.YLabel("Load");
            formsPlot9.Plot.ShowLegend();
            formsPlot9.Plot.Axes.AntiAlias(true);
            formsPlot9.Plot.Axes.Hairline(true);
            formsPlot9.Plot.Axes.AutoScale(true);
            formsPlot9.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#708090");
            formsPlot9.Plot.DataBackground.Color = ScottPlot.Color.FromHex("#343c43");
            formsPlot9.Plot.Axes.Color(ScottPlot.Color.FromColor(System.Drawing.Color.Ivory));
            formsPlot9.Plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#5a6773");
            formsPlot9.Plot.Legend.BackgroundColor = ScottPlot.Color.FromHex("#708090");
            formsPlot9.Plot.Legend.FontColor = ScottPlot.Color.FromColor(System.Drawing.Color.Ivory);
            formsPlot9.Plot.Legend.OutlineColor = ScottPlot.Color.FromHex("#5a6773");

            formsPlot10.Plot.XLabel("Stroke");
            formsPlot10.Plot.YLabel("Load");
            formsPlot10.Plot.ShowLegend();
            formsPlot10.Plot.Axes.AntiAlias(true);
            formsPlot10.Plot.Axes.Hairline(true);
            formsPlot10.Plot.Axes.AutoScale(true);
            formsPlot10.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#708090");
            formsPlot10.Plot.DataBackground.Color = ScottPlot.Color.FromHex("#343c43");
            formsPlot10.Plot.Axes.Color(ScottPlot.Color.FromColor(System.Drawing.Color.Ivory));
            formsPlot10.Plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#5a6773");
            formsPlot10.Plot.Legend.BackgroundColor = ScottPlot.Color.FromHex("#708090");
            formsPlot10.Plot.Legend.FontColor = ScottPlot.Color.FromColor(System.Drawing.Color.Ivory);
            formsPlot10.Plot.Legend.OutlineColor = ScottPlot.Color.FromHex("#5a6773");

            formsPlot11.Plot.XLabel("Stroke");
            formsPlot11.Plot.YLabel("Load");
            formsPlot11.Plot.ShowLegend();
            formsPlot11.Plot.Axes.AntiAlias(true);
            formsPlot11.Plot.Axes.Hairline(true);
            formsPlot11.Plot.Axes.AutoScale(true);
            formsPlot11.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#708090");
            formsPlot11.Plot.DataBackground.Color = ScottPlot.Color.FromHex("#343c43");
            formsPlot11.Plot.Axes.Color(ScottPlot.Color.FromColor(System.Drawing.Color.Ivory));
            formsPlot11.Plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#5a6773");
            formsPlot11.Plot.Legend.BackgroundColor = ScottPlot.Color.FromHex("#708090");
            formsPlot11.Plot.Legend.FontColor = ScottPlot.Color.FromColor(System.Drawing.Color.Ivory);
            formsPlot11.Plot.Legend.OutlineColor = ScottPlot.Color.FromHex("#5a6773");

            formsPlot12.Plot.XLabel("Stroke");
            formsPlot12.Plot.YLabel("Load");
            formsPlot12.Plot.ShowLegend();
            formsPlot12.Plot.Axes.AntiAlias(true);
            formsPlot12.Plot.Axes.Hairline(true);
            formsPlot12.Plot.Axes.AutoScale(true);
            formsPlot12.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#708090");
            formsPlot12.Plot.DataBackground.Color = ScottPlot.Color.FromHex("#343c43");
            formsPlot12.Plot.Axes.Color(ScottPlot.Color.FromColor(System.Drawing.Color.Ivory));
            formsPlot12.Plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#5a6773");
            formsPlot12.Plot.Legend.BackgroundColor = ScottPlot.Color.FromHex("#708090");
            formsPlot12.Plot.Legend.FontColor = ScottPlot.Color.FromColor(System.Drawing.Color.Ivory);
            formsPlot12.Plot.Legend.OutlineColor = ScottPlot.Color.FromHex("#5a6773");
        }
        private void InitializeUI()
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            // Set form properties
            this.Text = "Damping Force Function Tester";
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

            PreloadTabPages();
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

        public ScottPlot.Plottables.SignalXY Plot2_PRESENT;
        public ScottPlot.Plottables.SignalXY Plot2_MASTER;
        public ScottPlot.Plottables.SignalXY Plot2_LOWER;
        public ScottPlot.Plottables.SignalXY Plot2_UPPER;

        public ScottPlot.Plottables.SignalXY Plot3_PRESENT;
        public ScottPlot.Plottables.SignalXY Plot3_MASTER;
        public ScottPlot.Plottables.SignalXY Plot3_LOWER;
        public ScottPlot.Plottables.SignalXY Plot3_UPPER;

        public ScottPlot.Plottables.SignalXY Plot4_PRESENT;
        public ScottPlot.Plottables.SignalXY Plot4_MASTER;
        public ScottPlot.Plottables.SignalXY Plot4_LOWER;
        public ScottPlot.Plottables.SignalXY Plot4_UPPER;

        public ScottPlot.Plottables.SignalXY Plot5_PRESENT;
        public ScottPlot.Plottables.SignalXY Plot5_MASTER;
        public ScottPlot.Plottables.SignalXY Plot5_LOWER;
        public ScottPlot.Plottables.SignalXY Plot5_UPPER;

        public ScottPlot.Plottables.SignalXY Plot6_PRESENT;
        public ScottPlot.Plottables.SignalXY Plot6_MASTER;
        public ScottPlot.Plottables.SignalXY Plot6_LOWER;
        public ScottPlot.Plottables.SignalXY Plot6_UPPER;

        public ScottPlot.Plottables.SignalXY Plot7_PRESENT;
        public ScottPlot.Plottables.SignalXY Plot7_MASTER;
        public ScottPlot.Plottables.SignalXY Plot7_LOWER;
        public ScottPlot.Plottables.SignalXY Plot7_UPPER;

        public ScottPlot.Plottables.SignalXY Plot8_PRESENT;
        public ScottPlot.Plottables.SignalXY Plot8_MASTER;
        public ScottPlot.Plottables.SignalXY Plot8_LOWER;
        public ScottPlot.Plottables.SignalXY Plot8_UPPER;

        public ScottPlot.Plottables.SignalXY Plot9_PRESENT;
        public ScottPlot.Plottables.SignalXY Plot9_MASTER;
        public ScottPlot.Plottables.SignalXY Plot9_LOWER;
        public ScottPlot.Plottables.SignalXY Plot9_UPPER;

        public ScottPlot.Plottables.SignalXY Plot10_PRESENT;
        public ScottPlot.Plottables.SignalXY Plot10_MASTER;
        public ScottPlot.Plottables.SignalXY Plot10_LOWER;
        public ScottPlot.Plottables.SignalXY Plot10_UPPER;

        public ScottPlot.Plottables.SignalXY Plot11_PRESENT;
        public ScottPlot.Plottables.SignalXY Plot11_MASTER;
        public ScottPlot.Plottables.SignalXY Plot11_LOWER;
        public ScottPlot.Plottables.SignalXY Plot11_UPPER;

        public ScottPlot.Plottables.SignalXY Plot12_PRESENT;
        public ScottPlot.Plottables.SignalXY Plot12_MASTER;
        public ScottPlot.Plottables.SignalXY Plot12_LOWER;
        public ScottPlot.Plottables.SignalXY Plot12_UPPER;

        #endregion


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

        public void PlotSignalPlotting(ref ScottPlot.WinForms.FormsPlot plotbase, ref ScottPlot.Plottables.SignalXY plotsig, double[] xd, double[] yd)
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

        #endregion

        #region UIMethod
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

        private void button12_Click(object sender, EventArgs e)
        {

        }

        private void button15_Click(object sender, EventArgs e)
        {

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

        #endregion

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
        }

        private void button30_Click(object sender, EventArgs e)
        {
            //UPDATE MASTER DATA
            if (MasterTeachSetConfirm)
            {
                _WorkflowHandler.workUpdateMasterData();
                _WorkflowHandler.workUpdateMasterDatabase();
                MasterTeachSetConfirm = false;
            }
        }

        private void button80_Click(object sender, EventArgs e)
        {
            //VALIDATE MASTER DATA
            _WorkflowHandler.workMasterValidation();
            MasterTeachSetConfirm = false;

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }


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

    #endregion
}
