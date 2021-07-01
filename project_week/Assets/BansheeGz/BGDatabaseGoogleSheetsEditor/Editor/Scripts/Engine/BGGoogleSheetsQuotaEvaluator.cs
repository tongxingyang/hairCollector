/*
<copyright file="BGGoogleSheetsQuotaEvaluator.cs" company="BansheeGz">
    Copyright (c) 2018-2020 All Rights Reserved
</copyright>
*/

using System.Threading.Tasks;
using UnityEngine;

namespace BansheeGz.BGDatabase
{
    public class BGGoogleSheetsQuotaEvaluator
    {
        // private float started;
        private int numberOfReads;
        private int numberOfWrites;

        public int NumberOfReads => numberOfReads;

        public int NumberOfWrites => numberOfWrites;

        public int NumberOfOperations => numberOfWrites + numberOfReads;
        
        public BGGoogleSheetsQuotaEvaluator()
        {
            Clear();
        }

        public void Clear()
        {
            // started = Time.realtimeSinceStartup;
            numberOfReads = 0;
            numberOfWrites = 0;
        }

        public void ReadOperationRequest()
        {
            numberOfReads++;
            OperationRequest();
        }
        public void WriteOperationRequest()
        {
            numberOfWrites++;
            OperationRequest();
        }

        private async void OperationRequest()
        {
            var operations = NumberOfOperations;

            if (operations < 25) return;
            
            //this is approximation of free quota
            //most of the cases will never get here! 
            if (operations < 100)
            {
                if (operations == 25) await Task.Delay(1000);
                if (operations == 60) await Task.Delay(4000);
                if (operations == 80) await Task.Delay(5000);
            }
            else
            {
                if (operations % 25 == 0)
                {
                    await Task.Delay(1000);
                }
                else if (operations % 60 == 0)
                {
                    await Task.Delay(2000);
                }
                else if (operations % 80 == 0)
                {
                    await Task.Delay(5000);
                }
            }
        }
    }
}