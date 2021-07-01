/*
<copyright file="BGGoogleSheetBatchDataProvider.cs" company="BansheeGz">
    Copyright (c) 2018-2020 All Rights Reserved
</copyright>
*/

using System;
using System.Collections.Generic;
using Google.Apis.Sheets.v4.Data;
using UnityEngine;

namespace BansheeGz.BGDatabase
{
    public class BGGoogleSheetBatchDataProvider
    {
        private const int MaxBatchSize = 100000;

        private readonly List<BGGoogleSheetDataBatcher> batches;

        public BGGoogleSheetBatchDataProvider(List<BGGoogleSheetDataBatcher> batches)
        {
            this.batches = batches;
        }

        public List<List<ValueRange>> Provide(out int cellsCount)
        {
            cellsCount = 0;
            var result = new List<List<ValueRange>>();
            var batchList = new List<ValueRange>();
            result.Add(batchList);
            var threshold = MaxBatchSize;
            foreach (var batch in batches) Process(result, batchList, batch, ref cellsCount, ref threshold);
            return result;
        }

        private void Process(List<List<ValueRange>> list, List<ValueRange> result, BGGoogleSheetDataBatcher batch, ref int cellsCount, ref int threshold)
        {
            if (batch == null || !batch.HasData) return;

            var layout = batch.Layout;
            var sheetName = layout.SheetName;
            var rows = layout.Rows;
            var columns = layout.Columns;
            var address2Data = Index(batch.Data);
            foreach (var column in columns)
            {
                var fromColumn = column.Item1;
                var toColumn = column.Item2;
                var fromColumnA1 = BGGoogleSheetsUtils.ToA1(fromColumn);
                var toColumnA1 = BGGoogleSheetsUtils.ToA1(toColumn);
                foreach (var row in rows)
                {
                    var fromRow = row.Item1;
                    var toRow = row.Item2;

                    var range = BGGoogleSheetsUtils.ToRange(sheetName, fromColumnA1, fromRow, toColumnA1, toRow);

                    IList<IList<object>> rowsValues = new List<IList<object>>(toRow - fromRow);
                    for (var j = fromRow; j <= toRow; j++)
                    {
                        IList<object> rowValues = new List<object>(toColumn - fromColumn);
                        rowsValues.Add(rowValues);

                        for (var i = fromColumn; i <= toColumn; i++)
                        {
                            string val;
                            if (!address2Data.TryGetValue(new Address(i, j), out val)) throw new Exception("error: problem with index, can not find data");
                            rowValues.Add(val);
                        }
                    }

                    //total cells count
                    var delta = (toRow - fromRow + 1) * (toColumn - fromColumn + 1);
                    cellsCount += delta;

                    if (cellsCount > threshold && result.Count > 0)
                    {
                        threshold = NextThreshold(cellsCount);
                        result = new List<ValueRange>();
                        list.Add(result);
                    }

                    result.Add(new ValueRange
                    {
                        Range = range,
                        Values = rowsValues,
                    });
                }
            }
        }

        private int NextThreshold(int cellsCount)
        {
            return Mathf.CeilToInt(cellsCount / (float) MaxBatchSize) * MaxBatchSize;
        }
        

        private Dictionary<Address, string> Index(List<BGGoogleSheetDataBatcher.BatchData> batchData)
        {
            var result = new Dictionary<Address, string>();
            foreach (var data in batchData) result[new Address(data.Column, data.Row)] = data.Value;
            return result;
        }

        private struct Address
        {
            private readonly int column;
            private readonly int row;

            public Address(int column, int row)
            {
                this.column = column;
                this.row = row;
            }

            public bool Equals(Address other)
            {
                return column == other.column && row == other.row;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is Address && Equals((Address) obj);
            }

            public override int GetHashCode()
            {
                unchecked { return (column * 397) ^ row; }
            }

            public static bool operator ==(Address left, Address right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(Address left, Address right)
            {
                return !left.Equals(right);
            }
        }
    }
}