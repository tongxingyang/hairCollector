/*
<copyright file="BGGoogleSheetsDeleteSheets.cs" company="BansheeGz">
    Copyright (c) 2018-2020 All Rights Reserved
</copyright>
*/

using System.Collections.Generic;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace BansheeGz.BGDatabase
{
    public class BGGoogleSheetsDeleteSheets 
    {
        private readonly BGGoogleSheetsManager manager;
        private readonly bool batchSuported;
        private List<int> sheetIds;

        public BGGoogleSheetsDeleteSheets(BGGoogleSheetsManager manager, bool batchSuported)
        {
            this.manager = manager;
            this.batchSuported = batchSuported;
        }

/*
        public BGGHDeleteSheets(BGGoogleManager manager, params int[] sheetIds) : this(manager)
        {
            if (sheetIds != null)
            {
                foreach (var sheet in sheetIds) Add(sheet);
            }
        }
*/

        public void Add(int sheetId)
        {
            sheetIds = sheetIds ?? new List<int>();
            sheetIds.Add(sheetId);
        }

        public void Execute()
        {
            if (sheetIds == null || sheetIds.Count == 0) return;

            manager.DeleteSheets(batchSuported, sheetIds);
        }
    }
}