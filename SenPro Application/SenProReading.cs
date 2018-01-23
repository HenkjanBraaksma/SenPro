using System;
using System.Collections.Generic;

namespace SenPro_Application
{
    /// <summary>
    /// SenProReading is a container class for keeping a single reading
    /// from the Arduino. It contains a DateTime called DateTime and an
    /// array of doubles (one for each sensor) called Values.
    /// </summary>
    class SenProReading
    {
        public DateTime DateTime { get; set; }
        public double[] Values { get; set; }
    }
}