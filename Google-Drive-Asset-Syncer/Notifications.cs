using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;

namespace Google_Drive
{
    class Notifications
	{
		public static class NativeTest
		{
			private const string dllPath = "D:/Downloads/WinToast-1.2.0/example/console-example/x64/Release/WinToast Console.dll";

			[DllImport(dllPath)]
			public static extern int WindowsNotify(string notification, string content,string name, string appModelUserID);

			[DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
			public static extern int Helloworld();

			public static int NotifyService(string notification, string content)
			{
				return WindowsNotify(notification,content,"Tweedle Mobile","TweedleMobile");
			}

		}
	}
}
