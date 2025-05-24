using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;

namespace Model
{
    public class WordsReplaceModel : AbstractNotifyPropertyBaseClass
    {
        public AutoTransProcess AutoTransProcess { get; }
        public WordListModel WordListModel { get; }
        public WordsReplaceModel() {
            WordListModel = new WordListModel(this);
            AutoTransProcess = new AutoTransProcess(this);
        }

    }
}
