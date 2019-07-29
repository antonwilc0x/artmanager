// Copyright (c) Anthony Wilcox and contributors. All rights reserved.
// Licensed under the GNU GPL v3 license. See LICENSE file in the project
// root for full license information.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using ArtManager.Models;
using EntryPoint;
using LiteDB;
using Newtonsoft.Json;

namespace ArtManager.CLI
{
    class Order
    {
        Art Art { get; set; }

        public bool IsDebug { get; set; }

        readonly List<Art> _arts = new List<Art>();

        public Order() { }

        public Order(Art order)
        {
            Art = order;
        }

        public void DBInsert()
        {
            try
            {
                using (var db = new LiteDatabase(ArtmConsts.DBFILE))
                {
                    var art = db.GetCollection<Art>(ArtmConsts.DBCOL);
                    art.Insert(Art);
                }
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public void DBRaffle(string[] args)
        {
            var cli = Cli.Parse<RaffleArgs>(args);
            var rand = new Random();
            var tickets = rand.Next(cli.Tickets);
            var slots = rand.Next(cli.Slots);

            if (IsDebug)
            {

            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void DbListAll()
        {
            try
            {
                using (var db = new LiteDatabase(ArtmConsts.DBFILE))
                {
                    var art = db.GetCollection<Art>(ArtmConsts.DBCOL);
                    art.EnsureIndex(x => x.Hash);
                    var query = art.Include(x => x.Hash)
                        .Include(x => x.Custmer)
                        .Include(x => x.Name)
                        .FindAll();

                    foreach (var q in query)
                        _arts.Add(q);

                    var op = new JsonSerializerSettings()
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                    };
                    var json = JsonConvert.SerializeObject(_arts, Formatting.Indented, op);

                    if (IsDebug)
                        Console.WriteLine(json);
                    else if (Debugger.IsAttached)
                        Debug.WriteLine(json);
                }
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }
    }
}