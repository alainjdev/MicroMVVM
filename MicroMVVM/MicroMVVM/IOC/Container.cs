using System;
using System.Collections.Generic;
using System.Linq;

namespace AJdev.MicroMVVM.IOC
{
  public class Container : IContainer
  {
    #region Constants
    #endregion

    #region members 
    private static Container m_instance = null;

    private Dictionary<MappingKey, Type> m_map = null;

    private Dictionary<MappingKey, Object> m_GlobalInstances = null;
    #endregion

    #region Properties
    /// <summary>
    /// Pattern singleton: retourne l'instance du container
    /// </summary>
    public static Container Instance
    {
      get
      {
        if (m_instance == null)
        {
          m_instance = new Container();
        }
        return m_instance;
      }
    }

    #endregion

    #region Constructors
    private Container()
    {
      m_map = new Dictionary<MappingKey, Type>();
      m_GlobalInstances = new Dictionary<MappingKey, object>();
    }
    #endregion

    #region methods
    #region Register
    public void Register<TFrom, TTo>() where TTo : TFrom
    {
      Register<TFrom, TTo>(null, FetchTarget.GlobalInstance);
    }

    public void Register<TFrom, TTo>(string InstanceName) where TTo : TFrom
    {
      Register<TFrom, TTo>(InstanceName, FetchTarget.GlobalInstance);
    }

    public void Register<TFrom, TTo>(FetchTarget target) where TTo : TFrom
    {
      Register<TFrom, TTo>(null, target);
    }

    public void Register<TFrom, TTo>(string InstanceName, FetchTarget target) where TTo : TFrom
    {
      Register(typeof(TFrom), typeof(TTo), InstanceName, target);
    }

    public void Register<T>(string InstanceName, FetchTarget target)
    {
      if (string.IsNullOrEmpty(InstanceName))
      {
        Register<T, T>(null, target);
      }
      else
      {
        Register<Object, T>(InstanceName, target);
      }
    }

    public void Register(Type to, string InstanceName, FetchTarget target)
    {
      if (string.IsNullOrEmpty(InstanceName))
      {
        Register(to, to, null, target);
      }
      else
      {
        Register(typeof(Object), to, InstanceName, target);
      }
    }

    public void Register(Type from, Type to, string InstanceName = null, FetchTarget target = FetchTarget.GlobalInstance)
    {
      if (to == null)
      {
        throw new ArgumentNullException("to");
      }

      if (!from.IsAssignableFrom(to))
      {
        string errorMessage = $"Error trying to register the instance: '{from.FullName}' is not assignable from '{to.FullName}'";
        throw new InvalidOperationException(errorMessage);
      }

      var key = new MappingKey(from, InstanceName);

      if (m_map.ContainsKey(key))
      {
        string errorMessageFormat = $"The requested mapping already exists - {key.ToTraceString()}";
        throw new InvalidOperationException(errorMessageFormat);
      }

      m_map.Add(key, to);
    }
    #endregion

    #region UnRegister
    public void UnRegister<T>(string InstanceName = null)
    {
      UnRegister(typeof(T), InstanceName);
    }

    public void UnRegister(string InstanceName)
    {
      if (String.IsNullOrEmpty(InstanceName))
      {
        throw new ArgumentNullException("InstanceName must not be null");
      }
      UnRegister<Object>(InstanceName);
    }

    public void UnRegister(Type type, string InstanceName = null)
    {
      var key = new MappingKey(type, InstanceName);

      if (m_map.ContainsKey(key))
      {
        m_map.Remove(key);
      }

      if (m_GlobalInstances.ContainsKey(key))
      {
        m_GlobalInstances.Remove(key);
      }
    }
    #endregion

    #region IsRegister
    public bool IsRegistered<T>(string InstanceName = null)
    {
      return IsRegistered(typeof(T), InstanceName);
    }

    public bool IsRegistered(string InstanceName)
    {
      if (String.IsNullOrEmpty(InstanceName))
      {
        throw new ArgumentNullException("InstanceName must not be null");
      }
      return IsRegistered<Object>(InstanceName);
    }

    public bool IsRegistered(Type type, string InstanceName = null)
    {
      var key = new MappingKey(type, InstanceName);
      return m_map.ContainsKey(key);
    }
    #endregion

    #region Get
    public T Get<T>()
    {
      return Get<T>(null, FetchTarget.GlobalInstance);
    }

    public T Get<T>(string InstanceName)
    {
      return Get<T>(InstanceName, FetchTarget.GlobalInstance);
    }

    public T Get<T>(FetchTarget target)
    {
      return Get<T>(null, target);
    }

    public T Get<T>(string InstanceName, FetchTarget target)
    {
      return (T)Get(typeof(T), InstanceName, target);
    }

    public object Get(string InstanceName, FetchTarget target)
    {
      if (String.IsNullOrEmpty(InstanceName))
      {
        throw new ArgumentNullException("InstanceName must not be null");
      }

      return Get<Object>(InstanceName, target);
    }

    /// <summary>
    /// Retourne l'objet associé au type.
    /// Si le premier constructeur impose des paramètres, le type de ceux ci doivent être enregistrés
    /// Pour chacun des paramètres on utilisera la version non nommé et l'instance globale.
    /// </summary>
    /// <param name="type">Type à partir duquel il faut retourner un objet</param>
    /// <param name="InstanceName">si non null permet de distinguer plusieurs instances</param>
    /// <param name="target">permet de retourner une instance gloable ou bien de regénrer une nouvelle instance</param>
    /// <returns>objet associé</returns>
    public object Get(Type type, string InstanceName = null, FetchTarget target = FetchTarget.GlobalInstance)
    {
      var key = new MappingKey(type, InstanceName);
      if (target == FetchTarget.GlobalInstance)
      {
        if (m_GlobalInstances.ContainsKey(key))
        {
          return m_GlobalInstances[key];
        }
      }

      if (m_map.ContainsKey(key) == false)
      {
        string errorMessageFormat = $"The requested mapping does not exists - {key.ToTraceString()}";
        throw new InvalidOperationException(errorMessageFormat);
      }

      Type rType = m_map[key];

      Object o = null;

      var firstConstructor = rType.GetConstructors().FirstOrDefault();
      var constructorParameters = firstConstructor.GetParameters();
      if (constructorParameters.Count() == 0)
      {
        // constructeur sans paramètres
        o = Activator.CreateInstance(rType);
      }
      else
      {
        // Constructeur avec paramètre
        IList<Object> parameters = new List<Object>();
        foreach (var parameterToResolve in constructorParameters)
        {
          // Version globale et sans nom d'instance 
          parameters.Add(Get(parameterToResolve.ParameterType));
        }

        o = firstConstructor.Invoke(parameters.ToArray());
      }

      if (target == FetchTarget.GlobalInstance)
      {
        m_GlobalInstances[key] = o;
      }
      return o;
    }

    #endregion

    #endregion

    #region Helpers
    #endregion

  }
}
