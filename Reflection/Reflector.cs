using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AttributeTable = System.Collections.Generic.Dictionary<System.Type, object>;

namespace Common.Reflection
{
    public class Reflector
    {
        private static Dictionary<Type, AttributeTable> _attributeCache = new Dictionary<Type, AttributeTable>();
	    private static Dictionary<Type, AttributeTable> _fieldCache = new Dictionary<Type, AttributeTable>();
	    private static Dictionary<Type, AttributeTable> _methodCache = new Dictionary<Type, AttributeTable>();

	    public static ReflectedAttribute[] MapSetters<T>(Object obj) where T : Attribute
	    {
		    if (_attributeCache.ContainsKey(obj.GetType()))
		    {
			    if (_attributeCache[obj.GetType()].ContainsKey(typeof(T)))
			    {
				    return _attributeCache[obj.GetType()][typeof(T)] as ReflectedAttribute[];
			    }
		    }

		    MemberInfo[] members = obj.GetType().FindMembers(MemberTypes.Property, BindingFlags.FlattenHierarchy | BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.Public, null, null);
		    Dictionary<string, ReflectedAttribute> namedAttributes = new Dictionary<string, ReflectedAttribute>();

		    foreach(MemberInfo member in members)
		    {
			    object[] editableProperties = GetProperties<T>(member, true);
			    if (editableProperties.Length > 0)
			    {
				    if (member.DeclaringType == null)
				    {
					    continue;
				    }

				    PropertyInfo point = member as PropertyInfo;
				    Type baseType = member.DeclaringType.BaseType;
				    bool hasInheritedProperty = baseType != null ? baseType.GetProperties().Any(property => property.Name == point.Name) : false;
				    bool toAddOrOverride = true;

				    if (namedAttributes.ContainsKey(point.Name)) toAddOrOverride = hasInheritedProperty;
				    if (toAddOrOverride) namedAttributes[point.Name] = new ReflectedAttribute(point.PropertyType, point, point.Name, "");
			    }
		    }

		    AttributeTable table = new AttributeTable();
		    table.Add(typeof(T), namedAttributes.Values.ToArray());

		    _attributeCache.Add(obj.GetType(), table);

		    return _attributeCache[obj.GetType()][typeof(T)] as ReflectedAttribute[];
	    }

	    public static ReflectedField[] MapFields<T>(Object obj)
	    {
		    if (_fieldCache.ContainsKey(obj.GetType()))
		    {
			    if (_fieldCache[obj.GetType()].ContainsKey(typeof(T)))
			    {
				    return _fieldCache[obj.GetType()][typeof(T)] as ReflectedField[];
			    }
		    }

		    FieldInfo[] fields = obj.GetType().GetFields();
		    ArrayList fieldList = new ArrayList();

		    foreach(FieldInfo field in fields)
		    {
			    object[] tagged = field.GetCustomAttributes(typeof(T), true);

			    if (tagged.Length > 0)
			    {
				    ReflectedField reflectedMethod = new ReflectedField(field);
				    fieldList.Add(reflectedMethod);
			    }
		    }

		    AttributeTable table = new AttributeTable();
		    table.Add(typeof(T), fieldList.ToArray(typeof(ReflectedField)));

		    _fieldCache.Add(obj.GetType(), table);

		    return _fieldCache[obj.GetType()][typeof(T)] as ReflectedField[];
	    }

	    public static ReflectedMethod[] MapMethods<T>(Object obj)
	    {
		    if (_methodCache.ContainsKey(obj.GetType()))
		    {
			    if (_methodCache[obj.GetType()].ContainsKey(typeof(T)))
			    {
				    return _methodCache[obj.GetType()][typeof(T)] as ReflectedMethod[];
			    }
		    }

		    MethodInfo[] methods=  obj.GetType().GetMethods(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod);
		    ArrayList methodList = new ArrayList();

		    foreach(MethodInfo method in methods)
		    {
			    object[] tagged = method.GetCustomAttributes(typeof(T), true);

			    if (tagged.Length > 0)
			    {
				    ReflectedMethod reflectedMethod = new ReflectedMethod(method);
				    methodList.Add(reflectedMethod);
			    }
		    }

		    AttributeTable table = new AttributeTable();
		    table.Add(typeof(T), methodList.ToArray(typeof(ReflectedMethod)));

		    _methodCache.Add(obj.GetType(), table);

		    return _methodCache[obj.GetType()][typeof(T)] as ReflectedMethod[];
	    }
	    
	    private static object[] GetProperties<T>(MemberInfo member, bool inherit)
	    {
		    return member.GetCustomAttributes(typeof(T), inherit);
	    }
    }
}