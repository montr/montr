using System;
using System.Collections.Generic;
using Montr.Metadata.Models;

namespace Montr.Metadata.Services
{
	public interface IFieldProvider
	{
		FieldPurpose FieldPurpose { get; }

		Type FieldType { get; }

		/// <summary>
		/// Get metadata to edit properties of fields of this type.
		/// </summary>
		/// <returns></returns>
		IList<FieldMetadata> GetMetadata();

		// todo:
		// move to other service?
		// split read from client and validate?
		// metadata should be passed to validate.
		bool Validate(object value, out object parsed, out string[] errors);

		/// <summary>
		/// Read string value from storage and return object.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		object ReadFromStorage(string value);

		/// <summary>
		/// Write object value to string for storage.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		string WriteToStorage(object value);
	}

	public enum FieldPurpose
	{
		Information,
		Content
	}
}
