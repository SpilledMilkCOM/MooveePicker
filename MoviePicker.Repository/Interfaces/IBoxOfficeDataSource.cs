using System.Linq;

namespace MoviePicker.Repository.Interfaces
{
	/// <summary>
	/// The lower level CRUD things that the repository will use.
	/// </summary>
	public interface IBoxOfficeDataSource
    {
		IQueryable<IBoxOfficeSource> Sources { get; }

		IQueryable<IBoxOfficeValue> Values { get; }

		void AddSource(IBoxOfficeSource source);

		void AddValue(IBoxOfficeValue value);

		IBoxOfficeSource GetSource(int sourceId);

		IBoxOfficeValue GetValue(int sourceId);

		void DeleteSource(IBoxOfficeSource source);

		void DeleteValue(IBoxOfficeValue value);

		void Save();
	}
}