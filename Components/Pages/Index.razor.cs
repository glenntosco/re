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
    public partial class Index
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
        protected SecurityService Security { get; set; }

        [Inject]
        protected RealEstateDBService Service { get; set; }

        protected IEnumerable<Property> properties;

        int count;
        bool isLoading;

        IEnumerable<string> titles;
        string keywords;
        string title;

        int? bedrooms;
        int maxBedrooms;

        int? bathrooms;
        int maxBathrooms;

        int? garages;
        int maxGarages;

        IEnumerable<int> size;

        IEnumerable<decimal> price;

        async Task LoadData(LoadDataArgs args)
        {
            isLoading = true;

            var filters = new List<CompositeFilterDescriptor>();
            
            if (!string.IsNullOrEmpty(keywords))
            {
                var keywordFilter = new CompositeFilterDescriptor() { LogicalFilterOperator = LogicalFilterOperator.Or };

                var keywordFilterDescriptors = new List<CompositeFilterDescriptor>();

                string[] properties =
                {
                    nameof(Property.Title),
                    nameof(Property.Description),
                    nameof(Property.Features),
                    nameof(Property.NearbyAmenities),
                    nameof(Property.Address)
                };

                foreach (var keyword in keywords.Split(' '))
                {
                    foreach (var p in properties)
                    {
                        keywordFilterDescriptors.Add(new CompositeFilterDescriptor()
                        {
                            FilterOperator = FilterOperator.Contains,
                            Property = p,
                            FilterValue = keyword
                        });
                    }
                }

                keywordFilter.Filters = keywordFilterDescriptors;

                filters.Add(keywordFilter);
            }

            if (!string.IsNullOrEmpty(title))
            {
                filters.Add(new CompositeFilterDescriptor()
                {
                    Property = nameof(Property.Title),
                    FilterOperator = FilterOperator.Contains,
                    FilterValue = title
                });
            }

            if (bedrooms != null)
            {
                filters.Add(new CompositeFilterDescriptor()
                {
                    Property = nameof(Property.Bedrooms),
                    FilterOperator = FilterOperator.Equals,
                    FilterValue = bedrooms
                });
            }

            if (bathrooms != null)
            {
                filters.Add(new CompositeFilterDescriptor()
                {
                    Property = nameof(Property.Bathrooms),
                    FilterOperator = FilterOperator.Equals,
                    FilterValue = bathrooms
                });
            }

            if (garages != null)
            {
                filters.Add(new CompositeFilterDescriptor()
                {
                    Property = nameof(Property.Garage),
                    FilterOperator = FilterOperator.Equals,
                    FilterValue = garages
                });
            }

            if (IsFilterVisible)
            {
                if (size?.Any() == true)
                {
                    filters.Add(new CompositeFilterDescriptor()
                    {
                        Filters = new List<CompositeFilterDescriptor>()
                        {
                            new CompositeFilterDescriptor()
                            {
                                Property = nameof(Property.SquareMeters),
                                FilterOperator = FilterOperator.GreaterThanOrEquals,
                                FilterValue = size.First()
                            },
                            new CompositeFilterDescriptor()
                            {
                                Property = nameof(Property.SquareMeters),
                                FilterOperator = FilterOperator.LessThanOrEquals,
                                FilterValue = size.Last()
                            }
                        }
                    });
                }

                if (price?.Any() == true)
                {
                    filters.Add(new CompositeFilterDescriptor()
                    {
                        Filters = new List<CompositeFilterDescriptor>()
                        {
                            new CompositeFilterDescriptor()
                            {
                                Property = nameof(Property.Price),
                                FilterOperator = FilterOperator.GreaterThanOrEquals,
                                FilterValue = price.First()
                            },
                            new CompositeFilterDescriptor()
                            {
                                Property = nameof(Property.Price),
                                FilterOperator = FilterOperator.LessThanOrEquals,
                                FilterValue = price.Last()
                            }
                        }
                    });
                }
            }

            var result = await Service.GetPropertiesExtended(new ExtendedQuery()
            {
                Expand = nameof(Property.PropertyPictures),
                Filter = filters.ToFilterString<Property>(filterCaseSensitivity: FilterCaseSensitivity.CaseInsensitive),
                Top = args.Top,
                Skip = args.Skip,
                Calculate = IsFilterVisible ? 
                    "Titles,MaxBedrooms,MaxBathrooms,MaxGarages,MinMaxSize,MinMaxPrice" : "Titles,MaxBedrooms,MaxBathrooms,MaxGarages",
            });

            properties = result.Value;
            count = result.Count;

            titles = result.Calculations.Titles;

            maxBedrooms =result.Calculations.MaxBedrooms;
            maxBathrooms = result.Calculations.MaxBathrooms;
            maxGarages = result.Calculations.MaxGarages;

            if (IsFilterVisible)
            {
                size = result.Calculations.MinMaxSize;
                price = result.Calculations.MinMaxPrice;
            }

            isLoading = false;
        }
    }
}