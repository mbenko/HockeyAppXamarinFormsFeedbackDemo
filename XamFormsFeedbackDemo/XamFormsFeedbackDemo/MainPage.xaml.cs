﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

#if DROID
using HockeyApp.Android;
using XamFormsFeedbackDemo.Droid;
#endif

namespace XamFormsFeedbackDemo
{
	public partial class MainPage : ContentPage
	{
		public MainPage ()
		{
			InitializeComponent ();
		}
        public void btnFeedback(object sender, EventArgs e)
        {
#if DROID
            FeedbackManager.SetActivityForScreenshot(MainActivity.current);
            FeedbackManager.TakeScreenshot(MainActivity.current);

            FeedbackManager.ShowFeedbackActivity(MainActivity.current);
#endif
        }
    }
}

