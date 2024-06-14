using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows.Forms;

using System.Net.Sockets;
using System.Diagnostics;

namespace WindowsFormsApp1
{

    public class ConnectPLC
    {
        public string _ipv4Addr;
        public int _ipv4Socket;
        public Socket _objConnect;
        public bool _connState;
        public int _recentByteRecv;
        public int _recentByteSent;
        public byte[] _recentMsgSent;
        public byte[] _recentMsgRecv;

        public ConnectPLC()
        {
            this._connState = false;
        }

        public void SetConnection(string ipStr, int ipSoc)
        {
            if (!_connState)
            {
                this._ipv4Addr = ipStr;
                this._ipv4Socket = ipSoc;
                //IPHostEntry ipHost = Dns.GetHostEntry(_ipv4Addr);
                IPAddress ipAddr = IPAddress.Parse(_ipv4Addr);
                IPEndPoint localEndPoint = new IPEndPoint(ipAddr, _ipv4Socket);
                Socket sender = new Socket(ipAddr.AddressFamily,
                           SocketType.Stream, ProtocolType.Tcp);

                //sender.Bind(localEndPoint);
                sender.Connect(localEndPoint);
                this._objConnect = sender;
                this._connState = true;
            }
            
        }

        public void CloseConnection()
        {
            if (_connState)
            {
                this._objConnect.Shutdown(SocketShutdown.Both);
                this._objConnect.Close();
                this._connState = false;
            }
        }

        //int byteSent = connPLC.ConnObj().Send(messageSent);
        public void connSend(byte[] contentMsg)
        {
            if (_connState)
            {
                this._recentByteSent = _objConnect.Send(contentMsg);
                Debug.Write(contentMsg);
                Debug.Write(this._recentByteSent);
            }
        }

        public void connRecv()
        {
            if (_connState)
            {
                this._recentMsgRecv = new byte[1024];
                this._recentByteRecv = _objConnect.Receive(this._recentMsgRecv);
                Debug.Write(this._recentMsgRecv);
                Debug.Write(this._recentByteRecv);
            }
        }

        public Socket ConnObj()
        {
            if (_connState)
            {
                return this._objConnect;
            }
            else return null;
        }

        public byte[] getMsgSent()
        {
            return this._recentMsgSent;
        }

        public byte[] getMsgRecv()
        {
            return this._recentMsgRecv;
        }

        public int getByteSent()
        {
            return this._recentByteSent;
        }
        public int getByteRecv()
        {
            return this._recentByteRecv;
        }

        public bool getState()
        {
            return _connState;
        }

        public int getAvail()
        {
            if (_connState)
            {
                return _objConnect.Available;
            }
            else return 0;
            
        }
    }

    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ConnectPLC objConn = new ConnectPLC();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(objConn));
        }
    }
}
