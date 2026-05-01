using System;
using System.Collections.Generic;

namespace Depra.SerializeReference.Extensions
{
	public interface IVirtualType
	{
		IEnumerable<Type> GetDerivedTypes(Type referenceType);
	}
}