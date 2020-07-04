using System;
using System.Collections.Generic;
using System.Text;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Linq;

namespace PowerManagementController.Net
{
    public class Ping
    {
        public static PingResult PingHost(string AddressToPing, int NumPings)
        {
            //Ensure the address is not empty, if it is, return null
            if (string.IsNullOrEmpty(AddressToPing))
            {
                //todo logging, email alert
                return null;
            }
            if(NumPings == 0)
            {
                NumPings = 1;
            }
            //Create a list for holding all the ping replies we get
            List<PingReply> replies = new List<PingReply>();
            for(int x = 0; x< NumPings; x++)
            {
                //send the pings with 250ms sleeps until NumPings is sent
                replies.Add(DoPing(AddressToPing));
                System.Threading.Thread.Sleep(100);
            }
            //Determine how many dropped pings we have, we dont really care why they were dropped
            int dropped = 0;
            foreach(PingReply reply in replies)
            {
                if (reply.Status != IPStatus.Success)
                    dropped++;
            }
            //Create a new ping result, add all the values from the ping test that was just done
            PingResult result = new PingResult
            {
                PinggedAddress = AddressToPing,
                NumPings = NumPings,
                AvgResponse = Enumerable.Average(replies.Select(x => x.RoundtripTime).ToArray()),
                NumDropped = dropped
            };
            //return the result
            return result;
        }
        private static PingReply DoPing(string AddressToPing)
        {
            //we dont really care about the data
            string data = "This is a ping test";
            byte[] buffer = Encoding.ASCII.GetBytes(data);

            int timeout = 1024;
            System.Net.NetworkInformation.Ping sendPing = new System.Net.NetworkInformation.Ping();
            System.Net.NetworkInformation.PingOptions options = new System.Net.NetworkInformation.PingOptions
            {
                DontFragment = true
            };
            System.Net.NetworkInformation.PingReply reply = sendPing.Send(AddressToPing, timeout, buffer, options);
            return reply;
        }
    }
    public class PingResult
    {
        public string PinggedAddress { get; internal set; }
        public double AvgResponse { get; internal set; }
        public int NumPings { get; internal set; }
        public int NumDropped { get; internal set; }
    }
}
