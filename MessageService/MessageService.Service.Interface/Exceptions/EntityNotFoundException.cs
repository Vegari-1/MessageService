
namespace MessageService.Service.Interface.Exceptions
{

    public class EntityAlreadyExistsException : BaseException
    {
        public EntityAlreadyExistsException(Type entityType) : base(
             String.Format("{0} already exists", entityType.Name))
        {
            StatusCode = 409;
        }
    }
}

