using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows.Input;
using DentalCore.Domain.Abstract;
using DentalCore.Domain.Dto;
using DentalCore.Wpf.Abstract;
using DentalCore.Wpf.Commands;

namespace DentalCore.Wpf.ViewModels.Modals;

public class ProcedureUpdateViewModel : BaseViewModel
{
    private readonly int _procedureId;
    private readonly IProcedureService _procedureService;
    private readonly IModalService _modalService;
    
    private string _name;
    private int _price;
    private string? _errorMessage;

    public ICommand CancelCommand { get; }
    public ICommand UpdateProcedureCommand { get; }

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

    public ProcedureUpdateViewModel(
        int id,
        IProcedureService procedureService,
        IModalService modalService)
    {
        _procedureId = id;
        _procedureService = procedureService;
        _modalService = modalService;

        CancelCommand = new RelayCommand<object>(_ => modalService.CloseModal());
        UpdateProcedureCommand = new AsyncCommand(UpdateProcedure_Execute);
    }

    public override async Task LoadDataAsync()
    {
        try
        {
            var procedure = await _procedureService.GetAsync(_procedureId);

            Name = procedure.Name;
            Price = procedure.Price;
        }
        catch (Exception e)
        {
            _modalService.CloseModal();
        }
    }

    private async Task UpdateProcedure_Execute()
    {
        var procedure = await _procedureService.GetAsync(_procedureId);
        
        var dto = new ProcedureUpdateDto
        {
            Id = _procedureId,
            Name = Name,
            Price = Price,
            IsDiscountAllowed = procedure.IsDiscountAllowed
        };

        try
        {
            await _procedureService.UpdateAsync(dto);
            _modalService.CloseModalWithPageReload();
        }
        catch (ValidationException ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}