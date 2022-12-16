namespace NESCore.Mappers
{
    public class UnknownMapperException : Exception
    {
        public UnknownMapperException(int id) : base($"Unknown mapper: {id}")
        {
        }
    }
}
