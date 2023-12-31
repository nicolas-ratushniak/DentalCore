using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using DentalCore.Wpf.Abstract;
using DentalCore.Wpf.ViewModels.Inners;

namespace DentalCore.Wpf.ViewModels.Components;

public class DoctorSelectorViewModel : BaseViewModel
{
    private DoctorListItemViewModel? _selectedDoctor;
    private string _doctorSearchFilter = string.Empty;
    private bool _isDoctorListVisible;

    public ObservableCollection<DoctorListItemViewModel> Doctors { get; }
    public ICollectionView DoctorCollectionView { get; }

    public string DoctorSearchFilter
    {
        get => _doctorSearchFilter;
        set
        {
            if (value == _doctorSearchFilter) return;
            _doctorSearchFilter = value;

            OnPropertyChanged();
            OnDoctorFilterChanged();
        }
    }

    public DoctorListItemViewModel? SelectedDoctor
    {
        get => _selectedDoctor;
        set
        {
            if (Equals(value, _selectedDoctor)) return;
            _selectedDoctor = value;

            OnPropertyChanged();
            OnSelectedDoctorChanged();
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public bool IsDoctorListVisible
    {
        get => _isDoctorListVisible;
        set
        {
            if (value == _isDoctorListVisible) return;
            _isDoctorListVisible = value;
            OnPropertyChanged();
        }
    }

    public DoctorSelectorViewModel()
    {
        Doctors = new ObservableCollection<DoctorListItemViewModel>();
        DoctorCollectionView = CollectionViewSource.GetDefaultView(Doctors);
        
        DoctorCollectionView.Filter = o =>
        {
            if (o is DoctorListItemViewModel d)
            {
                return d.FullName.ToLower().StartsWith(DoctorSearchFilter.ToLower()) ||
                       d.Name.ToLower().StartsWith(DoctorSearchFilter.ToLower());
            }

            return false;
        };
    }
    
    private void OnDoctorFilterChanged()
    {
        var filter = DoctorSearchFilter;

        if (_selectedDoctor is null)
        {
            IsDoctorListVisible = !string.IsNullOrEmpty(filter);
        }
        else
        {
            if (filter == _selectedDoctor.FullName)
            {
                IsDoctorListVisible = false;
                return;
            }

            IsDoctorListVisible = true;
            SelectedDoctor = null;
        }

        DoctorCollectionView.Refresh();
    }

    private void OnSelectedDoctorChanged()
    {
        if (_selectedDoctor is null)
        {
            return;
        }

        DoctorSearchFilter = _selectedDoctor.FullName;
        IsDoctorListVisible = false;
    }
}