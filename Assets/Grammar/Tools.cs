namespace anim {

    using System;
    using System.Globalization;

    public class Tools
    {

        public static float parseFloat(string value)
        {
            return float.Parse(value, CultureInfo.InvariantCulture.NumberFormat);
        }
    }
}