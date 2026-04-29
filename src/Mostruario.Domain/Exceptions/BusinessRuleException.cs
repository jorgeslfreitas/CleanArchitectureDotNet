namespace Mostruario.Domain.Exceptions;

public class BusinessRuleException(string message) : Exception(message)
{
}