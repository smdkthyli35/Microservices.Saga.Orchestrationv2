﻿using MassTransit;
using MongoDB.Driver;
using Shared.Messages;
using Stock.API.Services;

namespace Stock.API.Consumers
{
    public class StockRollBackMessageConsumer(MongoDbService mongoDbService) : IConsumer<StockRollbackMessage>
    {
        public async Task Consume(ConsumeContext<StockRollbackMessage> context)
        {
            var stockCollection = mongoDbService.GetCollection<Models.Stock>();
            foreach (var orderItem in context.Message.OrderItems)
            {
                var stock = await (await stockCollection.FindAsync(s => s.ProductId == orderItem.ProductId)).FirstOrDefaultAsync();
                stock.Count += orderItem.Count;
                await stockCollection.FindOneAndReplaceAsync(s => s.ProductId == orderItem.ProductId, stock);
            }
        }
    }
}