using System;
using MediatR;
using Montr.Metadata.Models;

namespace Montr.Settings.Queries
{
	public class GetSettingsMetadata : MetadataRequest, IRequest<DataView>
	{
		public string EntityTypeCode { get; set; }

		public Guid? EntityUid { get; set; }

		public string OptionsTypeCode { get; set; }
	}
}
