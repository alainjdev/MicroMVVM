using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace AJdev.MicroMVVM.Base
{
  public abstract class BindableBase : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
    {
      if (Equals(storage, value))
      {
        return false;
      }

      storage = value;
      this.OnPropertyChanged(propertyName);
      return true;
    }

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
