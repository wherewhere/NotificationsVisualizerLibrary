using System.Collections.Generic;

namespace NotificationsVisualizerLibrary.Model
{
    internal class ObjectModelObjectFromStaticMethod : IObjectModelValue
    {
        public const string ROOT_JS_NAMESPACE = "notifLib";

        public string ClassName { get; set; }

        public string MethodName { get; set; }

        public ObjectModelObjectFromStaticMethod(string className, string methodName, params IObjectModelValue[] parameterValues)
        {
            ClassName = className;
            MethodName = methodName;

            foreach (IObjectModelValue cVal in parameterValues)
            {
                ParameterValues.Add(cVal);
            }
        }

        public List<IObjectModelValue> ParameterValues { get; private set; } = [];

        public override string ToString() => $"{ClassName}.{MethodName}({string.Join(", ", ParameterValues)})";

        public string ToCPlusPlusString(List<string> declaredVars) => $"{ClassName}::{MethodName}({string.Join(", ", ParameterValues)})";

        public string ToJavascriptString() => ToJavascriptString([]);

        public string ToJavascriptString(List<string> declaredVars) => $"{ROOT_JS_NAMESPACE}.{ClassName}.{ObjectModelObject.ToLowerCamelCase(MethodName)}({string.Join(", ", ParameterValues)})";
    }
}
