using Contracts.Commands;
using Contracts.Models;
using MongoDB.Driver;
using System;

namespace ApiAccountService.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class ApplicationDbContext
    {
        private readonly IMongoDatabase _database = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="database"></param>
        public ApplicationDbContext(string connectionString, string database)
        {
            var client = new MongoClient(connectionString);
            if (client != null)
                _database = client.GetDatabase(database);
        }

        /// <summary>
        /// 
        /// </summary>
        public IMongoCollection<AccessRight> AccessRight
        {
            get
            {
                return _database.GetCollection<AccessRight>("access_rights");
            }
        }
    }
}
