/*
<copyright file="BGGoogleSheets.cs" company="BansheeGz">
    Copyright (c) 2018-2020 All Rights Reserved
</copyright>
*/

using System;
using System.Collections.Generic;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace BansheeGz.BGDatabase
{
    public partial class BGGoogleSheets : BGSyncA
    {
        public BGGoogleSheets(BGLogger logger, BGRepo mainRepo, BGMergeSettingsEntity entitySettings, BGMergeSettingsMeta metaSettings, BGSyncNameMapConfig nameMapConfig = null) 
            : base(logger, mainRepo, entitySettings, metaSettings, nameMapConfig)
        {
        }

        public void Export(BGGoogleSheetsManager manager, bool transferRowsOrder)
        {
            var reader = new BGGoogleSheetsReader(Logger, manager, NameMapConfig);
            var writer = new BGGoogleSheetsWriter(Logger, manager, NameMapConfig);

            Export(false,
                repo => manager.Lock(reader.GetLock(repo)),
                (repo, readMeta) =>
                {
                    BGRepo metaRepo = null;
//                        if (readMeta) metaRepo = reader.ReadMeta(repo);

                    reader.ReadEntities(repo, false);
                    return metaRepo;
                },
                (transfer, repo, repoMeta) =>
                {
                    if (transfer)
                    {
                        reader.ClearRepo(repo);
                        reader.ClearMeta(repo);
                        writer.Clear();
                    }
                    else
                    {
                        if (MetaSettings.Mode == BGMergeModeEnum.Transfer) reader.ClearMeta(repo);

                        if (EntitySettings.Mode == BGMergeModeEnum.Transfer)
                        {
                            reader.ClearRepo(repo);
                            writer.Clear();
                        }
                    }

                    Write(repo, repoMeta, reader, writer, transferRowsOrder);
                });
        }


        public void Import(BGGoogleSheetsManager manager, bool updateNewIds, bool transferRowsOrder)
        {
            var reader = new BGGoogleSheetsReader(Logger, manager, NameMapConfig);
            var writer = new BGGoogleSheetsWriter(Logger, manager, NameMapConfig);

            List<BGGoogleSheetsWriter.IdUpdates> idUpdates = null;
            Import(updateNewIds, transferRowsOrder,
                repo =>
                {
                    if (updateNewIds) manager.Lock(reader.GetLock(repo));
                    else
                    {
                        //we do it here to ensure logger get proper sections (and yes, it's pretty ugly)
                        var info = reader.GetInfo(repo);
                    }
                },
                repo => null,
                repo => idUpdates = reader.ReadEntities(repo, updateNewIds),
                () =>
                {
                    if (BGUtil.IsEmpty(idUpdates)) return;
                    writer.UpdateIds(idUpdates);
                },
                repo => reader.GetInfo(repo)
            );
        }

        private void Write(BGRepo repo, BGRepo repoMeta, BGGoogleSheetsReader reader, BGGoogleSheetsWriter writer, bool transferRowsOrder)
        {
            repo.ForEachMeta(meta => writer.Add(meta, true, NameMapConfig));
            writer.Write(reader, repo, EntitySettings, repoMeta, MetaSettings, transferRowsOrder, MainRepo);
        }
    }
}