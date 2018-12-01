
using AJdev.MicroMVVM.Base;
using AJdev.MicroMVVM.IOC;
using AJdev.MicroMVVM.Services;
using System.Threading.Tasks;

namespace AJdev.MicroMVVM.ViewModels
{
  public abstract class ViewModelBase : BindableBase
  {
    #region members
    protected static readonly IPopupService PopupService;
    protected static readonly INavigationService NavigationService;
    #endregion

    #region Properties
    private bool _isBusy;

    public bool IsBusy
    {
      get
      {
        return _isBusy;
      }

      set
      {
        SetProperty<bool>(ref _isBusy, value);
      }
    }
    #endregion

    #region Constructors
    static ViewModelBase()
    {
      NavigationService = Container.Instance.Get<INavigationService>();
      PopupService = Container.Instance.Get<IPopupService>();
    }

    public ViewModelBase()
    {
      //DialogService = Container.Instance.Get<IDialogService>();
    }
    #endregion

    #region methods
    public virtual Task InitializeAsync(object navigationData)
    {
      return Task.FromResult(false);
    }
    #endregion
  }
}