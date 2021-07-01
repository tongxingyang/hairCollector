/*
<copyright file="BGGoogleSheetsInfo.cs" company="BansheeGz">
    Copyright (c) 2018-2020 All Rights Reserved
</copyright>
*/

using System.Collections.Generic;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace BansheeGz.BGDatabase
{
    public partial class BGGoogleSheetsInfo : BGBookInfo
    {
        private readonly BGIdDictionary<Sheet> metaId2Sheet = new BGIdDictionary<Sheet>();

        private readonly BGGoogleSheetsManager manager;

        public Sheet MetaEntry;
        public Sheet FieldEntry;
        public Sheet AddonEntry;

        public bool FieldMappingValid = true;
        public bool MetaMappingValid = true;
        public bool AddonMappingValid = true;
        public BGGoogleSheetsLock Lock { get; set; }


        public BGGoogleSheetsInfo(BGGoogleSheetsManager manager)
        {
            this.manager = manager;
        }

        public void AddEntitySheet(BGId metaId, BGEntitySheetInfo entitySheet, Sheet worksheetEntry)
        {
            base.AddEntitySheet(metaId, entitySheet);
            metaId2Sheet[metaId] = worksheetEntry;
        }

        public void ClearRepo()
        {
            var deleteSheets = new BGGoogleSheetsDeleteSheets(manager, false);
            ForEachEntitySheet(info => deleteSheets.Add(metaId2Sheet[info.MetaId].Properties.SheetId.Value));
            deleteSheets.Execute();
            metaId2Sheet.Clear();
            Clear();
        }

        public void ClearMeta()
        {
            var deleteSheets = new BGGoogleSheetsDeleteSheets(manager, false);
            if(MetaEntry!=null) deleteSheets.Add(MetaEntry.Properties.SheetId.Value);
            if(FieldEntry!=null) deleteSheets.Add(FieldEntry.Properties.SheetId.Value);
            if(AddonEntry!=null) deleteSheets.Add(AddonEntry.Properties.SheetId.Value);

            MetaEntry = null;
            FieldEntry = null;
            AddonEntry = null;

            MetaMappingValid = false;
            FieldMappingValid = false;
            AddonMappingValid = false;

            MetaSheet = null;
            FieldSheet = null;
            AddonSheet = null;
        }


        public Sheet GetEntry(BGId metaId)
        {
            return metaId2Sheet[metaId];
        }


    }
}