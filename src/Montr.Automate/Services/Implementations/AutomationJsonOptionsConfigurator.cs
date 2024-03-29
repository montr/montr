﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Montr.Automate.Models;
using Montr.Core.Services.Implementations;

namespace Montr.Automate.Services.Implementations
{
	public class AutomationJsonOptionsConfigurator : IConfigureOptions<MvcNewtonsoftJsonOptions>
	{
		private readonly JsonTypeProvider<AutomationCondition> _conditionTypeProvider;
		private readonly JsonTypeProvider<AutomationAction> _actionTypeProvider;

		public AutomationJsonOptionsConfigurator(
			JsonTypeProvider<AutomationCondition> conditionTypeProvider, JsonTypeProvider<AutomationAction> actionTypeProvider)
		{
			_conditionTypeProvider = conditionTypeProvider;
			_actionTypeProvider = actionTypeProvider;
		}

		public void Configure(MvcNewtonsoftJsonOptions options)
		{
			options.SerializerSettings.Converters.Add(
				new PolymorphicNewtonsoftJsonConverter<AutomationCondition>(x => x.Type, _conditionTypeProvider));

			options.SerializerSettings.Converters.Add(
				new PolymorphicNewtonsoftJsonConverter<AutomationAction>(x => x.Type, _actionTypeProvider));
		}
	}
}
