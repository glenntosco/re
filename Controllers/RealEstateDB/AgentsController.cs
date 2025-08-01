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
    [Route("odata/RealEstateDB/Agents")]
    public partial class AgentsController : ODataController
    {
        private RealEstateDBContext context;

        public AgentsController(RealEstateDBContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<Agent> GetAgents()
        {
            var items = this.context.Agents.AsQueryable<Agent>();
            this.OnAgentsRead(ref items);

            return items;
        }

        partial void OnAgentsRead(ref IQueryable<Agent> items);
    }
}
