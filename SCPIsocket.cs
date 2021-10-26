using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace LabToys
{
    public class SCPIsocket
    {
        private string hostIP = "localhost";
        private int hostPort = 5025;
        private bool stayConnected = false;
        private int timeout = 10000;
        private TcpClient deviceSocket = null;
        private NetworkStream deviceStream = null;

        public int Timeout { get => timeout; set => timeout = value; }

        //-------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public SCPIsocket(string ip, ushort port)
        {
            hostIP = ip;
            hostPort = port;
        }

        //-----------------------------------------------------------------------------------------
        private bool ConnectInternal(bool stayConnected = false)
        {
            try
            {
                deviceSocket = new TcpClient(hostIP, hostPort);
                deviceSocket.ReceiveTimeout = timeout;
                deviceStream = deviceSocket.GetStream();
                this.stayConnected = stayConnected;
            }
            catch
            {
                Close();
                return false;
            }

            return true;
        }

        //-----------------------------------------------------------------------------------------
        public bool Connect()
        {
            return ConnectInternal(true);
        }

        //-----------------------------------------------------------------------------------------
        public void Close()
        {
            if (deviceStream != null)
            {
                deviceStream.Close();
                deviceStream.Dispose();
                deviceStream = null;
            }

            if (deviceSocket != null)
            {
                deviceSocket.Close();
                deviceSocket.Dispose();
                deviceSocket = null;
            }
        }

        //-----------------------------------------------------------------------------------------
        public bool SendRaw(byte[] data, bool stayConnected = false)
        {
            if (deviceStream == null)
            {
                if (ConnectInternal() == false) return false;
            }

            try
            {
                deviceStream.Write(data, 0, data.Length);
                if (stayConnected == false
                    && this.stayConnected == false)
                {
                    Close();
                }
            }
            catch
            {
                Close();
                return false;
            }

            return true;
        }

        //-----------------------------------------------------------------------------------------
        public bool SendCommand(string command, bool stayConnected = false)
        {
            command = command + '\n';
            byte[] data = Encoding.ASCII.GetBytes(command);
            return SendRaw(data, stayConnected);
        }

        //-----------------------------------------------------------------------------------------
        public byte[] GetRaw(int respondLength = 4096, bool stayConnected = false)
        {
            if (deviceStream == null)
            {
                return null;
            }

            byte[] data = new byte[respondLength];
            try
            {
                int bytes = deviceStream.Read(data, 0, data.Length);
                Array.Resize(ref data, bytes);
                if (stayConnected == false
                    && this.stayConnected == false)
                {
                    Close();
                }
            }
            catch
            {
                Close();
                return new byte[0];
            }

            return data;
        }

        //-----------------------------------------------------------------------------------------
        public string GetAns(int respondLength = 4096, bool stayConnected = false)
        {
            byte[] data = GetRaw(respondLength, stayConnected);
            if (data.Length == 0)
            {
                return "";
            }

            string response = Encoding.ASCII.GetString(data);
            return response;
        }

        //-----------------------------------------------------------------------------------------
        public string SendCommandGetAns(string command, bool stayConnected = false)
        {
            if (SendCommand(command, true) == false)
            {
                return "";
            }
            return GetAns(1024, stayConnected);
        }

        //-----------------------------------------------------------------------------------------
        public byte[] SendCommandGetRaw(string command, int respondLength = 4096, bool stayConnected = false)
        {
            if (SendCommand(command, true) == false)
            {
                return new byte[0];
            }
            return GetRaw(respondLength, stayConnected);
        }
    }
}
