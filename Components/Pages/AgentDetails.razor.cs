using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

using Re.Models;
using Re.Models.RealEstateDB;

namespace Re.Components.Pages
{
    public partial class AgentDetails
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected RealEstateDBService Service { get; set; }

        [Parameter]
        public string Id { get; set; }

        protected Agent agent;
        protected override async Task OnInitializedAsync()
        {
            var agentFilter = new List<CompositeFilterDescriptor>() { new CompositeFilterDescriptor()
            {
                FilterOperator = FilterOperator.Equals,
                Property = nameof(Agent.Id),
                FilterValue = Id,
            } };

            var agentsResult = await Service.GetAgentsExtended(new ExtendedQuery() { Filter = agentFilter.ToFilterString<Agent>() });
            agent = agentsResult.Value.FirstOrDefault();
        }

        protected IEnumerable<Property> agentProperties;

        int count;
        bool isLoading;
        async Task LoadData(LoadDataArgs args, CompositeFilterDescriptor filter = null)
        {
            isLoading = true;

            var propertyFilter = new List<CompositeFilterDescriptor>() { new CompositeFilterDescriptor()
            {
                FilterOperator = FilterOperator.Equals,
                Property = nameof(Property.UserId),
                FilterValue = Id,
            } };

            if (filter != null)
            {
                propertyFilter.Add(filter);
            }

            var result = await Service.GetPropertiesExtended(new ExtendedQuery() 
            { 
                Expand = nameof(Property.PropertyPictures), 
                Filter = propertyFilter.ToFilterString<Property>(),
                Skip = args.Skip,
                Top = args.Top,
                Calculate = "AgentDeals,DealTypes,DealsByMonth"
            });

            agentProperties = result.Value;
            count = result.Count;

            agentDeals = result.Calculations.AgentDeals;

            dealTypes = result.Calculations.DealTypes;

            dealsByMonth = result.Calculations.DealsByMonth;

            isLoading = false;
        }

        IEnumerable<AgentDeal> agentDeals;

        IEnumerable<DealsType> dealTypes;

        IEnumerable<DealByMonth> dealsByMonth;
    }
}