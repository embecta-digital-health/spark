using System.Collections.Generic;
using Spark.Engine.Core;

namespace Spark.Engine.Service
{
    public interface ITransfer
    {
        void Externalize(IEnumerable<Entry> interactions);
        void Externalize(Entry interaction);
        void Internalize(IEnumerable<Entry> interactions, Mapper<string, IKey> mapper);
        void Internalize(Entry entry);
    }
}