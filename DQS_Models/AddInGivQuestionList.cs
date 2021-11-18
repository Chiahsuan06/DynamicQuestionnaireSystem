using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DQS_Models
{
    public class AddInGivQuestionList
    {
        public int TopicNum { get; set; }  //#
        public string TopicDescription { get; set; }  //問題
        public string TopicType { get; set; }   //問題種類
        public bool TopicMustKeyIn { get; set; }   //必填
        public string Options { get; set; }  //回答

    }
}
