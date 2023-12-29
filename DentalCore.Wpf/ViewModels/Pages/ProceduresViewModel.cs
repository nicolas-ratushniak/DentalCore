using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using DentalCore.Domain.Abstract;
using DentalCore.Wpf.Abstract;
using DentalCore.Wpf.Commands;
using DentalCore.Wpf.ViewModels.Inners;

namespace DentalCore.Wpf.ViewModels.Pages;

public class ProceduresViewModel : BaseViewModel
{
    private readonly IProcedureService _procedureService;
    private readonly ObservableCollection<ProcedureListItemViewModel> _procedures;
    private string _procedureSearchFilter = string.Empty;

    public ICollectionView ProcedureCollectionView { get; }

    public ICommand ProcedureCreateCommand { get; }
    public ICommand ProcedureEditCommand { get; }
    public ICommand ProcedureDeleteCommand { get; }

    public string ProcedureSearchFilter
    {
        get => _procedureSearchFilter;
        set
        {
            if (value == _procedureSearchFilter) return;
            _procedureSearchFilter = value;
            
            OnPropertyChanged();
            ProcedureCollectionView.Refresh();
        }
    }

    public ProceduresViewModel(
        IProcedureService procedureService,
        IModalService modalService)
    {
        _procedureService = procedureService;
        _procedures = new ObservableCollection<ProcedureListItemViewModel>();

        ProcedureCollectionView = CollectionViewSource.GetDefaultView(_procedures);
        
        ProcedureCollectionView.SortDescriptions.Add(
            new SortDescription(nameof(ProcedureListItemViewModel.Name), ListSortDirection.Ascending));

        ProcedureCollectionView.Filter = o => 
            o is ProcedureListItemViewModel p &&
            p.Name.ToLower().Contains(ProcedureSearchFilter.ToLower());

        ProcedureCreateCommand = new RelayCommand<object>(_ => modalService.OpenModal(ModalType.ProcedureCreate));
        ProcedureEditCommand = new RelayCommand<int>(id => modalService.OpenModal(ModalType.ProcedureUpdate, id));
        ProcedureDeleteCommand = new RelayCommand<object>(_ => { });

        LoadedCommand = new AsyncRelayCommand(LoadData);
    }

    public override async Task LoadData()
    {
        _procedures.Clear();
        
        foreach (var procedure in await GetProceduresAsync())
        {
            _procedures.Add(procedure);
        }
    }
    
    private async Task<List<ProcedureListItemViewModel>> GetProceduresAsync()
    {
        return (await _procedureService.GetAllAsync())
            .Select(p => new ProcedureListItemViewModel()
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price
            })
            .ToList();
    }
}