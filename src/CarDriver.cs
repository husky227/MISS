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
	    private GraphBuilder graphBuilder;
	    private List<Node> currentPath;
	    private Dictionary<string, Node> allNodes;
        private Dictionary<string, Space> allSpaceNodes;
	    private Node currentNode;
	    private Node targetNode;

		public unsafe CarDriver(Robot myRobot, List<Node> nodesList)
		{
            graphBuilder = new GraphBuilder(nodesList);
            FindCurrentNode();

			this.myRobot = myRobot;
            RosonLoader rl = new RosonLoader();
            rl.LoadRoson(@"..\..\..\..\WorldDefinition\SampleMap.roson");
            this.allNodes = rl.GetNodes();
            this.allSpaceNodes = rl.GetSpaces();
            Console.WriteLine("New robot attached: " + myRobot.name);
		}

	    public void SetTargetNode(Node node)
	    {
	        targetNode = node;
            CreatePath(currentNode, targetNode);
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
                //Console.WriteLine(myRobot.id + ": " + myRobot.rotation[0] + " " + myRobot.rotation[1] + " " + myRobot.rotation[2]);
		    }
		    else
		    {
                myRobot.joints[0].motorDesiredVelocity = newSpeed;
                myRobot.joints[1].motorDesiredVelocity = newSpeed;
                myRobot.joints[2].motorDesiredVelocity = newSpeed;
                myRobot.joints[3].motorDesiredVelocity = newSpeed;
		    }

            FindCurrentNode();
			return;
		}

        private void SetGraphBuilder(GraphBuilder gb)
        {
            this.graphBuilder = gb;
        }

	    private void FindCurrentNode()
	    {
            if (graphBuilder == null)
            {
                Console.WriteLine("Current node not found :<");
                return;
            }

            unsafe {
                if (myRobot == null || myRobot.position == null)
                {
                    return;
                }
                
                double* position = myRobot.position;
                double x = position[0];
                double y = position[1];
            
                Console.WriteLine("X: " + x + " Y: " + y);
                if (allNodes == null)
                {
                    return;
                }

                foreach (String key in allSpaceNodes.Keys) {
                    Space node = allSpaceNodes[key];
                    if(node.isInside(x, y)) {
                        Console.WriteLine("spacenode found! " + myRobot.name + " , " + node.Id);
                        currentNode = allNodes[node.NodeName];
                        //break;
                    }
                }
            }
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
	}
}
