using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabToys.DeltaElektronika
{
    public class PSC_ETH
    {
        private SCPIsocket device = null;

        //-------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        ///
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public PSC_ETH(string ip = "10.1.0.101", ushort port = 8462)
        {
            device = new SCPIsocket(ip, port);
            device.Timeout = 2000;
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
            if (ans.Length == 0) return new string[0];
            string[] idn = ans.Split(',');
            return idn;
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// *PUD
        /// </summary>
        /// <param name="info"></param>
        public bool SetProtectedUserData(string info)
        {
            if (info.Length > 72)
            {
                info = info.Remove(0, 72);
            }
            return device.SendCommand("*PUD " + info);
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// *PUD?
        /// </summary>
        /// <returns></returns>
        public string GetProtectedUserData()
        {
            string ans = device.SendCommandGetAns("*PUD?");
            if (ans.Length == 0) return "";
            return ans;
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// *SAV
        /// </summary>
        /// <param name="password"></param>
        public bool SaveSettings(string password = "")
        {
            if (password.Length > 0)
            {
                return device.SendCommand("*SAV " + password);
            }
            else
            {
                return device.SendCommand("*SAV");
            }
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// *RST
        /// </summary>
        public bool RestoreToDefaultState()
        {
            return device.SendCommand("*RST");
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// *RCL
        /// </summary>
        public bool RecallCalibration()
        {
            return device.SendCommand("*RCL");
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
        public bool SetOutputMaxVoltage(float max)
        {
            return device.SendCommand("SOUR:VOLT:MAX " + max.ToString("0.0000"));
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// SOURce:VOLTage:MAXimum?
        /// </summary>
        /// <returns></returns>
        public float GetOutputMaxVoltage()
        {
            string ans = device.SendCommandGetAns("SOUR:VOLT:MAX?");
            if (ans.Length == 0) return float.NaN;
            if (float.TryParse(ans, out float value))
            {
                return value;
            }
            else
            {
                return float.NaN;
            }
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// SOURce:CURRent:MAXimum
        /// </summary>
        /// <param name="max"></param>
        public bool SetOutputMaxCurrent(float max)
        {
            return device.SendCommand("SOUR:CURR:MAX " + max.ToString("0.0000"));
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// SOURce:CURRent:MAXimum?
        /// </summary>
        /// <returns></returns>
        public float GetOutputMaxCurrent()
        {
            string ans = device.SendCommandGetAns("SOUR:CURR:MAX?");
            if (ans.Length == 0) return float.NaN;
            if (float.TryParse(ans, out float value))
            {
                return value;
            }
            else
            {
                return float.NaN;
            }
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// SOURce:VOLTage
        /// </summary>
        /// <param name="voltage"></param>
        public bool SetOutputVoltage(float voltage)
        {
            return device.SendCommand("SOUR:VOLT " + voltage.ToString("0.0000"));
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// SOURce:VOLTage?
        /// </summary>
        /// <returns></returns>
        public float GetOutputVoltage()
        {
            string ans = device.SendCommandGetAns("SOUR:VOLT?");
            if (ans.Length == 0) return float.NaN;
            if (float.TryParse(ans, out float value))
            {
                return value;
            }
            else
            {
                return float.NaN;
            }
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// SOURce:CURRent
        /// </summary>
        /// <param name="current"></param>
        public bool SetOutputCurrent(float current)
        {
            return device.SendCommand("SOUR:CURR " + current.ToString("0.0000"));
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// SOURce:CURRent?
        /// </summary>
        /// <returns></returns>
        public float GetOutputCurrent()
        {
            string ans = device.SendCommandGetAns("SOUR:CURR?");
            if (ans.Length == 0) return float.NaN;
            if (float.TryParse(ans, out float value))
            {
                return value;
            }
            else
            {
                return float.NaN;
            }
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
            if (ans.Length == 0) return float.NaN;
            if (float.TryParse(ans, out float value))
            {
                return value;
            }
            else
            {
                return float.NaN;
            }
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// MEASure:CURRent?
        /// </summary>
        /// <returns></returns>
        public float MeasureOutputCurrent()
        {
            string ans = device.SendCommandGetAns("MEAS:CURR?");
            if (ans.Length == 0) return float.NaN;
            if (float.TryParse(ans, out float value))
            {
                return value;
            }
            else
            {
                return float.NaN;
            }
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
        public bool SetDigitalOutputs(int outputs)
        {
            outputs = outputs & 0x3F;
            return device.SendCommand("UOUT " + outputs.ToString());
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// UOUTput?
        /// </summary>
        /// <returns></returns>
        public int GetDigitalOutputs()
        {
            string ans = device.SendCommandGetAns("UOUT?");
            if (ans.Length == 0) return int.MinValue;
            if (int.TryParse(ans, out int value))
            {
                return value;
            }
            else
            {
                return int.MinValue;
            }
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// UINPut:CONDition?
        /// </summary>
        /// <returns></returns>
        public int GetDigitalInputs()
        {
            string ans = device.SendCommandGetAns("UINP:COND?");
            if (ans.Length == 0) return int.MinValue;
            if (int.TryParse(ans, out int value))
            {
                return value;
            }
            else
            {
                return int.MinValue;
            }
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
        public bool LockFrontPanel()
        {
            return device.SendCommand("SYST:FRON 1");
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// SYSTem:FRONtpanel[:STATus] 0 or OFF
        /// </summary>
        public bool UnlockFrontPanel()
        {
            return device.SendCommand("SYST:FRON 0");
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// SYSTem:FRONtpanel[:STATus]?
        /// </summary>
        /// <returns></returns>
        public bool GetFrontPanelLockStatus()
        {
            string ans = device.SendCommandGetAns("SYST:FRON?");
            if (ans.Length == 0) return false;
            if (ans == "1")
            {
                return true;
            }
            else
            {
                return false;
            }
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
        public bool EnableOutput()
        {
            return device.SendCommand("OUTP 1");
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// OUTPut
        /// </summary>
        public bool DisableOutput()
        {
            return device.SendCommand("OUTP 0");
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// OUTPut?
        /// </summary>
        /// <returns></returns>
        public bool GetOutputStatus()
        {
            string ans = device.SendCommandGetAns("OUTP?");
            if (ans.Length == 0) return false;
            if (ans == "1")
            {
                return true;
            }
            else
            {
                return false;
            }
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

        /// <summary>
        /// PROGram:CATalog
        /// </summary>
        /// <returns></returns>
        public string[] GetSequenceCatalog()
        {
            string ans = device.SendCommandGetAns("PROG:CAT?");
            if (ans.Length == 0) return new string[0];
            string[] catalog = ans.Split('\n');
            return catalog;
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// PROGram:SELected:NAME
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool SelectSequence(string name)
        {
            if (name.Length > 16)
            {
                name = name.Substring(0, 16);
            }
            return device.SendCommand("PROG:SEL:NAME " + name);
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// PROGram:SELected:NAME
        /// </summary>
        /// <returns></returns>
        public string GetSelectedSequenceName()
        {
            string ans = device.SendCommandGetAns("PROG:SEL:NAME?");
            if (ans.Length == 0) return "";
            return ans;
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// PROGram:SELected:STEP
        /// </summary>
        /// <param name="stepNo"></param>
        /// <returns></returns>
        public string GetSequenceStep(int stepNo)
        {
            if (stepNo <= 2000 && stepNo >= 1)
            {
                string ans = device.SendCommandGetAns("PROG:SEL:STEP " + stepNo.ToString() + "?");
                if (ans.Length == 0) return "";
                int idx = ans.IndexOf(' ');
                if (idx == -1) return "";
                return ans.Substring(idx + 1);
            }
            else
            {
                return "";
            }
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// #PROGram:SELected:STEP
        /// </summary>
        /// <param name="stepNo"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public bool SetSequenceStep( int stepNo, string step )
        {
            if( stepNo <= 2000 && stepNo >= 1 )
            {
                return device.SendCommand("PROG:SEL:STEP " + stepNo.ToString() + " " + step);
            }
            else
            {
                return false;
            }
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string[] GetCompleteSequence()
        {
            int idx = 1;
            string step = "";
            string[] steps = new string[2000];
            if (device.Connect() == false) return new string[0];
            while (step != "END\n")
            {
                step = GetSequenceStep(idx);
                if (step.Length == 0)
                {
                    break;
                }
                steps[idx - 1] = step.Substring(0, step.Length - 1);
                idx++;
            }
            device.Close();
            Array.Resize(ref steps, idx - 1);
            return steps;
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// PROGram:SELected:DELete
        /// </summary>
        /// <returns></returns>
        public bool DeleteSelectedSequence()
        {
            return device.SendCommand("PROG:SEL:DEL");
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// PROGram:SELected:STATe RUN
        /// </summary>
        /// <returns></returns>
        public bool StartSequence()
        {
            return device.SendCommand("PROG:SEL:STAT RUN");
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// PROGram:SELected:STATe PAUSe
        /// </summary>
        /// <returns></returns>
        public bool PauseSequence()
        {
            return device.SendCommand("PROG:SEL:STAT PAUS");
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// PROGram:SELected:STATe CONTinue
        /// </summary>
        /// <returns></returns>
        public bool ContinueSequence()
        {
            return device.SendCommand("PROG:SEL:STAT CONT");
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// PROGram:SELected:STATe NEXT
        /// </summary>
        /// <returns></returns>
        public bool NextStep()
        {
            return device.SendCommand("PROG:SEL:STAT NEXT");
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// PROGram:SELected:STATe STOP
        /// </summary>
        /// <returns></returns>
        public bool StopSequence()
        {
            return device.SendCommand("PROG:SEL:STAT STOP");
        }

        //-----------------------------------------------------------------------------------------
        public class SequenceStatus
        {
            private SequenceState state = SequenceState.ERROR;
            private int idx = 0;

            public SequenceState State { get => state; }
            public int Idx { get => idx; }

            public SequenceStatus(string status)
            {
                if (status.Contains("STOP") == true)
                {
                    state = SequenceState.STOP;
                }
                else if (status.Contains("PAUSE") == true)
                {
                    state = SequenceState.PAUSE;
                    idx = int.Parse(status.Substring("PAUSE".Length + 1));
                }
                else if (status.Contains("RUN") == true)
                {
                    state = SequenceState.RUN;
                    idx = int.Parse(status.Substring("RUN".Length + 1));
                }
            }
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// PROGram:SELected:STATe
        /// </summary>
        /// <returns></returns>
        public SequenceStatus GetSequenceStatus()
        {
            return new SequenceStatus(device.SendCommandGetAns("PROG:SEL:STAT?"));
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// TRIGger:IMMediate
        /// </summary>
        /// <returns></returns>
        public bool TriggerStep()
        {
            return device.SendCommand("TRIG:IMM");
        }

        //-----------------------------------------------------------------------------------------
        public bool SendSequence( string name, string[] steps )
        {
            if (device.Connect() == false) return false;

            //delete current sequence with the same name
            if (SelectSequence(name) == false) return false;
            if (DeleteSelectedSequence() == false) return false;

            //select sequence again and upload new sequence
            if (SelectSequence(name) == false) return false;
            for( int i=0; i<steps.Length; i++ )
            {
                if (SetSequenceStep(i + 1, steps[i]) == false) return false;
            }

            device.Close();
            return true;
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

        //-----------------------------------------------------------------------------------------
        public enum SequenceState
        {
            STOP,
            PAUSE,
            RUN,
            ERROR
        }

        #endregion
    }
}
