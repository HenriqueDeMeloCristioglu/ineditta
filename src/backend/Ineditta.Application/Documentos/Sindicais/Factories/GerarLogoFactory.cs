namespace Ineditta.Application.Documentos.Sindicais.Factories
{
    public static class GerarLogoFactory
    {
        public static byte[]? FromBase64ToBytes(string logo)
        {
            if (logo.IndexOf("base64,", StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                return Convert.FromBase64String(logo[(logo.IndexOf("base64,", StringComparison.InvariantCultureIgnoreCase) + 7)..]);
            }

            return null;
        }

        public static MemoryStream? ToMemoryUrl(string logo)
        {
            var bytes = FromBase64ToBytes(logo);

            if (bytes == null)
            {
                return null;
            }

            return new MemoryStream(bytes);
        }

    }
}
