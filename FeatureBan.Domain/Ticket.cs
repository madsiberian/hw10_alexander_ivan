using System;

namespace FeatureBan.Domain
{
    public class Ticket
    {
        public string Name { get; set; }
        public Stage Stage { get; private set; }
    }
}
