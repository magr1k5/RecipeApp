using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeApp.Classes
{
    public  class UserEvent
    {
        public int EventId { get; set; }
        public string EventName { get; set; }
        public int UserId { get; set; }
        public int[] RecipeIds { get; set; }
    }
}
