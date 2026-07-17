namespace OmniCare.SharedKernel.Domain;

/// <summary>
/// Violation d'un invariant métier. Réservée aux cas imprévus par le flux nominal ;
/// les échecs métier attendus passent par Result&lt;T&gt; (railway-oriented).
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}
