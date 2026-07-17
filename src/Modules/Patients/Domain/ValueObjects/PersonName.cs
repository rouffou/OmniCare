using OmniCare.SharedKernel.Domain;

namespace OmniCare.Modules.Patients.Domain.ValueObjects;

public sealed record PersonName
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }

    private PersonName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public static PersonName Create(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Le nom et le prénom du patient sont obligatoires.");
        return new PersonName(firstName.Trim(), lastName.Trim());
    }

    public string FullName => $"{FirstName} {LastName}";

    public override string ToString() => FullName;
}
