using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DQS_Models
{
    public class AddInGivQuestionList
    {
        public int Number { get; set; }  //#
        public string QuestionType { get; set; }    //種類
        public string Question { get; set; }  //問題
        public string Choose { get; set; }   //問題種類
        public bool Required { get; set; }   //必填
        public string Options { get; set; }  //回答

    }
}
