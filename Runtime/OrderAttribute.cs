using System;
using System.Collections.Generic;
using System.Linq;

namespace Depra.Inspector.SerializedReference
{
	public sealed class OrderAttribute : Attribute
	{
		private const int ORDER = 0;
		private readonly int _order;

		public OrderAttribute(int order) => _order = order;

		public static IEnumerable<Type> OrderBy(IEnumerable<Type> self) => self.OrderBy(type =>
			type?.GetCustomAttribute<OrderAttribute>()?._order ?? ORDER);
	}
}