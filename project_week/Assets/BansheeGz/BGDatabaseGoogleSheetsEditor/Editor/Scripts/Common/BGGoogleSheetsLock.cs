/*
<copyright file="BGGoogleSheetsLock.cs" company="BansheeGz">
    Copyright (c) 2018-2020 All Rights Reserved
</copyright>
*/

using System;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using Google.Apis.Sheets.v4.Data;

namespace BansheeGz.BGDatabase
{
    public class BGGoogleSheetsLock
    {
        public const string DBLockSheetName = "_dblock_";

        private readonly BGGoogleSheetsManager manager;
        private readonly Sheet entry;
        private bool lockWasObtained;

        private string address;

        private string Address
        {
            get
            {
                if (!string.IsNullOrEmpty(address)) return address;

                address = NetworkInterface
                    .GetAllNetworkInterfaces()
                    .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    .Select(nic => nic.GetPhysicalAddress().ToString())
                    .FirstOrDefault();

                if (string.IsNullOrEmpty(address)) address = Environment.MachineName + " " + new Guid();
                return address;
            }
        }

        private string LockValue
        {
            get { return manager.DataReadCell(DBLockSheetName, 1, 1); }
            set { manager.DataUpdate(DBLockSheetName, 1, 1, value); }
        }


        public BGGoogleSheetsLock(BGGoogleSheetsManager manager)
        {
            this.manager = manager;
        }

        public void ObtainLock(BGLogger logger)
        {
            lockWasObtained = false;
            var lockValue = LockValue;

            if (!string.IsNullOrEmpty(lockValue) && lockValue.Trim() != "")
            {
                var error = "GoogleSheets spreadsheet '" + manager.SpreadsheetName + "', you try to write to, is currently locked by another user. " +
                            "You can remove the lock manually by clearing cell A1 of sheet '" + DBLockSheetName + "'. The lock value is: " + lockValue;
                logger?.AppendWarning(error);
                throw new BGException(error);
            }

            LockValue = Address;
            logger?.AppendLine("Lock is obtained. Value=" + Address);
            lockWasObtained = true;
        }

        public void ReleaseLock(BGLogger logger)
        {
            if (!lockWasObtained) return;

            var lockValue = LockValue;

            LockValue = "";

            if (!Address.Equals(lockValue))
            {
                var error = "Warning!!! Possible data collision was detected! Expected lock value (" + DBLockSheetName + ".A1) was " + Address + " , but the actual value was " + lockValue +
                            ". Please review the exported data and ensure it's correct, read more here: https://www.bansheegz.com/BGDatabase/ThirdParty/GoogleSheets/";
                logger?.AppendWarning(error);
                throw new BGException(error);
            }

            lockWasObtained = false;
            logger?.AppendLine("Lock is released.");
        }
    }
}