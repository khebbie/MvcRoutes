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
            var methodArgs = new object[] {RouteTable.Routes};

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
            catch (MissingMethodException ex)
            {
                var methodInfo = o.Item2.GetMethod(methodName);
                
                object result = null;
                ParameterInfo[] parameters = methodInfo.GetParameters();
                object parameter2 = Activator.CreateInstance(parameters[1].ParameterType, null);

                methodArgs = new object[] { methodArgs[0], parameter2 };
                // Dynamically Invoke the method
                o.Item2.InvokeMember(methodName,
                                     BindingFlags.Default | BindingFlags.InvokeMethod,
                                     null,
                                     o.Item1,
                                     methodArgs);
                Console.WriteLine(ex);
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