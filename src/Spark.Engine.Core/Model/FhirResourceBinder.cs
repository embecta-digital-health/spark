using System;
using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.CSharp.RuntimeBinder;
using Spark.Core;
using Spark.Engine.Core;
using Spark.Engine.Formatters;

// ReSharper disable ArrangeTypeModifiers

namespace Spark.Engine.Model
{
    class FhirResourceBinder : IModelBinder
    {
        private readonly IFhirStore _db;
        private JsonFhirInputFormatter _fhirInputFormatter;

        public FhirResourceBinder(IFhirStore db)
        {
            _db = db;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelType);
            var valueProviderResult2 = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueProviderResult.Length > 0)
            {
                var valueAsString = valueProviderResult.FirstValue;

                if (string.IsNullOrEmpty(valueAsString))
                {
                    throw new RuntimeBinderException();
//                    return _fallbackBinder.BindModelAsync(bindingContext);
                }

//                _fhirInputFormatter.ReadRequestBodyAsync();

//                var result = HtmlEncoder.Default.Encode(valueAsString);
                var result = new Practitioner(){Id = "newprac000001", Active = true};
                bindingContext.Result = ModelBindingResult.Success(result);
            }

            return TaskCache.CompletedTask;
        }
    }
}