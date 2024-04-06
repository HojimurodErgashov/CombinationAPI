using CombinationAPI.Entity;

namespace CombinationAPI.Contracts
{
    public interface IRandomService
    {
        public List<Answer>? RandomAnswer(ref List<Answer> answerList);
        public List<Question> RandomQuestion(ref List<Question> questionList);
    }
}
