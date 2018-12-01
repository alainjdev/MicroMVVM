using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace AJdev.MicroMVVM.Services
{
  /// <summary>
  /// Classe représentant les paramètres de navigation 
  /// Les paramètres sont reprsenté dans un Dictionnaire clé, valeurs
  /// </summary>
  public class NavigationParameters : Dictionary<string, object>
  {
    #region Constants
    private readonly List<KeyValuePair<string, object>> _entries = new List<KeyValuePair<string, object>>();
    #endregion

    #region Properties

    /// <summary>
    /// Retourne le paramètre défini par sa clé
    /// </summary>
    /// <param name="key">clé du paramètre</param>
    /// <returns>valeur du paramètre ou nul si la clé n'existe pas</returns>
    public new object this[string key]
    {
      get
      {
        if (ContainsKey(key))
        {
          return base[key];
        }
        else
        {
          return null;
        }
      }
    }
    #endregion

    #region methods
    public T GetValue<T>(string key)
    {
      var value = this[key];

      if (value == null)
      {
        return default(T);
      }
      else if (value.GetType() == typeof(T))
      {
        return (T)value;
      }
      else if (typeof(T).GetTypeInfo().IsAssignableFrom(value.GetType().GetTypeInfo()))
      {
        return (T)value;
      }
      else
      {
        return (T)Convert.ChangeType(value, typeof(T));
      }
    }

    public IEnumerable<T> GetValues<T>(string key)
    {
      return Keys.Select(k => GetValue<T>(k)).AsEnumerable<T>();
    }

    public bool TryGetValue<T>(string key, out T value)
    {
      value = GetValue<T>(key);
      return (value != null);
    }


    public override string ToString()
    {
      var queryBuilder = new StringBuilder();

      if (_entries.Count > 0)
      {
        queryBuilder.Append('?');
        var first = true;

        foreach (string k in Keys)
        {
          if (!first)
          {
            queryBuilder.Append('&');
          }
          else
          {
            first = false;
          }

          queryBuilder.Append(Uri.EscapeDataString(k));
          queryBuilder.Append('=');
          queryBuilder.Append(Uri.EscapeDataString(this[k].ToString()));
        }
      }

      return queryBuilder.ToString();
    }
    #endregion
  }
}
