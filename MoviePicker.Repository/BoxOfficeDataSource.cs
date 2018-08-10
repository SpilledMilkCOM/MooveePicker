using MoviePicker.Repository.Interfaces;
using MoviePicker.Repository.Models;
using System.Data.Entity;           // From NuGet: EntityFramework
using System.Linq;

namespace MoviePicker.Repository
{
	public class BoxOfficeDataSource : DbContext, IBoxOfficeDataSource
	{
		public DbSet<BoxOfficeSource> Sources { get; set; }

		public DbSet<BoxOfficeValue> Values { get; set; }

		IQueryable<IBoxOfficeSource> IBoxOfficeDataSource.Sources => Sources;

		IQueryable<IBoxOfficeValue> IBoxOfficeDataSource.Values => Values;

		public void AddSource(IBoxOfficeSource source)
		{
			
		}

		public void AddValue(IBoxOfficeValue value)
		{
			
		}

		public void DeleteSource(IBoxOfficeSource source)
		{
			
		}

		public void DeleteValue(IBoxOfficeValue value)
		{
			
		}

		public IBoxOfficeSource GetSource(int sourceId)
		{
			return null;
		}

		public IBoxOfficeValue GetValue(int sourceId)
		{
			return null;
		}

		public void Save()
		{
			SaveChanges();
		}
	}
}
