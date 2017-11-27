using Spark.Core;
using System.Collections.Generic;
using Spark.Engine.Core;

namespace Spark.Service
{

    /// <summary>
    /// Transfer maps between local id's and references and absolute id's and references upon incoming or outgoing Interactions.
    /// It uses an Import or Export to do de actual work for incoming or outgoing Interactions respectively.
    /// </summary>
    public class Transfer
    {
        
        IGenerator generator;

        public Transfer(IGenerator generator)
        {
            this.generator = generator;
            
        }

        public void Internalize(Entry entry, ILocalhost localhost)
        {
            var import = new Import(localhost, this.generator);
            import.Add(entry);
            import.Internalize();
        }

        public void Internalize(IEnumerable<Entry> interactions, ILocalhost localhost)
        {
            var import = new Import(localhost, this.generator);
            import.Add(interactions);
            import.Internalize();
        }

        public void Externalize(Entry interaction, ILocalhost localhost)
        {
            Export export = new Export(localhost);
            export.Add(interaction);
            export.Externalize();
        }

        public void Externalize(IEnumerable<Entry> interactions, ILocalhost localhost)
        {
            Export export = new Export(localhost);
            export.Add(interactions);
            export.Externalize();
        }
    }
}
