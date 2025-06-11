//#pragma warning disable CA1416 // Validate platform compatibility


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
using DocumentFormat.OpenXml.Vml;
using System.Data;
using KVCOMSERVER;
using ScottPlot;
using System.Linq.Expressions;

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
        private STimer _clock1h;
        private STimer _clock1m;
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
        DATAMODEL_RL _Ldata;
        DATAMODEL_RL _Rdata;

        DATAMODEL_RESULT _Ljudge;
        DATAMODEL_RESULT _Rjudge;

        EXCELSTREAM _editmaster;
        DATAMODEL_MASTER _editdatamaster;

        EXCELSTREAM _copymaster;
        DATAMODEL_MASTER _copydatamaster;
        DATAMODEL_TEACHING_MASTER _TMaster;

        public string HeadDir;
        public string RealLogDir;
        public string MasterDir;

        bool _parameterRead;
        bool _parameterReadFlag;
        bool _parameterReadFlagComplete;
        bool _realtimeRead;
        bool _realtimeReadFlag;
        bool _realtimeReadFlagComplete;

        bool _masterIsUpdatingDatabase;
        public bool MasterIsUpdatingDatabase() { return _masterIsUpdatingDatabase; }
        public void MasterUpdatingDatabaseSet() { _masterIsUpdatingDatabase = true; }
        public void MasterUpdatingDatabaseReset() { _masterIsUpdatingDatabase = false; }

        bool _realPresentConfirm;
        public bool RealPresentConfirm() { return _realPresentConfirm; }
        public void RealPresentConfirmSet() { _realPresentConfirm = true; }
        public void RealPresentConfirmReset() { _realPresentConfirm = false; }

        bool _masterSetupConfirm;
        public bool MasterSetupConfirm() { return _masterSetupConfirm; }
        public void MasterSetupConfirmSet() { _masterSetupConfirm = true; }
        public void MasterSetupConfirmReset() { _masterSetupConfirm = false; }

        bool _masterDataValidation;
        public bool MasterDataValidation() { return _masterDataValidation; }
        public void MasterDataValidationSet() { _masterDataValidation = true; }
        public void MasterDataValidationReset() { _masterDataValidation = false; }

        bool _dataRMasterTeachIsExist;
        bool DataRMasterTeachIsExist() { return _dataRMasterTeachIsExist; }
        void DataRMasterTeachIsExistSet() { _dataRMasterTeachIsExist = true; }
        void DataRMasterTeachIsExistReset() { _dataRMasterTeachIsExist = false; }

        bool _dataLMasterTeachIsExist;
        bool DataLMasterTeachIsExist() { return _dataLMasterTeachIsExist; }
        void DataLMasterTeachIsExistSet() { _dataLMasterTeachIsExist = true; }
        void DataLMasterTeachIsExistReset() { _dataLMasterTeachIsExist = false; }

        bool _backgroundProcessOngoing { get; set; }

        string _kvMsgRecv;

        string[] dataRsideStep2addrs = new string[]
                    {
                        "ZF110000",
                        "ZF110400",
                        "ZF110800",
                        "ZF111200",
                        "ZF110000",
                        "ZF510000"
                    };
        string[] dataLsideStep2addrs = new string[]
                    {
                        "ZF210000",
                        "ZF210400",
                        "ZF210800",
                        "ZF211200",
                        "ZF210000",
                        "ZF510500"
                    };
        string[] dataMasterRsideStep2addrs = new string[]
                    {
                        "ZF117200",
                        "ZF117600",
                        "ZF118000",
                        "ZF118400",
                        "ZF118800",
                        "ZF119200",
                        "ZF119600",
                        "ZF120000",
                        "ZF511000",
                        "ZF511400",
                        "ZF512200",
                        "ZF511800"
                    };
        string[] dataMasterLsideStep2addrs = new string[]
                    {
                        "ZF217200",
                        "ZF217600",
                        "ZF218000",
                        "ZF218400",
                        "ZF218800",
                        "ZF219200",
                        "ZF219600",
                        "ZF220000",
                        "ZF515000",
                        "ZF515400",
                        "ZF516200",
                        "ZF515800"
                    };
        string[] dataMasterRsideStep3addrs = new string[]
                    {
                        "ZF120400",
                        "ZF120800",
                        "ZF121200",
                        "ZF121600",
                        "ZF122000",
                        "ZF122400",
                        "ZF122800",
                        "ZF123200",
                        "ZF513000",
                        "ZF513400",
                        "ZF514200",
                        "ZF513800"
                    };
        string[] dataMasterLsideStep3addrs = new string[]
                    {
                        "ZF220400",
                        "ZF220800",
                        "ZF221200",
                        "ZF221600",
                        "ZF222000",
                        "ZF222400",
                        "ZF222800",
                        "ZF223200",
                        "ZF517000",
                        "ZF517400",
                        "ZF518200",
                        "ZF517800"
                    };

        string[] RMasteringTeachStep2addrs = new string[]
        {
            "ZF131700",//RCOMPSTROKETEACH
            "ZF132500",//RCOMPLOADTEACH
            "ZF138100",//REXTNSTROKETEACH
            "ZF138900",//REXTNLOADTEACH
            "ZF511000",//RDIFFSTROKETEACH
            "ZF510000"//RDIFFLOADTEACH
        };
        string[] RMasteringTeachStep3addrs = new string[]
        {

        };
        string[] LMasteringTeachStep2addrs = new string[]
        {
            "ZF231700",//LCOMPSTROKETEACH
            "ZF232500",//LCOMPLOADTEACH
            "ZF238100",//LEXTNSTROKETEACH
            "ZF238900",//LEXTNLOADTEACH
            "ZF515000",//LDIFFSTROKETEACH
            "ZF510500"//LDIFFLOADTEACH
        };
        string[] LMasteringTeachStep3addrs = new string[]
        {

        };


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
            _Ldata = new DATAMODEL_RL();
            _Rdata = new DATAMODEL_RL();
            _masterData = new DATAMODEL_MASTER();
            _TMaster = new DATAMODEL_TEACHING_MASTER();

            _Ljudge = new DATAMODEL_RESULT();
            _Rjudge = new DATAMODEL_RESULT();

            _ctsClock = new CancellationTokenSource();
            _clock1h = new STimer(async _ => await Clock1h(_ctsClock.Token), null, TimeSpan.Zero, TimeSpan.FromMinutes(60));
            _clock1m = new STimer(async _ => await Clock1m(_ctsClock.Token), null, TimeSpan.Zero, TimeSpan.FromSeconds(60));
            _clock1s = new STimer(async _ => await Clock1s(_ctsClock.Token), null, TimeSpan.Zero, TimeSpan.FromMilliseconds(1000));
            _clock1ms = new STimer(async _ => await Clock1ms(_ctsClock.Token), null, TimeSpan.Zero, TimeSpan.FromMilliseconds(1));
            _clock10ms = new STimer(async _ => await Clock10ms(_ctsClock.Token), null, TimeSpan.Zero, TimeSpan.FromMilliseconds(10));
            _clock100ms = new STimer(async _ => await Clock100ms(_ctsClock.Token), null, TimeSpan.Zero, TimeSpan.FromMilliseconds(100));

            HeadDir = _settingObject.FILEDIR_SETTEI_GET();
            RealLogDir = HeadDir + $"LOG_REALTIME\\";
            MasterDir = HeadDir + $"MASTER_MODEL_DATA\\";
            CheckFolderPath(RealLogDir);
            CheckFolderPath(MasterDir);

            backgroundThread = new Thread(BackgroundWork);
            backgroundThread.Start();
            SetConnInternal_AtInit();

        }

        private async Task Clock1h(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            //await Task.Run(() => UpdateUIRealtimeList(), cancellationToken);
        }
        private async Task Clock1m(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            //await Task.Run(() => UpdateUIRealtimeList(), cancellationToken);
        }
        private async Task Clock1s(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;
            if (_uiObject.InvokeRequired)
            {
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.UpdateTime()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.UpdateDate()));
            }
            else
            {
                //_uiObject.UpdateTime();
            }

            //await Task.Run(() => UpdateUIRealtimeList(), cancellationToken);
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

        void SetConnInternal_AtInit()
        {
            SetConnection();
            _uiObject._connStat = 1;
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
                    //_uiPlotClear();
                    _kvconnObject.writeDataCommand("W0FE0", "", "0");
                }
                //Thread.Sleep(100);

                if ((byte)(STAT_INPUT[30] & 0x01) == 0x01)
                {

                    if (_uiObject.InvokeRequired)
                    {
                        _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.TabPageSelect(3)));
                    }
                    _kvconnObject.writeDataCommand("W0CF", "", "0");
                }
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
        void _eeipEventHandler_4() //Master Model
        {
            if (this.GetConnState() == 1)
            {
                byte[] TRIG = _eeipObject.AssemblyObject.getInstance(0x8F);

                if ((byte)(TRIG[0] & 0x01) == 0x01)
                {
                    MasterSetupConfirmReset();
                    byte[] MODEL_NAME_INPUT = _eeipObject.AssemblyObject.getInstance(0xAA);
                    string MODNAME = ParseByteString(MODEL_NAME_INPUT);
                    Debug.WriteLine(MODNAME);
                    _eeipTrigMasterFetch(MODNAME, ref MasterFileActive, ref _masterData);
                    if (MasterValidation(ref _masterData))
                    {
                        _eeipTrigMasterFetchModel(ref _masterData);
                        _eeipTrigMasterFetchGraph(ref _masterData);

                        MasterDataAssignRealPlot();
                        MasterDataAssignLMasterPlot();
                        MasterDataAssignRMasterPlot();
                        if (MasterValidation(ref _masterData))
                        {
                            uiPlotRealMasterUpdate();
                            uiPlotLTeachMasterUpdate();
                            uiPlotRTeachMasterUpdate();
                        }
                        uiUpdateMasterFetchTeachTable();
                        uiUPdateRealMasterActiveTable(_masterData);
                        if (MasterValidation(ref _masterData))
                        {
                            MasterDataValidationSet();
                        }
                    }
                    else
                    {
                        _kvconnObject.writeDataCommand("W0FF", "", "1");
                    }
                    uiSetModelName(_masterData._activeModelName);
                    MasterSetupConfirmSet();
                    _kvconnObject.writeDataCommand("W0F0", "", "1"); //>confirm if read file complete
                    _kvconnObject.writeDataCommand("W0D0", "", "0");
                    //Thread.Sleep(10);
                }

                if ((byte)(TRIG[2] & 0x01) == 0x01)
                {
                    MasterSetupConfirmReset();
                    //byte[] MODEL_NAME_INPUT = _eeipObject.AssemblyObject.getInstance(0xAB);
                    _eeipTrigMasterNewModelParam();
                    _kvconnObject.writeDataCommand("W0D1", "", "0");
                    //Thread.Sleep(10);
                }

                if ((byte)(TRIG[4] & 0x01) == 0x01)
                {
                    MasterSetupConfirmReset();
                    byte[] MODEL_NAME_INPUT = _eeipObject.AssemblyObject.getInstance(0xAA);
                    _editmaster = new EXCELSTREAM("MASTER");
                    _editdatamaster = new DATAMODEL_MASTER();
                    string MODNAME = ParseByteString(MODEL_NAME_INPUT);
                    Debug.WriteLine(MODNAME);
                    _eeipTrigMasterFetch(MODNAME, ref _editmaster, ref _editdatamaster);
                    MasterValidation(ref _masterData);
                    _eeipTrigMasterFetchModel(ref _editdatamaster);
                    //_eeipTrigMasterFetchGraph(ref _masterData);
                    _kvconnObject.writeDataCommand("W0F0", "", "1"); //>confirm if read file complete
                    _kvconnObject.writeDataCommand("W0D2", "", "0");
                }
                if ((byte)(TRIG[6] & 0x01) == 0x01)
                {
                    byte[] MODEL_NAME_INPUT = _eeipObject.AssemblyObject.getInstance(0xAA);
                    string MODNAME = ParseByteString(MODEL_NAME_INPUT);
                    Debug.WriteLine(MODNAME);
                    _eeipTrigMasterEditModelParam(ref _editmaster, ref _editdatamaster);
                    _kvconnObject.writeDataCommand("W0D3", "", "0");
                    //Thread.Sleep(10);
                }
                else if ((byte)(TRIG[6] & 0x02) == 0x02)
                {
                    _editmaster = null;
                    _editdatamaster = null;
                    _kvconnObject.writeDataCommand("W0D3", "", "0");
                }

                if ((byte)(TRIG[8] & 0x01) == 0x01)
                {
                    MasterSetupConfirmReset();
                    byte[] MODEL_NAME_INPUT = _eeipObject.AssemblyObject.getInstance(0xAA);
                    _copymaster = new EXCELSTREAM("MASTER");
                    _copydatamaster = new DATAMODEL_MASTER();
                    string MODNAME = ParseByteString(MODEL_NAME_INPUT);
                    Debug.WriteLine(MODNAME);
                    _eeipTrigMasterFetch(MODNAME, ref _copymaster, ref _copydatamaster);
                    _eeipTrigMasterFetchModel(ref _copydatamaster);
                    //_eeipTrigMasterFetchGraph(ref _masterData);
                    _kvconnObject.writeDataCommand("W0F0", "", "1"); //>confirm if read file complete
                    _kvconnObject.writeDataCommand("W0D4", "", "0");
                }
                if ((byte)(TRIG[10] & 0x01) == 0x01)
                {
                    byte[] MODEL_NAME_INPUT = _eeipObject.AssemblyObject.getInstance(0xAB);
                    string MODNAME = ParseByteString(MODEL_NAME_INPUT);
                    Debug.WriteLine(MODNAME);
                    _eeipTrigMasterCopyModel(MODNAME, ref _copymaster, ref _copydatamaster);
                    _kvconnObject.writeDataCommand("W0D5", "", "0");
                    //Thread.Sleep(10);
                }
                else if ((byte)(TRIG[10] & 0x02) == 0x02)
                {
                    _copymaster = null;
                    _copydatamaster = null;
                    _kvconnObject.writeDataCommand("W0D5", "", "0");
                }

                if ((byte)(TRIG[12] & 0x01) == 0x01)
                {
                    byte[] MODEL_NAME_INPUT = _eeipObject.AssemblyObject.getInstance(0xAA);
                    string MODNAME = ParseByteString(MODEL_NAME_INPUT);
                    Debug.WriteLine(MODNAME);
                    _eeipTrigMasterDeleteModel(MODNAME);
                    _kvconnObject.writeDataCommand("W0D6", "", "0");
                }

                if ((byte)(TRIG[18] & 0x01) == 0x01)
                {
                    byte[] MODEL_NAME_INPUT = _eeipObject.AssemblyObject.getInstance(0xAA);
                    string __MODNAME = ParseByteString(MODEL_NAME_INPUT);
                    string[] __files = Directory.GetFiles(MasterDir);
                    if (__files.Length > 0)
                    {
                        foreach (string __file in __files)
                        {
                            if (__file.Contains(__MODNAME))
                            {
                                _excelReadMasterData(__file, ref MasterFileActive, ref _masterData);
                            }
                        }
                    }
                    _kvconnObject.writeDataCommand("W0D9", "", "0");
                }
            }
        }

        void _eeipEventHandler_5() //Master Teaching
        {
            if (this.GetConnState() == 1)
            {
                byte[] TRIG = _eeipObject.AssemblyObject.getInstance(0x8F);

                if ((byte)(TRIG[20] & 0x01) == 0x01)
                {
                    //_updateMasterDatabase();
                    MasterDataValidationReset();
                    DataRMasterTeachIsExistReset();
                    DataLMasterTeachIsExistReset();
                    if (_uiObject.InvokeRequired)
                    {
                        _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.TabPageSelect(6)));
                    }

                    _kvconnObject.writeDataCommand("W0DA", "", "0");
                    //Thread.Sleep(10);
                }

                if ((byte)(TRIG[22] & 0x01) == 0x01)
                {
                    _kvMasterTeachDataUpload(ref _TMaster.RMasteringTeachStep2, RMasteringTeachStep2addrs, 404);
                    DataRMasterTeachIsExistSet();

                    MasterDataAssignRMasterPlot();
                    DataPlotRTeachRead();
                    uiPlotRTeachMasterUpdate();
                    uiPlotRTeachUpdate();
                    uiUpdateMasterRTeachTable();

                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.updateMasterReset()));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.updateTeachingReset()));
                    _kvconnObject.writeDataCommand("W0DB", "", "0");

                }
                if ((byte)(TRIG[24] & 0x01) == 0x01)
                {
                    _kvMasterTeachDataUpload(ref _TMaster.LMasteringTeachStep2, LMasteringTeachStep2addrs, 404);
                    DataLMasterTeachIsExistSet();

                    MasterDataAssignLMasterPlot();
                    DataPlotLTeachRead();
                    uiPlotLTeachMasterUpdate();
                    uiPlotLTeachUpdate();
                    uiUpdateMasterLTeachTable();

                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.updateMasterReset()));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.updateTeachingReset()));
                    _kvconnObject.writeDataCommand("W0DC", "", "0");

                }
                if ((byte)(TRIG[26] & 0x01) == 0x01)
                {
                    DataRMasterTeachIsExistReset();
                    DataLMasterTeachIsExistReset();

                    _kvconnObject.writeDataCommand("W0DD", "", "0");
                }


                if ((byte)(TRIG[28] & 0x01) == 0x01)
                {
                    MasterDataValidationSet();
                    _kvconnObject.writeDataCommand("W0DE", "", "0");
                }

                if ((byte)(TRIG[30] & 0x01) == 0x01)
                {
                    MasterDataValidationReset();
                    _kvconnObject.writeDataCommand("W0DF", "", "0");
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
        public void _kvMasterConfirm()
        {
            if (_masterSetupConfirm)
            {
                if (_uiObject != null && _uiObject.IsHandleCreated)
                {
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.masterSetupSet()));

                }
                _kvconnObject.writeDataCommand("W01A", "", "1");
            }
            else
            {
                if (_uiObject != null && _uiObject.IsHandleCreated)
                {
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.masterSetupReset()));
                }
                _kvconnObject.writeDataCommand("W01A", "", "0");
            }

            if (_masterDataValidation)
            {
                if (_uiObject != null && _uiObject.IsHandleCreated)
                {
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.masterValidSet()));
                }
                _kvconnObject.writeDataCommand("W01B", "", "1");
            }
            else
            {
                if (_uiObject != null && _uiObject.IsHandleCreated)
                {
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.masterValidReset()));
                }
                _kvconnObject.writeDataCommand("W01B", "", "0");
            }

        }


        void _eeipTrigMasterFetch(string MODNAME, ref EXCELSTREAM filemaster, ref DATAMODEL_MASTER datamaster)
        {
            string[] files = Directory.GetFiles(MasterDir);
            string filename;
            bool notfound = false;
            if (files.Length > 0)
            {
                foreach (string file in files)
                {
                    if (file == $"{MasterDir}{MODNAME}.XLSX" || file == $"{MasterDir}{MODNAME}.XLS" || file == $"{MasterDir}{MODNAME}.xlsx" || file == $"{MasterDir}{MODNAME}.xls")
                    {
                        notfound = false;
                        _excelReadMasterData(file, ref filemaster, ref datamaster);
                        break;
                    }
                    else
                    {
                        notfound = true;
                    }
                }

                if (notfound)
                {
                    DATAMODEL_MASTER _INITMODEL = new DATAMODEL_MASTER();
                    _INITMODEL._activeModelName = MODNAME;
                    _INITMODEL._activeKayabaNumber = "000000";
                    _excelInitMasterData(ref _INITMODEL);
                    foreach (string file in files)
                    {
                        if (file == $"{MasterDir}{MODNAME}.XLSX" || file == $"{MasterDir}{MODNAME}.XLS" || file == $"{MasterDir}{MODNAME}.xlsx" || file == $"{MasterDir}{MODNAME}.xls")
                        {
                            _excelReadMasterData(file, ref filemaster, ref datamaster);
                            break;
                        }
                    }

                    //MessageBox.Show("Master File for this model is not found. Please initiate setting.", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                DATAMODEL_MASTER _INITMODEL = new DATAMODEL_MASTER();
                _INITMODEL._activeModelName = MODNAME;
                _INITMODEL._activeKayabaNumber = "000000";
                _excelInitMasterData(ref _INITMODEL);
                foreach (string file in files)
                {
                    if (file == $"{MasterDir}{MODNAME}.XLSX" || file == $"{MasterDir}{MODNAME}.XLS" || file == $"{MasterDir}{MODNAME}.xlsx" || file == $"{MasterDir}{MODNAME}.xls")
                    {
                        _excelReadMasterData(file, ref filemaster, ref datamaster);
                        break;
                    }
                }

                //MessageBox.Show("Master File for this model is not found. Please initiate setting.", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        void _eeipTrigMasterFetchModel(ref DATAMODEL_MASTER datamaster)
        {
            _kvMasterModelDownload(ref datamaster);
            _kvMasterParamSizeDownload(ref datamaster);
            _kvMasterParam1Download(ref datamaster);
            _kvMasterParam2345Download(ref datamaster);
            _kvMasterParamDiffDownload(ref datamaster);
        }
        void _eeipTrigMasterFetchGraph(ref DATAMODEL_MASTER datamaster)
        {
            _kvMasterGraphDownload(ref datamaster.RMasteringStep2, dataMasterRsideStep2addrs, 400);
            _kvMasterGraphDownload(ref datamaster.LMasteringStep2, dataMasterLsideStep2addrs, 400);
            //_kvMasterGraphDownload(ref datamaster.RMasteringStep3, dataMasterRsideStep3addrs, 400);
            //_kvMasterGraphDownload(ref datamaster.LMasteringStep3, dataMasterLsideStep3addrs, 400);

            //_kvconnObject.writeDataCommand("W0F0", "", "1"); //>confirm if read file complete
        }
        void _eeipTrigMasterNewModelParam(/*byte[] MODNAME_VAR*/)
        {
            DATAMODEL_MASTER _NEWMODEL = new DATAMODEL_MASTER();
            _eeipMasterModelUpload(ref _NEWMODEL);
            _eeipMasterParamSizeUpload(ref _NEWMODEL);
            _eeipMasterParam1Upload(ref _NEWMODEL);
            _eeipMasterParam2345Upload(ref _NEWMODEL);
            _eeipMasterDiffParamUpload(ref _NEWMODEL);
            _excelInitMasterData(ref _NEWMODEL);

            _kvconnObject.writeDataCommand("W0F1", "", "1");
            //>confirm if complete add new model (only initiate master file and parameter, give warning to continue teaching)

            /*
            //string MODNAME = Encoding.Default.GetString(MODNAME_VAR);
            //foreach (string files in Directory.GetFiles(MasterDir))
            {
                //if (files.Contains(MODNAME))
                {
                    //_kvconnObject.writeDataCommand("W0FE", "", "1");
                    //>confirm if the file master already exist and warning to choose another name, or create another model
                }
                //else
                {
                    
                    _eeipMasterModelUpload(ref _masterData);
                    _eeipMasterParam1Upload(ref _masterData);
                    _eeipMasterParam2345Upload(ref _masterData);
                    //_excelInitMasterParamData(ref _masterData);

                    _kvconnObject.writeDataCommand("W0F1", "", "1");
                    //>confirm if complete add new model (only initiate master file and parameter, give warning to continue teaching)
                    
                }
            }
            */
        }
        void _eeipTrigMasterEditModelParam(ref EXCELSTREAM filemaster, ref DATAMODEL_MASTER datamaster)
        {
            _eeipMasterModelUpload(ref datamaster);
            _eeipMasterParamSizeUpload(ref datamaster);
            _eeipMasterParam1Upload(ref datamaster);
            _eeipMasterParam2345Upload(ref datamaster);
            _eeipMasterDiffParamUpload(ref datamaster);
            //test if need to reassign/reupload graph data or not, if need from which entity

            _excelStoreMasterParamData(ref datamaster, ref filemaster);
            //test also if excelstream need to reassign graph data or not
            _excelPrintMasterData(ref datamaster, ref filemaster);

            filemaster = null;
            datamaster = null;
            _kvconnObject.writeDataCommand("W0F2", "", "1");
            //>confirm if complete edit new model (only initiate master file and parameter, give warning to continue teaching)
        }
        void _eeipTrigMasterCopyModel(string MODNAME, ref EXCELSTREAM filemaster, ref DATAMODEL_MASTER datamaster)
        {
            //_eeipMasterModelUpload(ref datamaster);
            datamaster._activeModelName = MODNAME;
            filemaster.setModelName(datamaster._activeModelName);
            //test if need to reassign/reupload graph data or not, if need from which entity
            //test also if excelstream need to reassign graph data or not
            //use this method instead if need all data to be reassigned to master//_excelStoreMasterParamData(ref datamaster, ref filemaster);

            _excelPrintMasterData(ref datamaster, ref filemaster);

            filemaster = null;
            datamaster = null;
            _kvconnObject.writeDataCommand("W0F3", "", "1");
            //>confirm if complete edit new model (only initiate master file and parameter, give warning to continue teaching)
        }
        void _eeipTrigMasterDeleteModel(string MODNAME)
        {
            foreach (string files in Directory.GetFiles(MasterDir))
            {
                if (files.Contains(MODNAME))
                {
                    File.Delete(files);
                    _kvconnObject.writeDataCommand("W0F4", "", "1"); //>confirm if read file complete
                }
                else
                {
                    _kvconnObject.writeDataCommand("W0FE", "", "1"); //>confirm if not found
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

                    _eeipreadActiveModelData(ref _data);
                    _eeipreadStep1Param(ref _data);
                    _eeipreadParamSizeUpload(ref _data);
                    _eeipreadStep2345Param(ref _data);
                    _eeipreadDiffParam(ref _data);

                    _excelStoreParameterData(ref _data, ref RealtimeFileR1);
                    _excelStoreParameterData(ref _data, ref RealtimeFileL1);
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

                    _eeipreadDateTime(ref _data);
                    _eeipreadJudgement(ref _Rdata.Judgement, 0xA5);
                    _eeipreadJudgement(ref _Ldata.Judgement, 0xA6);
                    _eeipreadResultan(ref _Rjudge.Values, 0xB2);
                    _eeipreadResultan(ref _Ljudge.Values, 0xB3);
                    _kvreadRealtime(ref _Rdata.RealtimeStep2, dataRsideStep2addrs, 400);
                    //_kvreadRealtime(ref _Rdata.RealtimeStep3, "ZF111604", "ZF112004", "ZF112404", "ZF113208", "ZF111604", "ZF510000", 400);
                    _kvreadRealtime(ref _Ldata.RealtimeStep2, dataLsideStep2addrs, 400);
                    //_kvreadRealtime(ref _Ldata.RealtimeStep3, "ZF211604", "ZF212004", "ZF212404", "ZF213208", "ZF211604", "ZF510500", 400);

                    RealPresentConfirmSet();

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

                    if (_Ldata._Step1MaxLoad_NG != 1)
                    {
                        _Ldata._Step1MaxLoad_NG = 0;
                    }

                    if (_Ldata._Step2CompRef_NG != 1)
                    {
                        _Ldata._Step2CompRef_NG = 0;
                    }

                    if (_Ldata._Step2ExtnRef_NG != 1)
                    {
                        _Ldata._Step2ExtnRef_NG = 0;
                    }

                    if (_Ldata._Step2CompGraph_NG == 1)
                    {
                        D1Col = System.Drawing.Color.Red;
                    }
                    else
                    {
                        _Ldata._Step2CompGraph_NG = 0;
                        D1Col = System.Drawing.Color.LimeGreen;
                    }

                    if (_Ldata._Step2ExtnGraph_NG == 1)
                    {
                        D2Col = System.Drawing.Color.Red;
                    }
                    else
                    {
                        _Ldata._Step2ExtnGraph_NG = 0;
                        D2Col = System.Drawing.Color.LimeGreen;
                    }

                    //----------------------------------------------

                    if (_Rdata._Step1MaxLoad_NG != 1)
                    {
                        _Rdata._Step1MaxLoad_NG = 0;
                    }

                    if (_Rdata._Step2CompRef_NG != 1)
                    {
                        _Rdata._Step2CompRef_NG = 0;
                    }

                    if (_Rdata._Step2ExtnRef_NG != 1)
                    {
                        _Rdata._Step2ExtnRef_NG = 0;
                    }

                    if (_Rdata._Step2CompGraph_NG == 1)
                    {
                        D3Col = System.Drawing.Color.Red;
                    }
                    else
                    {
                        _Rdata._Step2CompGraph_NG = 0;
                        D3Col = System.Drawing.Color.LimeGreen;
                    }

                    if (_Rdata._Step2ExtnGraph_NG == 1)
                    {
                        D4Col = System.Drawing.Color.Red;
                    }
                    else
                    {
                        _Rdata._Step2ExtnGraph_NG = 0;
                        D4Col = System.Drawing.Color.LimeGreen;
                    }

                    if (_Ldata._Step2DiffGraph_NG == 1)
                    {
                        D5Col = System.Drawing.Color.Red;
                    }
                    else
                    {
                        _Ldata._Step2DiffGraph_NG = 0;
                        D5Col = System.Drawing.Color.LimeGreen;
                    }

                    if (_Rdata._Step2DiffGraph_NG == 1)
                    {
                        D6Col = System.Drawing.Color.Red;
                    }
                    else
                    {
                        _Rdata._Step2DiffGraph_NG = 0;
                        D6Col = System.Drawing.Color.LimeGreen;
                    }

                    _excelStoreRealtimeData(ref _data, ref _Rdata, ref RealtimeFileR1, "R");
                    _excelStoreRealtimeData(ref _data, ref _Ldata, ref RealtimeFileL1, "L");

                    _backgroundDataPlot1Read();
                    _uiPlot1Update();
                    _backgroundDataPlot2Read();
                    _uiPlot2Update();
                    _backgroundDataPlot3Read();
                    _uiPlot3Update();
                    _backgroundDataPlot4Read();
                    _uiPlot4Update();
                    _backgroundDataPlot9Read();
                    _uiPlot9Update();
                    _backgroundDataPlot10Read();
                    _uiPlot10Update();
                    uiUPdateRealDataTable(_Rdata, _Ldata);

                    uiPlotSignalLineHide(ref _uiObject.Plot1_MASTER);
                    uiPlotSignalLineHide(ref _uiObject.Plot2_MASTER);
                    uiPlotSignalLineHide(ref _uiObject.Plot3_MASTER);
                    uiPlotSignalLineHide(ref _uiObject.Plot4_MASTER);
                    uiPlotSignalLineHide(ref _uiObject.Plot9_MASTER);
                    uiPlotSignalLineHide(ref _uiObject.Plot10_MASTER);


                    if (_uiObject.InvokeRequired)
                    {
                        _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.Judge_Left_Lamp(_Ldata._Step1MaxLoad_NG + _Ldata._Step2CompRef_NG + _Ldata._Step2CompGraph_NG + _Ldata._Step2ExtnRef_NG + _Ldata._Step2ExtnGraph_NG + _Ldata._Step2DiffGraph_NG)));
                        _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PreStroke_Left_Value(_Ldata.Judgement[0], _Ldata._Step1MaxLoad_NG)));
                        //_uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.Step2_Left_Lamp(_Ldata._Step1MaxLoad_NG + _Ldata._Step2CompRef_NG + _Ldata._Step2CompGraph_NG + _Ldata._Step2ExtnRef_NG + _Ldata._Step2ExtnGraph_NG + _Ldata._Step2DiffGraph_NG)));
                        _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.Step2_Left_CompressValue(_Ldata.Judgement[1], _Ldata._Step2CompRef_NG)));
                        _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.Step2_Left_CompressLowLimit(_Ljudge.Values[2])));
                        _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.Step2_Left_CompressHiLimit(_Ljudge.Values[3])));
                        _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.Step2_Left_ExtensValue(_Ldata.Judgement[2], _Ldata._Step2ExtnRef_NG)));
                        _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.Step2_Left_ExtensLowLimit(_Ljudge.Values[5])));
                        _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.Step2_Left_ExtensHiLimit(_Ljudge.Values[6])));
                        _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.Plot_LeftComp_GraphJudge(_Ldata._Step2CompGraph_NG, Convert.ToSingle(_data.Step1Param[1]))));
                        _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.Plot_LeftExt_GraphJudge(_Ldata._Step2ExtnGraph_NG, Convert.ToSingle(_data.Step1Param[1]))));
                        _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.Plot_LeftDiff_GraphJudge(_Ldata._Step2DiffGraph_NG, Convert.ToSingle(_data.Step1Param[1]))));

                        _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.Judge_Right_Lamp(_Rdata._Step1MaxLoad_NG + _Rdata._Step2CompRef_NG + _Rdata._Step2CompGraph_NG + _Rdata._Step2ExtnRef_NG + _Rdata._Step2ExtnGraph_NG + _Rdata._Step2DiffGraph_NG)));
                        _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PreStroke_Right_Value(_Rdata.Judgement[0], _Rdata._Step1MaxLoad_NG)));
                        //_uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.Step2_Right_Lamp(_Rdata._Step1MaxLoad_NG + _Rdata._Step2CompRef_NG + _Rdata._Step2CompGraph_NG + _Rdata._Step2ExtnRef_NG + _Rdata._Step2ExtnGraph_NG + _Rdata._Step2DiffGraph_NG)));
                        _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.Step2_Right_CompressValue(_Rdata.Judgement[1], _Rdata._Step2CompRef_NG)));
                        _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.Step2_Right_CompressLowLimit(_Rjudge.Values[2])));
                        _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.Step2_Right_CompressHiLimit(_Rjudge.Values[3])));
                        _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.Step2_Right_ExtensValue(_Rdata.Judgement[2], _Rdata._Step2ExtnRef_NG)));
                        _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.Step2_Right_ExtensLowLimit(_Rjudge.Values[5])));
                        _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.Step2_Right_ExtensHiLimit(_Rjudge.Values[6])));
                        _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.Plot_RightComp_GraphJudge(_Rdata._Step2CompGraph_NG, Convert.ToSingle(_data.Step1Param[1]))));
                        _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.Plot_RightExt_GraphJudge(_Rdata._Step2ExtnGraph_NG, Convert.ToSingle(_data.Step1Param[1]))));
                        _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.Plot_RightDiff_GraphJudge(_Rdata._Step2DiffGraph_NG, Convert.ToSingle(_data.Step1Param[1]))));
                    }

                    _kvconnObject.writeDataCommand("W0C2", "", "0");
                    Thread.Sleep(1);
                    _realtimeReadFlag = true;
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
        void _excelReadMasterData(string modfile, ref EXCELSTREAM masterfile, ref DATAMODEL_MASTER masterdata)
        {
            masterfile.FileReadMaster(modfile);
            masterdata._activeModelName = masterfile.getModelName();
            masterdata._activeKayabaNumber = masterfile.getKYBNUM();

            masterdata._MaxLoadLimit = (float)masterfile.getParameterMaxLoad();
            masterdata._ProdLen = (float)masterfile.getParameterProdLength();

            masterdata.Step1Param = ParamStep1toObject(masterfile.getParameterStep1());
            masterdata.Step2345Param = ParamStep2345toObject(masterfile.getParameterStep2345());
            masterdata.DiffParam = ParamDifftoObject(masterfile.getParameterStep2345_2());

            masterdata.RMasteringStep2 = masterfile.getRsideMasterStep2();
            masterdata.LMasteringStep2 = masterfile.getLsideMasterStep2();

            masterdata.RMasteringStep3 = masterfile.getRsideMasterStep3();
            masterdata.LMasteringStep3 = masterfile.getLsideMasterStep3();


        }

        bool MasterValidation(ref DATAMODEL_MASTER masterdata)
        {
            bool confirm = true;
            if (masterdata._activeModelName == null || masterdata._activeModelName == "")
            {
                MasterDataValidationReset();
                confirm = false;
            }
            if (masterdata._MaxLoadLimit == 0.0f || masterdata._ProdLen == 0.0f)
            {
                MasterDataValidationReset();
                confirm = false;
            }

            if (Convert.ToInt16(masterdata.Step1Param[0]) != 0)
            {
                for (int i = 1; i < masterdata.Step1Param.Count; i++)
                {
                    if (TryConvertToInt(masterdata.Step1Param[i], out var val_int))
                    {
                        if (val_int == 0)
                        {
                            MasterDataValidationReset();
                            confirm = false;
                        }
                    }
                    if (TryConvertToFloat(masterdata.Step1Param[i], out var val_float))
                    {
                        if (val_float == 0.0f)
                        {
                            MasterDataValidationReset();
                            confirm = false;
                        }
                    }
                }
            }

            if (Convert.ToInt16(masterdata.Step2345Param[0]) != 0)
            {
                for (int i = 1; i < 10; i++)
                {
                    if (i != 2 || i != 6 || i != 9)
                    {
                        if (TryConvertToInt(masterdata.Step2345Param[i], out var val_int))
                        {
                            if (val_int == 0)
                            {
                                MasterDataValidationReset();
                                confirm = false;
                            }
                        }
                        if (TryConvertToFloat(masterdata.Step2345Param[i], out var val_float))
                        {
                            if (val_float == 0.0f)
                            {
                                MasterDataValidationReset();
                                confirm = false;
                            }
                        }
                    }
                }

                foreach (List<float> float_list in masterdata.RMasteringStep2)
                {
                    if (float_list.Sum() == 0.0f)
                    {
                        MasterDataValidationReset();
                        confirm = false;
                    }
                }

                foreach (List<float> float_list in masterdata.LMasteringStep2)
                {
                    if (float_list.Sum() == 0.0f)
                    {
                        MasterDataValidationReset();
                        confirm = false;
                    }
                }
            }

            if (Convert.ToInt16(masterdata.Step2345Param[10]) != 0)
            {
                for (int i = 11; i < 20; i++)
                {
                    if (i != 12 || i != 16 || i != 19)
                    {
                        if (TryConvertToInt(masterdata.Step2345Param[i], out var val_int))
                        {
                            if (val_int == 0)
                            {
                                MasterDataValidationReset();
                                confirm = false;
                            }
                        }
                        if (TryConvertToFloat(masterdata.Step2345Param[i], out var val_float))
                        {
                            if (val_float == 0.0f)
                            {
                                MasterDataValidationReset();
                                confirm = false;
                            }
                        }
                    }
                }

                foreach (List<float> float_list in masterdata.RMasteringStep3)
                {
                    if (float_list.Sum() == 0.0f)
                    {
                        MasterDataValidationReset();
                        confirm = false;
                    }
                }

                foreach (List<float> float_list in masterdata.LMasteringStep3)
                {
                    if (float_list.Sum() == 0.0f)
                    {
                        MasterDataValidationReset();
                        confirm = false;
                    }
                }
            }
            return confirm;
        }
        void _excelInitMasterData(ref DATAMODEL_MASTER feeddata)
        {
            EXCELSTREAM newmasterfile = new EXCELSTREAM("MASTER");
            _excelStoreMasterParamData(ref feeddata, ref newmasterfile);
            _excelPrintMasterData(ref feeddata, ref newmasterfile);
        }
        void _excelStoreMasterGraphData(ref DATAMODEL_MASTER feeddata, ref EXCELSTREAM excelmaster)
        {
            excelmaster.setRsideMasterStep2(feeddata.RMasteringStep2);
            excelmaster.setLsideMasterStep2(feeddata.LMasteringStep2);
            //excelmaster.setRsideMasterStep3(feeddata.RMasteringStep3);
            //excelmaster.setLsideMasterStep3(feeddata.LMasteringStep3);
        }
        void _excelStoreMasterParamData(ref DATAMODEL_MASTER feeddata, ref EXCELSTREAM excelmaster)
        {
            excelmaster.setModelName(feeddata._activeModelName);
            excelmaster.setKYBNUM(feeddata._activeKayabaNumber);
            excelmaster.setParameterStep1(feeddata.Step1Param);
            excelmaster.setParameterStep2345(feeddata.Step2345Param);
            excelmaster.setParameterStep2345_2(feeddata.DiffParam);

            excelmaster.setParameterMaxLoad(feeddata._MaxLoadLimit);
            excelmaster.setParameterProdLength(feeddata._ProdLen);
        }
        void _excelPrintMasterData(ref DATAMODEL_MASTER feeddata, ref EXCELSTREAM exceldata)
        {
            string _filename = ($"{MasterDir}\\{feeddata._activeModelName}.xlsx");
            exceldata.FilePrint(_filename);
        }
        void _excelStoreParameterData(ref DATAMODEL_COMMON feeddata, ref EXCELSTREAM exceldata)
        {
            exceldata.setModelName(feeddata._activeModelName);
            exceldata.setKYBNUM(feeddata._activeKayabaNumber);
            exceldata.setParameterStep1(feeddata.Step1Param);
            exceldata.setParameterStep2345(feeddata.Step2345Param);
            exceldata.setParameterStep2345_2(feeddata.DiffParam);

            exceldata.setParameterMaxLoad(feeddata._MaxLoadLimit);
            exceldata.setParameterProdLength(feeddata._ProdLen);
        }
        void _excelStoreRealtimeData(ref DATAMODEL_COMMON datacm, ref DATAMODEL_RL datarl, ref EXCELSTREAM exceldata, string side)
        {
            string DirRealtime = RealLogDir + $"YEAR_{datacm.DTM[0]}\\MONTH_{datacm.DTM[1]}\\DAY_{datacm.DTM[2]}";
            CheckFolderPath(DirRealtime);

            exceldata.RESET_LABEL_NG();
            exceldata.setDateTime(datacm.DTM);

            exceldata.setParameterMaxLoad(datacm._MaxLoadLimit);
            exceldata.setParameterProdLength(datacm._ProdLen);

            exceldata.setRealtimeJudgement(datarl.Judgement);
            exceldata.setRealtimeStep2(datarl.RealtimeStep2);

            if (datarl._Step1MaxLoad_NG == 1 | datarl._Step2CompRef_NG == 1 | datarl._Step2CompGraph_NG == 1 | datarl._Step2ExtnRef_NG == 1 | datarl._Step2ExtnGraph_NG == 1 | datarl._Step2DiffGraph_NG == 1)
            {
                exceldata.SET_LABEL_NG();

                if (datarl._Step1MaxLoad_NG == 1)
                {
                    exceldata.STEP1_MAXLOAD_NG_SET();
                }
                if (datarl._Step2CompRef_NG == 1)
                {
                    exceldata.STEP2_COMP_REF_NG_SET();
                }
                if (datarl._Step2CompGraph_NG == 1)
                {
                    exceldata.STEP2_COMP_GRAPH_NG_SET();
                }
                if (datarl._Step2ExtnRef_NG == 1)
                {
                    exceldata.STEP2_EXTN_REF_NG_SET();
                }
                if (datarl._Step2ExtnGraph_NG == 1)
                {
                    exceldata.STEP2_EXTN_GRAPH_NG_SET();
                }
                if (datarl._Step2DiffGraph_NG == 1)
                {
                    exceldata.STEP2_DIFF_GRAPH_NG_SET();
                }
                string _filename = ($"{DirRealtime}\\RealtimeData_{side}H_{datacm.DTM[3]}-{datacm.DTM[4]}-{datacm.DTM[5]}_NG_RESULT.xlsx");
                exceldata.FilePrint(_filename);

            }
            else
            {
                string _filenames = ($"{DirRealtime}\\RealtimeData_{side}H_{datacm.DTM[3]}-{datacm.DTM[4]}-{datacm.DTM[5]}.xlsx");
                exceldata.FilePrint(_filenames);
            }
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
        void _eeipreadActiveModelData(ref DATAMODEL_COMMON data)
        {
            try
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
                                if (_charINPUT[i] != 0x00)
                                {
                                    _charModelBuff[i] = _charINPUT[i];
                                }
                            }
                            else
                            {
                                if (_charINPUT[i + 1] != 0x00)
                                {
                                    _charModelBuff[i] = _charINPUT[i + 1];
                                }
                            }
                        }
                        else if (i % 2 == 1)
                        {
                            if (_charINPUT[i - 1] != 0x00)
                            {
                                _charModelBuff[i] = _charINPUT[i - 1];
                            }
                        }
                    }
                    else
                    {
                        if (i % 2 == 0)
                        {
                            if (i > _charINPUT.Length - 2)
                            {
                                if (_charINPUT[i] != 0x00)
                                {
                                    _charNumBuff[i - 20] = _charINPUT[i];
                                }
                            }
                            else
                            {
                                if (_charINPUT[i + 1] != 0x00)
                                {
                                    _charNumBuff[i - 20] = _charINPUT[i + 1];
                                }
                            }
                        }
                        else if (i % 2 == 1)
                        {
                            if (_charINPUT[i - 1] != 0x00)
                            {
                                _charNumBuff[i - 20] = _charINPUT[i - 1];
                            }
                        }
                    }
                }
                data._activeModelName = string.Join("", _charModelBuff.Where(c => c != '\0'));
                //Debug.Write(_data._activeModelName);
                //Debug.Write((char)'\n');
                data._activeKayabaNumber = string.Join("", _charNumBuff.Where(c => c != '\0'));
                //Debug.Write(_data._activeKayabaNumber);
                //Debug.Write((char)'\n');
            }
            catch { }
        }
        void _eeipreadDateTime(ref DATAMODEL_COMMON data)
        {
            try
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

                for (int i = 0; i < data.DTM.Count; i++)
                {
                    if (i == 0)
                    {
                        data.DTM[i] = Convert.ToString(2000 + _buffDTM[i]);
                    }
                    else
                    {
                        data.DTM[i] = _buffDTM[i].ToString();
                    }
                }
            }
            catch { }
        }
        //----
        void _eeipreadParamSizeUpload(ref DATAMODEL_COMMON datacommon)
        {
            try
            {
                byte[] _INPUT;
                List<byte[]> _buffPARAM1 = new List<byte[]>();
                _INPUT = _eeipObject.AssemblyObject.getInstance(0xB1);
                Thread.Sleep(1);

                byte[] buff = new byte[4];
                int iv = 0;
                for (int i = 0; i < _INPUT.Length; i++)
                {
                    if (i % 4 != 0 && i != (_INPUT.Length - 1))
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
                    else if (i == (_INPUT.Length - 1))
                    {
                        buff[iv] = _INPUT[i];
                        byte[] sbuff = new byte[] { };
                        Array.Resize(ref sbuff, buff.Length);
                        Buffer.BlockCopy(buff, 0, sbuff, 0, sbuff.Length);
                        _buffPARAM1.Add(sbuff);
                    }

                }
                Debug.WriteLine(BitConverter.ToSingle(_buffPARAM1[1], 0));
                datacommon._MaxLoadLimit = BitConverter.ToSingle(_buffPARAM1[1], 0);
                Debug.WriteLine(BitConverter.ToSingle(_buffPARAM1[2], 0));
                datacommon._ProdLen = BitConverter.ToSingle(_buffPARAM1[2], 0);
            }
            catch { }
        }
        //----
        void _eeipreadStep1Param(ref DATAMODEL_COMMON data)
        {
            try
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


                for (int i = 0; i < data.Step1Param.Count; i++)
                {
                    if (i == 0 | i == 4)
                    {
                        data.Step1Param[i] = BitConverter.ToInt32(_buffPARAM1[i], 0);
                    }
                    else
                    {
                        data.Step1Param[i] = BitConverter.ToSingle(_buffPARAM1[i], 0);
                    }
                }
            }
            catch { }
        }
        void _eeipreadStep2345Param(ref DATAMODEL_COMMON data)
        {
            try
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

                for (int i = 0; i < data.Step2345Param.Count; i++)
                {
                    if (i == 0 | i == 9 | i == 10 | i == 19)
                    {
                        data.Step2345Param[i] = BitConverter.ToInt16(_buffPARAM2345[i], 0);
                    }
                    else
                    {
                        data.Step2345Param[i] = BitConverter.ToSingle(_buffPARAM2345[i], 0);
                    }
                }
            }
            catch { }
        }

        void _eeipreadDiffParam(ref DATAMODEL_COMMON data)
        {
            try
            {
                byte[] _INPUT;
                List<byte[]> _buffDiffPARAM = new List<byte[]>();
                _INPUT = _eeipObject.AssemblyObject.getInstance(0xA9);
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
                        _buffDiffPARAM.Add(sbuff);
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

                            _buffDiffPARAM.Add(sbuff);
                            iv = 0;

                            buff[iv] = _INPUT[i];
                            iv++;
                        }
                    }
                }

                for (int i = 0; i < data.DiffParam.Count; i++)
                {
                    data.DiffParam[i] = BitConverter.ToSingle(_buffDiffPARAM[i], 0);
                }
            }
            catch { }
        }

        void _eeipreadJudgement(ref List<float> judgementresult, Int16 addr)
        {
            try
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
            catch { }
        }

        void _eeipreadResultan(ref List<float> resultants, Int16 addr)
        {
            try
            {
                byte[] _INPUT = _eeipObject.AssemblyObject.getInstance(addr);
                Thread.Sleep(1);

                float[] _buffresultants = new float[] { };
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
                        Array.Resize(ref _buffresultants, iz);
                        _buffresultants[iz - 1] = BitConverter.ToSingle(buff, 0);
                        resultants[iz - 1] = _buffresultants[iz - 1];
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
                            Array.Resize(ref _buffresultants, iz);
                            _buffresultants[iz - 1] = BitConverter.ToSingle(buff, 0);
                            resultants[iz - 1] = _buffresultants[iz - 1];
                            iv = 0;
                            Array.Clear(buff);

                            buff[iv] = _INPUT[i];
                            iv++;
                        }
                    }
                }
            }
            catch { }
        }

        void _eeipreadRealtime(ref List<List<object>> realtimeresult, Int16 addr)
        {
            try
            {

            }
            catch { }
        }
        void _kvreadRealtime(ref List<List<float>> realtimeresult, string[] addrs, int count)
        {
            try
            {
                realtimeresult.Clear();

                if (addrs.Length != 6)
                {
                    throw new ArgumentException("Array must have exactly 6 elements.");
                }

                for (int iv = 0; iv < addrs.Length; iv++)
                {
                    List<byte[]> DataList = new List<byte[]>(_kvconnObject.batchreadDataCommandInHex(addrs[iv], count));
                    realtimeresult.Add(hex16tofloat32(DataList));
                }
            }
            catch { }
        }
        void _kvreadComponent(ref List<float> datacomponents, string addrs, int count)
        {
            try
            {
                List<byte[]> DataList = new List<byte[]>(_kvconnObject.batchreadDataCommandInHex(addrs, count));
                datacomponents = hex16tofloat32(DataList);
            }
            catch { }
        }

        void _eeipMasterModelUpload(ref DATAMODEL_MASTER master)
        {
            try
            {
                byte[] _INPUT;
                _INPUT = _eeipObject.AssemblyObject.getInstance(0xAB);
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
                                if (_charINPUT[i] != 0x00)
                                {
                                    _charModelBuff[i] = _charINPUT[i];
                                }
                            }
                            else
                            {
                                if (_charINPUT[i + 1] != 0x00)
                                {
                                    _charModelBuff[i] = _charINPUT[i + 1];
                                }
                            }
                        }
                        else if (i % 2 == 1)
                        {
                            if (_charINPUT[i - 1] != 0x00)
                            {
                                _charModelBuff[i] = _charINPUT[i - 1];
                            }
                        }
                    }
                    else
                    {
                        if (i % 2 == 0)
                        {
                            if (i > _charINPUT.Length - 2)
                            {
                                if (_charINPUT[i] != 0x00)
                                {
                                    _charNumBuff[i - 20] = _charINPUT[i];
                                }
                            }
                            else
                            {
                                if (_charINPUT[i + 1] != 0x00)
                                {
                                    _charNumBuff[i - 20] = _charINPUT[i + 1];
                                }
                            }
                        }
                        else if (i % 2 == 1)
                        {
                            if (_charINPUT[i - 1] != 0x00)
                            {
                                _charNumBuff[i - 20] = _charINPUT[i - 1];
                            }
                        }
                    }
                }
                Debug.WriteLine(string.Join("", _charModelBuff.Where(c => c != '\0')));
                master._activeModelName = string.Join("", _charModelBuff.Where(c => c != '\0'));
                Debug.WriteLine(string.Join("", _charNumBuff.Where(c => c != '\0')));
                master._activeKayabaNumber = string.Join("", _charNumBuff.Where(c => c != '\0'));
            }
            catch { }
        }

        void _kvMasterModelDownload(ref DATAMODEL_MASTER master)
        {
            try
            {
                string[] hexModelBuff = StringToHex(SplitString2C(master._activeModelName));
                Array.Resize(ref hexModelBuff, 10);
                string[] hexNumBuff = StringToHex(SplitString2C(master._activeKayabaNumber));
                Array.Resize(ref hexNumBuff, 10);

                for (int i = 0; i < (hexModelBuff.Length); i++)
                {
                    if (hexModelBuff[i] == null)
                        hexModelBuff[i] = ("0000");
                }

                for (int i = 0; i < (hexNumBuff.Length); i++)
                {
                    if (hexNumBuff[i] == null)
                        hexNumBuff[i] = ("0000");
                }

                _kvconnObject.batchwriteDataCommand("W300", ".H", hexModelBuff.Length, hexModelBuff);
                Thread.Sleep(1);
                _kvconnObject.batchwriteDataCommand("W310", ".H", hexNumBuff.Length, hexNumBuff);
                Thread.Sleep(1);
            }
            catch { }
        }
        void _eeipMasterParamSizeUpload(ref DATAMODEL_MASTER masterparam1)
        {
            try
            {
                byte[] _INPUT;
                List<byte[]> _buffPARAM1 = new List<byte[]>();
                _INPUT = _eeipObject.AssemblyObject.getInstance(0xAF);
                Thread.Sleep(1);

                byte[] buff = new byte[4];
                int iv = 0;
                for (int i = 0; i < _INPUT.Length; i++)
                {
                    if (i % 4 != 0 && i != (_INPUT.Length - 1))
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
                    else if (i == (_INPUT.Length - 1))
                    {
                        buff[iv] = _INPUT[i];
                        byte[] sbuff = new byte[] { };
                        Array.Resize(ref sbuff, buff.Length);
                        Buffer.BlockCopy(buff, 0, sbuff, 0, sbuff.Length);
                        _buffPARAM1.Add(sbuff);
                    }

                }
                Debug.WriteLine(BitConverter.ToSingle(_buffPARAM1[1], 0));
                masterparam1._MaxLoadLimit = BitConverter.ToSingle(_buffPARAM1[1], 0);
                Debug.WriteLine(BitConverter.ToSingle(_buffPARAM1[2], 0));
                masterparam1._ProdLen = BitConverter.ToSingle(_buffPARAM1[2], 0);
            }
            catch { }
        }
        void _kvMasterParamSizeDownload(ref DATAMODEL_MASTER masterparam1) //not-yet confirmed, need excelstream format update
        {
            string[] tfdata = new string[] { };
            try
            {
                if (masterparam1._MaxLoadLimit is float value1)
                {
                    AppendToArray(ref tfdata, FloatToHexArray((float)value1));
                    Debug.WriteLine((float)value1);
                }
                if (masterparam1._ProdLen is float value2)
                {
                    AppendToArray(ref tfdata, FloatToHexArray(value2));
                    Debug.WriteLine((float)value2);
                }

                _kvconnObject.batchwriteDataCommand("W320", ".H", tfdata.Length, tfdata);
                Thread.Sleep(1);
            }
            catch { }
        }
        void _eeipMasterParam1Upload(ref DATAMODEL_MASTER masterparam1)
        {
            try
            {
                byte[] _INPUT;
                List<byte[]> _buffPARAM1 = new List<byte[]>();
                _INPUT = _eeipObject.AssemblyObject.getInstance(0xAD);
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
                for (int i = 0; i < masterparam1.Step1Param.Count; i++)
                {
                    if (i == 0 | i == 4)
                    {
                        Debug.WriteLine(BitConverter.ToInt16(_buffPARAM1[i], 0));
                        masterparam1.Step1Param[i] = BitConverter.ToInt16(_buffPARAM1[i], 0);
                    }
                    else
                    {
                        Debug.WriteLine(BitConverter.ToSingle(_buffPARAM1[i], 0));
                        masterparam1.Step1Param[i] = BitConverter.ToSingle(_buffPARAM1[i], 0);
                    }
                }
            }
            catch { }
        }
        void _kvMasterParam1Download(ref DATAMODEL_MASTER masterparam1)
        {
            string[] tfdata = new string[] { };
            try
            {
                for (int i = 0; i < masterparam1.Step1Param.Count; i++)
                {
                    if (masterparam1.Step1Param[i] is short value1)
                    {
                        AppendToArray(ref tfdata, IntToHex((int)value1));
                        Debug.WriteLine(value1);
                    }
                    else if (masterparam1.Step1Param[i] is float value2)
                    {
                        AppendToArray(ref tfdata, FloatToHexArray(value2));
                        Debug.WriteLine((float)value2);
                    }
                }
                _kvconnObject.batchwriteDataCommand("W330", ".H", tfdata.Length, tfdata);
                Thread.Sleep(1);
            }
            catch { }
        }

        void _eeipMasterParam2345Upload(ref DATAMODEL_MASTER masterparam)
        {
            try
            {
                byte[] _INPUT;
                List<byte[]> _buffPARAM2345 = new List<byte[]>();
                _INPUT = _eeipObject.AssemblyObject.getInstance(0xAE);
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

                for (int i = 0; i < masterparam.Step2345Param.Count; i++)
                {
                    if (i == 0 | i == 9 | i == 10 | i == 19)
                    {
                        Debug.WriteLine(BitConverter.ToInt16(_buffPARAM2345[i], 0));
                        masterparam.Step2345Param[i] = BitConverter.ToInt16(_buffPARAM2345[i], 0);
                    }
                    else
                    {
                        Debug.WriteLine(BitConverter.ToSingle(_buffPARAM2345[i], 0));
                        masterparam.Step2345Param[i] = BitConverter.ToSingle(_buffPARAM2345[i], 0);
                    }
                }
            }
            catch { }
        }

        void _eeipMasterDiffParamUpload(ref DATAMODEL_MASTER masterparam)
        {
            try
            {
                byte[] _INPUT;
                List<byte[]> _buffDiffPARAM = new List<byte[]>();
                _INPUT = _eeipObject.AssemblyObject.getInstance(0xA8);
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
                        _buffDiffPARAM.Add(sbuff);
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

                            _buffDiffPARAM.Add(sbuff);
                            iv = 0;

                            buff[iv] = _INPUT[i];
                            iv++;
                        }
                    }
                }

                for (int i = 0; i < masterparam.DiffParam.Count; i++)
                {
                    Debug.WriteLine(BitConverter.ToSingle(_buffDiffPARAM[i], 0));
                    masterparam.DiffParam[i] = BitConverter.ToSingle(_buffDiffPARAM[i], 0);
                }
            }
            catch { }
        }

        void _kvMasterParam2345Download(ref DATAMODEL_MASTER masterparam)
        {
            string[] tfdata = new string[] { };
            try
            {
                for (int i = 0; i < masterparam.Step2345Param.Count; i++)
                {
                    //Debug.WriteLine((float)masterparam2345.Step2345Param[i]);
                    if (masterparam.Step2345Param[i] is short value1)
                    {
                        AppendToArray(ref tfdata, IntToHex((int)value1));
                        Debug.WriteLine(value1);
                    }
                    else if (masterparam.Step2345Param[i] is float value2)
                    {
                        AppendToArray(ref tfdata, FloatToHexArray(value2));
                        Debug.WriteLine((float)value2);
                    }
                }
                _kvconnObject.batchwriteDataCommand("W340", ".H", tfdata.Length, tfdata);
                Thread.Sleep(1);
            }
            catch { }
        }

        void _kvMasterParamDiffDownload(ref DATAMODEL_MASTER masterparam)
        {
            string[] tfdata = new string[] { };
            try
            {
                for (int i = 0; i < masterparam.DiffParam.Count; i++)
                {
                    if (masterparam.DiffParam[i] is float value)
                    {
                        AppendToArray(ref tfdata, FloatToHexArray(value));
                        Debug.WriteLine((float)value);
                    }
                }
                _kvconnObject.batchwriteDataCommand("W380", ".H", tfdata.Length, tfdata);
                Thread.Sleep(1);
            }
            catch { }
        }

        void _kvMasterGraphUpload(ref List<List<float>> masterdata, string[] addrs, int count)
        {
            try
            {
                masterdata.Clear();

                if (addrs.Length != 12)
                {
                    throw new ArgumentException("Array must have exactly 12 elements.");
                }

                for (int iv = 0; iv < addrs.Length; iv++)
                {
                    List<byte[]> masterDataList = new List<byte[]>(_kvconnObject.batchreadDataCommandInHex(addrs[iv], count));
                    masterdata.Add(hex16tofloat32(masterDataList));
                }
            }
            catch { }
        }
        void _kvMasterGraphDownload(ref List<List<float>> masterdata, string[] addrs, int count)
        {
            //try
            {
                //masterdata.Clear();

                if (addrs.Length != 12)
                {
                    throw new ArgumentException("Array must have exactly 12 elements.");
                }

                for (int iv = 0; iv < masterdata.Count; iv++)
                {
                    string[] masterdatalist = new string[] { };
                    for (int ivy = 0; ivy < masterdata[iv].Count; ivy++)
                    {
                        if (masterdata[iv][ivy] is float value) AppendToArray(ref masterdatalist, FloatToHexArray(value));
                    }
                    _kvconnObject.batchwriteDataCommand(addrs[iv], ".H", masterdatalist.Length, masterdatalist);
                    Thread.Sleep(1);
                }
            }
            //catch { }
        }
        void _kvMasterTeachDataUpload(ref List<List<float>> MTeachData, string[] addrs, int count)
        {
            //try
            {
                MTeachData.Clear();
                for (int iv = 0; iv < addrs.Length; iv++)
                {
                    List<byte[]> DataList = new List<byte[]>(_kvconnObject.batchreadDataCommandInHex(addrs[iv], count));
                    MTeachData.Add(hex16tofloat32(DataList));
                }
            }
            //catch { }
        }

        void _updateMasterDatabase()
        {
            _kvMasterGraphUpload(ref _masterData.RMasteringStep2, dataMasterRsideStep2addrs, 400);
            _kvMasterGraphUpload(ref _masterData.LMasteringStep2, dataMasterLsideStep2addrs, 400);
            //_kvMasterGraphUpload(ref _masterData.RMasteringStep3, dataMasterRsideStep3addrs, 400);
            //_kvMasterGraphUpload(ref _masterData.LMasteringStep3, dataMasterLsideStep3addrs, 400);
            _excelStoreMasterGraphData(ref _masterData, ref MasterFileActive);
            _excelPrintMasterData(ref _masterData, ref MasterFileActive);

            //_kvconnObject.writeDataCommand("W0FA", "", "1");
        }

        string ParseByteString(byte[] MODNAME_BYTE)
        {
            try
            {
                char[] _charINPUT;
                char[] _charStringBuff = new char[20];
                _charINPUT = System.Text.Encoding.ASCII.GetString(MODNAME_BYTE).ToCharArray();

                for (int i = 0; i < _charINPUT.Length; i++)
                {
                    if (i < 20)
                    {
                        if (i % 2 == 0)
                        {
                            if (i > _charINPUT.Length - 2)
                            {
                                if (_charINPUT[i] != 0x00)
                                {
                                    _charStringBuff[i] = _charINPUT[i];
                                }
                            }
                            else
                            {
                                if (_charINPUT[i + 1] != 0x00)
                                {
                                    _charStringBuff[i] = _charINPUT[i + 1];
                                }
                            }
                        }
                        else if (i % 2 == 1)
                        {
                            if (_charINPUT[i - 1] != 0x00)
                            {
                                _charStringBuff[i] = _charINPUT[i - 1];
                            }
                        }
                    }
                }
                return string.Join("", _charStringBuff.Where(c => c != '\0'));
            }
            catch
            {
                return "";
            }
        }

        List<object> ParamStep1toObject<T>(List<T> dataread)
        {
            List<object> dataobject = new List<object>();
            for (int i = 0; i < dataread.Count; i++)
            {
                if (i == 0 | i == 4)
                {
                    dataobject.Add(Convert.ToInt16(dataread[i]));
                }
                else
                {
                    dataobject.Add(Convert.ToSingle(dataread[i]));
                }
            }
            return dataobject;
        }

        List<object> ParamStep2345toObject<T>(List<T> dataread)
        {
            List<object> dataobject = new List<object>();
            for (int i = 0; i < dataread.Count; i++)
            {
                if (i == 0 | i == 9 | i == 10 | i == 19)
                {
                    dataobject.Add(Convert.ToInt16(dataread[i]));
                }
                else
                {
                    dataobject.Add(Convert.ToSingle(dataread[i]));
                }
            }
            return dataobject;
        }

        List<object> ParamDifftoObject<T>(List<T> dataread)
        {
            List<object> dataobject = new List<object>();
            for (int i = 0; i < dataread.Count; i++)
            {
                dataobject.Add(Convert.ToSingle(dataread[i]));
            }
            return dataobject;
        }

        static string StringToHex(string input)
        {
            return string.Concat(input.Select(c => c == ' ' ? "00" : ((int)c).ToString("X2")));
        }

        static string[] StringToHex(string[] input)
        {
            return input.Select(ips => StringToHex(ips)).ToArray();
        }

        static string[] SplitString2C(string input)
        {
            if (input.Length % 2 != 0) input += " ";
            return Enumerable.Range(0, input.Length / 2)
                             .Select(i => input.Substring(i * 2, 2))
                             .ToArray();
        }

        public static string IntToHex(int num)
        {
            return num.ToString("X");
        }

        static string FloatToHex(float num)
        {
            byte[] bytes = BitConverter.GetBytes(num);
            string hexString = BitConverter.ToString(bytes).Replace("-", "");
            string swappedHex = string.Concat(Enumerable.Range(0, hexString.Length / 4)
                                                         .Select(i =>
                                                         {
                                                             string segment = hexString.Substring(i * 4, 4);
                                                             return segment.Substring(2, 2) + segment.Substring(0, 2);
                                                         }));
            return string.Concat(Enumerable.Range(0, swappedHex.Length / 8)
                                           .Select(i =>
                                           {
                                               string segment = swappedHex.Substring(i * 8, 8);
                                               return segment.Substring(4, 4) + segment.Substring(0, 4);
                                           }));
        }

        static string[] FloatToHexArray(float num)
        {
            string hexString = FloatToHex(num);
            string[] segments = Enumerable.Range(0, hexString.Length / 4)
                                          .Select(i => hexString.Substring(i * 4, 4))
                                          .ToArray();
            return segments.Select((value, index) => new { value, index })
                           .OrderBy(x => x.index % 2 == 0 ? x.index + 1 : x.index - 1)
                           .Select(x => x.value)
                           .ToArray();
        }

        static void AppendToArray<T>(ref T[] array, T newItem)
        {
            T[] newArray = new T[array.Length + 1];
            Array.Copy(array, newArray, array.Length);
            newArray[newArray.Length - 1] = newItem;
            Array.Resize(ref array, newArray.Length);
            Array.Copy(newArray, array, newArray.Length);
        }

        static void AppendToArray<T>(ref T[] array, T[] newItems)
        {
            T[] newArray = new T[array.Length + newItems.Length];
            if (array.Length > 0)
            {
                Array.Copy(array, newArray, array.Length);
                Array.Copy(newItems, 0, newArray, array.Length, newItems.Length);
            }
            else { Array.Copy(newItems, newArray, newItems.Length); }
            Array.Resize(ref array, newArray.Length);
            Array.Copy(newArray, array, newArray.Length);
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

        static bool isnotNull(float[] arraydata)
        {
            float buffer = arraydata.Sum();
            return buffer != 0;
        }

        static bool isnotNull(double[] arraydata)
        {
            double buffer = arraydata.Sum();
            return buffer != 0;
        }

        static bool isnotNull(List<float> arraydata)
        {
            float buffer = arraydata.Sum();
            return buffer != 0;
        }

        static bool isnotNull(List<double> arraydata)
        {
            double buffer = arraydata.Sum();
            return buffer != 0;
        }

        static bool isnotNull(List<List<float>> arraydata)
        {
            bool bbuff = true;
            foreach (List<float> sublist in arraydata)
            {
                float buffer = sublist.Sum();
                bbuff = bbuff & (buffer != 0);
            }
            return bbuff;
        }

        static bool isnotNull(List<List<double>> arraydata)
        {
            bool bbuff = true;
            foreach (List<double> sublist in arraydata)
            {
                double buffer = sublist.Sum();
                bbuff = bbuff & (buffer != 0);
            }
            return bbuff;
        }

        public static bool TryConvertToInt(object value, out int result)
        {
            result = 0;
            if (value == null)
                return false;

            // Try direct cast
            if (value is int i)
            {
                result = i;
                return true;
            }

            // Try parsing string representation
            return int.TryParse(value.ToString(), out result);
        }

        public static bool TryConvertToFloat(object value, out float result)
        {
            result = 0f;
            if (value == null)
                return false;

            // Try direct cast
            if (value is float f)
            {
                result = f;
                return true;
            }

            // Try parsing string representation
            return float.TryParse(value.ToString(), out result);
        }

        private async Task InvokeAsync(Action action, CancellationToken cancellationToken)
        {
            await Task.Run(() => action(), cancellationToken);
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
                    await _kvMasterConfirmAsync(_cts.Token);
                    await _eeipEventHandler_1Async(_cts.Token);
                    await _eeipEventHandler_2Async(_cts.Token);
                    await _eeipEventHandler_3Async(_cts.Token);
                    await _eeipEventHandler_4Async(_cts.Token);
                    await _eeipEventHandler_5Async(_cts.Token);

                    await _dataRefMonitorAsync(_cts.Token);


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
        private async Task _eeipEventHandler_4Async(CancellationToken cancellationToken)
        {
            await InvokeAsync(() => _eeipEventHandler_4(), cancellationToken);
        }
        private async Task _eeipEventHandler_5Async(CancellationToken cancellationToken)
        {
            await InvokeAsync(() => _eeipEventHandler_5(), cancellationToken);
        }
        private async Task _uibeaconnUpdateAsync(CancellationToken cancellationToken)
        {
            await InvokeAsync(() => _uibeaconnUpdate(), cancellationToken);
        }
        private async Task _kvMasterConfirmAsync(CancellationToken cancellationToken)
        {
            await InvokeAsync(() => _kvMasterConfirm(), cancellationToken);
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
        private async Task _dataRefMonitorAsync(CancellationToken cancellationToken)
        {
            await InvokeAsync(() => _dataRefMonitor(), cancellationToken);
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

        void _dataRefMonitor()
        {
            if (this.GetConnState() == 1)
            {
                byte[] DATA_INPUT = _eeipObject.AssemblyObject.getInstance(0xB0);
                Thread.Sleep(1);

                _dataLoadPosMonitor(DATA_INPUT);
            }

        }

        void _dataLoadPosMonitor(byte[] DATABytes)
        {
            byte[] _buffLLoad = new byte[4];
            byte[] _buffLPos = new byte[4];
            byte[] _buffRLoad = new byte[4];
            byte[] _buffRPos = new byte[4];
            int _buffLLoadIndex = 0;
            int _buffLPosIndex = 4;
            int _buffRLoadIndex = 8;
            int _buffRPosIndex = 12;
            float _LLoad;
            float _LPos;
            float _RLoad;
            float _RPos;

            for (int i = 0; i < 4; i++)
            {
                _buffLLoad[i] = DATABytes[_buffLLoadIndex + i];
                _buffLPos[i] = DATABytes[_buffLPosIndex + i];
                _buffRLoad[i] = DATABytes[_buffRLoadIndex + i];
                _buffRPos[i] = DATABytes[_buffRPosIndex + i];
            }

            _LLoad = BitConverter.ToSingle(_buffLLoad, 0);
            _LPos = BitConverter.ToSingle(_buffLPos, 0);
            _RLoad = BitConverter.ToSingle(_buffRLoad, 0);
            _RPos = BitConverter.ToSingle(_buffRPos, 0);

            if (_uiObject.InvokeRequired)
            {
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.uiLoadPosMonitor(_LLoad, _LPos, _RLoad, _RPos)));
            }
        }

        #region PlotDataObject

        System.Drawing.Color D1Col;
        System.Drawing.Color D2Col;
        System.Drawing.Color D3Col;
        System.Drawing.Color D4Col;
        System.Drawing.Color D5Col;
        System.Drawing.Color D6Col;

        System.Drawing.Color Master_D1Col = System.Drawing.Color.Gold;
        System.Drawing.Color Master_D2Col = System.Drawing.Color.Gold;
        System.Drawing.Color Master_D3Col = System.Drawing.Color.Gold;
        System.Drawing.Color Master_D4Col = System.Drawing.Color.Gold;
        System.Drawing.Color Master_D5Col = System.Drawing.Color.Gold;
        System.Drawing.Color Master_D6Col = System.Drawing.Color.Gold;

        System.Drawing.Color LLim_D1Col = System.Drawing.ColorTranslator.FromHtml("#ff6700");
        System.Drawing.Color LLim_D2Col = System.Drawing.ColorTranslator.FromHtml("#ff6700");
        System.Drawing.Color LLim_D3Col = System.Drawing.ColorTranslator.FromHtml("#ff6700");
        System.Drawing.Color LLim_D4Col = System.Drawing.ColorTranslator.FromHtml("#ff6700");
        System.Drawing.Color LLim_D5Col = System.Drawing.ColorTranslator.FromHtml("#ff6700");
        System.Drawing.Color LLim_D6Col = System.Drawing.ColorTranslator.FromHtml("#ff6700");

        System.Drawing.Color HLim_D1Col = System.Drawing.ColorTranslator.FromHtml("#ff6700");
        System.Drawing.Color HLim_D2Col = System.Drawing.ColorTranslator.FromHtml("#ff6700");
        System.Drawing.Color HLim_D3Col = System.Drawing.ColorTranslator.FromHtml("#ff6700");
        System.Drawing.Color HLim_D4Col = System.Drawing.ColorTranslator.FromHtml("#ff6700");
        System.Drawing.Color HLim_D5Col = System.Drawing.ColorTranslator.FromHtml("#ff6700");
        System.Drawing.Color HLim_D6Col = System.Drawing.ColorTranslator.FromHtml("#ff6700");

        //RealX
        public double[] dXD1;
        public double[] dXD2;
        public double[] dXD3;
        public double[] dXD4;
        public double[] dXD5;
        public double[] dXD6;
        //RealY
        public double[] dYD1;
        public double[] dYD2;
        public double[] dYD3;
        public double[] dYD4;
        public double[] dYD5;
        public double[] dYD6;

        //MasterX
        public double[] Master_dXD1;
        public double[] Master_dXD2;
        public double[] Master_dXD3;
        public double[] Master_dXD4;
        public double[] Master_dXD5;
        public double[] Master_dXD6;
        //MasterY
        public double[] Master_dYD1;
        public double[] Master_dYD2;
        public double[] Master_dYD3;
        public double[] Master_dYD4;
        public double[] Master_dYD5;
        public double[] Master_dYD6;

        //UpperX
        public double[] Upper_dXD1;
        public double[] Upper_dXD2;
        public double[] Upper_dXD3;
        public double[] Upper_dXD4;
        public double[] Upper_dXD5;
        public double[] Upper_dXD6;
        //UpperY
        public double[] Upper_dYD1;
        public double[] Upper_dYD2;
        public double[] Upper_dYD3;
        public double[] Upper_dYD4;
        public double[] Upper_dYD5;
        public double[] Upper_dYD6;

        //LowerX
        public double[] Lower_dXD1;
        public double[] Lower_dXD2;
        public double[] Lower_dXD3;
        public double[] Lower_dXD4;
        public double[] Lower_dXD5;
        public double[] Lower_dXD6;
        //LowerY
        public double[] Lower_dYD1;
        public double[] Lower_dYD2;
        public double[] Lower_dYD3;
        public double[] Lower_dYD4;
        public double[] Lower_dYD5;
        public double[] Lower_dYD6;

        System.Drawing.Color MMaster_D1Col = System.Drawing.Color.Gold;
        System.Drawing.Color MMaster_D2Col = System.Drawing.Color.Gold;
        System.Drawing.Color MMaster_D3Col = System.Drawing.Color.Gold;
        System.Drawing.Color MMaster_D4Col = System.Drawing.Color.Gold;
        System.Drawing.Color MMaster_D5Col = System.Drawing.Color.Gold;
        System.Drawing.Color MMaster_D6Col = System.Drawing.Color.Gold;

        System.Drawing.Color MTeach_D1Col = System.Drawing.Color.LimeGreen;
        System.Drawing.Color MTeach_D2Col = System.Drawing.Color.LimeGreen;
        System.Drawing.Color MTeach_D3Col = System.Drawing.Color.LimeGreen;
        System.Drawing.Color MTeach_D4Col = System.Drawing.Color.LimeGreen;
        System.Drawing.Color MTeach_D5Col = System.Drawing.Color.LimeGreen;
        System.Drawing.Color MTeach_D6Col = System.Drawing.Color.LimeGreen;

        System.Drawing.Color MLLim_D1Col = System.Drawing.ColorTranslator.FromHtml("#ff6700");
        System.Drawing.Color MLLim_D2Col = System.Drawing.ColorTranslator.FromHtml("#ff6700");
        System.Drawing.Color MLLim_D3Col = System.Drawing.ColorTranslator.FromHtml("#ff6700");
        System.Drawing.Color MLLim_D4Col = System.Drawing.ColorTranslator.FromHtml("#ff6700");
        System.Drawing.Color MLLim_D5Col = System.Drawing.ColorTranslator.FromHtml("#ff6700");
        System.Drawing.Color MLLim_D6Col = System.Drawing.ColorTranslator.FromHtml("#ff6700");

        System.Drawing.Color MHLim_D1Col = System.Drawing.ColorTranslator.FromHtml("#ff6700");
        System.Drawing.Color MHLim_D2Col = System.Drawing.ColorTranslator.FromHtml("#ff6700");
        System.Drawing.Color MHLim_D3Col = System.Drawing.ColorTranslator.FromHtml("#ff6700");
        System.Drawing.Color MHLim_D4Col = System.Drawing.ColorTranslator.FromHtml("#ff6700");
        System.Drawing.Color MHLim_D5Col = System.Drawing.ColorTranslator.FromHtml("#ff6700");
        System.Drawing.Color MHLim_D6Col = System.Drawing.ColorTranslator.FromHtml("#ff6700");

        //Master Data X
        public double[] MMaster_dXD1;
        public double[] MMaster_dXD2;
        public double[] MMaster_dXD3;
        public double[] MMaster_dXD4;
        public double[] MMaster_dXD5;
        public double[] MMaster_dXD6;
        //Master Data Y
        public double[] MMaster_dYD1;
        public double[] MMaster_dYD2;
        public double[] MMaster_dYD3;
        public double[] MMaster_dYD4;
        public double[] MMaster_dYD5;
        public double[] MMaster_dYD6;

        //Master Teach X
        public double[] MTeach_dXD1;
        public double[] MTeach_dXD2;
        public double[] MTeach_dXD3;
        public double[] MTeach_dXD4;
        public double[] MTeach_dXD5;
        public double[] MTeach_dXD6;
        //Master Teach Y
        public double[] MTeach_dYD1;
        public double[] MTeach_dYD2;
        public double[] MTeach_dYD3;
        public double[] MTeach_dYD4;
        public double[] MTeach_dYD5;
        public double[] MTeach_dYD6;

        //Master Upper X
        public double[] MUpper_dXD1;
        public double[] MUpper_dXD2;
        public double[] MUpper_dXD3;
        public double[] MUpper_dXD4;
        public double[] MUpper_dXD5;
        public double[] MUpper_dXD6;
        //Master Upper Y
        public double[] MUpper_dYD1;
        public double[] MUpper_dYD2;
        public double[] MUpper_dYD3;
        public double[] MUpper_dYD4;
        public double[] MUpper_dYD5;
        public double[] MUpper_dYD6;

        //Master Lower X
        public double[] MLower_dXD1;
        public double[] MLower_dXD2;
        public double[] MLower_dXD3;
        public double[] MLower_dXD4;
        public double[] MLower_dXD5;
        public double[] MLower_dXD6;
        //Master Lower Y
        public double[] MLower_dYD1;
        public double[] MLower_dYD2;
        public double[] MLower_dYD3;
        public double[] MLower_dYD4;
        public double[] MLower_dYD5;
        public double[] MLower_dYD6;

        bool _uiPlot1UpdateFlag;
        bool _uiPlot2UpdateFlag;
        bool _uiPlot3UpdateFlag;
        bool _uiPlot4UpdateFlag;
        bool _uiPlot5UpdateFlag;
        bool _uiPlot6UpdateFlag;
        bool _uiPlot7UpdateFlag;
        bool _uiPlot8UpdateFlag;
        bool _uiPlot9UpdateFlag;
        bool _uiPlot10UpdateFlag;
        bool _uiPlot11UpdateFlag;
        bool _uiPlot12UpdateFlag;

        bool _uiPlot1ResetFlag;
        bool _uiPlot2ResetFlag;
        bool _uiPlot3ResetFlag;
        bool _uiPlot4ResetFlag;
        bool _uiPlot5ResetFlag;
        bool _uiPlot6ResetFlag;
        bool _uiPlot7ResetFlag;
        bool _uiPlot8ResetFlag;
        bool _uiPlot9ResetFlag;
        bool _uiPlot10ResetFlag;
        bool _uiPlot11ResetFlag;
        bool _uiPlot12ResetFlag;

        #endregion

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

        void _uiPlot1Update()
        {
            if (_uiPlot1UpdateFlag)
            {
                double[] xd = new double[dXD1.Length];
                Array.Copy(dXD1, xd, dXD1.Length);

                double[] yd = new double[dYD1.Length];
                Array.Copy(dYD1, yd, dYD1.Length);

                if (_uiObject.InvokeRequired)
                {
                    //_uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.Plot1AddPlot(xd, yd, D1Col)));
                    //_uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.FormPlot1().Reset()));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot1(), ref _uiObject.Plot1_PRESENT, xd, yd)));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot1_PRESENT, D1Col)));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotBringToFront(ref _uiObject.FormPlot1(), ref _uiObject.Plot1_PRESENT)));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.workSumPlotCheck(ref _uiObject.FormPlot1())));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.FormPlot1().Refresh()));
                }
                else
                {
                    //_uiObject.Plot1Update(xd, yd, D1Col);
                    _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot1(), ref _uiObject.Plot1_PRESENT, xd, yd);
                    _uiObject.PlotChangeColor(ref _uiObject.Plot1_PRESENT, D1Col);
                    _uiObject.PlotBringToFront(ref _uiObject.FormPlot1(), ref _uiObject.Plot1_PRESENT);
                    _uiObject.workSumPlotCheck(ref _uiObject.FormPlot1());
                    _uiObject.FormPlot1().Refresh();
                }

                _uiPlot1UpdateFlag = false;

                //Thread.Sleep(10);
            }
        }
        void _uiPlot2Update()
        {
            if (_uiPlot2UpdateFlag)
            {
                double[] xd = new double[dXD2.Length];
                Array.Copy(dXD2, xd, dXD2.Length);

                double[] yd = new double[dYD2.Length];
                Array.Copy(dYD2, yd, dYD2.Length);

                if (_uiObject.InvokeRequired)
                {
                    //_uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.Plot2Update(xd, yd, D2Col)));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot2(), ref _uiObject.Plot2_PRESENT, xd, yd)));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot2_PRESENT, D2Col)));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotBringToFront(ref _uiObject.FormPlot2(), ref _uiObject.Plot2_PRESENT)));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.workSumPlotCheck(ref _uiObject.FormPlot2())));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.FormPlot2().Refresh()));
                }
                else
                {
                    //_uiObject.Plot2Update(xd, yd, D2Col);
                    _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot2(), ref _uiObject.Plot2_PRESENT, xd, yd);
                    _uiObject.PlotChangeColor(ref _uiObject.Plot2_PRESENT, D2Col);
                    _uiObject.PlotBringToFront(ref _uiObject.FormPlot2(), ref _uiObject.Plot2_PRESENT);
                    _uiObject.workSumPlotCheck(ref _uiObject.FormPlot2());
                    _uiObject.FormPlot2().Refresh();
                }

                _uiPlot2UpdateFlag = false;

                //Thread.Sleep(10);
            }
        }
        void _uiPlot3Update()
        {
            if (_uiPlot3UpdateFlag)
            {
                double[] xd = new double[dXD3.Length];
                Array.Copy(dXD3, xd, dXD3.Length);

                double[] yd = new double[dYD3.Length];
                Array.Copy(dYD3, yd, dYD3.Length);

                if (_uiObject.InvokeRequired)
                {
                    //_uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.Plot3Update(xd, yd, D3Col)));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot3(), ref _uiObject.Plot3_PRESENT, xd, yd)));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot3_PRESENT, D3Col)));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotBringToFront(ref _uiObject.FormPlot3(), ref _uiObject.Plot3_PRESENT)));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.workSumPlotCheck(ref _uiObject.FormPlot3())));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.FormPlot3().Refresh()));

                }
                else
                {
                    //_uiObject.Plot3Update(xd, yd, D3Col);
                    _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot3(), ref _uiObject.Plot3_PRESENT, xd, yd);
                    _uiObject.PlotChangeColor(ref _uiObject.Plot3_PRESENT, D3Col);
                    _uiObject.PlotBringToFront(ref _uiObject.FormPlot3(), ref _uiObject.Plot3_PRESENT);
                    _uiObject.workSumPlotCheck(ref _uiObject.FormPlot3());
                    _uiObject.FormPlot3().Refresh();
                }

                _uiPlot3UpdateFlag = false;

                //Thread.Sleep(10);
            }
        }
        void _uiPlot4Update()
        {
            if (_uiPlot4UpdateFlag)
            {
                double[] xd = new double[dXD4.Length];
                Array.Copy(dXD4, xd, dXD4.Length);

                double[] yd = new double[dYD4.Length];
                Array.Copy(dYD4, yd, dYD4.Length);

                if (_uiObject.InvokeRequired)
                {
                    //_uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.Plot4Update(xd, yd, D4Col)));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot4(), ref _uiObject.Plot4_PRESENT, xd, yd)));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot4_PRESENT, D4Col)));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotBringToFront(ref _uiObject.FormPlot4(), ref _uiObject.Plot4_PRESENT)));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.workSumPlotCheck(ref _uiObject.FormPlot4())));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.FormPlot4().Refresh()));
                }
                else
                {
                    //_uiObject.Plot4Update(xd, yd, D4Col);
                    _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot4(), ref _uiObject.Plot4_PRESENT, xd, yd);
                    _uiObject.PlotChangeColor(ref _uiObject.Plot4_PRESENT, D4Col);
                    _uiObject.PlotBringToFront(ref _uiObject.FormPlot4(), ref _uiObject.Plot4_PRESENT);
                    _uiObject.workSumPlotCheck(ref _uiObject.FormPlot4());
                    _uiObject.FormPlot4().Refresh();
                }

                _uiPlot4UpdateFlag = false;

                //Thread.Sleep(10);
            }
        }
        void _uiPlot9Update()
        {
            if (_uiPlot9UpdateFlag)
            {
                double[] xd = new double[dXD5.Length];
                Array.Copy(dXD5, xd, dXD5.Length);

                double[] yd = new double[dYD5.Length];
                Array.Copy(dYD5, yd, dYD5.Length);

                if (_uiObject.InvokeRequired)
                {
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot9(), ref _uiObject.Plot9_PRESENT, xd, yd)));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot9_PRESENT, D5Col)));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotBringToFront(ref _uiObject.FormPlot9(), ref _uiObject.Plot9_PRESENT)));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.workSumPlotCheck(ref _uiObject.FormPlot9())));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.FormPlot9().Refresh()));
                }
                else
                {
                    _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot9(), ref _uiObject.Plot9_PRESENT, xd, yd);
                    _uiObject.PlotChangeColor(ref _uiObject.Plot9_PRESENT, D5Col);
                    _uiObject.PlotBringToFront(ref _uiObject.FormPlot9(), ref _uiObject.Plot9_PRESENT);
                    _uiObject.workSumPlotCheck(ref _uiObject.FormPlot9());
                    _uiObject.FormPlot9().Refresh();
                }

                _uiPlot9UpdateFlag = false;

                //Thread.Sleep(10);
            }
        }
        void _uiPlot10Update()
        {
            if (_uiPlot10UpdateFlag)
            {
                double[] xd = new double[dXD6.Length];
                Array.Copy(dXD6, xd, dXD6.Length);

                double[] yd = new double[dYD6.Length];
                Array.Copy(dYD6, yd, dYD6.Length);

                if (_uiObject.InvokeRequired)
                {
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot10(), ref _uiObject.Plot10_PRESENT, xd, yd)));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot10_PRESENT, D6Col)));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotBringToFront(ref _uiObject.FormPlot10(), ref _uiObject.Plot10_PRESENT)));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.workSumPlotCheck(ref _uiObject.FormPlot10())));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.FormPlot10().Refresh()));
                }
                else
                {
                    _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot10(), ref _uiObject.Plot10_PRESENT, xd, yd);
                    _uiObject.PlotChangeColor(ref _uiObject.Plot10_PRESENT, D6Col);
                    _uiObject.PlotBringToFront(ref _uiObject.FormPlot10(), ref _uiObject.Plot10_PRESENT);
                    _uiObject.workSumPlotCheck(ref _uiObject.FormPlot10());
                    _uiObject.FormPlot10().Refresh();
                }

                _uiPlot10UpdateFlag = false;

                //Thread.Sleep(10);
            }
        }
        void _uiPlot5TeachUpdate()
        {
            if (_uiPlot5UpdateFlag)
            {
                double[] xd = new double[MTeach_dXD1.Length];
                Array.Copy(MTeach_dXD1, xd, MTeach_dXD1.Length);

                double[] yd = new double[MTeach_dYD1.Length];
                Array.Copy(MTeach_dYD1, yd, MTeach_dYD1.Length);

                if (_uiObject.InvokeRequired)
                {
                    //_uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.Plot1AddPlot(xd, yd, D1Col)));
                    //_uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.FormPlot1().Reset()));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot5(), ref _uiObject.Plot5_PRESENT, xd, yd)));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot5_PRESENT, MTeach_D1Col)));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotBringToFront(ref _uiObject.FormPlot5(), ref _uiObject.Plot5_PRESENT)));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.workSumPlotCheck(ref _uiObject.FormPlot5())));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.FormPlot5().Refresh()));
                }
                else
                {
                    //_uiObject.Plot1Update(xd, yd, D1Col);
                    _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot5(), ref _uiObject.Plot5_PRESENT, xd, yd);
                    _uiObject.PlotChangeColor(ref _uiObject.Plot5_PRESENT, MTeach_D1Col);
                    _uiObject.PlotBringToFront(ref _uiObject.FormPlot5(), ref _uiObject.Plot5_PRESENT);
                    _uiObject.workSumPlotCheck(ref _uiObject.FormPlot5());
                    _uiObject.FormPlot5().Refresh();
                }
                _uiPlot5UpdateFlag = false;
                //Thread.Sleep(10);
            }
        }
        void _uiPlot6TeachUpdate()
        {
            if (_uiPlot6UpdateFlag)
            {
                double[] xd = new double[MTeach_dXD2.Length];
                Array.Copy(MTeach_dXD2, xd, MTeach_dXD2.Length);

                double[] yd = new double[MTeach_dYD2.Length];
                Array.Copy(MTeach_dYD2, yd, MTeach_dYD2.Length);

                if (_uiObject.InvokeRequired)
                {
                    //_uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.Plot2Update(xd, yd, D2Col)));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot6(), ref _uiObject.Plot6_PRESENT, xd, yd)));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot6_PRESENT, MTeach_D2Col)));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotBringToFront(ref _uiObject.FormPlot6(), ref _uiObject.Plot6_PRESENT)));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.workSumPlotCheck(ref _uiObject.FormPlot6())));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.FormPlot6().Refresh()));
                }
                else
                {
                    //_uiObject.Plot2Update(xd, yd, D2Col);
                    _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot6(), ref _uiObject.Plot6_PRESENT, xd, yd);
                    _uiObject.PlotChangeColor(ref _uiObject.Plot6_PRESENT, MTeach_D2Col);
                    _uiObject.PlotBringToFront(ref _uiObject.FormPlot6(), ref _uiObject.Plot6_PRESENT);
                    _uiObject.workSumPlotCheck(ref _uiObject.FormPlot6());
                    _uiObject.FormPlot6().Refresh();
                }
                _uiPlot6UpdateFlag = false;
                //Thread.Sleep(10);
            }
        }
        void _uiPlot7TeachUpdate()
        {
            if (_uiPlot7UpdateFlag)
            {
                double[] xd = new double[MTeach_dXD3.Length];
                Array.Copy(MTeach_dXD3, xd, MTeach_dXD3.Length);

                double[] yd = new double[MTeach_dYD3.Length];
                Array.Copy(MTeach_dYD3, yd, MTeach_dYD3.Length);

                if (_uiObject.InvokeRequired)
                {
                    //_uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.Plot3Update(xd, yd, D3Col)));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot7(), ref _uiObject.Plot7_PRESENT, xd, yd)));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot7_PRESENT, MTeach_D3Col)));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotBringToFront(ref _uiObject.FormPlot7(), ref _uiObject.Plot7_PRESENT)));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.workSumPlotCheck(ref _uiObject.FormPlot7())));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.FormPlot7().Refresh()));

                }
                else
                {
                    //_uiObject.Plot3Update(xd, yd, D3Col);
                    _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot7(), ref _uiObject.Plot7_PRESENT, xd, yd);
                    _uiObject.PlotChangeColor(ref _uiObject.Plot7_PRESENT, MTeach_D3Col);
                    _uiObject.PlotBringToFront(ref _uiObject.FormPlot7(), ref _uiObject.Plot7_PRESENT);
                    _uiObject.workSumPlotCheck(ref _uiObject.FormPlot7());
                    _uiObject.FormPlot7().Refresh();
                }

                _uiPlot7UpdateFlag = false;
                //Thread.Sleep(10);
            }
        }
        void _uiPlot8TeachUpdate()
        {
            if (_uiPlot8UpdateFlag)
            {
                double[] xd = new double[MTeach_dXD4.Length];
                Array.Copy(MTeach_dXD4, xd, MTeach_dXD4.Length);

                double[] yd = new double[MTeach_dYD4.Length];
                Array.Copy(MTeach_dYD4, yd, MTeach_dYD4.Length);

                if (_uiObject.InvokeRequired)
                {
                    //_uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.Plot4Update(xd, yd, D4Col)));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot8(), ref _uiObject.Plot8_PRESENT, xd, yd)));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot8_PRESENT, MTeach_D4Col)));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotBringToFront(ref _uiObject.FormPlot8(), ref _uiObject.Plot8_PRESENT)));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.workSumPlotCheck(ref _uiObject.FormPlot8())));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.FormPlot8().Refresh()));
                }
                else
                {
                    //_uiObject.Plot4Update(xd, yd, D4Col);
                    _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot8(), ref _uiObject.Plot8_PRESENT, xd, yd);
                    _uiObject.PlotChangeColor(ref _uiObject.Plot8_PRESENT, MTeach_D4Col);
                    _uiObject.PlotBringToFront(ref _uiObject.FormPlot8(), ref _uiObject.Plot8_PRESENT);
                    _uiObject.workSumPlotCheck(ref _uiObject.FormPlot8());
                    _uiObject.FormPlot8().Refresh();
                }
                _uiPlot8UpdateFlag = false;
                //Thread.Sleep(10);
            }
        }
        void _uiPlot11TeachUpdate()
        {
            if (_uiPlot11UpdateFlag)
            {
                double[] xd = new double[MTeach_dXD5.Length];
                Array.Copy(MTeach_dXD5, xd, MTeach_dXD5.Length);

                double[] yd = new double[MTeach_dYD5.Length];
                Array.Copy(MTeach_dYD5, yd, MTeach_dYD5.Length);

                if (_uiObject.InvokeRequired)
                {
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot11(), ref _uiObject.Plot11_PRESENT, xd, yd)));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot11_PRESENT, MTeach_D5Col)));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotBringToFront(ref _uiObject.FormPlot11(), ref _uiObject.Plot11_PRESENT)));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.workSumPlotCheck(ref _uiObject.FormPlot11())));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.FormPlot11().Refresh()));
                }
                else
                {
                    _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot11(), ref _uiObject.Plot11_PRESENT, xd, yd);
                    _uiObject.PlotChangeColor(ref _uiObject.Plot11_PRESENT, MTeach_D5Col);
                    _uiObject.PlotBringToFront(ref _uiObject.FormPlot11(), ref _uiObject.Plot11_PRESENT);
                    _uiObject.workSumPlotCheck(ref _uiObject.FormPlot11());
                    _uiObject.FormPlot11().Refresh();
                }
                _uiPlot11UpdateFlag = false;
                //Thread.Sleep(10);
            }
        }
        void _uiPlot12TeachUpdate()
        {
            if (_uiPlot12UpdateFlag)
            {
                double[] xd = new double[MTeach_dXD6.Length];
                Array.Copy(MTeach_dXD6, xd, MTeach_dXD6.Length);

                double[] yd = new double[MTeach_dYD6.Length];
                Array.Copy(MTeach_dYD6, yd, MTeach_dYD6.Length);

                if (_uiObject.InvokeRequired)
                {
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot12(), ref _uiObject.Plot12_PRESENT, xd, yd)));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot12_PRESENT, MTeach_D6Col)));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotBringToFront(ref _uiObject.FormPlot12(), ref _uiObject.Plot12_PRESENT)));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.workSumPlotCheck(ref _uiObject.FormPlot12())));
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.FormPlot12().Refresh()));
                }
                else
                {
                    _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot12(), ref _uiObject.Plot12_PRESENT, xd, yd);
                    _uiObject.PlotChangeColor(ref _uiObject.Plot12_PRESENT, MTeach_D6Col);
                    _uiObject.PlotBringToFront(ref _uiObject.FormPlot12(), ref _uiObject.Plot12_PRESENT);
                    _uiObject.workSumPlotCheck(ref _uiObject.FormPlot12());
                    _uiObject.FormPlot12().Refresh();
                }
                _uiPlot12UpdateFlag = false;
                //Thread.Sleep(10);
            }
        }
        void uiPlotLTeachUpdate()
        {
            _uiPlot5TeachUpdate();
            _uiPlot6TeachUpdate();
            _uiPlot11TeachUpdate();
        }
        void uiPlotRTeachUpdate()
        {
            _uiPlot7TeachUpdate();
            _uiPlot8TeachUpdate();
            _uiPlot12TeachUpdate();
        }

        void _uiPlot1MasterUpdate()
        {
            double[] xdA = new double[Master_dXD1.Length];
            Array.Copy(Master_dXD1, xdA, Master_dXD1.Length);
            double[] ydA = new double[Master_dYD1.Length];
            Array.Copy(Master_dYD1, ydA, Master_dYD1.Length);

            double[] xdB = new double[Upper_dXD1.Length];
            Array.Copy(Upper_dXD1, xdB, Upper_dXD1.Length);
            double[] ydB = new double[Upper_dYD1.Length];
            Array.Copy(Upper_dYD1, ydB, Upper_dYD1.Length);

            double[] xdC = new double[Lower_dXD1.Length];
            Array.Copy(Lower_dXD1, xdC, Lower_dXD1.Length);
            double[] ydC = new double[Lower_dYD1.Length];
            Array.Copy(Lower_dYD1, ydC, Lower_dYD1.Length);

            if (_uiObject.InvokeRequired)
            {
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot1(), ref _uiObject.Plot1_MASTER, xdA, ydA)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot1_MASTER, Master_D1Col)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot1(), ref _uiObject.Plot1_UPPER, xdB, ydB)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot1_UPPER, HLim_D1Col)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot1(), ref _uiObject.Plot1_LOWER, xdC, ydC)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot1_LOWER, LLim_D1Col)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.workSumPlotCheck(ref _uiObject.FormPlot1())));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.FormPlot1().Refresh()));
            }
            else
            {
                _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot1(), ref _uiObject.Plot1_MASTER, xdA, ydA);
                _uiObject.PlotChangeColor(ref _uiObject.Plot1_MASTER, Master_D1Col);
                _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot1(), ref _uiObject.Plot1_UPPER, xdB, ydB);
                _uiObject.PlotChangeColor(ref _uiObject.Plot1_UPPER, HLim_D1Col);
                _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot1(), ref _uiObject.Plot1_LOWER, xdC, ydC);
                _uiObject.PlotChangeColor(ref _uiObject.Plot1_LOWER, LLim_D1Col);
                _uiObject.workSumPlotCheck(ref _uiObject.FormPlot1());
                _uiObject.FormPlot1().Refresh();
            }
        }
        void _uiPlot2MasterUpdate()
        {
            double[] xdA = new double[Master_dXD2.Length];
            Array.Copy(Master_dXD2, xdA, Master_dXD2.Length);
            double[] ydA = new double[Master_dYD2.Length];
            Array.Copy(Master_dYD2, ydA, Master_dYD2.Length);

            double[] xdB = new double[Upper_dXD2.Length];
            Array.Copy(Upper_dXD2, xdB, Upper_dXD2.Length);
            double[] ydB = new double[Upper_dYD2.Length];
            Array.Copy(Upper_dYD2, ydB, Upper_dYD2.Length);

            double[] xdC = new double[Lower_dXD2.Length];
            Array.Copy(Lower_dXD2, xdC, Lower_dXD2.Length);
            double[] ydC = new double[Lower_dYD2.Length];
            Array.Copy(Lower_dYD2, ydC, Lower_dYD2.Length);

            if (_uiObject.InvokeRequired)
            {
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot2(), ref _uiObject.Plot2_MASTER, xdA, ydA)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot2_MASTER, Master_D2Col)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot2(), ref _uiObject.Plot2_UPPER, xdB, ydB)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot2_UPPER, HLim_D2Col)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot2(), ref _uiObject.Plot2_LOWER, xdC, ydC)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot2_LOWER, LLim_D2Col)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.workSumPlotCheck(ref _uiObject.FormPlot2())));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.FormPlot2().Refresh()));
            }
            else
            {
                _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot2(), ref _uiObject.Plot2_MASTER, xdA, ydA);
                _uiObject.PlotChangeColor(ref _uiObject.Plot2_MASTER, Master_D2Col);
                _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot2(), ref _uiObject.Plot2_UPPER, xdB, ydB);
                _uiObject.PlotChangeColor(ref _uiObject.Plot2_UPPER, HLim_D2Col);
                _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot2(), ref _uiObject.Plot2_LOWER, xdC, ydC);
                _uiObject.PlotChangeColor(ref _uiObject.Plot2_LOWER, LLim_D2Col);
                _uiObject.workSumPlotCheck(ref _uiObject.FormPlot2());
                _uiObject.FormPlot2().Refresh();
            }
        }
        void _uiPlot3MasterUpdate()
        {
            double[] xdA = new double[Master_dXD3.Length];
            Array.Copy(Master_dXD3, xdA, Master_dXD3.Length);
            double[] ydA = new double[Master_dYD3.Length];
            Array.Copy(Master_dYD3, ydA, Master_dYD3.Length);

            double[] xdB = new double[Upper_dXD3.Length];
            Array.Copy(Upper_dXD3, xdB, Upper_dXD3.Length);
            double[] ydB = new double[Upper_dYD3.Length];
            Array.Copy(Upper_dYD3, ydB, Upper_dYD3.Length);

            double[] xdC = new double[Lower_dXD3.Length];
            Array.Copy(Lower_dXD3, xdC, Lower_dXD3.Length);
            double[] ydC = new double[Lower_dYD3.Length];
            Array.Copy(Lower_dYD3, ydC, Lower_dYD3.Length);

            if (_uiObject.InvokeRequired)
            {
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot3(), ref _uiObject.Plot3_MASTER, xdA, ydA)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot3_MASTER, Master_D3Col)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot3(), ref _uiObject.Plot3_UPPER, xdB, ydB)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot3_UPPER, HLim_D3Col)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot3(), ref _uiObject.Plot3_LOWER, xdC, ydC)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot3_LOWER, LLim_D3Col)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.workSumPlotCheck(ref _uiObject.FormPlot3())));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.FormPlot3().Refresh()));
            }
            else
            {
                _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot3(), ref _uiObject.Plot3_MASTER, xdA, ydA);
                _uiObject.PlotChangeColor(ref _uiObject.Plot3_MASTER, Master_D3Col);
                _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot3(), ref _uiObject.Plot3_UPPER, xdB, ydB);
                _uiObject.PlotChangeColor(ref _uiObject.Plot3_UPPER, HLim_D3Col);
                _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot3(), ref _uiObject.Plot3_LOWER, xdC, ydC);
                _uiObject.PlotChangeColor(ref _uiObject.Plot3_LOWER, LLim_D3Col);
                _uiObject.workSumPlotCheck(ref _uiObject.FormPlot3());
                _uiObject.FormPlot3().Refresh();
            }
        }
        void _uiPlot4MasterUpdate()
        {
            double[] xdA = new double[Master_dXD4.Length];
            Array.Copy(Master_dXD4, xdA, Master_dXD4.Length);
            double[] ydA = new double[Master_dYD4.Length];
            Array.Copy(Master_dYD4, ydA, Master_dYD4.Length);

            double[] xdB = new double[Upper_dXD4.Length];
            Array.Copy(Upper_dXD4, xdB, Upper_dXD4.Length);
            double[] ydB = new double[Upper_dYD4.Length];
            Array.Copy(Upper_dYD4, ydB, Upper_dYD4.Length);

            double[] xdC = new double[Lower_dXD4.Length];
            Array.Copy(Lower_dXD4, xdC, Lower_dXD4.Length);
            double[] ydC = new double[Lower_dYD4.Length];
            Array.Copy(Lower_dYD4, ydC, Lower_dYD4.Length);

            if (_uiObject.InvokeRequired)
            {
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot4(), ref _uiObject.Plot4_MASTER, xdA, ydA)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot4_MASTER, Master_D4Col)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot4(), ref _uiObject.Plot4_UPPER, xdB, ydB)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot4_UPPER, HLim_D4Col)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot4(), ref _uiObject.Plot4_LOWER, xdC, ydC)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot4_LOWER, LLim_D4Col)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.workSumPlotCheck(ref _uiObject.FormPlot4())));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.FormPlot4().Refresh()));
            }
            else
            {
                _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot4(), ref _uiObject.Plot4_MASTER, xdA, ydA);
                _uiObject.PlotChangeColor(ref _uiObject.Plot4_MASTER, Master_D4Col);
                _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot4(), ref _uiObject.Plot4_UPPER, xdB, ydB);
                _uiObject.PlotChangeColor(ref _uiObject.Plot4_UPPER, HLim_D4Col);
                _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot4(), ref _uiObject.Plot4_LOWER, xdC, ydC);
                _uiObject.PlotChangeColor(ref _uiObject.Plot4_LOWER, LLim_D4Col);
                _uiObject.workSumPlotCheck(ref _uiObject.FormPlot4());
                _uiObject.FormPlot4().Refresh();
            }
        }
        void _uiPlot9MasterUpdate()
        {
            double[] xdA = new double[Master_dXD5.Length];
            Array.Copy(Master_dXD5, xdA, Master_dXD5.Length);
            double[] ydA = new double[Master_dYD5.Length];
            Array.Copy(Master_dYD5, ydA, Master_dYD5.Length);

            double[] xdB = new double[Upper_dXD5.Length];
            Array.Copy(Upper_dXD5, xdB, Upper_dXD5.Length);
            double[] ydB = new double[Upper_dYD5.Length];
            Array.Copy(Upper_dYD5, ydB, Upper_dYD5.Length);

            double[] xdC = new double[Lower_dXD5.Length];
            Array.Copy(Lower_dXD5, xdC, Lower_dXD5.Length);
            double[] ydC = new double[Lower_dYD5.Length];
            Array.Copy(Lower_dYD5, ydC, Lower_dYD5.Length);

            if (_uiObject.InvokeRequired)
            {
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot9(), ref _uiObject.Plot9_MASTER, xdA, ydA)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot9_MASTER, Master_D5Col)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot9(), ref _uiObject.Plot9_UPPER, xdB, ydB)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot9_UPPER, HLim_D5Col)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot9(), ref _uiObject.Plot9_LOWER, xdC, ydC)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot9_LOWER, LLim_D5Col)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.workSumPlotCheck(ref _uiObject.FormPlot9())));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotZoomout(ref _uiObject.FormPlot9(), 1.3)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.FormPlot9().Refresh()));
            }
            else
            {
                _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot9(), ref _uiObject.Plot9_MASTER, xdA, ydA);
                _uiObject.PlotChangeColor(ref _uiObject.Plot9_MASTER, Master_D5Col);
                _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot9(), ref _uiObject.Plot9_UPPER, xdB, ydB);
                _uiObject.PlotChangeColor(ref _uiObject.Plot9_UPPER, HLim_D5Col);
                _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot9(), ref _uiObject.Plot9_LOWER, xdC, ydC);
                _uiObject.PlotChangeColor(ref _uiObject.Plot9_LOWER, LLim_D5Col);
                _uiObject.workSumPlotCheck(ref _uiObject.FormPlot9());
                _uiObject.PlotZoomout(ref _uiObject.FormPlot9(), 1.3);
                _uiObject.FormPlot9().Refresh();
            }
        }
        void _uiPlot10MasterUpdate()
        {
            double[] xdA = new double[Master_dXD6.Length];
            Array.Copy(Master_dXD6, xdA, Master_dXD6.Length);
            double[] ydA = new double[Master_dYD6.Length];
            Array.Copy(Master_dYD6, ydA, Master_dYD6.Length);

            double[] xdB = new double[Upper_dXD6.Length];
            Array.Copy(Upper_dXD6, xdB, Upper_dXD6.Length);
            double[] ydB = new double[Upper_dYD6.Length];
            Array.Copy(Upper_dYD6, ydB, Upper_dYD6.Length);

            double[] xdC = new double[Lower_dXD6.Length];
            Array.Copy(Lower_dXD6, xdC, Lower_dXD6.Length);
            double[] ydC = new double[Lower_dYD6.Length];
            Array.Copy(Lower_dYD6, ydC, Lower_dYD6.Length);

            if (_uiObject.InvokeRequired)
            {
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot10(), ref _uiObject.Plot10_MASTER, xdA, ydA)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot10_MASTER, Master_D6Col)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot10(), ref _uiObject.Plot10_UPPER, xdB, ydB)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot10_UPPER, HLim_D6Col)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot10(), ref _uiObject.Plot10_LOWER, xdC, ydC)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot10_LOWER, LLim_D6Col)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.workSumPlotCheck(ref _uiObject.FormPlot10())));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotZoomout(ref _uiObject.FormPlot10(), 1.3)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.FormPlot10().Refresh()));
            }
            else
            {
                _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot10(), ref _uiObject.Plot10_MASTER, xdA, ydA);
                _uiObject.PlotChangeColor(ref _uiObject.Plot10_MASTER, Master_D6Col);
                _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot10(), ref _uiObject.Plot10_UPPER, xdB, ydB);
                _uiObject.PlotChangeColor(ref _uiObject.Plot10_UPPER, HLim_D6Col);
                _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot10(), ref _uiObject.Plot10_LOWER, xdC, ydC);
                _uiObject.PlotChangeColor(ref _uiObject.Plot10_LOWER, LLim_D6Col);
                _uiObject.workSumPlotCheck(ref _uiObject.FormPlot10());
                _uiObject.PlotZoomout(ref _uiObject.FormPlot10(), 1.3);
                _uiObject.FormPlot10().Refresh();
            }
        }
        void _uiPlot5MasterUpdate()
        {
            double[] xdA = new double[MMaster_dXD1.Length];
            Array.Copy(MMaster_dXD1, xdA, MMaster_dXD1.Length);
            double[] ydA = new double[MMaster_dYD1.Length];
            Array.Copy(MMaster_dYD1, ydA, MMaster_dYD1.Length);

            double[] xdB = new double[MUpper_dXD1.Length];
            Array.Copy(MUpper_dXD1, xdB, MUpper_dXD1.Length);
            double[] ydB = new double[MUpper_dYD1.Length];
            Array.Copy(MUpper_dYD1, ydB, MUpper_dYD1.Length);

            double[] xdC = new double[MLower_dXD1.Length];
            Array.Copy(MLower_dXD1, xdC, MLower_dXD1.Length);
            double[] ydC = new double[MLower_dYD1.Length];
            Array.Copy(MLower_dYD1, ydC, MLower_dYD1.Length);

            if (_uiObject.InvokeRequired)
            {
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot5(), ref _uiObject.Plot5_MASTER, xdA, ydA)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot5_MASTER, MMaster_D1Col)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot5(), ref _uiObject.Plot5_UPPER, xdB, ydB)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot5_UPPER, MHLim_D1Col)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot5(), ref _uiObject.Plot5_LOWER, xdC, ydC)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot5_LOWER, MLLim_D1Col)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.workSumPlotCheck(ref _uiObject.FormPlot5())));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.FormPlot5().Refresh()));
            }
            else
            {
                _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot5(), ref _uiObject.Plot5_MASTER, xdA, ydA);
                _uiObject.PlotChangeColor(ref _uiObject.Plot5_MASTER, MMaster_D1Col);
                _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot5(), ref _uiObject.Plot5_UPPER, xdB, ydB);
                _uiObject.PlotChangeColor(ref _uiObject.Plot5_UPPER, MHLim_D1Col);
                _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot5(), ref _uiObject.Plot5_LOWER, xdC, ydC);
                _uiObject.PlotChangeColor(ref _uiObject.Plot5_LOWER, MLLim_D1Col);
                _uiObject.workSumPlotCheck(ref _uiObject.FormPlot5());
                _uiObject.FormPlot5().Refresh();
            }
        }
        void _uiPlot6MasterUpdate()
        {
            double[] xdA = new double[MMaster_dXD2.Length];
            Array.Copy(MMaster_dXD2, xdA, MMaster_dXD2.Length);
            double[] ydA = new double[MMaster_dYD2.Length];
            Array.Copy(MMaster_dYD2, ydA, MMaster_dYD2.Length);

            double[] xdB = new double[MUpper_dXD2.Length];
            Array.Copy(MUpper_dXD2, xdB, MUpper_dXD2.Length);
            double[] ydB = new double[MUpper_dYD2.Length];
            Array.Copy(MUpper_dYD2, ydB, MUpper_dYD2.Length);

            double[] xdC = new double[MLower_dXD2.Length];
            Array.Copy(MLower_dXD2, xdC, MLower_dXD2.Length);
            double[] ydC = new double[MLower_dYD2.Length];
            Array.Copy(MLower_dYD2, ydC, MLower_dYD2.Length);

            if (_uiObject.InvokeRequired)
            {
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot6(), ref _uiObject.Plot6_MASTER, xdA, ydA)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot6_MASTER, MMaster_D2Col)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot6(), ref _uiObject.Plot6_UPPER, xdB, ydB)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot6_UPPER, MHLim_D2Col)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot6(), ref _uiObject.Plot6_LOWER, xdC, ydC)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot6_LOWER, MLLim_D2Col)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.workSumPlotCheck(ref _uiObject.FormPlot6())));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.FormPlot6().Refresh()));
            }
            else
            {
                _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot6(), ref _uiObject.Plot6_MASTER, xdA, ydA);
                _uiObject.PlotChangeColor(ref _uiObject.Plot6_MASTER, MMaster_D2Col);
                _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot6(), ref _uiObject.Plot6_UPPER, xdB, ydB);
                _uiObject.PlotChangeColor(ref _uiObject.Plot6_UPPER, MHLim_D2Col);
                _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot6(), ref _uiObject.Plot6_LOWER, xdC, ydC);
                _uiObject.PlotChangeColor(ref _uiObject.Plot6_LOWER, MLLim_D2Col);
                _uiObject.workSumPlotCheck(ref _uiObject.FormPlot6());
                _uiObject.FormPlot6().Refresh();
            }
        }
        void _uiPlot7MasterUpdate()
        {
            double[] xdA = new double[MMaster_dXD3.Length];
            Array.Copy(MMaster_dXD3, xdA, MMaster_dXD3.Length);
            double[] ydA = new double[MMaster_dYD3.Length];
            Array.Copy(MMaster_dYD3, ydA, MMaster_dYD3.Length);

            double[] xdB = new double[MUpper_dXD3.Length];
            Array.Copy(MUpper_dXD3, xdB, MUpper_dXD3.Length);
            double[] ydB = new double[MUpper_dYD3.Length];
            Array.Copy(MUpper_dYD3, ydB, MUpper_dYD3.Length);

            double[] xdC = new double[MLower_dXD3.Length];
            Array.Copy(MLower_dXD3, xdC, MLower_dXD3.Length);
            double[] ydC = new double[MLower_dYD3.Length];
            Array.Copy(MLower_dYD3, ydC, MLower_dYD3.Length);

            if (_uiObject.InvokeRequired)
            {
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot7(), ref _uiObject.Plot7_MASTER, xdA, ydA)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot7_MASTER, MMaster_D3Col)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot7(), ref _uiObject.Plot7_UPPER, xdB, ydB)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot7_UPPER, MHLim_D3Col)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot7(), ref _uiObject.Plot7_LOWER, xdC, ydC)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot7_LOWER, MLLim_D3Col)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.workSumPlotCheck(ref _uiObject.FormPlot7())));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.FormPlot7().Refresh()));
            }
            else
            {
                _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot7(), ref _uiObject.Plot7_MASTER, xdA, ydA);
                _uiObject.PlotChangeColor(ref _uiObject.Plot7_MASTER, MMaster_D3Col);
                _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot7(), ref _uiObject.Plot7_UPPER, xdB, ydB);
                _uiObject.PlotChangeColor(ref _uiObject.Plot7_UPPER, MHLim_D3Col);
                _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot7(), ref _uiObject.Plot7_LOWER, xdC, ydC);
                _uiObject.PlotChangeColor(ref _uiObject.Plot7_LOWER, MLLim_D3Col);
                _uiObject.workSumPlotCheck(ref _uiObject.FormPlot7());
                _uiObject.FormPlot7().Refresh();
            }
        }
        void _uiPlot8MasterUpdate()
        {
            double[] xdA = new double[MMaster_dXD4.Length];
            Array.Copy(MMaster_dXD4, xdA, MMaster_dXD4.Length);
            double[] ydA = new double[MMaster_dYD4.Length];
            Array.Copy(MMaster_dYD4, ydA, MMaster_dYD4.Length);

            double[] xdB = new double[MUpper_dXD4.Length];
            Array.Copy(MUpper_dXD4, xdB, MUpper_dXD4.Length);
            double[] ydB = new double[MUpper_dYD4.Length];
            Array.Copy(MUpper_dYD4, ydB, MUpper_dYD4.Length);

            double[] xdC = new double[MLower_dXD4.Length];
            Array.Copy(MLower_dXD4, xdC, MLower_dXD4.Length);
            double[] ydC = new double[MLower_dYD4.Length];
            Array.Copy(MLower_dYD4, ydC, MLower_dYD4.Length);

            if (_uiObject.InvokeRequired)
            {
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot8(), ref _uiObject.Plot8_MASTER, xdA, ydA)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot8_MASTER, MMaster_D4Col)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot8(), ref _uiObject.Plot8_UPPER, xdB, ydB)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot8_UPPER, MHLim_D4Col)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot8(), ref _uiObject.Plot8_LOWER, xdC, ydC)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot8_LOWER, MLLim_D4Col)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.workSumPlotCheck(ref _uiObject.FormPlot8())));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.FormPlot8().Refresh()));
            }
            else
            {
                _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot8(), ref _uiObject.Plot8_MASTER, xdA, ydA);
                _uiObject.PlotChangeColor(ref _uiObject.Plot8_MASTER, MMaster_D4Col);
                _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot8(), ref _uiObject.Plot8_UPPER, xdB, ydB);
                _uiObject.PlotChangeColor(ref _uiObject.Plot8_UPPER, MHLim_D4Col);
                _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot8(), ref _uiObject.Plot8_LOWER, xdC, ydC);
                _uiObject.PlotChangeColor(ref _uiObject.Plot8_LOWER, MLLim_D4Col);
                _uiObject.workSumPlotCheck(ref _uiObject.FormPlot8());
                _uiObject.FormPlot8().Refresh();
            }
        }
        void _uiPlot11MasterUpdate()
        {
            double[] xdA = new double[MMaster_dXD5.Length];
            Array.Copy(MMaster_dXD5, xdA, MMaster_dXD5.Length);
            double[] ydA = new double[MMaster_dYD5.Length];
            Array.Copy(MMaster_dYD5, ydA, MMaster_dYD5.Length);

            double[] xdB = new double[MUpper_dXD5.Length];
            Array.Copy(MUpper_dXD5, xdB, MUpper_dXD5.Length);
            double[] ydB = new double[MUpper_dYD5.Length];
            Array.Copy(MUpper_dYD5, ydB, MUpper_dYD5.Length);

            double[] xdC = new double[MLower_dXD5.Length];
            Array.Copy(MLower_dXD5, xdC, MLower_dXD5.Length);
            double[] ydC = new double[MLower_dYD5.Length];
            Array.Copy(MLower_dYD5, ydC, MLower_dYD5.Length);

            if (_uiObject.InvokeRequired)
            {
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot11(), ref _uiObject.Plot11_MASTER, xdA, ydA)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot11_MASTER, MMaster_D5Col)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot11(), ref _uiObject.Plot11_UPPER, xdB, ydB)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot11_UPPER, MHLim_D5Col)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot11(), ref _uiObject.Plot11_LOWER, xdC, ydC)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot11_LOWER, MLLim_D5Col)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.workSumPlotCheck(ref _uiObject.FormPlot11())));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotZoomout(ref _uiObject.FormPlot11(), 1.3)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.FormPlot11().Refresh()));
            }
            else
            {
                _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot11(), ref _uiObject.Plot11_MASTER, xdA, ydA);
                _uiObject.PlotChangeColor(ref _uiObject.Plot11_MASTER, MMaster_D5Col);
                _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot11(), ref _uiObject.Plot11_UPPER, xdB, ydB);
                _uiObject.PlotChangeColor(ref _uiObject.Plot11_UPPER, MHLim_D5Col);
                _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot11(), ref _uiObject.Plot11_LOWER, xdC, ydC);
                _uiObject.PlotChangeColor(ref _uiObject.Plot11_LOWER, MLLim_D5Col);
                _uiObject.workSumPlotCheck(ref _uiObject.FormPlot11());
                _uiObject.PlotZoomout(ref _uiObject.FormPlot11(), 1.3);
                _uiObject.FormPlot11().Refresh();
            }
        }
        void _uiPlot12MasterUpdate()
        {
            double[] xdA = new double[MMaster_dXD6.Length];
            Array.Copy(MMaster_dXD6, xdA, MMaster_dXD6.Length);
            double[] ydA = new double[MMaster_dYD6.Length];
            Array.Copy(MMaster_dYD6, ydA, MMaster_dYD6.Length);

            double[] xdB = new double[MUpper_dXD6.Length];
            Array.Copy(MUpper_dXD6, xdB, MUpper_dXD6.Length);
            double[] ydB = new double[MUpper_dYD6.Length];
            Array.Copy(MUpper_dYD6, ydB, MUpper_dYD6.Length);

            double[] xdC = new double[MLower_dXD6.Length];
            Array.Copy(MLower_dXD6, xdC, MLower_dXD6.Length);
            double[] ydC = new double[MLower_dYD6.Length];
            Array.Copy(MLower_dYD6, ydC, MLower_dYD6.Length);

            if (_uiObject.InvokeRequired)
            {
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot12(), ref _uiObject.Plot12_MASTER, xdA, ydA)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot12_MASTER, MMaster_D6Col)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot12(), ref _uiObject.Plot12_UPPER, xdB, ydB)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot12_UPPER, MHLim_D6Col)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot12(), ref _uiObject.Plot12_LOWER, xdC, ydC)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotChangeColor(ref _uiObject.Plot12_LOWER, MLLim_D6Col)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.workSumPlotCheck(ref _uiObject.FormPlot12())));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.PlotZoomout(ref _uiObject.FormPlot12(), 1.3)));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.FormPlot12().Refresh()));
            }
            else
            {
                _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot12(), ref _uiObject.Plot12_MASTER, xdA, ydA);
                _uiObject.PlotChangeColor(ref _uiObject.Plot12_MASTER, MMaster_D6Col);
                _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot12(), ref _uiObject.Plot12_UPPER, xdB, ydB);
                _uiObject.PlotChangeColor(ref _uiObject.Plot12_UPPER, MHLim_D6Col);
                _uiObject.PlotSignalPlotting(ref _uiObject.FormPlot12(), ref _uiObject.Plot12_LOWER, xdC, ydC);
                _uiObject.PlotChangeColor(ref _uiObject.Plot12_LOWER, MLLim_D6Col);
                _uiObject.workSumPlotCheck(ref _uiObject.FormPlot12());
                _uiObject.PlotZoomout(ref _uiObject.FormPlot12(), 1.3);
                _uiObject.FormPlot12().Refresh();
            }
        }
        void uiPlotRealMasterUpdate()
        {
            _uiPlot1MasterUpdate();
            _uiPlot2MasterUpdate();
            _uiPlot3MasterUpdate();
            _uiPlot4MasterUpdate();
            _uiPlot9MasterUpdate();
            _uiPlot10MasterUpdate();
        }
        void uiPlotLTeachMasterUpdate()
        {
            _uiPlot5MasterUpdate();
            _uiPlot6MasterUpdate();
            _uiPlot11MasterUpdate();
        }
        void uiPlotRTeachMasterUpdate()
        {
            _uiPlot7MasterUpdate();
            _uiPlot8MasterUpdate();
            _uiPlot12MasterUpdate();
        }

        void _backgroundDataPlot1Read()
        {
            if (this.GetConnState() == 1)
            {
                float[] fXD1 = _Ldata.RealtimeStep2[0].ToArray();
                float[] fYD1 = _Ldata.RealtimeStep2[1].ToArray();

                int idxx = 0;
                for (int i = 0; i < fXD1.Length; i++)
                {
                    if (fXD1[i] > 0 && i != 0)
                    {
                        Array.Resize(ref dXD1, idxx + 1);
                        if ((double)fXD1[i] == dXD1[idxx - 1])
                        {
                            dXD1[idxx] = (double)fXD1[i] + 1;
                        }
                        else
                        {
                            dXD1[idxx] = (double)fXD1[i];
                        }
                        idxx++;
                    }
                    else if (i == 0)
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

                if (dXD1.Length != dYD1.Length)
                {
                    _uiPlot1UpdateFlag = false;
                }
                else
                {
                    _uiPlot1UpdateFlag = true;
                }
            }
        }
        void _backgroundDataPlot2Read()
        {
            if (this.GetConnState() == 1)
            {
                float[] fXD2 = _Ldata.RealtimeStep2[2].ToArray();
                Array.Reverse(fXD2);
                float[] fYD2 = _Ldata.RealtimeStep2[3].ToArray();
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

                if (dXD2.Length != dYD2.Length)
                {
                    _uiPlot2UpdateFlag = false;
                }
                else
                {
                    _uiPlot2UpdateFlag = true;
                }
            }
        }
        void _backgroundDataPlot3Read()
        {
            if (this.GetConnState() == 1)
            {
                float[] fXD3 = _Rdata.RealtimeStep2[0].ToArray();
                float[] fYD3 = _Rdata.RealtimeStep2[1].ToArray();

                int idxx = 0;
                int idxy = 0;
                for (int i = 0; i < fXD3.Length; i++)
                {
                    if (fXD3[i] > 0 && i != 0)
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

                if (dXD3.Length != dYD3.Length)
                {
                    _uiPlot3UpdateFlag = false;
                }
                else
                {
                    _uiPlot3UpdateFlag = true;
                }
            }
        }
        void _backgroundDataPlot4Read()
        {
            if (this.GetConnState() == 1)
            {
                float[] fXD4 = _Rdata.RealtimeStep2[2].ToArray();
                Array.Reverse(fXD4);
                float[] fYD4 = _Rdata.RealtimeStep2[3].ToArray();
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

                if (dXD4.Length != dYD4.Length)
                {
                    _uiPlot4UpdateFlag = false;
                }
                else
                {
                    _uiPlot4UpdateFlag = true;
                }
            }
        }
        void _backgroundDataPlot9Read()
        {
            if (this.GetConnState() == 1)
            {
                float[] fXD5 = _Ldata.RealtimeStep2[4].ToArray();
                float[] fYD5 = _Ldata.RealtimeStep2[5].ToArray();

                int idxx = 0;
                for (int i = 0; i < fXD5.Length; i++)
                {
                    if ((fXD5[i] >= Convert.ToSingle(_data.DiffParam[0])) && (fXD5[i] <= Convert.ToSingle(_data.DiffParam[1])))
                    {
                        if (fXD5[i] > 0 && i != 0 && idxx != 0)
                        {
                            Array.Resize(ref dXD5, idxx + 1);
                            if ((double)fXD5[i] == dXD5[idxx - 1])
                            {
                                dXD5[idxx] = (double)fXD5[i] + 1;
                            }
                            else
                            {
                                dXD5[idxx] = (double)fXD5[i];
                            }
                            idxx++;
                        }
                        else if (i == 0 | idxx == 0)
                        {
                            Array.Resize(ref dXD5, idxx + 1);
                            dXD5[idxx] = (double)fXD5[i];
                            idxx++;
                        }
                    }
                }

                int idxy = 0;
                for (int i = 0; i < fXD5.Length; i++)
                {
                    if ((fXD5[i] >= Convert.ToSingle(_data.DiffParam[0])) && (fXD5[i] <= Convert.ToSingle(_data.DiffParam[1])))
                    {
                        Array.Resize(ref dYD5, idxy + 1);
                        dYD5[idxy] = (double)fYD5[i];
                        idxy++;
                    }
                }

                //dXD1 = Array.ConvertAll(fXD1, x => (x != 0) ? (double)x);
                //dYD1 = Array.ConvertAll(fYD1, x => (x != 0) ? (double)x);

                if (dXD5.Length != dYD5.Length)
                {
                    _uiPlot9UpdateFlag = false;
                }
                else
                {
                    _uiPlot9UpdateFlag = true;
                }
            }
        }
        void _backgroundDataPlot10Read()
        {
            if (this.GetConnState() == 1)
            {
                float[] fXD6 = _Rdata.RealtimeStep2[4].ToArray();
                float[] fYD6 = _Rdata.RealtimeStep2[5].ToArray();

                int idxx = 0;
                for (int i = 0; i < fXD6.Length; i++)
                {
                    if ((fXD6[i] >= Convert.ToSingle(_data.DiffParam[0])) && (fXD6[i] <= Convert.ToSingle(_data.DiffParam[1])))
                    {
                        if (fXD6[i] > 0 && i != 0 && idxx != 0)
                        {
                            Array.Resize(ref dXD6, idxx + 1);
                            if ((double)fXD6[i] == dXD6[idxx - 1])
                            {
                                dXD6[idxx] = (double)fXD6[i] + 1;
                            }
                            else
                            {
                                dXD6[idxx] = (double)fXD6[i];
                            }
                            idxx++;
                        }
                        else if (i == 0 | idxx == 0)
                        {
                            Array.Resize(ref dXD6, idxx + 1);
                            dXD6[idxx] = (double)fXD6[i];
                            idxx++;
                        }
                    }
                }

                int idxy = 0;
                for (int i = 0; i < fXD6.Length; i++)
                {
                    if ((fXD6[i] >= Convert.ToSingle(_data.DiffParam[0])) && (fXD6[i] <= Convert.ToSingle(_data.DiffParam[1])))
                    {
                        Array.Resize(ref dYD6, idxy + 1);
                        dYD6[idxy] = (double)fYD6[i];
                        idxy++;
                    }
                }

                //dXD1 = Array.ConvertAll(fXD1, x => (x != 0) ? (double)x);
                //dYD1 = Array.ConvertAll(fYD1, x => (x != 0) ? (double)x);

                if (dXD6.Length != dYD6.Length)
                {
                    _uiPlot10UpdateFlag = false;
                }
                else
                {
                    _uiPlot10UpdateFlag = true;
                }
            }
        }
        void _DataPlot5Read()
        {
            float[] fXD = _TMaster.LMasteringTeachStep2[0].ToArray();
            float[] fYD = _TMaster.LMasteringTeachStep2[1].ToArray();

            int idxx = 0;
            for (int i = 0; i < fXD.Length; i++)
            {
                if (fXD[i] > 0 && i != 0)
                {
                    Array.Resize(ref MTeach_dXD1, idxx + 1);
                    if ((double)fXD[i] == MTeach_dXD1[idxx - 1])
                    {
                        MTeach_dXD1[idxx] = (double)fXD[i] + 1;
                    }
                    else
                    {
                        MTeach_dXD1[idxx] = (double)fXD[i];
                    }
                    idxx++;
                }
                else if (i == 0)
                {
                    Array.Resize(ref MTeach_dXD1, idxx + 1);
                    MTeach_dXD1[idxx] = (double)fXD[i];
                    idxx++;
                }
            }

            int idxy = 0;
            for (int i = 0; i < MTeach_dXD1.Length; i++)
            {
                Array.Resize(ref MTeach_dYD1, idxy + 1);
                MTeach_dYD1[idxy] = (double)fYD[i];
                idxy++;
            }

            //MTeach_dXD1 = Array.ConvertAll(fXD, x => (x != 0) ? (double)x);
            //MTeach_dYD1 = Array.ConvertAll(fYD, x => (x != 0) ? (double)x);

            if (MTeach_dXD1.Length != MTeach_dYD1.Length)
            {
                _uiPlot5UpdateFlag = false;
            }
            else
            {
                _uiPlot5UpdateFlag = true;
            }

        }
        void _DataPlot6Read()
        {
            float[] fXD = _TMaster.LMasteringTeachStep2[2].ToArray();
            Array.Reverse(fXD);
            float[] fYD = _TMaster.LMasteringTeachStep2[3].ToArray();
            Array.Reverse(fYD);

            MTeach_dXD2 = Array.ConvertAll(fXD, x => (double)x);
            MTeach_dYD2 = Array.ConvertAll(fYD, x => (double)x);

            if (MTeach_dXD2.Length != MTeach_dYD2.Length)
            {
                _uiPlot6UpdateFlag = false;
            }
            else
            {
                _uiPlot6UpdateFlag = true;
            }
        }
        void _DataPlot7Read()
        {
            float[] fXD = _TMaster.RMasteringTeachStep2[0].ToArray();
            float[] fYD = _TMaster.RMasteringTeachStep2[1].ToArray();

            int idxx = 0;
            int idxy = 0;
            for (int i = 0; i < fXD.Length; i++)
            {
                if (fXD[i] > 0 && i != 0)
                {
                    Array.Resize(ref MTeach_dXD3, idxx + 1);
                    if ((double)fXD[i] == MTeach_dXD3[idxx - 1])
                    {
                        MTeach_dXD3[idxx] = (double)fXD[i] + 1;
                    }
                    else
                    {
                        MTeach_dXD3[idxx] = (double)fXD[i];
                    }
                    idxx++;
                }
                else if (i == 0)
                {
                    Array.Resize(ref MTeach_dXD3, idxx + 1);
                    MTeach_dXD3[idxx] = (double)fXD[i];
                    idxx++;
                }
            }

            for (int i = 0; i < MTeach_dXD3.Length; i++)
            {
                Array.Resize(ref MTeach_dYD3, idxy + 1);
                MTeach_dYD3[idxy] = (double)fYD[i];
                idxy++;
            }

            //MTeach_dXD3 = Array.ConvertAll(fXD, x => (x != 0) ? (double)x);
            //MTeach_MTeach_dYD3 = Array.ConvertAll(fYD, x => (x != 0) ? (double)x);

            if (MTeach_dXD3.Length != MTeach_dYD3.Length)
            {
                _uiPlot7UpdateFlag = false;
            }
            else
            {
                _uiPlot7UpdateFlag = true;
            }
        }
        void _DataPlot8Read()
        {
            float[] fXD = _TMaster.RMasteringTeachStep2[2].ToArray();
            Array.Reverse(fXD);
            float[] fYD = _TMaster.RMasteringTeachStep2[3].ToArray();
            Array.Reverse(fYD);

            MTeach_dXD4 = Array.ConvertAll(fXD, x => (double)x);
            MTeach_dYD4 = Array.ConvertAll(fYD, x => (double)x);

            if (MTeach_dXD4.Length != MTeach_dYD4.Length)
            {
                _uiPlot8UpdateFlag = false;
            }
            else
            {
                _uiPlot8UpdateFlag = true;
            }
        }
        void _DataPlot11Read()
        {
            float[] fXD5 = _TMaster.LMasteringTeachStep2[4].ToArray();
            float[] fYD5 = _TMaster.LMasteringTeachStep2[5].ToArray();

            int idxx = 0;
            for (int i = 0; i < fXD5.Length; i++)
            {
                if ((fXD5[i] >= Convert.ToSingle(_masterData.DiffParam[0])) && (fXD5[i] <= Convert.ToSingle(_masterData.DiffParam[1])))
                {
                    if (fXD5[i] > 0 && i != 0 && idxx != 0)
                    {
                        Array.Resize(ref MTeach_dXD5, idxx + 1);
                        if ((double)fXD5[i] == MTeach_dXD5[idxx - 1])
                        {
                            MTeach_dXD5[idxx] = (double)fXD5[i] + 1;
                        }
                        else
                        {
                            MTeach_dXD5[idxx] = (double)fXD5[i];
                        }
                        idxx++;
                    }
                    else if (i == 0 | idxx == 0)
                    {
                        Array.Resize(ref MTeach_dXD5, idxx + 1);
                        MTeach_dXD5[idxx] = (double)fXD5[i];
                        idxx++;
                    }
                }
            }

            int idxy = 0;
            for (int i = 0; i < fXD5.Length; i++)
            {
                if ((fXD5[i] >= Convert.ToSingle(_masterData.DiffParam[0])) && (fXD5[i] <= Convert.ToSingle(_masterData.DiffParam[1])))
                {
                    Array.Resize(ref MTeach_dYD5, idxy + 1);
                    MTeach_dYD5[idxy] = (double)fYD5[i];
                    idxy++;
                }
            }

            //MTeach_dXD1 = Array.ConvertAll(fXD, x => (x != 0) ? (double)x);
            //MTeach_dYD1 = Array.ConvertAll(fYD, x => (x != 0) ? (double)x);

            if (MTeach_dXD5.Length != MTeach_dYD5.Length)
            {
                _uiPlot11UpdateFlag = false;
            }
            else
            {
                _uiPlot11UpdateFlag = true;
            }
        }
        void _DataPlot12Read()
        {
            float[] fXD6 = _TMaster.RMasteringTeachStep2[4].ToArray();
            float[] fYD6 = _TMaster.RMasteringTeachStep2[5].ToArray();

            int idxx = 0;
            for (int i = 0; i < fXD6.Length; i++)
            {
                if ((fXD6[i] >= Convert.ToSingle(_masterData.DiffParam[0])) && (fXD6[i] <= Convert.ToSingle(_masterData.DiffParam[1])))
                {
                    if (fXD6[i] > 0 && i != 0 && idxx != 0)
                    {
                        Array.Resize(ref MTeach_dXD6, idxx + 1);
                        if ((double)fXD6[i] == MTeach_dXD6[idxx - 1])
                        {
                            MTeach_dXD6[idxx] = (double)fXD6[i] + 1;
                        }
                        else
                        {
                            MTeach_dXD6[idxx] = (double)fXD6[i];
                        }
                        idxx++;
                    }
                    else if (i == 0 | idxx == 0)
                    {
                        Array.Resize(ref MTeach_dXD6, idxx + 1);
                        MTeach_dXD6[idxx] = (double)fXD6[i];
                        idxx++;
                    }
                }
            }

            int idxy = 0;
            for (int i = 0; i < fXD6.Length; i++)
            {
                if ((fXD6[i] >= Convert.ToSingle(_masterData.DiffParam[0])) && (fXD6[i] <= Convert.ToSingle(_masterData.DiffParam[1])))
                {
                    Array.Resize(ref MTeach_dYD6, idxy + 1);
                    MTeach_dYD6[idxy] = (double)fYD6[i];
                    idxy++;
                }
            }

            //MTeach_dXD1 = Array.ConvertAll(fXD, x => (x != 0) ? (double)x);
            //MTeach_dYD1 = Array.ConvertAll(fYD, x => (x != 0) ? (double)x);

            if (MTeach_dXD6.Length != MTeach_dYD6.Length)
            {
                _uiPlot12UpdateFlag = false;
            }
            else
            {
                _uiPlot12UpdateFlag = true;
            }
        }
        void DataPlotLTeachRead()
        {
            if (DataLMasterTeachIsExist())
            {
                _DataPlot5Read();
                _DataPlot6Read();
                _DataPlot11Read();
            }
        }
        void DataPlotRTeachRead()
        {
            if (DataRMasterTeachIsExist())
            {
                _DataPlot7Read();
                _DataPlot8Read();
                _DataPlot12Read();
            }
        }

        void MasterDataAssignPlot1()
        {
            float[] fXD = _masterData.LMasteringStep2[0].ToArray();
            float[] fYD1 = _masterData.LMasteringStep2[1].ToArray();
            float[] fYD2 = _masterData.LMasteringStep2[2].ToArray();
            float[] fYD3 = _masterData.LMasteringStep2[3].ToArray();

            int idxx = 0;
            for (int i = 0; i < fXD.Length; i++)
            {
                if (fXD[i] > 0 && i != 0)
                {
                    Array.Resize(ref Master_dXD1, idxx + 1);
                    Array.Resize(ref Upper_dXD1, idxx + 1);
                    Array.Resize(ref Lower_dXD1, idxx + 1);
                    if (((double)fXD[i] == Master_dXD1[idxx - 1]) && ((double)fXD[i] == Upper_dXD1[idxx - 1]) && ((double)fXD[i] == Lower_dXD1[idxx - 1]))
                    {
                        Master_dXD1[idxx] = (double)fXD[i] + 1;
                        Upper_dXD1[idxx] = (double)fXD[i] + 1;
                        Lower_dXD1[idxx] = (double)fXD[i] + 1;
                    }
                    else
                    {
                        Master_dXD1[idxx] = (double)fXD[i];
                        Upper_dXD1[idxx] = (double)fXD[i];
                        Lower_dXD1[idxx] = (double)fXD[i];
                    }
                    idxx++;
                }
                else if (i == 0)
                {
                    Array.Resize(ref Master_dXD1, idxx + 1);
                    Array.Resize(ref Upper_dXD1, idxx + 1);
                    Array.Resize(ref Lower_dXD1, idxx + 1);
                    Master_dXD1[idxx] = (double)fXD[i];
                    Upper_dXD1[idxx] = (double)fXD[i];
                    Lower_dXD1[idxx] = (double)fXD[i];
                    idxx++;
                }
            }

            int idxy = 0;
            for (int i = 0; i < Master_dXD1.Length; i++)
            {
                Array.Resize(ref Master_dYD1, idxy + 1);
                Array.Resize(ref Upper_dYD1, idxy + 1);
                Array.Resize(ref Lower_dYD1, idxy + 1);
                Master_dYD1[idxy] = (double)fYD1[i];
                Upper_dYD1[idxy] = (double)fYD2[i];
                Lower_dYD1[idxy] = (double)fYD3[i];
                idxy++;
            }
        }
        void MasterDataAssignPlot2()
        {
            float[] fXD = _masterData.LMasteringStep2[4].ToArray();
            Array.Reverse(fXD);
            float[] fYD1 = _masterData.LMasteringStep2[5].ToArray();
            Array.Reverse(fYD1);
            float[] fYD2 = _masterData.LMasteringStep2[6].ToArray();
            Array.Reverse(fYD2);
            float[] fYD3 = _masterData.LMasteringStep2[7].ToArray();
            Array.Reverse(fYD3);

            Master_dXD2 = Array.ConvertAll(fXD, x => (double)x);
            Upper_dXD2 = Array.ConvertAll(fXD, x => (double)x);
            Lower_dXD2 = Array.ConvertAll(fXD, x => (double)x);

            Master_dYD2 = Array.ConvertAll(fYD1, x => (double)x);
            Upper_dYD2 = Array.ConvertAll(fYD2, x => (double)x);
            Lower_dYD2 = Array.ConvertAll(fYD3, x => (double)x);
        }
        void MasterDataAssignPlot3()
        {
            float[] fXD = _masterData.RMasteringStep2[0].ToArray();
            float[] fYD1 = _masterData.RMasteringStep2[1].ToArray();
            float[] fYD2 = _masterData.RMasteringStep2[2].ToArray();
            float[] fYD3 = _masterData.RMasteringStep2[3].ToArray();

            int idxx = 0;
            for (int i = 0; i < fXD.Length; i++)
            {
                if (fXD[i] > 0 && i != 0)
                {
                    Array.Resize(ref Master_dXD3, idxx + 1);
                    Array.Resize(ref Upper_dXD3, idxx + 1);
                    Array.Resize(ref Lower_dXD3, idxx + 1);
                    if (((double)fXD[i] == Master_dXD3[idxx - 1]) && ((double)fXD[i] == Upper_dXD3[idxx - 1]) && ((double)fXD[i] == Lower_dXD3[idxx - 1]))
                    {
                        Master_dXD3[idxx] = (double)fXD[i] + 1;
                        Upper_dXD3[idxx] = (double)fXD[i] + 1;
                        Lower_dXD3[idxx] = (double)fXD[i] + 1;
                    }
                    else
                    {
                        Master_dXD3[idxx] = (double)fXD[i];
                        Upper_dXD3[idxx] = (double)fXD[i];
                        Lower_dXD3[idxx] = (double)fXD[i];
                    }
                    idxx++;
                }
                else if (i == 0)
                {
                    Array.Resize(ref Master_dXD3, idxx + 1);
                    Array.Resize(ref Upper_dXD3, idxx + 1);
                    Array.Resize(ref Lower_dXD3, idxx + 1);
                    Master_dXD3[idxx] = (double)fXD[i];
                    Upper_dXD3[idxx] = (double)fXD[i];
                    Lower_dXD3[idxx] = (double)fXD[i];
                    idxx++;
                }
            }

            int idxy = 0;
            for (int i = 0; i < Master_dXD3.Length; i++)
            {
                Array.Resize(ref Master_dYD3, idxy + 1);
                Array.Resize(ref Upper_dYD3, idxy + 1);
                Array.Resize(ref Lower_dYD3, idxy + 1);
                Master_dYD3[idxy] = (double)fYD1[i];
                Upper_dYD3[idxy] = (double)fYD2[i];
                Lower_dYD3[idxy] = (double)fYD3[i];
                idxy++;
            }
        }
        void MasterDataAssignPlot4()
        {
            float[] fXD = _masterData.RMasteringStep2[4].ToArray();
            Array.Reverse(fXD);
            float[] fYD1 = _masterData.RMasteringStep2[5].ToArray();
            Array.Reverse(fYD1);
            float[] fYD2 = _masterData.RMasteringStep2[6].ToArray();
            Array.Reverse(fYD2);
            float[] fYD3 = _masterData.RMasteringStep2[7].ToArray();
            Array.Reverse(fYD3);

            Master_dXD4 = Array.ConvertAll(fXD, x => (double)x);
            Upper_dXD4 = Array.ConvertAll(fXD, x => (double)x);
            Lower_dXD4 = Array.ConvertAll(fXD, x => (double)x);

            Master_dYD4 = Array.ConvertAll(fYD1, x => (double)x);
            Upper_dYD4 = Array.ConvertAll(fYD2, x => (double)x);
            Lower_dYD4 = Array.ConvertAll(fYD3, x => (double)x);
        }
        void MasterDataAssignPlot9()
        {
            float[] fXD = _masterData.LMasteringStep2[8].ToArray();
            float[] fYD1 = _masterData.LMasteringStep2[9].ToArray();
            float[] fYD2 = _masterData.LMasteringStep2[10].ToArray();
            float[] fYD3 = _masterData.LMasteringStep2[11].ToArray();

            int idxx = 0;
            for (int i = 0; i < fXD.Length; i++)
            {
                if ((fXD[i] >= Convert.ToSingle(_masterData.DiffParam[0])) && (fXD[i] <= Convert.ToSingle(_masterData.DiffParam[1])))
                {
                    if (fXD[i] > 0 && i != 0 && idxx != 0)
                    {
                        Array.Resize(ref Master_dXD5, idxx + 1);
                        Array.Resize(ref Upper_dXD5, idxx + 1);
                        Array.Resize(ref Lower_dXD5, idxx + 1);
                        if (((double)fXD[i] == Master_dXD5[idxx - 1]) && ((double)fXD[i] == Upper_dXD5[idxx - 1]) && ((double)fXD[i] == Lower_dXD5[idxx - 1]))
                        {
                            Master_dXD5[idxx] = (double)fXD[i] + 1;
                            Upper_dXD5[idxx] = (double)fXD[i] + 1;
                            Lower_dXD5[idxx] = (double)fXD[i] + 1;
                        }
                        else
                        {
                            Master_dXD5[idxx] = (double)fXD[i];
                            Upper_dXD5[idxx] = (double)fXD[i];
                            Lower_dXD5[idxx] = (double)fXD[i];
                        }
                        idxx++;
                    }
                    else if (i == 0 | idxx == 0)
                    {
                        Array.Resize(ref Master_dXD5, idxx + 1);
                        Array.Resize(ref Upper_dXD5, idxx + 1);
                        Array.Resize(ref Lower_dXD5, idxx + 1);
                        Master_dXD5[idxx] = (double)fXD[i];
                        Upper_dXD5[idxx] = (double)fXD[i];
                        Lower_dXD5[idxx] = (double)fXD[i];
                        idxx++;
                    }
                }
            }

            int idxy = 0;
            for (int i = 0; i < fXD.Length; i++)
            {
                if ((fXD[i] >= Convert.ToSingle(_masterData.DiffParam[0])) && (fXD[i] <= Convert.ToSingle(_masterData.DiffParam[1])))
                {
                    Array.Resize(ref Master_dYD5, idxy + 1);
                    Array.Resize(ref Upper_dYD5, idxy + 1);
                    Array.Resize(ref Lower_dYD5, idxy + 1);
                    Master_dYD5[idxy] = (double)fYD1[i];
                    Upper_dYD5[idxy] = (double)fYD2[i];
                    Lower_dYD5[idxy] = (double)fYD3[i];
                    idxy++;
                }
            }
        }
        void MasterDataAssignPlot10()
        {
            float[] fXD = _masterData.RMasteringStep2[8].ToArray();
            float[] fYD1 = _masterData.RMasteringStep2[9].ToArray();
            float[] fYD2 = _masterData.RMasteringStep2[10].ToArray();
            float[] fYD3 = _masterData.RMasteringStep2[11].ToArray();

            int idxx = 0;
            for (int i = 0; i < fXD.Length; i++)
            {
                if ((fXD[i] >= Convert.ToSingle(_masterData.DiffParam[0])) && (fXD[i] <= Convert.ToSingle(_masterData.DiffParam[1])))
                {
                    if (fXD[i] > 0 && i != 0 && idxx != 0)
                    {
                        Array.Resize(ref Master_dXD6, idxx + 1);
                        Array.Resize(ref Upper_dXD6, idxx + 1);
                        Array.Resize(ref Lower_dXD6, idxx + 1);
                        if (((double)fXD[i] == Master_dXD6[idxx - 1]) && ((double)fXD[i] == Upper_dXD6[idxx - 1]) && ((double)fXD[i] == Lower_dXD6[idxx - 1]))
                        {
                            Master_dXD6[idxx] = (double)fXD[i] + 1;
                            Upper_dXD6[idxx] = (double)fXD[i] + 1;
                            Lower_dXD6[idxx] = (double)fXD[i] + 1;
                        }
                        else
                        {
                            Master_dXD6[idxx] = (double)fXD[i];
                            Upper_dXD6[idxx] = (double)fXD[i];
                            Lower_dXD6[idxx] = (double)fXD[i];
                        }
                        idxx++;
                    }
                    else if (i == 0 | idxx == 0)
                    {
                        Array.Resize(ref Master_dXD6, idxx + 1);
                        Array.Resize(ref Upper_dXD6, idxx + 1);
                        Array.Resize(ref Lower_dXD6, idxx + 1);
                        Master_dXD6[idxx] = (double)fXD[i];
                        Upper_dXD6[idxx] = (double)fXD[i];
                        Lower_dXD6[idxx] = (double)fXD[i];
                        idxx++;
                    }
                }
            }

            int idxy = 0;
            for (int i = 0; i < fXD.Length; i++)
            {
                if ((fXD[i] >= Convert.ToSingle(_masterData.DiffParam[0])) && (fXD[i] <= Convert.ToSingle(_masterData.DiffParam[1])))
                {
                    Array.Resize(ref Master_dYD6, idxy + 1);
                    Array.Resize(ref Upper_dYD6, idxy + 1);
                    Array.Resize(ref Lower_dYD6, idxy + 1);
                    Master_dYD6[idxy] = (double)fYD1[i];
                    Upper_dYD6[idxy] = (double)fYD2[i];
                    Lower_dYD6[idxy] = (double)fYD3[i];
                    idxy++;
                }
            }
        }
        void MasterDataAssignPlot5()
        {
            float[] fXD = _masterData.LMasteringStep2[0].ToArray();
            float[] fYD1 = _masterData.LMasteringStep2[1].ToArray();
            float[] fYD2 = _masterData.LMasteringStep2[2].ToArray();
            float[] fYD3 = _masterData.LMasteringStep2[3].ToArray();

            int idxx = 0;
            for (int i = 0; i < fXD.Length; i++)
            {
                if (fXD[i] > 0 && i != 0)
                {
                    Array.Resize(ref MMaster_dXD1, idxx + 1);
                    Array.Resize(ref MUpper_dXD1, idxx + 1);
                    Array.Resize(ref MLower_dXD1, idxx + 1);
                    if (((double)fXD[i] == MMaster_dXD1[idxx - 1]) && ((double)fXD[i] == MUpper_dXD1[idxx - 1]) && ((double)fXD[i] == MLower_dXD1[idxx - 1]))
                    {
                        MMaster_dXD1[idxx] = (double)fXD[i] + 1;
                        MUpper_dXD1[idxx] = (double)fXD[i] + 1;
                        MLower_dXD1[idxx] = (double)fXD[i] + 1;
                    }
                    else
                    {
                        MMaster_dXD1[idxx] = (double)fXD[i];
                        MUpper_dXD1[idxx] = (double)fXD[i];
                        MLower_dXD1[idxx] = (double)fXD[i];
                    }
                    idxx++;
                }
                else if (i == 0)
                {
                    Array.Resize(ref MMaster_dXD1, idxx + 1);
                    Array.Resize(ref MUpper_dXD1, idxx + 1);
                    Array.Resize(ref MLower_dXD1, idxx + 1);
                    MMaster_dXD1[idxx] = (double)fXD[i];
                    MUpper_dXD1[idxx] = (double)fXD[i];
                    MLower_dXD1[idxx] = (double)fXD[i];
                    idxx++;
                }
            }

            int idxy = 0;
            for (int i = 0; i < MMaster_dXD1.Length; i++)
            {
                Array.Resize(ref MMaster_dYD1, idxy + 1);
                Array.Resize(ref MUpper_dYD1, idxy + 1);
                Array.Resize(ref MLower_dYD1, idxy + 1);
                MMaster_dYD1[idxy] = (double)fYD1[i];
                MUpper_dYD1[idxy] = (double)fYD2[i];
                MLower_dYD1[idxy] = (double)fYD3[i];
                idxy++;
            }

            if ((MMaster_dXD1.Length != MMaster_dYD1.Length) || (MLower_dXD1.Length != MLower_dYD1.Length) || (MUpper_dXD1.Length != MUpper_dYD1.Length))
            {
                _uiPlot5UpdateFlag = false;
            }
            else
            {
                _uiPlot5UpdateFlag = true;
            }
        }
        void MasterDataAssignPlot6()
        {
            float[] fXD = _masterData.LMasteringStep2[4].ToArray();
            Array.Reverse(fXD);
            float[] fYD1 = _masterData.LMasteringStep2[5].ToArray();
            Array.Reverse(fYD1);
            float[] fYD2 = _masterData.LMasteringStep2[6].ToArray();
            Array.Reverse(fYD2);
            float[] fYD3 = _masterData.LMasteringStep2[7].ToArray();
            Array.Reverse(fYD3);

            MMaster_dXD2 = Array.ConvertAll(fXD, x => (double)x);
            MUpper_dXD2 = Array.ConvertAll(fXD, x => (double)x);
            MLower_dXD2 = Array.ConvertAll(fXD, x => (double)x);

            MMaster_dYD2 = Array.ConvertAll(fYD1, x => (double)x);
            MUpper_dYD2 = Array.ConvertAll(fYD2, x => (double)x);
            MLower_dYD2 = Array.ConvertAll(fYD3, x => (double)x);

            if ((MMaster_dXD2.Length != MMaster_dYD2.Length) || (MUpper_dXD2.Length != MUpper_dYD2.Length) || (MLower_dXD2.Length != MLower_dYD2.Length))
            {
                _uiPlot6UpdateFlag = false;
            }
            else
            {
                _uiPlot6UpdateFlag = true;
            }
        }
        void MasterDataAssignPlot7()
        {
            float[] fXD = _masterData.RMasteringStep2[0].ToArray();
            float[] fYD1 = _masterData.RMasteringStep2[1].ToArray();
            float[] fYD2 = _masterData.RMasteringStep2[2].ToArray();
            float[] fYD3 = _masterData.RMasteringStep2[3].ToArray();

            int idxx = 0;
            for (int i = 0; i < fXD.Length; i++)
            {
                if (fXD[i] > 0 && i != 0)
                {
                    Array.Resize(ref MMaster_dXD3, idxx + 1);
                    Array.Resize(ref MUpper_dXD3, idxx + 1);
                    Array.Resize(ref MLower_dXD3, idxx + 1);
                    if (((double)fXD[i] == MMaster_dXD3[idxx - 1]) && ((double)fXD[i] == MUpper_dXD3[idxx - 1]) && ((double)fXD[i] == MLower_dXD3[idxx - 1]))
                    {
                        MMaster_dXD3[idxx] = (double)fXD[i] + 1;
                        MUpper_dXD3[idxx] = (double)fXD[i] + 1;
                        MLower_dXD3[idxx] = (double)fXD[i] + 1;
                    }
                    else
                    {
                        MMaster_dXD3[idxx] = (double)fXD[i];
                        MUpper_dXD3[idxx] = (double)fXD[i];
                        MLower_dXD3[idxx] = (double)fXD[i];
                    }
                    idxx++;
                }
                else if (i == 0)
                {
                    Array.Resize(ref MMaster_dXD3, idxx + 1);
                    Array.Resize(ref MUpper_dXD3, idxx + 1);
                    Array.Resize(ref MLower_dXD3, idxx + 1);
                    MMaster_dXD3[idxx] = (double)fXD[i];
                    MUpper_dXD3[idxx] = (double)fXD[i];
                    MLower_dXD3[idxx] = (double)fXD[i];
                    idxx++;
                }
            }

            int idxy = 0;
            for (int i = 0; i < MMaster_dXD3.Length; i++)
            {
                Array.Resize(ref MMaster_dYD3, idxy + 1);
                Array.Resize(ref MUpper_dYD3, idxy + 1);
                Array.Resize(ref MLower_dYD3, idxy + 1);
                MMaster_dYD3[idxy] = (double)fYD1[i];
                MUpper_dYD3[idxy] = (double)fYD2[i];
                MLower_dYD3[idxy] = (double)fYD3[i];
                idxy++;
            }

            if ((MMaster_dXD3.Length != MMaster_dYD3.Length) || (MLower_dXD3.Length != MLower_dYD3.Length) || (MUpper_dXD3.Length != MUpper_dYD3.Length))
            {
                _uiPlot7UpdateFlag = false;
            }
            else
            {
                _uiPlot7UpdateFlag = true;
            }
        }
        void MasterDataAssignPlot8()
        {
            float[] fXD = _masterData.RMasteringStep2[4].ToArray();
            Array.Reverse(fXD);
            float[] fYD1 = _masterData.RMasteringStep2[5].ToArray();
            Array.Reverse(fYD1);
            float[] fYD2 = _masterData.RMasteringStep2[6].ToArray();
            Array.Reverse(fYD2);
            float[] fYD3 = _masterData.RMasteringStep2[7].ToArray();
            Array.Reverse(fYD3);

            MMaster_dXD4 = Array.ConvertAll(fXD, x => (double)x);
            MUpper_dXD4 = Array.ConvertAll(fXD, x => (double)x);
            MLower_dXD4 = Array.ConvertAll(fXD, x => (double)x);

            MMaster_dYD4 = Array.ConvertAll(fYD1, x => (double)x);
            MUpper_dYD4 = Array.ConvertAll(fYD2, x => (double)x);
            MLower_dYD4 = Array.ConvertAll(fYD3, x => (double)x);

            if ((MMaster_dXD4.Length != MMaster_dYD4.Length) || (MUpper_dXD4.Length != MUpper_dYD4.Length) || (MLower_dXD4.Length != MLower_dYD4.Length))
            {
                _uiPlot8UpdateFlag = false;
            }
            else
            {
                _uiPlot8UpdateFlag = true;
            }
        }
        void MasterDataAssignPlot11()
        {
            float[] fXD = _masterData.LMasteringStep2[8].ToArray();
            float[] fYD1 = _masterData.LMasteringStep2[9].ToArray();
            float[] fYD2 = _masterData.LMasteringStep2[10].ToArray();
            float[] fYD3 = _masterData.LMasteringStep2[11].ToArray();

            int idxx = 0;
            for (int i = 0; i < fXD.Length; i++)
            {
                if ((fXD[i] >= Convert.ToSingle(_masterData.DiffParam[0])) && (fXD[i] <= Convert.ToSingle(_masterData.DiffParam[1])))
                {
                    if (fXD[i] > 0 && i != 0 && idxx != 0)
                    {
                        Array.Resize(ref MMaster_dXD5, idxx + 1);
                        Array.Resize(ref MUpper_dXD5, idxx + 1);
                        Array.Resize(ref MLower_dXD5, idxx + 1);
                        if (((double)fXD[i] == MMaster_dXD5[idxx - 1]) && ((double)fXD[i] == MUpper_dXD5[idxx - 1]) && ((double)fXD[i] == MLower_dXD5[idxx - 1]))
                        {
                            MMaster_dXD5[idxx] = (double)fXD[i] + 1;
                            MUpper_dXD5[idxx] = (double)fXD[i] + 1;
                            MLower_dXD5[idxx] = (double)fXD[i] + 1;
                        }
                        else
                        {
                            MMaster_dXD5[idxx] = (double)fXD[i];
                            MUpper_dXD5[idxx] = (double)fXD[i];
                            MLower_dXD5[idxx] = (double)fXD[i];
                        }
                        idxx++;
                    }
                    else if (i == 0 | idxx == 0)
                    {
                        Array.Resize(ref MMaster_dXD5, idxx + 1);
                        Array.Resize(ref MUpper_dXD5, idxx + 1);
                        Array.Resize(ref MLower_dXD5, idxx + 1);
                        MMaster_dXD5[idxx] = (double)fXD[i];
                        MUpper_dXD5[idxx] = (double)fXD[i];
                        MLower_dXD5[idxx] = (double)fXD[i];
                        idxx++;
                    }
                }
            }

            int idxy = 0;
            for (int i = 0; i < fXD.Length; i++)
            {
                if ((fXD[i] >= Convert.ToSingle(_masterData.DiffParam[0])) && (fXD[i] <= Convert.ToSingle(_masterData.DiffParam[1])))
                {
                    Array.Resize(ref MMaster_dYD5, idxy + 1);
                    Array.Resize(ref MUpper_dYD5, idxy + 1);
                    Array.Resize(ref MLower_dYD5, idxy + 1);
                    MMaster_dYD5[idxy] = (double)fYD1[i];
                    MUpper_dYD5[idxy] = (double)fYD2[i];
                    MLower_dYD5[idxy] = (double)fYD3[i];
                    idxy++;
                }
            }

            if ((MMaster_dXD5.Length != MMaster_dYD5.Length) || (MLower_dXD5.Length != MLower_dYD5.Length) || (MUpper_dXD5.Length != MUpper_dYD5.Length))
            {
                _uiPlot11UpdateFlag = false;
            }
            else
            {
                _uiPlot11UpdateFlag = true;
            }
        }
        void MasterDataAssignPlot12()
        {
            float[] fXD = _masterData.RMasteringStep2[8].ToArray();
            float[] fYD1 = _masterData.RMasteringStep2[9].ToArray();
            float[] fYD2 = _masterData.RMasteringStep2[10].ToArray();
            float[] fYD3 = _masterData.RMasteringStep2[11].ToArray();

            int idxx = 0;
            for (int i = 0; i < fXD.Length; i++)
            {
                if ((fXD[i] >= Convert.ToSingle(_masterData.DiffParam[0])) && (fXD[i] <= Convert.ToSingle(_masterData.DiffParam[1])))
                {
                    if (fXD[i] > 0 && i != 0 && idxx != 0)
                    {
                        Array.Resize(ref MMaster_dXD6, idxx + 1);
                        Array.Resize(ref MUpper_dXD6, idxx + 1);
                        Array.Resize(ref MLower_dXD6, idxx + 1);
                        if (((double)fXD[i] == MMaster_dXD6[idxx - 1]) && ((double)fXD[i] == MUpper_dXD6[idxx - 1]) && ((double)fXD[i] == MLower_dXD6[idxx - 1]))
                        {
                            MMaster_dXD6[idxx] = (double)fXD[i] + 1;
                            MUpper_dXD6[idxx] = (double)fXD[i] + 1;
                            MLower_dXD6[idxx] = (double)fXD[i] + 1;
                        }
                        else
                        {
                            MMaster_dXD6[idxx] = (double)fXD[i];
                            MUpper_dXD6[idxx] = (double)fXD[i];
                            MLower_dXD6[idxx] = (double)fXD[i];
                        }
                        idxx++;
                    }
                    else if (i == 0 | idxx == 0)
                    {
                        Array.Resize(ref MMaster_dXD6, idxx + 1);
                        Array.Resize(ref MUpper_dXD6, idxx + 1);
                        Array.Resize(ref MLower_dXD6, idxx + 1);
                        MMaster_dXD6[idxx] = (double)fXD[i];
                        MUpper_dXD6[idxx] = (double)fXD[i];
                        MLower_dXD6[idxx] = (double)fXD[i];
                        idxx++;
                    }
                }
            }

            int idxy = 0;
            for (int i = 0; i < fXD.Length; i++)
            {
                if ((fXD[i] >= Convert.ToSingle(_masterData.DiffParam[0])) && (fXD[i] <= Convert.ToSingle(_masterData.DiffParam[1])))
                {
                    Array.Resize(ref MMaster_dYD6, idxy + 1);
                    Array.Resize(ref MUpper_dYD6, idxy + 1);
                    Array.Resize(ref MLower_dYD6, idxy + 1);
                    MMaster_dYD6[idxy] = (double)fYD1[i];
                    MUpper_dYD6[idxy] = (double)fYD2[i];
                    MLower_dYD6[idxy] = (double)fYD3[i];
                    idxy++;
                }
            }

            if ((MMaster_dXD6.Length != MMaster_dYD6.Length) || (MLower_dXD6.Length != MLower_dYD6.Length) || (MUpper_dXD6.Length != MUpper_dYD6.Length))
            {
                _uiPlot12UpdateFlag = false;
            }
            else
            {
                _uiPlot12UpdateFlag = true;
            }
        }
        void MasterDataAssignRealPlot()
        {
            MasterDataAssignPlot1();
            MasterDataAssignPlot2();
            MasterDataAssignPlot3();
            MasterDataAssignPlot4();
            MasterDataAssignPlot9();
            MasterDataAssignPlot10();
        }
        void MasterDataAssignLMasterPlot()
        {
            MasterDataAssignPlot5();
            MasterDataAssignPlot6();
            MasterDataAssignPlot11();
        }
        void MasterDataAssignRMasterPlot()
        {
            MasterDataAssignPlot7();
            MasterDataAssignPlot8();
            MasterDataAssignPlot12();
        }

        void uiPlotSignalLineShow(ref ScottPlot.Plottables.SignalXY plotsig)
        {
            if (_uiObject.InvokeRequired)
            {
                _uiObject.BeginInvoke(new PlotSignalLineShowDelegate(InvokePlotSignalLineShow), new object[] { _uiObject, plotsig });
            }
            else
            {
                _uiObject.PlotSignalLineShow(ref plotsig);
            }
        }

        void uiPlotSignalLineHide(ref ScottPlot.Plottables.SignalXY plotsig)
        {
            if (_uiObject.InvokeRequired)
            {
                _uiObject.BeginInvoke(new PlotSignalLineHideDelegate(InvokePlotSignalLineHide), new object[] { _uiObject, plotsig });
            }
            else
            {
                _uiObject.PlotSignalLineHide(ref plotsig);
            }
        }

        delegate void PlotSignalLineShowDelegate(ref KVCOMSERVER.Form1 ui, ref ScottPlot.Plottables.SignalXY plot);
        static void InvokePlotSignalLineShow(ref KVCOMSERVER.Form1 ui, ref ScottPlot.Plottables.SignalXY plot)
        {
            ui.PlotSignalLineShow(ref plot);
        }

        delegate void PlotSignalLineHideDelegate(ref KVCOMSERVER.Form1 ui, ref ScottPlot.Plottables.SignalXY plot);
        static void InvokePlotSignalLineHide(ref KVCOMSERVER.Form1 ui, ref ScottPlot.Plottables.SignalXY plot)
        {
            ui.PlotSignalLineHide(ref plot);
        }



        public void uiSetModelName(string mod)
        {
            if (_uiObject.InvokeRequired)
            {
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.SetModelName(mod)));
            }
        }
        public void uiUPdateRealDataTable(DATAMODEL_RL dataR, DATAMODEL_RL dataL) //invoked when realtime update
        {
            if (_uiObject.InvokeRequired)
            {
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataRealCompSideRStroke = dataR.RealtimeStep2[0].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataRealCompSideRLoad = dataR.RealtimeStep2[1].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataRealExtnSideRStroke = dataR.RealtimeStep2[2].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataRealExtnSideRLoad = dataR.RealtimeStep2[3].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataRealSideRDiffStroke = dataR.RealtimeStep2[4].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataRealSideRDiffLoad = dataR.RealtimeStep2[5].ToArray()));

                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataRealCompSideLStroke = dataL.RealtimeStep2[0].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataRealCompSideLLoad = dataL.RealtimeStep2[1].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataRealExtnSideLStroke = dataL.RealtimeStep2[2].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataRealExtnSideLLoad = dataL.RealtimeStep2[3].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataRealSideLDiffStroke = dataL.RealtimeStep2[4].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataRealSideLDiffLoad = dataL.RealtimeStep2[5].ToArray()));
            }
            else
            {
                _uiObject.DataRealCompSideRStroke = dataR.RealtimeStep2[0].ToArray();
                _uiObject.DataRealCompSideRLoad = dataR.RealtimeStep2[1].ToArray();
                _uiObject.DataRealExtnSideRStroke = dataR.RealtimeStep2[2].ToArray();
                _uiObject.DataRealExtnSideRLoad = dataR.RealtimeStep2[3].ToArray();
                _uiObject.DataRealSideRDiffStroke = dataR.RealtimeStep2[4].ToArray();
                _uiObject.DataRealSideRDiffLoad = dataR.RealtimeStep2[5].ToArray();

                _uiObject.DataRealCompSideLStroke = dataL.RealtimeStep2[0].ToArray();
                _uiObject.DataRealCompSideLLoad = dataL.RealtimeStep2[1].ToArray();
                _uiObject.DataRealExtnSideLStroke = dataL.RealtimeStep2[2].ToArray();
                _uiObject.DataRealExtnSideLLoad = dataL.RealtimeStep2[3].ToArray();
                _uiObject.DataRealSideLDiffStroke = dataL.RealtimeStep2[4].ToArray();
                _uiObject.DataRealSideLDiffLoad = dataL.RealtimeStep2[5].ToArray();

            }
        }
        public void uiUPdateRealMasterActiveTable(DATAMODEL_MASTER dataM) //invoked when select model
        {
            if (_uiObject.InvokeRequired)
            {
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataRealCompSideRMaster = dataM.RMasteringStep2[1].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataRealCompSideRLower = dataM.RMasteringStep2[2].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataRealCompSideRUpper = dataM.RMasteringStep2[3].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataRealExtnSideRMaster = dataM.RMasteringStep2[5].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataRealExtnSideRLower = dataM.RMasteringStep2[6].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataRealExtnSideRUpper = dataM.RMasteringStep2[7].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataRealSideRDiffMaster = dataM.RMasteringStep2[9].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataRealSideRDiffLower = dataM.RMasteringStep2[10].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataRealSideRDiffUpper = dataM.RMasteringStep2[11].ToArray()));

                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataRealCompSideLMaster = dataM.LMasteringStep2[1].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataRealCompSideLLower = dataM.LMasteringStep2[2].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataRealCompSideLUpper = dataM.LMasteringStep2[3].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataRealExtnSideLMaster = dataM.LMasteringStep2[5].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataRealExtnSideLLower = dataM.LMasteringStep2[6].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataRealExtnSideLUpper = dataM.LMasteringStep2[7].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataRealSideLDiffMaster = dataM.LMasteringStep2[9].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataRealSideLDiffLower = dataM.LMasteringStep2[10].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataRealSideLDiffUpper = dataM.LMasteringStep2[11].ToArray()));
            }
            else
            {
                _uiObject.DataRealCompSideRMaster = dataM.RMasteringStep2[1].ToArray();
                _uiObject.DataRealCompSideRLower = dataM.RMasteringStep2[2].ToArray();
                _uiObject.DataRealCompSideRUpper = dataM.RMasteringStep2[3].ToArray();
                _uiObject.DataRealExtnSideRMaster = dataM.RMasteringStep2[5].ToArray();
                _uiObject.DataRealExtnSideRLower = dataM.RMasteringStep2[6].ToArray();
                _uiObject.DataRealExtnSideRUpper = dataM.RMasteringStep2[7].ToArray();
                _uiObject.DataRealSideRDiffMaster = dataM.RMasteringStep2[9].ToArray();
                _uiObject.DataRealSideRDiffLower = dataM.RMasteringStep2[10].ToArray();
                _uiObject.DataRealSideRDiffUpper = dataM.RMasteringStep2[11].ToArray();

                _uiObject.DataRealCompSideLMaster = dataM.LMasteringStep2[1].ToArray();
                _uiObject.DataRealCompSideLLower = dataM.LMasteringStep2[2].ToArray();
                _uiObject.DataRealCompSideLUpper = dataM.LMasteringStep2[3].ToArray();
                _uiObject.DataRealExtnSideLMaster = dataM.LMasteringStep2[5].ToArray();
                _uiObject.DataRealExtnSideLLower = dataM.LMasteringStep2[6].ToArray();
                _uiObject.DataRealExtnSideLUpper = dataM.LMasteringStep2[7].ToArray();
                _uiObject.DataRealSideLDiffMaster = dataM.LMasteringStep2[9].ToArray();
                _uiObject.DataRealSideLDiffLower = dataM.LMasteringStep2[10].ToArray();
                _uiObject.DataRealSideLDiffUpper = dataM.LMasteringStep2[11].ToArray();
            }
        }

        public void uiUpdateMasterFetchTeachTable() //invoked when teaching mode
        {
            if (_uiObject.InvokeRequired)
            {
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterCompSideLStroke = _masterData.LMasteringStep2[0].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterCompSideLMaster = _masterData.LMasteringStep2[1].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterCompSideLLower = _masterData.LMasteringStep2[2].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterCompSideLUpper = _masterData.LMasteringStep2[3].ToArray()));

                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterExtnSideLStroke = _masterData.LMasteringStep2[4].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterExtnSideLMaster = _masterData.LMasteringStep2[5].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterExtnSideLLower = _masterData.LMasteringStep2[6].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterExtnSideLUpper = _masterData.LMasteringStep2[7].ToArray()));

                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterSideLDiffStroke = _masterData.LMasteringStep2[8].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterSideLDiffMaster = _masterData.LMasteringStep2[9].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterSideLDiffLower = _masterData.LMasteringStep2[10].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterSideLDiffUpper = _masterData.LMasteringStep2[11].ToArray()));


                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterCompSideRStroke = _masterData.RMasteringStep2[0].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterCompSideRMaster = _masterData.RMasteringStep2[1].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterCompSideRLower = _masterData.RMasteringStep2[2].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterCompSideRUpper = _masterData.RMasteringStep2[3].ToArray()));

                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterExtnSideRStroke = _masterData.RMasteringStep2[4].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterExtnSideRMaster = _masterData.RMasteringStep2[5].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterExtnSideRLower = _masterData.RMasteringStep2[6].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterExtnSideRUpper = _masterData.RMasteringStep2[7].ToArray()));

                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterSideRDiffStroke = _masterData.RMasteringStep2[8].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterSideRDiffMaster = _masterData.RMasteringStep2[9].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterSideRDiffLower = _masterData.RMasteringStep2[10].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterSideRDiffUpper = _masterData.RMasteringStep2[11].ToArray()));
            }
        }

        public void uiUpdateMasterLTeachTable() //invoked when teaching mode
        {
            if (_uiObject.InvokeRequired)
            {
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterCompSideLStroke = _TMaster.LMasteringTeachStep2[0].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterCompSideLAccMaster = _TMaster.LMasteringTeachStep2[1].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterCompSideLMaster = _masterData.LMasteringStep2[1].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterCompSideLLower = _masterData.LMasteringStep2[2].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterCompSideLUpper = _masterData.LMasteringStep2[3].ToArray()));

                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterExtnSideLStroke = _TMaster.LMasteringTeachStep2[2].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterExtnSideLAccMaster = _TMaster.LMasteringTeachStep2[3].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterExtnSideLMaster = _masterData.LMasteringStep2[5].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterExtnSideLLower = _masterData.LMasteringStep2[6].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterExtnSideLUpper = _masterData.LMasteringStep2[7].ToArray()));

                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterSideLDiffStroke = _TMaster.LMasteringTeachStep2[4].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterSideLDiffAccMaster = _TMaster.LMasteringTeachStep2[5].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterSideLDiffMaster = _masterData.LMasteringStep2[9].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterSideLDiffLower = _masterData.LMasteringStep2[10].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterSideLDiffUpper = _masterData.LMasteringStep2[11].ToArray()));
            }
        }
        public void uiUpdateMasterRTeachTable() //invoked when teaching mode
        {
            if (_uiObject.InvokeRequired)
            {
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterCompSideRStroke = _TMaster.RMasteringTeachStep2[0].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterCompSideRAccMaster = _TMaster.RMasteringTeachStep2[1].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterCompSideRMaster = _masterData.RMasteringStep2[1].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterCompSideRLower = _masterData.RMasteringStep2[2].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterCompSideRUpper = _masterData.RMasteringStep2[3].ToArray()));

                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterExtnSideRStroke = _TMaster.RMasteringTeachStep2[2].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterExtnSideRAccMaster = _TMaster.RMasteringTeachStep2[3].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterExtnSideRMaster = _masterData.RMasteringStep2[5].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterExtnSideRLower = _masterData.RMasteringStep2[6].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterExtnSideRUpper = _masterData.RMasteringStep2[7].ToArray()));

                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterSideRDiffStroke = _TMaster.RMasteringTeachStep2[4].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterSideRDiffAccMaster = _TMaster.RMasteringTeachStep2[5].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterSideRDiffMaster = _masterData.RMasteringStep2[9].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterSideRDiffLower = _masterData.RMasteringStep2[10].ToArray()));
                _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.DataMasterSideRDiffUpper = _masterData.RMasteringStep2[11].ToArray()));
            }
        }
        public void workUpdateMasterData()
        {
            _masterData.LMasteringStep2[0] = _uiObject.DataMasterCompSideLStroke.ToList(item => item);
            _masterData.LMasteringStep2[1] = _uiObject.DataMasterCompSideLMaster.ToList(item => item);
            _masterData.LMasteringStep2[2] = _uiObject.DataMasterCompSideLLower.ToList(item => item);
            _masterData.LMasteringStep2[3] = _uiObject.DataMasterCompSideLUpper.ToList(item => item);
            _masterData.LMasteringStep2[4] = _uiObject.DataMasterExtnSideLStroke.ToList(item => item);
            _masterData.LMasteringStep2[5] = _uiObject.DataMasterExtnSideLMaster.ToList(item => item);
            _masterData.LMasteringStep2[6] = _uiObject.DataMasterExtnSideLLower.ToList(item => item);
            _masterData.LMasteringStep2[7] = _uiObject.DataMasterExtnSideLUpper.ToList(item => item);
            _masterData.LMasteringStep2[8] = _uiObject.DataMasterSideLDiffStroke.ToList(item => item);
            _masterData.LMasteringStep2[9] = _uiObject.DataMasterSideLDiffMaster.ToList(item => item);
            _masterData.LMasteringStep2[10] = _uiObject.DataMasterSideLDiffLower.ToList(item => item);
            _masterData.LMasteringStep2[11] = _uiObject.DataMasterSideLDiffUpper.ToList(item => item);

            _masterData.RMasteringStep2[0] = _uiObject.DataMasterCompSideRStroke.ToList(item => item);
            _masterData.RMasteringStep2[1] = _uiObject.DataMasterCompSideRMaster.ToList(item => item);
            _masterData.RMasteringStep2[2] = _uiObject.DataMasterCompSideRLower.ToList(item => item);
            _masterData.RMasteringStep2[3] = _uiObject.DataMasterCompSideRUpper.ToList(item => item);
            _masterData.RMasteringStep2[4] = _uiObject.DataMasterExtnSideRStroke.ToList(item => item);
            _masterData.RMasteringStep2[5] = _uiObject.DataMasterExtnSideRMaster.ToList(item => item);
            _masterData.RMasteringStep2[6] = _uiObject.DataMasterExtnSideRLower.ToList(item => item);
            _masterData.RMasteringStep2[7] = _uiObject.DataMasterExtnSideRUpper.ToList(item => item);
            _masterData.RMasteringStep2[8] = _uiObject.DataMasterSideRDiffStroke.ToList(item => item);
            _masterData.RMasteringStep2[9] = _uiObject.DataMasterSideRDiffMaster.ToList(item => item);
            _masterData.RMasteringStep2[10] = _uiObject.DataMasterSideRDiffLower.ToList(item => item);
            _masterData.RMasteringStep2[11] = _uiObject.DataMasterSideRDiffUpper.ToList(item => item);
        }
        public void workUpdateMasterDatabase()
        {
            _excelStoreMasterGraphData(ref _masterData, ref MasterFileActive);
            _excelPrintMasterData(ref _masterData, ref MasterFileActive);
            MasterDataAssignRealPlot();
            MasterDataAssignLMasterPlot();
            MasterDataAssignRMasterPlot();
            if (isnotNull(_masterData.RMasteringStep2) & isnotNull(_masterData.LMasteringStep2))
            {
                uiPlotRealMasterUpdate();
                uiPlotLTeachMasterUpdate();
                uiPlotRTeachMasterUpdate();
            }
            MasterUpdatingDatabaseReset();
        }
        public void workMasterValidation()
        {
            if (!MasterDataValidation())
            {
                _eeipTrigMasterFetch(_masterData._activeModelName, ref MasterFileActive, ref _masterData);
                _eeipTrigMasterFetchModel(ref _masterData);
                _eeipTrigMasterFetchGraph(ref _masterData);
                MasterDataAssignRealPlot();
                MasterDataAssignLMasterPlot();
                MasterDataAssignRMasterPlot();
                if (isnotNull(_masterData.RMasteringStep2) & isnotNull(_masterData.LMasteringStep2))
                {
                    uiPlotRealMasterUpdate();
                    uiPlotLTeachMasterUpdate();
                    uiPlotRTeachMasterUpdate();
                }
                uiUPdateRealMasterActiveTable(_masterData);
                MasterDataValidationSet();
            }
        }

        public void uiReloadRealtimeData()
        {
            if (MasterDataValidation())
            {
                MasterDataAssignRealPlot();
                uiPlotRealMasterUpdate();
                uiUPdateRealMasterActiveTable(_masterData);
                if (RealPresentConfirm())
                {
                    _backgroundDataPlot1Read();
                    _uiPlot1Update();
                    _backgroundDataPlot2Read();
                    _uiPlot2Update();
                    _backgroundDataPlot3Read();
                    _uiPlot3Update();
                    _backgroundDataPlot4Read();
                    _uiPlot4Update();
                    _backgroundDataPlot9Read();
                    _uiPlot9Update();
                    _backgroundDataPlot10Read();
                    _uiPlot10Update();
                    uiUPdateRealDataTable(_Rdata, _Ldata);
                }
            }
        }

        public void uiReloadTeachingData()
        {
            if (DataLMasterTeachIsExist())
            {
                _DataPlot5Read();
                _uiPlot5MasterUpdate();
                _DataPlot6Read();
                _uiPlot6MasterUpdate();
                _DataPlot11Read();
                _uiPlot11MasterUpdate();
            }
            if (DataRMasterTeachIsExist())
            {
                _DataPlot7Read();
                _uiPlot7MasterUpdate();
                _DataPlot8Read();
                _uiPlot8MasterUpdate();
                _DataPlot12Read();
                _uiPlot12MasterUpdate();
            }

            MasterDataAssignLMasterPlot();
            MasterDataAssignRMasterPlot();

            uiPlotLTeachMasterUpdate();
            uiPlotRTeachMasterUpdate();

            //uiUpdateMasterLTeachTable();
            //uiUpdateMasterRTeachTable();
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

        public float _MaxLoadLimit;
        public float _ProdLen;

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

        public float _step2DiffPosMin;
        public float _step2DiffPosMax;

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

        public float _step3DiffPosMin;
        public float _step3DiffPosMax;

        public List<string> DTM;
        public List<object> Step1Param;
        public List<object> Step2345Param;
        public List<object> DiffParam;

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

            DiffParam = new List<object>()
                {
                    _step2DiffPosMin,
                    _step2DiffPosMax,
                    _step3DiffPosMin,
                    _step3DiffPosMax
                };

        }
    }
    public class DATAMODEL_RL
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

        public DATAMODEL_RL()
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
        public int _Validation;

        public string _activeModelName;
        public string _activeKayabaNumber;
        public string _activeDay;
        public string _activeMonth;
        public string _activeYear;
        public string _activeHour;
        public string _activeMinute;
        public string _activeSecond;

        public float _MaxLoadLimit;
        public float _ProdLen;

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

        public float _step2DiffPosMin;
        public float _step2DiffPosMax;

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

        public float _step3DiffPosMin;
        public float _step3DiffPosMax;

        public List<string> DTM;
        public List<object> Step1Param;
        public List<object> Step2345Param;
        public List<object> DiffParam;

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

            DiffParam = new List<object>()
                {
                    _step2DiffPosMin,
                    _step2DiffPosMax,
                    _step3DiffPosMin,
                    _step3DiffPosMax
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
    public class DATAMODEL_TEACHING_MASTER
    {
        public List<List<float>> RMasteringTeachStep2;
        public List<List<float>> RMasteringTeachStep3;

        public List<List<float>> LMasteringTeachStep2;
        public List<List<float>> LMasteringTeachStep3;

        List<float> _RsideMasterStep2CompStroke;
        List<float> _RsideMasterStep2CompLoad;
        List<float> _RsideMasterStep2ExtnStroke;
        List<float> _RsideMasterStep2ExtnLoad;
        List<float> _RsideMasterStep2DiffStroke;
        List<float> _RsideMasterStep2DiffLoad;

        List<float> _RsideMasterStep3CompStroke;
        List<float> _RsideMasterStep3CompLoad;
        List<float> _RsideMasterStep3ExtnStroke;
        List<float> _RsideMasterStep3ExtnLoad;
        List<float> _RsideMasterStep3DiffStroke;
        List<float> _RsideMasterStep3DiffLoad;

        List<float> _LsideMasterStep2CompStroke;
        List<float> _LsideMasterStep2CompLoad;
        List<float> _LsideMasterStep2ExtnStroke;
        List<float> _LsideMasterStep2ExtnLoad;
        List<float> _LsideMasterStep2DiffStroke;
        List<float> _LsideMasterStep2DiffLoad;

        List<float> _LsideMasterStep3CompStroke;
        List<float> _LsideMasterStep3CompLoad;
        List<float> _LsideMasterStep3ExtnStroke;
        List<float> _LsideMasterStep3ExtnLoad;
        List<float> _LsideMasterStep3DiffStroke;
        List<float> _LsideMasterStep3DiffLoad;

        public DATAMODEL_TEACHING_MASTER()
        {

            RMasteringTeachStep2 = new List<List<float>>()
                {
                    _RsideMasterStep2CompStroke,
                    _RsideMasterStep2CompLoad,
                    _RsideMasterStep2ExtnStroke,
                    _RsideMasterStep2ExtnLoad,
                    _RsideMasterStep2DiffStroke,
                    _RsideMasterStep2DiffLoad
                };

            RMasteringTeachStep3 = new List<List<float>>()
                {
                    _RsideMasterStep3CompStroke,
                    _RsideMasterStep3CompLoad,
                    _RsideMasterStep3ExtnStroke,
                    _RsideMasterStep3ExtnLoad,
                    _RsideMasterStep3DiffStroke,
                    _RsideMasterStep3DiffLoad
                };

            LMasteringTeachStep2 = new List<List<float>>()
                {
                    _LsideMasterStep2CompStroke,
                    _LsideMasterStep2CompLoad,
                    _LsideMasterStep2ExtnStroke,
                    _LsideMasterStep2ExtnLoad,
                    _LsideMasterStep2DiffStroke,
                    _LsideMasterStep2DiffLoad
                };

            LMasteringTeachStep3 = new List<List<float>>()
                {
                    _LsideMasterStep3CompStroke,
                    _LsideMasterStep3CompLoad,
                    _LsideMasterStep3ExtnStroke,
                    _LsideMasterStep3ExtnLoad,
                    _LsideMasterStep3DiffStroke,
                    _LsideMasterStep3DiffLoad
                };
        }
    }
    public class DATAMODEL_RESULT
    {
        public List<float> Values;
        public float _MaxLoad;
        public float _Step2CompRef;
        public float _Step2CompRefMin;
        public float _Step2CompRefMax;
        public float _Step2ExtnRef;
        public float _Step2ExtnRefMin;
        public float _Step2ExtnRefMax;
        public float _Step3CompRef;
        public float _Step3CompRefMin;
        public float _Step3CompRefMax;
        public float _Step3ExtnRef;
        public float _Step3ExtnRefMin;
        public float _Step3ExtnRefMax;

        public DATAMODEL_RESULT()
        {
            Values = new List<float>()
                {
                    _MaxLoad,
                    _Step2CompRef,
                    _Step2CompRefMin,
                    _Step2CompRefMax,
                    _Step2ExtnRef,
                    _Step2ExtnRefMin,
                    _Step2ExtnRefMax,
                    _Step3CompRef,
                    _Step3CompRefMin,
                    _Step3CompRefMax,
                    _Step3ExtnRef,
                    _Step3ExtnRefMin,
                    _Step3ExtnRefMax

                };
        }

    }
}








