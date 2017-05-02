using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwareGroup.IoTDroneDisplay.DroneControls.Common
{
    public static class Extensions
    {
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        //public double ClampValue(double value, double minValue, double maxValue)
        //{
        //    double result = value;
        //    if (result > maxValue) result = maxValue;
        //    else if (result < minValue) result = minValue;
        //    return value;
        //}
    }
}
