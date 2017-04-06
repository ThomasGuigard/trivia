using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivia
{
    public class Questions
    {
     
        public String NameType { get; private set; }

        public List<String> LesQuestions { get; private set; }

        public Questions(String name)
        {
            this.NameType = name;
        }

        public void Generate(int index)
        {
            this.LesQuestions.Add(this.NameType + " Question " + index);
        }


        public void pickACardAndAskQuestion()
        {
            Console.WriteLine(this.LesQuestions.First());
            this.LesQuestions.RemoveAt(0);
        }

    }
}
