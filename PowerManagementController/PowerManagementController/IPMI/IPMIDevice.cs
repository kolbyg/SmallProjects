using System;
using System.Collections.Generic;
using System.Text;

namespace PowerManagementController.IPMI
{
    class IPMIDevice
    {
        ApplicationControl.ipmiutil IPMIUtil;
        string Hostname;
        string Username;
        string Password;
        public IPMIDevice(string Hostname, string Username, string Password, string PathToIPMIUtil)
        {
            IPMIUtil = new ApplicationControl.ipmiutil(PathToIPMIUtil);
            this.Hostname = Hostname;
            this.Username = Password;
            this.Username = Username;
        }
        public PowerStatus QueryPowerStatus()
        {
            return PowerStatus.Off;
        }
        public bool SetPowerStatus(PowerStatus DesiredState)
        {
            switch (DesiredState)
            {
                case PowerStatus.On:
                    IPMIUtil.InvokeCommand($"reset -u -N {Hostname} -U {Username} -P {Password}");
                    break;
                case PowerStatus.Off:
                    IPMIUtil.InvokeCommand($"reset -d -N {Hostname} -U {Username} -P {Password}");
                    break;
                case PowerStatus.SoftOff:
                    IPMIUtil.InvokeCommand($"reset -D -N {Hostname} -U {Username} -P {Password}");
                    break;
            }
            return false;
        }
    }
    enum PowerStatus
    {
        On,
        Off,
        SoftOff
    }
    enum IPMIType
    {
        Supermicro,
        HPE,
        Dell
    }
}
