using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

using Re.Models;
using Re.Models.RealEstateDB;

namespace Re.Components.Pages
{
    public partial class EditProperties
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
        public RealEstateDBService RealEstateDBService { get; set; }

        [Parameter]
        public int Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            property = await RealEstateDBService.GetPropertyById(id:Id);
        }
        protected bool errorVisible;
        protected Property property;

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task FormSubmit()
        {
            try
            {
                await RealEstateDBService.UpdateProperty(Id, property);
                DialogService.Close(property);
            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}