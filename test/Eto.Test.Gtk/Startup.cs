using Eto;
using Eto.Test;
namespace Eto.Test.Gtk
{
	class Startup
	{
		[STAThread]
		static void Main(string[] args)
		{
			var generator = new Eto.GtkSharp.Platform();
			
			var app = new TestApplication(generator);
			app.TestAssemblies.Add(typeof(Startup).Assembly);
			app.Run();
		}
	}
}

