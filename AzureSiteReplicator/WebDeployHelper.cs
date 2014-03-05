﻿using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Web.Deployment;

namespace AzureSiteReplicator
{
    public class WebDeployHelper
    {
        public DeploymentChangeSummary DeployContentToOneSite(string contentPath, string publishSettingsFile)
        {
            var sourceBaseOptions = new DeploymentBaseOptions();
            DeploymentBaseOptions destBaseOptions;
            string siteName = ParsePublishSettings(publishSettingsFile, out destBaseOptions);

            // Publish the content to the remote site
            using (DeploymentObject deploymentObject = DeploymentManager.CreateObject(DeploymentWellKnownProvider.ContentPath, contentPath, sourceBaseOptions))
            {
                // Note: would be nice to have an async flavor of this API...
                return deploymentObject.SyncTo(DeploymentWellKnownProvider.ContentPath, siteName, destBaseOptions, new DeploymentSyncOptions());
            }
        }

        private string ParsePublishSettings(string path, out DeploymentBaseOptions deploymentBaseOptions)
        {
            var document = XDocument.Load(path);
            var profile = document.Descendants("publishProfile").First();

            string siteName = profile.Attribute("msdeploySite").Value;

            deploymentBaseOptions = new DeploymentBaseOptions
            {
                ComputerName = String.Format("https://{0}/msdeploy.axd?site={1}", profile.Attribute("publishUrl").Value, siteName),
                UserName = profile.Attribute("userName").Value,
                Password = profile.Attribute("userPWD").Value,
                AuthenticationType = "Basic"
            };

            return siteName;
        }
    }
}
