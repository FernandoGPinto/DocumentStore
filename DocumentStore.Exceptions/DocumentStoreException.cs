namespace DocumentStore.Exceptions
{
    public class DocumentStoreException : Exception
    {
        public DocumentStoreException()
        {
        }

        public DocumentStoreException(string message)
            : base(message)
        {
        }

        public DocumentStoreException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}