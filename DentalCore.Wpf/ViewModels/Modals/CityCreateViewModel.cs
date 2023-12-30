using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows.Input;
using DentalCore.Domain.Abstract;
using DentalCore.Domain.Dto;
using DentalCore.Wpf.Abstract;
using DentalCore.Wpf.Commands;

namespace DentalCore.Wpf.ViewModels.Modals;

public class CityCreateViewModel : BaseViewModel
{
    private readonly ICommonService _commonService;
    private readonly IModalService _modalService;

    private string _name;
    private string? _errorMessage;
    
    public ICommand CancelCommand { get; }
    public ICommand CreateCityCommand { get; }
    
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

    public CityCreateViewModel(ICommonService commonService, IModalService modalService)
    {
        _commonService = commonService;
        _modalService = modalService;

        CancelCommand = new RelayCommand(modalService.CloseModal);
        CreateCityCommand = new AsyncRelayCommand(CreateCity_Execute);
    }

    private async Task CreateCity_Execute()
    {
        var dto = new CityCreateDto
        {
            Name = Name
        };

        try
        {
            await _commonService.AddCityAsync(dto);
            _modalService.CloseModal();
        }
        catch (ValidationException ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}