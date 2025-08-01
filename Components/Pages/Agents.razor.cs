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
    public partial class Agents
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

        private bool IsFilterVisible = false;

        [Inject]
        protected RealEstateDBService Service { get; set; }

        protected IEnumerable<Agent> agents;

        string name, specialties, notes;
        bool isLoading;
        int count;

        async Task LoadData(LoadDataArgs args)
        {
            isLoading = true;

            var filters = IsFilterVisible ? new List<CompositeFilterDescriptor>() { new CompositeFilterDescriptor()
            {
                LogicalFilterOperator = LogicalFilterOperator.Or,
                Filters = new List<CompositeFilterDescriptor>()
                {
                    new CompositeFilterDescriptor()
                    {
                        FilterOperator = FilterOperator.Contains,
                        Property = nameof(Agent.FirstName),
                        FilterValue = name,
                    },
                    new CompositeFilterDescriptor()
                    {
                        FilterOperator = FilterOperator.Contains,
                        Property = nameof(Agent.FirstName),
                        FilterValue = name,
                    },
                    new CompositeFilterDescriptor()
                    {
                        FilterOperator = FilterOperator.Contains,
                        Property = nameof(Agent.Specialties),
                        FilterValue = specialties,
                    },
                    new CompositeFilterDescriptor()
                    {
                        FilterOperator = FilterOperator.Contains,
                        Property = nameof(Agent.Notes),
                        FilterValue = notes,
                    }
                }
            }} : Enumerable.Empty<CompositeFilterDescriptor>();

            var agentsResult = await Service.GetAgentsExtended(new ExtendedQuery() 
            { 
                Filter = filters.ToFilterString<Agent>(filterCaseSensitivity: FilterCaseSensitivity.CaseInsensitive),
                Skip = args.Skip,
                Top = args.Top,
            });

            agents = agentsResult.Value;
            count = agentsResult.Count;

            isLoading = false;
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            await LoadData(new LoadDataArgs() { Skip = 0, Top = 2 });
        }
    }
}