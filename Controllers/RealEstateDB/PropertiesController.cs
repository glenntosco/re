using System;
using System.Net;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;

using Microsoft.EntityFrameworkCore;


using Re.Data;
using Re.Models.RealEstateDB;
namespace Re.Controllers.RealEstateDB
{
    [Route("odata/RealEstateDB/Properties")]
    public partial class PropertiesController : ODataController
    {
        private RealEstateDBContext context;

        public PropertiesController(RealEstateDBContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<Property> GetProperties()
        {
            var items = this.context.Properties.AsQueryable<Property>();
            this.OnPropertiesRead(ref items);

            return items;
        }

        partial void OnPropertiesRead(ref IQueryable<Property> items);

        partial void OnPropertyGet(ref SingleResult<Property> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/RealEstateDB/Properties(Id={Id})")]
        public SingleResult<Property> GetProperty(int key)
        {
            var items = this.context.Properties.Where(i => i.Id == key);
            var result = SingleResult.Create(items);

            OnPropertyGet(ref result);

            return result;
        }
        partial void OnPropertyDeleted(Property item);
        partial void OnAfterPropertyDeleted(Property item);

        [HttpDelete("/odata/RealEstateDB/Properties(Id={Id})")]
        public IActionResult DeleteProperty(int key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var item = this.context.Properties
                    .Where(i => i.Id == key)
                    .FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                this.OnPropertyDeleted(item);
                this.context.Properties.Remove(item);
                this.context.SaveChanges();
                this.OnAfterPropertyDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnPropertyUpdated(Property item);
        partial void OnAfterPropertyUpdated(Property item);

        [HttpPut("/odata/RealEstateDB/Properties(Id={Id})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutProperty(int key, [FromBody]Property item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (item == null || (item.Id != key))
                {
                    return BadRequest();
                }
                this.OnPropertyUpdated(item);
                this.context.Properties.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Properties.Where(i => i.Id == key);
                
                this.OnAfterPropertyUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/RealEstateDB/Properties(Id={Id})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchProperty(int key, [FromBody]Delta<Property> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var item = this.context.Properties.Where(i => i.Id == key).FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                patch.Patch(item);

                this.OnPropertyUpdated(item);
                this.context.Properties.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Properties.Where(i => i.Id == key);
                
                this.OnAfterPropertyUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnPropertyCreated(Property item);
        partial void OnAfterPropertyCreated(Property item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] Property item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (item == null)
                {
                    return BadRequest();
                }

                this.OnPropertyCreated(item);
                this.context.Properties.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Properties.Where(i => i.Id == item.Id);

                

                this.OnAfterPropertyCreated(item);

                return new ObjectResult(SingleResult.Create(itemToReturn))
                {
                    StatusCode = 201
                };
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }
    }
}
