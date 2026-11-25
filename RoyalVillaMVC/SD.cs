namespace RoyalVilla.Web
{
    public static class SD
    {
        public enum APIType
        {
            GET,
            POST,
            PUT,
            DELETE
        }

        public static string GetImageUrl(string? imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return "/images/placeholder-villa.jpg";
            }
            if (imageUrl.StartsWith("http"))
            {
                return imageUrl;
            }
            var url =$"/images/{imageUrl}";
            return url;
        }
    }
}
