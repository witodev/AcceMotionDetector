using System;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace MotionDetector
{
    // State object for receiving data from remote device.
    public class StateObject
    {
        // Client socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 256;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }

    public class SyncClient
    {
        private int size = 1024;

        public void StartClient(string send)
        {
            // Data buffer for incoming data.
            byte[] bytes = new byte[size];

            // Connect to a remote device.
            try
            {
                // Establish the remote endpoint for the socket.
                //This example uses port 11000 on the local computer.
                IPHostEntry ipHostInfo = Dns.GetHostEntry("donwito.ddns.us");
                Console.WriteLine("Host info:");
                int i;
                for (i = 0; i < ipHostInfo.AddressList.Length; i++)
                {
                    var item = ipHostInfo.AddressList[i].ToString();
                    Console.WriteLine(item);

                    if (item.Contains("."))
                    {
                        break;
                    }
                }
                IPAddress ipAddress = ipHostInfo.AddressList[i];
                //IPAddress ipAddress = IPAddress.Parse("192.168.1.14");
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 9876);

                // Create a TCP/IP  socket.
                Socket sender = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.
                try
                {
                    sender.Connect(remoteEP);

                    Console.WriteLine("Socket connected to {0}",
                        sender.RemoteEndPoint.ToString());

                    // Encode the data string into a byte array.
                    byte[] msg = Encoding.ASCII.GetBytes(send + "<EOF>");

                    // Send the data through the socket.
                    int bytesSent = sender.Send(msg);

                    // Receive the response from the remote device.
                    int bytesRec = sender.Receive(bytes);
                    Console.WriteLine("Echoed test = {0}",
                        Encoding.ASCII.GetString(bytes, 0, bytesRec));

                    // Release the socket.
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}