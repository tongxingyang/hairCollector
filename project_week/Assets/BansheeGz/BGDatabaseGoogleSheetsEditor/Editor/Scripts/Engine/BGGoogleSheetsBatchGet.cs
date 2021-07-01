/*
<copyright file="BGGoogleSheetsBatchGet.cs" company="BansheeGz">
    Copyright (c) 2018-2020 All Rights Reserved
</copyright>
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Google.Apis.Sheets.v4;

namespace BansheeGz.BGDatabase
{
    public class BGGoogleSheetsBatchGet
    {
        private const int MaxBatchSize = 100000;

        private readonly BGGoogleSheetsManager manager;
        private readonly List<DataRange> dataRanges = new List<DataRange>();

        private long lastRunTime = -1;

        public long LastRunTime => lastRunTime;

        public BGGoogleSheetsBatchGet(BGGoogleSheetsManager manager)
        {
            this.manager = manager;
        }


        public void AddRange(DataRange dataRange)
        {
            dataRanges.Add(dataRange);
        }

        public void Execute(DataMapper dataMapper,
            SpreadsheetsResource.ValuesResource.BatchGetRequest.MajorDimensionEnum dimension,
            SpreadsheetsResource.ValuesResource.BatchGetRequest.ValueRenderOptionEnum valueRender
        )
        {
            if (dataRanges.Count == 0) return;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var rangesOffset = 0;
            var batchCellsCount = 0;
            var rangesString = new List<string>(dataRanges.Count);
            for (var i = 0; i < dataRanges.Count; i++)
            {
                var dataRange = dataRanges[i];
                var delta = dataRange.CellsCount;
                if (delta + batchCellsCount > MaxBatchSize && rangesString.Count > 0)
                {
                    //This is very bad batching logic !!!! 
                    //we could split ranges here- BUT we need to keep ranges indexes intact cause entity dataMapper use DataRange indexes information!!!
                    Execute(dataMapper, dimension, valueRender, rangesString.ToArray(), rangesOffset);
                    rangesOffset = i;
                    rangesString.Clear();
                    batchCellsCount = 0;
                }

                batchCellsCount += delta;
                rangesString.Add(dataRange.A1);
            }

            Execute(dataMapper, dimension, valueRender, rangesString.ToArray(), rangesOffset);

            stopwatch.Stop();
            lastRunTime = stopwatch.ElapsedMilliseconds;
        }

        private void Execute(DataMapper dataMapper,
            SpreadsheetsResource.ValuesResource.BatchGetRequest.MajorDimensionEnum dimension,
            SpreadsheetsResource.ValuesResource.BatchGetRequest.ValueRenderOptionEnum valueRender,
            string[] rangesString,
            int rangesOffset
        )
        {
            if (rangesString==null || rangesString.Length == 0) return;
            manager.BatchDataGet(rangesString, false,
                dimension,
                valueRender,
                (rangeIndex, list) =>
                {
                    var realRangeIndex = rangesOffset + rangeIndex; 
                    var dataRange = dataRanges[realRangeIndex];
                    dataMapper.RangeStart(realRangeIndex);
                    if (list != null)
                    {
                        for (var i = 0; i < list.Count; i++)
                        {
                            var innerList = list[i];
                            if (innerList == null) continue;
                            
                            for (var j = 0; j < innerList.Count; j++)
                            {
                                var inner = innerList[j];
                                var value = inner?.ToString();
                                int column, row;
                                switch (dimension)
                                {
                                    case SpreadsheetsResource.ValuesResource.BatchGetRequest.MajorDimensionEnum.DIMENSIONUNSPECIFIED:
                                    case SpreadsheetsResource.ValuesResource.BatchGetRequest.MajorDimensionEnum.ROWS:
                                        column = dataRange.FromColumn + j;
                                        row = dataRange.FromRow + i;
                                        break;
                                    case SpreadsheetsResource.ValuesResource.BatchGetRequest.MajorDimensionEnum.COLUMNS:
                                        column = dataRange.FromColumn + i;
                                        row = dataRange.FromRow + j;
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException(nameof(dimension), dimension, null);
                                }
                                dataMapper.ProcessCell(realRangeIndex, column, row, value);
                            }
                        }
                    }

                    dataMapper.RangeEnd(realRangeIndex);
                });
        }


        public abstract class DataMapper
        {
            public abstract void ProcessCell(int rangeIndex, int column, int row, string cellValue);

            public virtual void RangeStart(int rangeIndex)
            {
            }

            public virtual void RangeEnd(int rangeIndex)
            {
            }
        }

        public sealed class DataRange
        {
            public readonly string SheetName;
            public readonly int FromColumn;
            public readonly int ToColumn;
            public readonly int FromRow;
            public readonly int ToRow;

            public int CellsCount => (ToColumn - FromColumn + 1) * (ToRow - FromRow + 1);
            public string A1 => BGGoogleSheetsUtils.ToRange(SheetName, FromColumn, FromRow, ToColumn, ToRow);

            public DataRange(string sheetName, int fromColumn, int toColumn, int fromRow, int toRow)
            {
                SheetName = sheetName;
                FromColumn = fromColumn;
                ToColumn = toColumn;
                FromRow = fromRow;
                ToRow = toRow;
            }

            public override string ToString()
            {
                return A1;
            }
        }
    }
}