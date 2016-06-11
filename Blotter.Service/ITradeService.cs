using Blotter.Model;
using Blotter.Service.Faults;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Blotter.Service
{
    public interface ITradeClientContract
    {
        [OperationContract(IsOneWay = true)]
        void TradeUpdated(Trade t);
    }

    [ServiceContract(SessionMode=SessionMode.Required, CallbackContract = typeof(ITradeClientContract))]
    public interface ITradeService
    {
        [OperationContract(IsOneWay = false)]
        [FaultContract(typeof(AttemptingToUpdateOldVersionOfTrade))]
        void AddUpdateTrade(Trade trade);

        [OperationContract(IsOneWay = false)]
        [FaultContract(typeof(TradeDoesNotExist))]
        List<Trade> GetHistoryForTrade(string tradeId);

        [OperationContract(IsOneWay = false)]
        List<Trade> GetTrades();

        [OperationContract(IsOneWay = false, IsInitiating = true)]
        void Subscribe();

        [OperationContract(IsOneWay = false, IsTerminating = true)]
        void Unsubscribe();

    }
}
