using QRCoder;

namespace buzzaraApi.Services
{
    public class QRCodeService
    {
        public string GerarQRCodeSvg(string texto)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(texto, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new SvgQRCode(qrCodeData);
            var svgImage = qrCode.GetGraphic(5); // Tamanho 5

            return svgImage;
        }
    }
}
