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
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Text;
using Hl7.Fhir.Serialization;
using System.IO;

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
            _fhirInputFormatter = new JsonFhirInputFormatter();
        }

        public  Task BindModelAsync(ModelBindingContext bindingContext)
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