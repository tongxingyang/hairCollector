/*
<copyright file="BGGoogleSheetsReaderInfo.cs" company="BansheeGz">
    Copyright (c) 2018-2020 All Rights Reserved
</copyright>
*/

using System;
using System.Collections.Generic;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using A = BansheeGz.BGDatabase.BGRepoConstants.AddonConstants;
using F = BansheeGz.BGDatabase.BGRepoConstants.FieldConstants;
using M = BansheeGz.BGDatabase.BGRepoConstants.MetaConstants;

namespace BansheeGz.BGDatabase
{
    public partial class BGGoogleSheetsReaderInfo
    {
        private readonly BGGoogleSheetsManager manager;
        private readonly BGLogger logger;
        private readonly BGSyncNameMapConfig nameMapConfig;
        private BGGoogleSheetsInfo info;

        public BGGoogleSheetsReaderInfo(BGLogger logger, BGGoogleSheetsManager manager, BGSyncNameMapConfig nameMapConfig = null)
        {
            this.logger = logger;
            this.manager = manager;
            this.nameMapConfig = nameMapConfig;
        }

        public BGGoogleSheetsInfo GetInfo(BGRepo repo)
        {
            if (info != null) return info;

            logger.Section("Reading spreadsheet structure..", () =>
            {
                info = new BGGoogleSheetsInfo(manager);

                //read info
                var i = 0;
                var sheets = manager.Spreadsheet.Sheets;
                logger.AppendLine("Found $ sheets", sheets?.Count ?? 0);

                if (sheets != null && sheets.Count > 0)
                {
                    //=====================================================================================================
                    //                             STEP 1 : Filter sheets
                    //=====================================================================================================
                    foreach (var sheet in sheets)
                    {
                        var name = sheet.Properties.Title;

                        if (name == BGGoogleSheetsLock.DBLockSheetName)
                        {
                            TryToReadLock(sheet);
                            continue;
                        }

                        var meta = nameMapConfig == null ? repo[name] : nameMapConfig.Map(repo, name);

                        if (logger.AppendWarning(meta == null, "Sheet $ is skipped: no meta found (or not included in settings)", name)) continue;
                        if (logger.AppendWarning(info.HasEntitySheet(meta.Id), "Sheet $ is skipped: duplicate name, sheet with the same name already processed", name)) continue;


                        //add info
                        var columnCount = sheet.Properties.GridProperties.ColumnCount.Value;
                        var rowCount = sheet.Properties.GridProperties.RowCount.Value;
                        var sheetInfo = new BGEntitySheetInfo(meta.Id, meta.Name, i++) {PhysicalColumnCount = columnCount, PhysicalRowCount = rowCount, SheetName = name};
                        info.AddEntitySheet(meta.Id, sheetInfo, sheet);
                        logger.AppendLine("Sheet [$] is mapped to meta [$].", name, meta.Name);

                        //this is probably not possible, cause GH does not allow to delete last cell 
                        if (logger.AppendLine(rowCount <= 0 || columnCount <= 0, "Sheet $ contains no data.", name)) continue;
                    }

                    //=====================================================================================================
                    //                             STEP 2 : Query headers (field names)
                    //=====================================================================================================

                    if (info.EntitySheetCount > 0)
                    {
                        var headersMapper = new HeadersMapper(repo, info, logger, nameMapConfig);
                        logger.SubSection(() =>
                        {
                            var readHeadersBatch = new BGGoogleSheetsBatchGet(manager);

                            info.ForEachEntitySheet(sheetInfo => readHeadersBatch.AddRange(
                                new BGGoogleSheetsBatchGet.DataRange(sheetInfo.SheetName, 1, sheetInfo.PhysicalColumnCount + 1, 1, 1))
                            );

                            readHeadersBatch.Execute(headersMapper, SpreadsheetsResource.ValuesResource.BatchGetRequest.MajorDimensionEnum.ROWS,
                                SpreadsheetsResource.ValuesResource.BatchGetRequest.ValueRenderOptionEnum.UNFORMATTEDVALUE);
                        }, "Mapping for $ sheet(s) ", info.EntitySheetCount);


                        //=====================================================================================================
                        //                             STEP 3 : Query entity ids values
                        //=====================================================================================================
                        var sheetIndexes = headersMapper.IdRangeIndexes;
                        var sheetsWithIdsCount = sheetIndexes.Count;
                        if (sheetsWithIdsCount > 0)
                        {
                            logger.SubSection(() =>
                            {
                                var readIdsBatch = new BGGoogleSheetsBatchGet(manager);

                                foreach (var sheetIndex in sheetIndexes)
                                {
                                    var entitySheet = info.GetEntitySheet(sheetIndex);
                                    readIdsBatch.AddRange(new BGGoogleSheetsBatchGet.DataRange(entitySheet.SheetName, entitySheet.IndexId, entitySheet.IndexId, 2, entitySheet.PhysicalRowCount));
                                }

                                readIdsBatch.Execute(new IdValuesMapper(logger, info, sheetIndexes), SpreadsheetsResource.ValuesResource.BatchGetRequest.MajorDimensionEnum.COLUMNS,
                                    SpreadsheetsResource.ValuesResource.BatchGetRequest.ValueRenderOptionEnum.UNFORMATTEDVALUE);
                            }, "Reading entity ids for $ sheet(s) ", sheetsWithIdsCount);
                        }
                    }
                }

                if (info.Lock == null)
                {
                    // we enabled locking by default- so lock sheet creation is forced 
                    logger.AppendLine("Lock sheet $ is not found. Trying to create one...", BGGoogleSheetsLock.DBLockSheetName);
                    manager.AddSheet(false, BGGoogleSheetsLock.DBLockSheetName, 1, 1);
                    logger.AppendLine("Lock sheet $ is created.", BGGoogleSheetsLock.DBLockSheetName);
                    info.Lock = new BGGoogleSheetsLock(manager);
                }
            });


            return info;
        }

        private void TryToReadLock(Sheet sheet)
        {
            //Useless check? probably 1 cell always exists
            if (sheet.Properties.GridProperties.ColumnCount > 0 && sheet.Properties.GridProperties.RowCount > 0)
            {
                info.Lock = new BGGoogleSheetsLock(manager);
                logger.AppendLine("Lock sheet $ is found.", BGGoogleSheetsLock.DBLockSheetName);
            }
            else logger.AppendWarning("Lock sheet $ found but is has no columns/rows. skipping...", BGGoogleSheetsLock.DBLockSheetName);
        }

        private class IdValuesMapper : BGGoogleSheetsBatchGet.DataMapper
        {
            private readonly BGEntitySheetInfo[] sheets;
            private readonly BGLogger logger;

            public IdValuesMapper(BGLogger logger, BGGoogleSheetsInfo info, List<int> sheetIndexes)
            {
                this.logger = logger;
                sheets = new BGEntitySheetInfo[sheetIndexes.Count];
                for (var i = 0; i < sheetIndexes.Count; i++) sheets[i] = info.GetEntitySheet(sheetIndexes[i]);
            }

            public override void RangeEnd(int rangeIndex)
            {
                var entitySheet = sheets[rangeIndex];
                logger.AppendLine("Sheet [$]. Found $ entities.", entitySheet.Name, entitySheet.RowCount);
            }

            public override void ProcessCell(int rangeIndex, int column, int row, string cellValue)
            {
                var entitySheet = sheets[rangeIndex];
                try
                {
                    var entityId = BGGoogleSheetsUtils.ReadId(cellValue, true);
                    if (entityId == BGId.Empty) return;
                    entitySheet.AddRow(entityId, row);
                }
                catch (Exception e)
                {
                    // ignored
                    logger.AppendWarning("Invalid id=$ at row $. Error= $", cellValue, row, e.Message);
                }
            }
        }


        private class HeadersMapper : BGGoogleSheetsBatchGet.DataMapper
        {
            private readonly BGGoogleSheetsInfo info;
            private readonly BGLogger logger;
            private readonly List<int> idRangeIndexes = new List<int>();
            private readonly BGMetaEntity[] metas;
            private readonly BGSyncNameMapConfig nameMapConfig;

            public List<int> IdRangeIndexes => idRangeIndexes;

            public HeadersMapper(BGRepo repo, BGGoogleSheetsInfo info, BGLogger logger, BGSyncNameMapConfig nameMapConfig = null)
            {
                this.info = info;
                this.logger = logger;
                this.nameMapConfig = nameMapConfig;
                metas = new BGMetaEntity[info.EntitySheetCount];
                for (var i = 0; i < metas.Length; i++) metas[i] = repo[info.GetEntitySheet(i).MetaId];
            }

            public override void RangeStart(int rangeIndex)
            {
                logger.SubSectionStart("Mapping for $", info.GetEntitySheet(rangeIndex).Name);
                base.RangeStart(rangeIndex);
            }

            public override void RangeEnd(int rangeIndex)
            {
                logger.SubSectionEnd();
            }

            public override void ProcessCell(int rangeIndex, int column, int row, string cellValue)
            {
                if (string.IsNullOrEmpty(cellValue)) return;
                var entitySheet = info.GetEntitySheet(rangeIndex);
                var meta = metas[rangeIndex];

                switch (cellValue)
                {
                    case BGBookInfo.IdHeader:
                        //duplicate column
                        if (logger.AppendWarning(entitySheet.IndexId >= 0, "duplicate _id at column $", column)) return;

                        logger.AppendLine("column #$ [_id]->[_id],", column);
                        entitySheet.IndexId = column;
                        if (entitySheet.PhysicalRowCount > 1) idRangeIndexes.Add(rangeIndex);
                        break;
                    default:
                        var field = nameMapConfig == null ? meta.GetField(cellValue, false) : nameMapConfig.Map(meta, cellValue);
                        if (logger.AppendWarning(field == null, "column $ skipped: no field $ or no mapping", column, cellValue)) return;
                        //how to treat adding missing settings??? all fields must be included
//                                            if (logger.AppendWarning(!entitySettings.IsFieldIncluded(field), "column $ skippped: field $ is not included in settings", columnA1, cell)) continue;

                        //duplicate column
                        if (logger.AppendWarning(entitySheet.HasField(field.Id), "duplicate field $ at column $", field.Name, column)) return;

                        logger.AppendLine("[column #$ $]->[$],", column, cellValue, field.Name);
                        entitySheet.AddField(field.Id, column);
                        break;
                }
            }
        }
    }
}