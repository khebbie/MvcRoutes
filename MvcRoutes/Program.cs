using System;
using System.Linq;
using System.Reflection;
using System.Web.Routing;

namespace MvcRoutes
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Count() != 1)
            {
                Console.WriteLine("Please send the path to your dll as the first argument");
                return;
            }
            CallRegisterRoutes(args);

            foreach (var route in RouteTable.Routes)
            {
                var rt = (Route)route;
                string methodsString = GetMethodsString(rt);
                
                Console.WriteLine(rt.Url + " httpMethods: " + methodsString);
            }
        }

        private static void CallRegisterRoutes(string[] args)
        {
            string ClassName = "MvcApplication";
            string MethodName = "RegisterRoutes";

            Object[] methodArgs = new[] {RouteTable.Routes};

            Assembly SampleAssembly = Assembly.LoadFrom(args[0]);
            try
            {
                foreach (Type type in SampleAssembly.GetTypes())
                {
                    if (type.FullName.EndsWith("." + ClassName))
                    {
                        // create an instance of the object

                        object ClassObj = Activator.CreateInstance(type);

                        // Dynamically Invoke the method

                        type.InvokeMember(MethodName,
                                          BindingFlags.Default | BindingFlags.InvokeMethod,
                                          null,
                                          ClassObj,
                                          methodArgs);
                    }
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                Console.WriteLine(ex.LoaderExceptions);
            }
        }

        private static string GetMethodsString(Route rt)
        {
            string methodsString = "GET, POST, DELETE, PUT";
            foreach (var constraint in rt.Constraints)
            {
                if (constraint.Key == "HttpVerbs")
                {
                    var allowedMethods = ((HttpMethodConstraint) constraint.Value).AllowedMethods.ToList();
                    methodsString = string.Join(", ", allowedMethods);
                }
            }
            return methodsString;
        }
    }
}
