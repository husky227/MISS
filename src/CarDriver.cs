/*************************************************************************
 *                                                                       *
 * This file is part of RoBOSS Simulation System,                        *
 * Copyright (C) 2004,2005 Dariusz Czyrnek, Wojciech Turek               *
 * All rights reserved.  Email: soofka@icslab.agh.edu.pl                 *
 *                                                                       *
 * RoBOSS Simulation System is free software; you can redistribute it    *
 * and/or modify it under the terms of The GNU General Public License    *
 * version 2.0 as published by the Free Software Foundation;             *
 * The text of the GNU General Public License is included with the       *
 * System in the file LICENSE.TXT.                                       *
 *                                                                       *
 * This program is distributed in the hope that it will be useful,       *
 * but WITHOUT ANY WARRANTY; without even the implied warranty of        *
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the file     *
 * LICENSE.TXT for more details.                                         *
 *                                                                       *
 *************************************************************************/
using System;
using RoBOSSCommunicator;


namespace CityDriver
{
	/// <summary>
	/// 
	/// </summary>
	public class CarDriver
	{
		public static double maxSpeed = 1;

		public Robot myRobot;

		public CarDriver(Robot myRobot)
		{
			this.myRobot = myRobot;
            Console.WriteLine("New robot attached: " + myRobot.name);

		}

		public void Refresh()
		{
			double newSpeed = maxSpeed;

            //myRobot.joints[2].motorDesiredPosition = newSpeed;
            //myRobot.joints[1].motorDesiredPosition = myRobot.joints[1].motorDesiredPosition + 1;
            //myRobot.joints[0].motorDesiredVelocity2 = newSpeed;
			myRobot.joints[0].motorDesiredVelocity = newSpeed;
			myRobot.joints[1].motorDesiredVelocity = newSpeed;
            myRobot.joints[2].motorDesiredVelocity = newSpeed;
            myRobot.joints[3].motorDesiredVelocity = newSpeed;

			return;
		}
	}
}
