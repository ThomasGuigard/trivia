using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivia
{
    public class QuestionsStack
    {

        public List<Questions> MesCategories;

        public void GenerateQuestion()
        {
            for (var i = 0; i < 50; i++)
            {
                foreach (var category in this.MesCategories)
                {
                    category.Generate(i);
                }
            }
        }

        public void AskQuestion(String CurrentCategory)
        {
            foreach (var category in this.MesCategories)
            {
                if (CurrentCategory == category.NameType)
                {
                    category.pickACardAndAskQuestion();
                }
      
            }
    
        }

    }
}
