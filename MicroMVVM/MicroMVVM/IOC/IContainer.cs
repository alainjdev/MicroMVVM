using System;

namespace AJdev.MicroMVVM.IOC
{
  public interface IContainer
  {
    void Register<TFrom, TTo>() where TTo : TFrom;
    void Register<TFrom, TTo>(string InstanceName) where TTo : TFrom;
    void Register<TFrom, TTo>(FetchTarget target) where TTo : TFrom;
    void Register<TFrom, TTo>(string InstanceName, FetchTarget target) where TTo : TFrom;
    void Register<T>(string InstanceName, FetchTarget target);
    void Register(Type from, Type to, string InstanceName, FetchTarget target);
    void Register(Type to, string InstanceName, FetchTarget target);

    void UnRegister<T>(string InstanceName = null);
    void UnRegister(Type type, string InstanceName = null);
    void UnRegister(string InstanceName);

    bool IsRegistered<T>(string InstanceName = null);
    bool IsRegistered(Type type, string InstanceName = null);
    bool IsRegistered(string InstanceName);

    T Get<T>();
    T Get<T>(string InstanceName);
    T Get<T>(FetchTarget target);
    T Get<T>(string InstanceName, FetchTarget target);
    Object Get(string InstanceName, FetchTarget target);
    Object Get(Type type, string InstanceName = null, FetchTarget target = FetchTarget.GlobalInstance);
  }
}
