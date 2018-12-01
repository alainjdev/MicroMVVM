using System;

namespace AJdev.MicroMVVM.IOC
{
  internal class MappingKey
  {
    #region Properties
    /// <summary>
    /// Type de dépendance
    /// </summary>
    public Type Type { get; protected set; }

    /// <summary>
    /// Nom de l'instance (optionnel)
    /// </summary>
    public string InstanceName { get; protected set; }
    #endregion

    /// <summary>
    /// Crée une nouvelle instance of <see cref="MappingKey"/>
    /// </summary>
    /// <param name="type">Type de dépendance</param>
    /// <param name="instanceName">Nom de l'instance</param>
    /// <exception cref="ArgumentNullException">type</exception>
    public MappingKey(Type type, string instanceName)
    {
      if (type == null)
        throw new ArgumentNullException("type");

      Type = type;
      InstanceName = instanceName;
    }


    /// <summary>
    /// Retourne le Hash Code de cette instance
    /// </summary>
    /// <returns>Hash code</returns>
    public override int GetHashCode()
    {
      unchecked
      {
        const int multiplier = 31;
        int hash = GetType().GetHashCode();

        hash = hash * multiplier + Type.GetHashCode();
        hash = hash * multiplier + (InstanceName == null ? 0 : InstanceName.GetHashCode());

        return hash;
      }
    }


    /// <summary>
    /// Determine si l'oblet en paramètre est egal à l'objet courant.
    /// </summary>
    /// <param name="obj">Objet à comparer avec l'objet courant</param>
    /// <returns>
    /// <c>true</c> si l'objet est égal à l'objet courant; sinon, <c>false</c>.
    /// </returns>
    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;

      MappingKey compareTo = obj as MappingKey;

      if (ReferenceEquals(this, compareTo))
        return true;

      if (compareTo == null)
        return false;

      return Type.Equals(compareTo.Type) &&
          string.Equals(InstanceName, compareTo.InstanceName, StringComparison.InvariantCultureIgnoreCase);
    }

    /// <summary>
    /// pour le debbug
    /// </summary>
    /// <returns>Returns a string that represents the current object.</returns>
    public override string ToString()
    {
      const string format = "{0} ({1}) - hash code: {2}";

      return string.Format(format, this.InstanceName ?? "[null]",
          this.Type.FullName,
          this.GetHashCode()
      );
    }

    /// <summary>
    /// si on a besoin de retourner des infos à l'application cliente
    /// </summary>
    /// <returns></returns>
    public string ToTraceString()
    {
      const string format = "Instance Name: {0} ({1})";

      return string.Format(format, this.InstanceName ?? "[null]",
          this.Type.FullName
      );
    }
  }
}
