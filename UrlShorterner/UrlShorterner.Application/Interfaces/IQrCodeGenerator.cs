using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Application.Interfaces
{
    public interface IQrCodeGenerator
    {
        byte[] GeneratePng(string content);
    }
}
