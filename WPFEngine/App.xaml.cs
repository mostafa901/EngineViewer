﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace WPFEngine
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>

    public partial class App: Application
    {
        public static string LocaldllPath;

        public App()
        {
            LocaldllPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }
    }
}
