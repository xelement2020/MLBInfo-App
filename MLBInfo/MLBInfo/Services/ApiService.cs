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

namespace MLBPlayersApp.Services
{
    public class ApiService :Config, IApiService
    {
        public async Task<IList<Team>> GetTeamsList(string seasonType, string season)
        {
            HttpClient httpClient = new HttpClient();

            var result = await httpClient.GetStringAsync($"{url}all_star_sw='{seasonType}'&sort_order='name_asc'&season={season}");
            var data = JsonConvert.DeserializeObject<TeamQuery>(result);
            return data?.TeamAllSeason?.QueryResults?.Teams;

        }

        public async Task<QueryResults> GetPlayersList(string search, string active)
        {
            
            search.Replace(" ", "_");
            search.ToLower();
            HttpClient httpClient = new HttpClient();
            var result = await httpClient.GetStringAsync($"{uri}.search_player_all.bam?sport_code='mlb'&active_sw='{active}'&name_part='{search}%25'");

            return JsonConvert.DeserializeObject<SearchQuery>(result)?.SearchPlayerAll?.QueryResults;
        }

        public async Task<PlayerData> GetPlayerData(string id)
        {
            HttpClient httpClient = new HttpClient();
            var result = await httpClient.GetStringAsync($"{uri}.player_info.bam?sport_code='mlb'&player_id={id}");

            return JsonConvert.DeserializeObject<PlayerInfoResult>(result)?.PlayerInfo?.QueryResults?.PlayerData;
        }

        public async Task<IList<Row>> GetRowData(string startSeason, string endSeason, string teamId)
        {
            HttpClient httpClient = new HttpClient();

            var result = await httpClient.GetStringAsync($"{url1}/json/named.roster_team_alltime.bam?start_season={startSeason}&end_season={endSeason}&team_id={teamId}");
            var data = JsonConvert.DeserializeObject<RowExample>(result);
            return data?.RowRosterTeamAlltime?.RowQueryResults?.Rows;
        }

      
    }
}
