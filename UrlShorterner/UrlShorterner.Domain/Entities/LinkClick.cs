using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Domain.Entities
{
    public class LinkClick
    {
        public long Id { get; set; }
        public Guid LinkId { get; set; }
        public DateTimeOffset ClickedAt { get; set; }
        public string? IpHash { get; set; }
        public string? Country { get; set; }
        public string? DeviceType { get; set; }
        public string? Browser { get; set; }
        public string? Referrer { get; set; }
    }
}
