using AutoMapper;
using Kopilych.Application.Common.Exceptions;
using Kopilych.Application.CQRS.Commands.PiggyBank.CreatePiggyBank;
using Kopilych.Application.CQRS.Commands.PiggyBank.DeletePiggyBank;
using Kopilych.Application.CQRS.Commands.PiggyBank.UpdatePiggyBank;
using Kopilych.Application.CQRS.Commands.PiggyBankCustomization.CreatePiggyBankCustomization;
using Kopilych.Application.CQRS.Commands.PiggyBankCustomization.UpdatePiggyBankCustomization;
using Kopilych.Application.CQRS.Commands.Transaction.CreateTransaction;
using Kopilych.Application.CQRS.Commands.Transaction.DeleteTransaction;
using Kopilych.Application.CQRS.Commands.Transaction.UpdateTransaction;
using Kopilych.Application.CQRS.Commands.User.UpdateUser;
using Kopilych.Application.CQRS.Commands.UserPiggyBank.CreateUserPiggyBank;
using Kopilych.Application.CQRS.Commands.UserPiggyBank.UpdateUserPiggyBank;
using Kopilych.Application.CQRS.Commands.UserSession.CreateOrUpdateUserSession;
using Kopilych.Application.CQRS.Commands.UserSession.DeleteUserSession;
using Kopilych.Application.CQRS.Queries.PiggyBank.GetPiggyBank;
using Kopilych.Application.CQRS.Queries.PiggyBank.GetPiggyBankByExternalId;
using Kopilych.Application.CQRS.Queries.PiggyBankCustomization.GetPiggyBankCustomizationById;
using Kopilych.Application.CQRS.Queries.Transaction.GetTransactionById;
using Kopilych.Application.CQRS.Queries.User.GetCurrentUserDetails;
using Kopilych.Application.CQRS.Queries.User.GetUserDetailsByExternalId;
using Kopilych.Application.CQRS.Queries.User.GetUserDetailsById;
using Kopilych.Application.CQRS.Queries.UserPiggyBank.GetUserPiggyBankLink;
using Kopilych.Application.CQRS.Queries.UserSession.GetSessionByUserId;
using Kopilych.Application.CQRS.Queries.UserSession.GetSessionByUserId;
using Kopilych.Application.Interfaces;
using Kopilych.Domain;
using Kopilych.Shared;
using Kopilych.Shared.DTO;
using Kopilych.Shared.View_Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml.Linq;
using static System.Collections.Specialized.BitVector32;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Kopilych.Application.Services
{
    internal class IntegrationService : IIntegrationService
    {
        private ApiEndpoints _apiEndpoints;
        private IMediator _mediator;
        private IApiCommunicatorService _apiCommunicatorService;
        private IMapper _mapper;
        private SessionDTO _session;
        private IFileService _fileService;
        private IServiceScopeFactory _serviceScopeFactory;
        private int _ctsTiming;

        public event Action AuthCompleted;

        public IntegrationService(ApiEndpoints apiEndpoints, IMapper mapper,IServiceScopeFactory serviceScopeFactory, int ctsTiming = 10000) 
        { 
            _serviceScopeFactory = serviceScopeFactory; _apiEndpoints = apiEndpoints;  _mapper = mapper; _ctsTiming = ctsTiming;
        }
        public string LoginPageAddress { get => _apiEndpoints.Login; }

        public bool IsSessionExists { get => !(_session == null); }

        public bool IsConfigured { get; private set; }

        public async Task Configure()
        {
            try
            {
                await LoadSessionFromDatabase();
            } catch (NotFoundException ex)
            {

            }
            IsConfigured = true;
        }

        public async Task<int> CreatePiggyBankCustomizationOnServerAsync(CreatePiggyBankCustomizationDTO createPiggyBankCustomizationDTO, CancellationToken cancellationToken)
        {
            return await RunScopedApiCommunication(async apiCommunicator => await _apiCommunicatorService.PostAsync<CreatePiggyBankCustomizationDTO, int>(_apiEndpoints.CreatePiggyBankCustomization, createPiggyBankCustomizationDTO, cancellationToken));
        }

        public async Task<int> CreatePiggyBankOnServerAsync(CreatePiggyBankDTO createPiggyBankDTO, CancellationToken cancellationToken)
        {
            return await RunScopedApiCommunication(async apiCommunicator => await _apiCommunicatorService.PostAsync<CreatePiggyBankDTO, int>(_apiEndpoints.CreatePiggyBank, createPiggyBankDTO, cancellationToken ));
        }

        public async Task<int> CreateTransactionOnServerAsync(CreateTransactionDTO createTransactionDTO, CancellationToken cancellationToken, bool updatePiggyBankBalance = true)
        {
            return await RunScopedApiCommunication(async apiCommunicator => await _apiCommunicatorService.PostAsync<CreateTransactionDTO, int>(_apiEndpoints.CreateTransaction.ReplacePlaceholders(new Dictionary<string, string> { { "bool", updatePiggyBankBalance.ToString() } }), createTransactionDTO, cancellationToken));
        }

        public async Task<int> CreateUserFriendshipOnServerAsync(CreateFriendshipDTO createFriendshipDTO, CancellationToken cancellationToken)
        {
            return await RunScopedApiCommunication(async apiCommunicator => await _apiCommunicatorService.PostAsync<CreateFriendshipDTO, int>(_apiEndpoints.CreateUserFriendship, createFriendshipDTO, cancellationToken));
        }

        public Task<int> CreateUserOnServerAsync(CreateUserDTO createUserDTO, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<int> CreateUserPiggyBankLinkOnServerAsync(CreateUserPiggyBankDTO createUserPiggyBankDTO, CancellationToken cancellationToken)
        {
            return await RunScopedApiCommunication(async apiCommunicator => await _apiCommunicatorService.PostAsync<CreateUserPiggyBankDTO, int>(_apiEndpoints.CreateUserPiggyBank, createUserPiggyBankDTO, cancellationToken));
        }

        public async Task DeletePiggyBankFromServerAsync(int piggyBankId, CancellationToken cancellationToken)
        {
            try
            {
                await RunScopedApiCommunication(async apiCommunicator => { await _apiCommunicatorService.DeleteAsync(_apiEndpoints.DeletePiggyBank.ReplacePlaceholders(new Dictionary<string, string> { { "id", piggyBankId.ToString() } }), cancellationToken); return Unit.Value; });
            } catch (HttpRequestException ex)
            {
                if (ex.StatusCode != System.Net.HttpStatusCode.NotFound)
                    throw ex;
            }
        }

        public async Task DeleteTransactionFromServerAsync(int transactionId, CancellationToken cancellationToken, bool updatePiggyBankBalance = true)
        {
            try
            {
                await RunScopedApiCommunication(async apiCommunicator => { await _apiCommunicatorService.DeleteAsync(_apiEndpoints.DeleteTransaction.ReplacePlaceholders(new Dictionary<string, string> { { "id", transactionId.ToString() }, {"bool", updatePiggyBankBalance.ToString() } }), cancellationToken); return Unit.Value; });
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode != System.Net.HttpStatusCode.NotFound)
                    throw ex;
            }
        }

        public async Task DeleteUserFriendshipFromServerAsync(int userFriendshipId, CancellationToken cancellationToken)
        {
            try
            {
                await RunScopedApiCommunication(async apiCommunicator => { await _apiCommunicatorService.DeleteAsync(_apiEndpoints.DeleteUserFriendship.ReplacePlaceholders(new Dictionary<string, string> { { "id", userFriendshipId.ToString() } }), cancellationToken); return Unit.Value; });
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode != System.Net.HttpStatusCode.NotFound)
                    throw ex;
            }
        }

        public Task DeleteUserFromServerAsync(int userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task DeleteUserPiggyBankCustomizationFromServerAsync(int piggyBankCustomizationId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteUserPiggyBankLinkFromServerAsync(int userPiggyBankId, CancellationToken cancellationToken)
        {
            try
            {
                await RunScopedApiCommunication(async apiCommunicator => { await _apiCommunicatorService.DeleteAsync(_apiEndpoints.DeleteUserPiggyBank.ReplacePlaceholders(new Dictionary<string, string> { { "id", userPiggyBankId.ToString() } }), cancellationToken); return Unit.Value; });
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode != System.Net.HttpStatusCode.NotFound)
                    throw ex;
            }
        }

        public Task<PaymentTypeDTO> GetPaymentTypeFromServerAsync(int paymentTypeId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<PiggyBankCustomizationDTO> GetPiggyBankCustomizationFromServerAsync(int piggyBankCustomizationId, CancellationToken cancellationToken)
        {
            try
            { 
            return await RunScopedApiCommunication(async apiCommunicator => await _apiCommunicatorService.GetAsync<PiggyBankCustomizationDTO>(_apiEndpoints.GetPiggyBankCustomization.ReplacePlaceholders(new Dictionary<string, string> { { "id", piggyBankCustomizationId.ToString() } }), cancellationToken));
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new NotFoundException(typeof(Domain.PiggyBankCustomization).ToString(), piggyBankCustomizationId);
                else
                    throw ex;
            }
        }

        public async Task<PiggyBankDTO> GetPiggyBankFromServerAsync(int piggyBankId, CancellationToken cancellationToken)
        {
            try
            { 
            return await RunScopedApiCommunication(async apiCommunicator => await _apiCommunicatorService.GetAsync<PiggyBankDTO>(_apiEndpoints.GetPiggyBank.ReplacePlaceholders(new Dictionary<string, string> { { "id", piggyBankId.ToString() } }), cancellationToken));
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new NotFoundException(typeof(Domain.PiggyBank).ToString(), piggyBankId);
                else
                    throw ex;
            }
        }

        public Task<PiggyBankTypeDTO> GetPiggyBankTypeFromServerAsync(int piggyBankTypeId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<TransactionDTO> GetTransactionFromServerAsync(int transactionId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<TransactionTypeDTO> GetTransactionTypeFromServerAsync(int transactionTypeId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<UserFriendshipDetailsDTO> GetUserFriendshipFromServerAsync(int userFriendshipId, CancellationToken cancellationToken)
        {
            try
            {
                return await RunScopedApiCommunication(async apiCommunicator => await _apiCommunicatorService.GetAsync<UserFriendshipDetailsDTO>(_apiEndpoints.GetUserFriendship.ReplacePlaceholders(new Dictionary<string, string> { { "id", userFriendshipId.ToString() } }), cancellationToken));
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new NotFoundException(typeof(Domain.UserFriendship).ToString(), userFriendshipId);
                else
                    throw ex;
            }
        }

        public async Task<UserDetailsDTO> GetUserFromServerAsync(int userId, CancellationToken cancellationToken)
        {
            try
            {
                return await RunScopedApiCommunication(async apiCommunicator => await _apiCommunicatorService.GetAsync<UserDetailsDTO>(_apiEndpoints.GetUser.ReplacePlaceholders(new Dictionary<string, string> { { "id", userId.ToString() } }), cancellationToken));
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new NotFoundException(typeof(Domain.User).ToString(), userId);
                else
                    throw ex;
            }
        }

        public async Task<bool> CheckIfServerOnline (CancellationToken cancellationToken)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    _apiCommunicatorService = scope.ServiceProvider.GetRequiredService<IApiCommunicatorService>();
                    await _apiCommunicatorService.GetAsync<object>(_apiEndpoints.HealthcheckAddress, cancellationToken);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<UserPiggyBankDTO> GetUserPiggyBankLinkFromServerAsync(int userPiggyBankLinkId, CancellationToken cancellationToken)
        {
            try
            { 
            return await RunScopedApiCommunication(async apiCommunicator => await _apiCommunicatorService.GetAsync<UserPiggyBankDTO>(_apiEndpoints.GetUserPiggyBank.ReplacePlaceholders(new Dictionary<string, string> { { "id", userPiggyBankLinkId.ToString() } }), cancellationToken));
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new NotFoundException(typeof(Domain.UserPiggyBank).ToString(), userPiggyBankLinkId);
                else
                    throw ex;
            }
        }

        public async  Task UpdatePiggyBankCustomizationOnServerAsync(int piggyBankCustomizationId, UpdatePiggyBankCustomizationDTO updatePiggyBankCustomizationDTO, CancellationToken cancellationToken)
        {
            try
            {
                await RunScopedApiCommunication(async apiCommunicator => await _apiCommunicatorService.PatchAsync<UpdatePiggyBankCustomizationDTO, object>(_apiEndpoints.UpdatePiggyBankCustomization.ReplacePlaceholders(new Dictionary<string, string> { { "id", piggyBankCustomizationId.ToString() } }), updatePiggyBankCustomizationDTO, cancellationToken));
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new NotFoundException(typeof(Domain.PiggyBankCustomization).ToString(), piggyBankCustomizationId);
                else
                    throw ex;
            }
        }

        public async Task UpdatePiggyBankOnServerAsync(int piggyBankId, UpdatePiggyBankDTO updatePiggyBankDTO, CancellationToken cancellationToken)
        {
            try
            { 
            await RunScopedApiCommunication(async apiCommunicator => await _apiCommunicatorService.PatchAsync<UpdatePiggyBankDTO, object>(_apiEndpoints.UpdatePiggyBank.ReplacePlaceholders(new Dictionary<string, string> { { "id", piggyBankId.ToString() } }), updatePiggyBankDTO, cancellationToken));
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new NotFoundException(typeof(Domain.PiggyBank).ToString(), piggyBankId);
                else
                    throw ex;
            }
        }

        public Task UpdateTransactionOnServerAsync(int transactionId, UpdateTransactionDTO updateTransactionDTO, CancellationToken cancellationToken, bool updatePiggyBankBalance = true)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateUserFriendshipOnServerAsync(int userFriendshipId, UpdateFriendshipDTO updateFriendshipDTO, CancellationToken cancellationToken)
        {
            try
            {
                await RunScopedApiCommunication(async apiCommunicator => await _apiCommunicatorService.PatchAsync<UpdateFriendshipDTO, object>(_apiEndpoints.UpdateUserFriendship.ReplacePlaceholders(new Dictionary<string, string> { { "id", userFriendshipId.ToString() } }), updateFriendshipDTO, cancellationToken));
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new NotFoundException(typeof(Domain.UserFriendship).ToString(), userFriendshipId);
                else
                    throw ex;
            }
        }

        public async Task UpdateUserOnServerAsync(int userId, UpdateUserDTO updateUserDTO, CancellationToken cancellationToken)
        {
            try
            {
                await RunScopedApiCommunication(async apiCommunicator => await _apiCommunicatorService.PatchAsync<UpdateUserDTO, object>(_apiEndpoints.UpdateUser.ReplacePlaceholders(new Dictionary<string, string> { { "id", userId.ToString() } }), updateUserDTO, cancellationToken));
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new NotFoundException(typeof(Domain.User).ToString(), userId);
                else
                    throw ex;
            }
        }

        public async Task UpdateUserPiggyBankLinkOnServerAsync(int userPiggyBankId, UpdateUserPiggyBankDTO updateUserPiggyBankDTO, CancellationToken cancellationToken)
        {
            try
            {
                await RunScopedApiCommunication(async apiCommunicator => await _apiCommunicatorService.PatchAsync<UpdateUserPiggyBankDTO, object>(_apiEndpoints.UpdateUserPiggyBank.ReplacePlaceholders(new Dictionary<string, string> { { "id", userPiggyBankId.ToString() } }), updateUserPiggyBankDTO, cancellationToken));
            } catch (HttpRequestException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new NotFoundException(typeof(Domain.UserPiggyBank).ToString(), userPiggyBankId);
                else
                    throw ex;
            }
}

        public async Task<SessionDTO> RefreshSession(CancellationToken cancellationToken)
        {
            var session = await _apiCommunicatorService.PostAsync<RefreshSessionDTO, SessionDTO>(_apiEndpoints.RefreshSession, new RefreshSessionDTO { RefreshToken = _session.RefreshToken });
            _session = session;
            var user = await ExecuteWithMediator(async mediator => _mediator.Send(new  GetCurrentUserDetailsQuery { }, cancellationToken));
            await ExecuteWithMediator(async mediator => _mediator.Send(new CreateOrUpdateUserSessionCommand { AccessToken = _session.AccessToken, UserId = user.Id, RefreshToken = _session.RefreshToken }, cancellationToken));
            return new SessionDTO { AccessToken = session.AccessToken, RefreshToken = session.RefreshToken };
        }

        public async Task<bool> CheckIfAccessTokenValid(CancellationToken cancellationToken)
        {
            _apiCommunicatorService.SetAccessToken(_session.AccessToken);
            try
            {
                await RunScopedApiCommunication(async apiCommunicator => await _apiCommunicatorService.GetAsync<UserDetailsDTO>(_apiEndpoints.GetCurrentUser, cancellationToken));
                return true;
            }
            catch (HttpRequestException ex)
            {
                var httpEx = ex;
                if (httpEx != null && httpEx.StatusCode == (System.Net.HttpStatusCode)401)
                    return false;
                else
                    throw ex;

            }
            return true;
        }

        public async Task<UserDetailsDTO> RunTwoWayUserIntegration(UserDetailsDTO localUser, UserDetailsDTO externalUser, CancellationToken cancellationToken)
        {
            var result = (UserDetailsDTO)localUser.Clone();
            if (localUser.ExternalId == externalUser.Id)
            {
                if (localUser.Version > externalUser.Version)
                {
                    byte[] photo = null;

                    if (localUser.PhotoPath != null)
                    {
                        if (CheckIfLocalFileExists(localUser.PhotoPath))
                        {
                            photo = await GetLocalPhotoAsync(localUser.PhotoPath);
                            if (photo != null)
                            {
                                await UploadUserPhotoOnServerAsync(externalUser.Id, photo, cancellationToken);
                                await ExecuteWithMediator(async mediator => await _mediator.Send(new UpdateUserCommand { Id = localUser.Id, ExternalId = localUser.ExternalId, InitiatorUserId = 0, IsExecuteByAdmin = true, PhotoPath = localUser.PhotoPath, Username = localUser.Username, Version = localUser.Version, PhotoIntegrated = true }, cancellationToken));
                                result.PhotoIntegrated = true;
                            }
                        }
                    }
                    await UpdateUserOnServerAsync(externalUser.Id, _mapper.Map<UpdateUserDTO>(localUser), cancellationToken);
                    result = (UserDetailsDTO)localUser.Clone();
                }
                else if (localUser.Version <= externalUser.Version)
                {
                    byte[] photo = null;
                    string photopath = null;
                    result = (UserDetailsDTO)externalUser.Clone();
                    try
                    {
                        photo = await GetUserPhotoFromServerAsync(externalUser.Id, cancellationToken);
                    }
                    catch (NotFoundException)
                    {

                    }

                    if (photo != null)
                    {
                        photopath = Path.Combine(GetDefaultLocalPath(), $"user_{localUser.Id}_{Guid.NewGuid().ToString()}.jpeg");
                        await UpdateLocalPhotoAsync(photo, photopath);
                        result.PhotoIntegrated = true;
                        result.PhotoPath = photopath;
                    }


                    await ExecuteWithMediator(async mediator => await _mediator.Send(new UpdateUserCommand { ExternalId = externalUser.Id, Version = externalUser.Version, Id = localUser.Id, PhotoPath = photopath, Username = externalUser.Username, InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken));

                    var extId = result.Id;
                    result.Id = localUser.Id;
                    result.ExternalId = extId;
                }
                else if (localUser.PhotoIntegrated = false && localUser.PhotoPath != null)
                {
                    if (CheckIfLocalFileExists(localUser.PhotoPath))
                    {
                        byte[] photo = null;
                        photo = await GetLocalPhotoAsync(localUser.PhotoPath);
                        if (photo != null)
                        {
                            await UploadUserPhotoOnServerAsync(externalUser.Id, photo, cancellationToken);
                            await ExecuteWithMediator(async mediator => await _mediator.Send(new UpdateUserCommand { ExternalId = externalUser.Id, Version = localUser.Version, Id = localUser.Id, PhotoPath = localUser.PhotoPath, Username = localUser.Username, InitiatorUserId = 0, IsExecuteByAdmin = true, PhotoIntegrated = true }, cancellationToken));
                        }
                    }
                    result = (UserDetailsDTO)localUser.Clone();
                    result.PhotoIntegrated = true;
                }
            }
            return result;

        }


        private async Task<T> ExecuteWithMediator<T>(Func<IMediator, Task<T>> func)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                _mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                return await func(_mediator);
            }
        }

        private async Task LoadSessionFromDatabase()
        {
            var currentUser = await ExecuteWithMediator(async mediator => await _mediator.Send(new GetCurrentUserDetailsQuery { }, CancellationToken.None));
            var session = _mapper.Map<SessionDTO>(await ExecuteWithMediator(async mediator => await _mediator.Send(new GetSessionByUserIdQuery { UserId = currentUser.Id }, CancellationToken.None)));
            _session = session;
        }

        private async Task<T> RunScopedApiCommunication<T>(Func<IApiCommunicatorService, Task<T>> func)
        {
            if (_session == null)
                await LoadSessionFromDatabase();
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                _apiCommunicatorService = scope.ServiceProvider.GetRequiredService<IApiCommunicatorService>();
                try
                {
                    return await func(_apiCommunicatorService);
                }
                catch (HttpRequestException ex) 
                {
                    if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        await LoadSessionFromDatabase();
                        try
                        {
                            await RefreshSession(CancellationToken.None);
                        }
                        catch (HttpRequestException ex1)
                        {
                            if (ex1.StatusCode != System.Net.HttpStatusCode.Unauthorized && ex1.StatusCode != System.Net.HttpStatusCode.BadRequest)
                                throw ex1;

                        }
                        _apiCommunicatorService.SetAccessToken(_session.AccessToken);
                    }
                    else
                        throw ex;
                }
                try
                {
                    return await func(_apiCommunicatorService);
                }
                catch (HttpRequestException ex)
                {
                    if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        var currentUser = await ExecuteWithMediator(async mediator => await _mediator.Send(new GetCurrentUserDetailsQuery { }, CancellationToken.None));
                        await ExecuteWithMediator(async mediator => await _mediator.Send(new DeleteUserSessionCommand { UserId = currentUser.Id }, CancellationToken.None));
                        LoadSessionFromDTO(null);
                    }
                    else
                        throw ex;
                }
                throw new NotAuthorizedException();
            }
        }



        public async Task<UserDetailsDTO> GetCurrentUserFromServerAsync(CancellationToken cancellationToken)
        {
            return await RunScopedApiCommunication(async apiCommunicator => await _apiCommunicatorService.GetAsync<UserDetailsDTO>(_apiEndpoints.GetCurrentUser));
        }

        private void LoadSessionFromDTO(SessionDTO session)
        {
            _session = session;
        }

        public void FinishAuth()
        {
            AuthCompleted?.Invoke();
        }

        public async Task LogoutAsync(bool full, LogoutDTO logoutDTO, CancellationToken cancellationToken)
        {
            if (full)
                await RunScopedApiCommunication(async apiCommunicator => await _apiCommunicatorService.PostAsync<object?,object>(_apiEndpoints.Logout.ReplacePlaceholders(new Dictionary<string, string> { { "bool", true.ToString() } }), null));
            else
                await RunScopedApiCommunication(async apiCommunicator => await _apiCommunicatorService.PostAsync<LogoutDTO, object>(_apiEndpoints.Logout.ReplacePlaceholders(new Dictionary<string, string> { { "bool", false.ToString() } }), _mapper.Map<LogoutDTO>(_session)));
            var currentUser = await ExecuteWithMediator(async mediator => await _mediator.Send(new GetCurrentUserDetailsQuery { }, CancellationToken.None));
            await ExecuteWithMediator(async mediator => await _mediator.Send(new DeleteUserSessionCommand { UserId = currentUser.Id }, CancellationToken.None));
            LoadSessionFromDTO(null);
        }

        public async Task<List<UserPiggyBankDTO>> GetUserPiggyBankLinksByUserIdFromServerAsync(int userId, CancellationToken cancellationToken)
        {
            try
            {
                return await RunScopedApiCommunication(async apiCommunicator => await _apiCommunicatorService.GetAsync<List<UserPiggyBankDTO>>(_apiEndpoints.GetUserPiggyBankLinksByUserId.ReplacePlaceholders(new Dictionary<string, string> { { "id", userId.ToString() } })));
            } catch (HttpRequestException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new NotFoundException(typeof(Domain.UserPiggyBank).ToString(), userId);
                else
                    throw ex;
            }
}

        public async Task<PiggyBankCustomizationDTO> GetPiggyBankCustomizationByPiggyBankIdFromServerAsync(int piggyBankId, CancellationToken cancellationToken)
        {
            try
            {
                return await RunScopedApiCommunication(async apiCommunicator => await _apiCommunicatorService.GetAsync<PiggyBankCustomizationDTO>(_apiEndpoints.GetPiggyBankCustomizationByPiggyBankId.ReplacePlaceholders(new Dictionary<string, string> { { "id", piggyBankId.ToString() } })));
            } catch (HttpRequestException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new NotFoundException(typeof(Domain.PiggyBankCustomization).ToString(), piggyBankId);
                else
                    throw ex;
            }

        }

        public async Task<PiggyBankDTO?> RunTwoWayPiggyBankIntegration(PiggyBankDTO? localPiggyBank, PiggyBankDTO? externalPiggyBank, CancellationToken cancellationToken)
        {
            var localuser = await ExecuteWithMediator(async mediator => await _mediator.Send(new GetCurrentUserDetailsQuery { }, CancellationToken.None));
            PiggyBankDTO result = null;
            if (localPiggyBank != null && localPiggyBank.IsDeleted == true)
            {
                if (localPiggyBank.ExternalId.HasValue)
                    await DeletePiggyBankFromServerAsync(localPiggyBank.ExternalId.Value, cancellationToken);
                await ExecuteWithMediator(async mediator => await _mediator.Send(new DeletePiggyBankCommand { Id = localPiggyBank.Id, InitiatorUserId = 0, IsExecuteByAdmin = true}, cancellationToken));
                result = null;
            } 
            else if (localPiggyBank == null && externalPiggyBank != null)
            {
                if (localuser.ExternalId != externalPiggyBank.OwnerId)
                    throw new ArgumentException("Piggy bank owner id and user external id mismatch.");
                var createPiggyBankDto = _mapper.Map<CreatePiggyBankDTO>(externalPiggyBank);
                createPiggyBankDto.ExternalId = externalPiggyBank.Id;
                createPiggyBankDto.OwnerId = localuser.Id;
                var piggyBankId = (await ExecuteWithMediator(async mediator => await _mediator.Send(new CreatePiggyBankCommand { Balance = createPiggyBankDto.Balance, Description = createPiggyBankDto.Description, Goal = createPiggyBankDto.Goal, GoalDate = createPiggyBankDto.GoalDate, InitiatorUserId = 0, IsExecuteByAdmin = true, Name = createPiggyBankDto.Name, OwnerId = createPiggyBankDto.OwnerId, Shared = createPiggyBankDto.Shared, Version = createPiggyBankDto.Version, ExternalId = createPiggyBankDto.ExternalId }, cancellationToken)));
                result = (await ExecuteWithMediator(async mediator => await _mediator.Send(new GetPiggyBankQuery { Id = piggyBankId, InitiatorUserId = 0, IsExecuteByAdmin = true}, cancellationToken)));
            } 
            else if (localPiggyBank != null && externalPiggyBank == null)
            {
                var createPiggyBankDto = _mapper.Map<CreatePiggyBankDTO>(localPiggyBank);
                createPiggyBankDto.OwnerId = localuser.ExternalId.Value;
                var piggyBankId = await CreatePiggyBankOnServerAsync(createPiggyBankDto, cancellationToken);
                //   var piggyBank = await GetPiggyBankFromServerAsync(piggyBankId, cancellationToken);
                await ExecuteWithMediator(async mediator => await _mediator.Send(new UpdatePiggyBankCommand { ExternalId = piggyBankId, Version = localPiggyBank.Version, Id = localPiggyBank.Id, InitiatorUserId = 0, IsExecuteByAdmin = true, Balance = localPiggyBank.Balance, Description = localPiggyBank.Description, Goal = localPiggyBank.Goal, GoalDate = localPiggyBank.GoalDate, IsDeleted = localPiggyBank.IsDeleted, Name = localPiggyBank.Name, Shared = localPiggyBank.Shared }, cancellationToken));
                result = (PiggyBankDTO)localPiggyBank.Clone();
                result.ExternalId = piggyBankId;
            }
            else if (localPiggyBank.ExternalId == externalPiggyBank.Id)
            {
                if (localPiggyBank.Version > externalPiggyBank.Version)
                {
                    await UpdatePiggyBankOnServerAsync(externalPiggyBank.Id, _mapper.Map<UpdatePiggyBankDTO>(localPiggyBank), cancellationToken);
                    result = (PiggyBankDTO)localPiggyBank.Clone();
                }
                else if (localPiggyBank.Version < externalPiggyBank.Version)
                {
                    result = (PiggyBankDTO)externalPiggyBank.Clone();
                    var extId = result.Id;
                    result.Id = localPiggyBank.Id;
                    result.ExternalId = extId;
                    result.OwnerId = localPiggyBank.OwnerId;
                    await ExecuteWithMediator(async mediator => await _mediator.Send(new UpdatePiggyBankCommand { ExternalId =  externalPiggyBank.ExternalId, Version = externalPiggyBank.Version, Id = localPiggyBank.Id, InitiatorUserId = 0, IsExecuteByAdmin = true, Balance = externalPiggyBank.Balance, Description = externalPiggyBank.Description, Goal = externalPiggyBank.Goal, GoalDate = externalPiggyBank.GoalDate, IsDeleted = externalPiggyBank.IsDeleted, Name = externalPiggyBank.Name, Shared = externalPiggyBank.Shared  }, cancellationToken));

                }
            }
            return result;
        }

        public async Task<UserPiggyBankDTO> RunTwoWayUserPiggyBankIntegration(UserPiggyBankDTO? localUserPiggyBank, UserPiggyBankDTO? externalUserPiggyBank, CancellationToken cancellationToken)
        {
            UserPiggyBankDTO result= null;
            var localuser = await ExecuteWithMediator(async mediator => await _mediator.Send(new GetCurrentUserDetailsQuery { }, CancellationToken.None));
            if (localUserPiggyBank == null && externalUserPiggyBank != null)
            {
                 if (localuser.ExternalId != externalUserPiggyBank.UserId)
                    throw new ArgumentException("UserPiggyBank UserId and User external id mismatch.");
                var piggyBank = await ExecuteWithMediator(async mediator => await mediator.Send(new GetPiggyBankByExternalIdQuery { ExternalId = externalUserPiggyBank.PiggyBankId, InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken));
                var createUserPiggyBankDto = _mapper.Map<CreateUserPiggyBankDTO>(externalUserPiggyBank);
                createUserPiggyBankDto.ExternalId = externalUserPiggyBank.Id;
                createUserPiggyBankDto.UserId = localuser.Id;
                createUserPiggyBankDto.PiggyBankId = piggyBank.Id;
                var userPiggyBankId = (await ExecuteWithMediator(async mediator => await _mediator.Send(new CreateUserPiggyBankCommand { ExternalId = createUserPiggyBankDto.ExternalId, HideBalance = createUserPiggyBankDto.HideBalance, InitiatorUserId = 0, IsExecuteByAdmin = true, PiggyBankId = createUserPiggyBankDto.PiggyBankId, Public = createUserPiggyBankDto.Public, UserId = createUserPiggyBankDto.UserId, Version = createUserPiggyBankDto.Version }, cancellationToken)));
                result = (await ExecuteWithMediator(async mediator => await _mediator.Send(new GetUserPiggyBankLinkQuery { Id = userPiggyBankId, InitiatorUserId = 0, IsExecuteByAdmin = true}, cancellationToken)));
            }
            else if (localUserPiggyBank != null && externalUserPiggyBank == null)
            {
                var createUserPiggyBankDto = _mapper.Map<CreateUserPiggyBankDTO>(localUserPiggyBank);
                createUserPiggyBankDto.UserId = localuser.ExternalId.Value;
                var piggyBank = await ExecuteWithMediator(async mediator => await mediator.Send(new GetPiggyBankQuery { Id = localUserPiggyBank.PiggyBankId, InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken));
                createUserPiggyBankDto.PiggyBankId = piggyBank.ExternalId.Value;
                var userPiggyBankId = await CreateUserPiggyBankLinkOnServerAsync(createUserPiggyBankDto, cancellationToken);
                await ExecuteWithMediator(async mediator => await _mediator.Send(new UpdateUserPiggyBankCommand { ExternalId = userPiggyBankId, HideBalance = localUserPiggyBank.HideBalance, InitiatorUserId = 0, IsExecuteByAdmin = true, Public = localUserPiggyBank.HideBalance, Version = localUserPiggyBank.Version, Id = localUserPiggyBank.Id }, cancellationToken));
                //var link = await GetUserPiggyBankLinkFromServerAsync(userPiggyBankId, cancellationToken);
                result = (UserPiggyBankDTO)localUserPiggyBank.Clone();
                result.ExternalId = userPiggyBankId;
            } else if (localUserPiggyBank.ExternalId == externalUserPiggyBank.Id)
            {
                if (localUserPiggyBank.Version > externalUserPiggyBank.Version)
                {
                    await UpdateUserPiggyBankLinkOnServerAsync(externalUserPiggyBank.Id, _mapper.Map<UpdateUserPiggyBankDTO>(localUserPiggyBank), cancellationToken);
                    result = (UserPiggyBankDTO)localUserPiggyBank.Clone();
                }
                else if (localUserPiggyBank.Version < externalUserPiggyBank.Version)
                {
                    result = (UserPiggyBankDTO)externalUserPiggyBank.Clone();
                    var extId = result.Id;
                    result.Id = localUserPiggyBank.Id;
                    result.ExternalId = extId;
                    result.PiggyBankId = localUserPiggyBank.Id;
                    result.UserId = localUserPiggyBank.Id;
                    await ExecuteWithMediator(async mediator => await _mediator.Send(new UpdateUserPiggyBankCommand { ExternalId = result.ExternalId, HideBalance = result.HideBalance, InitiatorUserId = 0, IsExecuteByAdmin = true, Public = result.Public, Version = result.Version, Id = result.Id}, cancellationToken));

                    
                }
            }
            return result;
        }

        public async Task<PiggyBankCustomizationDTO> RunTwoWayPiggyBankCustomizationIntegration(PiggyBankCustomizationDTO? localPiggyBankCustomization, PiggyBankCustomizationDTO? externalPiggyBankCustomization, CancellationToken cancellationToken)
        {
            PiggyBankCustomizationDTO result = null;
            var localuser = await ExecuteWithMediator(async mediator => await _mediator.Send(new GetCurrentUserDetailsQuery { }, CancellationToken.None));
            if (localPiggyBankCustomization == null && externalPiggyBankCustomization != null)
            {
                var piggyBank = await ExecuteWithMediator(async mediator => await mediator.Send(new GetPiggyBankByExternalIdQuery { ExternalId = externalPiggyBankCustomization.PiggyBankId, InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken));
                var createPiggyBankCustomizationDto = _mapper.Map<CreatePiggyBankCustomizationDTO>(externalPiggyBankCustomization);
                createPiggyBankCustomizationDto.ExternalId = externalPiggyBankCustomization.Id;
                createPiggyBankCustomizationDto.PiggyBankId = piggyBank.Id;

                byte[] photo = null;
                try
                {
                    photo = await GetPiggyBankCustomizationPhotoFromServerAsync(externalPiggyBankCustomization.Id, cancellationToken);
                }
                catch (NotFoundException)
                {

                }

                if (photo != null)
                {
                    var filepath = Path.Combine(GetDefaultLocalPath(), $"pbc_ext_{externalPiggyBankCustomization.Id}_{Guid.NewGuid().ToString()}.jpeg");
                    await UpdateLocalPhotoAsync(photo, filepath);
                    createPiggyBankCustomizationDto.PhotoPath = filepath;
                }

                createPiggyBankCustomizationDto.PhotoIntegrated = true;
                var piggyBankCustomizationId = (await ExecuteWithMediator(async mediator => await _mediator.Send(new CreatePiggyBankCustomizationCommand { ExternalId = createPiggyBankCustomizationDto.ExternalId, InitiatorUserId = 0, IsExecuteByAdmin = true, PhotoPath = createPiggyBankCustomizationDto.PhotoPath, PiggyBankId = createPiggyBankCustomizationDto.PiggyBankId, PiggyBankTypeId = createPiggyBankCustomizationDto.PiggyBankTypeId, Version = createPiggyBankCustomizationDto.Version }, cancellationToken)));
                result = (await ExecuteWithMediator(async mediator => await _mediator.Send(new GetPiggyBankCustomizationByIdQuery { Id = piggyBankCustomizationId, InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken)));


            }
            else if (localPiggyBankCustomization != null && externalPiggyBankCustomization == null)
            {
                var createPiggyBankCustomizationDto = _mapper.Map<CreatePiggyBankCustomizationDTO>(localPiggyBankCustomization);
                var piggyBank = await ExecuteWithMediator(async mediator => await mediator.Send(new GetPiggyBankQuery { Id = localPiggyBankCustomization.PiggyBankId, InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken));
                createPiggyBankCustomizationDto.PiggyBankId = piggyBank.ExternalId.Value;
                var piggyBankCustomizationId = await CreatePiggyBankCustomizationOnServerAsync(createPiggyBankCustomizationDto, cancellationToken);
                await ExecuteWithMediator(async mediator => await _mediator.Send(new UpdatePiggyBankCustomizationCommand { Id = localPiggyBankCustomization.Id, InitiatorUserId = 0, IsExecuteByAdmin = true, PhotoPath = localPiggyBankCustomization.PhotoPath, PiggyBankTypeId = localPiggyBankCustomization.PiggyBankTypeId, Version = localPiggyBankCustomization.Version, ExternalId = piggyBankCustomizationId }, cancellationToken));
                //  var customization = await GetPiggyBankCustomizationFromServerAsync(piggyBankCustomizationId, cancellationToken);
              
                result = (PiggyBankCustomizationDTO)localPiggyBankCustomization.Clone();
                result.ExternalId = piggyBankCustomizationId;
                byte[] photo = null;
                if (localPiggyBankCustomization.PhotoPath != null)
                {
                    if (CheckIfLocalFileExists(localPiggyBankCustomization.PhotoPath))
                    {
                        photo = await GetLocalPhotoAsync(localPiggyBankCustomization.PhotoPath);
                        if (photo != null)
                        {
                            await UploadPiggyBankCustomizationPhotoOnServerAsync(piggyBankCustomizationId, photo, cancellationToken);
                            await ExecuteWithMediator(async mediator => await _mediator.Send(new UpdatePiggyBankCustomizationCommand { Id = localPiggyBankCustomization.Id, InitiatorUserId = 0, IsExecuteByAdmin = true, PhotoPath = localPiggyBankCustomization.PhotoPath, PiggyBankTypeId = localPiggyBankCustomization.PiggyBankTypeId, Version = localPiggyBankCustomization.Version, ExternalId = piggyBankCustomizationId, PhotoIntegrated = true }, cancellationToken));
                        }
                    }
                }
            }
            else if (localPiggyBankCustomization.ExternalId == externalPiggyBankCustomization.Id)
            {
                if (localPiggyBankCustomization.Version > externalPiggyBankCustomization.Version)
                {
                    byte[] photo = null;

                    if (localPiggyBankCustomization.PhotoPath != null)
                    {
                        if (CheckIfLocalFileExists(localPiggyBankCustomization.PhotoPath))
                        {
                            photo = await GetLocalPhotoAsync(localPiggyBankCustomization.PhotoPath);
                            if (photo != null)
                            {
                                await UploadPiggyBankCustomizationPhotoOnServerAsync(externalPiggyBankCustomization.Id, photo, cancellationToken);
                                await ExecuteWithMediator(async mediator => await _mediator.Send(new UpdatePiggyBankCustomizationCommand { Id = localPiggyBankCustomization.Id, InitiatorUserId = 0, IsExecuteByAdmin = true, PhotoPath = localPiggyBankCustomization.PhotoPath, PiggyBankTypeId = localPiggyBankCustomization.PiggyBankTypeId, Version = localPiggyBankCustomization.Version, ExternalId = localPiggyBankCustomization.ExternalId, PhotoIntegrated = true }, cancellationToken));
                            }
                        }
                    }
                    result = (PiggyBankCustomizationDTO)localPiggyBankCustomization.Clone();
                    await UpdatePiggyBankCustomizationOnServerAsync(externalPiggyBankCustomization.Id, _mapper.Map<UpdatePiggyBankCustomizationDTO>(localPiggyBankCustomization), cancellationToken);

                }
                else if (localPiggyBankCustomization.Version < externalPiggyBankCustomization.Version)
                {

                    byte[] photo = null;
                    string photopath = null;
                    try
                    {
                        photo = await GetPiggyBankCustomizationPhotoFromServerAsync(externalPiggyBankCustomization.Id, cancellationToken);
                    }
                    catch (NotFoundException)
                    {

                    }

                    if (photo != null)
                    {
                        photopath = Path.Combine(GetDefaultLocalPath(), $"pbc_{localPiggyBankCustomization.Id}_{Guid.NewGuid().ToString()}.jpeg");
                        await UpdateLocalPhotoAsync(photo, photopath);
                    }

                    await ExecuteWithMediator(async mediator => await _mediator.Send(new UpdatePiggyBankCustomizationCommand { Id = localPiggyBankCustomization.Id, InitiatorUserId = 0, IsExecuteByAdmin = true, PhotoPath =  photopath, PiggyBankTypeId = externalPiggyBankCustomization.PiggyBankTypeId, Version = externalPiggyBankCustomization.Version, ExternalId = externalPiggyBankCustomization.Id, PhotoIntegrated = true }, cancellationToken));
                    result = (await ExecuteWithMediator(async mediator => await _mediator.Send(new GetPiggyBankCustomizationByIdQuery { Id = localPiggyBankCustomization.Id, InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken)));
                }
                else if (localPiggyBankCustomization.PhotoIntegrated = false && localPiggyBankCustomization.PhotoPath != null)
                {
                    if (CheckIfLocalFileExists(localPiggyBankCustomization.PhotoPath))
                    {
                        byte[] photo = null;
                        photo = await GetLocalPhotoAsync(localPiggyBankCustomization.PhotoPath);
                        if (photo != null)
                        {
                            await UploadPiggyBankCustomizationPhotoOnServerAsync(externalPiggyBankCustomization.Id, photo, cancellationToken);
                            await ExecuteWithMediator(async mediator => await _mediator.Send(new UpdatePiggyBankCustomizationCommand { Id = localPiggyBankCustomization.Id, InitiatorUserId = 0, IsExecuteByAdmin = true, PhotoPath = localPiggyBankCustomization.PhotoPath, PiggyBankTypeId = localPiggyBankCustomization.PiggyBankTypeId, Version = localPiggyBankCustomization.Version, ExternalId = localPiggyBankCustomization.ExternalId, PhotoIntegrated = true }, cancellationToken));
                        }
                    }
                    result = (PiggyBankCustomizationDTO)localPiggyBankCustomization.Clone();
                    result.PhotoIntegrated = true;
                }
            }
            
            return result;
        }

        public async Task<List<UserFriendshipDetailsDTO>> GetUserFriendshipsByUserIdFromServerAsync(int userId, CancellationToken cancellationToken)
        {
            try
            {
                return await RunScopedApiCommunication(async apiCommunicator => await _apiCommunicatorService.GetAsync<List<UserFriendshipDetailsDTO>>(_apiEndpoints.GetUserFriendshipsByUserId.ReplacePlaceholders(new Dictionary<string, string> { { "id", userId.ToString() } })));
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new NotFoundException(typeof(Domain.UserFriendship).ToString(), userId);
                else
                    throw ex;
            }
        }

        public async Task<List<TransactionDTO>> GetTransactionsByPiggyBankIdFromServerAsync(int piggyBankId, CancellationToken cancellationToken)
        {
            try
            {
                return await RunScopedApiCommunication(async apiCommunicator => await _apiCommunicatorService.GetAsync<List<TransactionDTO>>(_apiEndpoints.GetTransactionsByPiggyBankId.ReplacePlaceholders(new Dictionary<string, string> { { "id", piggyBankId.ToString() } })));
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new NotFoundException(typeof(Domain.PiggyBank).ToString(), piggyBankId);
                else
                    throw;
            }
        }

        public async Task<List<UserPiggyBankDTO>> GetUserPiggyBankLinksByPiggyBankIdFromServerAsync(int piggyBankId, CancellationToken cancellationToken)
        {
            try
            {
                return await RunScopedApiCommunication(async apiCommunicator => await _apiCommunicatorService.GetAsync<List<UserPiggyBankDTO>>(_apiEndpoints.GetUserPiggyBankLinksByPiggyBankId.ReplacePlaceholders(new Dictionary<string, string> { { "id", piggyBankId.ToString() } })));
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new NotFoundException(typeof(Domain.UserPiggyBank).ToString(), piggyBankId);
                else
                    throw;
            }
        }

        public async Task<TransactionDTO> RunTwoWayTransactionIntegration(TransactionDTO? localTransaction, TransactionDTO? externalTransaction, CancellationToken cancellationToken)
        {
            TransactionDTO result = null;
            var localuser = await ExecuteWithMediator(async mediator => await _mediator.Send(new GetCurrentUserDetailsQuery { }, CancellationToken.None));
            if (localTransaction != null && localTransaction.IsDeleted == true)
            {
                if (localTransaction.ExternalId.HasValue)
                    await DeleteTransactionFromServerAsync(localTransaction.ExternalId.Value, cancellationToken, false);
                await ExecuteWithMediator(async mediator => await _mediator.Send(new DeleteTransactionCommand { Id = localTransaction.Id, InitiatorUserId = 0, IsExecuteByAdmin = true, UpdatePiggyBankBalance = false }, cancellationToken));
                result = null;
            }
            else if (localTransaction == null && externalTransaction != null)
            {
                var piggyBank = await ExecuteWithMediator(async mediator => await mediator.Send(new GetPiggyBankByExternalIdQuery { ExternalId = externalTransaction.PiggyBankId, InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken));
                var createTransactionDto = _mapper.Map<CreateTransactionDTO>(externalTransaction);
                createTransactionDto.ExternalId = externalTransaction.Id;
                createTransactionDto.UserId = localuser.Id;
                createTransactionDto.PiggyBankId = piggyBank.Id;
                var transactionId = (await ExecuteWithMediator(async mediator => await _mediator.Send(new CreateTransactionCommand { Amount = createTransactionDto.Amount, Date = createTransactionDto.Date, Description = createTransactionDto.Description, ExternalId = createTransactionDto.ExternalId, InitiatorUserId = 0, IsExecuteByAdmin = true, PaymentTypeId = createTransactionDto.PaymentTypeId, PiggyBankId = createTransactionDto.PiggyBankId, TransactionTypeId = createTransactionDto.TransactionTypeId, UpdatePiggyBankBalance = false, UserId = createTransactionDto.UserId, Version = createTransactionDto.Version }, cancellationToken)));
                result = (await ExecuteWithMediator(async mediator => await _mediator.Send(new GetTransactionByIdQuery { Id = transactionId, InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken)));
            }
            else if (localTransaction != null && externalTransaction == null)
            {
                var createTransactionDto = _mapper.Map<CreateTransactionDTO>(localTransaction);
                var piggyBank = await ExecuteWithMediator(async mediator => await mediator.Send(new GetPiggyBankQuery { Id = localTransaction.PiggyBankId, InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken));
                createTransactionDto.PiggyBankId = piggyBank.ExternalId.Value;
                createTransactionDto.UserId = localuser.ExternalId.Value;
                var transactionId = await CreateTransactionOnServerAsync(createTransactionDto, cancellationToken, false);
                await ExecuteWithMediator(async mediator => await _mediator.Send(new UpdateTransactionCommand { Date = localTransaction.Date, Description = localTransaction.Description, Amount = localTransaction.Amount, ExternalId = transactionId, Id = localTransaction.Id, InitiatorUserId = 0, IsDeleted = localTransaction.IsDeleted, IsExecuteByAdmin = true, PaymentTypeId = localTransaction.PaymentTypeId, TransactionTypeId = localTransaction.TransactionTypeId, UpdatePiggyBankBalance = false, Version = localTransaction.Version}, cancellationToken));
                result = (TransactionDTO)localTransaction.Clone();
                result.ExternalId = transactionId;
            }
            else if (localTransaction.ExternalId == externalTransaction.Id)
            {
                if (localTransaction.Version > externalTransaction.Version)
                {
                    await UpdateTransactionOnServerAsync(externalTransaction.Id, _mapper.Map<UpdateTransactionDTO>(localTransaction), cancellationToken, false);
                    result = (TransactionDTO)localTransaction.Clone();
                }
                else if (localTransaction.Version < externalTransaction.Version)
                {
                    await ExecuteWithMediator(async mediator => await _mediator.Send(new UpdateTransactionCommand { Date = externalTransaction.Date, Id = localTransaction.Id, Amount = externalTransaction.Amount, Description = externalTransaction.Description, ExternalId = externalTransaction.ExternalId, InitiatorUserId = 0, IsDeleted = localTransaction.IsDeleted, IsExecuteByAdmin = true, PaymentTypeId = externalTransaction.PaymentTypeId, TransactionTypeId = externalTransaction.TransactionTypeId, UpdatePiggyBankBalance = false, Version = externalTransaction.Version }, cancellationToken));
                    result = (await ExecuteWithMediator(async mediator => await _mediator.Send(new GetTransactionByIdQuery { Id = localTransaction.Id, InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken)));
                }
            }
            return result;
        }

        public async Task<byte[]> GetUserPhotoFromServerAsync(int userId, CancellationToken cancellationToken)
        {
            try
            {
                return await RunScopedApiCommunication(async apiCommunicator => await _apiCommunicatorService.GetAsync<byte[]>(_apiEndpoints.GetUserPhoto.ReplacePlaceholders(new Dictionary<string, string> { { "id", userId.ToString() } })));
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new NotFoundException("user or user photo", userId);
                else
                    throw ex;
            }
        }

        public async Task DeleteUserPhotoFromServerAsync(int userId, CancellationToken cancellationToken)
        {
            try
            {
                await RunScopedApiCommunication(async apiCommunicator => { await _apiCommunicatorService.DeleteAsync(_apiEndpoints.DeleteUserPhoto.ReplacePlaceholders(new Dictionary<string, string> { { "id", userId.ToString() } })); return Unit.Value; });
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode != System.Net.HttpStatusCode.NotFound)
                    throw;
            }
        }

        public async Task UploadUserPhotoOnServerAsync(int userId, byte[] data, CancellationToken cancellationToken)
        {
            try
            {
                await RunScopedApiCommunication(async apiCommunicator => await _apiCommunicatorService.PutAsync<byte[], object>(_apiEndpoints.UpdateUserPhoto.ReplacePlaceholders(new Dictionary<string, string> { { "id", userId.ToString() } }), data));
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new NotFoundException("user or user photo", userId);
                else
                    throw;
            }
        }

        public async Task<byte[]> GetPiggyBankCustomizationPhotoFromServerAsync(int piggyBankCustomizationId, CancellationToken cancellationToken)
        {
            try
            {
                return await RunScopedApiCommunication(async apiCommunicator => await _apiCommunicatorService.GetAsync<byte[]>(_apiEndpoints.GetPiggyBankCustomizationPhoto.ReplacePlaceholders(new Dictionary<string, string> { { "id", piggyBankCustomizationId.ToString() } })));
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new NotFoundException("piggy bank customization or photo", piggyBankCustomizationId);
                else
                    throw;
            }
        }

        public async Task DeletePiggyBankCustomizationPhotoFromServerAsync(int piggyBankCustomizationId, CancellationToken cancellationToken)
        {
            try
            {
                await RunScopedApiCommunication(async apiCommunicator => { await _apiCommunicatorService.DeleteAsync(_apiEndpoints.DeletePiggyBankCustomizationPhoto.ReplacePlaceholders(new Dictionary<string, string> { { "id", piggyBankCustomizationId.ToString() } })); return Unit.Value; });
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode != System.Net.HttpStatusCode.NotFound)
                    throw;
            }
        }

        public async Task UploadPiggyBankCustomizationPhotoOnServerAsync(int piggyBankCustomizationId, byte[] data, CancellationToken cancellationToken)
        {
            try
            {
                await RunScopedApiCommunication(async apiCommunicator => await _apiCommunicatorService.PutAsync<byte[], object>(_apiEndpoints.UpdatePiggyBankCustomizationPhoto.ReplacePlaceholders(new Dictionary<string, string> { { "id", piggyBankCustomizationId.ToString() } }), data));
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new NotFoundException("piggy bank customization or photo", piggyBankCustomizationId);
                else
                    throw;
            }
        }

        public string GetPiggyBankCustomizationPhotoURL(int piggyBankCustomizationId)
        {
            return _apiEndpoints.GetPiggyBankCustomizationPhoto.ReplacePlaceholders(new Dictionary<string, string> { { "id", piggyBankCustomizationId.ToString() } });
        }

        public string? GetCurrentAccessToken()
        {
            return _session == null ? null : _session.AccessToken;
        }

        public string GetUserPhotoURL(int userId)
        {
            return _apiEndpoints.GetUserPhoto.ReplacePlaceholders(new Dictionary<string, string> { { "id", userId.ToString() } });
        }


        private async Task DeleteLocalPhotoAsync(string path)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                _fileService = scope.ServiceProvider.GetRequiredService<IFileService>();
                if (_fileService.Exist(path))
                {
                    await _fileService.RemoveFileAsync(path);
                }
            }
        }

        private async Task<byte[]> GetLocalPhotoAsync(string path)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                _fileService = scope.ServiceProvider.GetRequiredService<IFileService>();
                return await _fileService.ReadFileAsync(path);
            }
        }

        private bool CheckIfLocalFileExists(string path)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                _fileService = scope.ServiceProvider.GetRequiredService<IFileService>();
                return _fileService.Exist(path);
            }
        }

        private async Task UpdateLocalPhotoAsync(byte[] data, string path)
        {
            if (data != null)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    _fileService = scope.ServiceProvider.GetRequiredService<IFileService>();
                    await _fileService.SaveImageAsJpegAsync(data, path);
                }
            }

        }

        private string GetDefaultLocalPath()
        {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    _fileService = scope.ServiceProvider.GetRequiredService<IFileService>();
                    return _fileService.GetDefaultPath();
                }

        }
    }
}
