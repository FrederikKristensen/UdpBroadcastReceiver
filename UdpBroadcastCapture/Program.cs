using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using CoronaTestRest.Model;

namespace UdpBroadcastCapture
{
    class Program
    {
        private const string URI = "http://coronatest.azurewebsites.net/api/CoronaTests";
        // https://msdn.microsoft.com/en-us/library/tst0kwb1(v=vs.110).aspx
        // IMPORTANT Windows firewall must be open on UDP port 7000
        // Use the network EGV5-DMU2 to capture from the local IoT devices
        private const int Port = 7064 ;
        //private static readonly IPAddress IpAddress = IPAddress.Parse("192.168.5.137"); 
        // Listen for activity on all network interfaces
        // https://msdn.microsoft.com/en-us/library/system.net.ipaddress.ipv6any.aspx
        static void Main()
        {
            using (UdpClient socket = new UdpClient(new IPEndPoint(IPAddress.Any, Port)))
            {
                IPEndPoint remoteEndPoint = new IPEndPoint(0, 0);
                while (true)
                {
                    Console.WriteLine("Waiting for broadcast {0}", socket.Client.LocalEndPoint);
                    byte[] datagramReceived = socket.Receive(ref remoteEndPoint);

                    string message = Encoding.ASCII.GetString(datagramReceived, 0, datagramReceived.Length);
                    Console.WriteLine("Receives {0} bytes from {1} port {2} message {3}", datagramReceived.Length,
                        remoteEndPoint.Address, remoteEndPoint.Port, message);
                    
                    CoronaTest test = Parse(message);

                    sendtorest(test);
                }
            }
        }

        private static void sendtorest(CoronaTest message)
        {
            
        }

        // To parse data from the IoT devices in the teachers room, Elisagårdsvej
        private static CoronaTest Parse(string response)
        {
            // data = "Id: 1, Name: Maskine 1, Temperature: 38.7, Location: Der, Date: " + str(datetime.now())
            string[] parts = response.Split(',');

            // id
            string[] ids = parts[0].Split(':');
            int id = Convert.ToInt32(ids[1].Trim());

            // name
            string[] names = parts[1].Split(':');
            string name = names[1].Trim();

            // Temperature
            string[] temps = parts[2].Split(':');
            double temp = Convert.ToDouble(temps[1].Trim());

            // Location
            string[] locas = parts[3].Split(':');
            string loca = locas[1].Trim();

            // Date
            string[] dates = parts[4].Split(':');
            string[] date = dates[1].Split(' ');
            string dat = date[0] + date[1] + date[2] + date[3] + date[4];

            // Time
            string[] times = parts[4].Split(':');
            string[] time = times[1].Split(' ');
            string[] tim = time[5].Split('.');
            string ti = tim[0].Trim();

            CoronaTest test = new CoronaTest(id,name,temp,loca,dat,ti);
            return test;
        }
    }
}
