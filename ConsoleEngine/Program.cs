using EngineViewer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urho3DNet;

namespace ConsoleEngine
{
	class Program
	{
		static void Main(string[] args)
		{

			using (var context = new Context())
			{
				using (var application = new DefaultScene(context))
				{
					application.Run();
				}
			}

		}
	}
}
