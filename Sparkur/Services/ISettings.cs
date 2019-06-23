using Spark.Engine;
using Spark.Mongo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sparkur.Services
{
    public interface ISettings
    {
        SparkSettings SparkSettings { get; set; }
        MongStoreSettings MongoStoreSettings { get; set; }
    }
}
