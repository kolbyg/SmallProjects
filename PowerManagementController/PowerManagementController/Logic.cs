using PowerManagementController.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerManagementController
{
    class Logic : Program
    {
        public static void DoNodeSync()
        {
            //todo
        }
        public static void PingTests()
        {
            foreach (string host in config.LocalConfig.IPsToPing)
            {
                Net.PingResult result = Net.Ping.PingHost(host, 5);
                if (result.NumDropped != 0)
                    Console.WriteLine($"{result.NumDropped} Dropped Pings Detected!"); //todo, throw error, email alert
                Console.WriteLine($"Host {result.PinggedAddress} Avg Response {result.AvgResponse}ms");
            }
        }
        public static void SNMPChecks()
        {
            foreach (SNMPDevice device in config.LocalConfig.SNMPDevices)
            {
                Console.WriteLine($"Connecting to SNMP device: {device.SNMPIdentifier} - {device.SNMPUsername}@{device.SNMPHost}");
                Net.SNMPConnectionOptions sNMPConnectionOptions = new Net.SNMPConnectionOptions
                {
                    Authentication = device.SNMPAuthType,
                    Encryption = device.SNMPEncType,
                    SNMPEncryptionKey = device.SNMPEncryptionKey,
                    SNMPHost = device.SNMPHost,
                    SNMPPass = device.SNMPPassword,
                    SNMPPort = device.SNMPPort.ToString(),
                    SNMPUser = device.SNMPUsername
                };
                Console.WriteLine($"Auth Type is {device.SNMPAuthType.ToString()}, Encryption Type is {device.SNMPEncType.ToString()}");
                Net.SNMP snmp = new Net.SNMP(sNMPConnectionOptions);
                Console.WriteLine("Connection keys generated");
                List<Net.SNMPResult> results;
                Console.WriteLine("Checking OID Data...");
                foreach (SNMPOID oid in device.SNMPOIDs)
                {
                    results = snmp.QuerySNMP(oid.OID);
                    foreach (Net.SNMPResult result in results)
                    {
                        Console.WriteLine($"{oid.Description}: {result.Data}");
                    }
                }
                Console.WriteLine($"Done processing SNMP Device: {device.SNMPIdentifier}");
            }
        }
    }
}
