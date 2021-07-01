/*
<copyright file="BGGoogleSheetsManager.cs" company="BansheeGz">
    Copyright (c) 2018-2020 All Rights Reserved
</copyright>
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util;

namespace BansheeGz.BGDatabase
{
    public class BGGoogleSheetsManager : IDisposable
    {
        private readonly SheetsService service;
        private readonly string spreadsheetId;
        private readonly Spreadsheet spreadsheet;

        private readonly BGGoogleSheetsBatcher batcher;
        private readonly BGGoogleSheetsQuotaEvaluator quota;
        private readonly BGLogger logger;
        private BGGoogleSheetsLock @lock;

        private bool BatchingEnabled => batcher != null;

        public string SpreadSheetId => spreadsheet.SpreadsheetId;

        public Spreadsheet Spreadsheet => spreadsheet;

        public string SpreadsheetName => spreadsheet.Properties.Title;

        public BGGoogleSheetsQuotaEvaluator Quota => quota;

        public BGGoogleSheetsManager(SheetsService service, string spreadsheetId, BGLogger logger)
        {
            this.service = service;
            this.spreadsheetId = spreadsheetId;
            this.logger = logger;
            batcher = new BGGoogleSheetsBatcher(this, logger);
            quota = new BGGoogleSheetsQuotaEvaluator();
            
            
            quota.ReadOperationRequest();
            logger?.AppendLine("Trying to find the spreadsheet with id=$", spreadsheetId);
            spreadsheet = service.Spreadsheets.Get(spreadsheetId).Execute();
            if (spreadsheet == null) throw new BGException("Can not find spreadsheet with id $", spreadsheetId);
            logger?.AppendLine("Spreadsheet is found! name=$", spreadsheet.Properties.Title);

        }

        public void Dispose()
        {
            try
            {
                if (BatchingEnabled) batcher.Flush();
            }
            finally
            {
                @lock?.ReleaseLock(logger);
            }
        }

        //===================================================================================================
        //                                                BATCH NOT SUPPORTED
        //===================================================================================================
        public void DataUpdate(string sheetName, int column, int row, string value)
        {
            quota.WriteOperationRequest();
            var address = sheetName + "!" + BGGoogleSheetsUtils.ToA1(column) + row;
            var values = new List<IList<object>> {new List<object>()};
            values[0].Add(value);
            var updateRequest = service.Spreadsheets.Values.Update(new ValueRange {Range = address, Values = values}, SpreadSheetId, address);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            updateRequest.Execute();
        }

        public string DataReadCell(string sheetName, int column, int row)
        {
            quota.ReadOperationRequest();
            var address = sheetName + "!" + BGGoogleSheetsUtils.ToA1(column) + row;
            var response = service.Spreadsheets.Values.Get(SpreadSheetId, address).Execute();
            if (response?.Values != null && response.Values.Count > 0)
            {
                var list = response.Values[0];
                if (list != null && list.Count > 0) return list[0].ToString();
            }

            return null;
        }

        public void DataReadRange(string sheetName, int fromColumn, int fromRow, int toColumn, int toRow, Action<int, int, string> action)
        {
            quota.ReadOperationRequest();
            var address = sheetName + "!" + BGGoogleSheetsUtils.ToA1(fromColumn) + fromRow + ":" + BGGoogleSheetsUtils.ToA1(toColumn) + toRow;
            var response = service.Spreadsheets.Values.Get(SpreadSheetId, address).Execute();
            if (response?.Values != null && response.Values.Count > 0)
            {
                for (var i = 0; i < response.Values.Count; i++)
                {
                    var list = response.Values[i];
                    var row = fromRow + i;
                    if (list == null || list.Count == 0) continue;
                    for (var j = 0; j < list.Count; j++)
                    {
                        var val = list[j];
                        var column = fromColumn + j;
                        action(row, column, val == null ? "" : val.ToString());
                    }
                }
            }
        }

        public void BatchDataGet(string[] ranges, bool skipEmptyResult, 
            SpreadsheetsResource.ValuesResource.BatchGetRequest.MajorDimensionEnum majorDimension,
            SpreadsheetsResource.ValuesResource.BatchGetRequest.ValueRenderOptionEnum renderOption,
            Action<int, IList<IList<object>>> callback)
        {
            if (ranges == null || ranges.Length == 0) return;
            
            quota.ReadOperationRequest();
            
            var batchGet = service.Spreadsheets.Values.BatchGet(SpreadSheetId);
            batchGet.Ranges = new Repeatable<string>(ranges);
            batchGet.MajorDimension = majorDimension;
            batchGet.ValueRenderOption = renderOption;
            var response = batchGet.Execute();
            var resultRanges = response.ValueRanges;
            if (resultRanges != null && resultRanges.Count > 0)
            {
                for (var i = 0; i < resultRanges.Count; i++)
                {
                    var resultRange = resultRanges[i];
                    if (skipEmptyResult && (resultRange?.Values == null || resultRange.Values.Count == 0)) continue;
                    callback(i, resultRange?.Values);
                }
            }
        }


        //===================================================================================================
        //                                                BATCH SUPPORTED
        //===================================================================================================
        public void BatchUpdate(bool batchSupported, params Request[] requests)
        {
            if (requests == null || requests.Length == 0) return;
            if (batchSupported && BatchingEnabled)
            {
                batcher.Add(requests);
            }
            else
            {
                quota.WriteOperationRequest();
                var response = service.Spreadsheets.BatchUpdate(new BatchUpdateSpreadsheetRequest {Requests = requests}, SpreadSheetId).Execute();
//            var replies = response.Replies;
//            var count = replies == null ? 0 : replies.Count;
            }
        }


        public void BatchDataUpdate(bool batchSupported, BGGoogleSheetDataBatcher dataBatcher)
        {
            if (dataBatcher==null || !dataBatcher.HasData) return;
            
            if (batchSupported && BatchingEnabled)
            {
                batcher.AddBatchData(dataBatcher);
            }
            else
            {
                quota.WriteOperationRequest();
                var valueRanges = new List<ValueRange>();
                foreach (var batchData in dataBatcher.Data)
                {
                    var range = batchData.ToRange(dataBatcher.SheetName);
                    valueRanges.Add(new ValueRange {Range = range, Values = batchData.ValueLists});
                }
                BatchDataUpdate(valueRanges);
            }

        }

        public void BatchDataUpdate(List<ValueRange> valueRanges)
        {
            if (valueRanges == null || valueRanges.Count == 0) return;
            quota.WriteOperationRequest();
            service.Spreadsheets.Values.BatchUpdate(new BatchUpdateValuesRequest {Data = valueRanges, ValueInputOption = "USER_ENTERED"}, SpreadSheetId).Execute();
        }

        public void Resize(bool batchSupported, int sheetId, int rows, int columns)
        {
            if (batchSupported && BatchingEnabled)
            {
                batcher.AddResize(sheetId, rows, columns);
            }
            else
            {
                quota.WriteOperationRequest();
                service.Spreadsheets.BatchUpdate(new BatchUpdateSpreadsheetRequest {Requests = new List<Request> {GetResizeRequest(sheetId, rows, columns)}}, SpreadSheetId).Execute();
            }
        }

        public void AddSheet(bool batchSupported, string name, int rows, int columns)
        {
            if (batchSupported && BatchingEnabled)
            {
                batcher.AddSheet(name, rows, columns);
            }
            else
            {
                quota.WriteOperationRequest();
                service.Spreadsheets.BatchUpdate(new BatchUpdateSpreadsheetRequest {Requests = new List<Request> {GetAddSheetRequest(name, rows, columns)}}, SpreadSheetId).Execute();
            }
        }


        public void RemoveRows(bool batchSupported, int sheetId, List<int> rows)
        {
            if (rows == null || rows.Count == 0) return;
            if (batchSupported && BatchingEnabled)
            {
                batcher.RemoveRows(sheetId, rows);
            }
            else
            {
                quota.WriteOperationRequest();
                var requestList = new List<Request>();
                var distinct = rows.Distinct().ToList();
                foreach (var row in distinct) requestList.Add(GetRemoveRowsRequest(sheetId, row - 1, row));
                BatchUpdate(true, requestList.ToArray());
            }
        }

        public void DeleteSheets(bool batchSupported, List<int> sheetIds)
        {
            if (sheetIds == null || sheetIds.Count == 0) return;
            if (batchSupported && BatchingEnabled)
            {
                batcher.DeleteSheets(sheetIds);
            }
            else
            {
                quota.WriteOperationRequest();
                var requests = new Request[sheetIds.Count];
                var distinct = sheetIds.Distinct().ToList();
                for (var i = 0; i < distinct.Count; i++) requests[i] = GetDeleteSheetRequest(distinct[i]);
                BatchUpdate(false, requests);
            }
        }

        public void BatchDataUpdate(BGGoogleSheetDataBatcher.DataLayout layout, Action<BGGoogleSheetDataBatcher> action)
        {
//            ((GDataRequestFactory) service.RequestFactory).CustomHeaders.Add(IfMatch);

            var batcher = new BGGoogleSheetDataBatcher(layout);

            action(batcher);
            
            BatchDataUpdate(true, batcher);

//            ((GDataRequestFactory) service.RequestFactory).CustomHeaders.Remove(IfMatch);
        }

        //===================================================================================================
        //                                                Lock
        //===================================================================================================

        public void Lock(BGGoogleSheetsLock @lock)
        {
            this.@lock = @lock;
            @lock?.ObtainLock(logger);
        }

        //===================================================================================================
        //                                                Requests
        //===================================================================================================

        public static Request GetAddSheetRequest(string name, int rows, int columns)
        {
            return new Request
            {
                AddSheet = new AddSheetRequest
                {
                    Properties = new SheetProperties
                    {
                        Title = name,
                        GridProperties = new GridProperties
                        {
                            ColumnCount = columns,
                            RowCount = rows,
                        }
                    }
                }
            };
        }

        public static Request GetRemoveRowsRequest(int sheetId, int fromRow, int toRow)
        {
            return new Request
            {
                DeleteDimension = new DeleteDimensionRequest
                {
                    Range = new DimensionRange
                    {
                        SheetId = sheetId,
                        Dimension = "ROWS",
                        StartIndex = fromRow,
                        EndIndex = toRow,
                    }
                }
            };
        }

        public static Request GetResizeRequest(int sheetId, int rows, int columns)
        {
            return new Request
            {
                UpdateSheetProperties = new UpdateSheetPropertiesRequest
                {
                    Properties = new SheetProperties
                    {
                        SheetId = sheetId, GridProperties = new GridProperties
                        {
                            ColumnCount = columns,
                            RowCount = rows
                        }
                    },
                    Fields = "GridProperties.ColumnCount,GridProperties.RowCount"
                }
            };
        }

        public static Request GetDeleteSheetRequest(int sheetId)
        {
            return new Request
            {
                DeleteSheet = new DeleteSheetRequest
                {
                    SheetId = sheetId
                }
            };
        }

        public void BatchRowSwap(int sheetId, int index1, int index2)
        {
            if (index1 == index2) return;
            int from, to;
            if (index1 < index2)
            {
                from = index1;
                to = index2;
            }
            else
            {
                from = index2;
                to = index1;
            }

            var neighbours = Math.Abs(to - @from) == 1;
            var requests = neighbours ? new Request[1] :new Request[2];

            requests[0] = new Request
            {
                MoveDimension = new MoveDimensionRequest
                {
                    Source = new DimensionRange
                    {
                        SheetId = sheetId,
                        Dimension = "ROWS",
                        StartIndex = @from,
                        EndIndex = @from + 1,
                    },
                    DestinationIndex = to + 1
                }
            };
            if (!neighbours)
            {
                requests[1] = new Request
                {
                    MoveDimension = new MoveDimensionRequest
                    {
                        Source = new DimensionRange
                        {
                            SheetId = sheetId,
                            Dimension = "ROWS",
                            StartIndex = to -1,
                            EndIndex = to,
                        },
                        DestinationIndex = @from
                    }
                };
            }
            batcher.Add(requests);
        }
    }
}