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
    public partial class ManageProperties
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
        public RealEstateDBService Service { get; set; }

        protected IEnumerable<Property> properties;

        protected RadzenDataGrid<Property> grid0;
        int count;
        bool isLoading;

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            await LoadData(new LoadDataArgs() { Skip = 0, Top = 10 });
        }

        async Task LoadData(LoadDataArgs args)
        {
            isLoading = true;

            var result = await Service.GetPropertiesExtended(new ExtendedQuery()
            {
                Expand = nameof(Property.PropertyPictures),
                Filter = args.Filter,
                Top = args.Top,
                Skip = args.Skip
            });

            properties = result.Value;
            count = result.Count;

            isLoading = false;
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddProperties>("Add Property", null);
            await LoadData(new LoadDataArgs() { Skip = 0, Top = 10 });
            await grid0.Reload();
        }

        protected async Task EditRow(Property args)
        {
            await DialogService.OpenAsync<EditProperties>("Edit Property", new Dictionary<string, object> { {"Id", args.Id} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, Property property)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await Service.DeleteProperty(property.Id);

                    if (deleteResult != null)
                    {
                        await grid0.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"Unable to delete Property"
                });
            }
        }

        protected Property propertyChild;
        protected async Task GetChildData(Property args)
        {
            propertyChild = args;

            var filters = new List<CompositeFilterDescriptor>();

            filters.Add(new CompositeFilterDescriptor()
            {
                FilterOperator = FilterOperator.Equals,
                Property = nameof(PropertyPicture.PropertyId),
                FilterValue = args.Id,
            });

            var PropertyPicturesResult = await Service.GetPropertyPicturesExtended(
                new ExtendedQuery() { 
                    Filter = filters.ToFilterString<PropertyPicture>() 
                });

            if (PropertyPicturesResult != null)
            {
                args.PropertyPictures = PropertyPicturesResult.Value.ToList();
            }
        }
        protected PropertyPicture propertyPicturePropertyPictures;

        protected RadzenDataList<PropertyPicture> PropertyPicturesDataGrid;

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task PropertyPicturesAddButtonClick(MouseEventArgs args, Property data)
        {

            var dialogResult = await DialogService.OpenAsync<AddPropertyPicture>("Add PropertyPictures", new Dictionary<string, object> { { "PropertyId", data.Id } });
            await GetChildData(data);
            await PropertyPicturesDataGrid.Reload();

        }

        protected async Task PropertyPicturesRowSelect(PropertyPicture args, Property data)
        {
            var dialogResult = await DialogService.OpenAsync<EditPropertyPicture>("Edit PropertyPictures", new Dictionary<string, object> { {"Id", args.Id} });
            await GetChildData(data);
            await PropertyPicturesDataGrid.Reload();
        }

        protected async Task PropertyPicturesDeleteButtonClick(MouseEventArgs args, PropertyPicture propertyPicture)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await Service.DeletePropertyPicture(propertyPicture.Id);

                    await GetChildData(propertyChild);

                    if (deleteResult != null)
                    {
                        await PropertyPicturesDataGrid.Reload();
                    }
                }
            }
            catch (System.Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"Unable to delete PropertyPicture"
                });
            }
        }
    }
}