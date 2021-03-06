﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SystemClass = Tutorial24.System.System;
using System.Threading;
using System.Globalization;

namespace Tutorial24
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			SystemClass system;
			bool result;

			Thread.CurrentThread.CurrentCulture = new CultureInfo("en");
			system = new SystemClass();
			try
			{
				result = system.Initialize();
				if (result)
					system.Run();
			}
			finally
			{
				system.Shutdown();
			}
		}
	}
}
