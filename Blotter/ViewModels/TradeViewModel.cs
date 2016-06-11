using Blotter.Model;
using Blotter.Services;
using Blotter.TradeData;
using Caliburn.Micro;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Blotter.ViewModels
{
    public class TradeViewModel : PropertyChangedBase
    {
        private TradeServiceClient tradeClient;

        public TradeViewModel(Trade trade, TradeServiceClient client, IPermissionService permissions)
        {
            History = new ObservableCollection<Trade>();
            tradeClient = client;
            Trade = trade;
            IsEditable = permissions.IsEditable(trade);
        }

        private Trade original;
        private Trade trade;
        public Trade Trade
        {
            get { return trade; }
            set { trade = value; original = trade.Copy(); Refresh(); UpdateHistory(); }
        }

        public State State
        {
            get { return trade.State; }
        }

        public Counterparty Counterparty
        {
            get { return trade.Counterparty; }
            set
            {
                if (value == null) throw new ArgumentNullException("Counterparty", "Counterparty must be set");
                trade.Counterparty = value;
                NotifyOfPropertyChange("Counterparty");
            }
        }

        public Security Security
        {
            get { return trade.Security; }
            set
            {
                if (value == null) throw new ArgumentNullException("Security", "Security must be set");
                trade.Security = value;
                NotifyOfPropertyChange("Security");
            }
        }

        public decimal Price
        {
            get { return trade.Price; }
            set
            {
                if (value < 1) throw new ArgumentException("Price", "Price must be greater than 1");
                if (Security != null && Security.Currency == Currency.JPY && value < 200) throw new ArgumentException("Price", "Price must be 200 or higher for JPY");
                if (Security != null && Security.Currency == Currency.USD && value > 1000) throw new ArgumentException("Price", "Price must be 1000 or lower for USD");
                trade.Price = value;
                NotifyOfPropertyChange("Price");
            }
        }

        public int Units
        {
            get { return trade.Units; }
            set
            {
                if (value < 1) throw new ArgumentException("Units", "Units must be greater than 1");
                trade.Units = value;
                NotifyOfPropertyChange("Units");
            }
        }

        public BuySell BuySell
        {
            get { return trade.BuySell; }
            set
            {
                trade.BuySell = value;
                NotifyOfPropertyChange("BuySell");
            }
        }

        public bool IsEditable { get; private set; }

        public bool HasChanged()
        {
            return original.Counterparty != trade.Counterparty
                || original.Security != trade.Security
                || original.BuySell != trade.BuySell
                || original.Price != trade.Price
                || original.Units != trade.Units;
        }

        public ObservableCollection<Trade> History { get; private set; }

        private void UpdateHistory()
        {
            if(Trade.Version == 0)
            {
                return; 
            }

            var history = tradeClient.GetHistoryForTrade(trade.Id).ToList();
            History.Clear();
            history.ForEach(History.Add);
        }
    }
}
