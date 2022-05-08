using System;
using System.Reflection;

namespace Common.Reflection
{
    public struct ReflectedAttribute
    {
        private Type type;
        public Type Type { get => type; }
        private string name;
        public string Name { get => name; }
        private string readableName;
        public string ReadableName { get => readableName; }

        private PropertyInfo propertyInfo;
        public PropertyInfo PropertyInfo { get => propertyInfo; }

        public ReflectedAttribute(Type type, PropertyInfo propertyInfo, string name, string readableName)
        {
            this.type = type;
            this.propertyInfo = propertyInfo;
            this.name = name;
            this.readableName = readableName;
        }
    }

    public struct ReflectedMethod
    {
        private MethodInfo _methodInfo;
        public MethodInfo MethodInfo { get => _methodInfo; }

        public ReflectedMethod(MethodInfo methodInfo)
        {
            this._methodInfo = methodInfo;
        }
    }

    public struct ReflectedField
    {
        public FieldInfo FieldInfo;

        public ReflectedField(FieldInfo fieldInfo)
        {
            this.FieldInfo = fieldInfo;
        }
    }
}