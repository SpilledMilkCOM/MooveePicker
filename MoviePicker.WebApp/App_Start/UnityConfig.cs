using MoviePicker.Common;
using MoviePicker.Common.Interfaces;
using MoviePicker.Msf;
using MoviePicker.Repository;
using MoviePicker.Repository.Interfaces;
using MoviePicker.Repository.Models;
using MoviePicker.WebApp.Interfaces;
using MoviePicker.WebApp.Models;
using MoviePicker.WebApp.ViewModels;
using MoviePicker.WebApp.Utilities;
using System;

using Unity;
using Unity.Injection;
using Unity.Lifetime;
using System.Configuration;
using SM.COMS.Models.Interfaces;
using SM.COMS.Models;
using SM.COMS.Utilities;
using SM.COMS.Utilities.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using System.Collections.Generic;
using Microsoft.ApplicationInsights.DataContracts;

namespace MoviePicker.WebApp
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container =
          new Lazy<IUnityContainer>(() =>
          {
              var container = new UnityContainer();
              RegisterTypes(container);
              return container;
          });

        /// <summary>
        /// Configured Unity Container.
        /// </summary>
        public static IUnityContainer Container => container.Value;
        #endregion

        /// <summary>
        /// Registers the type mappings with the Unity container.
        /// </summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>
        /// There is no need to register concrete types such as controllers or
        /// API controllers (unless you want to change the defaults), as Unity
        /// allows resolving a concrete type even if it was not previously
        /// registered.
        /// </remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below.
            // Make sure to add a Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your type's mappings here.
            // container.RegisterType<IProductRepository, ProductRepository>();

            var appInsightsKey = ConfigurationManager.AppSettings["AppInsightsInstrumentationKey"];

            container.RegisterSingleton<TelemetryClient>(new InjectionConstructor(new TelemetryConfiguration(appInsightsKey)));

            var appInsights = container.Resolve<TelemetryClient>();

            appInsights.TrackTrace("Application Insights TelemetryClient registered with Unity IoC Container.", SeverityLevel.Information);

            // Initialize the Send Grid key to send email.
            var sendGridKey = ConfigurationManager.AppSettings["SendGridKey"];
            var sendGridTo = ConfigurationManager.AppSettings["SendGridTo"];

            container.RegisterType<IMailModel, MailModel>();
            container.RegisterType<IMailUtility, MailUtil>(new InjectionConstructor(sendGridKey, sendGridTo, container.Resolve<TelemetryClient>()));

            // Understanding Lifetime Managers
            // https://msdn.microsoft.com/en-us/library/ff660872(v=pandp.20).aspx

            // Since the MinerModel will contain mined data then I probably want to keep this around longer than a single call.
            // Inject "true" for the constructor because we want this filled with data.

            container.RegisterType<IMinerModel, MinerModel>(new ContainerControlledLifetimeManager()		// Effectively a singleton.
                    , new InjectionConstructor(true
                            , container.Resolve<IMailUtility>()
                            , container.Resolve<TelemetryClient>()));

			// Each request will create a new IndexViewModel (PerThreadLifetimeManager)

			container.RegisterType<IIndexViewModel, IndexViewModel>(new PerThreadLifetimeManager());

			// Each of these will construct each time injected into the controller.

			container.RegisterType<IClientInfoModel, ClientInfoModel>();
			container.RegisterType<IControllerUtility, ControllerUtility>();
			container.RegisterType<IFandangoViewModel, FandangoViewModel>();
			container.RegisterType<IInfoViewModel, InfoViewModel>();
			container.RegisterType<IMoviePicker, MsfMovieSolver>();
			//container.RegisterType<IMoviePicker, MooveePicker.MoviePicker>();		Still too slow.
			container.RegisterType<IMovieList, MovieList>();
			container.RegisterType<IServerInfoModel, ServerInfoModel>();
			container.RegisterType<ISimulationModel, SimulationModel>();

			container.RegisterType<IBoxOfficeDataSource, BoxOfficeDataSource>();
			container.RegisterType<IBoxOfficeSource, BoxOfficeSource>();
        }
	}
}