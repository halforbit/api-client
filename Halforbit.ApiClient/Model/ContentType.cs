
namespace Halforbit.ApiClient
{
    public class ContentType
    {
        public ContentType(
            string boundary,
            string charSet,
            string mediaType,
            string name)
        {
            Boundary = boundary;

            CharSet = charSet;

            MediaType = mediaType;

            Name = name;
        }

        public string Boundary { get; }

        public string CharSet { get; }

        public string MediaType { get; }

        public string Name { get; }
    }
}
