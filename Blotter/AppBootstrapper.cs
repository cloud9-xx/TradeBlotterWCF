using System.Windows;
using Caliburn.Micro;
using Blotter.ViewModels;
using System.Collections.Generic;
using System;
using Blotter.Services;
using Blotter.TradeData;
using Blotter.ReferenceData;

namespace Blotter
{
    public class AppBootstrapper : BootstrapperBase
    {
        public AppBootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }

    }
}
