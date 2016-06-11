using Blotter.Model;
using Blotter.ReferenceData;
using Blotter.TradeData;
using Caliburn.Micro;
using System.ServiceModel;
using System.Collections.ObjectModel;
using System;
using System.Linq;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows;
using Blotter.Services;

namespace Blotter.ViewModels
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ShellViewModel : PropertyChangedBase, IHaveDisplayName, ITradeServiceCallback
    {
        private TradeServiceClient tradeClient;
        private readonly string username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        private readonly IPermissionService permissions;

        private ObservableCollection<TradeViewModel> rawTrades;
        private ICollectionView trades;

        public ShellViewModel()
        {
            DisplayName = "Trade Blotter : " + username;
            BuyOrSellEnum = new ObservableCollection<BuySell>(Enum.GetValues(typeof(BuySell)).Cast<BuySell>());
            permissions = new PermissionService(username);

            //Long Initialisation
            Initialise();
        }

        public string DisplayName { get; set; }
        public ICollectionView Trades { get { return trades; } }
        public Collection<Security> Securities { get; set; }
        public Collection<Counterparty> Counterparties { get; set; }
        public Collection<BuySell> BuyOrSellEnum { get; set; }

        private bool showChildren;
        public bool ShowChildren
        {
            get { return showChildren; }
            set { showChildren = value; NotifyOfPropertyChange("ShowChildren"); }
        }

        private bool filterCancelled;
        public bool FilterCancelled
        {
            get { return filterCancelled; }
            set { filterCancelled = value; NotifyOfPropertyChange("FilterCancelled"); Trades.Refresh(); }
        }

        private void Initialise()
        {
            var referenceDataClient = new ReferenceDataServiceClient();
            Securities = new ObservableCollection<Security>(referenceDataClient.GetSecurities().ToList());
            Counterparties = new ObservableCollection<Counterparty>(referenceDataClient.GetCounterparties().ToList());

            tradeClient = new TradeServiceClient(new InstanceContext(null, this));
            tradeClient.Subscribe();

            rawTrades = new ObservableCollection<TradeViewModel>(tradeClient.GetTrades().Select(TransformTrade).ToList());
            trades = CollectionViewSource.GetDefaultView(rawTrades);

            trades.Filter = Filter;
            trades.SortDescriptions.Add(new SortDescription("Trade.Created", ListSortDirection.Descending));

            trades.Refresh();
        }

        public void AddTrade()
        {
            var t = new Trade();
            t.Id = Trade.GenerateTradeId();
            t.Created = DateTime.Now;
            t.CreatedBy = username;
            rawTrades.Add(TransformTrade(t));
        }

        public async void CommitRow(TradeViewModel row)
        {
            if (row.HasChanged())
            {
                row.Trade.LastUpdated = DateTime.Now;
                row.Trade.LastUpdatedBy = username;

                try
                {
                    await tradeClient.AddUpdateTradeAsync(row.Trade);
                }
                catch (FaultException<AttemptingToUpdateOldVersionOfTrade> e)
                {
                    //TODO: This is pretty basic. But its enough for now. You could build a whole comparison UI around it
                    MessageBox.Show("You attempted to update an old version of a Trade. Your Blotter must be out of Sync. Please restart it.");
                }
            }
        }

        public void TradeUpdated(Trade t)
        {
            var toUpdate = rawTrades.Where(x => x.Trade.Id == t.Id).FirstOrDefault();

            if (toUpdate != null)
            {
                toUpdate.Trade = t;
            }
            else
            {
                rawTrades.Add(TransformTrade(t));
            }
        }

        private TradeViewModel TransformTrade(Trade t)
        {
            return new TradeViewModel(t, tradeClient, permissions);
        }

        /// <summary>
        /// Extend the UI and this function to support more exotic filtering
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool Filter(object obj)
        {
            if (filterCancelled && obj is TradeViewModel)
            {
                return (obj as TradeViewModel).State != State.Cancelled;
            }
            return true;
        }
    }
}
