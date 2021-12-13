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
        public int answer1Value { get; set; }
        public string answer1percentage { get; set; }

        public string answer2 { get; set; }     //選項2
        public int answer2Value { get; set; }
        public string answer2percentage { get; set; }

        public string answer3 { get; set; }     //選項3
        public int answer3Value { get; set; }
        public string answer3percentage { get; set; }

        public string answer4 { get; set; }     //選項4
        public int answer4Value { get; set; }
        public string answer4percentage { get; set; }

        public string answer5 { get; set; }     //選項5
        public int answer5Value { get; set; }
        public string answer5percentage { get; set; }

        public string answer6 { get; set; }     //選項6
        public int answer6Value { get; set; }
        public string answer6percentage { get; set; }

        public int OptionsAll { get; set; }     //總選項數
        public string RDAns { get; set; }       //填答者回答
    }
}
