using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Spark.Core;
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
//            _fhirInputFormatter = new JsonFhirInputFormatter();
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }
            using (StreamReader reader
                = new StreamReader(bindingContext.HttpContext.Request.Body, Encoding.UTF8, true, 1024, true))
            {
                string bodyStr = reader.ReadToEnd();
                var resource = FhirParser.ParseResourceFromJson(bodyStr);
                bindingContext.Result = ModelBindingResult.Success(resource);
            }


            return Task.CompletedTask;
        }
    }
}