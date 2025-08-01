using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using Re.Data;
using Re.Models;
using Re.Models.RealEstateDB;
namespace Re.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ServerMethodsController : Controller
    {
        RealEstateDBService service;

        public ServerMethodsController(RealEstateDBService service)
        {
            this.service = service;
        }

        public async Task<IActionResult> GetPropertiesExtended()
        {
            var result = await service.GetPropertiesExtended(GetExtendedQuery());

            return Ok(System.Text.Json.JsonSerializer.Serialize(result, 
                new System.Text.Json.JsonSerializerOptions { PropertyNamingPolicy = null, ReferenceHandler = ReferenceHandler.IgnoreCycles }));
        }

        public async Task<IActionResult> GetPropertyPicturesExtended()
        {
            var result = await service.GetPropertyPicturesExtended(GetExtendedQuery());

            return Ok(System.Text.Json.JsonSerializer.Serialize(result,
                new System.Text.Json.JsonSerializerOptions { PropertyNamingPolicy = null, ReferenceHandler = ReferenceHandler.IgnoreCycles }));
        }

        public async Task<IActionResult> GetAgentsExtended()
        {
            var result = await service.GetAgentsExtended(GetExtendedQuery());

            return Ok(System.Text.Json.JsonSerializer.Serialize(result,
                new System.Text.Json.JsonSerializerOptions { PropertyNamingPolicy = null, ReferenceHandler = ReferenceHandler.IgnoreCycles }));
        }

        ExtendedQuery GetExtendedQuery()
        {
            var skip = GetQuery("$skip") ?? null;
            var top = GetQuery("$top") ?? null;

            return new ExtendedQuery()
            {
                Expand = GetQuery("$expand") ?? "",
                Filter = GetQuery("$filter") ?? "",
                OrderBy = GetQuery("$orderBy") ?? "",
                Select = GetQuery("$select") ?? "",
                Calculate = GetQuery("$calculate") ?? "",
                Skip = skip != null ? int.Parse(skip) : null,
                Top = top != null ? int.Parse(top) : null,
            };
        }

        string GetQuery(string name)
        {
            if (Request.Query?.ContainsKey(name) == true)
            {
                return Request.Query[name].ToString();
            }

            return null;
        }
    }
}
