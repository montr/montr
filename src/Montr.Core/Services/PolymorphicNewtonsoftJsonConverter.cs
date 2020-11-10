﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Montr.Core.Services
{
	public class PolymorphicNewtonsoftJsonConverter<TBaseType> : JsonConverter
	{
		private readonly string _typeProp;
		private readonly IDictionary<string, Type> _typeMap;

		public PolymorphicJsonConverterMode Mode { get; set; }

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
			return Mode switch
			{
				PolymorphicJsonConverterMode.Exact
					=> typeof(TBaseType) == objectType,

				PolymorphicJsonConverterMode.Inheritors
					=> typeof(TBaseType) != objectType &&
					   typeof(TBaseType).IsAssignableFrom(objectType),

				_ => throw new InvalidEnumArgumentException(nameof(Mode))
			};
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var jObject = JObject.Load(reader);

			var typeCode = (string)jObject[_typeProp];

			var type = _typeMap[typeCode];

			return jObject.ToObject(type, serializer);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			serializer.Serialize(writer, value);
		}
	}

	public enum PolymorphicJsonConverterMode : byte
	{
		/// <summary>
		/// Exact type check. Default mode.
		/// </summary>
		Exact,

		/// <summary>
		/// Inheritors types check.
		/// </summary>
		Inheritors
	}
}
