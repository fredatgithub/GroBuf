using System;

namespace GroBuf
{
    public delegate void WriterDelegate<in T>(T obj, bool writeEmpty, IntPtr result, ref int index);
    public delegate void WriterDelegate(object obj, bool writeEmpty, IntPtr result, ref int index);
}