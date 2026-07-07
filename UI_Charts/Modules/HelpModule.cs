using System;
using System.Collections.Generic;
using System.Text;
using UICharts.Desktop.Events;
using UICharts.Desktop.Views;
using Prism.Modularity;
using System.Windows;

namespace UICharts.Desktop.Modules
{
    public class HelpModule : IModule
    {
        private readonly IEventAggregator eventAggregator;

        public HelpModule(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            
            eventAggregator
                .GetEvent<ShowHelpRequestedEvent>()
                .Subscribe(OpenHelpWindow, keepSubscriberReferenceAlive: true);
        }

        private void OpenHelpWindow()
        {
            
            var window = new HelpWindow
            {
                Owner = Application.Current.MainWindow
            };

            window.ShowDialog();
        }
    }
}
