using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace LIBSETTEI
{
    public class SETTEI
    {
        string[] ALLSETTEI = new string[] { };

        private String _IPADDR;
        private String _PORTCOMM;
        private String _FILEDIR;
        private static readonly string IPADDR_FORMAT = (@"(?<=\<IPADDR\>)(.*)(?=\</IPADDR\>)");
        private static readonly string PORTCOMM_FORMAT = (@"(?<=\<PORTCOMM\>)(.*)(?=\</PORTCOMM\>)");
        private static readonly string FILEDIR_FORMAT = (@"(?<=\<FILEDIR\>)(.*)(?=\</FILEDIR\>)");

        public String IPADDR { get => _IPADDR != null ? _IPADDR : ""; set => _IPADDR = value; }
        public String PORTCOMM { get => _PORTCOMM != null ? _PORTCOMM : ""; set => _PORTCOMM = value; }
        public String FILEDIR { get => _FILEDIR != null ? _FILEDIR : ""; set => _FILEDIR = value; }

        public SETTEI(string INPUT)
        {
            string STREAM = File.ReadAllText(INPUT, Encoding.UTF8);
            IPADDR_SETTEI_SET(STREAM);
            PORTCOMM_SETTEI_SET(STREAM);
            FILEDIR_SETTEI_SET(STREAM);
        }

        public String IPADDR_SETTEI_GET() { return this.IPADDR; }
        public String IPADDR_SETTEI_SET(String stream)
        {
            Match result = Regex.Match(stream, IPADDR_FORMAT);
            if (result.Success)
            {
                this.IPADDR = result.Value;
                return (string)this.IPADDR;
            }
            else return "";

        }

        public String PORTCOMM_SETTEI_GET() { return this.PORTCOMM; }
        public String PORTCOMM_SETTEI_SET(String stream)
        {
            Match result = Regex.Match(stream, PORTCOMM_FORMAT);
            if (result.Success)
            {
                this.PORTCOMM = result.Value;
                return (string)this.PORTCOMM;
            }
            else return "";
        }

        public String FILEDIR_SETTEI_GET() { return this.FILEDIR; }
        public String FILEDIR_SETTEI_SET(String stream)
        {
            Match result = Regex.Match(stream, FILEDIR_FORMAT);
            if (result.Success)
            {
                this.FILEDIR = result.Value;
                return (string)this.FILEDIR;
            }
            else return "";
        }

        //public String

        public String[] SETTEI_GET()
        {
            this.ALLSETTEI = this.ALLSETTEI.Concat(new string[] { this.IPADDR, this.PORTCOMM }).ToArray();
            return this.ALLSETTEI;
        }

        public void SETTEI_FILESAVE()
        {

        }


    }


}
