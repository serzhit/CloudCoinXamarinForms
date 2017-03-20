using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace CloudCoin.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			Xamarin.Forms.Forms.Init();
			Xamarin.Forms.DependencyService.Register<CryptIOSImplementation>();


			// Code for starting up the Xamarin Test Cloud Agent
#if ENABLE_TEST_CLOUD
			Xamarin.Calabash.Start();
#endif

			LoadApplication(new App());
			return base.FinishedLaunching(app, options);
		}
	}
}
