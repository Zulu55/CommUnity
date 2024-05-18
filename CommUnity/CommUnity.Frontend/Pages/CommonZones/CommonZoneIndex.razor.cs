﻿using Blazored.Modal;
using Blazored.Modal.Services;
using CommUnity.FrontEnd.Pages.Apartments;
using CommUnity.FrontEnd.Repositories;
using CommUnity.Shared.Entities;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor;
using System.Diagnostics.Metrics;
using System.Net;

namespace CommUnity.FrontEnd.Pages.CommonZones
{
    public partial class CommonZoneIndex
    {
        private ResidentialUnit? residentialUnit;
        private List<CommonZone>? commonZones;

        private MudTable<CommonZone> table = new();
        private readonly int[] pageSizeOptions = { 10, 25, 50, int.MaxValue };
        private int totalRecords = 0;
        private bool loading;

        [Parameter] public int ResidentialUnitId { get; set; }

        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;

        [Parameter, SupplyParameterFromQuery] public string Filter { get; set; } = string.Empty;

        [CascadingParameter] IModalService Modal { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await LoadAsync();
        }

        private async Task LoadAsync()
        {
            await LoadTotalRecords();
        }

        private async Task<bool> LoadResidentialUnitAsync()
        {
            var responseHttp = await Repository.GetAsync<ResidentialUnit>($"/api/residentialunit/{ResidentialUnitId}");
            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo("/");
                    return false;
                }

                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return false;
            }
            residentialUnit = responseHttp.Response;
            return true;
        }

        private async Task<bool> LoadTotalRecords()
        {
            loading = true;
            if (residentialUnit is null)
            {
                var ok = await LoadResidentialUnitAsync();
                if (!ok)
                {
                    NoResidentialUnit();
                    return false;
                }
            }
            string baseUrl = "api/commonzones";
            string url;

            url = $"{baseUrl}/recordsnumber?id={ResidentialUnitId}&page=1&recordsnumber={int.MaxValue}";
            if (!string.IsNullOrWhiteSpace(Filter))
            {
                url += $"&filter={Filter}";
            }
            var responseHttp = await Repository.GetAsync<int>(url);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync(new SweetAlertOptions
                {
                    Title = "Error",
                    Text = message,
                    Icon = SweetAlertIcon.Error
                });
                return false;
            }
            totalRecords = responseHttp.Response;
            loading = false;
            return true;
        }

        private async Task<TableData<CommonZone>> LoadListAsync(TableState state)
        {
            int page = state.Page + 1;
            int pageSize = state.PageSize;

            string baseUrl = $"api/commonzones";
            string url;

            url = $"{baseUrl}?id={ResidentialUnitId}&page={page}&recordsnumber={pageSize}";
            if (!string.IsNullOrWhiteSpace(Filter))
            {
                url += $"&filter={Filter}";
            }

            var responseHttp = await Repository.GetAsync<List<CommonZone>>(url);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync(new SweetAlertOptions
                {
                    Title = "Error",
                    Text = message,
                    Icon = SweetAlertIcon.Error
                });
                return new TableData<CommonZone> { Items = new List<CommonZone>(), TotalItems = 0 };
            }
            if (responseHttp.Response == null)
            {
                return new TableData<CommonZone> { Items = new List<CommonZone>(), TotalItems = 0 };
            }
            return new TableData<CommonZone>
            {
                Items = responseHttp.Response,
                TotalItems = totalRecords
            };
        }

        private async Task SetFilterValue(string value)
        {
            Filter = value;
            await LoadAsync();
            await table.ReloadServerData();
        }

        private void ReturnAction()
        {
            NavigationManager.NavigateTo("/residentialunits");
        }

        private async Task ShowModalAsync(int id = 0, bool isEdit = false)
        {
            IModalReference modalReference;

            if (isEdit)
            {
                modalReference = Modal.Show<CommonZoneEdit>(string.Empty, new ModalParameters().Add("CommonZoneId", id));
            }
            else
            {
                modalReference = Modal.Show<CommonZoneCreate>(string.Empty, new ModalParameters().Add("ResidentialUnitId", ResidentialUnitId));
            }

            var result = await modalReference.Result;
            if (result.Confirmed)
            {
                await LoadAsync();
            }
            await table.ReloadServerData();
        }

        private void NoResidentialUnit()
        {
            NavigationManager.NavigateTo("/residentialunits");
        }

        private async Task DeleteAsync(CommonZone commonZone)
        {
            var result = await SweetAlertService.FireAsync(new SweetAlertOptions
            {
                Title = "¿Estás seguro?",
                Text = $"¿Estás seguro de que quieres eliminar la Zona Común {commonZone.Name}?",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
            });
            var confirm = string.IsNullOrEmpty(result.Value);
            if (confirm)
            {
                return;
            }

            var responseHttp = await Repository.DeleteAsync<ResidentialUnit>($"api/commonzones/{commonZone.Id}");
            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    var message = await responseHttp.GetErrorMessageAsync();
                    await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                    return;
                }
                else
                {
                    var message = await responseHttp.GetErrorMessageAsync();
                    await SweetAlertService.FireAsync(new SweetAlertOptions
                    {
                        Title = "Error",
                        Text = message,
                        Icon = SweetAlertIcon.Error
                    });
                }
                return;
            }
            await LoadAsync();
            await table.ReloadServerData();
            var toast = SweetAlertService.Mixin(new SweetAlertOptions
            {
                Toast = true,
                Position = SweetAlertPosition.BottomEnd,
                ShowConfirmButton = true,
                Timer = 3000
            });
            await toast.FireAsync("Registro Eliminado", string.Empty, SweetAlertIcon.Success);
        }
    }
}