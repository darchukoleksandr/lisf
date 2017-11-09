using System;
using System.Collections.Generic;

namespace lisf.Models
{
    class ClaimReport
    {
        public DateTime GameDateTime { get; set; }
        public IEnumerable<Claim> Claims { get; set; }
    }
}
