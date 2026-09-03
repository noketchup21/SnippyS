using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Domain.Entities
{
    public class LinkClickDailyStat
    {
        public Guid LinkId { get; set; }
        public DateOnly StatDate { get; set; }
        public int ClickCount { get; set; }
        public string? TopCountry { get; set; }
        public string? TopReferrer { get; set; }
        public string? DeviceBreakdownJson { get; set; }

        public Link Link { get; set; } = default!;
    }
}
