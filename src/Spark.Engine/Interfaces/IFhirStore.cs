/* 
 * Copyright (c) 2014, Furore (info@furore.com) and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the BSD 3-Clause license
 * available at https://raw.github.com/furore-fhir/spark/master/LICENSE
 */

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Spark.Engine.Core;

namespace Spark.Core
{
    public interface IFhirStore
    {
        // primary keys
        IList<string> List(string typename, DateTimeOffset? since = null);
        IList<string> History(string typename, DateTimeOffset? since = null);
        IList<string> History(IKey key, DateTimeOffset? since = null);
        IList<string> History(DateTimeOffset? since = null);

        // BundleEntries
        bool Exists(IKey key);

        Task<Entry> GetAsync(IKey key, ClaimsPrincipal principal);
        Task<IList<Entry>> GetAsync(IEnumerable<string> identifiers, string sortby, ClaimsPrincipal principal);
        Task<IList<Entry>> GetCurrent(IEnumerable<string> identifiers, string sortby, ClaimsPrincipal principal);

        Task Add(Entry entry, ClaimsPrincipal principal);
        void Add(IEnumerable<Entry> entries, ClaimsPrincipal principal);

        void Replace(Entry entry);

        void Clean();
    }

    

}

