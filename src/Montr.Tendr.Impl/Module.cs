﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Montr.Core;
using Montr.Core.Services;
using Montr.Tendr.Impl.Services;

namespace Montr.Tendr.Impl
{
	// ReSharper disable once UnusedType.Global
	public class Module : IModule
	{
		public void Configure(IAppBuilder appBuilder)
		{
			appBuilder.Services.AddSingleton<IContentProvider, ContentProvider>();
		}
	}
}
