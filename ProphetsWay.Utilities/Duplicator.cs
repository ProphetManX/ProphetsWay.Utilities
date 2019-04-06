namespace ProphetsWay.Utilities
{
    public static class Duplicator
    {
        /// <summary>
		/// Will duplicate any object, and any properties that are reference types will also be duplicated.  Will not copy Lists nicely?
		/// </summary>
		/// <returns>Returns a copy of the input type.</returns>
		public static T DuplicateObject<T>(this T original)
            where T : new()
        {
            var type = typeof(T);
            var copy = new T();

            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                if (!property.CanWrite)
                    continue;

                var propVal = property.PropertyType.IsByRef
                    ? property.GetValue(original, null).DuplicateObject()
                    : property.GetValue(original, null);

                if (propVal != null)
                    property.SetValue(copy, propVal, null);

            }

            return copy;
        }

        /// <summary>
        /// Will duplicate any object that can be serialized.  This will basically serialize your object to a byte array, 
        /// then deserialize it back to a new instance of an object of the same type, but all memory references to each other are broken.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original"></param>
        /// <returns></returns>
        public static T DuplicateObjectSerial<T>(this T original)
        {
            var copy = original.SerializeAsByteArr().DeserializeFromByteArr<T>();

            return copy;
        }

    }
}
