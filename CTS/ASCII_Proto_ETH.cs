using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabToys.CTS
{
    class ASCII_Proto_ETH
    {
        private SCPIsocket device = null;

        public ASCII_Proto_ETH( string ip, ushort port=1080 )
        {
            device = new SCPIsocket(ip, port);
            device.Timeout = 3000;
            device.LineEnding = "";
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------
        public float GetMeasuredTemp()
        {
            string ans = device.SendCommandGetAns("A0", 14, false, (int)SCPIsocket.ConnectionIdx.NO_IDX );
            if (ans.Length == 0) return float.NaN;
            if (float.TryParse(ans.Substring(3, 5), out float value))
            {
                return value;
            }
            else
            {
                return float.NaN;
            }
        }

        //-----------------------------------------------------------------------------------------
        public float ReadSetTemp()
        {
            string ans = device.SendCommandGetAns("A0", 14, false, (int)SCPIsocket.ConnectionIdx.NO_IDX);
            if (ans.Length == 0) return float.NaN;
            if (float.TryParse(ans.Substring(9, 5), out float value))
            {
                return value;
            }
            else
            {
                return float.NaN;
            }
        }

        //-----------------------------------------------------------------------------------------
        public bool SetTemp( float temp )
        {
            string command = "a0" + temp.ToString("000.0");
            string ans = device.SendCommandGetAns( command, 1, false, (int)SCPIsocket.ConnectionIdx.NO_IDX);
            if (ans.Length == 0) return false;
            if ( ans == "a" )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------
        public bool StartChamber()
        {
            string ans = device.SendCommandGetAns("s1 1", 2, false, (int)SCPIsocket.ConnectionIdx.NO_IDX);
            if (ans.Length == 0) return false;
            if (ans == "s1")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //-----------------------------------------------------------------------------------------
        public bool StopChamber()
        {
            string ans = device.SendCommandGetAns("s1 0", 2, false, (int)SCPIsocket.ConnectionIdx.NO_IDX);
            if (ans.Length == 0) return false;
            if (ans == "s1")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------
        public float ReadGradientUp()
        {
            string ans = device.SendCommandGetAns("U1", 14, false, (int)SCPIsocket.ConnectionIdx.NO_IDX);
            if (ans.Length == 0) return float.NaN;
            if (float.TryParse(ans.Substring(3, 5), out float value))
            {
                return value;
            }
            else
            {
                return float.NaN;
            }
        }

        //-----------------------------------------------------------------------------------------
        public float ReadGradientDown()
        {
            string ans = device.SendCommandGetAns("U1", 14, false, (int)SCPIsocket.ConnectionIdx.NO_IDX);
            if (ans.Length == 0) return float.NaN;
            if (float.TryParse(ans.Substring(9, 5), out float value))
            {
                return value;
            }
            else
            {
                return float.NaN;
            }
        }
    }
}
