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
using System.Windows;
using RoBOSSCommunicator;

namespace CityDriver
{
    /// <summary>
    /// </summary>
    public class CarDriver
    {
        public static double maxSpeed = 1;
        private readonly Dictionary<string, Node> allNodes;
        private readonly Dictionary<string, Space> allSpaceNodes;
        private readonly GraphBuilder graphBuilder;
        private readonly Dictionary<int, CarParameters> lastParameters;

        private Node currentNode;
        private List<Node> currentPath;
        private DateTime lastTime;
        public Robot myRobot;
        private double rotation;
        private Node targetNode;
        private double targetRotation;
        public double AngularVelocity;
        public double Velocity;

        public CarDriver(Robot myRobot, List<Node> nodesList)
        {
            graphBuilder = new GraphBuilder(nodesList);
            FindCurrentNode();

            this.myRobot = myRobot;
            var rl = new RosonLoader();
            rl.LoadRoson(@"..\..\..\..\WorldDefinition\SampleMap.roson");
            allNodes = rl.GetNodes();
            allSpaceNodes = rl.GetSpaces();
            lastParameters = new Dictionary<int, CarParameters>();
            Console.WriteLine("New robot attached: " + myRobot.name);
            lastTime = DateTime.Now;
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

        public void Move()
        {
            if (Velocity != 0)
            {
                if (Velocity > maxSpeed)
                {
                    Velocity = maxSpeed;
                }
                myRobot.joints[0].motorDesiredVelocity = Velocity; //lF
                myRobot.joints[1].motorDesiredVelocity = Velocity; //rF
                myRobot.joints[2].motorDesiredVelocity = Velocity; //lR
                myRobot.joints[3].motorDesiredVelocity = Velocity; //rR
            }
            else if (AngularVelocity != 0)
            {
                if (AngularVelocity > maxSpeed)
                {
                    AngularVelocity = maxSpeed;
                }
                myRobot.joints[0].motorDesiredVelocity = -AngularVelocity;
                myRobot.joints[1].motorDesiredVelocity = AngularVelocity;
                myRobot.joints[2].motorDesiredVelocity = -AngularVelocity;
                myRobot.joints[3].motorDesiredVelocity = AngularVelocity;
            }
        }

        public void Refresh(Dictionary<int, CarParameters> visibleDrivers)
        {
            var newSpeed = maxSpeed;

//            if (myRobot.id == 0)
//                Console.WriteLine(myRobot.position[0] + " " + myRobot.position[1]);

            MakeNextStep(visibleDrivers);

            Move();

            /*if (visibleDrivers.Count > 0 && myRobot.id == 14)
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
            }*/

            FindCurrentNode();
        }

        private void FindCurrentNode()
        {
            if (graphBuilder == null)
            {
                Console.WriteLine("Current node not found :<");
                return;
            }

            unsafe
            {
                if (myRobot == null || myRobot.position == null)
                {
                    return;
                }

                var position = myRobot.position;
                var x = position[0];
                var y = position[1];

                Console.WriteLine("X: " + x + " Y: " + y);
                if (allNodes == null)
                {
                    return;
                }

                foreach (var key in allSpaceNodes.Keys)
                {
                    var node = allSpaceNodes[key];
                    if (node.isInside(x, y))
                    {
                        Console.WriteLine("spacenode found! " + myRobot.name + " , " + node.Id);
                        currentNode = allNodes[node.NodeName];
                        //break;
                    }
                }
            }
        }

        private void CreatePath(Node start, Node end)
        {
            var position = start.Name;
            var target = end.Name;
            var path = graphBuilder.GetGraph().shortest_path(position, target);
            currentPath = new List<Node>();

            foreach (var nodeName in path)
            {
                currentPath.Add(allNodes[nodeName]);
            }
        }

        private unsafe void MakeNextStep(Dictionary<int, CarParameters> visibleDrivers)
        {
            if (currentNode == targetNode)
            {
                return;
            }

            var currentTime = DateTime.Now;
            var deltaTime = currentTime - lastTime;
            lastTime = currentTime;
            UpdateVisibleDriversParameters(visibleDrivers, deltaTime);
            targetRotation = 2*Math.PI - Vector.AngleBetween(new Vector(1, 0),
                new Vector(currentPath[currentPath.IndexOf(currentNode) + 1].Position.X - myRobot.position[0],
                    currentPath[currentPath.IndexOf(currentNode) + 1].Position.Y - myRobot.position[1]));
            rotation = CountRotation(myRobot.rotation[0], myRobot.rotation[3]);
            double deltaRotation = targetRotation - rotation;
            if (deltaRotation > Math.PI)
            {
                deltaRotation -= 2*Math.PI;
            }
            else if (deltaRotation < -Math.PI)
            {
                deltaRotation += 2*Math.PI;
            }

            if (deltaRotation > 0.01)
            {

            }
            else if (deltaRotation < -0.01)
            {

            }
        }

        private void UpdateVisibleDriversParameters(Dictionary<int, CarParameters> visibleDrivers, TimeSpan delta)
        {
            foreach (var driver in lastParameters)
            {
                if (!visibleDrivers.ContainsKey(driver.Key))
                {
                    lastParameters.Remove(driver.Key);
                }
            }

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

        private double CountRotation(double w, double z)
        {
            return w*z >= 0 ? Math.Asin(Math.Abs(z)) : Math.Asin(Math.Abs(z)) + Math.PI;
        }
    }
}