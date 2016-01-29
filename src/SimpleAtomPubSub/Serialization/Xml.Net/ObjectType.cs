using System;
using System.Collections;
using System.Reflection;

namespace SimpleAtomPubSub.Serialization.Xml.Net
{
    internal struct ObjectType
    {
        public static ObjectType Primitive => new ObjectType(1);
        public static ObjectType List => new ObjectType(2);
        public static ObjectType Dictionary => new ObjectType(3);
        public static ObjectType Other => new ObjectType(10);

        private ObjectType(int id)
        {
            Id = id;
        }

        public static ObjectType From(object value)
        {
            return From(value.GetType());
        }

        public static ObjectType From(Type type)
        {
            if (IsPrimitive(type))
            {
                return Primitive;
            }
            if (IsDictionary(type))
            {
                return Dictionary;
            }
            if (IsList(type))
            {
                return List;
            }

            return Other;
        }

        private int Id { get; }

        public override bool Equals(object obj)
        {
            return GetHashCode() == obj.GetHashCode();
        }

        public static bool operator ==(ObjectType a, ObjectType b)
        {
            if (a == null)
            {
                return false;
            }
            return a.Equals(b);
        }

        public static bool operator !=(ObjectType a, ObjectType b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        /// <summary>
        /// Checks if the type is a fundamental primitive object (e.g string, int etc.).
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>The boolean value indicating whether the type is a fundamental primitive.</returns>
        private static bool IsPrimitive(Type type) => (
            type.Equals(typeof(string))
            || type.Equals(typeof(char))
            || type.Equals(typeof(sbyte))
            || type.Equals(typeof(short))
            || type.Equals(typeof(int))
            || type.Equals(typeof(long))
            || type.Equals(typeof(byte))
            || type.Equals(typeof(ushort))
            || type.Equals(typeof(uint))
            || type.Equals(typeof(ulong))
            || type.Equals(typeof(double))
            || type.Equals(typeof(float))
            || type.Equals(typeof(decimal))
            || type.Equals(typeof(bool))
            || type.Equals(typeof(DateTime))
            );

        /// <summary>
        /// Checks if the type is a list (e.g List<T>, Array etc.).
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>The boolean value indicating whether the type is a list.</returns>
        private static bool IsList(Type type)
        {
            return typeof(ICollection).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
        }

        /// <summary>
        /// Checks if the object is a dictionary (e.g Dictionary<TKey, TValue>, HashTable etc.).
        /// </summary>
        /// <param name="value">The object to check.</param>
        /// <returns>The boolean value indicating whether the object is a dictionary.</returns>
        public static bool IsDictionary(Type type)
        {
            return typeof(IDictionary).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
        }
    }
}
