/*
<copyright file="BGGoogleSheetsReaderEntity.cs" company="BansheeGz">
    Copyright (c) 2018-2020 All Rights Reserved
</copyright>
*/

using System;
using System.Collections.Generic;
using Google.Apis.Sheets.v4;

namespace BansheeGz.BGDatabase
{
    public partial class BGGoogleSheetsReaderEntity
    {
        private readonly BGRepo repo;
        private readonly BGGoogleSheetsInfo info;
        private readonly BGGoogleSheetsManager manager;
        private readonly BGLogger logger;

        public BGGoogleSheetsReaderEntity(BGGoogleSheetsManager manager, BGLogger logger, BGGoogleSheetsInfo info, BGRepo repo)
        {
            this.logger = logger;
            this.manager = manager;
            this.repo = repo;
            this.info = info;
        }

        public List<BGGoogleSheetsWriter.IdUpdates> Read(bool updateNewIds)
        {
            var result = updateNewIds ? new List<BGGoogleSheetsWriter.IdUpdates>() : null;

            var batchGet = new BGGoogleSheetsBatchGet(manager);

            var mapper = new EntitiesMapper();
            info.ForEachEntitySheet(sheetInfo =>
            {
                if (logger.AppendWarning(!sheetInfo.HasAnyData, "No data found for $ meta.", sheetInfo.Name)) return;
                if (logger.AppendWarning(sheetInfo.PhysicalRowCount < 2, "No rows with data found for $ meta.", sheetInfo.Name)) return;

                var meta = repo.GetMeta(sheetInfo.MetaId);
                var columns = new List<int>();
                var column2Field = new Dictionary<int, BGField>();
                if (sheetInfo.HasId) columns.Add(sheetInfo.IndexId);
                sheetInfo.ForEachField((id, column) =>
                {
                    columns.Add(column);
                    column2Field[column] = meta.GetField(id);
                });

                var ranges = BGGoogleSheetsUtils.FindContinuousRanges(columns);
                mapper.AddMeta(meta, sheetInfo.IndexId, column2Field, sheetInfo.PhysicalRowCount - 1);
                foreach (var range in ranges)
                {
                    var dataRange = new BGGoogleSheetsBatchGet.DataRange(sheetInfo.SheetName, range.Item1, range.Item2, 2, sheetInfo.PhysicalRowCount + 1);
                    batchGet.AddRange(dataRange);
                    mapper.AddRange();
                }
            });

            mapper.AddingRangesComplete();

            batchGet.Execute(mapper, SpreadsheetsResource.ValuesResource.BatchGetRequest.MajorDimensionEnum.COLUMNS,
                SpreadsheetsResource.ValuesResource.BatchGetRequest.ValueRenderOptionEnum.FORMATTEDVALUE);
            logger.AppendLine("Reading formatted values batch executed in $ milliseconds", batchGet.LastRunTime);


            var formattedValues = new List<string[,]>();
            mapper.ForEach((i, data) => formattedValues.Add(data.ResetValues()));

            batchGet.Execute(mapper, SpreadsheetsResource.ValuesResource.BatchGetRequest.MajorDimensionEnum.COLUMNS,
                SpreadsheetsResource.ValuesResource.BatchGetRequest.ValueRenderOptionEnum.UNFORMATTEDVALUE);

            logger.AppendLine("Reading raw values batch executed in $ milliseconds", batchGet.LastRunTime);

            mapper.ForEach((i, data) =>
            {
                var formattedArray = formattedValues[i];
                var unFormattedArray = data.Values;
                var columnsCount = formattedArray.GetLength(0);
                var rowsCount = formattedArray.GetLength(1);
                var skipped = 0;
                var existingCount = 0;
                var newCount = 0;
                var emptyRows = 0;
                var idUpdates = result != null && data.HasId ? new BGGoogleSheetsWriter.IdUpdates(data.Meta.Name, data.IdColumnA1, info.GetEntry(data.Meta.Id)) : null;

                for (var row = 0; row < rowsCount; row++)
                {
                    var rowA1 = row + 2;
                    var entityId = BGId.Empty;
                    if (data.HasId)
                    {
                        var formatted = formattedArray[data.IdColumn, row];
                        var unFormatted = unFormattedArray[data.IdColumn, row];
                        try
                        {
                            entityId = BGGoogleSheetsUtils.ReadId(formatted, unFormatted, true);
                        }
                        catch
                        {
                            skipped++;
                            logger.AppendWarning("Invalid id $ at row $. Row is skipped", formatted, rowA1);
                            continue;
                        }
                    }

                    //skip row if its empty
                    if (IsRowEmpty(row, formattedArray, unFormattedArray))
                    {
                        emptyRows++;
                        continue;
                    }

                    if (entityId.IsEmpty)
                    {
                        //new object
                        if (data.Fields.Length == 0)
                        {
                            //no fields- no entity
                            skipped++;
                            continue;
                        }

                        newCount++;
                        var entity = data.Meta.NewEntity();
                        ReadFields(data, entity, formattedArray, unFormattedArray, row, columnsCount);
                        var id = entity.Id;
                        idUpdates?.List.Add(new BGGoogleSheetsWriter.IdUpdates.IdData(rowA1, id));
                    }
                    else
                    {
                        //existing
                        existingCount++;
                        if (data.Fields.Length == 0) continue;
                        ReadFields(data, data.Meta.NewEntity(entityId), formattedArray, unFormattedArray, row, columnsCount);
                    }
                }

                if (result != null && idUpdates != null && idUpdates.Count > 0) result.Add(idUpdates);
                logger.AppendLine("Meta $. Found $ existing entities and $ new entities. $ rows skipped. $ empty rows", data.Meta.Name, existingCount, newCount, skipped, emptyRows);
            });

            return result;
        }

        private bool IsRowEmpty(int row, string[,] formattedArray, string[,] unFormattedArray)
        {
            if (!CheckRowEmpty(row, formattedArray)) return false;
            if (!CheckRowEmpty(row, unFormattedArray)) return false;

            return true;
        }

        private static bool CheckRowEmpty(int row, string[,] values)
        {
            var length = values.GetLength(0);
            for (var i = 0; i < length; i++)
            {
                var value = values[i, row];
                if (value != null && value.Trim().Length != 0) return false;
            }

            return true;
        }

        private void ReadFields(MetaData metaData, BGEntity entity, string[,] formattedArray, string[,] unFormattedArray, int row, int columnsCount)
        {
            for (var column = 0; column < columnsCount; column++)
            {
                if (column == metaData.IdColumn) continue;

                var field = metaData.Fields[column];
                if (field == null) throw new BGException("something is wrong- the field is empty, meta $, column=$", metaData.Meta.Name, column);

                var formatted = formattedArray[column, row] ?? "";
                var unFormatted = unFormattedArray[column, row] ?? "";

                var resolveValue = "";
                try
                {
                    if (field.CustomStringFormatSupported)
                    {
                        //we have custom formatter.Lets do this: try to feed it with value, when with inputValue
                        try
                        {
                            field.FromCustomString(entity.Index, formatted);
                        }
                        catch
                        {
                            field.FromCustomString(entity.Index, unFormatted);
                        }
                    }
                    else
                    {
                        resolveValue = ResolveValue(field, formatted, unFormatted);
                        field.FromString(entity.Index, resolveValue);
                    }
                }
                catch (Exception e)
                {
                    logger.AppendWarning("Can not fetch field $ value for entity with id=$. Value=$. Error=$", field.Name, entity.Id, resolveValue, e.Message);
                }
            }
        }

        private string ResolveValue(BGField field, string formattedValue, string unformattedValue)
        {
            if (string.Equals(unformattedValue, formattedValue)) return formattedValue;
            if (unformattedValue == null) return formattedValue;

            var result = formattedValue;
            if (field is BGFieldStringA || field is BGFieldListString)
            {
                //incorrect formula
                if (string.Equals(formattedValue, "#ERROR!") && unformattedValue.StartsWith("="))
                {
                    if (unformattedValue.Length > 1 && (unformattedValue[1] == '+' || unformattedValue[1] == '-')) result = unformattedValue.Substring(1);
                    else result = unformattedValue;
                }
                // NAME error
                else if (string.Equals(formattedValue, "#NAME?"))
                {
                    if (unformattedValue.Length > 1 && (unformattedValue[1] == '+' || unformattedValue[1] == '-')) result = unformattedValue.Substring(1);
                    else result = unformattedValue;
                }
                //starting with '
                else if (unformattedValue.Length == formattedValue.Length + 1 && unformattedValue[0] == '\'') result = unformattedValue.Substring(1);
            }
            else if (
                field is BGFieldLong ||
                field is BGFieldInt ||
                field is BGFieldFloat ||
                field is BGFieldDouble ||
                field is BGFieldDecimal ||
                field is BGFieldLongNullable ||
                field is BGFieldIntNullable ||
                field is BGFieldFloatNullable ||
                field is BGFieldDoubleNullable ||
                field is BGFieldListFloat ||
                field is BGFieldListDouble
            )
            {
                result = ChoseNumericValue(field, unformattedValue, formattedValue);
            }
            else if (field is BGFieldRelationSingle
                     || field is BGFieldId
                     || field is BGFieldReferenceToEntityGo
                     || field is BGFieldReferenceToUnityObject
                     || field is BGFieldReferenceToEntityGoList
                     || field is BGFieldReferenceToUnityObjectList
            )
            {
                //sometimes id display value is broken
                var id = BGGoogleSheetsUtils.ReadId(formattedValue, unformattedValue);
                if (!id.IsEmpty) result = id.ToString();
            }
            else if (field is BGFieldRelationMultiple)
            {
                if (unformattedValue.Length < 25)
                {
                    var id = BGGoogleSheetsUtils.ReadId(formattedValue, unformattedValue);
                    if (!id.IsEmpty) result = id.ToString();
                }
                else if (unformattedValue[0] == '\'')
                {
                    result = unformattedValue.Substring(1);
                }
                else if (unformattedValue.StartsWith("=+"))
                {
                    result = unformattedValue.Substring(1);
                }
            }

            if (field is BGFieldHashtable)
            {
                if (!string.IsNullOrEmpty(unformattedValue) && unformattedValue[0] == '\'')
                {
                    result = unformattedValue.Substring(1);
                }
            }
            else if (BGLocalizationUglyHacks.GoogleSheetsHasField(field.GetType().FullName))
            {
                //move this to localization addon!
                //sometimes id display value is broken
                var id = BGGoogleSheetsUtils.ReadId(formattedValue, unformattedValue);
                if (!id.IsEmpty) result = id.ToString();
            }

            return result;
        }

        private string ChoseNumericValue(BGField field, string unformattedValue, string formattedValue)
        {
            var result = formattedValue;

            //google may use e+ format for numbers, reducing the number of digits
            //this is an old version
//            if (inputValue[0] == '-' || inputValue.IndexOf("E+", StringComparison.Ordinal) != -1)

            // replace input value if first character is '
            if (!string.IsNullOrEmpty(unformattedValue) && unformattedValue[0] == '\'') unformattedValue = unformattedValue.Substring(1);

            var tryExpFormat = !string.IsNullOrEmpty(unformattedValue) && unformattedValue.IndexOf("E+", StringComparison.Ordinal) != -1;

            if (field.ValueType == typeof(int) || field.ValueType == typeof(int?) || field.ValueType == typeof(List<int>))
            {
                if (tryExpFormat && TryParseInt(unformattedValue)) result = unformattedValue;
                else TryParseInt(formattedValue, null, () => TryParseInt(unformattedValue, () => result = unformattedValue));
            }
            else if (field.ValueType == typeof(float) || field.ValueType == typeof(float?) || field.ValueType == typeof(List<float>))
            {
                if (tryExpFormat && TryParseFloat(unformattedValue)) result = unformattedValue;
                else TryParseFloat(formattedValue, null, () => TryParseFloat(unformattedValue, () => result = unformattedValue));
            }
            else if (field.ValueType == typeof(double) || field.ValueType == typeof(double?) || field.ValueType == typeof(List<double>))
            {
                if (tryExpFormat && TryParseDouble(unformattedValue)) result = unformattedValue;
                else TryParseDouble(formattedValue, null, () => TryParseDouble(unformattedValue, () => result = unformattedValue));
            }
            else if (field.ValueType == typeof(decimal))
            {
                if (tryExpFormat && TryParseDecimal(unformattedValue)) result = unformattedValue;
                else TryParseDecimal(formattedValue, null, () => TryParseDecimal(unformattedValue, () => result = unformattedValue));
            }
            else if (field.ValueType == typeof(long) || field.ValueType == typeof(long?) || field.ValueType == typeof(List<long>))
            {
                if (tryExpFormat && TryParseLong(unformattedValue)) result = unformattedValue;
                else TryParseLong(formattedValue, null, () => TryParseLong(unformattedValue, () => result = unformattedValue));
            }

            return result;
        }

        private bool TryParseInt(string value, Action success = null, Action failure = null)
        {
            return Try(() => int.Parse(value), success, failure);
        }

        private bool TryParseFloat(string value, Action success = null, Action failure = null)
        {
            return Try(() => float.Parse(value), success, failure);
        }

        private bool TryParseLong(string value, Action success = null, Action failure = null)
        {
            return Try(() => long.Parse(value), success, failure);
        }

        private bool TryParseDouble(string value, Action success = null, Action failure = null)
        {
            return Try(() => double.Parse(value), success, failure);
        }

        private bool TryParseDecimal(string value, Action success = null, Action failure = null)
        {
            return Try(() => decimal.Parse(value), success, failure);
        }

        private bool Try(Action action, Action success = null, Action failure = null)
        {
            try
            {
                action();
                success?.Invoke();
                return true;
            }
            catch
            {
                failure?.Invoke();
                return false;
            }
        }

        private class EntitiesMapper : BGGoogleSheetsBatchGet.DataMapper
        {
            private readonly List<MetaData> metaList = new List<MetaData>();
            private readonly List<int> rangesList = new List<int>();
            private MetaData[] metaArray;
            private int[] rangesArray;

            public override void ProcessCell(int rangeIndex, int column, int row, string cellValue)
            {
                metaArray[rangesArray[rangeIndex]].SetValue(column, row, cellValue);
            }

            public void AddMeta(BGMetaEntity meta, int idColumn, Dictionary<int, BGField> column2Field, int valuesCount)
            {
                metaList.Add(new MetaData(meta, idColumn, column2Field, valuesCount));
            }

            public void AddRange()
            {
                rangesList.Add(metaList.Count - 1);
            }

            public void AddingRangesComplete()
            {
                metaArray = metaList.ToArray();
                rangesArray = rangesList.ToArray();
            }

            public void ForEach(Action<int, MetaData> action)
            {
                for (var i = 0; i < metaArray.Length; i++) action(i, metaArray[i]);
            }
        }

        private class MetaData
        {
            private readonly BGMetaEntity meta;

            private readonly BGField[] fields;

            //A1 based!!
            private readonly int[] column2Field;
            private string[,] values;

            public int IdColumn { get; }
            public int IdColumnA1 { get; }

            public string[,] Values => values;

            public BGField[] Fields => fields;

            public BGMetaEntity Meta => meta;

            public bool HasId => IdColumn >= 0;

            public MetaData(BGMetaEntity meta, int idColumn, Dictionary<int, BGField> column2Field, int valuesCount)
            {
                this.meta = meta;
                this.IdColumnA1 = idColumn;
                IdColumn = -1;
                
                //======================== field indexes for remapping
                var max = idColumn;
                foreach (var pair in column2Field)
                {
                    if (max < pair.Key) max = pair.Key;
                }

                this.column2Field = new int[max + 1];


                //======================== fields list
                var sortedList = new List<int>();
                if (idColumn > 0) sortedList.Add(idColumn);
                sortedList.AddRange(column2Field.Keys);
                sortedList.Sort();

                fields = new BGField[sortedList.Count];
                for (var i = 0; i < sortedList.Count; i++)
                {
                    var fieldIndex = sortedList[i];
                    this.column2Field[fieldIndex] = i;

                    if (fieldIndex != idColumn) fields[i] = BGUtil.Get(column2Field, fieldIndex);
                    else IdColumn = i;
                }

                //values
                values = new string[fields.Length, valuesCount];
            }

            public string[,] ResetValues()
            {
                var oldValues = values;
                values = new string[oldValues.GetLength(0), oldValues.GetLength(1)];
                return oldValues;
            }


            public void SetValue(int column, int row, string cellValue)
            {
                var fieldIndex = column2Field[column];
                //A1 notation - header row = -2
                values[fieldIndex, row - 2] = cellValue;
            }
        }
    }
}