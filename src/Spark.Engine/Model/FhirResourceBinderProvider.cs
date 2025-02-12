﻿using System;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Spark.Engine.Model
{
    public class FhirResourceBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(Resource))
            {
                return new BinderTypeModelBinder(typeof(FhirResourceBinder));
            }

            return null;
        }
    }
}