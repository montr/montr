﻿namespace Montr.Settings
{
	/// <summary>
	/// Marker interface to register and use JsonTypeProvider and PolymorphicNewtonsoftJsonConverter for settings types,
	/// conversion works with SettingsJsonConverterWrapper.
	/// </summary>
	public interface ISettingsType
	{
		// ReSharper disable once InconsistentNaming
		string __TypeCode { get; }
	}
}
