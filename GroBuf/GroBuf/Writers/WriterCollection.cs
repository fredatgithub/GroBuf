using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GroBuf.Writers
{
    internal class WriterCollection : IWriterCollection
    {
        public IWriterBuilder GetWriterBuilder(Type type)
        {
            var writerBuilder = (IWriterBuilder)writerBuilders[type];
            if(writerBuilder == null)
            {
                lock(writerBuildersLock)
                {
                    writerBuilder = (IWriterBuilder)writerBuilders[type];
                    if(writerBuilder == null)
                    {
                        writerBuilder = GetWriterBuilderInternal(type);
                        writerBuilders[type] = writerBuilder;
                    }
                }
            }
            return writerBuilder;
        }

        private static IWriterBuilder GetWriterBuilderInternal(Type type)
        {
            IWriterBuilder writerBuilder;
            if (type.GetCustomAttributes(typeof(GroBufCustomSerializationAttribute), false).Any())
            {
                MethodInfo customWriter = GroBufHelpers.GetMethod<GroBufWriterAttribute>(type);
                if (customWriter == null)
                    throw new MissingMethodException("Missing grobuf custom writer for type '" + type + "'");
                writerBuilder = new CustomWriterBuilder(type, customWriter);
            }
            else if(type == typeof(string))
                writerBuilder = new StringWriterBuilder();
            else if(type == typeof(DateTime))
                writerBuilder = new DateTimeWriterBuilder();
            else if(type == typeof(Guid))
                writerBuilder = new GuidWriterBuilder();
            else if(type.IsEnum)
                writerBuilder = new EnumWriterBuilder(type);
            else if(type.IsPrimitive || type == typeof(decimal))
                writerBuilder = new PrimitivesWriterBuilder(type);
            else if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                writerBuilder = new NullableWriterBuilder(type);
            else if(type.IsArray)
                writerBuilder = type.GetElementType().IsPrimitive ? (IWriterBuilder)new PrimitivesArrayWriterBuilder(type) : new ArrayWriterBuilder(type);
            else if(type == typeof(Array))
                writerBuilder = new ArrayWriterBuilder(type);
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                writerBuilder = new DictionaryWriterBuilder(type);
            else if (type == typeof(object))
                writerBuilder = new ObjectWriterBuilder();
            else
                writerBuilder = new ClassWriterBuilder(type);
            return writerBuilder;
        }

        private readonly Hashtable writerBuilders = new Hashtable();
        private readonly object writerBuildersLock = new object();
    }
}