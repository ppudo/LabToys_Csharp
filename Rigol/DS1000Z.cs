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
        private SCPI device = null;

        //-------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public DS1000Z(string ip, ushort port = 5555)
        {
            device = new SCPI(ip, port);
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout"></param>
        public void Connect(int timeout = 10000)
        {
            device.Connect(timeout);
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        public void Close()
        {
            device.Close();
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
        public void Autoscale()
        {
            device.SendCommand("AUT");
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// CLEar
        /// </summary>
        public void Clear()
        {
            device.SendCommand("CLE");
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// RUN
        /// </summary>
        public void Run()
        {
            device.SendCommand("RUN");
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// STOP
        /// </summary>
        public void Stop()
        {
            device.SendCommand("STOP");
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// SINGLE
        /// </summary>
        public void Single()
        {
            device.SendCommand("SINGLE");
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// TFORce
        /// </summary>
        public void TriggerForce()
        {
            device.SendCommand("TFOR");
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
        public void ClearDisplay()
        {
            device.SendCommand("DISP:CLE");
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// Download screenshoot from scope as bitmap
        /// DISPlay:DATA?
        /// </summary>
        /// <returns></returns>
        public byte[] GetScreenDataBitmap()
        {
            device.SendCommand("DISP:DATA?");
            string header = device.GetAns(2);                                                       //get begin of header #x - where x is length of rest of header
            int headerLen = header[1] - '0';                                                          //convert string to int
            int length = int.Parse(device.GetAns(headerLen)) + 1;                                   //get rest of heder info - this is length of bytes in stream with screen data +ending \n

            //get rest of stream data, loop reads data to end of stream in packet
            byte[] screenData = new byte[length];
            int receivedBytes = 0;
            while (receivedBytes < length)
            {
                byte[] data = device.GetRaw(length - receivedBytes);
                Array.Copy(data, 0, screenData, receivedBytes, data.Length);
                receivedBytes += data.Length;
            }

            Array.Resize(ref screenData, screenData.Length - 1);                                    //remove /n from end of stream
            return screenData;
        }

        //-----------------------------------------------------------------------------------------
        public void SaveScreenToImage(string path, ImageFormat format = ImageFormat.BITMAP)
        {
            byte[] bitmap = GetScreenDataBitmap();
            FileStream file = File.Create(path);

            if (format != ImageFormat.BITMAP)
            {
                //TO DO
            }

            file.Write(bitmap, 0, bitmap.Length);
            file.Close();
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// DISPlay:TYPE
        /// </summary>
        /// <param name="type"></param>
        public void SetDisplayType(DisplayType type = DisplayType.VECTORS)
        {
            device.SendCommand("DISP:TYPE " + DisplayTypeString[(int)type]);
        }

        //-----------------------------------------------------------------------------------------
        public DisplayType GetDisplayType()
        {
            string ans = device.SendCommandGetAns("DISP:TYPE?");
            int i = 0;
            for (i = 0; i < DisplayTypeString.Length; i++)
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
        //  III EEEEE EEEEE EEEEE    4   888   888     222         CCC   OOO  M   M M   M  OOO  N   N
        //   I  E     E     E       44  8   8 8   8   2   2       C   C O   O MM MM MM MM O   O NN  N
        //   I  EEE   EEE   EEE    4 4   888   888       2        C     O   O M M M M M M O   O N N N
        //   I  E     E     E     44444 8   8 8   8    22         C   C O   O M   M M   M O   O N  NN
        //  III EEEEE EEEEE EEEEE    4   888   888  # 22222        CCC   OOO  M   M M   M  OOO  N   N
        //

        /// <summary>
        /// *IDN?
        /// </summary>
        /// <returns></returns>
        public string[] GetIDN()
        {
            string ans = device.SendCommandGetAns("*IDN?");
            string[] idn = ans.Split(',');
            return idn;
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// *CLS
        /// </summary>
        public void ClearStatus()
        {
            device.SendCommand("*CLS");
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// *OPC
        /// </summary>
        public void EnableOperationComplete()
        {
            device.SendCommand("*OPC");
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// *OPC?
        /// </summary>
        /// <returns>Current satus of operation. If true then operation complete</returns>
        public bool IsOperationComplete()
        {
            string ans = device.SendCommandGetAns("*OPC?");
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
        public void RestoreToDefaultState()
        {
            device.SendCommand("*RST");
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// *TST?
        /// </summary>
        /// <returns></returns>
        public int SelfTest()
        {
            string ans = device.SendCommandGetAns("*TST?");
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
            BITMAP,
            JPG,
            PNG
        }

        //-----------------------------------------------------------------------------------------
        public enum DisplayType
        {
            VECTORS,
            DOTS
        }

        //---------------------------------------
        private readonly string[] DisplayTypeString = new string[]
        {
            "VECT",
            "DOTS"
        };
        #endregion
    }
}
