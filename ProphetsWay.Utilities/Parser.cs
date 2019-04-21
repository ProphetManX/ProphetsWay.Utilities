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
            object retval = null;
            object obj = null;
            var objType = cType;


            if (cType.IsGenericType && cType.GetGenericTypeDefinition() == typeof(Nullable<>))
                objType = new NullableConverter(cType).UnderlyingType;

            if (cType == typeof(DateTime) || cType == typeof(DateTime?))
                if (DateTime.TryParse(input, out DateTime dt))
                    obj = dt;

            if (cType == typeof(TimeSpan) || cType == typeof(TimeSpan?))
                if (TimeSpan.TryParse(input, out TimeSpan ts))
                    obj = ts;

            if (cType == typeof(IPAddress))
                if (IPAddress.TryParse(input, out IPAddress ip))
                    obj = ip;

            if (cType == typeof(short) || cType == typeof(short?))
                if (short.TryParse(input, out short s))
                    obj = s;

            if (cType == typeof(ushort) || cType == typeof(ushort?))
                if (ushort.TryParse(input, out ushort us))
                    obj = us;

            if (cType == typeof(int) || cType == typeof(int?))
                if (int.TryParse(input, out int i))
                    obj = i;

            if (cType == typeof(uint) || cType == typeof(uint?))
                if (uint.TryParse(input, out uint ui))
                    obj = ui;

            if (cType == typeof(long) || cType == typeof(long?))
                if (long.TryParse(input, out long l))
                    obj = l;

            if (cType == typeof(ulong) || cType == typeof(ulong?))
                if (ulong.TryParse(input, out ulong ul))
                    obj = ul;

            if (cType == typeof(double) || cType == typeof(double?))
                if (double.TryParse(input, out double d))
                    obj = d;

            if (cType == typeof(float) || cType == typeof(float?))
                if (float.TryParse(input, out float f))
                    obj = f;

            if (cType == typeof(bool))
                if (bool.TryParse(input, out bool b))
                    obj = b;

            if (cType == typeof(string))
                obj = input;

            if (obj != null)
                retval = Convert.ChangeType(obj, objType);

            return retval;
        }
	}
}