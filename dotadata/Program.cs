﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using dotadata;
using MatchNameSpace;
using DotaMatchHistory;

namespace dotadata
{
    class Program
    {
        public static string API = "23CEC905617913D3710DC832621110F3";
        
        static void Main(string[] args)
        {
            string apiKey = "23CEC905617913D3710DC832621110F3";
            string matchhistoryUrl = @"https://api.steampowered.com/IDOTA2Match_570/GetMatchHistory/V001/?key=";
            string herosUrl = @"https://api.steampowered.com/IEconDOTA2_570/GetHeroes/v0001/?key=";

            List<Match> matchHistory = new List<Match>();
            List<herosClass> heroesClassdota = new List<herosClass>();

            var heros = GetHeroes(herosUrl, API, heroesClassdota);
            matchHistory = GetMatchHistory(matchhistoryUrl, apiKey, heros);

            Match dota = new Match();
            
        }

        public static string ConvertHeroFromID(int id,herosClass.HeroesObject heroes)
        {
            foreach(var hero in heroes.result.heroes)
            {
                if(hero.id == id)
                {
                    return hero.name;
                }
            }
            return "noheroMatch";
        }

        public static List<Match> GetMatchHistory(string uri, string api, herosClass.HeroesObject heroes)
        {
            //create a container to store all of matches
            List<Match> _matches = new List<Match>();
            try
            {
                var response = string.Empty;
                
                //we format our url to include our api key
                Uri completeUri = new Uri(string.Format("{0}{1}", uri, API));
                WebClient client = new WebClient();

                //downloading the json response
                response = client.DownloadString(completeUri);

                //serializing json data to our class
                MatchRootObject ourResponse = JsonConvert.DeserializeObject<MatchRootObject>(response);
                foreach(var match in ourResponse.result.matches)
                {
                    //start looking up details on first match
                    Match _match = new Match();
                    Console.WriteLine("Match ID: {0}", match.match_id);
                    _match.lobby_type = match.lobby_type;
                    _match.match_id = match.match_id;
                    _match.match_seq_num = match.match_seq_num;
                    _match.players = match.players;
                    _match.start_time = match.start_time;

                    //looping through each player in the match
                    foreach(var player in match.players)
                    {
                        
                        string name = ConvertHeroFromID(player.hero_id,heroes);
                        //used to make names pretty
                        string splitword = "npc_dota_hero_";
                        var splitName = SplitTextByWord(name, splitword);
                        string cleanName = splitName[1].Replace("_", " ");

                        Console.WriteLine((UppercaseFirst(cleanName)));
                        Console.WriteLine("Account ID: {0}", player.account_id);
                        Console.WriteLine("Hero ID: {0}",player.hero_id);
                        Console.WriteLine("=======================================");
                    }

                    _matches.Add(_match);
                    
                }
                return _matches;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return _matches;
             
            }
        }
        public static string UppercaseFirst(string s)
        {
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }
        //static string UppercaseFirst(string s)
        //{
        //    // Check for empty string.
        //    if (string.IsNullOrEmpty(s))
        //    {
        //        return string.Empty;
        //    }
        //    // Return char and concat substring.
        //    return char.ToUpper(s[0]) + s.Substring(1);
        //}

        public static List<string> SplitTextByWord(string text, string splitTerm)
        {
            List<string> splitItems = new List<string>();
            if (string.IsNullOrEmpty(text)) return splitItems;
            if (string.IsNullOrEmpty(splitTerm))
            {
                splitItems.Add(text);
                return splitItems;
            }
            int nextPos = 0;
            int curPos = 0;
            while (nextPos > -1)
            {
                nextPos = text.IndexOf(splitTerm, curPos);
                if (nextPos != -1)
                {
                    splitItems.Add(text.Substring(curPos, nextPos - curPos));
                    curPos = nextPos + splitTerm.Length;
                }
            }
            splitItems.Add(text.Substring(curPos, text.Length - curPos));

            return splitItems;
        }

        public static herosClass.HeroesObject GetHeroes(string uri, string api,List<herosClass> heroes)
        {
            try
            {
                var response = string.Empty;
                Uri completeUri = new Uri(string.Format("{0}{1}", uri, api));
                WebClient client = new WebClient();
                response = client.DownloadString(completeUri);
                herosClass.HeroesObject ourResponse = JsonConvert.DeserializeObject<herosClass.HeroesObject>(response);
                return ourResponse;
                //foreach (var hero in ourResponse.result.heroes)
                //{
                //    Console.WriteLine("Hero: {0}", hero.name);
                   
                //}
            }
            catch (Exception)
            {

                throw;
            }

        }

        public void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}