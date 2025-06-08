using Kopilych.Application.CQRS.Commands.PiggyBank.CreatePiggyBank;
using Kopilych.Application.CQRS.Commands.PiggyBank.DeletePiggyBank;
using Kopilych.Application.CQRS.Commands.PiggyBank.UpdatePiggyBank;
using Kopilych.Application.CQRS.Commands.PiggyBankCustomization.CreatePiggyBankCustomization;
using Kopilych.Application.CQRS.Commands.PiggyBankCustomization.UpdatePiggyBankCustomization;
using Kopilych.Application.CQRS.Commands.UserPiggyBank.CreateUserPiggyBank;
using Kopilych.Application.CQRS.Commands.UserPiggyBank.DeleteUserPiggyBank;
using Kopilych.Application.CQRS.Queries.PiggyBank.GetPiggyBank;
using Kopilych.Application.CQRS.Queries.PiggyBank.GetPiggyBankByExternalId;
using Kopilych.Application.CQRS.Queries.PiggyBank.GetUserPiggyBanksCount;
using Kopilych.Application.CQRS.Queries.PiggyBankCustomization.GetPiggyBankCustomizationById;
using Kopilych.Application.CQRS.Queries.PiggyBankCustomization.GetPiggyBankCustomizationByPiggyBankId;
using Kopilych.Application.CQRS.Queries.PiggyBankType.GetAllPiggyBankTypes;
using Kopilych.Application.CQRS.Queries.PiggyBankType.GetPiggyBankTypeById;
using Kopilych.Application.CQRS.Queries.UserPiggyBank.GetLinksToCommonPiggyBank;
using Kopilych.Application.CQRS.Queries.UserPiggyBank.GetUserPiggyBankLink;
using Kopilych.Application.CQRS.Queries.UserPiggyBank.GetUserPiggyBankLinkByUserIdAndPiggyBankId;
using Kopilych.Application.CQRS.Queries.UserPiggyBank.GetUserPiggyBankLinksByPiggyBankId;
using Kopilych.Application.CQRS.Queries.UserPiggyBank.GetUserPiggyBankLinksByUserId;
using Kopilych.Application.Interfaces;
using Kopilych.Domain;
using Kopilych.Shared;
using Kopilych.Shared.DTO;
using Kopilych.Shared.View_Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using AutoMapper;
using Kopilych.Application.CQRS.Commands.UserPiggyBank.UpdateUserPiggyBank;

namespace Kopilych.Application.Services
{
    internal class PiggyBankService : IPiggyBankService
    {
        private IMediator _mediator;
        private IFileService _fileService;
        private IMapper _mapper;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IIntegrationService _integrationService;

        public PiggyBankService(IServiceScopeFactory serviceScopeFactory, IIntegrationService integrationService)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _integrationService = integrationService;
        }
        public async Task<PiggyBankDTO> GetPiggyBankDetailsAsync(int piggyBankId, CancellationToken cancellationToken, bool remote)
        {
            PiggyBankDTO result = null;
            if (!remote)
                result = await ExecuteWithMediator(async mediator => await mediator.Send(new GetPiggyBankQuery { Id = piggyBankId, InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken));
            else
            {
                result = await _integrationService.GetPiggyBankFromServerAsync(piggyBankId, cancellationToken);
                result.ExternalId = result.Id;
            }
            return result;
        }

        public async Task DeletePiggyBankAsync(int piggyBankId, CancellationToken cancellationToken, bool remote)
        {
            if (!remote)
                await ExecuteWithMediator(async mediator => await mediator.Send(new DeletePiggyBankCommand { Id = piggyBankId, InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken));
            else
            {
                await _integrationService.DeletePiggyBankFromServerAsync(piggyBankId, cancellationToken);
            }
        }


        public async Task<int> GetCommonPiggyBanksCountForUsersAsync(int userId1, int userId2, CancellationToken cancellationToken)
        {
            return (await ExecuteWithMediator(async mediator => await _mediator.Send(new GetCommonPiggyBanksIdsForUsersQuery { FirstUserId = userId1, SecondUserId = userId2, IsExecuteByAdmin = true, InitiatorUserId = 0 }, cancellationToken))).Count();
        }

        public async Task<List<PiggyBankTypeDTO>> GetAllPiggyBankTypesAsync(CancellationToken cancellationToken)
        {
            return (await ExecuteWithMediator(async mediator => await _mediator.Send(new GetAllPiggyBankTypesQuery { IsExecuteByAdmin = true, InitiatorUserId = 0 }, cancellationToken)));
        }


        public async Task<int> GetUserPiggyBankLinksCountForPiggyBank(int piggyBankId, CancellationToken cancellationToken)
        {
            return (await ExecuteWithMediator(async mediator => await _mediator.Send(new GetUserPiggyBankLinksByPiggyBankIdQuery { PiggyBankId = piggyBankId, InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken))).Count();
        }

        public async Task<List<int>> CreatePiggyBankWithCustomizationAndLinkToUser(CreatePiggyBankDTO piggyBank, CreatePiggyBankCustomizationDTO piggyBankCustomization, CancellationToken cancellationToken, bool remote)
        {
            var piggyBankId = (await ExecuteWithMediator(async mediator => await _mediator.Send(new CreatePiggyBankCommand { Balance = piggyBank.Balance, Description = piggyBank.Description, Goal = piggyBank.Goal, GoalDate = piggyBank.GoalDate, InitiatorUserId = 0, IsExecuteByAdmin = true, Name = piggyBank.Name, OwnerId = piggyBank.OwnerId, Shared = piggyBank.Shared, Version = piggyBank.Version, ExternalId = piggyBank.ExternalId }, cancellationToken)));
            var customizationId = (await ExecuteWithMediator(async mediator => await _mediator.Send(new CreatePiggyBankCustomizationCommand { PiggyBankId = piggyBankId, PiggyBankTypeId = piggyBankCustomization.PiggyBankTypeId, PhotoPath = piggyBankCustomization.PhotoPath, Version = piggyBankCustomization.Version, InitiatorUserId = 0, IsExecuteByAdmin = true, ExternalId = piggyBankCustomization.ExternalId }, cancellationToken)));
            var link = (await ExecuteWithMediator(async mediator => await _mediator.Send(new CreateUserPiggyBankCommand { PiggyBankId = piggyBankId, HideBalance = false, Public = true, UserId = piggyBank.OwnerId, InitiatorUserId = 0, IsExecuteByAdmin = true, ExternalId = null, Version = 0 }, cancellationToken)));
            return new List<int>() { piggyBankId, customizationId, link };
        }

        public async Task<int> CreatePiggyBankAsync(CreatePiggyBankDTO piggyBank, CancellationToken cancellationToken, bool remote)
        {
            int result = 0;
            if (!remote)
               result = (await ExecuteWithMediator(async mediator => await _mediator.Send(new CreatePiggyBankCommand { Balance = piggyBank.Balance, Description = piggyBank.Description, Goal = piggyBank.Goal, GoalDate = piggyBank.GoalDate, InitiatorUserId = 0, IsExecuteByAdmin = true, Name = piggyBank.Name, OwnerId = piggyBank.OwnerId, Shared = piggyBank.Shared, Version = piggyBank.Version, ExternalId = piggyBank.ExternalId }, cancellationToken)));
            else
                result = await _integrationService.CreatePiggyBankOnServerAsync(piggyBank, cancellationToken);
            return result;
        }

        public async Task<int> CreatePiggyBankCustomizationAsync(CreatePiggyBankCustomizationDTO piggyBankCustomization, CancellationToken cancellationToken, bool remote)
        {
            int result = 0;
            if (!remote)
                result = (await ExecuteWithMediator(async mediator => await _mediator.Send(new CreatePiggyBankCustomizationCommand { PiggyBankId = piggyBankCustomization.PiggyBankId, PiggyBankTypeId = piggyBankCustomization.PiggyBankTypeId, PhotoPath = piggyBankCustomization.PhotoPath, Version = piggyBankCustomization.Version, InitiatorUserId = 0, IsExecuteByAdmin = true, ExternalId = piggyBankCustomization.ExternalId }, cancellationToken)));
            else
                result = await _integrationService.CreatePiggyBankCustomizationOnServerAsync(piggyBankCustomization, cancellationToken);
            return result;
        }

        public async Task<int> CreateUserPiggyBankLinkAsync(CreateUserPiggyBankDTO userPiggyBank, CancellationToken cancellationToken, bool remote)
        {
            int result = 0;
            if (!remote)
                result = (await ExecuteWithMediator(async mediator => await _mediator.Send(new CreateUserPiggyBankCommand { PiggyBankId = userPiggyBank.PiggyBankId, HideBalance = userPiggyBank.HideBalance, Public = userPiggyBank.Public, UserId = userPiggyBank.UserId, InitiatorUserId = 0, IsExecuteByAdmin = true, ExternalId = userPiggyBank.ExternalId, Version = userPiggyBank.Version }, cancellationToken)));
            else 
            {
                result = await _integrationService.CreateUserPiggyBankLinkOnServerAsync(userPiggyBank, cancellationToken);
            }
            return result;
        }

        public async Task UpdateUserPiggyBankLinkAsync(int userPiggyBankId, UpdateUserPiggyBankDTO updateUserPiggyBank, CancellationToken cancellationToken, bool remote)
        {
            if (!remote)
            {
                var upb = (await ExecuteWithMediator(async mediator => await _mediator.Send(new GetUserPiggyBankLinkQuery { Id = userPiggyBankId, InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken)));
                var result = (await ExecuteWithMediator(async mediator => await _mediator.Send(new UpdateUserPiggyBankCommand
                {
                    Id = upb.Id,
                    InitiatorUserId = 0,
                    IsExecuteByAdmin = true,
                    Version = updateUserPiggyBank.EditedFields.Contains(nameof(updateUserPiggyBank.Version)) ? updateUserPiggyBank.Version.Value : upb.Version,
                    HideBalance = updateUserPiggyBank.EditedFields.Contains(nameof(updateUserPiggyBank.HideBalance)) ? updateUserPiggyBank.HideBalance.Value : upb.HideBalance,
                    Public = updateUserPiggyBank.EditedFields.Contains(nameof(updateUserPiggyBank.Public)) ? updateUserPiggyBank.Public.Value : upb.Public,
                    ExternalId = updateUserPiggyBank.EditedFields.Contains(nameof(updateUserPiggyBank.ExternalId)) ? updateUserPiggyBank.ExternalId : upb.ExternalId
                }, cancellationToken)));
            }
            else
            {
                await _integrationService.UpdateUserPiggyBankLinkOnServerAsync(userPiggyBankId, updateUserPiggyBank, cancellationToken);
            }
        }


        public async Task UpdatePiggyBankAsync(int piggyBankId, UpdatePiggyBankDTO updatePiggyBankDTO, CancellationToken cancellationToken, bool remote)
        {
            if (!remote)
            {
                var piggybank = await this.GetPiggyBankDetailsAsync(piggyBankId, cancellationToken, remote);
                var result = (await ExecuteWithMediator(async mediator => await _mediator.Send(new UpdatePiggyBankCommand
                {
                    Id = piggybank.Id,
                    InitiatorUserId = 0,
                    Description = updatePiggyBankDTO.EditedFields.Contains(nameof(updatePiggyBankDTO.Description)) ? updatePiggyBankDTO.Description : piggybank.Description,
                    Balance = updatePiggyBankDTO.EditedFields.Contains(nameof(updatePiggyBankDTO.Balance)) ? updatePiggyBankDTO.Balance.Value : piggybank.Balance,
                    GoalDate = updatePiggyBankDTO.EditedFields.Contains(nameof(updatePiggyBankDTO.GoalDate)) ? updatePiggyBankDTO.GoalDate : piggybank.GoalDate,
                    Goal = updatePiggyBankDTO.EditedFields.Contains(nameof(updatePiggyBankDTO.Goal)) ? updatePiggyBankDTO.Goal : piggybank.Goal,
                    Name = updatePiggyBankDTO.EditedFields.Contains(nameof(updatePiggyBankDTO.Name)) ? updatePiggyBankDTO.Name : piggybank.Name,
                    IsExecuteByAdmin = true,
                    Shared = updatePiggyBankDTO.EditedFields.Contains(nameof(updatePiggyBankDTO.Shared)) ? updatePiggyBankDTO.Shared.Value : piggybank.Shared,
                    Version = updatePiggyBankDTO.EditedFields.Contains(nameof(updatePiggyBankDTO.Version)) ? updatePiggyBankDTO.Version.Value : piggybank.Version,
                    IsDeleted = updatePiggyBankDTO.EditedFields.Contains(nameof(updatePiggyBankDTO.IsDeleted)) ? updatePiggyBankDTO.IsDeleted : piggybank.IsDeleted,
                    ExternalId = updatePiggyBankDTO.EditedFields.Contains(nameof(updatePiggyBankDTO.ExternalId)) ? updatePiggyBankDTO.ExternalId : piggybank.ExternalId,
                }, cancellationToken)));
            }
            else
            {
                await _integrationService.UpdatePiggyBankOnServerAsync(piggyBankId, updatePiggyBankDTO, cancellationToken);
            }
        }

        public async Task UpdatePiggyBankCustomizationAsync(int piggyBankCustomizationId, UpdatePiggyBankCustomizationDTO updatePiggyBankCustomizationDTO, CancellationToken cancellationToken, bool remote)
        {
            if (!remote)
            {
                var customization = (await ExecuteWithMediator(async mediator => await _mediator.Send(new GetPiggyBankCustomizationByIdQuery { Id = piggyBankCustomizationId, InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken)));
                var result = (await ExecuteWithMediator(async mediator => await _mediator.Send(new UpdatePiggyBankCustomizationCommand
                {
                    Id = customization.Id,
                    InitiatorUserId = 0,
                    IsExecuteByAdmin = true,
                    Version = updatePiggyBankCustomizationDTO.EditedFields.Contains(nameof(updatePiggyBankCustomizationDTO.Version)) ? updatePiggyBankCustomizationDTO.Version.Value : customization.Version,
                    PiggyBankTypeId = updatePiggyBankCustomizationDTO.EditedFields.Contains(nameof(updatePiggyBankCustomizationDTO.PiggyBankTypeId)) ? updatePiggyBankCustomizationDTO.PiggyBankTypeId.Value : customization.PiggyBankTypeId,
                    PhotoPath = updatePiggyBankCustomizationDTO.EditedFields.Contains(nameof(updatePiggyBankCustomizationDTO.PhotoPath)) ? updatePiggyBankCustomizationDTO.PhotoPath : customization.PhotoPath,
                    ExternalId = updatePiggyBankCustomizationDTO.EditedFields.Contains(nameof(updatePiggyBankCustomizationDTO.ExternalId)) ? updatePiggyBankCustomizationDTO.ExternalId : customization.ExternalId
                }, cancellationToken)));
            }
            else
            {
                await _integrationService.UpdatePiggyBankCustomizationOnServerAsync(piggyBankCustomizationId, updatePiggyBankCustomizationDTO, cancellationToken);
            }
        }


        public async Task<int> GetCurrentPiggyBanksCountForUserAsync(int userId, CancellationToken cancellationToken)
        {
            return (await ExecuteWithMediator(async mediator => await _mediator.Send(new GetUserPiggyBanksCountQuery { UserId = userId, InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken)));
        }

        public async Task<PiggyBankTypeDTO> GetPiggyBankTypeDetailsAsync(int id, CancellationToken cancellationToken)
        {
            return (await ExecuteWithMediator(async mediator => await _mediator.Send(new GetPiggyBankTypeByIdQuery { Id = id, InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken)));
        }

        public async Task<UserPiggyBankDTO> GetUserPiggyBankLinkByUserIdAndPiggyBankId(int userId, int piggyBankId, CancellationToken cancellationToken)
        {

            var link = (await ExecuteWithMediator(async mediator => await _mediator.Send(new GetUserPiggyBankLinkByUserIdAndPiggyBankIdQuery { PiggyBankId = piggyBankId, UserId = userId, InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken)));
            return link;
        }

        public async Task UnlinkAllUsersExceptOwnerAsync(int piggyBankId, CancellationToken cancellationToken)
        {
            var piggybank = await this.GetPiggyBankDetailsAsync(piggyBankId, cancellationToken, false);
            var links = (await ExecuteWithMediator(async mediator => await _mediator.Send(new GetUserPiggyBankLinksByPiggyBankIdQuery { PiggyBankId = piggyBankId, InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken)));
            foreach (var link in links)
            {
                if (link.UserId != piggybank.OwnerId)
                    await ExecuteWithMediator(async mediator => await _mediator.Send(new DeleteUserPiggyBankCommand { Id = link.Id, InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken));
            }
        }

        private async Task<T> ExecuteWithMediator<T>(Func<IMediator, Task<T>> func)
        {

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                _mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                return await func(_mediator);
            }

        }

        public async Task<PiggyBankCustomizationDTO> GetPiggyBankCustomizationByPiggyBankIdAsync(int piggyBankId, CancellationToken cancellationToken, bool remote)
        {
            PiggyBankCustomizationDTO result = null;
            if (!remote)
                result = (await ExecuteWithMediator(async mediator => await _mediator.Send(new GetPiggyBankCustomizationByPiggyBankIdQuery { PiggyBankId = piggyBankId, InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken)));
            else
            {
                result = await _integrationService.GetPiggyBankCustomizationByPiggyBankIdFromServerAsync(piggyBankId, cancellationToken);
                result.PhotoPath = _integrationService.GetPiggyBankCustomizationPhotoURL(result.Id);
                result.ExternalId = result.Id;
            }
            return result;
        }

        public async Task<byte[]> GetPiggyBankCustomizationPhotoAsync(PiggyBankCustomizationDTO piggyBankCustomization, CancellationToken cancellationToken, bool remote)
        {
            if (remote)
                return await _integrationService.GetPiggyBankCustomizationPhotoFromServerAsync(piggyBankCustomization.ExternalId.Value, cancellationToken);
            else
                return await _fileService.ReadFileAsync(piggyBankCustomization.PhotoPath);
        }

        public async Task<PiggyBankCustomizationDTO> GetPiggyBankCustomizationByIdAsync(int piggyBankCustomizationId, CancellationToken cancellationToken, bool remote)
        {
            PiggyBankCustomizationDTO result = null;
            if (!remote)
                result = (await ExecuteWithMediator(async mediator => await _mediator.Send(new GetPiggyBankCustomizationByIdQuery { Id = piggyBankCustomizationId, InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken)));
            else
            {
                result = await _integrationService.GetPiggyBankCustomizationFromServerAsync(piggyBankCustomizationId, cancellationToken);
                result.ExternalId = result.Id;
            }
            return result;
        }

        public async Task<List<UserPiggyBankDTO>> GetUserPiggyBankLinksByUserIdAsync(int userId, CancellationToken cancellationToken, bool remote)
        {
            List<UserPiggyBankDTO> result;
            if (!remote)
                result = (await ExecuteWithMediator(async mediator => await _mediator.Send(new GetUserPiggyBankLinksByUserIdQuery { UserId = userId, InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken)));
            else
            {
                result = await _integrationService.GetUserPiggyBankLinksByUserIdFromServerAsync(userId, cancellationToken);
                foreach (var e in result)
                    e.ExternalId = e.Id;
            }
            return result;
        }

        public async Task<UserPiggyBankDTO> GetUserPiggyBankLinkAsync(int userPiggyBankId, CancellationToken cancellationToken, bool remote)
        {
            UserPiggyBankDTO result;
            if (!remote)
                result = (await ExecuteWithMediator(async mediator => await _mediator.Send(new GetUserPiggyBankLinkQuery { Id = userPiggyBankId, InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken)));
            else
            {
                result = await _integrationService.GetUserPiggyBankLinkFromServerAsync(userPiggyBankId, cancellationToken);
                result.ExternalId = result.Id;
            }
            return result;
        }

        public async Task<PiggyBankDTO> RunTwoWayPiggyBankIntegration(PiggyBankDTO? localPiggyBank, PiggyBankDTO? externalPiggyBank, CancellationToken cancellationToken)
        {
            return await _integrationService.RunTwoWayPiggyBankIntegration(localPiggyBank, externalPiggyBank, cancellationToken);
        }

        public async Task<UserPiggyBankDTO> RunTwoWayUserPiggyBankIntegration(UserPiggyBankDTO? localUserPiggyBank, UserPiggyBankDTO? externalUserPiggyBank, CancellationToken cancellationToken)
        {
            return await _integrationService.RunTwoWayUserPiggyBankIntegration(localUserPiggyBank, externalUserPiggyBank, cancellationToken);
        }

        public async Task<PiggyBankCustomizationDTO> RunTwoWayPiggyBankCustomizationIntegration(PiggyBankCustomizationDTO? localPiggyBankCustomization, PiggyBankCustomizationDTO? externalPiggyBankCustomization, CancellationToken cancellationToken)
        {
            return await _integrationService.RunTwoWayPiggyBankCustomizationIntegration(localPiggyBankCustomization, externalPiggyBankCustomization, cancellationToken);
        }

        public async Task<PiggyBankDTO> GetPiggyBankDetailsByExternalId(int piggyBankId, CancellationToken cancellationToken)
        {
            return await ExecuteWithMediator(async mediator => await mediator.Send(new GetPiggyBankByExternalIdQuery { ExternalId = piggyBankId, InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken));
        }

        public async Task<List<UserPiggyBankDTO>> GetUserPiggyBankLinksByPiggyBankIdAsync(int piggyBankId, CancellationToken cancellationToken, bool remote)
        {
            if (!remote)
                return await ExecuteWithMediator(async mediator => await mediator.Send(new GetUserPiggyBankLinksByPiggyBankIdQuery { PiggyBankId = piggyBankId, InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken));
            else
            {
               var list =  await _integrationService.GetUserPiggyBankLinksByPiggyBankIdFromServerAsync(piggyBankId, cancellationToken);
                foreach (var e in list)
                {
                    e.ExternalId = e.Id;
                }
                return list;
            }

        }

        public async Task DeleteUserPiggyBankAsync(int userPiggyBankId, CancellationToken cancellationToken, bool remote)
        {
            if (!remote)
                await ExecuteWithMediator(async mediator => await mediator.Send(new DeleteUserPiggyBankCommand { Id = userPiggyBankId, InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken));
            else
            {
                await _integrationService.DeleteUserPiggyBankLinkFromServerAsync(userPiggyBankId, cancellationToken);
            }
        }

        public async Task<string> UploadPiggyBankCustomizationPhotoAsync(int piggyBankCustomizationId, byte[] photo, CancellationToken cancellationToken, bool remote)
        {
            var pbc = await GetPiggyBankCustomizationByIdAsync(piggyBankCustomizationId, cancellationToken, remote);
            if (!remote)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    _mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                    _fileService = scope.ServiceProvider.GetRequiredService<IFileService>();
                    string jpegPath = Path.Combine(_fileService.GetDefaultPath(), $"pbc_{pbc.Id}_{Guid.NewGuid().ToString()}.jpeg");

                    if (_fileService.Exist(jpegPath))
                         await _fileService.RemoveFileAsync(jpegPath);

                    await _fileService.CreateFileAsync(jpegPath, photo, true);
                    var updateDto = _mapper.Map<UpdatePiggyBankCustomizationDTO>(pbc);
                    updateDto.PhotoPath = jpegPath;
                    await UpdatePiggyBankCustomizationAsync(piggyBankCustomizationId, updateDto, cancellationToken, false);

                    if (_fileService.Exist(pbc.PhotoPath))
                        await _fileService.RemoveFileAsync(pbc.PhotoPath);
                    return jpegPath;
                }
            }
            else
            {
               await _integrationService.UploadPiggyBankCustomizationPhotoOnServerAsync(piggyBankCustomizationId, photo, cancellationToken);
               return _integrationService.GetPiggyBankCustomizationPhotoURL(piggyBankCustomizationId);
            }
        }
    }
}
