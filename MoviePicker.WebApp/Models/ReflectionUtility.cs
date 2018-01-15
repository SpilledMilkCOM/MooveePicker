namespace MoviePicker.WebApp.Models
{
	public static class ReflectionUtility
	{
		public static object GetValue(object obj, string propertyName)
		{
			//TODO: Store the info statically so it only has to be done once.

			return obj.GetType().GetProperty(propertyName)?.GetValue(obj);
		}
	}
}