/*
<copyright file="BGGoogleSheetsReader.cs" company="BansheeGz">
    Copyright (c) 2018-2020 All Rights Reserved
</copyright>
*/

using System.Collections.Generic;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace BansheeGz.BGDatabase
{
    public partial class BGGoogleSheetsReader
    {
        private readonly BGGoogleSheetsReaderInfo readerInfo;
        private readonly BGLogger logger;
        private readonly BGGoogleSheetsManager manager;

/*
        public BGGoogleManager Service
        {
            get { return manager; }
        }
*/

        public BGGoogleSheetsReader(BGLogger logger, BGGoogleSheetsManager manager, BGSyncNameMapConfig nameMapConfig = null)
        {
            this.logger = logger;
            this.manager = manager;
            readerInfo = new BGGoogleSheetsReaderInfo(logger, this.manager, nameMapConfig);
        }


        public List<BGGoogleSheetsWriter.IdUpdates> ReadEntities(BGRepo repo, bool updateNewIds)
        {
//            return new BGGoogleSheetsReaderEntity(manager, logger, infoProvider.GetInfo(repo), repo).Read(updateNewIds);
            return new BGGoogleSheetsReaderEntity(manager,logger, readerInfo.GetInfo(repo), repo).Read(updateNewIds);
        }

        public void ClearRepo(BGRepo repo)
        {
            readerInfo.GetInfo(repo).ClearRepo();
        }

        public void ClearMeta(BGRepo repo)
        {
            readerInfo.GetInfo(repo).ClearMeta();
        }

        public Sheet GetSheet(BGId metaId)
        {
            return readerInfo.GetInfo(null).GetEntry(metaId);
        }

        public BGGoogleSheetsInfo GetInfo(BGRepo repo)
        {
            return readerInfo.GetInfo(repo);
        }

        public BGGoogleSheetsLock GetLock(BGRepo repo)
        {
            return readerInfo.GetInfo(repo).Lock;
        }
    }
}