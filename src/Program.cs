using System;
using System.Windows;

internal class Program
{
	[STAThread]
	private static void Main()
	{
		Application application = new Application();
		application.Run(new MainWindow());
	}
}
