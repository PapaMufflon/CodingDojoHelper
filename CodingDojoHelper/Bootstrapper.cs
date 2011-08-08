using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using CodingDojoHelper.Events;
using CodingDojoHelper.Helper;
using CodingDojoHelper.Helper.Interfaces;
using CodingDojoHelper.Views;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Unity;

namespace CodingDojoHelper
{
    public class Bootstrapper : UnityBootstrapper
    {
        public Shell Shell { get; set; }

        private const string MainRegion = "MainRegion";
        private IRegionManager _regionManager;
        private IRegion _mainRegion;

        protected override DependencyObject CreateShell()
        {
            FillRegions();
            RegisterTypes();
            SetupSession();
            SubscribeToEvents();

            Shell = new Shell();
            return Shell;
        }
        
        private void FillRegions()
        {
            _regionManager = Container.Resolve<IRegionManager>();
            _regionManager.RegisterViewWithRegion(MainRegion, typeof(ConfigView));
            _regionManager.RegisterViewWithRegion(MainRegion, typeof(DojoView));
            _regionManager.RegisterViewWithRegion(MainRegion, typeof(DigestView));
            _regionManager.RegisterViewWithRegion(MainRegion, typeof(AdvancedConfigView));
        }

        private void RegisterTypes()
        {
            Container.RegisterType<IInterceptKeys, InterceptKeys>();
            Container.RegisterType<ICodingDojo, CodingDojo>();
            Container.RegisterType<IStopwatch, DojoStopwatch>();
            Container.RegisterType<ISoundPlayer, PlaylistSoundPlayer>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IKombatSoundPlayer, KombatSoundPlayer>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ISession, Session>(new ContainerControlledLifetimeManager());
        }

        private void SetupSession()
        {
            var session = Container.Resolve<ISession>();

            session.Set(Session.ChangeDeveloperKey, Keys.Scroll);
            session.Set(Session.EndKataKey, Keys.Pause);

            session.Set(Session.CycleTime, TimeSpan.FromMinutes(4));
            session.Set(Session.FinishHimTime, TimeSpan.FromMinutes(5));
            session.Set(Session.FinishHimTimeActive, false);
            session.Set(Session.DojoTime, TimeSpan.FromMinutes(55));

            session.Set(Session.CombatantImages, new List<Uri>
            {
                new Uri("pack://application:,,,/CodingDojoHelper;component/Resources/kano.gif"),
                new Uri("pack://application:,,,/CodingDojoHelper;component/Resources/johnny-cage.gif"),
                new Uri("pack://application:,,,/CodingDojoHelper;component/Resources/liu-kang.gif"),
                new Uri("pack://application:,,,/CodingDojoHelper;component/Resources/raiden.gif"),
                new Uri("pack://application:,,,/CodingDojoHelper;component/Resources/scorpion.gif"),
                new Uri("pack://application:,,,/CodingDojoHelper;component/Resources/sonya.gif"),
                new Uri("pack://application:,,,/CodingDojoHelper;component/Resources/reptile.gif")
            });
        }

        private void SubscribeToEvents()
        {
            var eventAggregator = Container.Resolve<IEventAggregator>();
            eventAggregator.GetEvent<KataFinishedEvent>().Subscribe(OnKataFinished);
            eventAggregator.GetEvent<ResetKataEvent>().Subscribe(OnResetKata);
            eventAggregator.GetEvent<StartKataEvent>().Subscribe(OnStartKata);
            eventAggregator.GetEvent<ShowAdvancedConfigEvent>().Subscribe(OnShowAdvancedConfigEvent);
        }

        private void OnKataFinished(ICodingDojo codingDojo)
        {
            _mainRegion = _regionManager.Regions[MainRegion];
            _mainRegion.Activate(_mainRegion.Views.OfType<DigestView>().First());
        }

        private void OnResetKata(object obj)
        {
            _mainRegion = _regionManager.Regions[MainRegion];
            _mainRegion.Activate(_mainRegion.Views.OfType<ConfigView>().First());
        }

        private void OnStartKata(object obj)
        {
            _mainRegion = _regionManager.Regions[MainRegion];
            _mainRegion.Activate(_mainRegion.Views.OfType<DojoView>().First());
        }

        private void OnShowAdvancedConfigEvent(object obj)
        {
            _mainRegion = _regionManager.Regions[MainRegion];
            _mainRegion.Activate(_mainRegion.Views.OfType<AdvancedConfigView>().First());
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            return new ConfigurationModuleCatalog();
        }
    }
}
