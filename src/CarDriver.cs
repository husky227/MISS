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
using System.Collections.Generic;
using RoBOSSCommunicator;


namespace CityDriver
{
	/// <summary>
	/// 
	/// </summary>
	public class CarDriver
	{
	    private List<CarParameters> lastParameters;

		public static double maxSpeed = 1;
		public Robot myRobot;

		public unsafe CarDriver(Robot myRobot, GraphBuilder gb)
		{
			this.myRobot = myRobot;
		    String position = this.myRobot.name;
		    String target = "node240";
            gb.GetGraph().shortest_path(position, target);
            Console.WriteLine("New robot attached: " + myRobot.name);
		}

		public unsafe void Refresh(List<CarParameters> visibleDrivers)
		{
			double newSpeed = maxSpeed;

//            if (myRobot.id == 0)
//                Console.WriteLine(myRobot.position[0] + " " + myRobot.position[1]);

            if (visibleDrivers.Count > 0 && myRobot.id == 14)
		    {

		        myRobot.joints[0].motorDesiredVelocity = newSpeed;
		        myRobot.joints[1].motorDesiredVelocity = -newSpeed;
		        myRobot.joints[2].motorDesiredVelocity = newSpeed;
                myRobot.joints[3].motorDesiredVelocity = -newSpeed;
                Console.WriteLine(myRobot.id + ": " + myRobot.rotation[0] + " " + myRobot.rotation[1] + " " + myRobot.rotation[2]);
		    }
		    else
		    {
                myRobot.joints[0].motorDesiredVelocity = newSpeed;
                myRobot.joints[1].motorDesiredVelocity = newSpeed;
                myRobot.joints[2].motorDesiredVelocity = newSpeed;
                myRobot.joints[3].motorDesiredVelocity = newSpeed;
		    }

			return;
		}
	}
}
