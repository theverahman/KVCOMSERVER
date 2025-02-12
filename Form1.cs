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

using DRW = System.Drawing;
using Point = System.Drawing.Point;
using Label = System.Windows.Forms.Label;

using LIBKVPROTOCOL;
using WORKFLOW;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;

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

        CustomTableLayoutPanel tabMasterSideL;
        List<TextBox> tabMasterSideLStroke = new List<TextBox>();
        List<TextBox> tabMasterSideLMaster = new List<TextBox>();
        List<TextBox> tabMasterSideLLower = new List<TextBox>();
        List<TextBox> tabMasterSideLReal = new List<TextBox>();
        List<TextBox> tabMasterSideLUpper = new List<TextBox>();

        CustomTableLayoutPanel tabMasterSideR;
        List<TextBox> tabMasterSideRStroke = new List<TextBox>();
        List<TextBox> tabMasterSideRMaster = new List<TextBox>();
        List<TextBox> tabMasterSideRLower = new List<TextBox>();
        List<TextBox> tabMasterSideRReal = new List<TextBox>();
        List<TextBox> tabMasterSideRUpper = new List<TextBox>();



        public string settingIpv4;
        public int settingPortIp;
        public string msgToBeSent;
        public int _connStat;
        public int _beaconn;

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
                Location = new DRW.Point(10, 10),
                Size = new Size(4, 1080)
            };

            drawingBorderRight = new Panel
            {
                BackColor = Color.Transparent,
                Location = new DRW.Point(1883, 10),
                Size = new Size(5, 1080)
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
                RowCount = 51,
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
                        BackColor = Color.LightGray

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
                RowCount = 51,
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
                RowCount = 51,
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
                tabMasterSideRLower.Add(tbx);
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
                tabMasterSideRReal.Add(tbx);
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
                RowCount = 51,
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
                tabMasterSideLLower.Add(tbx);
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
                tabMasterSideLReal.Add(tbx);
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
                tabMasterSideLUpper.Add(tbx);
                tabMasterSideL.Controls.Add(tbx, 4, row);
            }
            // Add the CustomTableLayoutPanel to the form
            tabPage8.Controls.Add(tabMasterSideL);
            tabMasterSideL.BringToFront();
        }

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
            InitializeCustomComponents();
            InitializeBorderComponent();
            drawingBorderUpper.BringToFront();
            drawingBorderLower.BringToFront();
            drawingBorderLeft.BringToFront();
            drawingBorderRight.BringToFront();
            drawingPanel.BringToFront();

        }

        private void InitializeUI()
        {
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

            tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl1.DrawItem += TabControl_DrawItem;
            tabControl1.Selected += TabControl_Selected;

            defLayoutPanelRealSideR();
            defLayoutPanelRealSideL();
            defLayoutPanelMasterSideR();
            defLayoutPanelMasterSideL();
        }

        private void TabControl_Selected(object sender, TabControlEventArgs e)
        {
            // Refresh the TabControl to apply the drawing changes
            tabControl1.Invalidate();
        }

        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            // This will hide the tabs by not drawing them
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
    }

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

}
