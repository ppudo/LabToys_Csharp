using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

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
            device.Timeout = 3000;
        }

        #region VARIABLES
        //-------------------------------------------------------------------------------------------------------------------------------------------
        //  V   V   A   RRRR  III   A   BBBB  L     EEEEE  SSS 
        //  V   V  A A  R   R  I   A A  B   B L     E     S    
        //  V   V A   A RRRR   I  A   A BBBB  L     EEE    SSS 
        //   V V  AAAAA R R    I  AAAAA B   B L     E         S
        //    V   A   A R  R  III A   A BBBB  LLLLL EEEEE  SSS 
        private DeviceStatus status = new DeviceStatus();

        //-----------------------------------------------------------------------------------------
        public DeviceStatus Status { get => status; }

        #endregion

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
            return device.SendCommand("*PUD " + info) == (int)SCPIsocket.ConnectionIdx.NO_IDX ? true : false;
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
                return device.SendCommand("*SAV " + password) == (int)SCPIsocket.ConnectionIdx.NO_IDX ? true : false;
            }
            else
            {
                return device.SendCommand("*SAV") == (int)SCPIsocket.ConnectionIdx.NO_IDX ? true : false;
            }
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
        /// *RCL
        /// </summary>
        public bool RecallCalibration()
        {
            return device.SendCommand("*RCL") == (int)SCPIsocket.ConnectionIdx.NO_IDX ? true : false;
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
            return device.SendCommand("SOUR:VOLT:MAX " + max.ToString("0.0000")) == (int)SCPIsocket.ConnectionIdx.NO_IDX ? true : false;
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
            return device.SendCommand("SOUR:CURR:MAX " + max.ToString("0.0000")) == (int)SCPIsocket.ConnectionIdx.NO_IDX ? true : false;
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
            return device.SendCommand("SOUR:VOLT " + voltage.ToString("0.0000")) == (int)SCPIsocket.ConnectionIdx.NO_IDX ? true : false;
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
            return device.SendCommand("SOUR:CURR " + current.ToString("0.0000")) == (int)SCPIsocket.ConnectionIdx.NO_IDX ? true : false;
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
        private float MeasureOutputVoltage( int connIdx )
        {
            string ans = device.SendCommandGetAns("MEAS:VOLT?", 1024, false, connIdx);
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
        public float MeasureOutputVoltage()
        {
            return MeasureOutputVoltage((int)SCPIsocket.ConnectionIdx.NO_IDX);
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// MEASure:CURRent?
        /// </summary>
        /// <returns></returns>
        private float MeasureOutputCurrent( int connIdx )
        {
            string ans = device.SendCommandGetAns("MEAS:CURR?", 1024, false, connIdx);
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
        public float MeasureOutputCurrent()
        {
            return MeasureOutputCurrent((int)SCPIsocket.ConnectionIdx.NO_IDX);
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
            return device.SendCommand("UOUT " + outputs.ToString()) == (int)SCPIsocket.ConnectionIdx.NO_IDX ? true : false;
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// UOUTput?
        /// </summary>
        /// <returns></returns>
        private int GetDigitalOutputs( int connIdx )
        {
            string ans = device.SendCommandGetAns("UOUT?", 1024, false, connIdx);
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
        public int GetDigitalOutputs()
        {
            return GetDigitalOutputs((int)SCPIsocket.ConnectionIdx.NO_IDX);
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// UINPut:CONDition?
        /// </summary>
        /// <returns></returns>
        private int GetDigitalInputs( int connIdx )
        {
            string ans = device.SendCommandGetAns("UINP:COND?", 1024, false, connIdx);
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
        public int GetDigitalInputs()
        {
            return GetDigitalInputs((int)SCPIsocket.ConnectionIdx.NO_IDX);
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
            return device.SendCommand("SYST:FRON 1") == (int)SCPIsocket.ConnectionIdx.NO_IDX ? true : false;
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// SYSTem:FRONtpanel[:STATus] 0 or OFF
        /// </summary>
        public bool UnlockFrontPanel()
        {
            return device.SendCommand("SYST:FRON 0") == (int)SCPIsocket.ConnectionIdx.NO_IDX ? true : false;
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

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// SYSTem:REMote[:STATus] REMote
        /// SYSTem:REMote[:STATus] LOCal
        /// </summary>
        /// <returns></returns>
        public bool SetRemoteMode( RemoteStatus status )
        {
            if( status == RemoteStatus.REMOTE )
            {
                return device.SendCommand("SYST:REM REM") == (int)SCPIsocket.ConnectionIdx.NO_IDX ? true : false;
            }
            else
            {
                return device.SendCommand("SYST:REM LOC") == (int)SCPIsocket.ConnectionIdx.NO_IDX ? true : false;
            }
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// SYSTem:REMote[:STATus]?
        /// </summary>
        /// <returns></returns>
        public RemoteStatus GetRemoteMode()
        {
            string ans = device.SendCommandGetAns("SYST:REM?");
            if (ans.Length == 0) return RemoteStatus.ERROR;

            if( ans.Contains("REM") )
            {
                return RemoteStatus.REMOTE;
            }
            else if( ans.Contains("LOC") )
            {
                return RemoteStatus.LOCAL;
            }
            else
            {
                return RemoteStatus.ERROR;
            }
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// SYSTem:REMote:CV[:STATus] REMote
        /// SYSTem:REMote:CV[:STATus] LOCal
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool SetRemoteVoltage(RemoteStatus status)
        {
            if (status == RemoteStatus.REMOTE)
            {
                return device.SendCommand("SYST:REM:CV REM") == (int)SCPIsocket.ConnectionIdx.NO_IDX ? true : false;
            }
            else
            {
                return device.SendCommand("SYST:REM:CV LOC") == (int)SCPIsocket.ConnectionIdx.NO_IDX ? true : false;
            }
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// SYSTem:REMote:CV[:STATus]?
        /// </summary>
        /// <returns></returns>
        public RemoteStatus GetRemoteVoltage()
        {
            string ans = device.SendCommandGetAns("SYST:REM:CV?");
            if (ans.Length == 0) return RemoteStatus.ERROR;

            if (ans.Contains("REM"))
            {
                return RemoteStatus.REMOTE;
            }
            else if (ans.Contains("LOC"))
            {
                return RemoteStatus.LOCAL;
            }
            else
            {
                return RemoteStatus.ERROR;
            }
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// SYSTem:REMote:CC[:STATus] REMote
        /// SYSTem:REMote:CC[:STATus] LOCal
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool SetRemoteCurrent(RemoteStatus status)
        {
            if (status == RemoteStatus.REMOTE)
            {
                return device.SendCommand("SYST:REM:CC REM") == (int)SCPIsocket.ConnectionIdx.NO_IDX ? true : false;
            }
            else
            {
                return device.SendCommand("SYST:REM:CC LOC") == (int)SCPIsocket.ConnectionIdx.NO_IDX ? true : false;
            }
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// SYSTem:REMote:CC[:STATus]?
        /// </summary>
        /// <returns></returns>
        public RemoteStatus GetRemoteCurrent()
        {
            string ans = device.SendCommandGetAns("SYST:REM:CC?");
            if (ans.Length == 0) return RemoteStatus.ERROR;

            if (ans.Contains("REM"))
            {
                return RemoteStatus.REMOTE;
            }
            else if (ans.Contains("LOC"))
            {
                return RemoteStatus.LOCAL;
            }
            else
            {
                return RemoteStatus.ERROR;
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
            return device.SendCommand("OUTP 1") == (int)SCPIsocket.ConnectionIdx.NO_IDX ? true : false;
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// OUTPut
        /// </summary>
        public bool DisableOutput()
        {
            return device.SendCommand("OUTP 0") == (int)SCPIsocket.ConnectionIdx.NO_IDX ? true : false;
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
        private bool SelectSequence(string name, int connIdx)
        {
            if (name.Length > 16)
            {
                name = name.Substring(0, 16);
            }
            return device.SendCommand("PROG:SEL:NAME " + name, false, connIdx) == connIdx ? true : false;
        }

        //-----------------------------------------------------------------------------------------
        public bool SelectSequence(string name)
        {
            return SelectSequence(name, (int)SCPIsocket.ConnectionIdx.NO_IDX);
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// PROGram:SELected:NAME
        /// </summary>
        /// <returns></returns>
        private string GetSelectedSequenceName( int connIdx)
        {
            string ans = device.SendCommandGetAns("PROG:SEL:NAME?", 1024, false, connIdx);
            if (ans.Length == 0) return "";
            return ans;
        }

        //-----------------------------------------------------------------------------------------
        public string GetSelectedSequenceName()
        {
            return GetSelectedSequenceName((int)SCPIsocket.ConnectionIdx.NO_IDX);
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// PROGram:SELected:STEP
        /// </summary>
        /// <param name="stepNo"></param>
        /// <returns></returns>
        private string GetSequenceStep(int stepNo, int connIdx)
        {
            if (stepNo <= 2000 && stepNo >= 1)
            {
                string ans = device.SendCommandGetAns("PROG:SEL:STEP " + stepNo.ToString() + "?", 1024, false, connIdx);
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
        public string GetSequenceStep( int stepNo )
        {
            return GetSequenceStep(stepNo, (int)SCPIsocket.ConnectionIdx.NO_IDX);
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// #PROGram:SELected:STEP
        /// </summary>
        /// <param name="stepNo"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        private bool SetSequenceStep( int stepNo, string step, int connIdx )
        {
            if( stepNo <= 2000 && stepNo >= 1 )
            {
                return device.SendCommand("PROG:SEL:STEP " + stepNo.ToString() + " " + step, false, connIdx) == connIdx ? true : false;
            }
            else
            {
                return false;
            }
        }

        public bool SetSequenceStep(int stepNo, string step)
        {
            return SetSequenceStep(stepNo, step, (int)SCPIsocket.ConnectionIdx.NO_IDX);
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
            int connIdx = device.Connect();
            if (connIdx == (int)SCPIsocket.ConnectionIdx.ERROR) return new string[0];
            while ( !step.Contains("END") )
            {
                step = GetSequenceStep(idx, connIdx);
                if (step.Length == 0)
                {
                    break;
                }
                steps[idx - 1] = step;
                idx++;
            }
            device.Close( connIdx );
            Array.Resize(ref steps, idx - 1);
            return steps;
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// PROGram:SELected:DELete
        /// </summary>
        /// <returns></returns>
        private bool DeleteSelectedSequence( int connIdx )
        {
            return device.SendCommand("PROG:SEL:DEL", false, connIdx) == connIdx ? true : false;
        }

        //-----------------------------------------------------------------------------------------
        public bool DeleteSelectedSequence()
        {
            return DeleteSelectedSequence((int)SCPIsocket.ConnectionIdx.NO_IDX);
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// PROGram:SELected:STATe RUN
        /// </summary>
        /// <returns></returns>
        public bool StartSequence()
        {
            return device.SendCommand("PROG:SEL:STAT RUN") == (int)SCPIsocket.ConnectionIdx.NO_IDX ? true : false;
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// PROGram:SELected:STATe PAUSe
        /// </summary>
        /// <returns></returns>
        public bool PauseSequence()
        {
            return device.SendCommand("PROG:SEL:STAT PAUS") == (int)SCPIsocket.ConnectionIdx.NO_IDX ? true : false;
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// PROGram:SELected:STATe CONTinue
        /// </summary>
        /// <returns></returns>
        public bool ContinueSequence()
        {
            return device.SendCommand("PROG:SEL:STAT CONT") == (int)SCPIsocket.ConnectionIdx.NO_IDX ? true : false;
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// PROGram:SELected:STATe NEXT
        /// </summary>
        /// <returns></returns>
        public bool NextStep()
        {
            return device.SendCommand("PROG:SEL:STAT NEXT") == (int)SCPIsocket.ConnectionIdx.NO_IDX ? true : false;
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// PROGram:SELected:STATe STOP
        /// </summary>
        /// <returns></returns>
        public bool StopSequence()
        {
            return device.SendCommand("PROG:SEL:STAT STOP") == (int)SCPIsocket.ConnectionIdx.NO_IDX ? true : false;
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// PROGram:SELected:STATe
        /// </summary>
        /// <returns></returns>
        private SequenceStatus GetSequenceStatus( int connIdx )
        {
            return new SequenceStatus(device.SendCommandGetAns("PROG:SEL:STAT?", 1024, false, connIdx));
        }

        //-----------------------------------------------------------------------------------------
        public SequenceStatus GetSequenceStatus()
        {
            return GetSequenceStatus((int)SCPIsocket.ConnectionIdx.NO_IDX);
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// TRIGger:IMMediate
        /// </summary>
        /// <returns></returns>
        public bool TriggerStep()
        {
            return device.SendCommand("TRIG:IMM") == (int)SCPIsocket.ConnectionIdx.NO_IDX ? true : false;
        }

        //-----------------------------------------------------------------------------------------
        public bool SendSequence( string name, string[] steps )
        {
            int connIdx = device.Connect();
            if ( connIdx == (int)SCPIsocket.ConnectionIdx.ERROR ) return false;

            //delete current sequence with the same name
            if (SelectSequence(name, connIdx) == false) return false;
            if (DeleteSelectedSequence(connIdx) == false) return false;

            //select sequence again and upload new sequence
            if (SelectSequence(name, connIdx) == false) return false;
            for( int i=0; i<steps.Length; i++ )
            {
                if (SetSequenceStep(i + 1, steps[i], connIdx) == false) return false;
            }

            device.Close( connIdx );
            return true;
        }
        #endregion

        #region ADDITIONAL_FUNCTIONS
        //-------------------------------------------------------------------------------------------------------------------------------------------
        //    A   DDDD  DDDD  III TTTTT III  OOO  N   N   A   L           EEEEE U   U N   N  CCC  TTTTT III  OOO  N   N  SSS 
        //   A A  D   D D   D  I    T    I  O   O NN  N  A A  L           E     U   U NN  N C   C   T    I  O   O NN  N S
        //  A   A D   D D   D  I    T    I  O   O N N N A   A L           EEE   U   U N N N C       T    I  O   O N N N  SSS
        //  AAAAA D   D D   D  I    T    I  O   O N  NN AAAAA L           E     U   U N  NN C   C   T    I  O   O N  NN     S
        //  A   A DDDD DDDD   III   T   III  OOO  N   N A   A LLLLL       E      UUU  N   N  CCC    T   III  OOO  N   N  SSS

        private int refreshConnIdx = (int)SCPIsocket.ConnectionIdx.NO_IDX;

        public bool RefreshDeviceStatus( bool closeConnection=true )
        {
            //start connection or restore from free
            refreshConnIdx = device.Connect(refreshConnIdx);
            if (refreshConnIdx == (int)SCPIsocket.ConnectionIdx.ERROR)
            {
                refreshConnIdx = (int)SCPIsocket.ConnectionIdx.NO_IDX;
                return false;
            }

            //get status
            status.measuredVoltage = MeasureOutputVoltage( refreshConnIdx );
            status.measuredCurrent = MeasureOutputCurrent( refreshConnIdx );
            status.CalculateOutputPower();

            status.digitalOutputs = GetDigitalOutputs( refreshConnIdx );
            status.digitalInputs = GetDigitalInputs( refreshConnIdx );

            status.selectedSequence = GetSelectedSequenceName( refreshConnIdx );
            if(status.selectedSequence != "" )
            {
                status.sequenceStatus = GetSequenceStatus( refreshConnIdx );
            }

            //close connection or go to free
            if( closeConnection )
            {
                device.Close( refreshConnIdx );
                refreshConnIdx = (int)SCPIsocket.ConnectionIdx.NO_IDX;
            }
            else
            {
                device.Free( refreshConnIdx );
            }

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

        //-----------------------------------------------------------------------------------------
        public enum RemoteStatus
        {
            REMOTE,
            LOCAL,
            ERROR
        }

        #endregion

        #region CLASSES
        //-------------------------------------------------------------------------------------------------------------------------------------------
        //   CCC  L       A    SSS   SSS  EEEEE  SSS
        //  C   C L      A A  S     S     E     S
        //  C     L     A   A  SSS   SSS  EEE    SSS
        //  C   C L     AAAAA     S     S E         S
        //   CCC  LLLLL A   A  SSS   SSS  EEEEE  SSS
        //

        public class SequenceStatus
        {
            private SequenceState state = SequenceState.ERROR;
            private int idx = 0;

            //------------------------------------------------------------
            public SequenceState State { get => state; }
            public int Idx { get => idx; }

            //------------------------------------------------------------
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
        public class DeviceStatus
        {
            protected internal float measuredVoltage = float.NaN;
            protected internal float measuredCurrent = float.NaN;
            protected internal float outputPower = float.NaN;

            protected internal int digitalOutputs = int.MinValue;
            protected internal int digitalInputs = int.MinValue;

            protected internal string selectedSequence = "";
            protected internal SequenceStatus sequenceStatus = null;

            //------------------------------------------------------------
            public float MeasuredVoltage { get => measuredVoltage; }
            public float MeasuredCurrent { get => measuredCurrent; }
            public float OutputPower { get => outputPower; }

            public int DigitalOutputs { get => digitalOutputs; }
            public int DigitalInputs { get => digitalInputs; }

            public string SelectedSequence { get => selectedSequence; }
            public SequenceStatus SequenceStatus { get => sequenceStatus; }

            //------------------------------------------------------------
            public DeviceStatus()
            {
                
            }

            //------------------------------------------------------------
            public void CalculateOutputPower()
            {
                if (!float.IsNaN(measuredVoltage)
                && !float.IsNaN(measuredCurrent))
                {
                    outputPower = measuredVoltage * measuredCurrent;
                }
                else
                {
                    outputPower = float.NaN;
                }
            }
        }

        #endregion
    }
}
