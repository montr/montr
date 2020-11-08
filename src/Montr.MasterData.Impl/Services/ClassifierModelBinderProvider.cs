using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Montr.MasterData.Models;

namespace Montr.MasterData.Impl.Services
{
	public class ClassifierModelBinderProvider : IModelBinderProvider
	{
		public IModelBinder GetBinder(ModelBinderProviderContext context)
		{
			Console.WriteLine("!!! --- " + context.Metadata.ModelType);

			if (context.Metadata.ModelType == typeof(Classifier))
			{
				var subclasses = new[] { typeof(Classifier), typeof(Numerator), };

				var binders = new Dictionary<Type, (ModelMetadata, IModelBinder)>();
				foreach (var type in subclasses)
				{
					var modelMetadata = context.MetadataProvider.GetMetadataForType(type);

					binders[type] = (modelMetadata, context.CreateBinder(modelMetadata));
				}

				return new ClassifierModelBinder(binders);

				// return new BinderTypeModelBinder(typeof(ClassifierModelBinder));
			}

			return null;
		}
	}

	public class ClassifierModelBinder : IModelBinder
	{
		private readonly Dictionary<Type, (ModelMetadata, IModelBinder)> _binders;

		public ClassifierModelBinder(Dictionary<Type, (ModelMetadata, IModelBinder)> binders)
		{
			_binders = binders;
		}

		public async Task BindModelAsync(ModelBindingContext bindingContext)
		{
			var typeCode = ModelNames.CreatePropertyModelName(bindingContext.ModelName, nameof(Classifier.TypeCode));

			IModelBinder modelBinder;
			ModelMetadata modelMetadata;

			if (typeCode == "numerator")
			{
				(modelMetadata, modelBinder) = _binders[typeof(Numerator)];
			}
			else
			{
				bindingContext.Result = ModelBindingResult.Failed();
				return;
			}

			var newBindingContext = DefaultModelBindingContext.CreateBindingContext(
				bindingContext.ActionContext,
				bindingContext.ValueProvider,
				modelMetadata,
				bindingInfo: null,
				bindingContext.ModelName);

			await modelBinder.BindModelAsync(newBindingContext);
			bindingContext.Result = newBindingContext.Result;

			if (newBindingContext.Result.IsModelSet)
			{
				// Setting the ValidationState ensures properties on derived types are correctly
				bindingContext.ValidationState[newBindingContext.Result] = new ValidationStateEntry
				{
					Metadata = modelMetadata
				};
			}
		}
	}
}
