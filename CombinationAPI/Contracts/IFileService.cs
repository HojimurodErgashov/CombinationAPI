using CombinationAPI.Entity;

namespace CombinationAPI.Contracts
{
    public interface IFileService
    {
        Task<string> GetStringInFile(IFormFile file , string uploadsFolder);
        void WritToFile(string filePath ,ref List<Question> questions);

        List<Question> SetInformationToData(string data, ref List<Question> questionlist, ref List<Answer> answerlist , string fileName);

        public Task<string> GetStringInFileWithImage(IFormFile file, string uploadsFolder);
    }
}
