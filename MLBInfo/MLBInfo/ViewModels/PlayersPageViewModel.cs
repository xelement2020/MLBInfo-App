﻿using MLBApp;
using MLBInfo.Models;
using MLBInfo.ViewModels;
using MLBPlayersApp.Models;
using MLBPlayersApp.Services;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using Prism.Navigation.TabbedPages;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MLBPlayersApp.ViewModels
{
    public class PlayersPageViewModel :BaseViewModel, INotifyPropertyChanged
    {
        public ObservableCollection<Player> Players { get; set; }

        public PlayerData Player { get; set; }


        private Player oldPlayer;

        public event PropertyChangedEventHandler PropertyChanged;
        public string SearchEntry { get; set; }
        public DelegateCommand SearchPlayerCommand { get; set; }
        public DelegateCommand ViewPlayerInfoCommand { get; set; }

        public PlayersPageViewModel(INavigationService navigationService, IApiService apiService, PageDialogService pagedialogservice, SeassonData seassonData) : base(navigationService, apiService, pagedialogservice, seassonData)
        {
            SearchPlayerCommand = new DelegateCommand(async() =>
            {
                if(!string.IsNullOrEmpty(SearchEntry)) await GetPlayerData(SearchEntry); 
            });

            ViewPlayerInfoCommand = new DelegateCommand(async () =>
            {
                await ViewPlayerInfo();
            });
        }

        public async Task GetPlayerData(string search)
        {
            Players?.Clear();
            if (await this.HasInternet())
            {
                try
                {
                    var results = await ApiService.GetPlayersList(search);
                    Players = new ObservableCollection<Player>(results as List<Player>);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"API EXCEPTION {ex}");
                }

            }
        }

        public async Task ViewPlayerInfo()
        {
            if (await this.HasInternet())
            {
                var nav = new NavigationParameters();
                nav.Add("Picture", oldPlayer.PlayerPicture);
                Player = await ApiService.GetPlayerData(oldPlayer.PlayerId);
                nav.Add("Name", Player.NameDisplayFirstLast);
                nav.Add("TeamName", Player.TeamName);
                nav.Add("PrimaryPosition", Player.PrimaryPosition);
                nav.Add("JerseyNumber", Player.JerseyNumber);
                nav.Add("Weight", Player.Weight);
                nav.Add("Age", Player.Age);
                nav.Add("BirthCountry", Player.BirthCountry);
                nav.Add("Status", Player.Status);
                nav.Add("TeamId", Player.TeamId);
                nav.Add("Twitter", Player.TwitterId);
                await NavigationService.NavigateAsync(NavConstants.PlayerInfoPage, nav);
            }
        }

        public void HideOrShowPlayer(Player player)
        {
            if (oldPlayer == player)
            {
                player.IsVisible = !player.IsVisible;
                UpdatePlayersList(player);
            }
            else
            {
                if(oldPlayer != null)
                {
                    oldPlayer.IsVisible = false;
                    UpdatePlayersList(oldPlayer);
                }
                player.IsVisible = true;
                UpdatePlayersList(player);

            }
            oldPlayer = player;
        }

        private void UpdatePlayersList(Player player)
        {
            int index = Players.IndexOf(player);
            Players.Remove(player);
            Players.Insert(index, player);
        }
    }
}
