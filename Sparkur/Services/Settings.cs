using Spark.Engine;
using Spark.Mongo;
using Sparkur.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sparkur.Services
{
    public class Settings : ISettings
    {
        public SparkSettings SparkSettings { get; set; }
        public MongStoreSettings MongoStoreSettings { get; set; }
    }
}
