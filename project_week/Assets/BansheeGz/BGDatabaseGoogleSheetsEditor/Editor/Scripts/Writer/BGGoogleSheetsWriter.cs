/*
<copyright file="BGGoogleSheetsWriter.cs" company="BansheeGz">
    Copyright (c) 2018-2020 All Rights Reserved
</copyright>
*/

using System.Collections.Generic;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace BansheeGz.BGDatabase
{
    public partial class BGGoogleSheetsWriter
    {
        private readonly BGGoogleSheetsInfo info;
        private readonly BGLogger logger;
        private readonly BGGoogleSheetsManager manager;
        private readonly BGSyncNameMapConfig nameMapConfig;

        public BGGoogleSheetsWriter(BGLogger logger, BGGoogleSheetsManager manager, BGSyncNameMapConfig nameMapConfig = null)
        {
            this.logger = logger;
            this.manager = manager;
            this.nameMapConfig = nameMapConfig;
            info = new BGGoogleSheetsInfo(manager);
        }

        public void Add(BGMetaEntity meta, bool addObjects, BGSyncNameMapConfig syncNameMapConfig = null)
        {
            var sheetInfo = new BGEntitySheetInfo(meta.Id, meta.Name, 0) {SheetName = syncNameMapConfig == null ? meta.Name : syncNameMapConfig.GetName(meta)};
            info.AddEntitySheet(meta.Id, sheetInfo);
            meta.ForEachField(field => sheetInfo.AddField(field.Id, 0));
            if (addObjects) meta.ForEachEntity(entity => sheetInfo.AddRow(entity.Id, 0));
        }

        public void Clear()
        {
            info.ClearRepo();
        }


        public void Write(BGGoogleSheetsReader reader, BGRepo repo, BGMergeSettingsEntity settings, BGRepo repoMeta, BGMergeSettingsMeta metaSettings, bool transferRowsOrder, BGRepo sourceRepo)
        {
            var readerInfo = reader.GetInfo(repo);
            new BGGoogleSheetsWriterEntity(manager, nameMapConfig).Write(logger, reader, repo, settings, info, readerInfo, transferRowsOrder, sourceRepo);
        }


        public void UpdateIds(List<IdUpdates> updates)
        {
            if (updates == null || updates.Count == 0) return;

            new BGGoogleSheetsWriterEntity(manager, nameMapConfig).UpdateIds(logger, updates);
        }

        public class IdUpdates
        {
            public readonly int IdColumn = -1;
            public readonly List<IdData> List = new List<IdData>();
            public readonly Sheet Sheet;
            public readonly string MetaName;

            public int Count
            {
                get { return List.Count; }
            }

            public IdUpdates(string metaName, int idColumn, Sheet sheet)
            {
                MetaName = metaName;
                IdColumn = idColumn;
                Sheet = sheet;
            }

            public class IdData
            {
                public readonly int Row;
                public readonly BGId Id;

                public IdData(int row, BGId id)
                {
                    Row = row;
                    Id = id;
                }

                public override string ToString()
                {
                    return Row + "=" + Id;
                }
            }
        }
    }
}