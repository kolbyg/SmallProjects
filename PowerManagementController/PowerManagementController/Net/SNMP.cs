using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Security;
using Lextm.SharpSnmpLib.Messaging;
using System.Net;
using System.Runtime.InteropServices;

namespace PowerManagementController.Net
{
    class SNMP
    {
        SNMPConnectionOptions ConnectionOptions;
        public SNMP(SNMPConnectionOptions Options)
        {
            //Store the options locally
            ConnectionOptions = Options;
            ConnectionOptions.PrivacyProvider = CreateKeys();
        }
        public List<SNMPResult> QuerySNMP(string OID)
        {
            ISnmpPdu pdu = DoQuery(OID).Pdu();
            IList<Variable> vars = pdu.Variables;
            List<SNMPResult> results = new List<SNMPResult>();
            foreach(Variable variable in vars)
            {
                SNMPResult result = new SNMPResult()
                {
                    ID = variable.Id.ToString(),
                    Data = variable.Data.ToString()
                };
                results.Add(result);
            }
            return results;
        }
        private ISnmpMessage DoQuery(string OID)
        {
            Discovery discovery = Messenger.GetNextDiscovery(SnmpType.GetRequestPdu);
            ReportMessage report = discovery.GetResponse(5000, new IPEndPoint(IPAddress.Parse(ConnectionOptions.SNMPHost), 161));
            GetRequestMessage request = new GetRequestMessage(VersionCode.V3, Messenger.NextMessageId, Messenger.NextRequestId, new OctetString(ConnectionOptions.SNMPUser), new List<Variable> { new Variable(new ObjectIdentifier(OID)) }, ConnectionOptions.PrivacyProvider, Messenger.MaxMessageSize, report);
            ISnmpMessage reply = request.GetResponse(5000, new IPEndPoint(IPAddress.Parse(ConnectionOptions.SNMPHost), Convert.ToInt32(ConnectionOptions.SNMPPort)));
            if (reply.Pdu().ErrorStatus.ToInt32() != 0) // != ErrorCode.NoError
            {
                throw ErrorException.Create(
                    "error in response",
                    IPAddress.Parse(ConnectionOptions.SNMPHost),
                    reply);
            }
            return reply;
        }
        private IPrivacyProvider CreateKeys()
        {
            IAuthenticationProvider auth = null;
            IPrivacyProvider priv = null;

            switch (ConnectionOptions.Authentication)
            {
                case Authentication.MD5:
                    auth = new MD5AuthenticationProvider(new OctetString(ConnectionOptions.SNMPPass));
                    break;
                case Authentication.SHA:
                    auth = new SHA1AuthenticationProvider(new OctetString(ConnectionOptions.SNMPPass));
                    break;
            }
            switch (ConnectionOptions.Encryption)
            {
                case Encryption.AES:
                    priv = new AESPrivacyProvider(new OctetString(ConnectionOptions.SNMPEncryptionKey), auth);
                    break;
                case Encryption.DES:
                    priv = new DESPrivacyProvider(new OctetString(ConnectionOptions.SNMPEncryptionKey), auth);
                    break;
            }
            return priv;
        }
    }
    public class SNMPConnectionOptions
    {
        public Encryption Encryption { get; set; }
        public Authentication Authentication { get; set; }
        public string SNMPUser { get; set; }
        public string SNMPPass { get; set; }
        public string SNMPEncryptionKey { get; set; }
        public string SNMPHost { get; set; }
        public string SNMPPort { get; set; }
        internal IPrivacyProvider PrivacyProvider { get; set; }

    }
    public enum Encryption
    {
        AES,
        DES
    }
    public enum Authentication
    {
        MD5,
        SHA
    }
    public class SNMPResult
    {
        public string ID { get; internal set; }
        public string Data { get; internal set; }
    }
}
