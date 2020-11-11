using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Montr.Core.Services
{
	public class PolymorphicNewtonsoftJsonConverter<TBaseType> : JsonConverter
	{
		private readonly string _typeProp;
		private readonly IDictionary<string, Type> _typeMap;

		public PolymorphicNewtonsoftJsonConverter(string typeProp, IDictionary<string, Type> typeMap)
		{
			_typeProp = typeProp;
			_typeMap = typeMap;
		}

		public PolymorphicNewtonsoftJsonConverter(Expression<Func<TBaseType, object>> typePropExpr, IDictionary<string, Type> typeMap)
		{
			_typeProp = ExpressionHelper.GetMemberName(typePropExpr).ToLowerInvariant();
			_typeMap = typeMap;
		}

		public override bool CanConvert(Type objectType)
		{
			return typeof(TBaseType) == objectType;
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var jObject = JObject.Load(reader);

			var typeCode = (string)jObject[_typeProp];

			var type = _typeMap[typeCode];

			return jObject.ToObject(type, serializer);
		}

		public override bool CanWrite => false;

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			// serializer.Serialize(writer, value);

			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// Considered harmful - cannot deserialize complex trees.
	/// See also https://stackoverflow.com/questions/19307752/deserializing-polymorphic-json-classes-without-type-information-using-json-net
	/// </summary>
	/// <typeparam name="TBaseType"></typeparam>
	public class PolymorphicNewtonsoftJsonConverterWithPopulate<TBaseType> : JsonConverter // where TBaseType : new()
	{
		private readonly string _typeProp;
		private readonly IDictionary<string, Type> _typeMap;

		public PolymorphicNewtonsoftJsonConverterWithPopulate(Expression<Func<TBaseType, object>> typePropExpr, IDictionary<string, Type> typeMap)
		{
			_typeProp = ExpressionHelper.GetMemberName(typePropExpr).ToLowerInvariant();
			_typeMap = typeMap;
		}

		public override bool CanConvert(Type objectType)
		{
			return typeof(TBaseType).IsAssignableFrom(objectType);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var jObject = JObject.Load(reader);

			var typeCode = (string)jObject[_typeProp];

			if (typeCode == null || _typeMap.TryGetValue(typeCode, out var type) == false)
			{
				type = typeof(TBaseType);
			}

			var item = Activator.CreateInstance(type);
			serializer.Populate(jObject.CreateReader(), item);
			return item;
		}

		public override bool CanWrite => false;

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}
