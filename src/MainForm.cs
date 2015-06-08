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
using System.Threading;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Net;
using System.Windows.Forms.VisualStyles;
using Newtonsoft.Json;
using RoBOSSCommunicator;
using Newtonsoft.Json.Linq;
using CityDriver.drawing;

namespace CityDriver
{
    /// <summary>
    /// Summary description for Form1.
    /// </summary>
    public class MainForm : System.Windows.Forms.Form
    {
        private System.ComponentModel.IContainer components;

        private Communicator communicator = null;

        ArrayList drivers;
        private Thread refreshThread;
        private Label connectionStatusLabel;
        private Button connectButton;
        public TextBox portTextBox;
        public TextBox addressTextBox;
        private GroupBox controllerConnectionGroupBox;
        private ListBox listBox1;
        private Label label1;
        private Label label2;
        private Label label3;
        private Panel panel1;
		private ThreadStart refreshThreadStarter;
        static private List<Node> nodesList = new List<Node>();
        private static String _rosonPath;
        private Dictionary<int, List<Point>> futurePoints = new Dictionary<int,List<Point>>(); 
        private static DrawingManager drawingManager;

        private Robot selectedRobot = null;

        public MainForm(RosonLoader rl)
        {
            InitializeComponent();
            addressTextBox.Text = Dns.Resolve(Dns.GetHostName()).AddressList[0].ToString();
            drivers = new ArrayList();

            drawingManager = new DrawingManager(panel1, rl.GetBoundaries());
            drawingManager.draw();

            List<Wall> items = new List<Wall>();
            foreach(var value in rl.GetWalls().Values)
                items.Add(value);
            drawingManager.cleanBackground();
            drawingManager.drawWalls(items);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {

            if (communicator != null)
            {
                connectButton_Click(this, null);
            }

            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.connectionStatusLabel = new System.Windows.Forms.Label();
            this.connectButton = new System.Windows.Forms.Button();
            this.portTextBox = new System.Windows.Forms.TextBox();
            this.addressTextBox = new System.Windows.Forms.TextBox();
            this.controllerConnectionGroupBox = new System.Windows.Forms.GroupBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.controllerConnectionGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // connectionStatusLabel
            // 
            this.connectionStatusLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.connectionStatusLabel.Location = new System.Drawing.Point(8, 48);
            this.connectionStatusLabel.Name = "connectionStatusLabel";
            this.connectionStatusLabel.Size = new System.Drawing.Size(224, 16);
            this.connectionStatusLabel.TabIndex = 6;
            this.connectionStatusLabel.Text = "Simulation Controller not connected";
            // 
            // connectButton
            // 
            this.connectButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.connectButton.Location = new System.Drawing.Point(160, 16);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(72, 20);
            this.connectButton.TabIndex = 8;
            this.connectButton.Text = "connect";
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // portTextBox
            // 
            this.portTextBox.Location = new System.Drawing.Point(112, 16);
            this.portTextBox.Name = "portTextBox";
            this.portTextBox.Size = new System.Drawing.Size(48, 20);
            this.portTextBox.TabIndex = 7;
            this.portTextBox.Text = "4468";
            // 
            // addressTextBox
            // 
            this.addressTextBox.Location = new System.Drawing.Point(8, 16);
            this.addressTextBox.Name = "addressTextBox";
            this.addressTextBox.Size = new System.Drawing.Size(104, 20);
            this.addressTextBox.TabIndex = 5;
            this.addressTextBox.Text = "128.0.0.2";
            // 
            // controllerConnectionGroupBox
            // 
            this.controllerConnectionGroupBox.Controls.Add(this.addressTextBox);
            this.controllerConnectionGroupBox.Controls.Add(this.portTextBox);
            this.controllerConnectionGroupBox.Controls.Add(this.connectButton);
            this.controllerConnectionGroupBox.Controls.Add(this.connectionStatusLabel);
            this.controllerConnectionGroupBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.controllerConnectionGroupBox.Location = new System.Drawing.Point(0, 0);
            this.controllerConnectionGroupBox.Name = "controllerConnectionGroupBox";
            this.controllerConnectionGroupBox.Size = new System.Drawing.Size(240, 72);
            this.controllerConnectionGroupBox.TabIndex = 13;
            this.controllerConnectionGroupBox.TabStop = false;
            this.controllerConnectionGroupBox.Text = "Controller Connection";
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(12, 78);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(228, 316);
            this.listBox1.TabIndex = 14;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 415);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Selected robot:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 428);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Velocity: 0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 441);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 18;
            this.label3.Text = "Rotation: 0";
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(247, 13);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(759, 547);
            this.panel1.TabIndex = 19;
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(1018, 572);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.controllerConnectionGroupBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "City Driver";
            this.controllerConnectionGroupBox.ResumeLayout(false);
            this.controllerConnectionGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK) //wait until file is chosen
            {
                _rosonPath = ofd.FileName;
                Console.WriteLine("Is type ok? " + _rosonPath.EndsWith(".roson"));
                if (_rosonPath.EndsWith(".roson") != true)
                {
                    MessageBox.Show("Wrong file extension.",
                            "Important Note",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error,
                            MessageBoxDefaultButton.Button1);
                    return;
                }
                RosonLoader rl = new RosonLoader();
                rl.LoadRoson(@_rosonPath);
                Console.WriteLine("Reading roson file from: " + _rosonPath);
                nodesList.AddRange(rl.GetNodes().Values);
                //Console.ReadKey();
                Application.Run(new MainForm(rl));
            }

        }


        private void connectButton_Click(object sender, System.EventArgs e)
        {
            if (communicator != null)
            {
                refreshThread.Abort();
                drivers.Clear();
                communicator.Dispose();
                communicator = null;
                return;
            }

            communicator = new Communicator();
            connectionStatusLabel.Text = "Connecting to Controller...";
            Refresh();
            if (communicator.Connect(addressTextBox.Text, portTextBox.Text, "servo tester") < 0)
            {
                connectionStatusLabel.Text = "Unable to contact Controller";
                Refresh();
                communicator.Dispose();
                communicator = null;
                return;
            }

            connectButton.Text = "Disconnect";


            foreach (Robot robot in communicator.robots)
            {
                listBox1.Items.Add(robot.name);
            }

            refreshThreadStarter = new ThreadStart(this.RefreshThread);
            refreshThread = new Thread(refreshThreadStarter);
            refreshThread.Priority = ThreadPriority.Normal;
            refreshThread.Start();
            connectionStatusLabel.Text = "Connected to Controller";
        }

        //TODO: mess, and bangladesh type. link robots names with robot objects to avoid searching
        delegate object GetSelectedRobotCallback();
        delegate void SetRobotDataCallback(Robot robot);

        private object GetSelectedRobot()
        {
            if (this.listBox1.InvokeRequired)
            {
                GetSelectedRobotCallback d = new GetSelectedRobotCallback(GetSelectedRobot);
                return this.Invoke(d);
            }
            else
            {
                return listBox1.SelectedItem;
            }
        }

        private void SetRobotData(Robot robot)
        {
            if (this.listBox1.InvokeRequired)
            {
                SetRobotDataCallback d = new SetRobotDataCallback(SetRobotData);
                this.Invoke(d, new object[] { robot });
            }
            else
            {
                label1.Text = robot.name;
                unsafe
                {
                    label2.Text = (*robot.lineralVelocity).ToString();
                    label3.Text = (*robot.rotation).ToString();
                }

                selectedRobot = robot;
            }
        }

	    private void drawBackground(Graphics g)
	    {
            g.FillRectangle(new SolidBrush(Color.FromArgb(0, Color.Black)), panel1.DisplayRectangle);

            System.Drawing.Point[] points = new System.Drawing.Point[4];

            points[0] = new System.Drawing.Point(0, 0);
            points[1] = new System.Drawing.Point(0, panel1.Height);
            points[2] = new System.Drawing.Point(panel1.Width, panel1.Height);
            points[3] = new System.Drawing.Point(panel1.Width, 0);

            Brush brush = new SolidBrush(Color.Black);

            g.FillPolygon(brush, points);
	    }

        private void drawRobot(Graphics g, Robot currenRobot, int x, int y, int width, int height, double angle)
        {
            g.FillRectangle(new SolidBrush(Color.FromArgb(0, Color.Black)), panel1.DisplayRectangle);

            System.Drawing.Point[] points = new System.Drawing.Point[4];

            int hWidth = width/2;
            int hHeight = height/2;

            points[0] = new System.Drawing.Point(x-hWidth, y-hHeight);
            points[1] = new System.Drawing.Point(x - hWidth, y+hHeight);
            points[2] = new System.Drawing.Point(x + hWidth, y + hHeight);
            points[3] = new System.Drawing.Point(x + hWidth, y - hHeight);

            Brush brush = new SolidBrush(Color.GreenYellow);

            g.FillPolygon(brush, points);
        }

	    private void drawOnPanel(Robot currenRobot)
	    {
            Graphics g = panel1.CreateGraphics();
	        drawBackground(g);
	        drawRobot(g, currenRobot, 20, 20, 10, 20, 0);
	    }
        
	    private void selectRobot()
	    {
	        Object text = (string)GetSelectedRobot();
            if(text != null) {
	            Robot current = null;
	            foreach (Robot robot in communicator.robots)
	            {
	                if (robot.name.Equals(text))
	                {
	                    current = robot;
	                    SetRobotData(robot);
	                    break;
	                }
	            }
                if(current != null) { 
                    foreach (Robot robot in communicator.robots)
                    {
                        if (robot != current)
                        {
                            current = robot;
                            SetRobotData(robot);
                            break;
                        }
                    }
                }
            }
	    }
        //TODO: end of bangladesh style

        private void updateDraw()
        {
            selectRobot();
            Dictionary<Robot, RobotType> robots = new Dictionary<Robot,RobotType>();
            List<Node> nodes = null; 
            foreach (CarDriver driver in drivers)
            {
                if (driver.myRobot == selectedRobot)
                {
                    robots.Add(driver.myRobot, RobotType.Selected);
                    nodes = driver.GetPath();
                    //TODO: add visible robots
                }
                else
                {
                    if (!robots.ContainsKey(driver.myRobot))
                    {
                        robots.Add(driver.myRobot, RobotType.Normal);
                    }
                }
            }

            drawingManager.clean();
            drawingManager.draw();
            //draw robots
            foreach (KeyValuePair<Robot, RobotType> entry in robots)
            {
                drawingManager.drawRobot(entry.Key, entry.Value);
            }
            if(nodes != null) 
            {
                drawingManager.drawNodes(nodes);
            }
        }

		private void RefreshThread()
		{
            /*	try
                {*/
            int counter = 0;
            while (true)
            {
                counter++;
                //update only 1/30 times to avoid flickering
                if (counter >= 30)
                {
                    counter = 0;
                    updateDraw();
                }

                if (communicator.Receive(Communicator.RECEIVEBLOCKLEVEL_WaitForNewTimestamp) < 0)
                {
                    Console.WriteLine("koniec 1");

                    connectButton_Click(this, null);
                    return;
                }
                if (drivers.Count < communicator.robots.Length)
                {
                    /*try
                    {*/
                        int newRobotId = communicator.RequestRobot("Capo");
                        if (newRobotId != -1)
                            unsafe
                            {
//                                Console.WriteLine(newRobotId + ": " + communicator.robots[newRobotId].position[0] + " " +
//                                                  communicator.robots[newRobotId].position[1]);
                                drivers.Add(new CarDriver(communicator.robots[newRobotId], nodesList, _rosonPath));
                                ((CarDriver) drivers[drivers.Count - 1]).RandTargetNode();
                            } /*
                    }
                    catch (Exception e) {
                        Console.WriteLine(e.StackTrace);
                        Console.WriteLine("Error while adding new robot");
                    }*/
                }
                else if (drivers.Count > communicator.robots.Length)
                {
                    communicator.ReleaseRobot(((CarDriver)drivers[0]).myRobot.id);
                    drivers.RemoveAt(0);
                }
                //ownedLabel.Text = "cars owned: "+drivers.Count.ToString();
                if (drivers.Count == communicator.robots.Length)
                {
                    futurePoints = new Dictionary<int, List<Point>>();
                    foreach (CarDriver driver in drivers)
                    {
                        Dictionary<int, CarParameters> visibleCars = new Dictionary<int, CarParameters>();
                        futurePoints.Add(driver.myRobot.id, saveFuturePositions(driver));
                        foreach (CarDriver driv in drivers)
                        {
                            if (driv != driver && IsClose(driver, driv))
                                unsafe
                                {
                                    visibleCars.Add(driv.myRobot.id, new CarParameters(driv.myRobot.id, driv.myRobot.position, driv.myRobot.rotation));
                                }
                        }
                        driver.Refresh(visibleCars);
                    }

                    List<Collision> collisions = findCollisions();
                    Console.WriteLine("I found collisions for robots: ");
                    foreach (Collision col in collisions)
                    {
                        Console.WriteLine(col.Id1 + " with " + col.Id2);
                    }
                }

                if (communicator.Send() < 0)
                {
                    Console.WriteLine("koniec 2");
                    connectButton_Click(this, null);
                    return;
                }
            }/*
			}
			catch(Exception e)
			{
                Console.WriteLine(e.Message);
                Console.WriteLine("koniec 3");
				connectButton_Click(this, null);
				return;
			}*/
        }

        private unsafe List<Point> saveFuturePositions(CarDriver driver) {
            double velocity = *driver.myRobot.lineralVelocity;
            double angle = driver.CountRotation(driver.myRobot.rotation);
            var position = driver.myRobot.position;
            var x = position[0];
            var y = position[1];
            int time;
            List<Point> result = new List<Point>();
            for (time = 1; time < 10; time++)
            {
                double path = velocity * time;
                result.Add(new Point(x + path*Math.Cos(angle), y + path*Math.Sin(angle)));
            }
            return result;
        }

        private unsafe List<Collision> findCollisions()
        {
            List<Collision> result = new List<Collision>();
            List<int> collisionIds = new List<int>();

            for (int i = 0; i < 9; i++)
            {
                List<Point> tmpPoints = new List<Point>();
                List<int> tmpRobotsIds = new List<int>();

                foreach (int id in futurePoints.Keys)
                {
                    if (!collisionIds.Contains(id)) {
                        tmpRobotsIds.Add(id);
                        tmpPoints.Add(futurePoints[id][i]);
                    }
                    List<int> collisions = checkPoints(tmpPoints);
                    int counter = 1;
                    int tmpId = 0;
                    foreach (int listId in collisions) {
                        collisionIds.Add(tmpRobotsIds[listId]);
                        if (counter == 1)
                        {
                            counter++;
                            tmpId = tmpRobotsIds[listId];
                        }
                        else
                        {
                            counter = 1;
                            result.Add(new Collision(tmpId, tmpRobotsIds[listId], i));
                        }
                    }
                }
            }
            return result;
        }

        private List<int> checkPoints(List<Point> points)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < points.Count - 1; i++)
            {
                for (int j = i + 1; j < points.Count; j++)
                {
                    if ((points[i].X - points[j].X) * (points[i].X - points[j].X) + (points[i].Y - points[j].Y) * (points[i].Y - points[j].Y) < 0.0603 * 0.0603)
                    {
                        result.Add(i);
                        result.Add(j);
                    }
                }
            }
            return result;
        }

        private static unsafe bool IsClose(CarDriver driver, CarDriver driv)
        {
            
            return Math.Sqrt((driver.myRobot.position[0] - driv.myRobot.position[0]) * (driver.myRobot.position[0] - driv.myRobot.position[0]) + (driver.myRobot.position[1] - driv.myRobot.position[1]) * (driver.myRobot.position[1] - driv.myRobot.position[1])) <= 1;
        }
    }
}
