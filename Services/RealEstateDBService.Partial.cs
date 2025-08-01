using Radzen;
using System.Data;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;

using Re.Data;
using Re.Models;
using Re.Models.RealEstateDB;
namespace Re
{
    public partial class RealEstateDBService
    {
        ApplicationUser user;

        public RealEstateDBService(RealEstateDBContext context,
            NavigationManager navigationManager,
            IHttpContextAccessor httpContextAccessor,
            ApplicationIdentityDbContext identityDbContext)
        {
            this.context = context;
            this.navigationManager = navigationManager;

            var userId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            user = identityDbContext.Users.FirstOrDefault(u => u.Id == userId);
        }

        partial void OnPropertyCreated(Property item)
        {
            if (user != null && user.Name != "Admin")
            {
                item.UserId = user.Id;
            }
        }

        partial void OnPropertiesRead(ref IQueryable<Property> items)
        {
            if (user != null && user.Name != "Admin")
            {
                items = items.Where(item => item.UserId == user.Id);
            }
        }

        public async Task<RealEstateDBServiceResult<Property>> GetPropertiesExtended(ExtendedQuery query = null)
        {
            var items = Context.Properties.AsQueryable();

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                items = FilterAndSort(items, query);
            }

            OnPropertiesRead(ref items);

            return await Task.FromResult(new RealEstateDBServiceResult<Property> 
            { 
                Value = Page(items, query).ToList(), 
                Count = items.Count(),
                Calculations = GetCalculations(query, items)
            });
        }

        public async Task<RealEstateDBServiceResult<Agent>> GetAgentsExtended(ExtendedQuery query = null)
        {
            var items = Context.Agents.AsQueryable();

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                items = FilterAndSort(items, query);
            }
            OnAgentsRead(ref items);

            return await Task.FromResult(new RealEstateDBServiceResult<Agent>
            {
                Value = Page(items, query).ToList(),
                Count = items.Count()
            });
        }

        public async Task<RealEstateDBServiceResult<PropertyPicture>> GetPropertyPicturesExtended(ExtendedQuery query = null)
        {
            var items = Context.PropertyPictures.AsQueryable();

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                items = FilterAndSort(items, query);
            }

            OnPropertyPicturesRead(ref items);

            return await Task.FromResult(new RealEstateDBServiceResult<PropertyPicture>
            {
                Value = Page(items, query).ToList(),
                Count = items.Count(),
            });
        }

        Calculations GetCalculations(ExtendedQuery query, IQueryable<Property> items)
        {
            var calculations = new Calculations();
            if (query.Calculate != null && items.Any())
            {
                if (query.Calculate.Contains("Titles"))
                {
                    calculations.Titles = items.Select(p => p.Title).Distinct();
                }

                if (query.Calculate.Contains("MaxBedrooms"))
                {
                    calculations.MaxBedrooms = items.Max(p => p.Bedrooms);
                }

                if (query.Calculate.Contains("MaxBathrooms"))
                {
                    calculations.MaxBathrooms = items.Max(p => p.Bathrooms);
                }

                if (query.Calculate.Contains("MaxGarages"))
                {
                    calculations.MaxGarages = items.Max(p => p.Garage);
                }

                if (query.Calculate.Contains("MinMaxSize"))
                {
                    calculations.MinMaxSize = new int[] { items.Min(p => p.SquareMeters), items.Max(p => p.SquareMeters) };
                }

                if (query.Calculate.Contains("MinMaxPrice"))
                {
                    calculations.MinMaxPrice = new decimal[] { items.Min(p => p.Price), items.Max(p => p.Price) };
                }

                if (query.Calculate.Contains("AgentDeals"))
                {
                    calculations.AgentDeals = items.GroupBy(p => p.Title).Select(g => new AgentDeal() { PropertyType = g.Key, Deals = g.Count() });
                }

                if (query.Calculate.Contains("DealTypes"))
                {
                    calculations.DealTypes = items.GroupBy(p => p.Type).Select(g => new DealsType() { DealType = g.Key == 1 ? "Sale" : "Rent", Deals = g.Count() });
                }

                if (query.Calculate.Contains("DealsByMonth"))
                {
                    calculations.DealsByMonth = items.Where(p => p.ClosedDate != null).GroupBy(d => d.ClosedDate.Value.Month).Select(g => new DealByMonth() { Month = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(g.Key), Deals = g.Count() });
                }
            }
            return calculations;
        }

        public IQueryable<T> FilterAndSort<T>(IQueryable<T> items, Query query = null)
        {
            var result = items;

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        result = result.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        result = result.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    result = result.OrderBy(query.OrderBy);
                }
            }

            return result;
        }

        public IQueryable<T> Page<T>(IQueryable<T> items, Query query = null)
        {
            var result = items;

            if (query != null)
            {
                if (query.Skip.HasValue)
                {
                    result = result.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    result = result.Take(query.Top.Value);
                }
            }

            return result;
        }
    }
}