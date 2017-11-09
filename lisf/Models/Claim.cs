using System;
using System.Collections.Generic;

namespace lisf.Models
{
    class Claim
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public int Diameter { get; set; }
        public int Structures { get; set; }

        public IEnumerable<Member> Members { get; set; }
    }
}
