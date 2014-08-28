using System;


namespace Persistence.Csv
{
	public class RepositoryClosedException : Exception
	{
		public RepositoryClosedException(string message):base(message)
		{
		}
	}
}