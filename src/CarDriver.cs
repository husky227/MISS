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
        private const double ConstAlfa = 0.4;
        private const double ConstDistance = 0.9;
        private const double MinGap = 0.3;
        public static double maxSpeed = 0.3;
        private const double Tolerance = 0.05;

        private Dictionary<string, Node> allNodes;
        private readonly Dictionary<string, Space> allSpaceNodes;
        private List<Wall> allWalls;
        private GraphBuilder graphBuilder;
//        private readonly Dictionary<int, CarParameters> lastParameters;
        private Node currentNode;
        private Node firstNode;
        private List<Node> currentPath;
        public double DeltaVelocity;
        private DateTime lastTime;
        public Robot myRobot;
        private double rotation;
        private Node targetNode;
        public double Velocity;

        private bool avoidingCollision;
        private double avoidingVelocity;
        private double avoidingDeltaVelocity;

        public CarDriver(Robot myRobot, List<Node> nodesList, RosonLoader rl)
        {
            allNodes = new Dictionary<string, Node>(rl.GetNodes());
            allSpaceNodes = rl.GetSpaces();
            allWalls = new List<Wall>();
            allWalls.AddRange(rl.GetWalls().Values);

            Console.WriteLine("Number of walls: " + allWalls.Count);

            this.myRobot = myRobot;
            graphBuilder = new GraphBuilder(nodesList);
            FindCurrentNode(rl.robots);
            firstNode = currentNode;


//            lastParameters = new Dictionary<int, CarParameters>();
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

        public void SetTargetPoint(double x, double y)
        {
            Node target = new Node(NodeKind.SpaceNode, "target", x, y);
            target.Nodes.Add(firstNode);
            firstNode.Nodes.Remove(currentNode);
            firstNode.Nodes.Add(target);
            allNodes.Remove("target");
            allNodes.Add("target", target);
            graphBuilder = new GraphBuilder(allNodes.Values.ToList());
            currentNode = firstNode;
            SetTargetNode(target);
        }

        public void RandTargetPoint()
        {
            var rand = new Random();
            double x = rand.NextDouble() * 3.2 + 0.4;
            double y = rand.NextDouble() * 3.2 + 0.4;
            SetTargetPoint(x, y);
        }

        public void Move()
        {
            if (avoidingCollision)
            {
                myRobot.joints[0].motorDesiredVelocity = (avoidingVelocity + Velocity + avoidingDeltaVelocity + DeltaVelocity) / Radius;
                myRobot.joints[1].motorDesiredVelocity = (avoidingVelocity + Velocity - avoidingDeltaVelocity - DeltaVelocity) / Radius;
                myRobot.joints[2].motorDesiredVelocity = (avoidingVelocity + Velocity + avoidingDeltaVelocity + DeltaVelocity) / Radius;
                myRobot.joints[3].motorDesiredVelocity = (avoidingVelocity + Velocity - avoidingDeltaVelocity - DeltaVelocity) / Radius;
            }
            else
            {
                myRobot.joints[0].motorDesiredVelocity = (Velocity + DeltaVelocity) / Radius;
                myRobot.joints[1].motorDesiredVelocity = (Velocity - DeltaVelocity) / Radius;
                myRobot.joints[2].motorDesiredVelocity = (Velocity + DeltaVelocity) / Radius;
                myRobot.joints[3].motorDesiredVelocity = (Velocity - DeltaVelocity) / Radius;
            }
            
            //            Console.WriteLine((Velocity + DeltaVelocity) / Radius + "   " + (Velocity - DeltaVelocity) / Radius);
        }

        public void Refresh(Dictionary<int, CarParameters> visibleDrivers)
        {
            var newSpeed = maxSpeed;

            //            if (myRobot.id == 0)
            //                Console.WriteLine(myRobot.position[0] + " " + myRobot.position[1]);

            MakeNextStep(visibleDrivers);

            Move();

//            FindCurrentNode();

            CheckForCollision();
        }

        private unsafe void CheckForCollision()
        {
            Point end = currentNode.Position;
            Point start = new Point(myRobot.position[0], myRobot.position[1]);

            List<Wall> intersectingWalls = new List<Wall>();

            foreach (Wall wall in allWalls)
            {
                Point wallStart = wall.From;
                Point wallEnd = wall.To;

                if(Helpers.doesIntersect(start, end, wallStart, wallEnd))
                {
                    Console.WriteLine("Found intersecting walls!");
                    intersectingWalls.Add(wall);
                    Console.WriteLine("przecina");
                }
            }

            //list of points above intersecting walls
            List<Pair> pointsList = new List<Pair>();
            foreach (Wall wall in intersectingWalls)
            {
                Point[] points = Helpers.boundingPoints(wall.From, wall.To);
                foreach (Point point in points)
                {
                    double dist = Helpers.CountDistance(point, start);
                    Pair pointPair = new Pair(point, dist);
                    pointsList.Add(pointPair);
                }
            }

            if (pointsList.Count > 0)
            {
                Console.WriteLine("Obstacle wall found. Found " + pointsList.Count + " alternative points");
                pointsList.Sort();
                Node currNode = new Node(NodeKind.GateNode, "", pointsList[0].First.X, pointsList[0].First.Y);
                currentNode = currNode;
            }
        }

        private void FindCurrentNode(Dictionary<string, Capo> robots)
        {
            foreach (var robot in robots)
                unsafe
                {
                    if (Math.Abs(robot.Value.X - myRobot.position[0]) < Tolerance &&
                        Math.Abs(robot.Value.Y - myRobot.position[1]) < Tolerance)
                    {
                        currentNode = robot.Value.Node;
                        return;
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
                currentPath.Insert(0, allNodes[nodeName]);
            }
            currentPath.Insert(0, start);
            foreach (var node in currentPath)
            {
                Console.WriteLine(node.Id);
            }
        }

        private unsafe void MakeNextStep(Dictionary<int, CarParameters> visibleDrivers)
        {
            if (currentPath.Count < 2)
            {
                RandTargetPoint();
            }
            if (currentNode == targetNode)
            {
                RandTargetPoint();
                return;
            }
            var distance = CountDistance(myRobot.position[0], myRobot.position[1]);
            if (distance < 0.2)
            {
//                Console.WriteLine("jest punkt " + currentNode.Id);
                currentNode = GetNextNode();
                return;
            }

            var currentTime = DateTime.Now;
            var deltaTime = currentTime - lastTime;
            lastTime = currentTime;
//            UpdateVisibleDriversParameters(visibleDrivers, deltaTime);

            rotation = CountRotation(myRobot.rotation);
            //            Console.WriteLine(rotation);
            var alfa = CountAlfa(myRobot.position[0], myRobot.position[1], rotation);

            DeltaVelocity = (alfa * ConstAlfa);
            Velocity = maxSpeed;
        }

        private double CountAlfa(double x, double y, double rotation)
        {
            var angle = 
                    (Vector.AngleBetween(new Vector(1, 0),
                        new Vector(GetNextNode().Position.X - x, GetNextNode().Position.Y - y)) * Math.PI / 180) - Math.PI / 2;
            angle = angle > Math.PI ? angle - 2 * Math.PI : angle < -Math.PI ? angle + 2 * Math.PI : angle;
//            Console.WriteLine(angle + " \t" + rotation + " \t" + (angle - rotation));
            angle -= rotation;
            angle = angle > Math.PI ? angle - 2 * Math.PI : angle < -Math.PI ? angle + 2 * Math.PI : angle;
            return angle;
        }

        private Node GetNextNode()
        {
            return currentPath[currentPath.IndexOf(currentNode) + 1];
        }

        private double CountDistance(double x, double y)
        {
            return
                Math.Sqrt((GetNextNode().Position.X - x) * (GetNextNode().Position.X - x) +
                          (GetNextNode().Position.Y - y) * (GetNextNode().Position.Y - y));
        }

        public unsafe double CountRotation(double* rotationQuaternion)
        {
//            return w * z > 0 ? Math.Asin(z) : (Math.Asin(z) < 0 ? Math.Asin(z) : Math.Asin(z) + Math.PI);
            double test = rotationQuaternion[1] * rotationQuaternion[3] + rotationQuaternion[2] * rotationQuaternion[0];
            if (test > 0.4999)
            { // singularity at north pole
                return 2 * Math.Atan2(rotationQuaternion[1], rotationQuaternion[0]);
            }
            if (test < -0.4999)
            { // singularity at south pole
                return -2 * Math.Atan2(rotationQuaternion[1], rotationQuaternion[0]);
            }
            double sqx = rotationQuaternion[1] * rotationQuaternion[1];
            double sqy = rotationQuaternion[3] * rotationQuaternion[3];
            double sqz = rotationQuaternion[2] * rotationQuaternion[2];
            return Math.Atan2(2 * rotationQuaternion[3] * rotationQuaternion[0] - 2 * rotationQuaternion[1] * rotationQuaternion[2], 1 - 2 * sqy - 2 * sqz);
        }

        public List<Node> GetPath()
        {
            return currentPath;
        }

        //return true if Capo a is on right of Capo b
        //http://stackoverflow.com/questions/1560492/how-to-tell-whether-a-point-is-to-the-right-or-left-side-of-a-line
        public bool isFirstRight(Robot a, Robot b)
        {
            unsafe
            {
                Point start = new Point(a.position[0], a.position[1]);
                double rotation = CountRotation(a.rotation);

                Point end = new Point(start.X + a.lineralVelocity[0], start.Y + a.lineralVelocity[1]);

                Point posB = new Point(b.position[0], b.position[1]);
                return ((end.X - start.X) * (posB.Y - start.Y) - (end.Y - start.Y) * (posB.X - start.X)) <= 0;
            }
        }

        public void StartAvoidingCollision(double aV, double aDV)
        {
            avoidingCollision = true;
            avoidingVelocity = aV;
            avoidingDeltaVelocity = aDV;
        }

        public void StopAvoidingCollision()
        {
            avoidingCollision = false;
            avoidingVelocity = 0.0;
            avoidingDeltaVelocity = 0.0;
        }
    }
}