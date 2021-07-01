/*
<copyright file="BGGoogleSheetRowsProvider.cs" company="BansheeGz">
    Copyright (c) 2018-2020 All Rights Reserved
</copyright>
*/

using System.Collections.Generic;
using System.Linq;
using Google.Apis.Sheets.v4.Data;

namespace BansheeGz.BGDatabase
{
    public class BGGoogleSheetRowsProvider
    {
        private readonly Dictionary<int, List<int>> sheetId2RowsToRemove;

        public BGGoogleSheetRowsProvider(Dictionary<int, List<int>> sheetId2RowsToRemove)
        {
            this.sheetId2RowsToRemove = sheetId2RowsToRemove;
        }

        public Request[] Provide(out int deletedRowsCount)
        {
            deletedRowsCount = 0;
            var requestList = new List<Request>();
            foreach (var pair in sheetId2RowsToRemove)
            {
                var sheetId = pair.Key;
                var list = pair.Value;
                if (list == null || list.Count == 0) continue;

                var distinct = list.Distinct().OrderByDescending(i => i).ToList();
                var toRow = distinct[0];
                var num = 1;
                for (var i = 1; i < distinct.Count; i++)
                {
                    var current = distinct[i];
                    if (toRow - current == num)
                    {
                        num++;
                    }
                    else
                    {
                        requestList.Add(BGGoogleSheetsManager.GetRemoveRowsRequest(sheetId, toRow - num, toRow));
                        deletedRowsCount += num;
                        toRow = current;
                        num = 1;
                    }
                }

                requestList.Add(BGGoogleSheetsManager.GetRemoveRowsRequest(sheetId, toRow - num, toRow));
                deletedRowsCount += num;
            }

            return requestList.ToArray();
        }
    }
}