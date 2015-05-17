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
        private const double Radius = 0.0603;
        private const double ConstAlfa = 0.1;
        private const double ConstDistance = 0.3;
        public static double maxSpeed = 0.3;
        private readonly Dictionary<string, Node> allNodes;
        private readonly Dictionary<string, Space> allSpaceNodes;
        private readonly GraphBuilder graphBuilder;
        private readonly Dictionary<int, CarParameters> lastParameters;
        private Node currentNode;
        private List<Node> currentPath;
        public double DeltaVelocity;
        private DateTime lastTime;
        public Robot myRobot;
        private double rotation;
        private Node targetNode;
        public double Velocity;

        public CarDriver(Robot myRobot, List<Node> nodesList)
        {
            var rl = new RosonLoader();
            rl.LoadRoson(@"..\..\..\..\WorldDefinition\Mapa.roson");
            allNodes = rl.GetNodes();
            allSpaceNodes = rl.GetSpaces();

            this.myRobot = myRobot;
            graphBuilder = new GraphBuilder(nodesList);
            FindCurrentNode();

            lastParameters = new Dictionary<int, CarParameters>();
            //            Console.WriteLine("New robot attached: " + myRobot.name);
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
            myRobot.joints[0].motorDesiredVelocity = (Velocity + DeltaVelocity) / Radius;
            myRobot.joints[1].motorDesiredVelocity = (Velocity - DeltaVelocity) / Radius;
            myRobot.joints[2].motorDesiredVelocity = (Velocity + DeltaVelocity) / Radius;
            myRobot.joints[3].motorDesiredVelocity = (Velocity - DeltaVelocity) / Radius;
            //            Console.WriteLine((Velocity + DeltaVelocity) / Radius + "   " + (Velocity - DeltaVelocity) / Radius);
        }

        public void Refresh(Dictionary<int, CarParameters> visibleDrivers)
        {
            var newSpeed = maxSpeed;

            //            if (myRobot.id == 0)
            //                Console.WriteLine(myRobot.position[0] + " " + myRobot.position[1]);

            MakeNextStep(visibleDrivers);

            Move();

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

                //                Console.WriteLine("X: " + x + " Y: " + y);
                if (allNodes == null)
                {
                    return;
                }

                foreach (var key in allSpaceNodes.Keys)
                {
                    var node = allSpaceNodes[key];
                    if (node.isInside(x, y))
                    {
                        //                                                Console.WriteLine("spacenode found! " + myRobot.name + " , " + node.Id);
                        currentNode = allNodes[node.NodeName];
                        //break;
                    }
                }
            }
        }

        private void CreatePath(Node start, Node end)
        {
            var position = start.Id;
            var target = end.Id;
            var path = graphBuilder.GetGraph().shortest_path(position, target);
            currentPath = new List<Node>();

            foreach (var nodeName in path)
            {
                currentPath.Add(allNodes[nodeName]);
            }
        }

        private unsafe void MakeNextStep(Dictionary<int, CarParameters> visibleDrivers)
        {
            //            foreach (var node in currentPath)
            //            {
            //                Console.WriteLine(node.Id);
            //            }
            //            
            //            Console.WriteLine(currentNode.Id + "   " + GetNextNode().Id + "    " + targetNode.Id);
            var distance = CountDistance(myRobot.position[0], myRobot.position[1]);
            if (distance < 0.2)
            {
                Velocity = 0;
                DeltaVelocity = 0;
                Console.WriteLine("jest punkt " + currentNode.Id);
                currentNode = GetNextNode();
            }
            if (currentNode == targetNode)
            {
                return;
            }

            var currentTime = DateTime.Now;
            var deltaTime = currentTime - lastTime;
            lastTime = currentTime;
            UpdateVisibleDriversParameters(visibleDrivers, deltaTime);

            rotation = CountRotation(myRobot.position[0], myRobot.position[1]);
            //            Console.WriteLine(rotation);
            var alfa = CountAlfa(myRobot.position[0], myRobot.position[1], rotation);

            DeltaVelocity = (alfa * ConstAlfa);
            Velocity = maxSpeed * (distance * ConstDistance) > maxSpeed ? maxSpeed : maxSpeed * (distance * ConstDistance);
                        Console.WriteLine(myRobot.position[0] + " \t" + myRobot.position[1] + " \t" + GetNextNode().Position.X + " \t" + GetNextNode().Position.Y + "\t" + alfa + "\t" + rotation);
            //TODO dodac wyliczanie Velocity
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

        private double CountAlfa(double x, double y, double rotation)
        {
            var angle = 2 * Math.PI -
                    (Vector.AngleBetween(new Vector(1, 0),
                        new Vector(GetNextNode().Position.X - x, GetNextNode().Position.Y - y)) * Math.PI / 180);
//            Console.WriteLine(angle + " \t" + rotation + " \t" + (angle - rotation));
            angle -= rotation;
            return angle > Math.PI ? angle - 2 * Math.PI : angle < -Math.PI ? angle + 2 * Math.PI : angle;
        }

        private Node GetNextNode()
        {
            return currentPath[(currentPath.IndexOf(currentNode) == -1 ? currentPath.Count : currentPath.IndexOf(currentNode)) - 1];
        }

        private double CountDistance(double x, double y)
        {
            return
                Math.Sqrt((GetNextNode().Position.X - x) * (GetNextNode().Position.X - x) +
                          (GetNextNode().Position.Y - y) * (GetNextNode().Position.Y - y));
        }

        private double CountRotation(double x, double y)
        {
            return 2 * Math.PI -
                    (Vector.AngleBetween(new Vector(1, 0),
                        new Vector(x, y)) * Math.PI / 180);
        }
    }
}