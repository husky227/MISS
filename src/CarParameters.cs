using System;
using System.Collections.Generic;
using System.Text;

namespace CityDriver
{
    public class CarParameters
    {
        public double[] position = new double[3];
        public double[] rotation = new double[3];
        public double velocity;
        public double angularVelocity;

        public unsafe CarParameters(double* position, double* rotation)
        {
            for (int i = 0; i < 3; i++)
            {
                this.position[i] = position[i];
                this.rotation[i] = rotation[i];
                velocity = 0.0;
                angularVelocity = 0.0;
            }
        }
    }
}
