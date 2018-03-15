using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupPolicyInterface.Models
{
    class GroupPolicy
    {
        public string _name { get; set; }
        public string _description { get; set; }
        public string _regPath { get; set; }
        public bool _isActive { get; set; }
    }
}
