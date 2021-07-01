/*
<copyright file="BGGoogleSheetDataBatcher.cs" company="BansheeGz">
    Copyright (c) 2018-2020 All Rights Reserved
</copyright>
*/

using System;
using System.Collections;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase
{
    public class BGGoogleSheetDataBatcher
    {
        private readonly List<BatchData> data = new List<BatchData>();
        private readonly DataLayout layout;

        public List<BatchData> Data => data;

        public DataLayout Layout => layout;

        public bool HasData => data.Count > 0;

        public string SheetName => layout.SheetName;


        internal BGGoogleSheetDataBatcher(DataLayout dataLayout)
        {
            this.layout = dataLayout;
        }


        public void Add(int row, int column, string value)
        {
            data.Add(new BatchData(row, column, value));
        }

        
        
        public class DataLayout
        {
            private readonly string sheetName;

            private readonly List<Tuple<int, int>> columns;
            private readonly List<Tuple<int, int>> rows;

            public string SheetName => sheetName;

            public List<Tuple<int, int>> Columns => columns;

            public List<Tuple<int, int>> Rows => rows;

            public DataLayout(string sheetName, List<Tuple<int, int>> columns, List<Tuple<int, int>> rows)
            {
                this.sheetName = sheetName;
                this.columns = columns;
                this.rows = rows;
                if (columns == null || columns.Count == 0) throw new BGException("columns can not be null or empty");
                if (rows == null || rows.Count == 0) throw new BGException("rows can not be null or empty");
            }
        }

        public class BatchData
        {
            private readonly int row;
            private readonly int column;
            private readonly string value;

            public int Row => row;

            public int Column => column;

            public string Value => value ?? "";

            public IList<IList<object>> ValueLists => new List<IList<object>> {new List<object> {Value}};


            public BatchData(int row, int column, string value)
            {
                this.row = row;
                this.column = column;
                this.value = value;
            }

            public string ToRange(string sheetName)
            {
                return BGGoogleSheetsUtils.ToRange(sheetName, column, row);
            }

            public override string ToString()
            {
                return column + "," + row + "=" + value;
            }
        }
    }
}