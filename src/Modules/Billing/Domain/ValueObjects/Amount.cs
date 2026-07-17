using OmniCare.SharedKernel.Domain;

namespace OmniCare.Modules.Billing.Domain.ValueObjects;

/// <summary>
/// Montant monétaire en euros, non négatif, arrondi au centime
/// (arrondi bancaire — MidpointRounding.ToEven).
/// </summary>
public sealed record Amount
{
    public decimal Value { get; private set; }

    private Amount(decimal value) => Value = value;

    public static Amount Create(decimal value)
    {
        if (value < 0)
            throw new DomainException($"Un montant ne peut pas être négatif : {value}.");
        return new Amount(Math.Round(value, 2, MidpointRounding.ToEven));
    }

    public static readonly Amount Zero = new(0m);

    public static Amount operator +(Amount left, Amount right) => Create(left.Value + right.Value);
    public static Amount operator -(Amount left, Amount right) => Create(left.Value - right.Value);

    public override string ToString() => $"{Value:0.00} EUR";
}
