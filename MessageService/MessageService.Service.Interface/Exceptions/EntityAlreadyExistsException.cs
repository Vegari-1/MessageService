
namespace MessageService.Service.Interface.Exceptions
{

    public class EntityNotFoundException : BaseException
    {
        public EntityNotFoundException(Type entityType, string entityFields) : base(
             String.Format("{0} with given {1} does not exists", entityType.Name, entityFields))
        {
            StatusCode = 404;
        }
    }
}

