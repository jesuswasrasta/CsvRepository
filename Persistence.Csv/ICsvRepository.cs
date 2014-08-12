namespace Persistence.Csv
{
	public interface ICsvRepository<T> : IRepository<T> where T : CsvRecord, new()
	{
	}
}