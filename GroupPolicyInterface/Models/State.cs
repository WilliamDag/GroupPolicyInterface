using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupPolicyInterface.Models
{
    class State
    {
        public State(string stateOption)
        {
            _stateOption = stateOption;
        }

        public string _stateOption { get; set; }

        public static List<State> GetAvailableStates()
        {
            var states = new List<State>();

            states.Add(new State("Enabled"));
            states.Add(new State("Disabled"));
            states.Add(new State("Not Configured"));

            return states;
        }
    }
}
