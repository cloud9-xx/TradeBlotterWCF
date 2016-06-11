using System;
using System.Collections.Generic;
using System.Linq;
using Blotter.Model;
using System.ServiceModel;

namespace Blotter.Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class TradeService : ITradeService, IDisposable
    {
        private ITradeClientContract callback = null;
        private BlotterContext.TradeUpdateHandler tradeChangeHandler;

        private BlotterContext context = BlotterContext.Instance;


        public void AddUpdateTrade(Trade trade)
        {
            context.AddUpdateTrade(trade);
        }

        public List<Trade> GetTrades()
        {
            return context.Trades.ToList();
        }

        public List<Trade> GetHistoryForTrade(string tradeId)
        {
            return context.GetHistoryForTrade(tradeId).ToList();
        }

        public void Subscribe()
        {
            //Ensure we do not double subscribe
            if (tradeChangeHandler == null)
            {
                callback = OperationContext.Current.GetCallbackChannel<ITradeClientContract>();

                tradeChangeHandler = new BlotterContext.TradeUpdateHandler(TradeService_TradeChangeEvent);

                context.Subscribe(tradeChangeHandler);
            }
        }

        private void TradeService_TradeChangeEvent(Trade trade)
        {
            if (callback != null)
            {
                try
                {
                    callback.TradeUpdated(trade);
                }
                catch
                {
                    Unsubscribe();
                    //TODO: Do something with this exception. But for now swallow it, to ensure we dont break everything just because we have a broken callback
                    //TODO: Add Logging
                }
            }
        }

        public void Unsubscribe()
        {
            if (tradeChangeHandler != null)
            {
                context.Unsubscribe(tradeChangeHandler);
                tradeChangeHandler = null;
                callback = null;
            }
        }

        public void Dispose()
        {
            //Avoid memory leaks by ensuring we unsubscribe from the events
            Unsubscribe();
        }
    }
}
