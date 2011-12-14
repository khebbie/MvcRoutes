using System;
using System.Reflection;
using System.Web.Routing;

namespace MvcRoutes
{
    static class  ReflectionUtil
    {
        static public string AssemblyName { get; set; }
        public static void CallMethod(string methodName, string className)
        {
            Object[] methodArgs = new[] {RouteTable.Routes};

            var o = GetObject(className);
            try
            {
                // Dynamically Invoke the method
                o.Item2.InvokeMember(methodName,
                                     BindingFlags.Default | BindingFlags.InvokeMethod,
                                     null,
                                     o.Item1,
                                     methodArgs);  
            }
            catch (ReflectionTypeLoadException ex)
            {
                Console.WriteLine(ex.LoaderExceptions);
            }
        }

        public static Type GetType(string className)
        {
            Assembly sampleAssembly = Assembly.LoadFrom(AssemblyName);
            try
            {
                foreach (Type type in sampleAssembly.GetTypes())
                {
                    if (type.FullName.EndsWith("." + className))
                    {
                        return type;
                    }
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                Console.WriteLine(ex.LoaderExceptions);
            }
            return null;
        }

        public static Tuple<object, Type> GetObject(string className)
        {
            try
            {
                var type = GetType(className);

                object classObj = Activator.CreateInstance(type);

                return new Tuple<object, Type>(classObj, type);
                    
            }
            catch (ReflectionTypeLoadException ex)
            {
                Console.WriteLine(ex.LoaderExceptions);
            }
            return null;
        }
    }
}