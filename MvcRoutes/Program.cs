using System;
using System.Collections.Generic;
using System.Linq;
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
            ReflectionUtil.AssemblyName = args[0];
            const string className = "MvcApplication";
            const string methodName = "RegisterRoutes";

            ReflectionUtil.CallMethod(methodName, className);
        }

        private static string GetMethodsString(Route rt)
        {
            string methodsString = string.Empty;
            foreach (var constraint in rt.Constraints)
            {
                if (constraint.Key == "HttpVerbs")
                {
                    var allowedMethods = ((HttpMethodConstraint) constraint.Value).AllowedMethods.ToList();
                    methodsString = string.Join(", ", allowedMethods);
                }
            }

            if(methodsString == string.Empty)
            {
                methodsString = ResolveMethodUsingAttributes(rt);
            }
            return methodsString;
        }

        private static Tuple<string, string> GetControllerAndActionNameFromRoute(Route rt)
        {
            string controllername = string.Empty;
            string actionName = string.Empty;
            const string controllerKey = "controller";
            const string actionKey = "Action";
            if (rt.Defaults.ContainsKey(controllerKey))
            {
                controllername = rt.Defaults[controllerKey].ToString();
                controllername += "Controller";
            }

            if (rt.Defaults.ContainsKey(actionKey))
                actionName = rt.Defaults[actionKey].ToString();

            return new Tuple<string, string>(controllername, actionName);
        }

        private static string ResolveMethodUsingAttributes(Route rt)
        {
            if (rt.Defaults == null)
                return string.Empty;

            var controllerAndActionNameFromRoute = GetControllerAndActionNameFromRoute(rt);
            string controllerName = controllerAndActionNameFromRoute.Item1;
            string actionName = controllerAndActionNameFromRoute.Item2;
            
            if (string.IsNullOrEmpty(actionName))
                return string.Empty;
            
            var controllerType = ReflectionUtil.GetType(controllerName);

            if(controllerType == null)
            {
                Console.WriteLine("Controller not found even though is it marked as a controller for an action: " + controllerName);
                return string.Empty;
            }
            var actionMethodInfo = controllerType.GetMethod(actionName);

            var customAttributes = Attribute.GetCustomAttributes(actionMethodInfo);
            
            return  GetMethodsFromAttributes(customAttributes);
        }

        private static string GetMethodsFromAttributes(IEnumerable<Attribute> customAttributes)
        {
            var attributeToMethodName = new Dictionary<string, string>
                                            {
                                                {"HttpGetAttribute", "GET"},
                                                {"HttpPostAttribute", "POST"},
                                                {"HttpDeleteAttribute", "DELETE"},
                                                {"HttpPutAttribute", "PUT"}
                                            };
            var methodList = new List<string>();
            foreach (var customAttribute in customAttributes)
            {
                var attributeName = customAttribute.GetType().Name;
                if (attributeToMethodName.ContainsKey(attributeName))
                {
                    methodList.Add(attributeToMethodName[attributeName]);
                }
            }
            return  string.Join(",", methodList);
        }
    }
}
