using Unity;

namespace XUnitTests
{
	public abstract class MoviePickerValidationTestsContext
	{
		public IUnityContainer UnityContainer { get; } = new UnityContainer();

		protected abstract void SetupContainer();
	}
}