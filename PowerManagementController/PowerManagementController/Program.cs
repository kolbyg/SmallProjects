using System;
using System.Collections.Generic;

namespace PowerManagementController
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
                test();
            switch (args[0])
            {
                case "--cycle":
                    break;
                case "--reset":
                    break;
                default:
                    break;
            }
        }
        static void DoCycle()
        {
            //Ping test all listed ips
            //check snmp for ups battery level, load, etc
            //check ilo/ipmi for server power status
            //do sync cycle between nodes
            //evaluate data and determine actions
        }
        static void Reset()
        {

        }
        static void test()
        {
            string[] toPing = { "172.31.6.21", "172.31.6.22", "172.31.0.111" };
            foreach(string host in toPing)
            {
                Net.PingResult result = Net.Ping.PingHost(host, 5);
                if (result.NumDropped != 0)
                    Console.WriteLine($"{result.NumDropped} Dropped Pings Detected!");
                Console.WriteLine($"Host {result.PinggedAddress} Avg Response {result.AvgResponse}ms");
            }
            string[] snmpToCheck = { "1.3.6.1.4.1.3808.1.1.1.3.2.1.0", "1.3.6.1.4.1.3808.1.1.1.2.1.1.0", "1.3.6.1.4.1.3808.1.1.1.2.2.3.0", "1.3.6.1.4.1.3808.1.1.1.2.2.8.0", "1.3.6.1.4.1.3808.1.1.1.4.2.4.0" };
            Net.SNMPConnectionOptions sNMPConnectionOptions = new Net.SNMPConnectionOptions
            {
                Authentication = Net.Authentication.SHA,
                Encryption = Net.Encryption.DES,
                SNMPEncryptionKey = "",
                SNMPHost = "172.31.6.21",
                SNMPPass = "",
                SNMPPort = "161",
                SNMPUser = "user1"
            };
            Net.SNMP snmp = new Net.SNMP(sNMPConnectionOptions);
            //get input voltage
            List<Net.SNMPResult> results;
            foreach(string snmpcheck in snmpToCheck)
            {
                results = snmp.QuerySNMP(snmpcheck);
                foreach (Net.SNMPResult result in results)
                {
                    Console.WriteLine($"{result.ID}: {result.Data}");
                }
            }
            Console.ReadLine();

        }
    }
}
