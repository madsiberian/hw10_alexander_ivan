using System.Collections.Generic;

namespace FeatureBan.Domain
{
    public class Ticket
    {
        public string Name { get; set; }
        public Stage Stage { get; set; }
        public bool IsAssigned { get; set; }
        public bool IsBlocked { get; set; }
        public string AssigneeName { get; set; }
    }
}
