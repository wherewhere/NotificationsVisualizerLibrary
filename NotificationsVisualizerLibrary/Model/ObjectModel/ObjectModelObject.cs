using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NotificationsVisualizerLibrary.Model
{
    internal interface IObjectModelValue
    {
        string ToJavascriptString();
        string ToJavascriptString(List<string> declaredVars);
        string ToCPlusPlusString(List<string> declaredVars);
    }

    internal partial class ObjectModelObject : IObjectModelValue, IEnumerable<KeyValuePair<string, IObjectModelValue>>
    {
        public const string ROOT_JS_NAMESPACE = "notifLib";

        public string Name { get; set; }

        public ObjectModelObject(string name, params IObjectModelValue?[] constructorValues)
        {
            Name = name;

            foreach (IObjectModelValue? cVal in constructorValues)
            {
                ConstructorValues.Add(cVal);
            }
        }

        public List<IObjectModelValue?> ConstructorValues { get; private set; } = [];

        public List<KeyValuePair<string, IObjectModelValue?>> PropertyValues { get; private set; } = [];

        public void Add(string propertyName, object? propertyValue)
        {
            if (propertyValue == null)
            {
                return;
            }

            if (propertyValue is not IObjectModelValue objectPropertyValue)
            {
                objectPropertyValue = Create(propertyValue);
            }

            PropertyValues.Add(new KeyValuePair<string, IObjectModelValue?>(propertyName, objectPropertyValue));
        }

        public List<KeyValuePair<ObjectModelBindingPropertyAttribute, string>> Bindings { get; private set; } = [];

        public void AddBinding(ObjectModelBindingPropertyAttribute attr, string bindingName)
        {
            if (bindingName == null)
            {
                return;
            }

            Bindings.Add(new KeyValuePair<ObjectModelBindingPropertyAttribute, string>(attr, bindingName));
        }

        public override string ToString()
        {
            string answer = $"new {Name}({string.Join(", ", ConstructorValues)})";

            if (PropertyValues.Count > 0 || Bindings.Count > 0)
            {
                string propertyValues = "\n{";
                bool addedProperty = false;

                foreach (KeyValuePair<string, IObjectModelValue?> pVal in PropertyValues.Where(i => i.Value != null))
                {
                    string? val = pVal.Value!.ToString();
                    if (val == null)
                    {
                        continue;
                    }

                    propertyValues += $"\n    {pVal.Key} = ";

                    propertyValues += IndentNewLines(val, 4);

                    propertyValues += ",";

                    addedProperty = true;
                }

                if (Bindings.Count > 0)
                {
                    foreach (KeyValuePair<ObjectModelBindingPropertyAttribute, string> b in Bindings)
                    {
                        propertyValues += $"\n    {b.Key.PropertyName} = new {b.Key.BindableTypeName}({new ObjectModelString(b.Value)}),";
                    }

                    addedProperty = true;
                }

                // Trim the extra ending ","
                if (addedProperty)
                {
                    propertyValues = propertyValues[..^1];
                    propertyValues += "\n}";
                    answer += propertyValues;
                }
            }

            return answer;
        }

        public string ToJavascriptString() => ToJavascriptString([]);

        public string ToJavascriptString(List<string> declaredVars)
        {
            string jsVar = Var();
            string answer = "";
            string propertyValues = "";

            answer += $"{DeclareJsVar(declaredVars)} = new {ROOT_JS_NAMESPACE}.{Name}({string.Join(", ", ConstructorValues)});";

            // If there's complex properties, we need to construct those first
            foreach (KeyValuePair<string, IObjectModelValue?> pVal in PropertyValues.Where(i => i.Value is ObjectModelObject))
            {
                string val = pVal.Value!.ToJavascriptString(declaredVars);
                if (val != null)
                {
                    answer += $"\n{val}\n";
                    answer += $"{jsVar}.{ToLowerCamelCase(pVal.Key)} = {(pVal.Value as ObjectModelObject)!.Var()};\n";
                }
            }

            if (PropertyValues.Count > 0)
            {
                foreach (KeyValuePair<string, IObjectModelValue?> pVal in PropertyValues.Where(i => i.Value is not null and not ObjectModelObject))
                {
                    string val = pVal.Value!.ToJavascriptString(declaredVars);
                    if (val == null)
                    {
                        continue;
                    }

                    // If list, we construct an item then add it, then repeat
                    if (pVal.Value is ObjectModelListInitialization initialization)
                    {
                        IObjectModelValue[] listValues = [.. initialization.Values.OfType<IObjectModelValue>()];
                        if (listValues.Length > 0)
                        {
                            propertyValues += "\n";

                            foreach (IObjectModelValue listItem in listValues)
                            {
                                // If complex value, need to construct it first
                                if (listItem is ObjectModelObject @object)
                                {
                                    propertyValues += $"\n{listItem.ToJavascriptString(declaredVars)}";
                                    propertyValues += $"\n{jsVar}.{ToLowerCamelCase(pVal.Key)}.push({@object.Var()});";
                                }
                                else
                                {
                                    propertyValues += $"\n{jsVar}.{ToLowerCamelCase(pVal.Key)}.push({listItem.ToJavascriptString(declaredVars)});";
                                }

                                propertyValues += "\n";
                            }
                        }
                    }
                    else
                    {
                        propertyValues += $"\n{jsVar}.{ToLowerCamelCase(pVal.Key)} = {val};";
                    }
                }
            }

            if (Bindings.Count > 0)
            {
                foreach (KeyValuePair<ObjectModelBindingPropertyAttribute, string> b in Bindings)
                {
                    ObjectModelEnum enumValue = new(Name + "BindableProperty", b.Key.PropertyName);

                    propertyValues += $"\n{jsVar}.bindings.insert({enumValue.ToJavascriptString()}, {new ObjectModelString(b.Value).ToJavascriptString()});";
                }
            }

            answer += propertyValues;

            return answer;
        }

        public string ToCPlusPlusString() => ToCPlusPlusString([]);

        public string ToCPlusPlusString(List<string> declaredVars)
        {
            string cppVar = Var();
            string answer = "";
            string propertyValues = "";

            answer += $"{DeclareCPlusPlusVar(declaredVars)} = ref new {Name}({string.Join(", ", ConstructorValues)});";

            // If there's complex properties, we need to construct those first
            foreach (KeyValuePair<string, IObjectModelValue?> pVal in PropertyValues.Where(i => i.Value is ObjectModelObject))
            {
                string val = pVal.Value!.ToCPlusPlusString(declaredVars);
                if (val != null)
                {
                    answer += $"\n{val}\n";
                    answer += $"{cppVar}->{pVal.Key} = {(pVal.Value as ObjectModelObject)!.Var()};\n";
                }
            }

            if (PropertyValues.Count > 0)
            {
                foreach (KeyValuePair<string, IObjectModelValue?> pVal in PropertyValues.Where(i => i.Value is not null and not ObjectModelObject))
                {
                    string val = pVal.Value!.ToCPlusPlusString(declaredVars);
                    if (val == null)
                    {
                        continue;
                    }

                    // If list, we construct an item then add it, then repeat
                    if (pVal.Value is ObjectModelListInitialization initialization)
                    {
                        IObjectModelValue[] listValues = [.. initialization.Values.OfType<IObjectModelValue>()];
                        if (listValues.Length > 0)
                        {
                            propertyValues += "\n";

                            foreach (IObjectModelValue listItem in listValues)
                            {
                                // If complex value, need to construct it first
                                if (listItem is ObjectModelObject @object)
                                {
                                    propertyValues += $"\n{listItem.ToCPlusPlusString(declaredVars)}";
                                    propertyValues += $"\n{cppVar}->{pVal.Key}->Append({@object.Var()});";
                                }
                                else
                                {
                                    propertyValues += $"\n{cppVar}->{pVal.Key}->Append({listItem.ToCPlusPlusString(declaredVars)});";
                                }

                                propertyValues += "\n";
                            }
                        }
                    }
                    else
                    {
                        propertyValues += $"\n{cppVar}->{pVal.Key} = {val};";
                    }
                }
            }

            if (Bindings.Count > 0)
            {
                foreach (KeyValuePair<ObjectModelBindingPropertyAttribute, string> b in Bindings)
                {
                    ObjectModelEnum enumValue = new($"{Name}BindableProperty", b.Key.PropertyName);

                    propertyValues += $"\n{cppVar}->Bindings->Insert({enumValue.ToCPlusPlusString(declaredVars)}, {new ObjectModelString(b.Value).ToCPlusPlusString(declaredVars)});";
                }
            }

            answer += propertyValues;

            return answer;
        }

        private string DeclareJsVar(List<string> declaredVars)
        {
            string jsVar = Var();

            if (declaredVars.Contains(jsVar))
            {
                return jsVar;
            }
            else
            {
                declaredVars.Add(jsVar);
                return $"var {jsVar}";
            }
        }

        private string DeclareCPlusPlusVar(List<string> declaredVars)
        {
            string v = Var();

            if (declaredVars.Contains(v))
            {
                return v;
            }
            else
            {
                declaredVars.Add(v);
                return $"auto {v}";
            }
        }

        private string Var() => ToLowerCamelCase(Name);

        internal static string ToLowerCamelCase(string str) => $"{str[..1].ToLower()}{str[1..]}";

        public static string IndentNewLines(string str, int amount)
        {
            string indentation = string.Join(" ", new string[amount + 1]);
            return str.Replace("\n", "\n" + indentation);
        }

        public IEnumerator<KeyValuePair<string, IObjectModelValue>> GetEnumerator() => PropertyValues.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [return: NotNullIfNotNull(nameof(obj))]
        public static IObjectModelValue? Create(object? obj) => obj switch
        {
            IObjectModelValue value => value,
            string @string => new ObjectModelString(@string),
            double @double => new ObjectModelDouble(@double),
            Enum @enum => new ObjectModelEnum(RetrieveEnumName(@enum), @enum.ToString()),
            Uri uri => new ObjectModelUri(uri),
            bool @bool => new ObjectModelBool(@bool),
            DateTime dateTime => new ObjectModelDateTime(dateTime),
            int @int => new ObjectModelInt(@int),
            _ => null,
        };

        private static string RetrieveEnumName(Enum e)
        {
            Type typeInfo = e.GetType();

            return typeInfo.Name;
        }
    }

    internal class ObjectModelListInitialization : IObjectModelValue
    {
        public List<IObjectModelValue?> Values { get; private set; } = [];

        public ObjectModelListInitialization() { }

        public ObjectModelListInitialization(IEnumerable<IObjectModelValue?> values) => Values.AddRange(values);

        public void Add(IObjectModelValue? value) => Values.Add(value);

        public override string? ToString()
        {
            if (Values.Count == 0)
            {
                return null;
            }

            IEnumerable<string> valuesWithIndentation = Values.OfType<IObjectModelValue>().Select(i => ObjectModelObject.IndentNewLines(i.ToString()!, 4));

            return $"\n{{\n    {string.Join(",\n    ", valuesWithIndentation)}\n}}";
        }

        public string ToJavascriptString() => "Shouldn't get hit";

        public string ToJavascriptString(List<string> declaredVars) => "Shouldn't get hit";

        public string ToCPlusPlusString(List<string> declaredVars) => "Shouldn't get hit";
    }

    internal class ObjectModelString(string value) : IObjectModelValue
    {
        public string Value { get; private set; } = value;

        public override string ToString() => $"\"{Value.Replace("\"", "\\\"")}\"";

        public string ToJavascriptString() => ToString();

        public string ToJavascriptString(List<string> declaredVars) => ToJavascriptString();

        public string ToCPlusPlusString(List<string> declaredVars) => ToString();
    }

    internal class ObjectModelDouble(double value) : IObjectModelValue
    {
        public double Value { get; private set; } = value;

        public override string ToString() => Value.ToString();

        public string ToJavascriptString() => ToString();

        public string ToJavascriptString(List<string> declaredVars) => ToJavascriptString();

        public string ToCPlusPlusString(List<string> declaredVars) => ToString();
    }

    internal class ObjectModelInt(int value) : IObjectModelValue
    {
        public int Value { get; private set; } = value;

        public override string ToString() => Value.ToString();

        public string ToJavascriptString() => ToString();

        public string ToJavascriptString(List<string> declaredVars) => ToJavascriptString();

        public string ToCPlusPlusString(List<string> declaredVars) => ToString();
    }

    internal class ObjectModelEnum(string enumName, string enumValue) : IObjectModelValue
    {
        public string EnumName { get; private set; } = enumName;
        public string EnumValue { get; private set; } = enumValue;

        public override string ToString() => $"{EnumName}.{EnumValue}";

        public string ToJavascriptString() => $"{ObjectModelObject.ROOT_JS_NAMESPACE}.{EnumName}.{ObjectModelObject.ToLowerCamelCase(EnumValue)}";

        public string ToJavascriptString(List<string> declaredVars) => ToJavascriptString();

        public string ToCPlusPlusString(List<string> declaredVars) => $"{EnumName}::{EnumValue}";
    }

    internal class ObjectModelUri(Uri value) : IObjectModelValue
    {
        public Uri Value { get; private set; } = value;

        public override string ToString() => $"new Uri({new ObjectModelString(Value.OriginalString)})";

        public string ToJavascriptString() => $"new Windows.Foundation.Uri({new ObjectModelString(Value.OriginalString).ToJavascriptString()})";

        public string ToJavascriptString(List<string> declaredVars) => ToJavascriptString();

        public string ToCPlusPlusString(List<string> declaredVars) => $"ref new Windows::Foundation::Uri({new ObjectModelString(Value.OriginalString).ToCPlusPlusString(declaredVars)})";
    }

    internal class ObjectModelBool(bool value) : IObjectModelValue
    {
        public bool Value { get; set; } = value;

        public override string ToString() => Value.ToString().ToLower();

        public string ToCPlusPlusString(List<string> declaredVars) => ToString();

        public string ToJavascriptString() => ToString();

        public string ToJavascriptString(List<string> declaredVars) => ToString();
    }

    internal class ObjectModelDateTime(DateTime value) : IObjectModelValue
    {
        public DateTime Value { get; set; } = value;

        public override string ToString() => $"new DateTime({Value.Year}, {Value.Month}, {Value.Day}, {Value.Hour}, {Value.Minute}, {Value.Second}, DateTimeKind.{Value.Kind})";

        public string ToJavascriptString() => $"new Date({new ObjectModelString(Value.ToString("o"))})";

        public string ToJavascriptString(List<string> declaredVars) => ToJavascriptString();

        public string ToCPlusPlusString(List<string> declaredVars) => $"\"Conversion currently not supported\"";
    }
}
