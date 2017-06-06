using System.Collections.Generic;
using Hl7.Fhir.Model;

namespace Spark.Engine.Interfaces
{
    interface IResourceValidator
    {
        IEnumerable<OperationOutcome> Validate(Resource resource);
    }
}
