using System;
using System.IO;
using System.Reflection;

namespace Saga
{
    public delegate bool UnresolvedType(string name, out Type type);

    public static class CoreService
    {
        #region Private Members

        private static event UnresolvedType _OnProbeUnresolvedType;

        public static TraceLog log = new TraceLog("General", string.Empty, 4);

        #endregion Private Members

        #region Public Members

        public static event UnresolvedType OnProbeUnresolvedType
        {
            add
            {
                if (_OnProbeUnresolvedType == null)
                    _OnProbeUnresolvedType += value;
                else
                    throw new Exception("Hook is already used.");
            }
            remove
            {
                if (_OnProbeUnresolvedType == value)
                    _OnProbeUnresolvedType -= value;
                else
                    throw new Exception("You cannot unhook any events yourself");
            }
        }

        #endregion Public Members

        #region Public Static Methods

        public static void SetTraceLog(TraceLog tracelog)
        {
            log = tracelog;
        }

        public static bool TryFindType(string name, out Type type)
        {
            if (IsExternalDefinition(name))
            {
                return TryFindTypeInExternalAssembly(name, out type);
            }
            else
            {
                if (_OnProbeUnresolvedType != null)
                    return _OnProbeUnresolvedType.Invoke(name, out type);

                type = null;
                return false;
            }
        }

        public static bool TryFindType(string name, out object type)
        {
            Type typeDefintion = null;
            if (IsExternalDefinition(name))
            {
                TryFindTypeInExternalAssembly(name, out typeDefintion);
                return CreateInstance(typeDefintion, out type);
            }
            else
            {
                if (_OnProbeUnresolvedType != null)
                    _OnProbeUnresolvedType.Invoke(name, out typeDefintion);
                return CreateInstance(typeDefintion, out type);
            }
        }

        private static bool CreateInstance(Type type, out object typeDefintion)
        {
            typeDefintion = null;
            if (type == null) return false;
            try
            {
                ConstructorInfo info = type.GetConstructor(Type.EmptyTypes);
                typeDefintion = info.Invoke(new Object[] { });
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static T Find<T>(string name) where T : class
        {
            Type type = typeof(T);
            if (type.IsSubclassOf(typeof(Delegate)) == false)
                throw new ArgumentException("Type must derive from Delegate");

            string typeString = string.Empty;
            string methodString = string.Empty;
            int lastindex = name.LastIndexOf(".");

            if (lastindex > -1 && lastindex != (name.Length - 1))
            {
                typeString = name.Substring(0, lastindex);
                methodString = name.Substring(lastindex + 1, name.Length - (lastindex + 1));

                Type d;
                if (!TryFindType(typeString, out d)) return null;
                if (d == null) return null;

                MethodInfo e = d.GetMethod(methodString, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
                if (e == null) return null;

                return Delegate.CreateDelegate(type, e) as T;
            }
            else
            {
                throw new ArgumentException(string.Format("Supplied method string is invalid {0}", name));
            }
        }

        #endregion Public Static Methods

        #region Private Static Methods

        private static bool TryFindTypeInExternalAssembly(string name, out Type type)
        {
            try
            {
                string[] names = name.Split(",".ToCharArray(), 2);
                string path = Path.Combine(Environment.CurrentDirectory, names[0]);
                if (File.Exists(path))
                {
                    Assembly a = Assembly.LoadFile(path);
                    type = a.GetType(names[1].Trim(' '), false, false);
                    return (type != null);
                }
                else
                {
                    type = null;
                    return false;
                }
            }
            catch (BadImageFormatException ex)
            {
                log.WriteError("CoreService", ex.Message);
                type = null;
                return false;
            }
        }

        private static bool IsExternalDefinition(string name)
        {
            string[] names = name.Split(",".ToCharArray(), 2);
            return names.Length == 2;
        }

        #endregion Private Static Methods
    }
}