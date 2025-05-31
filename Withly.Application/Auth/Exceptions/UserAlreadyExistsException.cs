namespace Withly.Application.Auth.Exceptions;

public class UserAlreadyExistsException(string message) : Exception(message);