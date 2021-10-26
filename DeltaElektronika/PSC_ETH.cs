using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabToys.DeltaElektronika
{
    class PSC_ETH
    {
        private SCPI device = null;

        //-------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        ///
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public PSC_ETH(string ip, ushort port = 8462)
        {
            device = new SCPI(ip, port);
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout"></param>
        public void Connect(int timeout = 2000)
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

        #region GENERAL_INSTRUCTIONS
        //-------------------------------------------------------------------------------------------------------------------------------------------
        //   GGG  EEEEE N   N EEEEE RRRR    A   L
        //  G     E     NN  N E     R   R  A A  L
        //  G  GG EEE   N N N EEE   RRRR  A   A L
        //  G   G E     N  NN E     R R   AAAAA L
        //   GGGG EEEEE N   N EEEEE R  R  A   A LLLLL
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
        /// *PUD
        /// </summary>
        /// <param name="info"></param>
        public void SetProtectedUserData(string info)
        {
            if (info.Length > 72)
            {
                info = info.Remove(0, 72);
            }
            device.SendCommand("*PUD " + info);
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// *PUD?
        /// </summary>
        /// <returns></returns>
        public string GetProtectedUserData()
        {
            string ans = device.SendCommandGetAns("*PUD?");
            return ans;
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// *SAV
        /// </summary>
        /// <param name="password"></param>
        public void SaveSettings(string password = "")
        {
            if (password.Length > 0)
            {
                device.SendCommand("*SAV " + password);
            }
            else
            {
                device.SendCommand("*SAV");
            }
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
        /// *RCL
        /// </summary>
        public void RecallCalibration()
        {
            device.SendCommand("*RCL");
        }
        #endregion

        #region SOURCE_SUBSYSTEM
        //-------------------------------------------------------------------------------------------------------------------------------------------
        //   SSS   OOO  U   U RRRR   CCC  EEEEE
        //  S     O   O U   U R   R C   C E
        //   SSS  O   O U   U RRRR  C     EEE
        //      S O   O U   U R R   C   C E
        //   SSS   OOO   UUU  R  R   CCC  EEEEE
        //

        /// <summary>
        /// SOURce:VOLTage:MAXimum
        /// </summary>
        /// <param name="max"></param>
        public void SetOutputMaxVoltage(float max)
        {
            device.SendCommand("SOUR:VOLT:MAX " + max.ToString("0.0000"));
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// SOURce:VOLTage:MAXimum?
        /// </summary>
        /// <returns></returns>
        public float GetOutputMaxVoltage()
        {
            string ans = device.SendCommandGetAns("SOUR:VOLT:MAX?");
            return float.Parse(ans);
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// SOURce:CURRent:MAXimum
        /// </summary>
        /// <param name="max"></param>
        public void SetOutputMaxCurrent(float max)
        {
            device.SendCommand("SOUR:CURR:MAX " + max.ToString("0.0000"));
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// SOURce:CURRent:MAXimum?
        /// </summary>
        /// <returns></returns>
        public float GetOutputMaxCurrent()
        {
            string ans = device.SendCommandGetAns("SOUR:CURR:MAX?");
            return float.Parse(ans);
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// SOURce:VOLTage
        /// </summary>
        /// <param name="voltage"></param>
        public void SetOutputVoltage(float voltage)
        {
            device.SendCommand("SOUR:VOLT " + voltage.ToString("0.0000"));
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// SOURce:VOLTage?
        /// </summary>
        /// <returns></returns>
        public float GetOutputVoltage()
        {
            string ans = device.SendCommandGetAns("SOUR:VOLT?");
            return float.Parse(ans);
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// SOURce:CURRent
        /// </summary>
        /// <param name="current"></param>
        public void SetOutputCurrent(float current)
        {
            device.SendCommand("SOUR:CURR " + current.ToString("0.0000"));
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// SOURce:CURRent?
        /// </summary>
        /// <returns></returns>
        public float GetOutputCurrent()
        {
            string ans = device.SendCommandGetAns("SOUR:CURR?");
            return float.Parse(ans);
        }
        #endregion

        #region MEASURE_SUBSYSTEM
        //-------------------------------------------------------------------------------------------------------------------------------------------
        //  M   M EEEEE   A    SSS  U   U RRRR  EEEEE
        //  MM MM E      A A  S     U   U R   R E
        //  M M M EEE   A   A  SSS  U   U RRRR  EEE
        //  M   M E     AAAAA     S U   U R R   E
        //  M   M EEEEE A   A  SSS   UUU  R  R  EEEEE
        //

        /// <summary>
        /// MEASure:VOLTage?
        /// </summary>
        /// <returns></returns>
        public float MeasureOutputVoltage()
        {
            string ans = device.SendCommandGetAns("MEAS:VOLT?");
            return float.Parse(ans);
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// MEASure:CURRent?
        /// </summary>
        /// <returns></returns>
        public float MeasureOutputCurrent()
        {
            string ans = device.SendCommandGetAns("MEAS:CURR?");
            return float.Parse(ans);
        }

        //-----------------------------------------------------------------------------------------
        //commented as it's only avaibale in firmware version 3.4.0 - other on tested device
        ///// <summary>
        ///// MEASure:POWer?
        ///// </summary>
        ///// <returns></returns>
        //public float MeasureOutputPower()
        //{
        //    string ans = device.SendCommandGetAns("MEAS:POW?");
        //    return float.Parse(ans);
        //}
        #endregion

        #region DIGITAL_USER_IO
        //-------------------------------------------------------------------------------------------------------------------------------------------
        //  DDDD  III  GGG  III TTTTT   A   L
        //  D   D  I  G      I    T    A A  L
        //  D   D  I  G  GG  I    T   A   A L
        //  D   D  I  G   G  I    T   AAAAA L
        //  DDDD  III  GGGG III   T   A   A LLLLL
        //

        /// <summary>
        /// UOUTput
        /// </summary>
        /// <param name="outputs"></param>
        public void SetDigitalOutputs(byte outputs)
        {
            device.SendCommand("UOUT " + outputs.ToString());
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// UOUTput?
        /// </summary>
        /// <returns></returns>
        public byte GetDigitalOutputs()
        {
            string ans = device.SendCommandGetAns("UOUT?");
            return byte.Parse(ans);
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// UINPut:CONDition?
        /// </summary>
        /// <returns></returns>
        public byte GetDigitalInputs()
        {
            string ans = device.SendCommandGetAns("UINP:COND?");
            return byte.Parse(ans);
        }
        #endregion

        #region SYSTEM_SUBSYSTEM
        //-------------------------------------------------------------------------------------------------------------------------------------------
        //   SSS  Y   Y  SSS  TTTTT EEEEE M   M
        //  S      Y Y  S       T   E     MM MM
        //   SSS    Y    SSS    T   EEE   M M M
        //      S   Y       S   T   E     M   M
        //   SSS    Y    SSS    T   EEEEE M   M
        //

        /// <summary>
        /// SYSTem:FRONtpanel[:STATus] 1 or ON
        /// </summary>
        public void LockFrontPanel()
        {
            device.SendCommand("SYST:FRON 1");
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// SYSTem:FRONtpanel[:STATus] 0 or OFF
        /// </summary>
        public void UnlockFrontPanel()
        {
            device.SendCommand("SYST:FRON 0");
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// SYSTem:FRONtpanel[:STATus]?
        /// </summary>
        /// <returns></returns>
        public bool GetFrontPanelLockStatus()
        {
            string ans = device.SendCommandGetAns("SYST:FRON?");
            int ansInt = int.Parse(ans);
            if (ansInt == 0)
            {
                return false;
            }
            return true;
        }
        #endregion

        #region OUTPUT
        //-------------------------------------------------------------------------------------------------------------------------------------------
        //   OOO  U   U TTTTT PPPP  U   U TTTTT
        //  O   O U   U   T   P   P U   U   T
        //  O   O U   U   T   PPPP  U   U   T
        //  O   O U   U   T   P     U   U   T
        //   OOO   UUU    T   P      UUU    T
        //

        /// <summary>
        /// OUTPut
        /// </summary>
        public void EnableOutput()
        {
            device.SendCommand("OUTP 1");
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// OUTPut
        /// </summary>
        public void DisableOutput()
        {
            device.SendCommand("OUTP 0");
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// OUTPut?
        /// </summary>
        /// <returns></returns>
        public bool GetOutputStatus()
        {
            string ans = device.SendCommandGetAns("OUTP?");
            int ansInt = int.Parse(ans);
            if (ansInt == 0)
            {
                return false;
            }
            return true;
        }
        #endregion

        #region SEQUENCER
        //-------------------------------------------------------------------------------------------------------------------------------------------
        //   SSS  EEEEE  QQQ  U   U EEEEE N   N  CCC  EEEEE RRRR
        //  S     E     Q   Q U   U E     NN  N C   C E     R   R
        //   SSS  EEE   Q   Q U   U EEE   N N N C     EEE   RRRR
        //      S E     Q   Q U   U E     N  NN C   C E     R R
        //   SSS  EEEEE  QQQ   UUU  EEEEE N   N  CCC  EEEEE R  R
        //                 Q

        #endregion

        #region ENUM
        //-------------------------------------------------------------------------------------------------------------------------------------------
        //  EEEEE N   N U   U M   M
        //  E     NN  N U   U MM MM
        //  EEE   N N N U   U M M M
        //  E     N  NN U   U M   M
        //  EEEEE N   N  UUU  M   M
        //

        public enum DigitalOutputs
        {
            OUTPUT_A = 0x01,
            OUTPUT_B = 0x02,
            OUTPUT_C = 0x04,
            OUTPUT_D = 0x08,
            OUTPUT_E = 0x10,
            OUTPUT_F = 0x20
        }

        //-----------------------------------------------------------------------------------------
        public enum DigitalInputs
        {
            INPUT_A = 0x01,
            INPUT_B = 0x02,
            INPUT_C = 0x04,
            INPUT_D = 0x08,
            INPUT_E = 0x10,
            INPUT_F = 0x20,
            INPUT_G = 0x40,
            INPUT_H = 0x80
        }
        #endregion
    }
}
