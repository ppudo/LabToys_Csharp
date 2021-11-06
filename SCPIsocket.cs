﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;

namespace LabToys
{
    public class SCPIsocket
    {
        private string hostIP = "localhost";
        private int hostPort = 5025;
        private bool stayConnected = false;
        private int timeout = 10000;
        private int sendDelay = 1;
        private TcpClient deviceSocket = null;
        private NetworkStream deviceStream = null;

        private int idxConnectionCounter = 0;
        private List<int> connectionList = new List<int>();
        private int currentConnectionIdx = (int)ConnectionIdx.NO_IDX;
        private List<int> freeConnectionList = new List<int>();
        private int connectionWaitTime = 5;

        public int Timeout { get => timeout; set => timeout = value; }
        public int SendDelay { get => sendDelay; set => sendDelay = value; }

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
        private int ConnectInternal(bool stayConnected = false, int oldIdx = (int)ConnectionIdx.NO_IDX)
        {
            int idx = (int)ConnectionIdx.NO_IDX;

            //recall old socket idx if oldIdx is in use
            if ( oldIdx != (int)ConnectionIdx.NO_IDX )
            {
                if( freeConnectionList.Remove(oldIdx) == true )
                {
                    idx = oldIdx;
                }
            }

            //create new Idx
            if( idx == (int)ConnectionIdx.NO_IDX )
            {
                do
                {
                    idxConnectionCounter++;
                    if (idxConnectionCounter >= int.MaxValue)
                    {
                        idxConnectionCounter = 1;
                    }
                }
                while (connectionList.Contains(idxConnectionCounter)
                        || freeConnectionList.Contains(idxConnectionCounter));
            }
     
            //create socket if there is no socket
            if( deviceStream == null )
            {
                try
                {
                    deviceSocket = new TcpClient(hostIP, hostPort);
                    deviceSocket.ReceiveTimeout = timeout;
                    deviceStream = deviceSocket.GetStream();
                    this.stayConnected = stayConnected;

                    currentConnectionIdx = idx;
                }
                catch
                {
                    DisposeStreamSocket();
                    return (int)ConnectionIdx.ERROR;
                }
            }

            connectionList.Add(idx);
            return idx;
        }

        //-----------------------------------------------------------------------------------------
        public int Connect( int oldIdx = (int)ConnectionIdx.NO_IDX )
        {
            return ConnectInternal(true, oldIdx);
        }

        //-----------------------------------------------------------------------------------------
        public void Close( int connIdx )
        {
            if( ( connectionList.Remove(connIdx)
                    || freeConnectionList.Remove(connIdx) )
                && connectionList.Count == 0
                && freeConnectionList.Count == 0 )
            {
                //connection can be close if there is no other connection and when all free connection are also closed
                //additionaly connIdx must be properly removed from list - against puting random numbers
                DisposeStreamSocket();
            }

            //if this was current connection we need to change it to other if there is other avaiable
            if( currentConnectionIdx == connIdx )
            {
                if( connectionList.Count > 0 )
                {
                    currentConnectionIdx = connectionList[0];
                }
                else
                {
                    currentConnectionIdx = (int)ConnectionIdx.NO_IDX;
                }
            }
        }

        //-----------------------------------------------------------------------------------------
        public void Free( int connIdx )
        {
            if( connectionList.Remove(connIdx) )
            {
                //connection is moved to free only when it can be properly removed from connection list
                freeConnectionList.Add(connIdx);
            }

            //if this was current connection we need to change it to other if there is other avaiable
            if (currentConnectionIdx == connIdx)
            {
                if (connectionList.Count > 0)
                {
                    currentConnectionIdx = connectionList[0];
                }
                else
                {
                    currentConnectionIdx = (int)ConnectionIdx.NO_IDX;
                }
            }
        }

        //-----------------------------------------------------------------------------------------
        public int SendRaw(byte[] data, bool stayConnected = false, int connIdx = (int)ConnectionIdx.NO_IDX)
        {
            //check for connection
            if (deviceStream == null
                || connIdx == (int)ConnectionIdx.NO_IDX )
            {
                connIdx = ConnectInternal(false, connIdx);
                if (connIdx == (int)ConnectionIdx.ERROR) return connIdx;
            }

            //check that we are allowed for transmit
            while( connIdx != currentConnectionIdx )
            {
                Thread.Sleep(connectionWaitTime);
            }

            //send message
            try
            {
                deviceStream.Write(data, 0, data.Length);
                Thread.Sleep(this.sendDelay);
                if (stayConnected == false
                    && this.stayConnected == false)
                {
                    Close(connIdx);
                    connIdx = (int)ConnectionIdx.NO_IDX;
                }
            }
            catch
            {
                Close(connIdx);
                return (int)ConnectionIdx.ERROR;
            }

            return connIdx;
        }

        //-----------------------------------------------------------------------------------------
        public int SendCommand(string command, bool stayConnected = false, int connIdx = (int)ConnectionIdx.NO_IDX)
        {
            command = command + '\n';
            byte[] data = Encoding.ASCII.GetBytes(command);
            return SendRaw(data, stayConnected, connIdx);
        }

        //-----------------------------------------------------------------------------------------
        public byte[] GetRaw(int respondLength = 4096, bool stayConnected = false, int connIdx = (int)ConnectionIdx.NO_IDX)
        {
            if (deviceStream == null
                && connIdx == (int)ConnectionIdx.NO_IDX)
            {
                return new byte[0];
            }

            byte[] data = new byte[respondLength];
            try
            {
                int bytes = deviceStream.Read(data, 0, data.Length);
                Array.Resize(ref data, bytes);
                if (stayConnected == false
                    && this.stayConnected == false)
                {
                    Close( connIdx );
                }
            }
            catch
            {
                Close( connIdx );
                return new byte[0];
            }

            return data;
        }

        //-----------------------------------------------------------------------------------------
        public string GetAns(int respondLength = 4096, bool stayConnected = false, int connIdx = (int)ConnectionIdx.NO_IDX)
        {
            byte[] data = GetRaw(respondLength, stayConnected, connIdx );
            if (data.Length == 0)
            {
                return "";
            }

            string response = Encoding.ASCII.GetString(data);
            return response.Substring( 0, response.Length-1 );
        }

        //-----------------------------------------------------------------------------------------
        public string SendCommandGetAns(string command, bool stayConnected = false, int connIdx = (int)ConnectionIdx.NO_IDX)
        {
            connIdx = SendCommand(command, true, connIdx);
            if (connIdx == (int)ConnectionIdx.ERROR)
            {
                return "";
            }
            return GetAns(1024, stayConnected, connIdx);
        }

        //-----------------------------------------------------------------------------------------
        public byte[] SendCommandGetRaw(string command, int respondLength = 4096, bool stayConnected = false, int connIdx = (int)ConnectionIdx.NO_IDX)
        {
            connIdx = SendCommand(command, true, connIdx);
            if (connIdx == (int)ConnectionIdx.ERROR)
            {
                return new byte[0];
            }
            return GetRaw(respondLength, stayConnected, connIdx);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------
        private void DisposeStreamSocket()
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

        #region ENUM
        //-------------------------------------------------------------------------------------------------------------------------------------------
        //  EEEEE N   N U   U M   M
        //  E     NN  N U   U MM MM
        //  EEE   N N N U   U M M M
        //  E     N  NN U   U M   M
        //  EEEEE N   N  UUU  M   M
        //
        public enum ConnectionIdx
        {
            ACTION_ON_SOCKET    = -2,
            ERROR               = -1,
            NO_IDX              = 0           
        }

        #endregion
    }
}
