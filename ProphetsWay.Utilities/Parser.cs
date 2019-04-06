using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;

namespace ProphetsWay.Utilities
{
	public static class Parser
	{
        /// <summary>
		/// Finds the value in a string dictionary, then converts that value into a typed value.
		/// </summary>
		public static T GetValueFromKey<T>(this IDictionary<string, string> dictionary, string key)
        {
            return dictionary.ContainsKey(key)
                    ? GetValue<T>(dictionary[key])
                    : default;
        }

        /// <summary>
        /// Converts the string value into a typed value
        /// </summary>
        public static T GetValue<T>(this string input)
		{
            T retval = default;

			if (typeof(T).IsEnum)
			{
				EnumTryParse(input, out retval);
				return retval;
			}

			var cType = typeof(T);
			var objType = cType;

			if (cType.IsGenericType && cType.GetGenericTypeDefinition() == typeof(Nullable<>))
				objType = new NullableConverter(cType).UnderlyingType;

			var obj = ParseStringAsType(input, cType);

			if (obj != null)
				retval = (T)Convert.ChangeType(obj, objType);

			return retval;
		}

		private static void EnumTryParse<T>(string strEnumValue, out T result)
		{
			if (string.IsNullOrEmpty(strEnumValue))
			{
				result = default;
				return;
			}

			var typeFixed = strEnumValue.Replace(' ', '_');
			if (Enum.IsDefined(typeof(T), typeFixed))
				result = (T)Enum.Parse(typeof(T), typeFixed, true);
			else
			{
				result = default;

				foreach (var value in Enum.GetNames(typeof(T)).Where(value => value.Equals(typeFixed, StringComparison.OrdinalIgnoreCase)))
				{
					result = (T)Enum.Parse(typeof(T), value);
                    return;
				}

				result = (T)Enum.Parse(typeof(T), strEnumValue);
			}
		}

		private static object ParseStringAsType(string input, Type cType)
		{
			var conversionType = cType;
			object retval = null;
			var objType = conversionType;
			object obj = null;

			DateTime dt;
			TimeSpan ts;
			int i;
			ushort u;
			long l;
			short s;
			double d;
            float f;

            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition() == typeof(Nullable<>))
                objType = new NullableConverter(conversionType).UnderlyingType;

            if (conversionType == typeof(DateTime?))
				if (DateTime.TryParse(input, out dt))
					obj = dt;

			if (conversionType == typeof(DateTime))
				if (DateTime.TryParse(input, out dt))
					obj = dt;

			if (conversionType == typeof(TimeSpan?))
				if (TimeSpan.TryParse(input, out ts))
					obj = ts;

			if (conversionType == typeof(TimeSpan))
				if (TimeSpan.TryParse(input, out ts))
					obj = ts;

			if (conversionType == typeof(IPAddress))
				if (IPAddress.TryParse(input, out IPAddress ip))
					obj = ip;

			if (conversionType == typeof(short?))
				if (short.TryParse(input, out s))
					obj = s;

			if (conversionType == typeof(short))
				if (short.TryParse(input, out s))
					obj = s;

			if (conversionType == typeof(ushort))
				if (ushort.TryParse(input, out u))
					obj = u;

			if (conversionType == typeof(ushort?))
				if (ushort.TryParse(input, out u))
					obj = u;

			if (conversionType == typeof(int?))
				if (int.TryParse(input, out i))
					obj = i;

			if (conversionType == typeof(int))
				if (int.TryParse(input, out i))
					obj = i;

			if (conversionType == typeof(long?))
				if (long.TryParse(input, out l))
					obj = l;

			if (conversionType == typeof(long))
				if (long.TryParse(input, out l))
					obj = l;

			if (conversionType == typeof(double?))
				if (double.TryParse(input, out d))
					obj = d;

			if (conversionType == typeof(double))
				if (double.TryParse(input, out d))
					obj = d;

			if (conversionType == typeof(float?))
				if (float.TryParse(input, out f))
					obj = f;

			if (conversionType == typeof(float))
				if (float.TryParse(input, out f))
					obj = f;

			if (conversionType == typeof(bool))
				if (bool.TryParse(input, out bool b))
					obj = b;

			if (conversionType == typeof(string))
				obj = input;

			if (obj != null)
				retval = Convert.ChangeType(obj, objType);

			return retval;
		}

		
	}
}