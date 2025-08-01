using System;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Radzen;

using Re.Data;
using Re.Models.RealEstateDB;
namespace Re
{
    public partial class RealEstateDBService
    {
        RealEstateDBContext Context
        {
           get
           {
             return this.context;
           }
        }

        private readonly RealEstateDBContext context;
        private readonly NavigationManager navigationManager;

        public RealEstateDBService(RealEstateDBContext context, NavigationManager navigationManager)
        {
            this.context = context;
            this.navigationManager = navigationManager;
        }

        public void Reset() => Context.ChangeTracker.Entries().Where(e => e.Entity != null).ToList().ForEach(e => e.State = EntityState.Detached);

        public void ApplyQuery<T>(ref IQueryable<T> items, Query query = null)
        {
            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }
        }


        public async Task ExportAgentsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/realestatedb/agents/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/realestatedb/agents/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAgentsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/realestatedb/agents/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/realestatedb/agents/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAgentsRead(ref IQueryable<Agent> items);

        public async Task<IQueryable<Agent>> GetAgents(Query query = null)
        {
            var items = Context.Agents.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnAgentsRead(ref items);

            return await Task.FromResult(items);
        }

        public async Task ExportPropertiesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/realestatedb/properties/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/realestatedb/properties/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportPropertiesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/realestatedb/properties/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/realestatedb/properties/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnPropertiesRead(ref IQueryable<Property> items);

        public async Task<IQueryable<Property>> GetProperties(Query query = null)
        {
            var items = Context.Properties.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnPropertiesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnPropertyGet(Property item);
        partial void OnGetPropertyById(ref IQueryable<Property> items);


        public async Task<Property> GetPropertyById(int id)
        {
            var items = Context.Properties
                              .AsNoTracking()
                              .Where(i => i.Id == id);

 
            OnGetPropertyById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnPropertyGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnPropertyCreated(Property item);
        partial void OnAfterPropertyCreated(Property item);

        public async Task<Property> CreateProperty(Property property)
        {
            OnPropertyCreated(property);

            var existingItem = Context.Properties
                              .Where(i => i.Id == property.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Properties.Add(property);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(property).State = EntityState.Detached;
                throw;
            }

            OnAfterPropertyCreated(property);

            return property;
        }

        public async Task<Property> CancelPropertyChanges(Property item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnPropertyUpdated(Property item);
        partial void OnAfterPropertyUpdated(Property item);

        public async Task<Property> UpdateProperty(int id, Property property)
        {
            OnPropertyUpdated(property);

            var itemToUpdate = Context.Properties
                              .Where(i => i.Id == property.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(property);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterPropertyUpdated(property);

            return property;
        }

        partial void OnPropertyDeleted(Property item);
        partial void OnAfterPropertyDeleted(Property item);

        public async Task<Property> DeleteProperty(int id)
        {
            var itemToDelete = Context.Properties
                              .Where(i => i.Id == id)
                              .Include(i => i.PropertyPictures)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnPropertyDeleted(itemToDelete);


            Context.Properties.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterPropertyDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportPropertyPicturesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/realestatedb/propertypictures/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/realestatedb/propertypictures/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportPropertyPicturesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/realestatedb/propertypictures/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/realestatedb/propertypictures/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnPropertyPicturesRead(ref IQueryable<PropertyPicture> items);

        public async Task<IQueryable<PropertyPicture>> GetPropertyPictures(Query query = null)
        {
            var items = Context.PropertyPictures.AsQueryable();

            items = items.Include(i => i.Property);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnPropertyPicturesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnPropertyPictureGet(PropertyPicture item);
        partial void OnGetPropertyPictureById(ref IQueryable<PropertyPicture> items);


        public async Task<PropertyPicture> GetPropertyPictureById(int id)
        {
            var items = Context.PropertyPictures
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.Property);
 
            OnGetPropertyPictureById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnPropertyPictureGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnPropertyPictureCreated(PropertyPicture item);
        partial void OnAfterPropertyPictureCreated(PropertyPicture item);

        public async Task<PropertyPicture> CreatePropertyPicture(PropertyPicture propertypicture)
        {
            OnPropertyPictureCreated(propertypicture);

            var existingItem = Context.PropertyPictures
                              .Where(i => i.Id == propertypicture.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.PropertyPictures.Add(propertypicture);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(propertypicture).State = EntityState.Detached;
                throw;
            }

            OnAfterPropertyPictureCreated(propertypicture);

            return propertypicture;
        }

        public async Task<PropertyPicture> CancelPropertyPictureChanges(PropertyPicture item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnPropertyPictureUpdated(PropertyPicture item);
        partial void OnAfterPropertyPictureUpdated(PropertyPicture item);

        public async Task<PropertyPicture> UpdatePropertyPicture(int id, PropertyPicture propertypicture)
        {
            OnPropertyPictureUpdated(propertypicture);

            var itemToUpdate = Context.PropertyPictures
                              .Where(i => i.Id == propertypicture.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(propertypicture);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterPropertyPictureUpdated(propertypicture);

            return propertypicture;
        }

        partial void OnPropertyPictureDeleted(PropertyPicture item);
        partial void OnAfterPropertyPictureDeleted(PropertyPicture item);

        public async Task<PropertyPicture> DeletePropertyPicture(int id)
        {
            var itemToDelete = Context.PropertyPictures
                              .Where(i => i.Id == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnPropertyPictureDeleted(itemToDelete);


            Context.PropertyPictures.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterPropertyPictureDeleted(itemToDelete);

            return itemToDelete;
        }
        }
}