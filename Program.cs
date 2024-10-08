﻿using System;
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
using System.Text.RegularExpressions;
using System.IO;

using FUNCTION_TESTER;
using FUNCTION_TESTER.DB;
using LIBKVPROTOCOL;
using LIBSETTEI;
using LIBEXCELMANIPULATOR;
using Sres.Net.EEIP;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using ClosedXML.Excel;
using MoreLinq;
using DocumentFormat.OpenXml.Spreadsheet;
using MoreLinq.Extensions;
using DocumentFormat.OpenXml.Drawing.Charts;
using System.Reflection;

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
        Thread backgroundThread;

        SETTEI _settingObject;
        KVPROTOCOL _kvconnObject;
        EEIPClient _eeipObject;

        EXCELSTREAM MasterFileL1;
        EXCELSTREAM MasterFileR1;
        EXCELSTREAM RealtimeFileL1;
        EXCELSTREAM RealtimeFileR1;

        DATAMODEL _data;
        DATAMODEL_L _Ldata;
        DATAMODEL_R _Rdata;

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

            MasterFileL1 = new EXCELSTREAM("MASTER");
            MasterFileR1 = new EXCELSTREAM("MASTER");
            RealtimeFileL1 = new EXCELSTREAM("REALTIME");
            RealtimeFileR1 = new EXCELSTREAM("REALTIME");

            _data = new DATAMODEL();
            _Ldata = new DATAMODEL_L();
            _Rdata = new DATAMODEL_R();

            backgroundThread = new Thread(BackgroundWork);
            backgroundThread.Start();

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

        void _eeipEventHandler_1()
        {
            if (this.GetConnState() == 1)
            {
                byte[] STAT_INPUT = _eeipObject.AssemblyObject.getInstance(0xA0);
                _eeipBeacon(STAT_INPUT);
                //Thread.Sleep(100);
            }
        }

        void _eeipEventHandler_2()
        {
            if (this.GetConnState() == 1)
            {
                byte[] STAT_INPUT = _eeipObject.AssemblyObject.getInstance(0xA0);
                _eeipTriggerReadParameter(STAT_INPUT);
                //Thread.Sleep(10);
            }

        }

        void _eeipEventHandler_3()
        {
            if (this.GetConnState() == 1)
            {
                byte[] STAT_INPUT = _eeipObject.AssemblyObject.getInstance(0xA0);
                _eeipTriggerReadRealtime(STAT_INPUT);
                //Thread.Sleep(10);
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
                    Debug.Write("RL Read On");
                    Debug.Write((char)'\n');

                    _realtimeReadFlag = true;

                    _eeipreadDateTime();
                    _eeipreadJudgement(ref _Rdata.Judgement, 0xA5);
                    _eeipreadJudgement(ref _Ldata.Judgement, 0xA6);
                    _kvreadRealtime(ref _Rdata.RealtimeStep2, "ZF110000", "ZF110400", "ZF110800", "ZF111200", "ZF110000", "ZF510000", 400);
                    //_kvreadRealtime(ref _Rdata.RealtimeStep3, "ZF111604", "ZF112004", "ZF112404", "ZF113208", "ZF111604", "ZF510000", 400);
                    _kvreadRealtime(ref _Ldata.RealtimeStep2, "ZF210000", "ZF210400", "ZF210800", "ZF211200", "ZF210000", "ZF510500", 400);
                    //_kvreadRealtime(ref _Ldata.RealtimeStep3, "ZF211604", "ZF212004", "ZF212404", "ZF213208", "ZF211604", "ZF510500", 400);
                    
                    _excelStoreRealtimeData();
                }
                if (_realtimeReadFlag)
                {
                    //_kvconnObject.SetConnection(_uiObject.settingIpv4, _uiObject.settingPortIp);
                    _kvconnObject.writeDataCommand("W0C2", "", "0");
                    //_kvconnObject.CloseConnection();
                    
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
            RealtimeFileR1.setDateTime(_data.DTM);
            RealtimeFileL1.setDateTime(_data.DTM);

            RealtimeFileR1.setRealtimeJudgement(_Rdata.Judgement);
            RealtimeFileL1.setRealtimeJudgement(_Ldata.Judgement);

            RealtimeFileR1.setRealtimeStep2(_Rdata.RealtimeStep2);
            RealtimeFileL1.setRealtimeStep2(_Ldata.RealtimeStep2);

            //RealtimeFileR1.setRealtimeStep3(_Rdata.RealtimeStep3);
            //RealtimeFileL1.setRealtimeStep3(_Ldata.RealtimeStep3);
            string DirRealtime = $"C:\\FTP_DB_FUNCTION_TESTER\\LOG_REALTIME\\YEAR_20{_data.DTM[0]}\\MONTH_{_data.DTM[1]}\\DAY_{_data.DTM[2]}";
            CheckFolderPath(DirRealtime);

            string _filenameR1 = ($"{DirRealtime}\\RealtimeData_RH_{_data.DTM[3]}-{_data.DTM[4]}-{_data.DTM[5]}.xlsx");
            RealtimeFileR1.FilePrint(_filenameR1);

            string _filenameL1 = ($"{DirRealtime}\\RealtimeData_LH_{_data.DTM[3]}-{_data.DTM[4]}-{_data.DTM[5]}.xlsx");
            RealtimeFileL1.FilePrint(_filenameL1);

            _realtimeReadFlag = false;
            _kvconnObject.writeDataCommand("W0C2", "", "0");
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
                        _uiObject.setTextBox2(_kvMsgRecv);
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

                //Debug.Write("DateTime");
                //Debug.Write((char)'\n');

                byte[] buff = new byte[2];
                int iv = 0;

                //for (int i = 0; i < _INPUT.Length; i++) { Debug.Write(_INPUT[i]); Debug.Write(", "); }
                //Debug.Write((char)'\n');

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
                        //Debug.Write(BitConverter.ToInt16(sbuff, 0));
                        //Debug.Write(", ");
                    }
                }
                //Debug.Write((char)'\n');
                //Debug.Write(_buffDTM.Count().ToString());
                //Debug.Write((char)'\n');

                for (int i = 0; i < _data.DTM.Count(); i++)
                {
                    _data.DTM[i] = _buffDTM[i].ToString();
                    //Debug.Write(_data.DTM[i].ToString());

                }
                //Debug.Write((char)'\n');
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

                //Debug.Write("Step1Parameter");
                //Debug.Write((char)'\n');

                byte[] buff = new byte[4];
                int iv = 0;
                //Debug.Write(_INPUT.Length);
                //Debug.Write((char)'\n');

                //for (int i = 0; i < _INPUT.Length; i++) { Debug.Write(_INPUT[i]); Debug.Write(", "); }
                //Debug.Write((char)'\n');

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
                //Debug.Write((char)'\n');
                //Debug.Write(_buffPARAM1.Count().ToString());
                //Debug.Write((char)'\n');

                //Debug.Write(_data.Step1Param.Count().ToString());
                //Debug.Write((char)'\n');

                for (int i = 0; i < _data.Step1Param.Count(); i++)
                {
                    if (i == 0 || i == 4)
                    {
                        _data.Step1Param[i] = BitConverter.ToInt32(_buffPARAM1[i], 0).ToString();
                    }
                    else
                    {
                        _data.Step1Param[i] = BitConverter.ToSingle(_buffPARAM1[i], 0).ToString();
                    }

                    //for (int it = 0; it < _buffPARAM1[i].Length; it++) { Debug.Write(_buffPARAM1[i][it]); Debug.Write(", "); }
                    //Debug.Write(" : ");
                    //Debug.Write(_data.Step1Param[i]);
                    //Debug.Write((char)'\n');
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

                //Debug.Write("Step2345Parameter");
                //Debug.Write((char)'\n');

                byte[] buff = new byte[4];
                int iv = 0;

                //for (int i = 0; i < _INPUT.Length; i++) { Debug.Write(_INPUT[i]); Debug.Write(", "); }
                //Debug.Write((char)'\n');

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
                ////Debug.Write((char)'\n');
                ////Debug.Write(_buffPARAM2345.Count().ToString());
                ////Debug.Write((char)'\n');

                ////Debug.Write(_data.Step2345Param.Count().ToString());
                ////Debug.Write((char)'\n');

                for (int i = 0; i < _data.Step2345Param.Count(); i++)
                {
                    if (i == 0 || i == 9 || i == 10 || i == 19)
                    {
                        _data.Step2345Param[i] = BitConverter.ToInt16(_buffPARAM2345[i], 0).ToString();
                    }
                    else
                    {
                        _data.Step2345Param[i] = BitConverter.ToSingle(_buffPARAM2345[i], 0).ToString();
                    }
                    //for (int it = 0; it < _buffPARAM2345[i].Length; it++) { Debug.Write(_buffPARAM2345[i][it]); Debug.Write(", "); }
                    //Debug.Write(" : ");
                    //Debug.Write(_data.Step2345Param[i]);
                    //Debug.Write((char)'\n');
                }
            }
            //catch { }
        }

        void _eeipreadJudgement(ref List<string> judgementresult, Int16 addr)
        {
            //try
            {
                byte[] _INPUT = _eeipObject.AssemblyObject.getInstance(addr);
                Thread.Sleep(1);
                //Debug.Write("Judgement");
                //Debug.Write((char)'\n');
                //for (int i = 0; i < _INPUT.Length; i++) { Debug.Write(_INPUT[i]); Debug.Write(", "); }
                //Debug.Write((char)'\n');


                double[] _buffJudgement = new double[] { };
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
                        _buffJudgement[iz - 1] = Convert.ToDouble(BitConverter.ToSingle(buff, 0));
                        judgementresult[iz - 1] = _buffJudgement[iz - 1].ToString();
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
                            _buffJudgement[iz - 1] = Convert.ToDouble(BitConverter.ToSingle(buff, 0));
                            judgementresult[iz - 1] = _buffJudgement[iz - 1].ToString();
                            iv = 0;
                            Array.Clear(buff);

                            buff[iv] = _INPUT[i];
                            iv++;
                        }
                    }

                }

                //for (int it = 0; it < judgementresult.Count() - 1; it++)
                //{
                //  Debug.Write(_buffJudgement[it].ToString());
                //  Debug.Write(" : ");
                //  Debug.Write(judgementresult[i]);
                //  Debug.Write((char)'\n');
                //}
            }
            //catch { }
        }

        void _eeipreadRealtime(ref List<List<string>> realtimeresult, Int16 addr)
        {
            try
            {

            }
            catch { }
        }

        void _kvreadRealtime(ref List<List<string>> realtimeresult, string addr1, string addr2, string addr3, string addr4, string addr5, string addr6, int count)
        {
            //try
            {
                realtimeresult.Clear();

                //Debug.Write("RealTimeData");
                //Debug.Write((char)'\n');

                //_kvconnObject.SetConnection(_uiObject.settingIpv4, _uiObject.settingPortIp);

                List<byte[]> comp_stroke = new List<byte[]>(_kvconnObject.batchreadDataCommandInHex(addr1, count));
                //Thread.Sleep(20);
                //_kvconnObject.CloseConnection();
                //Thread.Sleep(20);

                //_kvconnObject.SetConnection(_uiObject.settingIpv4, _uiObject.settingPortIp);
                List<byte[]> comp_load = new List<byte[]>(_kvconnObject.batchreadDataCommandInHex(addr2, count));
                //Thread.Sleep(20);
                //_kvconnObject.CloseConnection();
                //Thread.Sleep(20);

                //_kvconnObject.SetConnection(_uiObject.settingIpv4, _uiObject.settingPortIp);
                List<byte[]> extn_stroke = new List<byte[]>(_kvconnObject.batchreadDataCommandInHex(addr3, count));
                //Thread.Sleep(20);
                //_kvconnObject.CloseConnection();
                //Thread.Sleep(20);

                //_kvconnObject.SetConnection(_uiObject.settingIpv4, _uiObject.settingPortIp);
                List<byte[]> extn_load = new List<byte[]>(_kvconnObject.batchreadDataCommandInHex(addr4, count));
                //Thread.Sleep(20);
                //_kvconnObject.CloseConnection();
                //Thread.Sleep(20);

                //_kvconnObject.SetConnection(_uiObject.settingIpv4, _uiObject.settingPortIp);
                List<byte[]> diff_stroke = new List<byte[]>(_kvconnObject.batchreadDataCommandInHex(addr5, count));
                //Thread.Sleep(20);
                //_kvconnObject.CloseConnection();
                //Thread.Sleep(20);

                //_kvconnObject.SetConnection(_uiObject.settingIpv4, _uiObject.settingPortIp);
                List<byte[]> diff_load = new List<byte[]>(_kvconnObject.batchreadDataCommandInHex(addr6, count));
                //Thread.Sleep(20);
                
                //_kvconnObject.CloseConnection();
                //Thread.Sleep(20);

                List<float> float_comp_stroke = new List<float>(hex16tofloat32(comp_stroke));
                List<float> float_comp_load = new List<float>(hex16tofloat32(comp_load));
                List<float> float_extn_stroke = new List<float>(hex16tofloat32(extn_stroke));
                List<float> float_extn_load = new List<float>(hex16tofloat32(extn_load));
                List<float> float_diff_stroke = new List<float>(hex16tofloat32(diff_stroke));
                List<float> float_diff_load = new List<float>(hex16tofloat32(diff_load));

                List<string> string_comp_stroke = float_comp_stroke.ConvertAll(new Converter<float, string>(floattostring));
                List<string> string_comp_load = float_comp_load.ConvertAll(new Converter<float, string>(floattostring));
                List<string> string_extn_stroke = float_extn_stroke.ConvertAll(new Converter<float, string>(floattostring));
                List<string> string_extn_load = float_extn_load.ConvertAll(new Converter<float, string>(floattostring));
                List<string> string_diff_stroke = float_diff_stroke.ConvertAll(new Converter<float, string>(floattostring));
                List<string> string_diff_load = float_diff_load.ConvertAll(new Converter<float, string>(floattostring));

                realtimeresult.Add(string_comp_stroke);
                realtimeresult.Add(string_comp_load);
                realtimeresult.Add(string_extn_stroke);
                realtimeresult.Add(string_extn_load);
                realtimeresult.Add(string_diff_stroke);
                realtimeresult.Add(string_diff_load);
            }
            //catch { }
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

                    await _uiPlot1UpdateAsync(_cts.Token);
                    await _backgroundDataPlot1ReadAsync(_cts.Token);

                    await _uiPlot2UpdateAsync(_cts.Token);
                    await _backgroundDataPlot2ReadAsync(_cts.Token);

                    await _uiPlot3UpdateAsync(_cts.Token);
                    await _backgroundDataPlot3ReadAsync(_cts.Token);

                    await _uiPlot4UpdateAsync(_cts.Token);
                    await _backgroundDataPlot4ReadAsync(_cts.Token);
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


        bool _uiPlot1UpdateFlag;
        bool _uiPlot2UpdateFlag;
        bool _uiPlot3UpdateFlag;
        bool _uiPlot4UpdateFlag;

        bool _uiPlot1ResetFlag;
        bool _uiPlot2ResetFlag;
        bool _uiPlot3ResetFlag;
        bool _uiPlot4ResetFlag;



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
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.Plot1Update(xd, yd)));
                }
                else
                {
                    _uiObject.Plot1Update(xd, yd);
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
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.Plot2Update(xd, yd)));
                }
                else
                {
                    _uiObject.Plot2Update(xd, yd);
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
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.Plot3Update(xd, yd)));
                }
                else
                {
                    _uiObject.Plot3Update(xd, yd);
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
                    _uiObject.BeginInvoke(new MethodInvoker(() => _uiObject.Plot4Update(xd, yd)));
                }
                else
                {
                    _uiObject.Plot4Update(xd, yd);
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
                else if((byte)(TRIG[0] & 0x01) == 0x01) 
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
            }
        }

        int parse2_idx;
        private void _backgroundDataPlot2Read()
        {
            if (this.GetConnState() == 1)
            {
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
                    _uiPlot1UpdateFlag = true;
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
            }
        }

        int parse3_idx;
        private void _backgroundDataPlot3Read()
        {
            if (this.GetConnState() == 1)
            {
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
            }
        }

        int parse4_idx;
        private void _backgroundDataPlot4Read()
        {
            if (this.GetConnState() == 1)
            {
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
            }
        }
    }

    public class DATAMODEL
    {
        public string _activeModelName;
        public　string _activeKayabaNumber;
        public　string _activeDay;
        public　string _activeMonth;
        public　string _activeYear;
        public　string _activeHour;
        public　string _activeMinute;
        public　string _activeSecond;

        public string _step1Enable;
        public string _step1Stroke;
        public string _step1CompresSpeed;
        public string _step1ExtendSpeed;
        public string _step1CycleCount;
        public string _step1MaxLoad;

        public string _step2Enable;
        public string _step2CompresSpeed;
        public string _step2CompressJudgeMin;
        public string _step2CompressJudgeMax;
        public string _step2CompressLoadRef;
        public string _step2ExtendSpeed;
        public string _step2ExtendJudgeMin;
        public string _step2ExtendJudgeMax;
        public string _step2ExtendLoadRef;
        public string _step2LoadRefTolerance;

        public string _step3Enable;
        public string _step3CompresSpeed;
        public string _step3CompressJudgeMin;
        public string _step3CompressJudgeMax;
        public string _step3CompressLoadRef;
        public string _step3ExtendSpeed;
        public string _step3ExtendJudgeMin;
        public string _step3ExtendJudgeMax;
        public string _step3ExtendLoadRef;
        public string _step3LoadRefTolerance;

        public　List<string> DTM;
        public　List<string> Step1Param;
        public　List<string> Step2345Param;

        public DATAMODEL()
        {
            DTM = new List<string>()
                {
                    _activeDay,
                    _activeMonth,
                    _activeYear,
                    _activeHour,
                    _activeMinute,
                    _activeSecond
                };

            Step1Param = new List<string>()
                {
                    _step1Enable,
                    _step1Stroke,
                    _step1CompresSpeed,
                    _step1ExtendSpeed,
                    _step1CycleCount,
                    _step1MaxLoad
                };

            Step2345Param = new List<string>()
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
        public List<string> Judgement;
        public List<List<string>> RealtimeStep2;
        public List<List<string>> RealtimeStep3;
        public List<List<string>> MasteringStep2;
        public List<List<string>> MasteringStep3;

        public string _MaxLoad;
        public string _Step2CompLoadRef;
        public string _Step2ExtnLoadRef;
        public string _Step3CompLoadRef;
        public string _Step3ExtnLoadRef;

        List<string> _RealtimeStep2CompStroke;
        List<string> _RealtimeStep2CompLoad;
        List<string> _RealtimeStep2ExtnStroke;
        List<string> _RealtimeStep2ExtnLoad;
        List<string> _RealtimeStep2DiffStroke;
        List<string> _RealtimeStep2DiffLoad;

        List<string> _RealtimeStep3CompStroke;
        List<string> _RealtimeStep3CompLoad;
        List<string> _RealtimeStep3ExtnStroke;
        List<string> _RealtimeStep3ExtnLoad;
        List<string> _RealtimeStep3DiffStroke;
        List<string> _RealtimeStep3DiffLoad;

        List<string> _MasterStep2CompStroke;
        List<string> _MasterStep2CompLoad;
        List<string> _MasterStep2CompLoadLower;
        List<string> _MasterStep2CompLoadUpper;
        List<string> _MasterStep2ExtnStroke;
        List<string> _MasterStep2ExtnLoad;
        List<string> _MasterStep2ExtnLoadLower;
        List<string> _MasterStep2ExtnLoadUpper;
        List<string> _MasterStep2DiffStroke;
        List<string> _MasterStep2DiffLoad;
        List<string> _MasterStep2DiffLoadLower;
        List<string> _MasterStep2DiffLoadUpper;

        List<string> _MasterStep3CompStroke;
        List<string> _MasterStep3CompLoad;
        List<string> _MasterStep3CompLoadLower;
        List<string> _MasterStep3CompLoadUpper;
        List<string> _MasterStep3ExtnStroke;
        List<string> _MasterStep3ExtnLoad;
        List<string> _MasterStep3ExtnLoadLower;
        List<string> _MasterStep3ExtnLoadUpper;
        List<string> _MasterStep3DiffStroke;
        List<string> _MasterStep3DiffLoad;
        List<string> _MasterStep3DiffLoadLower;
        List<string> _MasterStep3DiffLoadUpper;

        public DATAMODEL_R()
        {
            Judgement = new List<string>() 
            {
                _MaxLoad,
                _Step2CompLoadRef,
                _Step2ExtnLoadRef,
                _Step3CompLoadRef,
                _Step3ExtnLoadRef
            };

            RealtimeStep2 = new List<List<string>>()
            {
                _RealtimeStep2CompStroke,
                _RealtimeStep2CompLoad,
                _RealtimeStep2ExtnStroke,
                _RealtimeStep2ExtnLoad,
                _RealtimeStep2DiffStroke,
                _RealtimeStep2DiffLoad
            };

            RealtimeStep3 = new List<List<string>>()
            {
                _RealtimeStep3CompStroke,
                _RealtimeStep3CompLoad,
                _RealtimeStep3ExtnStroke,
                _RealtimeStep3ExtnLoad,
                _RealtimeStep3DiffStroke,
                _RealtimeStep3DiffLoad
            };

            MasteringStep2 = new List<List<string>>()
            {
                _MasterStep2CompStroke,
                _MasterStep2CompLoad,
                _MasterStep2CompLoadLower,
                _MasterStep2CompLoadUpper,
                _MasterStep2ExtnStroke,
                _MasterStep2ExtnLoad,
                _MasterStep2ExtnLoadLower,
                _MasterStep2ExtnLoadUpper,
                _MasterStep2DiffStroke,
                _MasterStep2DiffLoad,
                _MasterStep2DiffLoadLower,
                _MasterStep2DiffLoadUpper
            };

            MasteringStep3 = new List<List<string>>()
            {
                _MasterStep3CompStroke,
                _MasterStep3CompLoad,
                _MasterStep3CompLoadLower,
                _MasterStep3CompLoadUpper,
                _MasterStep3ExtnStroke,
                _MasterStep3ExtnLoad,
                _MasterStep3ExtnLoadLower,
                _MasterStep3ExtnLoadUpper,
                _MasterStep3DiffStroke,
                _MasterStep3DiffLoad,
                _MasterStep3DiffLoadLower,
                _MasterStep3DiffLoadUpper
            };
        }
    }

    public class DATAMODEL_L
    {
        public List<string> Judgement;
        public List<List<string>> RealtimeStep2;
        public List<List<string>> RealtimeStep3;
        public List<List<string>> MasteringStep2;
        public List<List<string>> MasteringStep3;

        public string _MaxLoad;
        public string _Step2CompLoadRef;
        public string _Step2ExtnLoadRef;
        public string _Step3CompLoadRef;
        public string _Step3ExtnLoadRef;

        List<string> _RealtimeStep2CompStroke;
        List<string> _RealtimeStep2CompLoad;
        List<string> _RealtimeStep2ExtnStroke;
        List<string> _RealtimeStep2ExtnLoad;
        List<string> _RealtimeStep2DiffStroke;
        List<string> _RealtimeStep2DiffLoad;

        List<string> _RealtimeStep3CompStroke;
        List<string> _RealtimeStep3CompLoad;
        List<string> _RealtimeStep3ExtnStroke;
        List<string> _RealtimeStep3ExtnLoad;
        List<string> _RealtimeStep3DiffStroke;
        List<string> _RealtimeStep3DiffLoad;

        List<string> _MasterStep2CompStroke;
        List<string> _MasterStep2CompLoad;
        List<string> _MasterStep2CompLoadLower;
        List<string> _MasterStep2CompLoadUpper;
        List<string> _MasterStep2ExtnStroke;
        List<string> _MasterStep2ExtnLoad;
        List<string> _MasterStep2ExtnLoadLower;
        List<string> _MasterStep2ExtnLoadUpper;
        List<string> _MasterStep2DiffStroke;
        List<string> _MasterStep2DiffLoad;
        List<string> _MasterStep2DiffLoadLower;
        List<string> _MasterStep2DiffLoadUpper;

        List<string> _MasterStep3CompStroke;
        List<string> _MasterStep3CompLoad;
        List<string> _MasterStep3CompLoadLower;
        List<string> _MasterStep3CompLoadUpper;
        List<string> _MasterStep3ExtnStroke;
        List<string> _MasterStep3ExtnLoad;
        List<string> _MasterStep3ExtnLoadLower;
        List<string> _MasterStep3ExtnLoadUpper;
        List<string> _MasterStep3DiffStroke;
        List<string> _MasterStep3DiffLoad;
        List<string> _MasterStep3DiffLoadLower;
        List<string> _MasterStep3DiffLoadUpper;

        public DATAMODEL_L()
        {
            Judgement = new List<string>()
            {
                _MaxLoad,
                _Step2CompLoadRef,
                _Step2ExtnLoadRef,
                _Step3CompLoadRef,
                _Step3ExtnLoadRef
            };

            RealtimeStep2 = new List<List<string>>()
            {
                _RealtimeStep2CompStroke,
                _RealtimeStep2CompLoad,
                _RealtimeStep2ExtnStroke,
                _RealtimeStep2ExtnLoad,
                _RealtimeStep2DiffStroke,
                _RealtimeStep2DiffLoad
            };

            RealtimeStep3 = new List<List<string>>()
            {
                _RealtimeStep3CompStroke,
                _RealtimeStep3CompLoad,
                _RealtimeStep3ExtnStroke,
                _RealtimeStep3ExtnLoad,
                _RealtimeStep3DiffStroke,
                _RealtimeStep3DiffLoad
            };

            MasteringStep2 = new List<List<string>>()
            {
                _MasterStep2CompStroke,
                _MasterStep2CompLoad,
                _MasterStep2CompLoadLower,
                _MasterStep2CompLoadUpper,
                _MasterStep2ExtnStroke,
                _MasterStep2ExtnLoad,
                _MasterStep2ExtnLoadLower,
                _MasterStep2ExtnLoadUpper,
                _MasterStep2DiffStroke,
                _MasterStep2DiffLoad,
                _MasterStep2DiffLoadLower,
                _MasterStep2DiffLoadUpper
            };

            MasteringStep3 = new List<List<string>>()
            {
                _MasterStep3CompStroke,
                _MasterStep3CompLoad,
                _MasterStep3CompLoadLower,
                _MasterStep3CompLoadUpper,
                _MasterStep3ExtnStroke,
                _MasterStep3ExtnLoad,
                _MasterStep3ExtnLoadLower,
                _MasterStep3ExtnLoadUpper,
                _MasterStep3DiffStroke,
                _MasterStep3DiffLoad,
                _MasterStep3DiffLoadLower,
                _MasterStep3DiffLoadUpper
            };
        }
    }
}
