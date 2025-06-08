using AutoMapper;
using Kopilych.Application.Interfaces;
using Kopilych.Mobile.Middleware;
using Kopilych.Mobile.View_Models;
using Kopilych.Mobile.Views;
using Kopilych.Shared.DTO;

namespace Kopilych.Mobile
{
    public partial class App : Microsoft.Maui.Controls.Application
    {
        private readonly IUserInfoService _userInfoService;
        private readonly IIntegrationService _integrationService;
        private readonly IMapper _mapper;
        public App(IUserInfoService userInfoService, IIntegrationService integrationService, IMapper mapper)
        {
            _userInfoService = userInfoService;
            InitializeComponent();
            MainPage = new AppShell();
            _integrationService = integrationService;
            _mapper = mapper;   
        }

        public async Task HandleAuthCallback(string success, string accessToken, string refreshToken)
        {
            try
            {
                if (success == "true")
                {
                    var currUser = await _userInfoService.GetCurrentUserDetailsAsync(CancellationToken.None, false);
                    await _userInfoService.СreateOrUpdateUserSessionAsync(new CreateOrUpdateUserSessionDTO { UserId = currUser.Id, AccessToken = accessToken, RefreshToken = refreshToken }, CancellationToken.None);
                    var externalUser = await _userInfoService.GetCurrentUserDetailsAsync(CancellationToken.None, true);
                    var updateUserDTO = _mapper.Map<UpdateUserDTO>(currUser);
                    updateUserDTO.ExternalId = externalUser.ExternalId;
                    await _userInfoService.UpdateUserAsync(currUser.Id, updateUserDTO, CancellationToken.None, false);
                    currUser = await _userInfoService.GetCurrentUserDetailsAsync(CancellationToken.None, false);
                    await _userInfoService.RunTwoWayUserIntegration(currUser, externalUser, CancellationToken.None);
                    _integrationService.FinishAuth();
                }
            }
            catch (Exception ex) 
            {
                CustomExceptionHandlerMiddleware.Handle(ex);
            }
          
        }

    }
}
    
