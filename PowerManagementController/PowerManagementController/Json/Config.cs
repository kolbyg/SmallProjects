using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using System.Text.Json.Serialization;
using PowerManagementController.Net;
using Lextm.SharpSnmpLib.Messaging;

namespace PowerManagementController.Json
{
    class Config
    {
        public ConfigFile LocalConfig;
        public Config()
        {
            Deserialize();
        }
        private void Deserialize()
        {
            using (StreamReader file = File.OpenText(Environment.CurrentDirectory + @"\config.json"))
                LocalConfig = JsonConvert.DeserializeObject<ConfigFile>(file.ReadToEnd());
        }

    }
    class ConfigFile
    {
        [JsonProperty] public string Identifier { get; set; }
        [JsonProperty] public string Location { get; set; }
        [JsonProperty] public string NodeKey { get; set; }
        [JsonProperty] public bool IsOrphaned { get; set; }
        [JsonProperty] public string ipmiutilPath { get; set; }
        [JsonProperty] public List<Hypervisor> LocalHypervisors { get; set; }
        [JsonProperty] public List<PowerDevice> LocalPowerDevices { get; set; }
        [JsonProperty] public List<PingDevice> LocalPingDevices { get; set; }
        [JsonProperty] public List<string> Nodes { get; set; }
    }
    class PingDevice
    {
        [JsonProperty] public string Identifier { get; set; }
        [JsonProperty] public string Hostname { get; set; }
    }
    class SNMPDevice
    {
        [JsonProperty] public List<SNMPOID> SNMPOIDs { get; set; }
        [JsonProperty] public string SNMPUsername { get; set; }
        [JsonProperty] public string SNMPPassword { get; set; }
        [JsonProperty] public string SNMPEncryptionKey { get; set; }
        [JsonProperty] public string SNMPHost { get; set; }
        [JsonProperty] public string SNMPIdentifier { get; set; }
        [JsonProperty] public int SNMPPort { get; set; }
        [JsonProperty] public Authentication SNMPAuthType { get; set; }
        [JsonProperty] public Encryption SNMPEncType { get; set; }
    }
    class SNMPOID
    {
        [JsonProperty] public string OID { get; set; }
        [JsonProperty] public string Description { get; set; }
    }
    class Node
    {
        [JsonProperty] public string Identifier { get; set; }
        [JsonProperty] public string Host { get; set; }
        [JsonProperty] public string Location { get; set; }
        [JsonProperty] public string LocalPowerDevices { get; set; }
        [JsonProperty] public string LocalHypervisors { get; set; }
    }
    class PowerDevice
    {
        [JsonProperty] public string Identifier { get; set; }
        [JsonProperty] public string Hostname { get; set; }
        [JsonProperty] public SNMPDevice SNMPDevice { get; set; }
    }
    class Hypervisor
    {
        [JsonProperty] public string Identifier { get; set; }
        [JsonProperty] public string Hostname { get; set; }
        [JsonProperty] public string IPMIHostname { get; set; }
        [JsonProperty] public string IPMIUsername { get; set; }
        [JsonProperty] public string IPMIPassword { get; set; }
        [JsonProperty] public IPMI.IPMIType IPMIType { get; set; }
        [JsonProperty] public IPMI.PowerStatus PowerStatus { get; set; }
    }

}
