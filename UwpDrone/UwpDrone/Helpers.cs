using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UwpDrone
{
    class Helpers
    {
        public static float Constrain(float value, float lower, float upper)
        {
            if (value < lower)
            {
                return lower;
            }
            else if (value > upper)
            {
                return upper;
            }

            return value;
        }

        public static ushort Constrain(ushort value, ushort lower, ushort upper)
        {
            if (value < lower)
            {
                return lower;
            }
            else if (value > upper)
            {
                return upper;
            }

            return value;
        }

        public static float ApplyDeadband(float value, float deadband)
        {
            if (Math.Abs(value) < deadband)
            {
                return value;
            }
            else if (value > 0)
            {
                return value - deadband;
            }

            return value + deadband;
        }
    }
}
