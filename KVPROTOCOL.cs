using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LIBKVPROTOCOL
{
    public class KVPROTOCOL
    {
        string _ipv4Addr;
        int _ipv4Socket;
        Socket _objConnect;
        bool _connState;
        int _recentByteRecv;
        int _recentByteSent;
        byte[] _recentMsgSent;
        byte[] _recentMsgRecv;


        byte[] setbitCMD = { 0x53, 0x54, 0x20 };
        byte[] resetbitCMD = { 0x52, 0x53, 0x20 };
        byte[] readCMD = { 0x52, 0x44, 0x20 };
        byte[] batchreadCMD = { 0x52, 0x44, 0x53, 0x20 };
        byte[] writeCMD = { 0x57, 0x52, 0x20 };
        byte[] batchwriteCMD = { 0x57, 0x52, 0x53, 0x20 };

        public KVPROTOCOL()
        {
            this._connState = false;
        }

        public int GetConnState()
        {
            return Convert.ToInt16(_connState);
        }

        public byte[] toBytes(string strData)
        {
            return Encoding.ASCII.GetBytes(strData);
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
                //Debug.Write(contentMsg);
                //Debug.Write(this._recentByteSent);
            }
        }

        public void connRecv()
        {
            if (_connState)
            {
                this._recentMsgRecv = new byte[1024];
                this._recentByteRecv = _objConnect.Receive(this._recentMsgRecv);
                //Debug.Write(this._recentMsgRecv);
                //Debug.Write(this._recentByteRecv);
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

        public int setbitCommand(string cmdaddress)
        {
            List<byte[]> cmdByte = new List<byte[]>();
            byte[] cmdaddrbyte = this.toBytes(cmdaddress);

            cmdByte.Add(setbitCMD);
            cmdByte.Add(cmdaddrbyte);
            byte[] endbyte = new byte[] { 0x0D };
            cmdByte.Add(endbyte);

            this.connSend(cmdByte.SelectMany(a => a).ToArray());

            Thread.Sleep(10);

            if (this.getState())
            {
                if (this.getAvail() > 0)
                {
                    this.connRecv();
                    if (this._recentMsgRecv[0] == 0x4f && this._recentMsgRecv[1] == 0x4b)
                    {
                        return 1;
                    }
                    else if (this._recentMsgRecv[0] == 0x45 && this._recentMsgRecv[1] == 0x30)
                    {
                        return 11;
                    }
                    else if (this._recentMsgRecv[0] == 0x45 && this._recentMsgRecv[1] == 0x31)
                    {
                        return 12;
                    }
                    else return 13;
                }
                else return 0;
            }
            else return 0;
        }

        public int resetbitCommand(string cmdaddress)
        {
            List<byte[]> cmdByte = new List<byte[]>();
            byte[] cmdaddrbyte = this.toBytes(cmdaddress);

            cmdByte.Add(resetbitCMD);
            cmdByte.Add(cmdaddrbyte);
            byte[] endbyte = new byte[] { 0x0D };
            cmdByte.Add(endbyte);

            this.connSend(cmdByte.SelectMany(a => a).ToArray());
            Thread.Sleep(10);

            if (this.getState())
            {
                if (this.getAvail() > 0)
                {
                    this.connRecv();
                    if (this._recentMsgRecv[0] == 0x4f && this._recentMsgRecv[1] == 0x4b)
                    {
                        return 1;
                    }
                    else if (this._recentMsgRecv[0] == 0x45 && this._recentMsgRecv[1] == 0x30)
                    {
                        return 11;
                    }
                    else if (this._recentMsgRecv[0] == 0x45 && this._recentMsgRecv[1] == 0x31)
                    {
                        return 12;
                    }
                    else return 13;
                }
                else return 0;
            }
            else return 0;
        }

        public int readbitCommand(string cmdaddress)
        {
            List<byte[]> cmdByte = new List<byte[]>();
            byte[] cmdaddrbyte = this.toBytes(cmdaddress);

            cmdByte.Add(readCMD);
            cmdByte.Add(cmdaddrbyte);
            byte[] endbyte = new byte[] { 0x0D };
            cmdByte.Add(endbyte);

            this.connSend(cmdByte.SelectMany(a => a).ToArray());
            Thread.Sleep(10);

            if (this.getState())
            {
                if (this.getAvail() > 0)
                {
                    this.connRecv();
                    if (this._recentMsgRecv[0] == 0x31)
                    {
                        return 1;
                    }
                    else if (this._recentMsgRecv[0] == 0x45 && this._recentMsgRecv[1] == 0x30)
                    {
                        return 11;
                    }
                    else if (this._recentMsgRecv[0] == 0x45 && this._recentMsgRecv[1] == 0x31)
                    {
                        return 12;
                    }
                    else return 13;
                }
                else return 0;
            }
            else return 0;
        }


        public byte[] readDataCommand(string cmdaddress, string cmdformat)
        {
            List<byte[]> cmdByte = new List<byte[]>();
            byte[] cmdaddrbyte = this.toBytes(cmdaddress);
            byte[] cmdformbyte = this.toBytes(cmdformat);

            cmdByte.Add(readCMD);
            cmdByte.Add(cmdaddrbyte);
            cmdByte.Add(cmdformbyte);
            byte[] endbyte = new byte[] { 0x0D };
            cmdByte.Add(endbyte);

            this.connSend(cmdByte.SelectMany(a => a).ToArray());
            Thread.Sleep(10);

            if (this.getState())
            {
                if (this.getAvail() > 0)
                {
                    this.connRecv();
                    return this.getMsgRecv();
                }
                else return null;
            }
            else return null;
        }

        public List<byte[]> batchreadDataCommand(string cmdaddress, string cmdformat, int count)
        {
            List<byte[]> cmdByte = new List<byte[]>();
            List<byte[]> recvBytes = new List<byte[]>();
            byte[] cmdaddrbyte = this.toBytes(cmdaddress);
            byte[] cmdformbyte = this.toBytes(cmdformat);

            cmdByte.Add(batchreadCMD);
            cmdByte.Add(cmdaddrbyte);
            cmdByte.Add(cmdformbyte);
            byte[] spacebyte = new byte[] { 0x20 };
            cmdByte.Add(spacebyte);
            cmdByte.Add(this.toBytes(count.ToString()));
            byte[] endbyte = new byte[] { 0x0D };
            cmdByte.Add(endbyte);

            this.connSend(cmdByte.SelectMany(a => a).ToArray());
            Thread.Sleep(10);

            if (this.getState())
            {
                if (this.getAvail() > 0)
                {
                    this.connRecv();
                    byte[] recvData = this.getMsgRecv();

                    int iy = 0;
                    int iz = 0;

                    byte[] parseByte = { };
                    List<byte[]> recvByte = new List<byte[]>();

                    for (int ix = 0; ix < recvData.Length; ix++)
                    {
                        if (ix < recvData.Length)
                        {
                            if (recvData[ix] == (byte)0x20)
                            {
                                byte[] sbuff = new byte[] { };
                                Array.Resize(ref sbuff, parseByte.Length);
                                Buffer.BlockCopy(parseByte, 0, sbuff, 0, sbuff.Length);
                                recvByte.Add(sbuff);

                                Array.Clear(parseByte, 0, parseByte.Length);
                                Array.Resize(ref parseByte, 0);
                                iz = 0;
                                iy++;
                            }
                            else
                            {
                                Array.Resize(ref parseByte, parseByte.Length + 1);
                                parseByte[iz] = (recvData[ix]);
                                iz++;
                            }
                        }
                        else if (ix == recvData.Length)
                        {
                            byte[] sbuff = new byte[] { };
                            Array.Resize(ref sbuff, parseByte.Length);
                            Buffer.BlockCopy(parseByte, 0, sbuff, 0, sbuff.Length);
                            recvByte.Add(sbuff);

                            Array.Clear(parseByte, 0, parseByte.Length);
                            Array.Resize(ref parseByte, 0);
                            iz = 0;
                            iy++;
                        }
                        else
                        {
                            Array.Resize(ref parseByte, parseByte.Length + 1);
                            parseByte[iz] = (recvData[ix]);
                            iz++;
                        }
                    }
                    return recvByte;
                }
                else return null;
            }
            else return null;
        }

        public int writeDataCommand(string cmdaddress, string cmdformat, string cmdvalue)
        {
            List<byte[]> cmdByte = new List<byte[]>();
            byte[] cmdaddrbyte = this.toBytes(cmdaddress);
            byte[] cmdformbyte = this.toBytes(cmdformat);
            byte[] cmdvaluebyte = this.toBytes(cmdvalue);

            cmdByte.Add(writeCMD);
            cmdByte.Add(cmdaddrbyte);
            cmdByte.Add(cmdformbyte);
            byte[] spacebyte = new byte[] { 0x20 };
            cmdByte.Add(spacebyte);
            cmdByte.Add(cmdvaluebyte);
            byte[] endbyte = new byte[] { 0x0D };
            cmdByte.Add(endbyte);

            this.connSend(cmdByte.SelectMany(a => a).ToArray());
            Thread.Sleep(10);

            if (this.getState())
            {
                if (this.getAvail() > 0)
                {
                    this.connRecv();
                    if (this._recentMsgRecv[0] == 0x4f && this._recentMsgRecv[1] == 0x4b)
                    {
                        return 1;
                    }
                    else if (this._recentMsgRecv[0] == 0x45 && this._recentMsgRecv[1] == 0x30)
                    {
                        return 11;
                    }
                    else if (this._recentMsgRecv[0] == 0x45 && this._recentMsgRecv[1] == 0x31)
                    {
                        return 12;
                    }
                    else return 13;
                }
                else return 0;
            }
            else return 0;
        }

        public int batchwriteDataCommand(string cmdaddress, string cmdformat, int count, string[] cmdvalue)
        {
            List<byte[]> cmdByte = new List<byte[]>();
            byte[] cmdaddrbyte = this.toBytes(cmdaddress);
            byte[] cmdformbyte = this.toBytes(cmdformat);

            cmdByte.Add(batchwriteCMD);
            cmdByte.Add(cmdaddrbyte);
            cmdByte.Add(cmdformbyte);
            byte[] spacebyte = new byte[] { 0x20 };
            cmdByte.Add(spacebyte);
            cmdByte.Add(this.toBytes(count.ToString()));

            for (int iv = 0; iv < count; iv++)
            {
                cmdByte.Add(spacebyte);
                byte[] cmdvaluebyte = this.toBytes(cmdvalue[iv]);
                cmdByte.Add(cmdvaluebyte);
            }
            byte[] endbyte = new byte[] { 0x0D };
            cmdByte.Add(endbyte);

            this.connSend(cmdByte.SelectMany(a => a).ToArray());
            Thread.Sleep(10);

            if (this.getState())
            {
                if (this.getAvail() > 0)
                {
                    this.connRecv();
                    if (this._recentMsgRecv[0] == 0x4f && this._recentMsgRecv[1] == 0x4b)
                    {
                        return 1;
                    }
                    else if (this._recentMsgRecv[0] == 0x45 && this._recentMsgRecv[1] == 0x30)
                    {
                        return 11;
                    }
                    else if (this._recentMsgRecv[0] == 0x45 && this._recentMsgRecv[1] == 0x31)
                    {
                        return 12;
                    }
                    else return 13;
                }
                else return 0;
            }
            else return 0;
        }


    }
}
