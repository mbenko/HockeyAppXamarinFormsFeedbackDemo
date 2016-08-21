using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

// --- Add usings
using Android.Hardware;
using HockeyApp.Android;

namespace XamFormsFeedbackDemo.Droid
{
	[Activity (Label = "XamFormsFeedbackDemo", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{
        // --- Add Globals
        public static string HOCKEYAPP_APPID = "c4695faeedb14c4b98635838fe763d8c";
        public static Android.App.Activity current;

        protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

            // --- Register this as a listener with the underlying service.
            var sensorManager = GetSystemService(SensorService) as Android.Hardware.SensorManager;
            var sensor = sensorManager.GetDefaultSensor(Android.Hardware.SensorType.Accelerometer);

            current = this;
            sensorManager.RegisterListener(new MyShakeHandler(current), sensor, Android.Hardware.SensorDelay.Normal);
            InitializeHockeyApp();



            global::Xamarin.Forms.Forms.Init (this, bundle);
            LoadApplication(new App());
        }

        // --- Register the CrashManager, UpdateManager, FeedbackManager and start Tracking
        private void InitializeHockeyApp()
        {
            CrashManager.Register(this, HOCKEYAPP_APPID);

            UpdateManager.Register(this, HOCKEYAPP_APPID);
            FeedbackManager.Register(this, HOCKEYAPP_APPID);
            Tracking.StartUsage(this);

        }

        // --- Handle feedback from shake
        public void HandleFeedback()
        {
            FeedbackManager.SetActivityForScreenshot(MainActivity.current);
            FeedbackManager.TakeScreenshot(MainActivity.current);

            FeedbackManager.ShowFeedbackActivity(MainActivity.current);
        }

    }

    // --- Implement ISensorEventListener
    public class MyShakeHandler : Java.Lang.Object, ISensorEventListener
    {
        // --- Reference to parent activity
        private MainActivity parent;

        // Handle Shake from - http://stackoverflow.com/questions/23120186/can-xamarin-handle-shake-accelerometer-on-android
        bool hasUpdated = false;

        DateTime lastUpdate;
        float last_x = 0.0f;
        float last_y = 0.0f;
        float last_z = 0.0f;

        const int ShakeDetectionTimeLapse = 250;
        const double ShakeThreshold = 800;

        // --- In constructor set parent
        public MyShakeHandler(Activity context) : base()
        {
            parent = (MainActivity)context;
        }

        public void OnAccuracyChanged(Android.Hardware.Sensor sensor, Android.Hardware.SensorStatus accuracy)
        {
        }

        public void OnSensorChanged(Android.Hardware.SensorEvent e)
        {
            if (e.Sensor.Type == Android.Hardware.SensorType.Accelerometer)
            {
                float x = e.Values[0];
                float y = e.Values[1];
                float z = e.Values[2];

                DateTime curTime = System.DateTime.Now;
                if (hasUpdated == false)
                {
                    hasUpdated = true;
                    lastUpdate = curTime;
                    last_x = x;
                    last_y = y;
                    last_z = z;
                }
                else
                {
                    if ((curTime - lastUpdate).TotalMilliseconds > ShakeDetectionTimeLapse)
                    {
                        float diffTime = (float)(curTime - lastUpdate).TotalMilliseconds;
                        lastUpdate = curTime;
                        float total = x + y + z - last_x - last_y - last_z;
                        float speed = Math.Abs(total) / diffTime * 10000;

                        if (speed > ShakeThreshold)
                        {
                            // --- Call parent's Feedback handler
                            parent.HandleFeedback();
                        }

                        last_x = x;
                        last_y = y;
                        last_z = z;
                    }
                }
            }
        }
    }



}

