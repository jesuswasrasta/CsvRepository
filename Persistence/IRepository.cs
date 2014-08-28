#region Usings
using System;
using System.Linq;


#endregion


namespace Persistence
{
	public interface IRepository<T> : IDisposable where T : EntityBase, new()
	{
		void Store(T entity);

		T Select(T entity);

		//IQueryable<T> Select(Predicate<T> predicate);
		IQueryable<T> Select(Func<T, bool> func);

		void Delete(T entity);
	}
}