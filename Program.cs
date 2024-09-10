using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Diagnostics;
using LIBKVPROTOCOL;
using LIBSETTEI;

namespace KVCOMSERVER
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            KVPROTOCOL objConn = new KVPROTOCOL();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(objConn));
        }
    }
}
