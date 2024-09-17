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
        SETTEI _settingObject;
        KVPROTOCOL _kvconnObject;
        EEIPClient _eeipObject;

        EXCELSTREAM MasterFile1;
        EXCELSTREAM RealtimeFile1;

        DATAMODEL _data;

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

            MasterFile1 = new EXCELSTREAM("MASTER");
            RealtimeFile1 = new EXCELSTREAM("REALTIME");

            _data = new DATAMODEL();

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

        public void SendMessage(string msgs)
        {
            _kvconnObject.connSend
                (
                    Encoding.ASCII.GetBytes(msgs)
                );
        }

        void _eeipBeacon()
        {
            while (true)
            {
                byte[] STAT_INPUT = _eeipObject.AssemblyObject.getInstance(0xA0);
                if (STAT_INPUT[0] == 0x01)
                {
                    _kvconnObject.setbitCommand("W0A0");
                }
                if (STAT_INPUT[0] == 0x00)
                {
                    _kvconnObject.resetbitCommand("W0A0");
                }
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
                        //_uiObject.setTextBox2(Encoding.ASCII.GetString(_kvconnObject.getMsgRecv(), 0, _kvconnObject.getByteRecv()));
                        _kvMsgRecv = Encoding.ASCII.GetString(_kvconnObject.getMsgRecv(), 0, _kvconnObject.getByteRecv());
                    }
                }
            }
            catch { }
        }

        void _readActiveModelData()
        {
            try
            {
                byte[] _INPUT;
                _INPUT = _eeipObject.AssemblyObject.getInstance(0xA1);
                char[] _charINPUT;
                _charINPUT = System.Text.Encoding.ASCII.GetString(_INPUT).ToCharArray();



            }
            catch { }
        }

        void _readDateTime()
        {
            try
            {
                byte[] _INPUT;
                _INPUT = _eeipObject.AssemblyObject.getInstance(0xA2);
                char[] _charINPUT;
                _charINPUT = System.Text.Encoding.ASCII.GetString(_INPUT).ToCharArray();


            }
            catch { }
        }

        void _readParameterData()
        {
            try
            {

            }
            catch { }
        }

        void _readRealTimeData()
        {
            try
            {

            }
            catch { }
        }

        void _readMasterData()
        {
            try
            {

            }
            catch { }
        }


        public void BackgroundWork_1()
        {
            int counter = 0;
            while (counter < 5)
            {
                counter++;
                Thread.Sleep(10);
            }
            DoWorkOnUI_1();
            _backgroundMessageRecv();
            _eeipBeacon();
        }

        private void DoWorkOnUI_1()
        {
            while (true)
            {
                MethodInvoker methodInvokerDelegate = delegate ()
                {

                };
                //This will be true if Current thread is not UI thread.
                _uiObject.Invoke(methodInvokerDelegate);
            }
        }
    }

    public class DATAMODEL
    {
        string _activeModelName;
        string _activeKayabaNumber;
        string _activeDay;
        string _activeMonth;
        string _activeYear;
        string _activeHour;
        string _activeMinute;
        string _activeSecond;

        List<string> DTM;
        List<string> Step1Param;
        List<string> Step2345Param;
        List<string> Judgement;
        List<string> RealtimeStep2;
        List<string> RealtimeStep3;
        List<string> MasteringStep2;
        List<string> MasteringStep3;

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

        }

        



    }
}
