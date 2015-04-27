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
using System.Linq;
using RoBOSSCommunicator;


namespace CityDriver
{
	/// <summary>
	/// 
	/// </summary>
	public class CarDriver
	{
	    private Dictionary<int, CarParameters> lastParameters;
	    private DateTime lastTime;

		public static double maxSpeed = 1;
		public Robot myRobot;

	    private GraphBuilder graphBuilder;
	    private List<Node> currentPath;
	    private Dictionary<string, Node> allNodes;
	    private Node currentNode;
	    private Node targetNode;

		public CarDriver(Robot myRobot, List<Node> nodesList)
		{
            graphBuilder = new GraphBuilder(nodesList);
            FindCurrentNode();

			this.myRobot = myRobot;
            this.allNodes = new RosonLoader().GetNodes();
            FindCurrentNode();
            lastParameters = new Dictionary<int, CarParameters>();
            lastTime = DateTime.Now;
            //Console.WriteLine("New robot attached: " + myRobot.name);
		}

	    public void SetTargetNode(Node node)
	    {
	        targetNode = node;
            CreatePath(currentNode, targetNode);
	    }

	    public void RandTargetNode()
	    {
	        var rand = new Random();
	        SetTargetNode(allNodes.ElementAt(rand.Next(allNodes.Count)).Value);
	    }

		public void Refresh(Dictionary<int, CarParameters> visibleDrivers)
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
                //Console.WriteLine(myRobot.id + ": " + myRobot.rotation[0] + " " + myRobot.rotation[1] + " " + myRobot.rotation[2]);
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

	    private void FindCurrentNode()
	    {
	        Node node;
	        // TODO Piotruœ
	        currentNode = node;
	    }

	    private void CreatePath(Node start, Node end)
	    {
	        String position = start.Name;
            String target = end.Name;
            List<String> path = graphBuilder.GetGraph().shortest_path(position, target);
            currentPath = new List<Node>();
	        
            foreach (String nodeName in path)
	        {
	            currentPath.Add(allNodes[nodeName]);
	        }

	    }

	    private void MakeNextStep(Dictionary<int, CarParameters> visibleDrivers)
	    {
	        if (currentNode == targetNode)
	        {
	            return;
	        }

	        UpdateVisibleDriversParameters(visibleDrivers);
	    }

	    private void UpdateVisibleDriversParameters(Dictionary<int, CarParameters> visibleDrivers)
	    {
	        foreach (var driver in lastParameters)
	        {
	            if (!visibleDrivers.ContainsKey(driver.Key))
	            {
	                lastParameters.Remove(driver.Key);
	            }
	        }

	        DateTime currentTime = DateTime.Now;
	        TimeSpan delta = lastTime - currentTime;
	        foreach (var driver in visibleDrivers)
	        {
	            CarParameters value;
	            if (lastParameters.TryGetValue(driver.Key, out value))
	            {
	                value.SetNewPosition(driver.Value.Position, delta);
	                value.SetNewRotation(driver.Value.Rotation, delta);
	            }
	            else
	            {
	                lastParameters.Add(driver.Key, driver.Value);
	            }
	        }
	    }
	}
}
