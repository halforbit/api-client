
namespace Halforbit.ApiClient
{
    public class ContentType
    {
        public ContentType(
            string value,
            string boundary,
            string charSet,
            string mediaType,
            string name)
        {
            Value = value;

            Boundary = boundary;

            CharSet = charSet;

            MediaType = mediaType;

            Name = name;
        }

        public string Value { get; }

        public string Boundary { get; }

        public string CharSet { get; }

        public string MediaType { get; }

        public string Name { get; }

        public static implicit operator ContentType(string str)
        {
            if (str == null)
            {
                return null;
            }
            
            var contentType = !string.IsNullOrWhiteSpace(str) ?
                new System.Net.Mime.ContentType(str) :
                null;

            return new ContentType(
                value: str,
                boundary: contentType?.Boundary ?? null,
                charSet: contentType?.CharSet ?? null,
                mediaType: contentType?.MediaType ?? null,
                name: contentType?.Name ?? null);
        }
    }
}
