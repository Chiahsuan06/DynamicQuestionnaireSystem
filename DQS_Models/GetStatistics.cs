using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DQS_Models
{
    public class GetStatistics
    {
        public int TopicNum { get; set; }  //題目編號
        public string TopicDescription { get; set; } //題目
        public string TopicType { get; set; }

        public string answer1 { get; set; }     //選項1
        public int answer1Vaule { get; set; }

        public string answer2 { get; set; }     //選項2
        public int answer2Vaule { get; set; }

        public string answer3 { get; set; }     //選項3
        public int answer3Vaule { get; set; }

        public string answer4 { get; set; }     //選項4
        public int answer4Vaule { get; set; }

        public string answer5 { get; set; }     //選項5
        public int answer5Vaule { get; set; }

        public string answer6 { get; set; }     //選項6
        public int answer6Vaule { get; set; }

        public int OptionsAll { get; set; }     //總選項數
        public string RDAns { get; set; }       //填答者回答
    }
}
