using AJdev.MicroMVVM.IOC;
using AJdev.MicroMVVM.Services;
using System;
using System.Linq;
using Xamarin.Forms;

namespace AJdev.MicroMVVM.ViewModels
{
  public static class ViewModelLocator
  {
    #region Properties
    public static readonly BindableProperty AutoWireViewModelProperty = BindableProperty.CreateAttached(
      "AutoWireViewModel",
      typeof(bool),
      typeof(ViewModelLocator),
      default(bool),
      propertyChanged: OnAutoWireViewModelChanged);

    public static bool GetAutoWireViewModel(BindableObject bindable)
    {
      return (bool)bindable.GetValue(ViewModelLocator.AutoWireViewModelProperty);
    }

    public static void SetAutoWireViewModel(BindableObject bindable, bool value)
    {
      bindable.SetValue(ViewModelLocator.AutoWireViewModelProperty, value);
    }
    #endregion

    #region Cnstructors
    static ViewModelLocator()
    {
      // Enregistrement des services de navigation et de Popup
      Container.Instance.Register<IPopupService, PopupService>();
      Container.Instance.Register<INavigationService, NavigationService>();
    }
    #endregion

    #region methods
    /// <summary>
    /// Permet de lier un viewmodel à une vue.
    /// la liaison est faite par le nom
    /// Le type de la vue doit etre du type VVVView et le ModelView correspondant doit être VVVViewModel
    /// Le type du ViewModel doit hériter de ViewModelBase et être dans une des assemblies déjà chargés.
    /// Le ViewModel doit être enregistré dans l'IOC
    /// Le view modèle est alors instancié ou récupéré s'il existe déjà et le BindingContext de la vue est mis à jour.
    /// </summary>
    /// <param name="bindable">Vue</param>
    /// <param name="oldValue">Ancienne valeur de la propriété</param>
    /// <param name="newValue">Nouvelle valeur de la propriété</param>
    private static void OnAutoWireViewModelChanged(BindableObject bindable, object oldValue, object newValue)
    {
      var view = bindable as Element;
      if (view == null)
      {
        return;
      }

      Type viewType = view.GetType();
      string viewname = viewType.Name;
      if (viewname.EndsWith("View") == false)
      {
        return;
      }

      string viewModelTypeName = $"{viewname}Model";

      // Recherche un type dérivant de ViewModeleBase et portant le Nom viewModelTypeName dans les assembleis chargées
      Type viewModelType = (from asm in AppDomain.CurrentDomain.GetAssemblies()
                            where (asm.IsDynamic == false)
                            from t in asm.GetTypes()
                            where t.Name.Equals(viewModelTypeName) && t.IsSubclassOf(typeof(ViewModelBase))
                            select t).FirstOrDefault();

      if (viewModelType == null)
      {
        return;
      }

      var viewModel = Container.Instance.Get(viewModelType);
      view.BindingContext = viewModel;
    }
    #endregion
  }
}