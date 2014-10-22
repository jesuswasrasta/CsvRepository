#region Usings
using System;


#endregion


namespace Persistence
{
	/// <summary>
	/// Base class for storable entities.
	/// </summary>
	public abstract class EntityBase
	{
		#region Fields
		private static readonly Random _random = new Random(int.MaxValue);
		#endregion


		#region Properties
		/// <summary>
		///     Gets the unique identifier of the entity.
		/// </summary>
		/// <value>
		///     The identifier.
		/// </value>
		public int Id { get; private set; }
		#endregion


		#region Constructors
		protected EntityBase()
		{
			Id = _random.Next();
		}
		#endregion
	}
}