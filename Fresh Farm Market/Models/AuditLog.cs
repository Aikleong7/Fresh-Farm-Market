﻿namespace Fresh_Farm_Market.Models
{
    public class AuditLog
    {
        public string userId { get; set; }
        public string Activity{ get; set; }
        public DateTime DateTime { get; set; }
    }
}
