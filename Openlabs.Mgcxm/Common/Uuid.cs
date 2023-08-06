using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Openlabs.Mgcxm.Common;

internal interface IUuid
{
    string ToString();

    int GetUniqueId();
    Guid Id { get; }
}

/// <summary>
/// Represents a universally unique identifier (UUID) and provides methods for working with UUIDs.
/// </summary>
public class Uuid : IEquatable<Uuid>, IUuid
{
    /// <summary>
    /// Initializes a new instance of the Uuid class with a randomly generated unique identifier.
    /// </summary>
    public Uuid()
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Initializes a new instance of the Uuid class with the specified unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier for the Uuid.</param>
    public Uuid(Guid id)
    {
        Id = id;
    }

    /// <summary>
    /// Initializes a new instance of the Uuid class with the specified unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier for the Uuid in MD5 format.</param>
    public Uuid(string id)
    {
        if (id.Length != 32)
            throw new ArgumentException("ID is not MD5 format.");

       Id = Guid.Parse(
           string.Format(
               "{0}-{1}-{2}-{3}",
               id.Substring(0, 8),
               id.Substring(8, 4),
               id.Substring(12, 4),
               id.Substring(16, 4),
               id.Substring(20, 12)
               )
           );
    }

    /// <summary>
    /// Gets the unique identifier of the Uuid.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the string representation of the Uuid.
    /// </summary>
    /// <returns>A string representing the Uuid.</returns>
    public override string ToString()
    {
        return Id.ToString();
    }

    /// <summary>
    /// Gets a unique numeric identifier for the Uuid.
    /// </summary>
    /// <returns>An integer representing the unique numeric identifier.</returns>
    public int GetUniqueId()
    {
        // Example implementation: Convert the first 4 bytes of the Guid to an integer.
        return BitConverter.ToInt32(Id.ToByteArray(), 0);
    }

    /// <summary>
    /// Determines whether this Uuid is equal to another Uuid.
    /// </summary>
    /// <param name="other">The Uuid to compare with this Uuid.</param>
    /// <returns>True if the Uuids are equal; otherwise, false.</returns>
    public bool Equals(Uuid other)
    {
        if (other == null)
            return false;

        return Id == other.Id;
    }

    /// <summary>
    /// Determines whether this Uuid is equal to another object.
    /// </summary>
    /// <param name="obj">The object to compare with this Uuid.</param>
    /// <returns>True if the object is a Uuid and is equal to this Uuid; otherwise, false.</returns>
    public override bool Equals(object obj)
    {
        if (obj is Uuid otherUuid)
            return Equals(otherUuid);

        return false;
    }

    /// <summary>
    /// Returns the hash code for this Uuid.
    /// </summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    public override int GetHashCode() => Id.GetHashCode();

    // Additional helper method (example)
    /// <summary>
    /// Creates a new Uuid with a randomly generated unique identifier.
    /// </summary>
    /// <returns>A new Uuid instance with a random unique identifier.</returns>
    public static Uuid Create() => new Uuid();

    public static readonly Uuid Empty = new Uuid(Guid.Empty);
}