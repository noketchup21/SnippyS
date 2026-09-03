using System;
using System.Collections.Generic;
using System.Text;
using QRCoder;
using UrlShortener.Application.Interfaces;

namespace UrlShortener.Infrastructure.Services
{
    public class QrCodeGenerator : IQrCodeGenerator
    {
        public byte[] GeneratePng(string content)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            var pngQrCode = new PngByteQRCode(qrCodeData);
            return pngQrCode.GetGraphic(20);
        }
    }
}
