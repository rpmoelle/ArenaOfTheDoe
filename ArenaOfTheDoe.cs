using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using System.IO;
using System.Reflection;


namespace KinectSkeletonApplication3
{
    public partial class MainWindow : Window
    {
        /*Arena of the Doe BETA 1/31/13
        * Authors: Rachel Moeller
        * Description: This code manages the artwork Arena of the Doe. It tracks and responds to a users movement by 
        * playing a series of video clips featuring a figure. This gives the illusion of interaction.
        * 
        * CURRENT ISSUES:
        * 1)Seamless transition between clips. The clips are triggered, but the media element takes a few seconds before 
        * playing the corresponding video. During this time a blank screen is displayed.
         * ////////FIXED WITH BACKGROUND HANDLING->MATCH THE BACKGROUND COLOR TO THE PROJECTION BACKGROUND
        * 2)Devise a better way of keeping track of the users position. Need to center the figure.
        * ///////FIXED WITH A STATIC EXTERNAL ARENA/DISREGARD OTHER USERS
        * 
        * Thanks to Carol Yarbrough and Payton Walker for their programming expertise and time.
        * Thanks to Adam Guthrie for manning the camera and being an all together amazing person(understatement.)
        * */


        //Instantiate the Kinect runtime. Required to initialize the device.
        //IMPORTANT NOTE: You can pass the device ID here, in case more than one Kinect device is connected.


        //External Conditions:
        //Center value have been calibrated for standing about 4 feet from the Kinect sensor
        //Have Kinect plugged in, build, then it will work. Other wise EXCEPTION FROM HELL RETURNS.

        //bool center = false;
        //bool left = false;
        //bool right = false;
        //bool isMedia = false;

        //int userStayedStill = 0;
    
        KinectSensor runtime = KinectSensor.KinectSensors[0];

        public MainWindow()
        {
            InitializeComponent();

            //Runtime initialization is handled when the window is opened. When the window
            //is closed, the runtime MUST be unitialized.
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
            this.Unloaded += new RoutedEventHandler(MainWindow_Unloaded);

            runtime.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(runtime_SkeletonFrameReady);
        }

        void runtime_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            SkeletonFrame skeletonSet = e.OpenSkeletonFrame();

            if (skeletonSet != null)
            {
                Skeleton[] skeletonData = new Skeleton[skeletonSet.SkeletonArrayLength];



                skeletonSet.CopySkeletonDataTo(skeletonData);

                Skeleton data = (from s in skeletonData
                                 where s.TrackingState == SkeletonTrackingState.Tracked
                                 select s).FirstOrDefault();
                if (data != null)
                {
                    //SetEllipsePosition(Head, data.Joints[JointType.ShoulderCenter]);
                    //SetEllipsePosition(leftHand, data.Joints[JointType.HipCenter]);
                   // SetEllipsePosition(rightHand, data.Joints[JointType.HandRight]);
                    ProcessGesture(data.Joints[JointType.Head], data.Joints[JointType.HandLeft], data.Joints[JointType.HandRight]);
                }
            }

        }


        //CONTROL
        private void ProcessGesture(Joint head, Joint handleft, Joint handright)
        {
           // Microsoft.Kinect.SkeletonPoint vector = new Microsoft.Kinect.SkeletonPoint();
            //vector.X = ScaleVector(800, handright.Position.X);
            //vector.Y = ScaleVector(600, -handright.Position.Y);
            //vector.Z = handright.Position.Z;

            

            //handright.Position = vector;
              if (head.Position.X > 1.1)//right
            {

                //media.Source = new System.Uri("C://Users/Rachel/Desktop/DeerMovementsM/UserMovedRightFromCenterToRight.mp4");
                //media.Play();
                media.Source = new System.Uri("C://Users/Rachel/Desktop/DeerMovements/UserMovedLeftFromRightToCenter.avi");
                media.Play();
              }

            else if (head.Position.X < -.94)//left
            {
                //media.Source = new System.Uri("C://Users/Rachel/Desktop/DeerMovementsM/UserMovedLeftFromCenterToLeft.mp4");
                //media.Play();
                media.Source = new System.Uri("C://Users/Rachel/Desktop/DeerMovements/UserMovedRightFromLeftToCenter.avi");
                media.Play();
                //change media state to open after each video

            }
            
            
            
            ////
            ////
            ////Depth Handler
              else if (head.Position.Z < 1.02)//forward
            {
                media.Source = new System.Uri("C://Users/Rachel/Desktop/DeerMovements/RunAway.avi");
                media.Play();
                //media.Source = new System.Uri("C://Users/Rachel/Desktop/DeerMovements/UserMovedLeftFromRightToCenter.avi");
                //media.Play();
                
                
            }
            ///Depth Handler 
            else if (head.Position.Z > 3.9)//back
            {
                media.Source = new System.Uri("C://Users/Rachel/Desktop/DeerMovements/RunAway.avi");
                media.Play();
                
            }
            ////
            ////
            ////Sit handler
            /*else if (head.Position.Y < .29)//back
            {
                media.Source = new System.Uri("C://Users/Rachel/Desktop/DeerMovementsM/UserSat.mp4");
                media.Play();
                //media.Source = new System.Uri("C://Users/Rachel/Desktop/DeerMovementsM/Blank.mp4");
                //media.Play();
            }

              detectHand.Text = Convert.ToString(head.Position.X); 
            */

        }

        private void SetEllipsePosition(Ellipse ellipse, Joint joint)
        {
            Microsoft.Kinect.SkeletonPoint vector = new Microsoft.Kinect.SkeletonPoint();
            vector.X = ScaleVector(800, joint.Position.X);
            vector.Y = ScaleVector(600, -joint.Position.Y);
            vector.Z = joint.Position.Z;

            Joint updatedJoint = new Joint();
            //updatedJoint.JointType = joint.JointType;
            updatedJoint.TrackingState = JointTrackingState.Tracked;
            updatedJoint.Position = vector;

            // Canvas.SetLeft(ellipse, updatedJoint.Position.X);
            //Canvas.SetTop(ellipse, updatedJoint.Position.Y);


        }

        private float ScaleVector(int length, float position)
        {
            float value = (((((float)length) / 1f) / 2f) * position) + (length / 2);
            if (value > length)
            {
                return (float)length;
            }
            if (value < 0f)
            {
                return 0f;
            }
            return value;
        }



        void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            runtime.Stop();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //Since only a color video stream is needed, RuntimeOptions.UseColor is used.
            //runtime.Initialize(runtime.ColorStream.Enable() | runtime.SkeletonStream.Enable());
         


            runtime.ColorStream.Enable();
            runtime.SkeletonStream.Enable();
            runtime.Start();
        }


    }
}
