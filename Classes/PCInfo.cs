using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detector.Classes
{
    class PCInfo
    {
        public string _PCModelName;
        public string _PCVendorName;
        public string _IPComputerName;
        public string _UserName;

        public string PCModelName
        {
            get { return _PCModelName; }
            set { _PCModelName = value; }
        }
        public string PCVendorName
        {
            get { return _PCVendorName; }
            set { _PCVendorName = value; }
        }
        public string IPComputerName
        {
            get { return _IPComputerName; }
            set { _IPComputerName = value; }
        }

        public string UserName
        {
            get { return _UserName; }
            set { _UserName = value; }
        }
    }
}
