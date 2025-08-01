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
    public partial class PropertyDetails
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

        [Parameter]
        public int Id { get; set; }

        [Inject]
        protected RealEstateDBService Service { get; set; }

        protected Property property;
        protected Agent agent;
        protected IEnumerable<Property> similarProperties;
        protected override async Task OnInitializedAsync()
        {
            var propertiesResult = await Service.GetPropertiesExtended(new ExtendedQuery() { 
                Expand = "PropertyPictures", 
                Filter = new List<CompositeFilterDescriptor>() { new CompositeFilterDescriptor()
                {
                    FilterOperator = FilterOperator.Equals,
                    Property = nameof(Property.Id),
                    FilterValue = Id,
                }}.ToFilterString<Property>() 
            });

            property = propertiesResult.Value.FirstOrDefault();

            var agentsResult = await Service.GetAgentsExtended(new ExtendedQuery()
            {
                Filter = new List<CompositeFilterDescriptor>() { new CompositeFilterDescriptor()
                {
                    FilterOperator = FilterOperator.Equals,
                    Property = nameof(Property.Id),
                    FilterValue = property.UserId,
                }}.ToFilterString<Agent>()
            });

            agent = agentsResult.Value.FirstOrDefault();

            var similarPropertiesResult = await Service.GetPropertiesExtended(new ExtendedQuery() 
            { 
                Expand = "PropertyPictures", 
                Filter = new List<CompositeFilterDescriptor>() { 
                    new CompositeFilterDescriptor()
                    {
                        FilterOperator = FilterOperator.NotEquals,
                        Property = nameof(Property.Id),
                        FilterValue = Id,
                    },
                    new CompositeFilterDescriptor()
                    {
                        FilterOperator = FilterOperator.Equals,
                        Property = nameof(Property.Type),
                        FilterValue = property.Type,
                    }
                }.ToFilterString<Property>(),
                Top = 2
            });

            similarProperties = similarPropertiesResult.Value;
        }

        string AmenityIcon(string amenity)
        {
            amenity = amenity.ToLowerInvariant();

            string icon = "check";

            if (amenity.Contains("school"))
            {
                icon = "school";
            }
            else if (amenity.Contains("hospital"))
            {
                icon = "local_hospital";
            }
            else if (amenity.Contains("shopping"))
            {
                icon = "local_mall";
            }
            else if (amenity.Contains("park"))
            {
                icon = "nature_people";
            }
            else if (amenity.Contains("transport"))
            {
                icon = "directions_subway";
            }

            return icon;
        }
    }
}