﻿using System;
using System.Threading.Tasks;
using JiraRestApiWrapper;
using System.Net;
using JiraRestApiWrapper.JiraModel;

namespace JiraRESTClient
{
    public class RestClient
    {
        public JiraClient JiraClient { get; private set; }

        public async Task<bool> AuthenticateToJiraAsync(string jiraUrl, string username, string password, string jiraProject,
                                        string proxyUrl = "", string proxyUser = "", string proxyPassword = "")
        {
            var userExists = false;

            //Connect to Jira with username and password. Please be aware that the information returned by the Jira REST API depends on the access rigths of the user.
            //Add Jira URI to proxy bypass list if you get proxy errors

            var proxy = new WebProxy(proxyUrl, true, null, new NetworkCredential(proxyUser, proxyPassword));
            var client = new JiraClient(new JiraAccount
                                        {
                                            ServerUrl = jiraUrl,
                                            User = username,
                                            Password = password
                                        }, proxy);
            JiraClient = client;
            userExists = true;

            try
            {
                //Just to test if login was valid
                ProjectMeta projectMetaData = client.GetProjectMeta(jiraProject);
            }            
            catch(Exception ex)
            {
                userExists = false;
            }
                   
            return userExists;
        }

        public async Task<bool> UpdateJiraIssueAsync(JiraRestApiWrapper.JiraClient client, string issueKey, string score, customfield actionStatus)
        {
            bool updated = false;

            var updateIssue = new
            {
                //{"errorMessages":[],"errors":{"customfield_13901":"Invalid value 'customfield_13901' passed for customfield 'Action'. 
                //Allowed values are: 15205[New], 13902[Investigating], 15600[Scored], 13906[Backlog], 13905[Refined], 13904[In Progress], 15000[To Deploy], 15001[Done],
                //                      13903[On Hold], -1"}}
                fields = new
                {
                    customfield_13503 = decimal.Parse(score),
                    customfield_13901 = actionStatus
                }
            };

            if (client.UpdateIssueFields(issueKey, updateIssue))
            {
                updated = true;
            }

            return updated;
        }
    }
}