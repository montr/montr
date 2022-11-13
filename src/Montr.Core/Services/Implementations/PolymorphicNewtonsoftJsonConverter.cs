using System;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Montr.Core.Services.Implementations
{
	public class PolymorphicNewtonsoftJsonConverter : JsonConverter
	{
		private readonly Type _baseType;
		private readonly string _typeProp;
		private readonly IJsonTypeProvider _typeProvider;

		public PolymorphicJsonConvertMode ConvertMode { get; init; }

		public bool UseBaseTypeIfTypeNotFound { get; init; }

		/// <summary>
		/// Considered harmful - cannot deserialize complex trees.
		/// See also https://stackoverflow.com/questions/19307752/deserializing-polymorphic-json-classes-without-type-information-using-json-net
		/// </summary>
		public bool UsePopulate { get; init; }

		public PolymorphicNewtonsoftJsonConverter(Type baseType, string typeProp, IJsonTypeProvider typeProvider)
		{
			_baseType = baseType;
			_typeProp = typeProp;
			_typeProvider = typeProvider;
		}

		public override bool CanConvert(Type objectType)
		{
			switch (ConvertMode)
			{
				case PolymorphicJsonConvertMode.Base:
					return _baseType == objectType;
				case PolymorphicJsonConvertMode.BaseAndInheritors:
					return _baseType.IsAssignableFrom(objectType);
				default:
					throw new InvalidOperationException($"Mode {ConvertMode} is not supported.");
			}
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var jObject = JObject.Load(reader);

			string typeCode = null;

			if (jObject.TryGetValue(_typeProp, StringComparison.OrdinalIgnoreCase, out var typeJObject))
			{
				typeCode = (string)typeJObject;
			}

			Type resultType = null;

			if (typeCode != null)
			{
				_typeProvider.TryGetType(typeCode, out resultType);
			}

			if (resultType == null && UseBaseTypeIfTypeNotFound)
			{
				resultType = _baseType;
			}

			if (resultType == null)
			{
				throw new InvalidOperationException($"Unable to determine result type for \"{_typeProp}\" : \"{typeCode}\" of {_baseType}.");
			}

			object result;

			if (UsePopulate)
			{
				result = Activator.CreateInstance(resultType);

				if (result == null)
				{
					throw new InvalidOperationException($"Unable to create instance of {resultType} for {typeCode} of {_baseType}.");
				}

				serializer.Populate(jObject.CreateReader(), result);
			}
			else
			{
				result = jObject.ToObject(resultType, serializer);
			}

			return result;
		}

		public override bool CanWrite => false;

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			// serializer.Serialize(writer, value);

			throw new NotImplementedException();
		}
	}

	public class PolymorphicNewtonsoftJsonConverter<TBaseType> : PolymorphicNewtonsoftJsonConverter
	{
		public PolymorphicNewtonsoftJsonConverter(
			Expression<Func<TBaseType, object>> typePropExpr, IJsonTypeProvider typeProvider)
			: base(typeof(TBaseType), ExpressionHelper.GetMemberName(typePropExpr), typeProvider)
		{
		}
	}

	public enum PolymorphicJsonConvertMode : byte
	{
		Base,

		BaseAndInheritors
	}
}
