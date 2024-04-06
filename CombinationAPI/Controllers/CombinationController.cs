using CombinationAPI.Contracts;
using CombinationAPI.Entity;

using Microsoft.AspNetCore.Mvc;

namespace CombinationAPI.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class CombinationController : ControllerBase
    {

        private readonly string _uploadsFolder;
        private readonly IFileService _fileService;
        private readonly IRandomService _randomService;

        public CombinationController(IWebHostEnvironment hostEnvironment , IFileService fileService , IRandomService randomService)
        {
            _uploadsFolder = Path.Combine(hostEnvironment.ContentRootPath, "uploads");
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _randomService = randomService ?? throw new ArgumentNullException(nameof(randomService));
        }


        [HttpPost("combinationTest")]
        public async Task<IActionResult> UploadWordFile(IFormFile file)
        {
            string uploadDirectoryPath = "C:\\Users\\User\\Desktop\\CombinationAPI\\CombinationAPI\\uploads";
            Directory.CreateDirectory(uploadDirectoryPath);



            if (file == null || file.Length == 0 || !file.FileName.EndsWith(".docx"))
                return BadRequest("Fayl topilmadi");

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string allText = await _fileService.GetStringInFile(file, _uploadsFolder);

            List<Question> questions = new List<Question>();
            List<Answer> answers = new List<Answer>();   
            _fileService.SetInformationToData(allText ,ref questions , ref answers , fileName);

            foreach (Question question in questions)
            {
                List<Answer> answers1 = answers.Where(p => p.QuestionId == question.Id).ToList();
                List<Answer> answers2 = _randomService.RandomAnswer(ref answers1);
                question.Answers = answers2;
            }

            List<Question> questionsList = new List<Question>();
            questionsList = _randomService.RandomQuestion(ref questions);

            string downloadDirectoryPath = "C:\\Users\\User\\Desktop\\CombinationAPI\\CombinationAPI\\downloads";
            Directory.CreateDirectory(downloadDirectoryPath);

            string downlaodDirectoryPath = "C:\\Users\\User\\Desktop\\CombinationAPI\\CombinationAPI\\images";
            Directory.CreateDirectory(downloadDirectoryPath);




            string filePath = downloadDirectoryPath + "\\" + fileName;
            _fileService.WritToFile(filePath,ref questionsList);


            if (!System.IO.File.Exists(filePath))
                return NotFound();

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", file.Name + "Copy" + Path.GetExtension(file.FileName));
        }

    }
}