﻿namespace FeatureBan.Domain
{
    public class Ticket
    {
        public string Name { get; set; }
        public Stage Stage { get; private set; }
        public bool IsAssigned { get; set; }
        public bool IsBlocked { get; set; }
    }
}
