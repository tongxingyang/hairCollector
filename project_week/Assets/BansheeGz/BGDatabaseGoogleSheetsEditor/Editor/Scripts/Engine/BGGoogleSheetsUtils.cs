/*
<copyright file="BGGoogleSheetsUtils.cs" company="BansheeGz">
    Copyright (c) 2018-2020 All Rights Reserved
</copyright>
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BansheeGz.BGDatabase
{
    public static class BGGoogleSheetsUtils
    {
        // Original from Apache POI https://github.com/cuba-platform/apache-poi/blob/master/poi/src/java/org/apache/poi/ss/util/CellReference.java convertNumToColString method
        public static string ToA1(int index)
        {
            var excelColNum = index;

            var colRef = "";
            var colRemain = excelColNum;

            while (colRemain > 0)
            {
                var thisPart = colRemain % 26;
                if (thisPart == 0) { thisPart = 26; }

                colRemain = (colRemain - thisPart) / 26;

                // The letter A is at 65
                var colChar = (char) (thisPart + 64);
                colRef = colChar + colRef;
            }

            return colRef;
        }

        public static List<Tuple<int, int>> FindContinuousRanges(ICollection<int> values)
        {
            var result = new List<Tuple<int, int>>();
            if (values == null || values.Count == 0) return result;

            var sorted = values.Distinct().OrderBy(i => i).ToList();
            var start = sorted[0];
            var num = 1;
            for (var i = 1; i < sorted.Count; i++)
            {
                var current = sorted[i];
                if (current - start == num)
                {
                    num++;
                }
                else
                {
                    result.Add(new Tuple<int, int>(start, start + num - 1)); 
                    start = current;
                    num = 1;
                }
            }
            result.Add(new Tuple<int, int>(start, start + num - 1)); 
            
            
            return result;
        }

        public static string ToRange(string sheetName, int fromColumn, int fromRow, int toColumn, int toRow)
        {
            return sheetName + '!' + ToA1(fromColumn) + fromRow + ':' + ToA1(toColumn)  + toRow;
        }
        
        public static string ToRange(string sheetName, string fromColumn, int fromRow, string toColumn, int toRow)
        {
            return sheetName + '!' + fromColumn + fromRow + ':' + toColumn  + toRow;
        }

        public static string ToRange(string sheetName, int column, int row)
        {
            return   sheetName + '!' + (BGGoogleSheetsUtils.ToA1(column) + row);
        }
        
        public static BGId ReadId(string value, bool throwException = false)
        {
            if (String.IsNullOrEmpty(value)) return BGId.Empty;
            try
            {
                if (value.Length == 22) return new BGId(value);

                if (value.Length == 23)
                {
                    if (value[0] == '=' || value[0] == '\'') return new BGId(value.Substring(1));
                }
            }
            catch(Exception e)
            {
                // ignored
                if (throwException) throw e;
            }

            return BGId.Empty;
        }
        public static BGId ReadId(string value, string inputValue, bool throwException = false)
        {
            if (String.IsNullOrEmpty(value)) return BGId.Empty;
            try
            {
                if (value.Length == 22) return new BGId(value);

                if (inputValue == null) return BGId.Empty;

                if (inputValue.Length == 22) return new BGId(inputValue);
                if (inputValue.Length == 23)
                {
                    if (inputValue[0] == '=' || inputValue[0] == '\'') return new BGId(inputValue.Substring(1));
                }
            }
            catch
            {
                // ignored
            }

            return BGId.Empty;
        }

        public static string IdToString(BGId id)
        {
            if (id.IsEmpty) return "";
            var value = id.ToString();
            if (value[0] == '+') value = '\'' + value;
            return value;
        }

        public static string BoolToString(bool value)
        {
            return value ? "1" : "0";
        }
    }
}