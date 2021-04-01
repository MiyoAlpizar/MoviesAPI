using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoviesAPI.Helpers
{
    public class TypeBinder<T> : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var propertyName = bindingContext.ModelName;
            var valueProvider = bindingContext.ValueProvider.GetValue(propertyName);
            if (valueProvider == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }
            try
            {
                var value = JsonConvert.DeserializeObject<T>(valueProvider.FirstValue);
                bindingContext.Result = ModelBindingResult.Success(value);
            }
            catch (Exception)
            {
                bindingContext.ModelState.TryAddModelError(propertyName, $"Invalid type for {nameof(T)}");
            }
            return Task.CompletedTask;
        }
    }
}
