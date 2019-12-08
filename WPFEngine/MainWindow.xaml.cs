using EngineViewer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Urho3DNet;

namespace WPFEngine
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow: System.Windows.Window
	{
		public MainWindow()
		{
			InitializeComponent();
			WindowStyle = WindowStyle.None;
			Loaded += delegate
			{
				 DefaultScene.Parent = rbfxHost.Handle;

				Task.Run(() =>
				{
					using (var context = new Context())
					{
						using (var application = new DefaultScene(context))
						{
                            application.Context.Cache.AddResourceDir($"{App.LocaldllPath}/Resources/3D");

                            application.Run();
						}
					}
				});
			};
		}
	}
}
