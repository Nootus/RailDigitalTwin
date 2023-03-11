namespace Rail.DigitalTwin.Core.Utilities
{
    public static class SpeedCalculator
    {
        private static double _maxAcceleration;
        private static double _maxDeceleration;

        static SpeedCalculator()
        {
            double u, v, t1, t2, t, s;

            // max acceleration for WP5 
            u = 110 * 5.0 / 18; // 110 kmph
            t1 = 312.1; // secs when train reached u velocity
            v = 120 * 5.0 / 18; // 120 kmph
            t2 = 402; // secs when train reached v velocity 
            t = t2 - t1;
            _maxAcceleration = (v - u) / t;

            // maxDeceleration for WP7
            u = 155 * 5.0 / 18; // 155 kmph
            s = 1.2 * 1000; // 1.2 kms
            _maxDeceleration = (u * u) / (2 * s);
        }

        // get the recommended speed for next 1 sec
        public static double GetRecommendedSpeed(double currentSpeed, double targetSpeed)
        {
            double recommendedSpeed = currentSpeed + _maxAcceleration;
            return recommendedSpeed < targetSpeed ? recommendedSpeed : targetSpeed;
        }
    }
}
