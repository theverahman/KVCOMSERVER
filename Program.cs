#pragma warning disable CA1416 // Validate platform compatibility


using System;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Reflection;
using System.DirectoryServices;
using System.Text.RegularExpressions;
using System.IO;

using FUNCTION_TESTER;
using FUNCTION_TESTER.DB;
using LIBKVPROTOCOL;
using LIBSETTEI;
using LIBEXCELMANIPULATOR;
using Sres.Net.EEIP;

using ClosedXML.Excel;
using MoreLinq;
using MoreLinq.Extensions;
using SixLabors.ImageSharp.Drawing;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Packaging;

using Microsoft.Office.Interop;
using Excel = Microsoft.Office.Interop.Excel;
using MethodInvoker = System.Windows.Forms.MethodInvoker;
using STimer = System.Threading.Timer;

namespace KVCOMSERVER
{
    internal class Program
    {
        //WORKFLOW.WORKFLOWHANDLER _WorkflowHandler = new WORKFLOW.WORKFLOWHANDLER(1);
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
            Application.ThreadException += (sender, e) =>
            {
                Environment.Exit(1); // Terminate the program with a non-zero exit code
            };

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}


namespace WORKFLOW
{
    public class WORKFLOWHANDLER
    {
        private KVCOMSERVER.Form1 _uiObject;
        private CancellationTokenSource _cts;
        private CancellationTokenSource _ctsClock;
        private STimer _clock1s;
        private STimer _clock1ms;
        private STimer _clock10ms;
        private STimer _clock100ms;
        Thread backgroundThread;

        SETTEI _settingObject;
        KVPROTOCOL _kvconnObject;
        EEIPClient _eeipObject;

        EXCELSTREAM MasterFileActive;
        EXCELSTREAM RealtimeFileL1;
        EXCELSTREAM RealtimeFileR1;

        EXCELSTREAM LogBufferReadFileL1;
        EXCELSTREAM LogBufferReadFileR1;

        DATAMODEL_COMMON _data;
        DATAMODEL_MASTER _masterData;
        DATAMODEL_L _Ldata;
        DATAMODEL_R _Rdata;

        public string HeadDir;
        public string RealLogDir;
        public string MasterDir;

        bool _parameterRead;
        bool _parameterReadFlag;
        bool _parameterReadFlagComplete;
        bool _realtimeRead;
        bool _realtimeReadFlag;
        bool _realtimeReadFlagComplete;

        bool _backgroundProcessOngoing { get; set; }

        string _kvMsgRecv;


        public WORKFLOWHANDLER(KVCOMSERVER.Form1 formobject)
        {
            _uiObject = formobject;
            _settingObject = new SETTEI("FILESETTING.SETTEI");
            _uiObject.settingIpv4 = _settingObject.IPADDR_SETTEI_GET();
            _uiObject.settingPortIp = Convert.ToInt16(_settingObject.PORTCOMM_SETTEI_GET());

            _kvconnObject = new KVPROTOCOL();
            _eeipObject = new EEIPClient();
            _eeipObject.IPAddress = _uiObject.settingIpv4;

            MasterFileActive = new EXCELSTREAM("MASTER");
            RealtimeFileL1 = new EXCELSTREAM("REALTIME");
            RealtimeFileR1 = new EXCELSTREAM("REALTIME");

            LogBufferReadFileL1 = new EXCELSTREAM("REALTIME");
            LogBufferReadFileR1 = new EXCELSTREAM("REALTIME");

            _data = new DATAMODEL_COMMON();
            _Ldata = new DATAMODEL_L();
            _Rdata = new DATAMODEL_R();

            _ctsClock = new CancellationTokenSource();
            _clock1s = new STimer(async _ => await Clock1s(_ctsClock.Token), null, TimeSpan.Zero, TimeSpan.FromMilliseconds(1000));
            _clock1ms = new STimer(async _ => await Clock1ms(_ctsClock.Token), null, TimeSpan.Zero, TimeSpan.FromMilliseconds(1));
            _clock10ms = new STimer(async _ => await Clock10ms(_ctsClock.Token), null, TimeSpan.Zero, TimeSpan.FromMilliseconds(10));
            _clock100ms = new STimer(async _ => await Clock100ms(_ctsClock.Token), null, TimeSpan.Zero, TimeSpan.FromMilliseconds(100));

            HeadDir = _settingObject.FILEDIR_SETTEI_GET();
            RealLogDir = HeadDir + $"LOG_REALTIME\\";
            MasterDir = HeadDir + $"MASTER_MODEL_DATA\\";

            backgroundThread = new Thread(BackgroundWork);
            backgroundThread.Start();

        }

        private async Task Clock1s(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            await Task.Run(() => UpdateUIRealtimeList(), cancellationToken);
        }

        private async Task Clock1ms(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;
        }

        private async Task Clock10ms(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;
        }

        private async Task Clock100ms(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            
        }

        

        void UpdateUIRealtimeList()
        {
            if (_uiObject.InvokeRequired)
            {
                _uiObject.BeginInvoke(new MethodInvoker(_uiObject.RealtimeUpdateList));
            }
            else
            {
                _uiObject.RealtimeUpdateList();
            }
        }



        void CheckFolderPath(string pathblazer) { if (!Directory.Exists(pathblazer)) { Directory.CreateDirectory(pathblazer); } }

        public bool Get_backgroundProcessOngoing()
        {
            return _backgroundProcessOngoing;
        }

        public void Set_backgroundProcessOngoing()
        {
            _backgroundProcessOngoing = true;
        }

        public void Res_backgroundProcessOngoing()
        {
            _backgroundProcessOngoing = false;
        }

        public void SetConnection()
        {
            _kvconnObject.SetConnection(_uiObject.settingIpv4, _uiObject.settingPortIp);
            _eeipObject.RegisterSession();
        }

        public void CloseConnection()
        {
            _kvconnObject.CloseConnection();
            _eeipObject.UnRegisterSession();
        }

        public int GetConnState()
        {
            return ((int)_eeipObject.SessionStatus());
        }

        public void SendMessage(string msgs)
        {
            _kvconnObject.connSend
                (
                    Encoding.ASCII.GetBytes(msgs)
                );
        }

        void _eeipEventHandler_1() //Beacon
        {
            if (this.GetConnState() == 1)
            {
                byte[] STAT_INPUT = _eeipObject.AssemblyObject.getInstance(0xA0);
                _eeipBeacon(STAT_INPUT);
                byte[] TRIG = _eeipObject.AssemblyObject.getInstance(0x8E);
                if ((byte)(TRIG[0] & 0x01) == 0x01)
                {
                    _uiPlotClear();
                    _kvconnObject.writeDataCommand("W0FE0", "", "0");
                }
                    //Thread.Sleep(100);
            }
        }

        void _eeipEventHandler_2() //Parameter Data Retrieve
        {
            if (this.GetConnState() == 1)
            {
                byte[] STAT_INPUT = _eeipObject.AssemblyObject.getInstance(0xA0);
                _eeipTriggerReadParameter(STAT_INPUT);
                //Thread.Sleep(10);
            }

        }

        void _eeipEventHandler_3() //Realtime Data Retrieve
        {
            if (this.GetConnState() == 1)
            {
                byte[] STAT_INPUT = _eeipObject.AssemblyObject.getInstance(0xA0);
                _eeipTriggerReadRealtime(STAT_INPUT);
                //Thread.Sleep(10);
            }

        }

        void _eeipEventHandler_4() //Master Fetch Model
        {
            if (this.GetConnState() == 1)
            {
                byte[] TRIG = _eeipObject.AssemblyObject.getInstance(0x8E); //undetermined byte address
                if ((byte)(TRIG[0] & 0x01) == 0x01) //undetermined value
                {
                    //undetermined parameter address
                    //byte[] MODEL_NAME_INPUT = _eeipObject.AssemblyObject.getInstance(0xA0);
                    ////undetermined method
                    //_eeipTrigMasterFetchModel(MODEL_NAME_INPUT); 
                    //Thread.Sleep(10);
                }
            }

        }

        void _eeipBeacon(byte[] STAT_INPUT)
        {
            if ((byte)(STAT_INPUT[0] & 0x01) == 0x01)
            {
                _uiObject._beaconn = 1;
                //_kvconnObject.SetConnection(_uiObject.settingIpv4, _uiObject.settingPortIp);
                _kvconnObject.writeDataCommand("W0A0", "", "1");
                //_kvconnObject.CloseConnection();

            }
            if ((byte)(STAT_INPUT[0] & 0x01) == 0x00)
            {
                _uiObject._beaconn = 0;
                //_kvconnObject.SetConnection(_uiObject.settingIpv4, _uiObject.settingPortIp);
                _kvconnObject.writeDataCommand("W0A0", "", "0");
                //_kvconnObject.CloseConnection();

            }

        }

        void _eeipTrigMasterFetchModel(byte[] MODNAME_VAR)
        {
            string MODNAME = Encoding.Default.GetString(MODNAME_VAR);
            foreach (string files in Directory.GetFiles(MasterDir))
            {
                if (files.Contains(MODNAME))
                {
                    _excelReadMasterData(files);
                    //>//_kvconnObject.writeDataCommand("W0A0", "", "0"); //>confirm if read file complete
                    //MasterFileL1.FileRead(files);
                    //MasterFileR1.FileRead(files);
                }
                else
                {
                    //_kvconnObject.writeDataCommand("W0A0", "", "0"); //>confirm if not found
                    //MessageBox.Show("Master File for this model is not found. Please initiate setting.", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        

        void _eeipTriggerReadParameter(byte[] STAT_INPUT)
        {
            if ((byte)(STAT_INPUT[2] & 0x01) == 0x01)
            {
                if (!_parameterReadFlag)
                {
                    Debug.Write("Parameter Read On");
                    Debug.Write((char)'\n');

                    _parameterReadFlag = true;

                    _eeipreadActiveModelData();
                    _eeipreadStep1Param();
                    _eeipreadStep2345Param();

                    _excelStoreParameterData();
                }

                if (_parameterReadFlag)
                {
                    //_kvconnObject.SetConnection(_uiObject.settingIpv4, _uiObject.settingPortIp);
                    _kvconnObject.writeDataCommand("W0C1", "", "0");
                    //_kvconnObject.CloseConnection();

                    Thread.Sleep(1);
                }

            }
            if ((byte)(STAT_INPUT[2] & 0x01) == 0x00)
            {
                if (_parameterReadFlag)
                {
                    Debug.Write("Parameter Read Off");
                    Debug.Write((char)'\n');
                    _parameterReadFlag = false;
                }
            }
        }

        void _eeipTriggerReadRealtime(byte[] STAT_INPUT)
        {
            if ((byte)(STAT_INPUT[4] & 0x01) == 0x01)
            {
                if (!_realtimeReadFlag)
                {
                    _realtimeReadFlag = true;

                    _Rdata._Step1MaxLoad_NG = 0;
                    _Rdata._Step2CompRef_NG = 0;
                    _Rdata._Step2CompGraph_NG = 0;
                    _Rdata._Step2ExtnRef_NG = 0;
                    _Rdata._Step2ExtnGraph_NG = 0;
                    _Rdata._Step2DiffGraph_NG = 0;

                    _Ldata._Step1MaxLoad_NG = 0;
                    _Ldata._Step2CompRef_NG = 0;
                    _Ldata._Step2CompGraph_NG = 0;
                    _Ldata._Step2ExtnRef_NG = 0;
                    _Ldata._Step2ExtnGraph_NG = 0;
                    _Ldata._Step2DiffGraph_NG = 0;

                    Debug.Write("RL Read On");
                    Debug.Write((char)'\n');

                    _eeipreadDateTime();
                    _eeipreadJudgement(ref _Rdata.Judgement, 0xA5);
                    _eeipreadJudgement(ref _Ldata.Judgement, 0xA6);
                    _kvreadRealtime(ref _Rdata.RealtimeStep2, "ZF110000", "ZF110400", "ZF110800", "ZF111200", "ZF110000", "ZF510000", 400);
                    //_kvreadRealtime(ref _Rdata.RealtimeStep3, "ZF111604", "ZF112004", "ZF112404", "ZF113208", "ZF111604", "ZF510000", 400);
                    _kvreadRealtime(ref _Ldata.RealtimeStep2, "ZF210000", "ZF210400", "ZF210800", "ZF211200", "ZF210000", "ZF510500", 400);
                    //_kvreadRealtime(ref _Ldata.RealtimeStep3, "ZF211604", "ZF212004", "ZF212404", "ZF213208", "ZF211604", "ZF510500", 400);

                    _Rdata._Step1MaxLoad_NG = _kvconnObject.readbitCommand("LR201");
                    _Rdata._Step2CompRef_NG = _kvconnObject.readbitCommand("LR203");
                    _Rdata._Step2CompGraph_NG = _kvconnObject.readbitCommand("LR205");
                    _Rdata._Step2ExtnRef_NG = _kvconnObject.readbitCommand("LR207");
                    _Rdata._Step2ExtnGraph_NG = _kvconnObject.readbitCommand("LR209");
                    _Rdata._Step2DiffGraph_NG = _kvconnObject.readbitCommand("LR211");

                    _Ldata._Step1MaxLoad_NG = _kvconnObject.readbitCommand("LR601");
                    _Ldata._Step2CompRef_NG = _kvconnObject.readbitCommand("LR603");
                    _Ldata._Step2CompGraph_NG = _kvconnObject.readbitCommand("LR605");
                    _Ldata._Step2ExtnRef_NG = _kvconnObject.readbitCommand("LR607");
                    _Ldata._Step2ExtnGraph_NG = _kvconnObject.readbitCommand("LR609");
                    _Ldata._Step2DiffGraph_NG = _kvconnObject.readbitCommand("LR611");

                    if (_Rdata._Step2CompGraph_NG == 1)
                    {
                        D1Col = System.Drawing.Color.Red;
                    }
                    else
                    {
                        D1Col = System.Drawing.Color.LimeGreen;
                    }

                    if (_Rdata._Step2ExtnGraph_NG == 1)
                    {
                        D2Col = System.Drawing.Color.Red;
                    }
                    else
                    {
                        D2Col = System.Drawing.Color.LimeGreen;
                    }

                    if (_Ldata._Step2CompGraph_NG == 1)
                    {
                        D3Col = System.Drawing.Color.Red;
                    }
                    else
                    {
                        D3Col = System.Drawing.Color.LimeGreen;
                    }
                    if (_Ldata._Step2ExtnGraph_NG == 1)
                    {
                        D4Col = System.Drawing.Color.Red;
                    }
                    else
                    {
                        D4Col = System.Drawing.Color.LimeGreen;
                    }

                    _excelStoreRealtimeData();

                    _backgroundDataPlot1Read();
                    _uiPlot1Update();
                    _backgroundDataPlot2Read();
                    _uiPlot2Update();
                    _backgroundDataPlot3Read();
                    _uiPlot3Update();
                    _backgroundDataPlot4Read();
                    _uiPlot4Update();
                }

                if (_realtimeReadFlag)
                {
                    _kvconnObject.writeDataCommand("W0C2", "", "0");
                    Thread.Sleep(1);
                }
            }
            if ((byte)(STAT_INPUT[4] & 0x01) == 0x00)
            {
                if (_realtimeReadFlag)
                {
                    Debug.Write("RL Read Off");
                    Debug.Write((char)'\n');
                    _realtimeReadFlag = false;
                }
            }
        }

        void _excelStoreParameterData()
        {
            RealtimeFileR1.setModelName(_data._activeModelName);
            RealtimeFileR1.setParameterStep1(_data.Step1Param);
            RealtimeFileR1.setParameterStep2345(_data.Step2345Param);

            RealtimeFileL1.setModelName(_data._activeModelName);
            RealtimeFileL1.setParameterStep1(_data.Step1Param);
            RealtimeFileL1.setParameterStep2345(_data.Step2345Param);
        }

        void _excelStoreRealtimeData()
        {
            RealtimeFileR1.RESET_LABEL_NG();
            RealtimeFileL1.RESET_LABEL_NG();

            RealtimeFileR1.setDateTime(_data.DTM);
            RealtimeFileL1.setDateTime(_data.DTM);

            RealtimeFileR1.setRealtimeJudgement(_Rdata.Judgement);
            RealtimeFileL1.setRealtimeJudgement(_Ldata.Judgement);

            RealtimeFileR1.setRealtimeStep2(_Rdata.RealtimeStep2);
            RealtimeFileL1.setRealtimeStep2(_Ldata.RealtimeStep2);

            string DirRealtime = RealLogDir + $"YEAR_{_data.DTM[0]}\\MONTH_{_data.DTM[1]}\\DAY_{_data.DTM[2]}";
            CheckFolderPath(DirRealtime);

            if (_Rdata._Step1MaxLoad_NG == 1 | _Rdata._Step2CompRef_NG == 1 | _Rdata._Step2CompGraph_NG == 1 | _Rdata._Step2ExtnRef_NG == 1 | _Rdata._Step2ExtnGraph_NG == 1 | _Rdata._Step2DiffGraph_NG == 1)
            {
                RealtimeFileR1.SET_LABEL_NG();

                if(_Rdata._Step1MaxLoad_NG == 1)
                {
                    RealtimeFileR1.STEP1_MAXLOAD_NG_SET();
                }
                if (_Rdata._Step2CompRef_NG == 1)
                {
                    RealtimeFileR1.STEP2_COMP_REF_NG_SET();
                }
                if (_Rdata._Step2CompGraph_NG == 1)
                {
                    RealtimeFileR1.STEP2_COMP_GRAPH_NG_SET();
                }
                if (_Rdata._Step2ExtnRef_NG == 1)
                {
                    RealtimeFileR1.STEP2_EXTN_REF_NG_SET();
                }
                if (_Rdata._Step2ExtnGraph_NG == 1)
                {
                    RealtimeFileR1.STEP2_EXTN_GRAPH_NG_SET();
                }
                
                if (_Rdata._Step2DiffGraph_NG == 1)
                {
                    RealtimeFileR1.STEP2_DIFF_GRAPH_NG_SET();
                }

                string _filenameR1 = ($"{DirRealtime}\\RealtimeData_RH_{_data.DTM[3]}-{_data.DTM[4]}-{_data.DTM[5]}_NG_RESULT.xlsx");
                RealtimeFileR1.FilePrint(_filenameR1);
            }
            else
            {
                string _filenameR1 = ($"{DirRealtime}\\RealtimeData_RH_{_data.DTM[3]}-{_data.DTM[4]}-{_data.DTM[5]}.xlsx");
                RealtimeFileR1.FilePrint(_filenameR1);
            }

            if (_Ldata._Step1MaxLoad_NG == 1 | _Ldata._Step2CompRef_NG == 1 | _Ldata._Step2CompGraph_NG == 1 | _Ldata._Step2ExtnRef_NG == 1 | _Ldata._Step2ExtnGraph_NG == 1 | _Ldata._Step2DiffGraph_NG == 1)
            {
                RealtimeFileL1.SET_LABEL_NG();

                if (_Ldata._Step1MaxLoad_NG == 1)
                {
                    RealtimeFileL1.STEP1_MAXLOAD_NG_SET();
                }
                if (_Ldata._Step2CompRef_NG == 1)
                {
                    RealtimeFileL1.STEP2_COMP_REF_NG_SET();
                }
                if (_Ldata._Step2CompGraph_NG == 1)
                {
                    RealtimeFileL1.STEP2_COMP_GRAPH_NG_SET();
                }
                if (_Ldata._Step2ExtnRef_NG == 1)
                {
                    RealtimeFileL1.STEP2_EXTN_REF_NG_SET();
                }
                if (_Ldata._Step2ExtnGraph_NG == 1)
                {
                    RealtimeFileL1.STEP2_EXTN_GRAPH_NG_SET();
                }
                if (_Ldata._Step2DiffGraph_NG == 1)
                {
                    RealtimeFileL1.STEP2_DIFF_GRAPH_NG_SET();
                }

                string _filenameL1 = ($"{DirRealtime}\\RealtimeData_LH_{_data.DTM[3]}-{_data.DTM[4]}-{_data.DTM[5]}_NG_RESULT.xlsx");
                RealtimeFileL1.FilePrint(_filenameL1);
            }
            else
            {
                string _filenameL1 = ($"{DirRealtime}\\RealtimeData_LH_{_data.DTM[3]}-{_data.DTM[4]}-{_data.DTM[5]}.xlsx");
                RealtimeFileL1.FilePrint(_filenameL1);
            }

            //should this removed because there's already call for this outside the function after this execution
            _realtimeReadFlag = false;
            _kvconnObject.writeDataCommand("W0C2", "", "0");
            //should this removed because there's already call for this outside the function after this execution
        }

        void _excelReadMasterData(string modfile)
        {
            MasterFileActive.FileReadMaster(modfile);
        }


        void _backgroundMessageRecv()
        {
            try
            {
                if (_kvconnObject.getState())
                {
                    if (_kvconnObject.getAvail() > 0)
                    {
                        _kvconnObject.connRecv();
                        _kvMsgRecv = new string(Encoding.ASCII.GetString(_kvconnObject.getMsgRecv(), 0, _kvconnObject.getByteRecv()));
                        //_uiObject.setTextBox2(_kvMsgRecv);
                    }
                }
            }
            catch { }
        }

        void _eeipreadActiveModelData()
        {
            //try
            {
                byte[] _INPUT;
                _INPUT = _eeipObject.AssemblyObject.getInstance(0xA1);
                char[] _charINPUT;
                _charINPUT = System.Text.Encoding.ASCII.GetString(_INPUT).ToCharArray();
                Thread.Sleep(1);

                char[] _charModelBuff = new char[20];
                char[] _charNumBuff = new char[20];

                for (int i = 0; i < _charINPUT.Length; i++)
                {
                    if (i < 20)
                    {
                        if (i % 2 == 0)
                        {
                            if (i > _charINPUT.Length - 2)
                            {
                                _charModelBuff[i] = _charINPUT[i];
                            }
                            else
                            {
                                _charModelBuff[i] = _charINPUT[i + 1];
                            }
                        }
                        else if (i % 2 == 1)
                        {
                            _charModelBuff[i] = _charINPUT[i - 1];
                        }
                    }
                    else
                    {
                        if (i % 2 == 0)
                        {
                            if (i > _charINPUT.Length - 2)
                            {
                                _charNumBuff[i - 20] = _charINPUT[i];
                            }
                            else
                            {
                                _charNumBuff[i - 20] = _charINPUT[i + 1];
                            }
                        }
                        else if (i % 2 == 1)
                        {
                            _charNumBuff[i - 20] = _charINPUT[i - 1];
                        }
                    }
                }

                _data._activeModelName = string.Join("", _charModelBuff);
                //Debug.Write(_data._activeModelName);
                //Debug.Write((char)'\n');
                _data._activeKayabaNumber = string.Join("", _charNumBuff);
                //Debug.Write(_data._activeKayabaNumber);
                //Debug.Write((char)'\n');
            }
            //catch { }
        }

        void _eeipreadDateTime()
        {
            //try
            {
                byte[] _INPUT;
                List<int> _buffDTM = new List<int>();
                _INPUT = _eeipObject.AssemblyObject.getInstance(0xA2);
                Thread.Sleep(1);

                byte[] buff = new byte[2];
                int iv = 0;


                for (int i = 0; i < _INPUT.Length; i++)
                {
                    if (i % 2 == 0)
                    {
                        buff[0] = _INPUT[i];
                    }
                    else if (i % 2 == 1)
                    {
                        buff[1] = _INPUT[i];
                        byte[] sbuff = new byte[] { };
                        Array.Resize(ref sbuff, buff.Length);
                        Buffer.BlockCopy(buff, 0, sbuff, 0, sbuff.Length);

                        _buffDTM.Add(BitConverter.ToInt16(sbuff, 0));
                        
                    }
                }

                for (int i = 0; i < _data.DTM.Count(); i++)
                {
                    if (i == 0)
                    {
                        _data.DTM[i] = Convert.ToString(2000 + _buffDTM[i]);
                    }
                    else
                    {
                        _data.DTM[i] = _buffDTM[i].ToString();
                    }
                }
            }
            //catch { }
        }

        void _eeipreadStep1Param()
        {
            //try
            {
                byte[] _INPUT;
                List<byte[]> _buffPARAM1 = new List<byte[]>();
                _INPUT = _eeipObject.AssemblyObject.getInstance(0xA3);
                Thread.Sleep(1);

                byte[] buff = new byte[4];
                int iv = 0;

                for (int i = 0; i < _INPUT.Length; i++)
                {
                    if (i < 1)
                    {
                        buff[iv] = _INPUT[i];
                        iv++;
                    }
                    else if (i == _INPUT.Length - 1)
                    {
                        buff[iv] = _INPUT[i];
                        byte[] sbuff = new byte[] { };
                        Array.Resize(ref sbuff, buff.Length);
                        Buffer.BlockCopy(buff, 0, sbuff, 0, sbuff.Length);
                        _buffPARAM1.Add(sbuff);
                    }
                    else
                    {
                        if (i % 4 != 0)
                        {
                            buff[iv] = _INPUT[i];
                            iv++;
                        }
                        else if (i % 4 == 0)
                        {

                            byte[] sbuff = new byte[] { };
                            Array.Resize(ref sbuff, buff.Length);
                            Buffer.BlockCopy(buff, 0, sbuff, 0, sbuff.Length);

                            _buffPARAM1.Add(sbuff);
                            iv = 0;

                            buff[iv] = _INPUT[i];
                            iv++;
                        }
                    }

                }
                

                for (int i = 0; i < _data.Step1Param.Count(); i++)
                {
                    if (i == 0)
                    {
                        _data.Step1Param[0] = BitConverter.ToInt32(_buffPARAM1[i], 0);
                    }
                    else if (i == 4)
                    {
                        _data.Step1Param[4] = BitConverter.ToInt32(_buffPARAM1[i], 0);
                    }
                    else
                    {
                        _data.Step1Param[i] = BitConverter.ToSingle(_buffPARAM1[i], 0);
                    }

                }
            }
            //catch { }
        }

        void _eeipreadStep2345Param()
        {
            //try
            {
                byte[] _INPUT;
                List<byte[]> _buffPARAM2345 = new List<byte[]>();
                _INPUT = _eeipObject.AssemblyObject.getInstance(0xA4);
                Thread.Sleep(1);

                byte[] buff = new byte[4];
                int iv = 0;

                for (int i = 0; i < _INPUT.Length; i++)
                {
                    if (i < 1)
                    {
                        buff[iv] = _INPUT[i];
                        iv++;
                    }
                    else if (i == _INPUT.Length - 1)
                    {
                        buff[iv] = _INPUT[i];
                        byte[] sbuff = new byte[] { };
                        Array.Resize(ref sbuff, buff.Length);
                        Buffer.BlockCopy(buff, 0, sbuff, 0, sbuff.Length);
                        _buffPARAM2345.Add(sbuff);
                    }
                    else
                    {
                        if (i % 4 != 0)
                        {
                            buff[iv] = _INPUT[i];
                            iv++;
                        }
                        else if (i % 4 == 0)
                        {
                            byte[] sbuff = new byte[] { };
                            Array.Resize(ref sbuff, buff.Length);
                            Buffer.BlockCopy(buff, 0, sbuff, 0, sbuff.Length);

                            _buffPARAM2345.Add(sbuff);
                            iv = 0;

                            buff[iv] = _INPUT[i];
                            iv++;
                        }
                    }
                }

                for (int i = 0; i < _data.Step2345Param.Count(); i++)
                {
                    if (i == 0)
                    {
                        _data.Step2345Param[0] = BitConverter.ToInt16(_buffPARAM2345[i], 0);
                    }
                    else if (i == 9)
                    {
                        _data.Step2345Param[9] = BitConverter.ToInt16(_buffPARAM2345[i], 0);
                    }
                    else if (i == 10)
                    {
                        _data.Step2345Param[10] = BitConverter.ToInt16(_buffPARAM2345[i], 0);
                    }
                    else if (i == 19)
                    {
                        _data.Step2345Param[19] = BitConverter.ToInt16(_buffPARAM2345[i], 0);
                    }
                    else
                    {
                        _data.Step2345Param[i] = BitConverter.ToSingle(_buffPARAM2345[i], 0);
                    }
                }
            }
            //catch { }
        }

        void _eeipreadJudgement(ref List<float> judgementresult, Int16 addr)
        {
            //try
            {
                byte[] _INPUT = _eeipObject.AssemblyObject.getInstance(addr);
                Thread.Sleep(1);

                float[] _buffJudgement = new float[] { };
                byte[] buff = new byte[4];
                int iv = 0;
                int iz = 0;
                int iend = _INPUT.Length - 1;

                for (int i = 0; i < iend; i++)
                {
                    if (i < 1)
                    {
                        buff[iv] = _INPUT[i];
                        iv++;
                    }
                    else if (i == iend)
                    {
                        buff[iv] = _INPUT[i];

                        iz++;
                        Array.Resize(ref _buffJudgement, iz);
                        _buffJudgement[iz - 1] = BitConverter.ToSingle(buff, 0);
                        judgementresult[iz - 1] = _buffJudgement[iz - 1];
                        iv = 0;
                        Array.Clear(buff);
                    }
                    else
                    {
                        if (i % 4 != 0)
                        {
                            buff[iv] = _INPUT[i];
                            iv++;
                        }
                        else if (i % 4 == 0)
                        {
                            iz++;
                            Array.Resize(ref _buffJudgement, iz);
                            _buffJudgement[iz - 1] = BitConverter.ToSingle(buff, 0);
                            judgementresult[iz - 1] = _buffJudgement[iz - 1];
                            iv = 0;
                            Array.Clear(buff);

                            buff[iv] = _INPUT[i];
                            iv++;
                        }
                    }

                }
            }
            //catch { }
        }

        void _eeipreadRealtime(ref List<List<object>> realtimeresult, Int16 addr)
        {
            try
            {

            }
            catch { }
        }

        void _kvreadRealtime(ref List<List<float>> realtimeresult, string addr1, string addr2, string addr3, string addr4, string addr5, string addr6, int count)
        {
            try
            {
                realtimeresult.Clear();

                List<byte[]> comp_stroke = new List<byte[]>(_kvconnObject.batchreadDataCommandInHex(addr1, count));
                List<byte[]> comp_load = new List<byte[]>(_kvconnObject.batchreadDataCommandInHex(addr2, count));
                List<byte[]> extn_stroke = new List<byte[]>(_kvconnObject.batchreadDataCommandInHex(addr3, count));
                List<byte[]> extn_load = new List<byte[]>(_kvconnObject.batchreadDataCommandInHex(addr4, count));
                List<byte[]> diff_stroke = new List<byte[]>(_kvconnObject.batchreadDataCommandInHex(addr5, count));
                List<byte[]> diff_load = new List<byte[]>(_kvconnObject.batchreadDataCommandInHex(addr6, count));
                
                realtimeresult.Add(hex16tofloat32(comp_stroke));
                realtimeresult.Add(hex16tofloat32(comp_load));
                realtimeresult.Add(hex16tofloat32(extn_stroke));
                realtimeresult.Add(hex16tofloat32(extn_load));
                realtimeresult.Add(hex16tofloat32(diff_stroke));
                realtimeresult.Add(hex16tofloat32(diff_load));
            }
            catch { }
        }

        void _kvreadMaster(ref List<List<float>> masterdata, string addr1, string addr2, string addr3, string addr4, string addr5, string addr6, string addr7, string addr8, string addr9, string addr10, string addr11, string addr12, int count)
        {
            try
            {
                masterdata.Clear();

                List<byte[]> comp_stroke = new List<byte[]>(_kvconnObject.batchreadDataCommandInHex(addr1, count));
                List<byte[]> comp_load = new List<byte[]>(_kvconnObject.batchreadDataCommandInHex(addr2, count));
                List<byte[]> comp_upper = new List<byte[]>(_kvconnObject.batchreadDataCommandInHex(addr3, count));
                List<byte[]> comp_lower = new List<byte[]>(_kvconnObject.batchreadDataCommandInHex(addr4, count));
                List<byte[]> extn_stroke = new List<byte[]>(_kvconnObject.batchreadDataCommandInHex(addr5, count));
                List<byte[]> extn_load = new List<byte[]>(_kvconnObject.batchreadDataCommandInHex(addr6, count));
                List<byte[]> extn_upper = new List<byte[]>(_kvconnObject.batchreadDataCommandInHex(addr7, count));
                List<byte[]> extn_lower = new List<byte[]>(_kvconnObject.batchreadDataCommandInHex(addr8, count));
                List<byte[]> diff_stroke = new List<byte[]>(_kvconnObject.batchreadDataCommandInHex(addr9, count));
                List<byte[]> diff_load = new List<byte[]>(_kvconnObject.batchreadDataCommandInHex(addr10, count));
                List<byte[]> diff_upper = new List<byte[]>(_kvconnObject.batchreadDataCommandInHex(addr11, count));
                List<byte[]> diff_lower = new List<byte[]>(_kvconnObject.batchreadDataCommandInHex(addr12, count));

                masterdata.Add(hex16tofloat32(comp_stroke));
                masterdata.Add(hex16tofloat32(comp_load));
                masterdata.Add(hex16tofloat32(comp_upper));
                masterdata.Add(hex16tofloat32(comp_lower));
                masterdata.Add(hex16tofloat32(extn_stroke));
                masterdata.Add(hex16tofloat32(extn_load));
                masterdata.Add(hex16tofloat32(extn_upper));
                masterdata.Add(hex16tofloat32(extn_lower));
                masterdata.Add(hex16tofloat32(diff_stroke));
                masterdata.Add(hex16tofloat32(diff_load));
                masterdata.Add(hex16tofloat32(diff_upper));
                masterdata.Add(hex16tofloat32(diff_lower));
            }
            catch { }
        }

        public static string floattostring(float pf)
        {
            return new string(pf.ToString());
        }

        List<float> hex16tofloat32(List<byte[]> hexdata)
        {
            List<float> floatdata = new List<float>();
            List<byte[]> buffs = new List<byte[]>();
            byte[] qbytebuff = new byte[4];
            int iend = hexdata.Count - 1;
            int iv = 0;

            for (int i = 0; i < iend; i++)
            {
                if (i % 2 == 0)
                {
                    for (int ivy = 0; ivy < hexdata[i].Length; ivy++)
                    {
                        qbytebuff[iv] = hexdata[i][ivy];
                        iv++;
                    }
                }
                else if (i % 2 != 0)
                {
                    for (int ivy = 0; ivy < hexdata[i].Length; ivy++)
                    {
                        qbytebuff[iv] = hexdata[i][ivy];
                        iv++;
                    }
                    floatdata.Add(BitConverter.ToSingle(qbytebuff, 0));
                    iv = 0;
                    Array.Clear(qbytebuff);
                }
            }
            return floatdata;
        }

        List<float> hex16tofloat32_InvertedList(List<byte[]> hexdata)
        {
            List<float> floatdata = new List<float>();
            List<byte[]> buffs = new List<byte[]>();
            byte[] qbytebuff = new byte[4];
            int iend = hexdata.Count - 1;
            int iv = 0;

            for (int i = iend; i >= 0; i--)
            {
                if (i % 2 == 0)
                {
                    for (int ivy = 0; ivy < hexdata[i].Length; ivy++)
                    {
                        qbytebuff[iv] = hexdata[i][ivy];
                        iv++;
                    }
                }
                else if (i % 2 != 0)
                {
                    for (int ivy = 0; ivy < hexdata[i].Length; ivy++)
                    {
                        qbytebuff[iv] = hexdata[i][ivy];
                        iv++;
                    }
                    floatdata.Add(BitConverter.ToSingle(qbytebuff, 0));
                    iv = 0;
                    Array.Clear(qbytebuff);
                }
            }
            return floatdata;
        }

        double[] _bytearrayToDoubleXAxis(byte[] bytearraystream)
        {
            byte[] streaminput = new byte[bytearraystream.Length];
            Array.Copy(bytearraystream, streaminput, bytearraystream.Length);
            List<byte[]> _buffList = new List<byte[]>();
            double[] _buffResult = new double[] { };

            //try
            {
                byte[] buff = new byte[4];
                int iv = 0;
                int iz = 0;
                int iend = streaminput.Length - 1;

                for (int i = 0; i < streaminput.Length; i++)
                {
                    if (i < 1)
                    {
                        buff[iv] = streaminput[i];
                        iv++;
                    }
                    else if (i == iend)
                    {
                        buff[iv] = streaminput[i];

                        iz++;
                        double dbuff = Convert.ToDouble(BitConverter.ToInt32(buff, 0));
                        Array.Resize(ref _buffResult, iz); _buffResult[iz - 1] = dbuff;
                        //if (dbuff != 0) { Array.Resize(ref _buffResult, iz); _buffResult[iz - 1] = dbuff; }
                        iv = 0;
                        Array.Clear(buff);

                        //Debug.Write(_buffResult[iz - 1]);
                        //Debug.Write((char)'\n');
                    }
                    else
                    {
                        if (i % 4 != 0)
                        {
                            buff[iv] = streaminput[i];
                            iv++;
                        }
                        else if (i % 4 == 0)
                        {
                            iz++;
                            double dbuff = Convert.ToDouble(BitConverter.ToInt32(buff, 0));
                            Array.Resize(ref _buffResult, iz); _buffResult[iz - 1] = dbuff;
                            //if (dbuff != 0) { Array.Resize(ref _buffResult, iz); _buffResult[iz - 1] = dbuff; }
                            iv = 0;
                            Array.Clear(buff);

                            buff[iv] = streaminput[i];
                            iv++;

                            //Debug.Write(_buffResult[iz - 1]);
                            //Debug.Write((char)'\n');
                        }
                    }
                }
            }
            //catch { }
            return _buffResult;
        }

        double[] _bytearrayToDoubleYAxis(byte[] bytearraystream)
        {
            byte[] streaminput = new byte[bytearraystream.Length];
            Array.Copy(bytearraystream, streaminput, bytearraystream.Length);
            List<byte[]> _buffList = new List<byte[]>();
            double[] _buffResult = new double[] { };

            //try
            {
                byte[] buff = new byte[4];
                int iv = 0;
                int iz = 0;
                int iend = streaminput.Length - 1;

                for (int i = 0; i < streaminput.Length; i++)
                {
                    if (i < 1)
                    {
                        buff[iv] = streaminput[i];
                        iv++;
                    }
                    else if (i == iend)
                    {
                        buff[iv] = streaminput[i];

                        iz++;
                        double dbuff = Convert.ToDouble(BitConverter.ToInt32(buff, 0));
                        Array.Resize(ref _buffResult, iz); _buffResult[iz - 1] = dbuff;
                        //if (dbuff != 0) { Array.Resize(ref _buffResult, iz); _buffResult[iz - 1] = dbuff; }
                        iv = 0;
                        Array.Clear(buff);

                        //Debug.Write(_buffResult[iz - 1]);
                        //Debug.Write((char)'\n');
                    }
                    else
                    {
                        if (i % 4 != 0)
                        {
                            buff[iv] = streaminput[i];
                            iv++;
                        }
                        else if (i % 4 == 0)
                        {
                            iz++;
                            double dbuff = Convert.ToDouble(BitConverter.ToInt32(buff, 0));
                            Array.Resize(ref _buffResult, iz); _buffResult[iz - 1] = dbuff;
                            //if (dbuff != 0) { Array.Resize(ref _buffResult, iz); _buffResult[iz - 1] = dbuff; }
                            iv = 0;
                            Array.Clear(buff);

                            buff[iv] = streaminput[i];
                            iv++;

                            //Debug.Write(_buffResult[iz - 1]);
                            //Debug.Write((char)'\n');
                        }
                    }
                }
            }
            //catch { }
            return _buffResult;
        }

        static decimal[] FloatToDecimal(float[] floatArray)
        {
            // Initialize a decimal array with the same length as the float array
            decimal[] decimalArray = new decimal[floatArray.Length];

            // Convert each float to decimal
            for (int i = 0; i < floatArray.Length; i++)
            {
                decimalArray[i] = (decimal)floatArray[i];
            }

            return decimalArray;
        }

        static uint FloatToUInt32(float value)
        {
            // Get the byte array from the float
            byte[] bytes = BitConverter.GetBytes(value);

            // Convert the byte array to an unsigned 32-bit integer
            return BitConverter.ToUInt32(bytes, 0);
        }

        static uint[] ConvertFloatArrayToUInt32Array(float[] floatArray)
        {
            uint[] unsignedIntArray = new uint[floatArray.Length];

            for (int i = 0; i < floatArray.Length; i++)
            {
                unsignedIntArray[i] = FloatToUInt32(floatArray[i]);
            }

            return unsignedIntArray;
        }

        static string[] ConvertFloatArrayToUInt32StringArray(float[] floatArray)
        {
            string[] unsignedIntStringArray = new string[floatArray.Length];

            for (int i = 0; i < floatArray.Length; i++)
            {
                // Convert float to unsigned 32-bit integer and then to string
                uint unsignedIntValue = BitConverter.ToUInt32(BitConverter.GetBytes(floatArray[i]), 0);
                unsignedIntStringArray[i] = unsignedIntValue.ToString();
            }

            return unsignedIntStringArray;
        }

        static uint[] StringToUInt32Array(string str)
        {
            int newSize = (str.Length + 1) / 2;
            uint[] uint32Values = new uint[newSize];

            for (int i = 0; i < str.Length; i += 2)
            {
                uint firstCharValue = (uint)str[i];
                uint secondCharValue = (i + 1 < str.Length) ? (uint)str[i + 1] : 0;

                uint32Values[i / 2] = (firstCharValue << 16) | secondCharValue;
            }

            return uint32Values;
        }

        static ushort[] StringToUInt16Array(string str)
        {
            // Calculate the size of the new array
            int newSize = (str.Length + 1) / 2; // Round up for odd lengths
            ushort[] uint16Values = new ushort[newSize];

            for (int i = 0; i < str.Length; i += 2)
            {
                // Get the first character (higher byte)
                byte firstCharValue = (byte)str[i];

                // Get the second character (lower byte) if it exists
                byte secondCharValue = (i + 1 < str.Length) ? (byte)str[i + 1] : (byte)0; // Use 0 if there's no second character

                // Combine the two characters into one ushort value
                uint16Values[i / 2] = (ushort)((firstCharValue << 8) | secondCharValue); // Shift first char and combine
            }

            return uint16Values;
        }

        static string[] ConvertStringToUInt32StringArray(string stringArray)
        {
            uint[] uints = StringToUInt32Array(stringArray);
            string[] unsignedIntStringArray = new string[uints.Length];

            for (int i = 0; i < unsignedIntStringArray.Length; i++)
            {
                unsignedIntStringArray[i] = uints[i].ToString();
            }
            return unsignedIntStringArray;
        }

        static string[] ConvertStringToUInt16StringArray(string stringArray)
        {
            ushort[] ushorts = StringToUInt16Array(stringArray);
            string[] unsignedInt16StringArray = new string[ushorts.Length];

            for (int i = 0; i < unsignedInt16StringArray.Length; i++)
            {
                unsignedInt16StringArray[i] = ushorts[i].ToString();
            }
            return unsignedInt16StringArray;
        }

        private async Task InvokeAsync(Action action, CancellationToken cancellationToken)
        {
            await Task.Run(() => action(), cancellationToken);
        }

        public async Task BackgroundWorkAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await _eeipEventHandler_1Async(cancellationToken);
                await _eeipEventHandler_2Async(cancellationToken);
                await _eeipEventHandler_3Async(cancellationToken);
                await _uibeaconnUpdateAsync(cancellationToken);
                await Task.Delay(10, cancellationToken);
            }
        }

        public async void BackgroundWork()
        {
            int counter = 0;
            while (counter < 1)
            {
                counter++;
                Thread.Sleep(10);
            }
            while (true)
            {
                _cts = new CancellationTokenSource();

                while (!_cts.Token.IsCancellationRequested)
                {
                    await _uibeaconnUpdateAsync(_cts.Token);
                    await _eeipEventHandler_1Async(_cts.Token);
                    await _eeipEventHandler_2Async(_cts.Token);
                    await _eeipEventHandler_3Async(_cts.Token);

                    //await _uiPlot1UpdateAsync(_cts.Token);
                    //await _backgroundDataPlot1ReadAsync(_cts.Token);

                    //await _uiPlot2UpdateAsync(_cts.Token);
                    //await _backgroundDataPlot2ReadAsync(_cts.Token);

                    //await _uiPlot3UpdateAsync(_cts.Token);
                    //await _backgroundDataPlot3ReadAsync(_cts.Token);

                    //await _uiPlot4UpdateAsync(_cts.Token);
                   // await _backgroundDataPlot4ReadAsync(_cts.Token);
                }
                Thread.Sleep(1);
            }
        }

        public void abortTasks()
        {
            _cts.Cancel();
        }

        private async Task _eeipEventHandler_1Async(CancellationToken cancellationToken)
        {
            await InvokeAsync(() => _eeipEventHandler_1(), cancellationToken);
        }

        private async Task _eeipEventHandler_2Async(CancellationToken cancellationToken)
        {
            await InvokeAsync(() => _eeipEventHandler_2(), cancellationToken);
        }

        private async Task _eeipEventHandler_3Async(CancellationToken cancellationToken)
        {
            await InvokeAsync(() => _eeipEventHandler_3(), cancellationToken);
        }

        private async Task _uibeaconnUpdateAsync(CancellationToken cancellationToken)
        {
            await InvokeAsync(() => _uibeaconnUpdate(), cancellationToken);
        }

        private async Task _backgroundMessageRecvAsync(CancellationToken cancellationToken)
        {
            await InvokeAsync(() => _backgroundMessageRecv(), cancellationToken);
        }

        private async Task _uiPlot1UpdateAsync(CancellationToken cancellationToken)
        {
            await InvokeAsync(() => _uiPlot1Update(), cancellationToken);
        }

        private async Task _backgroundDataPlot1ReadAsync(CancellationToken cancellationToken)
        {
            await InvokeAsync(() => _backgroundDataPlot1Read(), cancellationToken);
        }

        private async Task _uiPlot2UpdateAsync(CancellationToken cancellationToken)
        {
            await InvokeAsync(() => _uiPlot2Update(), cancellationToken);
        }

        private async Task _backgroundDataPlot2ReadAsync(CancellationToken cancellationToken)
        {
            await InvokeAsync(() => _backgroundDataPlot2Read(), cancellationToken);
        }

        private async Task _uiPlot3UpdateAsync(CancellationToken cancellationToken)
        {
            await InvokeAsync(() => _uiPlot3Update(), cancellationToken);
        }

        private async Task _backgroundDataPlot3ReadAsync(CancellationToken cancellationToken)
        {
            await InvokeAsync(() => _backgroundDataPlot3Read(), cancellationToken);
        }

        private async Task _uiPlot4UpdateAsync(CancellationToken cancellationToken)
        {
            await InvokeAsync(() => _uiPlot4Update(), cancellationToken);
        }

        private async Task _backgroundDataPlot4ReadAsync(CancellationToken cancellationToken)
        {
            await InvokeAsync(() => _backgroundDataPlot4Read(), cancellationToken);
        }



        private void _uibeaconnUpdate()
        {
            if (this.GetConnState() == 1)
            {
                if (_uiObject.InvokeRequired)
                {
                    _uiObject.BeginInvoke(new MethodInvoker(_uiObject.connStatLampOn));
                }
            }
            else
            {
                if (_uiObject.InvokeRequired)
                {
                    _uiObject.BeginInvoke(new MethodInvoker(_uiObject.connStatLampOff));
                }
            }

            if (_uiObject._beaconn == 1)
            {
                if (_uiObject.InvokeRequired)
                {
                    _uiObject.BeginInvoke(new MethodInvoker(_uiObject.beaconnStatLampOn));
                }
            }
            else if (_uiObject._beaconn == 0)
            {
                if (_uiObject.InvokeRequired)
                {
                    _uiObject.BeginInvoke(new MethodInvoker(_uiObject.beaconnStatLampOff));
                }
            }

            Thread.Sleep(1);
        }

        //double[] _dXD1;
        public double[] dXD1;
        public double[] dXD2;
        public double[] dXD3;
        public double[] dXD4;

        //double[] _dYD1;
        public double[] dYD1;
        public double[] dYD2;
        public double[] dYD3;
        public double[] dYD4;

        System.Drawing.Color D1Col;
        System.Drawing.Color D2Col;
        System.Drawing.Color D3Col;
        System.Drawing.Color D4Col;

        bool _uiPlot1UpdateFlag;
        bool _uiPlot2UpdateFlag;
        bool _uiPlot3UpdateFlag;
        bool _uiPlot4UpdateFlag;

        bool _uiPlot1ResetFlag;
        bool _uiPlot2ResetFlag;
        bool _uiPlot3ResetFlag;
        bool _uiPlot4ResetFlag;

        

        private void _uiPlotClear()
        {
            if (_uiObject.InvokeRequired)
            {
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.AllPlotReset()));
            }
            else
            {
                _uiObject.AllPlotReset();
            }
        }

        private void _uiPlot1Update()
        {
            if (_uiPlot1UpdateFlag)
            {
                double[] xd = new double[dXD1.Length];
                Array.Copy(dXD1, xd, dXD1.Length);

                double[] yd = new double[dYD1.Length];
                Array.Copy(dYD1, yd, dYD1.Length);

                if (_uiObject.InvokeRequired)
                {
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.Plot1Update(xd, yd, D1Col)));
                }
                else
                {
                    _uiObject.Plot1Update(xd, yd, D1Col);
                }

                _uiPlot1UpdateFlag = false;

                //Thread.Sleep(10);
            }
        }

        private void _uiPlot2Update()
        {
            if (_uiPlot2UpdateFlag)
            {
                double[] xd = new double[dXD2.Length];
                Array.Copy(dXD2, xd, dXD2.Length);

                double[] yd = new double[dYD2.Length];
                Array.Copy(dYD2, yd, dYD2.Length);

                if (_uiObject.InvokeRequired)
                {
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.Plot2Update(xd, yd, D2Col)));
                }
                else
                {
                    _uiObject.Plot2Update(xd, yd, D2Col);
                }

                _uiPlot2UpdateFlag = false;

                //Thread.Sleep(10);
            }
        }

        private void _uiPlot3Update()
        {
            if (_uiPlot3UpdateFlag)
            {
                double[] xd = new double[dXD3.Length];
                Array.Copy(dXD3, xd, dXD3.Length);

                double[] yd = new double[dYD3.Length];
                Array.Copy(dYD3, yd, dYD3.Length);

                if (_uiObject.InvokeRequired)
                {
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.Plot3Update(xd, yd, D3Col)));
                }
                else
                {
                    _uiObject.Plot3Update(xd, yd, D3Col);
                }

                _uiPlot3UpdateFlag = false;

                //Thread.Sleep(10);
            }
        }

        private void _uiPlot4Update()
        {
            if (_uiPlot4UpdateFlag)
            {
                double[] xd = new double[dXD4.Length];
                Array.Copy(dXD4, xd, dXD4.Length);

                double[] yd = new double[dYD4.Length];
                Array.Copy(dYD4, yd, dYD4.Length);

                if (_uiObject.InvokeRequired)
                {
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.Plot4Update(xd, yd, D4Col)));
                }
                else
                {
                    _uiObject.Plot4Update(xd, yd, D4Col);
                }

                _uiPlot4UpdateFlag = false;

                //Thread.Sleep(10);
            }
        }

        int parse1_idx;
        private void _backgroundDataPlot1Read()
        {
            if (this.GetConnState() == 1)
            {

                //List<byte[]> comp_stroke = new List<byte[]>(_kvconnObject.batchreadDataCommandInHex("ZF110000", 400));
                //List<byte[]> comp_load = new List<byte[]>(_kvconnObject.batchreadDataCommandInHex("ZF110400", 400));

                //List<float> float_comp_stroke = new List<float>(hex16tofloat32(comp_stroke));
                //List<float> float_comp_load = new List<float>(hex16tofloat32(comp_load));

                //float[] fXD1 = float_comp_stroke.ToArray();
                //float[] fYD1 = float_comp_load.ToArray();

                float[] fXD1 = _Rdata.RealtimeStep2[0].ToArray();
                float[] fYD1 = _Rdata.RealtimeStep2[1].ToArray();

                int idxx = 0;
                for (int i = 0; i < fXD1.Length; i++)
                {
                    if (fXD1[i] != 0 && i != 0)
                    {
                        Array.Resize(ref dXD1, idxx + 1);
                        if((double)fXD1[i] == dXD1[idxx-1]) 
                        {
                            dXD1[idxx] = (double)fXD1[i] + 1;
                        }
                        else
                        {
                            dXD1[idxx] = (double)fXD1[i];
                        }
                        idxx++;
                    }
                    else if(i == 0)
                    {
                        Array.Resize(ref dXD1, idxx + 1);
                        dXD1[idxx] = (double)fXD1[i];
                        idxx++;
                    }
                }

                int idxy = 0;
                for (int i = 0; i < dXD1.Length; i++)
                {
                    Array.Resize(ref dYD1, idxy + 1);
                    dYD1[idxy] = (double)fYD1[i];
                    idxy++;
                }

                //dXD1 = Array.ConvertAll(fXD1, x => (x != 0) ? (double)x);
                //dYD1 = Array.ConvertAll(fYD1, x => (x != 0) ? (double)x);

                if (dXD1.Count() != dYD1.Count())
                {
                    _uiPlot1UpdateFlag = false;
                }

                _uiPlot1UpdateFlag = true;
                /*
                byte[] TRIG = _eeipObject.AssemblyObject.getInstance(0x8E);
                if ((byte)(TRIG[0] & 0x02) == 0x02)
                {
                    parse1_idx = 0;
                    dXD1 = new double[] { };
                    dYD1 = new double[] { };

                    Array.Resize(ref dXD1, 1);
                    Array.Resize(ref dYD1, 1);

                    byte[] RH_COMP_STROKE_REALTIME_PARSE = _eeipObject.AssemblyObject.getInstance(0xB4);
                    dXD1[parse1_idx] = _bytearrayToDoubleXAxis(RH_COMP_STROKE_REALTIME_PARSE)[0];

                    byte[] RH_COMP_LOAD_REALTIME_PARSE = _eeipObject.AssemblyObject.getInstance(0xB5);
                    dYD1[parse1_idx] = _bytearrayToDoubleYAxis(RH_COMP_LOAD_REALTIME_PARSE)[0];

                    _kvconnObject.writeDataCommand("W0FE0", "", "3");

                    _uiPlot1UpdateFlag = true;
                }
                else if ((byte)(TRIG[0] & 0x01) == 0x01)
                {
                    parse1_idx += 1;
                    Array.Resize(ref dXD1, dXD1.Length + 1);
                    Array.Resize(ref dYD1, dYD1.Length + 1);

                    byte[] RH_COMP_STROKE_REALTIME_PARSE = _eeipObject.AssemblyObject.getInstance(0xB4);
                    dXD1[parse1_idx] = _bytearrayToDoubleXAxis(RH_COMP_STROKE_REALTIME_PARSE)[0];

                    byte[] RH_COMP_LOAD_REALTIME_PARSE = _eeipObject.AssemblyObject.getInstance(0xB5);
                    dYD1[parse1_idx] = _bytearrayToDoubleYAxis(RH_COMP_LOAD_REALTIME_PARSE)[0];

                    _kvconnObject.writeDataCommand("W0FE0", "", "3");

                    _uiPlot1UpdateFlag = true;
                }
                */
            }
        }

        int parse2_idx;
        private void _backgroundDataPlot2Read()
        {
            if (this.GetConnState() == 1)
            {
                //List<byte[]> extn_stroke = new List<byte[]>(_kvconnObject.batchreadDataCommandInHex("ZF110800", 400));
                //List<byte[]> extn_load = new List<byte[]>(_kvconnObject.batchreadDataCommandInHex("ZF111200", 400));

                //List<float> float_extn_stroke = new List<float>(hex16tofloat32_InvertedList(extn_stroke));
                //List<float> float_extn_load = new List<float>(hex16tofloat32_InvertedList(extn_load));

                //float[] fXD2 = float_extn_stroke.ToArray();
                //float[] fYD2 = float_extn_load.ToArray();

                float[] fXD2 = _Rdata.RealtimeStep2[2].ToArray();
                Array.Reverse(fXD2);
                float[] fYD2 = _Rdata.RealtimeStep2[3].ToArray();
                Array.Reverse(fYD2);
                //int idxx = 0;
                //int idxy = 0;
                //foreach (float x in fXD2) { if (x != 0) idxx++; }
                //foreach (float y in fYD2) { if (y != 0) idxy++; }

                //Array.Resize(ref dXD2, idxx + 1);
                //Array.Resize(ref dYD2, idxy);

                //int idx = 0;
                //int idy = 0;
                //foreach (float x in fXD2) { if (x != 0) { dXD2[idx] = (double)x; idx++; } }
                //foreach (float y in fYD2) { if (y != 0) { dYD2[idy] = (double)y; idy++; } }

                dXD2 = Array.ConvertAll(fXD2, x => (double)x);
                dYD2 = Array.ConvertAll(fYD2, x => (double)x);

                if (dXD2.Count() != dYD2.Count())
                {
                    _uiPlot2UpdateFlag = false;
                }

                _uiPlot2UpdateFlag = true;
                /*
                byte[] TRIG = _eeipObject.AssemblyObject.getInstance(0x8E);
                if ((byte)(TRIG[2] & 0x02) == 0x02)
                {
                    parse2_idx = 0;
                    dXD2 = new double[] { };
                    dYD2 = new double[] { };

                    Array.Resize(ref dXD2, 1);
                    Array.Resize(ref dYD2, 1);

                    byte[] RH_EXTN_STROKE_REALTIME = _eeipObject.AssemblyObject.getInstance(0xB6);
                    dXD2[parse2_idx] = _bytearrayToDoubleXAxis(RH_EXTN_STROKE_REALTIME)[0];

                    byte[] RH_EXTN_LOAD_REALTIME = _eeipObject.AssemblyObject.getInstance(0xB7);
                    dYD2[parse2_idx] = _bytearrayToDoubleYAxis(RH_EXTN_LOAD_REALTIME)[0];

                    _kvconnObject.writeDataCommand("W0FE1", "", "3");
                    _uiPlot2UpdateFlag = true;
                }
                else if ((byte)(TRIG[2] & 0x01) == 0x01)
                {
                    parse2_idx += 1;
                    Array.Resize(ref dXD2, dXD2.Length + 1);
                    Array.Resize(ref dYD2, dYD2.Length + 1);

                    byte[] RH_EXTN_STROKE_REALTIME = _eeipObject.AssemblyObject.getInstance(0xB6);
                    dXD2[parse2_idx] = _bytearrayToDoubleXAxis(RH_EXTN_STROKE_REALTIME)[0];

                    byte[] RH_EXTN_LOAD_REALTIME = _eeipObject.AssemblyObject.getInstance(0xB7);
                    dYD2[parse2_idx] = _bytearrayToDoubleYAxis(RH_EXTN_LOAD_REALTIME)[0];

                    _kvconnObject.writeDataCommand("W0FE1", "", "3");
                    _uiPlot2UpdateFlag = true;
                }
                */
            }
        }

        int parse3_idx;
        private void _backgroundDataPlot3Read()
        {
            if (this.GetConnState() == 1)
            {
                //List<byte[]> comp_stroke = new List<byte[]>(_kvconnObject.batchreadDataCommandInHex("ZF210000", 400));
                //List<byte[]> comp_load = new List<byte[]>(_kvconnObject.batchreadDataCommandInHex("ZF210400", 400));

                //List<float> float_comp_stroke = new List<float>(hex16tofloat32(comp_stroke));
                //List<float> float_comp_load = new List<float>(hex16tofloat32(comp_load));

                //float[] fXD3 = float_comp_stroke.ToArray();
                //float[] fYD3 = float_comp_load.ToArray();

                float[] fXD3 = _Ldata.RealtimeStep2[0].ToArray();
                float[] fYD3 = _Ldata.RealtimeStep2[1].ToArray();

                int idxx = 0;
                int idxy = 0;
                for (int i = 0; i < fXD3.Length; i++)
                {
                    if (fXD3[i] != 0 && i != 0)
                    {
                        Array.Resize(ref dXD3, idxx + 1);
                        if ((double)fXD3[i] == dXD3[idxx - 1])
                        {
                            dXD3[idxx] = (double)fXD3[i] + 1;
                        }
                        else
                        {
                            dXD3[idxx] = (double)fXD3[i];
                        }
                        idxx++;
                    }
                    else if (i == 0)
                    {
                        Array.Resize(ref dXD3, idxx + 1);
                        dXD3[idxx] = (double)fXD3[i];
                        idxx++;
                    }
                }

                for (int i = 0; i < dXD3.Length; i++)
                {
                    Array.Resize(ref dYD3, idxy + 1);
                    dYD3[idxy] = (double)fYD3[i];
                    idxy++;
                }

                //dXD3 = Array.ConvertAll(fXD3, x => (x != 0) ? (double)x);
                //dYD3 = Array.ConvertAll(fYD3, x => (x != 0) ? (double)x);

                if (dXD3.Count() != dYD3.Count())
                {
                    _uiPlot3UpdateFlag = false;
                }

                _uiPlot3UpdateFlag = true;
                /*
                byte[] TRIG = _eeipObject.AssemblyObject.getInstance(0x8E);
                if ((byte)(TRIG[4] & 0x02) == 0x02)
                {
                    parse3_idx = 0;
                    dXD3 = new double[] { };
                    dYD3 = new double[] { };

                    Array.Resize(ref dXD3, 1);
                    Array.Resize(ref dYD3, 1);

                    byte[] LH_COMP_STROKE_REALTIME = _eeipObject.AssemblyObject.getInstance(0xB8);
                    dXD3[parse3_idx] = _bytearrayToDoubleXAxis(LH_COMP_STROKE_REALTIME)[0];

                    byte[] LH_COMP_LOAD_REALTIME = _eeipObject.AssemblyObject.getInstance(0xB9);
                    dYD3[parse3_idx] = _bytearrayToDoubleYAxis(LH_COMP_LOAD_REALTIME)[0];

                    _kvconnObject.writeDataCommand("W0FE2", "", "3");
                    _uiPlot3UpdateFlag = true;
                }
                else if ((byte)(TRIG[4] & 0x01) == 0x01)
                {
                    parse3_idx += 1;
                    Array.Resize(ref dXD3, dXD3.Length + 1);
                    Array.Resize(ref dYD3, dYD3.Length + 1);

                    byte[] LH_COMP_STROKE_REALTIME = _eeipObject.AssemblyObject.getInstance(0xB8);
                    dXD3[parse3_idx] = _bytearrayToDoubleXAxis(LH_COMP_STROKE_REALTIME)[0];

                    byte[] LH_COMP_LOAD_REALTIME = _eeipObject.AssemblyObject.getInstance(0xB9);
                    dYD3[parse3_idx] = _bytearrayToDoubleYAxis(LH_COMP_LOAD_REALTIME)[0];

                    _kvconnObject.writeDataCommand("W0FE2", "", "3");
                    _uiPlot3UpdateFlag = true;
                }
                */
            }
        }

        int parse4_idx;
        private void _backgroundDataPlot4Read()
        {
            if (this.GetConnState() == 1)
            {
                //List<byte[]> extn_stroke = new List<byte[]>(_kvconnObject.batchreadDataCommandInHex("ZF210800", 400));
                //List<byte[]> extn_load = new List<byte[]>(_kvconnObject.batchreadDataCommandInHex("ZF211200", 400));

                //List<float> float_extn_stroke = new List<float>(hex16tofloat32_InvertedList(extn_stroke));
                //List<float> float_extn_load = new List<float>(hex16tofloat32_InvertedList(extn_load));

                //float[] fXD4 = float_extn_stroke.ToArray();
                //float[] fYD4 = float_extn_load.ToArray();

                float[] fXD4 = _Ldata.RealtimeStep2[2].ToArray();
                Array.Reverse(fXD4);
                float[] fYD4 = _Ldata.RealtimeStep2[3].ToArray();
                Array.Reverse(fYD4);

                //int idxx = 0;
                //int idxy = 0;
                //foreach (float x in fXD4) { if (x != 0) idxx++; }
                //foreach (float y in fYD4) { if (y != 0) idxy++; }

                //Array.Resize(ref dXD4, idxx + 1);
                //Array.Resize(ref dYD4, idxy);

                //int idx = 0;
                //int idy = 0;
                //foreach (float x in fXD4) { if (x != 0) { dXD4[idx] = (double)x; idx++; } }
                //foreach (float y in fYD4) { if (y != 0) { dYD4[idy] = (double)y; idy++; } }

                dXD4 = Array.ConvertAll(fXD4, x => (double)x);
                dYD4 = Array.ConvertAll(fYD4, x => (double)x);

                if (dXD4.Count() != dYD4.Count())
                {
                    _uiPlot4UpdateFlag = false;
                }

                _uiPlot4UpdateFlag = true;
                /*
                byte[] TRIG = _eeipObject.AssemblyObject.getInstance(0x8E);
                if ((byte)(TRIG[6] & 0x02) == 0x02)
                {
                    parse4_idx = 0;
                    dXD4 = new double[] { };
                    dYD4 = new double[] { };

                    Array.Resize(ref dXD4, 1);
                    Array.Resize(ref dYD4, 1);

                    byte[] LH_EXTN_STROKE_REALTIME = _eeipObject.AssemblyObject.getInstance(0xBA);
                    dXD4[parse4_idx] = _bytearrayToDoubleXAxis(LH_EXTN_STROKE_REALTIME)[0];

                    byte[] LH_EXTN_LOAD_REALTIME = _eeipObject.AssemblyObject.getInstance(0xBB);
                    dYD4[parse4_idx] = _bytearrayToDoubleYAxis(LH_EXTN_LOAD_REALTIME)[0];

                    _kvconnObject.writeDataCommand("W0FE3", "", "3");
                    _uiPlot4UpdateFlag = true;
                }
                else if ((byte)(TRIG[6] & 0x01) == 0x01)
                {
                    parse4_idx += 1;
                    Array.Resize(ref dXD4, dXD4.Length + 1);
                    Array.Resize(ref dYD4, dYD4.Length + 1);

                    byte[] LH_EXTN_STROKE_REALTIME = _eeipObject.AssemblyObject.getInstance(0xBA);
                    dXD4[parse4_idx] = _bytearrayToDoubleXAxis(LH_EXTN_STROKE_REALTIME)[0];

                    byte[] LH_EXTN_LOAD_REALTIME = _eeipObject.AssemblyObject.getInstance(0xBB);
                    dYD4[parse4_idx] = _bytearrayToDoubleYAxis(LH_EXTN_LOAD_REALTIME)[0];

                    _kvconnObject.writeDataCommand("W0FE3", "", "3");
                    _uiPlot4UpdateFlag = true;
                }
                */
            }
        }
    }

    public class DATAMODEL_COMMON
    {
        public string _activeModelName;
        public string _activeKayabaNumber;
        public string _activeDay;
        public string _activeMonth;
        public string _activeYear;
        public string _activeHour;
        public string _activeMinute;
        public string _activeSecond;

        public int _step1Enable;
        public float _step1Stroke;
        public float _step1CompresSpeed;
        public float _step1ExtendSpeed;
        public int _step1CycleCount;
        public float _step1MaxLoad;

        public int _step2Enable;
        public float _step2CompresSpeed;
        public float _step2CompressJudgeMin;
        public float _step2CompressJudgeMax;
        public float _step2CompressLoadRef;
        public float _step2ExtendSpeed;
        public float _step2ExtendJudgeMin;
        public float _step2ExtendJudgeMax;
        public float _step2ExtendLoadRef;
        public int _step2LoadRefTolerance;

        public int _step3Enable;
        public float _step3CompresSpeed;
        public float _step3CompressJudgeMin;
        public float _step3CompressJudgeMax;
        public float _step3CompressLoadRef;
        public float _step3ExtendSpeed;
        public float _step3ExtendJudgeMin;
        public float _step3ExtendJudgeMax;
        public float _step3ExtendLoadRef;
        public int _step3LoadRefTolerance;

        public List<string> DTM;
        public List<object> Step1Param;
        public List<object> Step2345Param;

        public DATAMODEL_COMMON()
        {
            DTM = new List<String>()
                {
                    _activeDay,
                    _activeMonth,
                    _activeYear,
                    _activeHour,
                    _activeMinute,
                    _activeSecond
                };

            Step1Param = new List<object>()
                {
                    _step1Enable,
                    _step1Stroke,
                    _step1CompresSpeed,
                    _step1ExtendSpeed,
                    _step1CycleCount,
                    _step1MaxLoad
                };

            Step2345Param = new List<object>()
                {
                    _step2Enable,
                    _step2CompresSpeed,
                    _step2CompressJudgeMin,
                    _step2CompressJudgeMax,
                    _step2CompressLoadRef,
                    _step2ExtendSpeed,
                    _step2ExtendJudgeMin,
                    _step2ExtendJudgeMax,
                    _step2ExtendLoadRef,
                    _step2LoadRefTolerance,
                    _step3Enable,
                    _step3CompresSpeed,
                    _step3CompressJudgeMin,
                    _step3CompressJudgeMax,
                    _step3CompressLoadRef,
                    _step3ExtendSpeed,
                    _step3ExtendJudgeMin,
                    _step3ExtendJudgeMax,
                    _step3ExtendLoadRef,
                    _step3LoadRefTolerance
                };

        }
    }

    public class DATAMODEL_R
    {
        public int _Step1MaxLoad_NG;
        public int _Step2CompRef_NG;
        public int _Step2ExtnRef_NG;
        public int _Step2CompGraph_NG;
        public int _Step2ExtnGraph_NG;
        public int _Step2DiffGraph_NG;

        public List<float> Judgement;
        public List<List<float>> RealtimeStep2;
        public List<List<float>> RealtimeStep3;
        public List<List<float>> MasteringStep2;
        public List<List<float>> MasteringStep3;

        public float _MaxLoad;
        public float _Step2CompLoadRef;
        public float _Step2ExtnLoadRef;
        public float _Step3CompLoadRef;
        public float _Step3ExtnLoadRef;

        List<float> _RealtimeStep2CompStroke;
        List<float> _RealtimeStep2CompLoad;
        List<float> _RealtimeStep2ExtnStroke;
        List<float> _RealtimeStep2ExtnLoad;
        List<float> _RealtimeStep2DiffStroke;
        List<float> _RealtimeStep2DiffLoad;

        List<float> _RealtimeStep3CompStroke;
        List<float> _RealtimeStep3CompLoad;
        List<float> _RealtimeStep3ExtnStroke;
        List<float> _RealtimeStep3ExtnLoad;
        List<float> _RealtimeStep3DiffStroke;
        List<float> _RealtimeStep3DiffLoad;

        public DATAMODEL_R()
        {
            Judgement = new List<float>()
            {
                _MaxLoad,
                _Step2CompLoadRef,
                _Step2ExtnLoadRef,
                _Step3CompLoadRef,
                _Step3ExtnLoadRef
            };

            RealtimeStep2 = new List<List<float>>()
            {
                _RealtimeStep2CompStroke,
                _RealtimeStep2CompLoad,
                _RealtimeStep2ExtnStroke,
                _RealtimeStep2ExtnLoad,
                _RealtimeStep2DiffStroke,
                _RealtimeStep2DiffLoad
            };

            RealtimeStep3 = new List<List<float>>()
            {
                _RealtimeStep3CompStroke,
                _RealtimeStep3CompLoad,
                _RealtimeStep3ExtnStroke,
                _RealtimeStep3ExtnLoad,
                _RealtimeStep3DiffStroke,
                _RealtimeStep3DiffLoad
            };
        }
    }

    public class DATAMODEL_L
    {
        public int _Step1MaxLoad_NG;
        public int _Step2CompRef_NG;
        public int _Step2ExtnRef_NG;
        public int _Step2CompGraph_NG;
        public int _Step2ExtnGraph_NG;
        public int _Step2DiffGraph_NG;

        public List<float> Judgement;
        public List<List<float>> RealtimeStep2;
        public List<List<float>> RealtimeStep3;
        public List<List<float>> MasteringStep2;
        public List<List<float>> MasteringStep3;

        public float _MaxLoad;
        public float _Step2CompLoadRef;
        public float _Step2ExtnLoadRef;
        public float _Step3CompLoadRef;
        public float _Step3ExtnLoadRef;

        List<float> _RealtimeStep2CompStroke;
        List<float> _RealtimeStep2CompLoad;
        List<float> _RealtimeStep2ExtnStroke;
        List<float> _RealtimeStep2ExtnLoad;
        List<float> _RealtimeStep2DiffStroke;
        List<float> _RealtimeStep2DiffLoad;

        List<float> _RealtimeStep3CompStroke;
        List<float> _RealtimeStep3CompLoad;
        List<float> _RealtimeStep3ExtnStroke;
        List<float> _RealtimeStep3ExtnLoad;
        List<float> _RealtimeStep3DiffStroke;
        List<float> _RealtimeStep3DiffLoad;

        public DATAMODEL_L()
        {
            Judgement = new List<float>()
            {
                _MaxLoad,
                _Step2CompLoadRef,
                _Step2ExtnLoadRef,
                _Step3CompLoadRef,
                _Step3ExtnLoadRef
            };

            RealtimeStep2 = new List<List<float>>()
            {
                _RealtimeStep2CompStroke,
                _RealtimeStep2CompLoad,
                _RealtimeStep2ExtnStroke,
                _RealtimeStep2ExtnLoad,
                _RealtimeStep2DiffStroke,
                _RealtimeStep2DiffLoad
            };

            RealtimeStep3 = new List<List<float>>()
            {
                _RealtimeStep3CompStroke,
                _RealtimeStep3CompLoad,
                _RealtimeStep3ExtnStroke,
                _RealtimeStep3ExtnLoad,
                _RealtimeStep3DiffStroke,
                _RealtimeStep3DiffLoad
            };
        }
    }

    public class DATAMODEL_MASTER
    {

        public string _activeModelName;
        public string _activeKayabaNumber;
        public string _activeDay;
        public string _activeMonth;
        public string _activeYear;
        public string _activeHour;
        public string _activeMinute;
        public string _activeSecond;

        public int _step1Enable;
        public float _step1Stroke;
        public float _step1CompresSpeed;
        public float _step1ExtendSpeed;
        public int _step1CycleCount;
        public float _step1MaxLoad;

        public int _step2Enable;
        public float _step2CompresSpeed;
        public float _step2CompressJudgeMin;
        public float _step2CompressJudgeMax;
        public float _step2CompressLoadRef;
        public float _step2ExtendSpeed;
        public float _step2ExtendJudgeMin;
        public float _step2ExtendJudgeMax;
        public float _step2ExtendLoadRef;
        public int _step2LoadRefTolerance;

        public int _step3Enable;
        public float _step3CompresSpeed;
        public float _step3CompressJudgeMin;
        public float _step3CompressJudgeMax;
        public float _step3CompressLoadRef;
        public float _step3ExtendSpeed;
        public float _step3ExtendJudgeMin;
        public float _step3ExtendJudgeMax;
        public float _step3ExtendLoadRef;
        public int _step3LoadRefTolerance;

        public List<string> DTM;
        public List<object> Step1Param;
        public List<object> Step2345Param;

        public List<List<float>> RMasteringStep2;
        public List<List<float>> RMasteringStep3;

        public List<List<float>> LMasteringStep2;
        public List<List<float>> LMasteringStep3;

        List<float> _RsideMasterStep2CompStroke;
        List<float> _RsideMasterStep2CompLoad;
        List<float> _RsideMasterStep2CompLoadLower;
        List<float> _RsideMasterStep2CompLoadUpper;
        List<float> _RsideMasterStep2ExtnStroke;
        List<float> _RsideMasterStep2ExtnLoad;
        List<float> _RsideMasterStep2ExtnLoadLower;
        List<float> _RsideMasterStep2ExtnLoadUpper;
        List<float> _RsideMasterStep2DiffStroke;
        List<float> _RsideMasterStep2DiffLoad;
        List<float> _RsideMasterStep2DiffLoadLower;
        List<float> _RsideMasterStep2DiffLoadUpper;

        List<float> _RsideMasterStep3CompStroke;
        List<float> _RsideMasterStep3CompLoad;
        List<float> _RsideMasterStep3CompLoadLower;
        List<float> _RsideMasterStep3CompLoadUpper;
        List<float> _RsideMasterStep3ExtnStroke;
        List<float> _RsideMasterStep3ExtnLoad;
        List<float> _RsideMasterStep3ExtnLoadLower;
        List<float> _RsideMasterStep3ExtnLoadUpper;
        List<float> _RsideMasterStep3DiffStroke;
        List<float> _RsideMasterStep3DiffLoad;
        List<float> _RsideMasterStep3DiffLoadLower;
        List<float> _RsideMasterStep3DiffLoadUpper;

        List<float> _LsideMasterStep2CompStroke;
        List<float> _LsideMasterStep2CompLoad;
        List<float> _LsideMasterStep2CompLoadLower;
        List<float> _LsideMasterStep2CompLoadUpper;
        List<float> _LsideMasterStep2ExtnStroke;
        List<float> _LsideMasterStep2ExtnLoad;
        List<float> _LsideMasterStep2ExtnLoadLower;
        List<float> _LsideMasterStep2ExtnLoadUpper;
        List<float> _LsideMasterStep2DiffStroke;
        List<float> _LsideMasterStep2DiffLoad;
        List<float> _LsideMasterStep2DiffLoadLower;
        List<float> _LsideMasterStep2DiffLoadUpper;

        List<float> _LsideMasterStep3CompStroke;
        List<float> _LsideMasterStep3CompLoad;
        List<float> _LsideMasterStep3CompLoadLower;
        List<float> _LsideMasterStep3CompLoadUpper;
        List<float> _LsideMasterStep3ExtnStroke;
        List<float> _LsideMasterStep3ExtnLoad;
        List<float> _LsideMasterStep3ExtnLoadLower;
        List<float> _LsideMasterStep3ExtnLoadUpper;
        List<float> _LsideMasterStep3DiffStroke;
        List<float> _LsideMasterStep3DiffLoad;
        List<float> _LsideMasterStep3DiffLoadLower;
        List<float> _LsideMasterStep3DiffLoadUpper;

        public DATAMODEL_MASTER()
        {

            DTM = new List<String>()
            {
                _activeDay,
                _activeMonth,
                _activeYear,
                _activeHour,
                _activeMinute,
                _activeSecond
            };

            Step1Param = new List<object>()
            {
                _step1Enable,
                _step1Stroke,
                _step1CompresSpeed,
                _step1ExtendSpeed,
                _step1CycleCount,
                _step1MaxLoad
            };

            Step2345Param = new List<object>()
            {
                _step2Enable,
                _step2CompresSpeed,
                _step2CompressJudgeMin,
                _step2CompressJudgeMax,
                _step2CompressLoadRef,
                _step2ExtendSpeed,
                _step2ExtendJudgeMin,
                _step2ExtendJudgeMax,
                _step2ExtendLoadRef,
                _step2LoadRefTolerance,
                _step3Enable,
                _step3CompresSpeed,
                _step3CompressJudgeMin,
                _step3CompressJudgeMax,
                _step3CompressLoadRef,
                _step3ExtendSpeed,
                _step3ExtendJudgeMin,
                _step3ExtendJudgeMax,
                _step3ExtendLoadRef,
                _step3LoadRefTolerance
            };

            RMasteringStep2 = new List<List<float>>()
            {
                _RsideMasterStep2CompStroke,
                _RsideMasterStep2CompLoad,
                _RsideMasterStep2CompLoadLower,
                _RsideMasterStep2CompLoadUpper,
                _RsideMasterStep2ExtnStroke,
                _RsideMasterStep2ExtnLoad,
                _RsideMasterStep2ExtnLoadLower,
                _RsideMasterStep2ExtnLoadUpper,
                _RsideMasterStep2DiffStroke,
                _RsideMasterStep2DiffLoad,
                _RsideMasterStep2DiffLoadLower,
                _RsideMasterStep2DiffLoadUpper
            };

            RMasteringStep3 = new List<List<float>>()
            {
                _RsideMasterStep3CompStroke,
                _RsideMasterStep3CompLoad,
                _RsideMasterStep3CompLoadLower,
                _RsideMasterStep3CompLoadUpper,
                _RsideMasterStep3ExtnStroke,
                _RsideMasterStep3ExtnLoad,
                _RsideMasterStep3ExtnLoadLower,
                _RsideMasterStep3ExtnLoadUpper,
                _RsideMasterStep3DiffStroke,
                _RsideMasterStep3DiffLoad,
                _RsideMasterStep3DiffLoadLower,
                _RsideMasterStep3DiffLoadUpper
            };

            LMasteringStep2 = new List<List<float>>()
            {
                _LsideMasterStep2CompStroke,
                _LsideMasterStep2CompLoad,
                _LsideMasterStep2CompLoadLower,
                _LsideMasterStep2CompLoadUpper,
                _LsideMasterStep2ExtnStroke,
                _LsideMasterStep2ExtnLoad,
                _LsideMasterStep2ExtnLoadLower,
                _LsideMasterStep2ExtnLoadUpper,
                _LsideMasterStep2DiffStroke,
                _LsideMasterStep2DiffLoad,
                _LsideMasterStep2DiffLoadLower,
                _LsideMasterStep2DiffLoadUpper
            };

            LMasteringStep3 = new List<List<float>>()
            {
                _LsideMasterStep3CompStroke,
                _LsideMasterStep3CompLoad,
                _LsideMasterStep3CompLoadLower,
                _LsideMasterStep3CompLoadUpper,
                _LsideMasterStep3ExtnStroke,
                _LsideMasterStep3ExtnLoad,
                _LsideMasterStep3ExtnLoadLower,
                _LsideMasterStep3ExtnLoadUpper,
                _LsideMasterStep3DiffStroke,
                _LsideMasterStep3DiffLoad,
                _LsideMasterStep3DiffLoadLower,
                _LsideMasterStep3DiffLoadUpper
            };
        }
    }


}
