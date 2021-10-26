using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;

namespace LabToys
{
    public class SCPI
    {
        private string hostIP = "";
        private int hostPort = 0;
        private TcpClient deviceSocket = null;
        private NetworkStream deviceStream = null;

        //-------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public SCPI(string ip, ushort port)
        {
            hostIP = ip;
            hostPort = port;
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout"></param>
        public void Connect(int timeout = 10000)
        {
            deviceSocket = new TcpClient(hostIP, hostPort);
            deviceSocket.ReceiveTimeout = timeout;
            deviceStream = deviceSocket.GetStream();
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        public void Close()
        {
            deviceStream.Close();
            deviceSocket.Close();

            deviceStream.Dispose();
            deviceStream = null;

            deviceSocket.Dispose();
            deviceSocket = null;
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        public void SendCommand(string command)
        {
            command = command + '\n';
            byte[] data = System.Text.Encoding.ASCII.GetBytes(command);
            deviceStream.Write(data, 0, data.Length);
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public string SendCommandGetAns(string command, int respondLength = 1400)
        {
            command = command + '\n';
            byte[] data = System.Text.Encoding.ASCII.GetBytes(command);
            deviceStream.Write(data, 0, data.Length);

            data = new byte[respondLength];
            int bytes = deviceStream.Read(data, 0, data.Length);
            string response = System.Text.Encoding.ASCII.GetString(data);

            return response;
        }

        //-----------------------------------------------------------------------------------------
        public byte[] SendCommandGetRaw(string command, int respondLength = 1400)
        {
            command = command + '\n';
            byte[] data = System.Text.Encoding.ASCII.GetBytes(command);
            deviceStream.Write(data, 0, data.Length);

            data = new byte[respondLength];
            int bytes = deviceStream.Read(data, 0, data.Length);
            Array.Resize(ref data, bytes);
            return data;
        }

        //-----------------------------------------------------------------------------------------
        public byte[] GetRaw(int respondLength = 1400)
        {
            byte[] data = new byte[respondLength];
            int bytes = deviceStream.Read(data, 0, data.Length);
            Array.Resize(ref data, bytes);
            return data;
        }

        //-----------------------------------------------------------------------------------------
        public string GetAns(int respondLength = 1400)
        {
            byte[] data = new byte[respondLength];
            int bytes = deviceStream.Read(data, 0, data.Length);
            string response = System.Text.Encoding.ASCII.GetString(data);

            return response;
        }

    }
}
