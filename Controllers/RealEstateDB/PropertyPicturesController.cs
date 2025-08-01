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
    [Route("odata/RealEstateDB/PropertyPictures")]
    public partial class PropertyPicturesController : ODataController
    {
        private RealEstateDBContext context;

        public PropertyPicturesController(RealEstateDBContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<PropertyPicture> GetPropertyPictures()
        {
            var items = this.context.PropertyPictures.AsQueryable<PropertyPicture>();
            this.OnPropertyPicturesRead(ref items);

            return items;
        }

        partial void OnPropertyPicturesRead(ref IQueryable<PropertyPicture> items);

        partial void OnPropertyPictureGet(ref SingleResult<PropertyPicture> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/RealEstateDB/PropertyPictures(Id={Id})")]
        public SingleResult<PropertyPicture> GetPropertyPicture(int key)
        {
            var items = this.context.PropertyPictures.Where(i => i.Id == key);
            var result = SingleResult.Create(items);

            OnPropertyPictureGet(ref result);

            return result;
        }
        partial void OnPropertyPictureDeleted(PropertyPicture item);
        partial void OnAfterPropertyPictureDeleted(PropertyPicture item);

        [HttpDelete("/odata/RealEstateDB/PropertyPictures(Id={Id})")]
        public IActionResult DeletePropertyPicture(int key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var item = this.context.PropertyPictures
                    .Where(i => i.Id == key)
                    .FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                this.OnPropertyPictureDeleted(item);
                this.context.PropertyPictures.Remove(item);
                this.context.SaveChanges();
                this.OnAfterPropertyPictureDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnPropertyPictureUpdated(PropertyPicture item);
        partial void OnAfterPropertyPictureUpdated(PropertyPicture item);

        [HttpPut("/odata/RealEstateDB/PropertyPictures(Id={Id})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutPropertyPicture(int key, [FromBody]PropertyPicture item)
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
                this.OnPropertyPictureUpdated(item);
                this.context.PropertyPictures.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.PropertyPictures.Where(i => i.Id == key);
                Request.QueryString = Request.QueryString.Add("$expand", "Property");
                this.OnAfterPropertyPictureUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/RealEstateDB/PropertyPictures(Id={Id})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchPropertyPicture(int key, [FromBody]Delta<PropertyPicture> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var item = this.context.PropertyPictures.Where(i => i.Id == key).FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                patch.Patch(item);

                this.OnPropertyPictureUpdated(item);
                this.context.PropertyPictures.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.PropertyPictures.Where(i => i.Id == key);
                Request.QueryString = Request.QueryString.Add("$expand", "Property");
                this.OnAfterPropertyPictureUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnPropertyPictureCreated(PropertyPicture item);
        partial void OnAfterPropertyPictureCreated(PropertyPicture item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] PropertyPicture item)
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

                this.OnPropertyPictureCreated(item);
                this.context.PropertyPictures.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.PropertyPictures.Where(i => i.Id == item.Id);

                Request.QueryString = Request.QueryString.Add("$expand", "Property");

                this.OnAfterPropertyPictureCreated(item);

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
