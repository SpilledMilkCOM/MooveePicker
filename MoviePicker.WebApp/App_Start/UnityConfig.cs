using MoviePicker.Common;
using MoviePicker.Common.Interfaces;
using MoviePicker.Msf;
using MoviePicker.WebApp.Interfaces;
using MoviePicker.WebApp.Models;
using System;

using Unity;
using Unity.Injection;
using Unity.Lifetime;

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

			// Understanding Lifetime Managers
			// https://msdn.microsoft.com/en-us/library/ff660872(v=pandp.20).aspx

			// Since the MinerModel will contain mined data then I probably want to keep this around longer than a single call.
			// (But if everything else is manipulated through Angular then it might not be a big deal.)

			container.RegisterType<IMinerModel, MinerModel>(new ContainerControlledLifetimeManager(), new InjectionConstructor(true));		// Effectively a singleton.

			// Each request will create a new IndexViewModel (PerThreadLifetimeManager)

			container.RegisterType<IIndexViewModel, IndexViewModel>(new PerThreadLifetimeManager());

			// Each of these will construct each time injected into the controller.

			container.RegisterType<IClientInfoModel, ClientInfoModel>();
			container.RegisterType<IInfoViewModel, InfoViewModel>();
			container.RegisterType<IMoviePicker, MsfMovieSolver>();
			container.RegisterType<IMovieList, MovieList>();
			container.RegisterType<IServerInfoModel, ServerInfoModel>();
			container.RegisterType<ISimulationModel, SimulationModel>();
		}
	}
}