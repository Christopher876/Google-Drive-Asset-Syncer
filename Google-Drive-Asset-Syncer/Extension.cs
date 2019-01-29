using System;
using System.Text;

namespace ExtensionMethods
{
	public static class StringExtensions
	{
		//Extension to compare a file extension with another file extension
		public static bool CompareExtension(this string str,string toCompare)
		{
			StringBuilder builder = new StringBuilder();
			bool extension = false;
			foreach (char c in str)
			{
				if (extension)
				{
					builder.Append(c);
				}
				if (c == '.')
				{
					extension = true;
					builder.Append(c);
				}
			}

			if (toCompare == builder.ToString()) return true;
			else return false;
		}
	}
}
