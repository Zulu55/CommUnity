using Blazored.Modal.Services;
using CommUnity.FrontEnd.Pages.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CommUnity.FrontEnd.Shared
{
    public partial class AuthLinks
    {
        private string? photoUser;

        [Inject] NavigationManager NavigationManager { get; set; } = null!;
        [Inject] IDialogService DialogService { get; set; } = null!;
        [CascadingParameter]
        private Task<AuthenticationState> AuthenticationStateTask { get; set; } = null!;

        [CascadingParameter] private IModalService Modal { get; set; } = default!;

        protected override async Task OnParametersSetAsync()
        {
            var authenticationState = await AuthenticationStateTask;
            var claims = authenticationState.User.Claims.ToList();
            var photoClaim = claims.FirstOrDefault(x => x.Type == "Photo");
            var nameClaim = claims.FirstOrDefault(x => x.Type == "UserName");
            if (photoClaim is not null)
            {
                photoUser = photoClaim.Value;
            }
        }

        private void EditAction()
        {
            NavigationManager.NavigateTo("/auth/edituser");

        }

        private void ShowModalLogIn()
        {
            DialogOptions closeOnEscapeKey = new DialogOptions() { CloseOnEscapeKey = true };
            DialogService.Show<Login>("Inicio de Sesion", closeOnEscapeKey);
        }

        private void ShowModalLogOut()
        {
            Modal.Show<Logout>();
        }
    }
}