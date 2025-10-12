namespace Withly.Domain.Exceptions;

public class EntityNotFoundException(string message) : Exception(message)
{
    public static EntityNotFoundException For<T>(string id) =>
        new($"Entity of type {typeof(T).Name} with ID {id} was not found.");
}