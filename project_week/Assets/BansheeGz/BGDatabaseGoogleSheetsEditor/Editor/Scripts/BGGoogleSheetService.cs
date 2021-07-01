/*
<copyright file="BGGoogleSheetService.cs" company="BansheeGz">
    Copyright (c) 2018-2020 All Rights Reserved
</copyright>
*/

using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Json;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;

namespace BansheeGz.BGDatabase
{
    [BGPlugin(Version = "1.2")]
    public class BGGoogleSheetService : BGGoogleSheetServiceI, BGGoogleSheetServiceV2
    {
        private const string RedirectUrl = "urn:ietf:wg:oauth:2.0:oob";
        private static readonly string[] Scopes = {SheetsService.Scope.Spreadsheets};

        private BansheeGz.BGDatabase.Editor.BGDsGoogleSheets ds;
        private BGSyncNameMapConfig nameMapConfig;
        private ClientSecrets ClientSecrets
        {
            get
            {
                return new ClientSecrets
                {
                    ClientId = ds.ClientId,
                    ClientSecret = ds.ClientSecret,
                };
            }
        }

        private SheetsService Service
        {
            get
            {
                // Create the service.
                switch (ds.DataSourceType)
                {
                    case BansheeGz.BGDatabase.Editor.BGDsGoogleSheets.DataSourceTypeEnum.OAuth:
                        return new SheetsService(new BaseClientService.Initializer()
                        {
                            HttpClientInitializer = GoogleCredential.FromJson(NewtonsoftJsonSerializer.Instance.Serialize(new JsonCredentialParameters
                            {
                                ClientId = ds.ClientId,
                                ClientSecret = ds.ClientSecret,
                                RefreshToken = ds.RefreshToken,
                                Type = JsonCredentialParameters.AuthorizedUserCredentialType
                            })),
                            ApplicationName = ds.ApplicationName,
                        });
                    case BansheeGz.BGDatabase.Editor.BGDsGoogleSheets.DataSourceTypeEnum.Service:
                        return new SheetsService(new BaseClientService.Initializer()
                        {
                            HttpClientInitializer = GoogleCredential.FromJson(NewtonsoftJsonSerializer.Instance.Serialize(new JsonCredentialParameters
                            {
                                ClientEmail = ds.ClientEmail,
                                PrivateKey = ds.PrivateKey,
                                Type = JsonCredentialParameters.ServiceAccountCredentialType
                            })).CreateScoped(SheetsService.Scope.Spreadsheets),
                        });
                    case BansheeGz.BGDatabase.Editor.BGDsGoogleSheets.DataSourceTypeEnum.APIKey:
                        return new SheetsService(new BaseClientService.Initializer()
                            {
                                //HttpClientInitializer = credential,
                                ApplicationName = string.IsNullOrEmpty(ds.ApplicationName) || ds.ApplicationName.Trim().Length == 0 ? "BGDatabase" : ds.ApplicationName,
                                ApiKey = ds.APIKey,
                            }
                        );
                    case BansheeGz.BGDatabase.Editor.BGDsGoogleSheets.DataSourceTypeEnum.Anonymous:
                        throw new NotImplementedException("Anonymous DataSource type does not support GoogleSheets v.4 Service");
                    default:
                        throw new ArgumentOutOfRangeException("ds.DataSourceType");
                }
            }
        }


/*
        public OAuth2Parameters GetParams()
        {
            var p = new OAuth2Parameters
            {
                ClientId = ds.ClientId.Trim(),
                ClientSecret = ds.ClientSecret.Trim(),

                RedirectUri = "urn:ietf:wg:oauth:2.0:oob",
                Scope = "https://spreadsheets.google.com/feeds https://docs.google.com/feeds",
                AccessType = "offline",
                TokenType = "refresh",
            };
            if (!string.IsNullOrEmpty(ds.RefreshToken)) p.RefreshToken = ds.RefreshToken;

            return p;
        }
*/

        public void Call(BGLogger logger, Action<BGGoogleSheetsManager> action)
        {
            WithCallback(() =>
            {
                using (var manager = new BGGoogleSheetsManager(Service, ds.SpreadSheetId, logger))
                {
                    action(manager);
                }
            });
        }


        public void WithCallback(Action action)
        {
            var old = ServicePointManager.ServerCertificateValidationCallback;
            ServicePointManager.ServerCertificateValidationCallback = AuthCallback;
            try
            {
                action();
            }
            finally
            {
                ServicePointManager.ServerCertificateValidationCallback = old;
            }
        }


        private bool AuthCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors)
        {
//            Debug.Log("inside AuthCallback");
            var isOk = true;
            if (sslpolicyerrors == SslPolicyErrors.None) return isOk;

            // If there are errors in the certificate chain,
            // look at each error to determine the cause.
            foreach (X509ChainStatus t in chain.ChainStatus)
            {
                if (t.Status == X509ChainStatusFlags.RevocationStatusUnknown) continue;
                chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                var chainIsValid = chain.Build((X509Certificate2) certificate);
                if (chainIsValid) continue;

                isOk = false;
                break;
            }

            return isOk;
        }

        public void Init(BansheeGz.BGDatabase.Editor.BGDsGoogleSheets dataSource)
        {
            ds = dataSource;
        }

        public BGGoogleSheetRefreshTokenServiceI CreateRefreshTokenService()
        {
            return new BGGoogleSheetRefreshTokenService(ClientSecrets, WithCallback);
        }
       

        //============================================= V1
        public void Export(BGLogger logger, BGRepo repo, BGMergeSettingsEntity modelSettingsEntity, BGMergeSettingsMeta mergeSettingsMeta, bool transferRowsOrder)
        {
            if (ds.DataSourceType == BansheeGz.BGDatabase.Editor.BGDsGoogleSheets.DataSourceTypeEnum.APIKey)
                throw new Exception("Your DataSource type is APIKey (readonly). " +
                                    "Can not write to GoogleSheets with such DataSource type");

            Call(logger, manager =>
            {
                new BGGoogleSheets(logger, repo, modelSettingsEntity, new BGMergeSettingsMeta(), nameMapConfig).Export(manager, transferRowsOrder);
            });
        }

        public void Import(BGLogger logger, BGRepo repo, BGMergeSettingsEntity modelSettingsEntity, BGMergeSettingsMeta mergeSettingsMeta, bool updateNewIds, bool transferRowsOrder)
        {
            if (ds.DataSourceType == BansheeGz.BGDatabase.Editor.BGDsGoogleSheets.DataSourceTypeEnum.APIKey && updateNewIds)
                throw new Exception("Your DataSource type is APIKey (readonly). " +
                                    "You toggled on 'update new Ids on import' parameter on, which means you need write access. " +
                                    "Either turn off this toggle or chose another DataSource type.");


            Call(logger, manager =>
            {
                new BGGoogleSheets(logger, repo, modelSettingsEntity, new BGMergeSettingsMeta(), nameMapConfig).Import(manager, updateNewIds, transferRowsOrder);
            });
        }

        //============================================= V2
        public void Export(BGGoogleSheetServiceV2ExportTask task)
        {
            nameMapConfig = task.nameMapConfig;
            Export(task.logger, task.repo, task.modelSettingsEntity, new BGMergeSettingsMeta(), task.transferRowsOrder);
        }
        public void Import(BGGoogleSheetServiceV2ImportTask task)
        {
            nameMapConfig = task.nameMapConfig;
            Import(task.logger, task.repo, task.modelSettingsEntity, new BGMergeSettingsMeta(), task.updateNewIds, task.transferRowsOrder);
            
        }

        
        //============================================= Inner classes
        private class BGGoogleSheetRefreshTokenService : BGGoogleSheetRefreshTokenServiceI 
        {
            private readonly string url;
            private readonly GoogleAuthorizationCodeFlow flow;
            private readonly Action<Action> withCallback;

            public string Url
            {
                get
                {
                    return url;
                }
            }

            public BGGoogleSheetRefreshTokenService(ClientSecrets clientSecrets, Action<Action> withCallback)
            {
                this.withCallback = withCallback;
                flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                {
                    Scopes = Scopes,
                    ClientSecrets = clientSecrets,
                    DataStore = new NullDataStore(),
                    // ProjectId = ds.ApplicationName // this does not help to change project name!
                });

                url = flow.CreateAuthorizationCodeRequest(RedirectUrl).Build().AbsoluteUri;
           }

            public void Exchange(string code, out string token, out string refreshToken)
            {
                string token2 = null;
                string refreshToken2 = null;
                withCallback(() =>
                {
                    var response = flow.ExchangeCodeForTokenAsync("user", code, RedirectUrl, CancellationToken.None).Result;
                    token2 = response.AccessToken;
                    refreshToken2 = response.RefreshToken;
                });
                token = token2;
                refreshToken = refreshToken2;
            }
        }
            
    }
}