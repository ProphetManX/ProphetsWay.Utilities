namespace ProphetsWay.Utilities
{
    public static class Duplicator
    {
        /*   I'm not comfortable with putting this function out in the wild just yet
         *   I am unsure how much effort I'll need to put in to verify it's correct
         *   and how many unit test vectors will need to be created to fully exercize it
         *   I'm leaving it commentd out until I come across a need/request to add it back in

        /// <summary>
        /// PROTOTYPE: USE AT OWN RISK
		/// Will duplicate any object, and any properties that are reference types will also be duplicated.  
        /// Will not copy Lists nicely?
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

        //*/


        /* However I'm leaving this one in, because as long as your object is marked "Serializable"
         * then this will work perfectly!  */
        /// <summary>
        /// Will duplicate any object that can be serialized (read: must be marked with attribute 'Serializable').  
        /// This will basically serialize your object to a byte array, and then deserialize it back 
        /// to a new instance of an object of the same type, but all memory references to each other are broken.
        /// </summary>
        public static T DuplicateObjectSerial<T>(this T original)
        {
            var copy = original.SerializeAsByteArr().DeserializeFromByteArr<T>();

            return copy;
        }

    }
}
