using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows.Input;
using DentalCore.Domain.Abstract;
using DentalCore.Domain.Dto;
using DentalCore.Wpf.Abstract;
using DentalCore.Wpf.Commands;

namespace DentalCore.Wpf.ViewModels.Modals;

public class ProcedureCreateViewModel : BaseViewModel
{
    private readonly IProcedureService _procedureService;
    private readonly IModalService _modalService;
    
    private string _name;
    private int _price;
    private string? _errorMessage;

    public ICommand CancelCommand { get; }
    public ICommand CreateProcedureCommand { get; }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set
        {
            if (value == _errorMessage) return;
            _errorMessage = value;
            OnPropertyChanged();
        }
    }

    public string Name
    {
        get => _name;
        set
        {
            if (value == _name) return;
            _name = value;
            OnPropertyChanged();
        }
    }

    public int Price
    {
        get => _price;
        set
        {
            if (value == _price) return;
            _price = value;
            OnPropertyChanged();
        }
    }

    public ProcedureCreateViewModel(
        IProcedureService procedureService,
        IModalService modalService)
    {
        _procedureService = procedureService;
        _modalService = modalService;

        CancelCommand = new RelayCommand(modalService.CloseModal);
        CreateProcedureCommand = new AsyncRelayCommand(CreateProcedure_Execute);
    }

    private async Task CreateProcedure_Execute()
    {
        var dto = new ProcedureCreateDto
        {
            Name = Name,
            Price = Price,
            IsDiscountAllowed = true
        };

        try
        {
            await _procedureService.AddAsync(dto);
            _modalService.CloseModalWithPageReload();
        }
        catch (ValidationException ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}