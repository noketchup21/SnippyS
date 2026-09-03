using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Application.Interfaces
{
    public record ClickEvent(Guid LinkId, DateTimeOffset ClickedAt, string? IpHash, string? Country, string? DeviceType, string? Browser, string? Referrer);

    public interface IClickEventPublisher
    {
        Task PublishAsync(ClickEvent clickEvent, CancellationToken ct = default);
    }
}
