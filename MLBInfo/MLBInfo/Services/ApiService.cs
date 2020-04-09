﻿using MLBApp;
using MLBInfo.Models;
using MLBPlayersApp.Models;
using MLBTeamsApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Xamarin.Forms;

namespace MLBPlayersApp.Services
{
    public class ApiService:Config, IApiService
    {
        public async Task<IList<Team>> GetTeamsList(string seasonType, string season)
        {
            HttpClient httpClient = new HttpClient();

            var result = await httpClient.GetStringAsync($"{url}all_star_sw='{seasonType}'&sort_order='name_asc'&season={season}");
            var data = JsonConvert.DeserializeObject<TeamQuery>(result);
            return data?.TeamAllSeason?.QueryResults?.Teams;

        }

        public async Task<List<Player>> GetPlayersList(string search)
        {
            search = search.Trim().Replace(" ", "_").ToLower();
            HttpClient httpClient = new HttpClient();
            var result = await httpClient.GetStringAsync($"{uri}.search_player_all.bam?sport_code='mlb'&name_part='{search}%25'");

            QueryResults queryResults = JsonConvert.DeserializeObject<SearchQuery>(result)?.SearchPlayerAll?.QueryResults;
            List<Player> players = queryResults.PlayersList;
            foreach(Player player in players)
            {
                var playerImage = await httpClient.GetStringAsync($"{player_image}{player.NameDisplayFirstLast.Replace(" ", "_")}");
                PlayerImageData playerImageData = JsonConvert.DeserializeObject<PlayerImageData>(playerImage);

                if (playerImageData.PlayerImage != null)
                {
                    player.PlayerPicture = (playerImageData.PlayerImage[0].StrCutout != null) ? playerImageData.PlayerImage[0].StrCutout : playerImageData.PlayerImage[0].StrThumb;
                }
                else
                {
                    player.PlayerPicture = "ic_account_circle.png";
                }
            }

            return players;
        }

        public async Task<PlayerData> GetPlayerData(string id)
        {
            HttpClient httpClient = new HttpClient();
            var result = await httpClient.GetStringAsync($"{uri}.player_info.bam?sport_code='mlb'&player_id={id}");

            PlayerData player = JsonConvert.DeserializeObject<PlayerInfoResult>(result)?.PlayerInfo?.QueryResults?.PlayerData;

            return player;
        }

        public async Task<IList<Row>> GetRowData(string teamId)
        {
            HttpClient httpClient = new HttpClient();

            var result = await httpClient.GetStringAsync($"{url1}.roster_40.bam?team_id='{teamId}'");
            var data = JsonConvert.DeserializeObject<TeamRoster>(result);
            return data?.Roster40.QueryResults.Row;
        }

        public async Task<List<Game>> GetUpcomingGames()
        {
            string season = DateTime.Now.ToString("yyyy");
            string currentDate = DateTime.Now.ToString("yyyyMMdd");
            string lastDate = DateTime.Now.AddDays(30).ToString("yyyyMMdd");

            HttpClient httpClient = new HttpClient();
            var result = await httpClient.GetStringAsync($"{url1}.mlb_broadcast_info.bam?src_type='TV'&src_comment='National'&tcid=mm_mlb_schedule&sort_by='game_time_et_asc'&start_date='{currentDate}'&end_date='{lastDate}'&season={season}");
            GamesResults gamesResults = JsonConvert.DeserializeObject<UpcomingGames>(result)?.MlbBroadcastInfo?.GamesResults;

            httpClient.DefaultRequestHeaders.Add("x-rapidapi-key", "44a347ecac0e3450b4fc668536b1a191");

            var logos = await httpClient.GetStringAsync(logos_url);
            TeamLogos teamLogos = JsonConvert.DeserializeObject<TeamLogos>(logos);

            var data = gamesResults.GamesList;
            foreach (Game game in data)
            {
                string homeTeamName = game.HomeTeamFull.Replace(" ", "");
                var homeTeamLogo = from homeTeam in teamLogos.TeamsList where homeTeamName == homeTeam.TeamName select homeTeam.Logo;
                game.HomeTeamLogo = homeTeamLogo.First<string>();
            
                string awayTeamName = game.AwayTeamFull.Replace(" ", "");
                var awayTeamLogo = from awayTeam in teamLogos.TeamsList where awayTeamName == awayTeam.TeamName select awayTeam.Logo;
                game.AwayTeamLogo = awayTeamLogo.First<string>();
            }

            return data;
        }
    }
}
