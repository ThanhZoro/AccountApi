using ApiAccountService.Extensions;
using ApiAccountService.Models;
using ApiAccountService.Repository;
using Contracts.Commands;
using Contracts.Models;
using MassTransit;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Nest;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Consumers
{
    /// <summary>
    /// 
    /// </summary>
    public class DeleteAccessRightConsumer : IConsumer<IDeleteAccessRight>
    {
        private IAccessRightRepository _accessRightRepository;
        private ElasticClient _esClient;
        private readonly IDistributedCache _distributedCache;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessRightRepository"></param>
        /// <param name="esSettings"></param>
        /// <param name="distributedCache"></param>
        public DeleteAccessRightConsumer(IAccessRightRepository accessRightRepository, IOptions<ElasticSearchSettings> esSettings, IDistributedCache distributedCache)
        {
            _accessRightRepository = accessRightRepository;
            var node = new Uri($"http://{esSettings.Value.Host}:{esSettings.Value.Port}");
            var connSettings = new ConnectionSettings(node);
            _esClient = new ElasticClient(connSettings);
            _distributedCache = distributedCache;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Consume(ConsumeContext<IDeleteAccessRight> context)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(theme: ConsoleTheme.None)
                .CreateLogger();
            var start = Stopwatch.GetTimestamp();
            Log.Information("Received command {CommandName}-{MessageId}: {@Messages}", GetType().Name, context.MessageId, context.Message);

            var model = context.Message.Cast<DeleteAccessRight>();
            var data = await _accessRightRepository.Delete(model);

            await context.RespondAsync(data);
            var response = await _esClient.IndexManyAsync(data.AccessRights, "access_rights");
            foreach (var item in model.UserList)
            {
                await _distributedCache.RemoveAsync($"access-right-{model.CompanyId}-{item}");
            }

            Log.Information("Completed command {CommandName}-{MessageId} {ExecuteTime}ms", GetType().Name, context.MessageId, (Stopwatch.GetTimestamp() - start) * 1000 / (double)Stopwatch.Frequency);
        }
    }
}
