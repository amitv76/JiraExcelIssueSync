using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiraRestApiWrapper;
using System.Net;
using JiraRestApiWrapper.JiraModel;

namespace JiraRESTClient
{
    public class RestClient
    {
        public JiraClient JiraClient { get; private set; }

        public bool AuthenticateToJira(string jiraUrl, string username, string password)
        {
            var userExists = false;

            //Connect to Jira with username and password. Please be aware that the information returned by the Jira REST API depends on the access rigths of the user.
            //Add Jira URI to proxy bypass list if you get proxy errors

            var proxy = new WebProxy("ncproxy.ideal.corp.local:8080", true, null, new NetworkCredential("VermaAmi", "G0lden01"));
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
                ProjectMeta projectMetaData = client.GetProjectMeta("MUNI");
            }            
            catch(Exception ex)
            {
                userExists = false;
            }
                   
            return userExists;
        }
    }
}