/*
<copyright file="BGGoogleSheetsWriterEntity.cs" company="BansheeGz">
    Copyright (c) 2018-2020 All Rights Reserved
</copyright>
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace BansheeGz.BGDatabase
{
    public partial class BGGoogleSheetsWriterEntity
    {
        private static readonly HashSet<Type> SpecialFields = new HashSet<Type>
        {
            typeof(BGFieldString),
            typeof(BGFieldText),
            typeof(BGFieldListString),


            typeof(BGFieldLong),
            typeof(BGFieldLongNullable),
            typeof(BGFieldListLong),

            typeof(BGFieldDouble),
            typeof(BGFieldDoubleNullable),
            typeof(BGFieldListDouble),

            typeof(BGFieldDecimal),

            typeof(BGFieldHashtable),

            typeof(BGFieldRelationSingle),
            typeof(BGFieldRelationMultiple),
            typeof(BGFieldRelationMultiple),
            typeof(BGFieldId),
            typeof(BGFieldReferenceToEntityGo),
            typeof(BGFieldReferenceToEntityGoList),
            typeof(BGFieldReferenceToUnityObject),
            typeof(BGFieldReferenceToUnityObjectList),
            
            typeof(BGFieldFloat),
        };


        private static readonly List<BGId> IdList = new List<BGId>();
        protected readonly BGGoogleSheetsManager manager;
        protected readonly BGSyncNameMapConfig nameMapConfig;

        public BGGoogleSheetsWriterEntity(BGGoogleSheetsManager manager, BGSyncNameMapConfig nameMapConfig = null)
        {
            this.manager = manager;
            this.nameMapConfig = nameMapConfig;
        }

        public void Write(BGLogger logger, BGGoogleSheetsReader reader, BGRepo repo, BGMergeSettingsEntity settings, BGBookInfo info, BGBookInfo readerInfo, bool transferRowsOrder, BGRepo sourceRepo)
        {
            logger.Section("Writing entities", () =>
            {
                info.ForEachEntitySheet(writerInfo =>
                {
                    var metaId = writerInfo.MetaId;
                    var meta = repo.GetMeta(metaId);

                    logger.SubSection(() =>
                    {
                        var metaExists = readerInfo.HasEntitySheet(metaId);

                        Sheet sheet;
                        if (metaExists)
                        {
                            logger.AppendLine("Sheet for $ meta is found.", meta.Name);

                            //meta exists
                            sheet = reader.GetSheet(metaId);
                            var readerEntityInfo = readerInfo.GetEntitySheet(metaId);

                            //=====================================================================================================
                            //                             STEP 1 : Merge fields
                            //=====================================================================================================
                            IdList.Clear();
                            var maximumColumn = readerEntityInfo.PhysicalColumnCount;

                            if (readerEntityInfo.HasId) writerInfo.IndexId = readerEntityInfo.IndexId;
                            else writerInfo.IndexId = ++maximumColumn;

                            writerInfo.FieldIds.ForEach(fieldId =>
                            {
                                if (readerEntityInfo.HasField(fieldId)) writerInfo.SetField(fieldId, readerEntityInfo.GetFieldColumn(fieldId));
                                else
                                {
                                    writerInfo.SetField(fieldId, ++maximumColumn);
                                    IdList.Add(fieldId);
                                }
                            });

                            //=====================================================================================================
                            //                             STEP 2 : Merge entities
                            //=====================================================================================================
                            var maximumRow = readerEntityInfo.PhysicalRowCount;
                            var isAddingMissing = settings.IsAddingMissing(metaId);
                            var newCount = 0;
                            var existingCount = 0;
                            writerInfo.EntityIds.ForEach(entityId =>
                            {
                                if (readerEntityInfo.HasRow(entityId))
                                {
                                    writerInfo.SetEntity(entityId, readerEntityInfo.GetRow(entityId));
                                    existingCount++;
                                }
                                else
                                {
                                    if (isAddingMissing)
                                    {
                                        newCount++;
                                        writerInfo.SetEntity(entityId, ++maximumRow);
                                    }
                                }
                            });

                            //=====================================================================================================
                            //                             STEP 3 : Resize if needed
                            //=====================================================================================================
                            //resize sheet if needed
                            if (readerEntityInfo.PhysicalColumnCount != maximumColumn || readerEntityInfo.PhysicalRowCount != maximumRow)
                            {
                                logger.AppendLine("Resizing sheet dimension from ($,$) to ($,$). (rows, columns)", readerEntityInfo.PhysicalRowCount, readerEntityInfo.PhysicalColumnCount, maximumRow,
                                    maximumColumn);
                                manager.Resize(true, sheet.Properties.SheetId.Value, maximumRow, maximumColumn);
//                                sheet.Rows = (uint) maximumRow;
//                                sheet.Cols = (uint) maximumColumn;
//                                sheet.Update();
                            }

                            //=====================================================================================================
                            //                             STEP 4 : Update (fields+entities)
                            //=====================================================================================================
                            //update headers
                            UpdateHeaders(sheet.Properties.Title, writerInfo, meta, !readerEntityInfo.HasId, IdList);
                            IdList.Clear();

                            //update entities
                            UpdateEntities(sheet.Properties.Title, writerInfo, meta);

                            //sort
                            if (transferRowsOrder)
                            {
                                var sourceMeta = sourceRepo.GetMeta(meta.Id);
                                if (sourceMeta != null)
                                {
                                    var rowOrder = new BGRowsOrder(logger, meta, (i1, i2) => Swap(sheet, i1, i2));
                                    meta.ForEachEntity(entity =>
                                    {
                                        var sourceEntity = sourceMeta.GetEntity(entity.Id);
                                        if (sourceEntity == null) return;
                                        var rowIndex = writerInfo.GetRow(entity.Id);
                                        if (rowIndex == -1) return;

                                        rowOrder.Add(new BGRowsOrder.EntityOrderInfo(sourceEntity, entity, rowIndex - 1));
                                    });
                                    rowOrder.Complete(null);
                                }
                            }

                            logger.AppendLine("Entities written. # new entities and $ existing are updated", newCount, existingCount);
                            //remove
                            if (settings.IsRemovingOrphaned(metaId))
                            {
                                RemoveById(logger, readerEntityInfo, writerInfo, sheet, readerEntityInfo.IndexId);
                            }
                        }
                        else
                        {
                            //meta does not exists
                            if (logger.AppendLine(!settings.IsAddingMissing(metaId), "Sheet for $ meta is not found and settings does not allow to add missing entities, so meta is skipped.",
                                meta.Name)) return;

                            logger.AppendLine("Sheet for $ meta is not found. Creating a new sheet", meta.Name);

                            //number of fields + _id column
                            var columns = writerInfo.FieldsCount + 1;
                            //number of rows + header (field names)
                            var rows = meta.CountEntities + 1;

                            var sheetName = nameMapConfig == null ? meta.Name : nameMapConfig.GetName(meta);
                            manager.AddSheet(true, sheetName, rows, columns); //new WorksheetEntry(rows, columns, meta.Name);
//                            sheet = reader.SpreadSheet.Worksheets.Insert(sheet);

                            var index = 1;
                            writerInfo.IndexId = index++;
                            IdList.Clear();
                            meta.ForEachField(field =>
                            {
                                writerInfo.SetField(field.Id, index++);
                                IdList.Add(field.Id);
                            });
                            var j = 2;
                            meta.ForEachEntity(entity => writerInfo.SetEntity(entity.Id, j++));

                            //update headers
                            UpdateHeaders(sheetName, writerInfo, meta, true, IdList);
                            IdList.Clear();

                            //update entities
                            UpdateEntities(sheetName, writerInfo, meta);

                            logger.AppendLine("$ entities written.", j - 2);
                        }
                    }, "Writing $ meta", meta.Name);
                });
            });
        }

        private void Swap(Sheet sheet, int index1, int index2)
        {
            manager.BatchRowSwap(sheet.Properties.SheetId.Value, index1, index2);
        }

        private void UpdateHeaders(string sheetName, BGEntitySheetInfo info, BGMetaEntity meta, bool updateId, List<BGId> newFields)
        {
            if (!updateId && newFields.Count == 0) return;

            var columns = new List<int>();
            if (updateId) columns.Add(info.IndexId);
            newFields.ForEach(fieldId => columns.Add(info.GetFieldColumn(fieldId)));

            manager.BatchDataUpdate(new BGGoogleSheetDataBatcher.DataLayout(sheetName, BGGoogleSheetsUtils.FindContinuousRanges(columns), new List<Tuple<int, int>> {new Tuple<int, int>(1, 1)}),
                batcher =>
                {
                    if (updateId) batcher.Add(1, info.IndexId, "_id");
                    if (newFields.Count > 0)
                    {
                        newFields.ForEach(fieldId =>
                        {
                            var field = meta.GetField(fieldId);
                            batcher.Add(1, info.GetFieldColumn(fieldId), nameMapConfig == null ? field.Name : nameMapConfig.GetName(field));
                        });
                    }
                });
        }

        private void UpdateEntities(string sheetName, BGEntitySheetInfo info, BGMetaEntity meta)
        {
            if (info.RowCount == 0) return;

            var columns = new List<int> {info.IndexId};
            info.ForEachField((fieldId, i) => columns.Add(i));
            var rows = new List<int>();
            info.ForEachRow((entityId, row) => rows.Add(row));

            manager.BatchDataUpdate(new BGGoogleSheetDataBatcher.DataLayout(sheetName, BGGoogleSheetsUtils.FindContinuousRanges(columns), BGGoogleSheetsUtils.FindContinuousRanges(rows)), batcher =>
            {
                info.ForEachRow((entityId, row) =>
                {
                    var entity = meta[entityId];
                    batcher.Add(row, info.IndexId, BGGoogleSheetsUtils.IdToString(entity.Id));
                    info.ForEachField((fieldId, i) => batcher.Add(row, i, ToString(meta.GetField(fieldId), entityId, entity.Index)));
                });
            });
        }

        private string ToString(BGField field, BGId entityId, int entityIndex)
        {
            //=================== trying to solve possible issues with Google Sheets (GS)
            string val;
            if (field.CustomStringFormatSupported)
            {
                val = field.ToCustomString(entityIndex);
            }
            else
            {
                //default format

                val = field.ToString(entityIndex);

                if (SpecialFields.Contains(field.GetType()))
                {
                    //GS can not store numbers with more than 15 digits
                    //double needs 17 digits and long needs even more to keep value intact, so we store it as string with D/L prefix

                    //-------- double
                    if (field is BGFieldFloat)
                    {
                        var floatValue = ((BGFieldFloat) field)[entityIndex];
                        return floatValue.ToString("g7", CultureInfo.InvariantCulture);
                    }
                    else if (field.ValueType == typeof(double))
                    {
                        return '\'' + val;
                    }
                    else if (field.ValueType == typeof(double?))
                    {
                        if (!string.IsNullOrEmpty(val)) return '\'' + val;
                        return val;
                    }

                    else if (field is BGFieldListDouble)
                    {
                        var fieldTyped = (BGFieldListDouble) field;
                        if (fieldTyped.CountValues(entityIndex) == 1) return '\'' + val;
                    }

                    //-------- decimal
                    if (field.ValueType == typeof(decimal))
                    {
                        return '\'' + val;
                    }

                    //-------- long
                    else if (field.ValueType == typeof(long))
                    {
                        return '\'' + val;
                    }
                    else if (field.ValueType == typeof(long?))
                    {
                        if (!string.IsNullOrEmpty(val)) return '\'' + val;
                        return val;
                    }
                    else if (field is BGFieldListLong)
                    {
                        var fieldTyped = (BGFieldListLong) field;
                        if (fieldTyped.CountValues(entityIndex) == 1)
                        {
                            return '\'' + val;
                        }
                    }

                    else if (field is BGRelationI
                             || field is BGFieldId
                             || field is BGFieldReferenceToEntityGo
                             || field is BGFieldReferenceToUnityObject
                             || field is BGFieldReferenceToEntityGoList
                             || field is BGFieldReferenceToUnityObjectList)
                    {
                        if (!string.IsNullOrEmpty(val) && val[0] == '+')
                        {
                            return '\'' + val;
                        }
                    }
                    else if (field is BGFieldStringA || field is BGFieldListString)
                    {
                        if (!string.IsNullOrEmpty(val))
                        {
                            if (val[0] == '+') return '\'' + val;
                            if (val[0] == '\'') return '\'' + val;
                        }
                    }
                    else if (field is BGFieldHashtable)
                    {
                        if (!string.IsNullOrEmpty(val)) return '\'' + val;
                    }
                }
                else if (BGLocalizationUglyHacks.GoogleSheetsHasField(field.GetType().FullName))
                {
                    //move this to localization addon!
                    if (!string.IsNullOrEmpty(val) && val[0] == '+')
                    {
                        return '\'' + val;
                    }
                }
            }

            return val;
        }

        public void UpdateIds(BGLogger logger, List<BGGoogleSheetsWriter.IdUpdates> updates)
        {
            if (updates == null || updates.Count == 0) return;

            logger.Section("Updating ids", () =>
            {
                logger.AppendLine("Number of metas with new rows=$", updates.Count);
                foreach (var update in updates)
                {
                    if (update?.List == null || update.List.Count == 0)
                    {
                        logger.AppendLine("No new ids for meta $. skipping", update.Sheet.Properties.Title);
                        continue;
                    }

                    var idColumn = update.IdColumn;
                    var updateSheet = update.Sheet;
                    if (update.IdColumn < 0)
                    {
                        //add id column
                        logger.AppendLine("No _id column for meta $. Creating new _id column", updateSheet.Properties.Title);
                        var columnCount = updateSheet.Properties.GridProperties.ColumnCount.Value;
                        var rowCount = updateSheet.Properties.GridProperties.RowCount.Value;
                        idColumn = columnCount + 1;
                        manager.Resize(true, updateSheet.Properties.SheetId.Value, rowCount, columnCount + 1);
                    }

                    logger.AppendLine("Updating new ids for meta $. Number of new rows=$", updateSheet.Properties.Title, update.List.Count);

                    var rows = new List<int>();
                    if (idColumn != update.IdColumn) rows.Add(1);
                    update.List.ForEach(data => rows.Add(data.Row));

                    manager.BatchDataUpdate(new BGGoogleSheetDataBatcher.DataLayout(
                        updateSheet.Properties.Title,
                        new List<Tuple<int, int>> {new Tuple<int, int>(idColumn, idColumn)},
                        BGGoogleSheetsUtils.FindContinuousRanges(rows)
                    ), batcher =>
                    {
                        //update id header if not exists
                        if (idColumn != update.IdColumn) batcher.Add(1, idColumn, "_id");

                        update.List.ForEach(data => batcher.Add(data.Row, idColumn, BGGoogleSheetsUtils.IdToString(data.Id)));
                    });
                }
            });
        }

        private void RemoveById(BGLogger logger, BGSheetInfoA readerInfo, BGSheetInfoA writerInfo, Sheet sheet, int idIndex)
        {
            if (idIndex < 0) return;

            var rows = new List<int>();
            readerInfo.ForEachRow((id, row) =>
            {
                if (writerInfo.HasRow(id)) return;
                rows.Add(row);
            });

            if (rows.Count <= 0) return;

            rows.Sort((a, b) => -1 * a.CompareTo(b));

            manager.RemoveRows(true, sheet.Properties.SheetId.Value, rows);

            logger.AppendLine("Orphaned rows request submitted. number of rows to remove=$", rows.Count);
        }
    }
}