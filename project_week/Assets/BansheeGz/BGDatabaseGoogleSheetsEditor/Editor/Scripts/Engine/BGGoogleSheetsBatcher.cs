/*
<copyright file="BGGoogleSheetsBatcher.cs" company="BansheeGz">
    Copyright (c) 2018-2020 All Rights Reserved
</copyright>
*/

using System.Collections.Generic;
using Google.Apis.Sheets.v4.Data;

namespace BansheeGz.BGDatabase
{
    public class BGGoogleSheetsBatcher
    {
        private readonly BGGoogleSheetsManager manager;

        private readonly List<AddSheetRequest> addSheetRequests = new List<AddSheetRequest>();
        private readonly List<int> deleteSheetIds = new List<int>();
        private readonly List<ResizeSheetRequest> resizeSheetRequests = new List<ResizeSheetRequest>();
        private readonly List<Request> requests = new List<Request>();
        private readonly List<BGGoogleSheetDataBatcher> batches = new List<BGGoogleSheetDataBatcher>();
        private readonly Dictionary<int, List<int>> sheetId2RowsToRemove = new Dictionary<int, List<int>>();

        private readonly BGLogger logger;

        private bool AnyValue => addSheetRequests.Count > 0
                                 || deleteSheetIds.Count > 0
                                 || resizeSheetRequests.Count > 0
                                 || requests.Count > 0
                                 || batches.Count > 0
                                 || sheetId2RowsToRemove.Count > 0;


        public BGGoogleSheetsBatcher(BGGoogleSheetsManager manager, BGLogger logger)
        {
            this.manager = manager;
            this.logger = logger ?? new BGLogger(false);
        }

        public void Flush()
        {
            if (!AnyValue) return;
            logger.Section("Flushing batches..", () =>
            {
                var batchUpdateRequests = new List<Request>();

                //=======================================   Update meta
                var addSheetCount = addSheetRequests.Count;
                if (addSheetCount > 0)
                {
                    foreach (var addSheetRequest in addSheetRequests) batchUpdateRequests.Add(BGGoogleSheetsManager.GetAddSheetRequest(addSheetRequest.name, addSheetRequest.rows, addSheetRequest.columns));
                    addSheetRequests.Clear();
                }

                var deleteSheetsCount = deleteSheetIds.Count;
                if (deleteSheetsCount > 0)
                {
                    foreach (var deleteSheetId in deleteSheetIds) batchUpdateRequests.Add(BGGoogleSheetsManager.GetDeleteSheetRequest(deleteSheetId));
                    deleteSheetIds.Clear();
                }

                var resizeSheets = resizeSheetRequests.Count;
                if (resizeSheets > 0)
                {
                    foreach (var resizeSheet in resizeSheetRequests) batchUpdateRequests.Add(BGGoogleSheetsManager.GetResizeRequest(resizeSheet.sheetId, resizeSheet.rows, resizeSheet.columns));
                    resizeSheetRequests.Clear();
                }
                
                if (batchUpdateRequests.Count > 0)
                {
                    manager.BatchUpdate(false, batchUpdateRequests.ToArray());
                }


                //=======================================   Update data
                var dataBatchesCount = batches.Count;
                var valueRangesCount = 0;
                var cellsCount = 0;
                if (dataBatchesCount > 0)
                {
                    var list = new BGGoogleSheetBatchDataProvider(batches).Provide(out cellsCount);
                    foreach (var valueRanges in list)
                    {
                        valueRangesCount = valueRanges.Count; 
                        manager.BatchDataUpdate(valueRanges);
                    }
                    batches.Clear();
                }

                //=======================================   generic requests (sorting/deletes)
                var requestsCount = requests.Count;
                var genericRequests = new List<Request>();
                if (requestsCount > 0)
                {
                    genericRequests.AddRange(requests);
                    requests.Clear();
                }

                if (genericRequests.Count > 0)
                {
                    manager.BatchUpdate(false, genericRequests.ToArray());
                }

                
                //=======================================   Remove should be the last!
                var deleteRowsSheetsCount = sheetId2RowsToRemove.Count;
                var removeRangesCount = 0;
                var deletedRowsCount = 0;
                if (deleteRowsSheetsCount > 0)
                {
                    var ranges = new BGGoogleSheetRowsProvider(sheetId2RowsToRemove).Provide(out deletedRowsCount);
                    removeRangesCount = ranges.Length;
                    manager.BatchUpdate(false, ranges);
                    sheetId2RowsToRemove.Clear();
                }

                logger.AppendLine("Number of add sheets=$", addSheetCount);
                logger.AppendLine("Number of delete sheets=$", deleteSheetsCount);
                logger.AppendLine("Number of resize sheets=$", resizeSheets);
                logger.AppendLine("Number of generic requests=$", requestsCount);
                logger.AppendLine("Number of data update batches=$ with $ value ranges and $ cells updated", dataBatchesCount, valueRangesCount, cellsCount);
                logger.AppendLine("Number of sheets with remove rows=$ with $ value ranges and $ rows deleted", deleteRowsSheetsCount, removeRangesCount, deletedRowsCount);
            });
        }


/*
        public void Clear()
        {
            requests.Clear();
            sheetId2RowsToRemove.Clear();
            addSheetRequests.Clear();
            resizeSheetRequests.Clear();
            deleteSheetIds.Clear();
            batches.Clear();
        }
*/

        public void Add(Request[] requests)
        {
            if (requests == null || requests.Length == 0) return;
            this.requests.AddRange(requests);
        }

        public void RemoveRows(int sheetId, List<int> rows)
        {
            List<int> myRows;
            if (!sheetId2RowsToRemove.TryGetValue(sheetId, out myRows))
            {
                myRows = new List<int>();
                sheetId2RowsToRemove.Add(sheetId, myRows);
            }

            myRows.AddRange(rows);
        }

        public void AddSheet(string name, int rows, int columns)
        {
            addSheetRequests.Add(new AddSheetRequest(name, rows, columns));
        }

        public void AddResize(int sheetId, int rows, int columns)
        {
            resizeSheetRequests.Add(new ResizeSheetRequest(sheetId, rows, columns));
        }

        public void DeleteSheets(List<int> sheetIds)
        {
            deleteSheetIds.AddRange(sheetIds);
        }

        public void AddBatchData(BGGoogleSheetDataBatcher data)
        {
            if (data == null || !data.HasData) return;
            batches.Add(data);
        }

        private struct AddSheetRequest
        {
            public string name;
            public int rows;
            public int columns;

            public AddSheetRequest(string name, int rows, int columns)
            {
                this.name = name;
                this.rows = rows;
                this.columns = columns;
            }
        }

        private struct ResizeSheetRequest
        {
            public int sheetId;
            public int rows;
            public int columns;

            public ResizeSheetRequest(int sheetId, int rows, int columns)
            {
                this.sheetId = sheetId;
                this.rows = rows;
                this.columns = columns;
            }
        }
    }
}