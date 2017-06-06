using System;
using Spark.Engine.Core;

namespace Spark.Engine.Service
{
    public interface IServiceListener
    {
        void Inform(Uri location, Entry interaction);
    }

}
