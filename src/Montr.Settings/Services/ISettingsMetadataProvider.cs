using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Montr.Metadata.Models;

namespace Montr.Settings.Services
{
	public interface ISettingsMetadataProvider
	{
		Task<ICollection<FieldMetadata>> GetMetadata(Type type);
	}
}
