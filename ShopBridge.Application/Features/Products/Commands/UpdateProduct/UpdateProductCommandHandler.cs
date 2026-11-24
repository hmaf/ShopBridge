using MediatR;
using ShopBridge.Application.Abstractions.Persistence;
using ShopBridge.Application.Abstractions.Repositories;
using ShopBridge.Application.Common.Mapping;
using ShopBridge.Domain.Entities;
using ShopBridge.Domain.Exceptions;

namespace ShopBridge.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler
    : IRequestHandler<UpdateProductCommand, ApiResponse<Guid>>
{
    private readonly IGenericRepository<Product> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductCommandHandler(
        IGenericRepository<Product> repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<Guid>> Handle(
        UpdateProductCommand request, 
        CancellationToken cancellationToken)
    {
        var product = await _repository.GetById(
            request.id,
            cancellationToken);

        if (product is null)
        {
            BusinessException.Throw(
                611241429,
                $"Product with ID '{request.id}' was not found.",
                BusinessErrorType.NotFound);
        }

        product.UpdateInformation(
            request.name,
            request.price,
            request.stock);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<Guid>.SuccessResponse(product.Id);
    }
}
