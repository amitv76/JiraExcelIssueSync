﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JiraRestApiWrapper.JiraModel
{
    public class IssueCreateMeta
    {
        public string expand { get; set; }
        public List<ProjectMeta> projects { get; set; }
    }
}
