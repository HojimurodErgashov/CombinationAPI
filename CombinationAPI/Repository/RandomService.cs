using CombinationAPI.Contracts;
using CombinationAPI.Entity;

namespace CombinationAPI.Repository
{
    public class RandomService : IRandomService
    {

        public List<Question> RandomQuestion(ref List<Question> questionList)
        {
            List<Question> questions = new List<Question>();

                int[] array = new int[questionList.Count];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = i + 1;
                }

                RandomIndex(array);

                foreach (int index in array)
                {
                    questions.Add(questionList.Where(p => p.Id == index).FirstOrDefault());
                }
            return questions;
        }

        public List<Answer>? RandomAnswer(ref List<Answer> answerList)
        {
            int k = answerList.FirstOrDefault().QuestionId - 1;
            int[] array = new int[answerList.Count];
            for(int i = 0; i < array.Length; i++)
            {
                array[i] = i + 1;
            }

            RandomIndex(array);
            List<Answer> answers = new List<Answer>();

            foreach(int index in array) 
            {
                answers.Add(answerList.Where(p => p.Id == index + k * 4).FirstOrDefault());
            }

            return answers;
        }

        private int[] RandomIndex(int[] array)
        {
            Random random = new Random();
            int n = array.Length;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                int value = array[k];
                array[k] = array[n];
                array[n] = value;
            }

            return array;
        }
    }
}
