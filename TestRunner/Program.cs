namespace TestRunner
{
	class Program
	{
		static void Main(string[] args)
		{
			TestHarness test = new TestHarness();

			test.Invoke();
		}
	}
}