using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Pipes;

namespace LabToys.Rigol
{
    public class DS1000Z
    {
        private SCPIsocket device = null;

        //-------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public DS1000Z(string ip, ushort port = 5555)
        {
            device = new SCPIsocket(ip, port);
            device.Timeout = 10000;
        }

        #region BASIC_COMMANDS
        //-------------------------------------------------------------------------------------------------------------------------------------------
        //  BBBB    A    SSS  III  CCC         CCC   OOO  M   M M   M   A   N   N DDDD   SSS
        //  B   B  A A  S      I  C   C       C   C O   O MM MM MM MM  A A  NN  N D   D S
        //  BBBB  A   A  SSS   I  C           C     O   O M M M M M M A   A N N N D   D  SSS
        //  B   B AAAAA     S  I  C   C       C   C O   O M   M M   M AAAAA N  NN D   D     S
        //  BBBB  A   A  SSS  III  CCC         CCC   OOO  M   M M   M A   A N   N DDDD   SSS
        //

        /// <summary>
        /// AUTOscale
        /// </summary>
        public bool Autoscale()
        {
            return device.SendCommand("AUT") == (int)SCPIsocket.ConnectionIdx.NO_IDX ? true : false;
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// CLEar
        /// </summary>
        public bool Clear()
        {
            return device.SendCommand("CLE") == (int)SCPIsocket.ConnectionIdx.NO_IDX ? true : false;
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// RUN
        /// </summary>
        public bool Run()
        {
            return device.SendCommand("RUN") == (int)SCPIsocket.ConnectionIdx.NO_IDX ? true : false;
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// STOP
        /// </summary>
        public bool Stop()
        {
            return device.SendCommand("STOP") == (int)SCPIsocket.ConnectionIdx.NO_IDX ? true : false;
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// SINGLE
        /// </summary>
        public bool Single()
        {
            return device.SendCommand("SINGLE") == (int)SCPIsocket.ConnectionIdx.NO_IDX ? true : false;
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// TFORce
        /// </summary>
        public bool TriggerForce()
        {
            return device.SendCommand("TFOR") == (int)SCPIsocket.ConnectionIdx.NO_IDX ? true : false;
        }
        #endregion

        #region DISPLAY_COMMANDS
        //-------------------------------------------------------------------------------------------------------------------------------------------
        //  DDDD  III  SSS  PPPP  L       A   Y   Y
        //  D   D  I  S     P   P L      A A   Y Y
        //  D   D  I   SSS  PPPP  L     A   A   Y
        //  D   D  I      S P     L     AAAAA   Y
        //  DDDD  III  SSS  P     LLLLL A   A   Y
        //

        /// <summary>
        /// DISPlay:CLEar
        /// </summary>
        public bool ClearDisplay()
        {
            return device.SendCommand("DISP:CLE") == (int)SCPIsocket.ConnectionIdx.NO_IDX ? true : false;
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// Download screenshoot from scope as bitmap
        /// DISPlay:DATA?
        /// </summary>
        /// <returns></returns>
        public byte[] GetScreenDataBitmap()
        {
            int connIdx = device.Connect();
            if (connIdx == (int)SCPIsocket.ConnectionIdx.ERROR) return new byte[0];

            connIdx = device.SendCommand("DISP:DATA?", false, connIdx );
            if (connIdx == (int)SCPIsocket.ConnectionIdx.ERROR) return new byte[0];

            string header = device.GetAns(2, false, connIdx);                                       //get begin of header #x - where x is length of rest of header
            if (header.Length == 0) return new byte[0];

            int headerLen = header[1] - '0';                                                        //convert string to int
            header = device.GetAns(headerLen, false, connIdx);
            if (header.Length == 0) return new byte[0];
            int length = int.Parse(header) + 1;                                                     //get rest of heder info - this is length of bytes in stream with screen data +ending \n

            //get rest of stream data, loop reads data to end of stream in packet
            byte[] screenData = new byte[length];
            int receivedBytes = 0;
            while (receivedBytes < length)
            {
                byte[] data = device.GetRaw(length - receivedBytes, false, connIdx);
                if (data.Length == 0) return new byte[0];
                Array.Copy(data, 0, screenData, receivedBytes, data.Length);
                receivedBytes += data.Length;
            }

            device.Close(connIdx);
            Array.Resize(ref screenData, screenData.Length - 1);                                    //remove /n from end of stream
            return screenData;
        }

        //-----------------------------------------------------------------------------------------
        public bool SaveScreenToImage(string path, ImageFormat format = ImageFormat.BITMAP)
        {
            //receive data from scope
            byte[] bitmap = GetScreenDataBitmap();
            if (bitmap.Length == 0) return false;

            //save to file
            try
            {
                FileStream file = File.Create(path);
                if (format != ImageFormat.BITMAP)
                {
                    //TO DO
                }

                file.Write(bitmap, 0, bitmap.Length);
                file.Close();
            }
            catch
            {
                return false;
            }

            return true;
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// DISPlay:TYPE
        /// </summary>
        /// <param name="type"></param>
        public bool SetDisplayType(DisplayType type = DisplayType.VECTORS)
        {
            return device.SendCommand("DISP:TYPE " + DisplayTypeString[(int)type]) == (int)SCPIsocket.ConnectionIdx.NO_IDX ? true : false;
        }

        //-----------------------------------------------------------------------------------------
        public DisplayType GetDisplayType()
        {
            string ans = device.SendCommandGetAns("DISP:TYPE?");
            if (ans.Length == 0) return DisplayType.ERROR;
            int i = 0;
            for (i = 0; i < DisplayTypeString.Length - 1; i++)
            {
                if (ans == DisplayTypeString[i])
                {
                    break;
                }
            }

            return (DisplayType)i;
        }
        #endregion

        #region IEEE488_2_COMMON_COMMANDS
        //-------------------------------------------------------------------------------------------------------------------------------------------
        //  III EEEEE EEEEE EEEEE    4   888   888     222         CCC   OOO  M   M M   M  OOO  N   N        CCC   OOO  M   M M   M   A   N   N DDDD   SSS
        //   I  E     E     E       44  8   8 8   8   2   2       C   C O   O MM MM MM MM O   O NN  N       C   C O   O MM MM MM MM  A A  NN  N D   D S
        //   I  EEE   EEE   EEE    4 4   888   888       2        C     O   O M M M M M M O   O N N N       C     O   O M M M M M M A   A N N N D   D  SSS
        //   I  E     E     E     44444 8   8 8   8    22         C   C O   O M   M M   M O   O N  NN       C   C O   O M   M M   M AAAAA N  NN D   D     S
        //  III EEEEE EEEEE EEEEE    4   888   888  # 22222        CCC   OOO  M   M M   M  OOO  N   N        CCC   OOO  M   M M   M A   A N   N DDDD   SSS
        //

        /// <summary>
        /// *IDN?
        /// </summary>
        /// <returns></returns>
        public string[] GetIDN()
        {
            string ans = device.SendCommandGetAns("*IDN?");
            if (ans.Length == 0) return new string[0];
            string[] idn = ans.Split(',');
            return idn;
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// *CLS
        /// </summary>
        public bool ClearStatus()
        {
            return device.SendCommand("*CLS") == (int)SCPIsocket.ConnectionIdx.NO_IDX ? true : false;
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// *OPC
        /// </summary>
        public bool EnableOperationComplete()
        {
            return device.SendCommand("*OPC") == (int)SCPIsocket.ConnectionIdx.NO_IDX ? true : false;
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// *OPC?
        /// </summary>
        /// <returns>Current satus of operation. If true then operation complete</returns>
        public bool IsOperationComplete()
        {
            string ans = device.SendCommandGetAns("*OPC?");
            if (ans.Length == 0) return false;

            if (ans == "1")
            {
                return true;
            }
            return false;
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// *RST
        /// </summary>
        public bool RestoreToDefaultState()
        {
            return device.SendCommand("*RST") == (int)SCPIsocket.ConnectionIdx.NO_IDX ? true : false;
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// *TST?
        /// </summary>
        /// <returns></returns>
        public int SelfTest()
        {
            string ans = device.SendCommandGetAns("*TST?");
            if (ans == null) return int.MinValue;
            return int.Parse(ans);
        }
        #endregion

        #region ENUM
        //-------------------------------------------------------------------------------------------------------------------------------------------
        //  EEEEE N   N U   U M   M
        //  E     NN  N U   U MM MM
        //  EEE   N N N U   U M M M
        //  E     N  NN U   U M   M
        //  EEEEE N   N  UUU  M   M
        //

        public enum ImageFormat
        {
            BITMAP
            //JPG,
            //PNG
        }

        //-----------------------------------------------------------------------------------------
        public enum DisplayType
        {
            VECTORS,
            DOTS,
            ERROR
        }

        //---------------------------------------
        private readonly string[] DisplayTypeString = new string[]
        {
            "VECT",
            "DOTS",
            "VECT"
        };
        #endregion
    }
}
