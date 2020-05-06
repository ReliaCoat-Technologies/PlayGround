using System;
using System.Linq;

namespace SoftwareThemeDesigner.Utilities
{
	public static class ValueFilterHelpers
	{
		public static bool containsFilterInOrder(this object obj, string filter, string memberPath = "")
		{
			// O(n) mechanism for determining if source string contains character of filter string in order.
			var filterCharArray = filter.ToLower().ToCharArray();

			if (!filterCharArray.Any())
				return true;
			
			var stringValue = !string.IsNullOrWhiteSpace(memberPath) 
				? obj.getObjectMember(memberPath) 
				: obj.ToString();

			var sourceCharArray = stringValue?.ToLower().ToCharArray() ?? new char[0];

			var filterCharIndex = 0;

			foreach (var sourceChar in sourceCharArray)
			{
				if (sourceChar == filterCharArray[filterCharIndex])
					filterCharIndex++;

				if (filterCharIndex >= filterCharArray.Length)
					return true;
			}

			return false;
		}

		public static string getObjectMember(this object obj, string memberPath)
		{
			// Uses reflection -- efficient?
			var paths = memberPath.Split('.');

			foreach (var path in paths)
			{
				obj = obj?.GetType().GetProperty(path)?.GetValue(obj);

				if (obj == null)
					return null;
			}

			// Error raised when the memberPath of the object is not a string.
			if (!(obj is string))
				throw new Exception("\"memberPath\" must lead to type(String) type member.");

			return obj.ToString();
		}
	}
}