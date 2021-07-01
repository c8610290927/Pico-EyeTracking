using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataSync;

namespace LabData
{
    public class LabResultDemoData1 : LabDataBase
    {
        public LabResultDemoData1(string testResult01, string testResult02)
        {
            TestResult01 = testResult01;
            TestResult02 = testResult02;
        }

        public string TestResult01 { get;private set; }
        public string TestResult02 { get; private set; }

    }
}


