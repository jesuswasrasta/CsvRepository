#region Usings
using System;


#endregion


namespace Persistence.Csv
{
	public class RepositoryException : Exception
	{
		public RepositoryException(string message) : base(message)
		{
		}
	}
}