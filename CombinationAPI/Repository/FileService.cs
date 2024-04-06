using CombinationAPI.Contracts;
using CombinationAPI.Entity;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace CombinationAPI.Repository
{
    public class FileService : IFileService
    {
        public async Task<string> GetStringInFile(IFormFile file , string uploadsFolder)
        {
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            using (WordprocessingDocument doc = WordprocessingDocument.Open(filePath, false))
            {
                string allText = "";
                foreach (var paragraph in doc.MainDocumentPart.Document.Descendants<Paragraph>())
                {
                    allText += paragraph.InnerText;
                }




                return allText;
            }
        }

        public List<Question>? SetInformationToData(string data , ref List<Question> questionlist , ref List<Answer> answerList , string fileName)
        {
            string directoryPath = "C:\\Users\\User\\Desktop\\CombinationAPI\\CombinationAPI\\uploads";
            Directory.CreateDirectory(directoryPath);

            if (data==string.Empty)
            {
                return null;
            }

            int questionId = 1;
            int answerId = 1;
            int index = 0;
            string[] fullquestions = data.Split('#');

            foreach (var str in fullquestions)
            {
                var fqst = str.Trim();
                if(fqst != string.Empty)
                {
                    int index1 = 0;
                    int index2 = 0;
                    int index3 = 0;

                    if(fqst.Contains('.'))
                    {
                        index1 = fqst.LastIndexOf('.');
                    }

                    if(fqst.Contains('?'))
                    {
                        index2 = fqst.LastIndexOf('?');
                    }
                    
                    if(fqst.Contains(':'))
                    {
                        index3 = fqst.LastIndexOf(':');
                    }

                    index = maxthree(index1 , index2 , index3);

                    Question question = new Question();
                    if(index > 0)
                    {
                        question.Title = fqst.Substring(0 , index + 1);
                        question.Id = questionId++;
                        question.FileName = fileName;

                        var anString = fqst.Substring(index + 1).Trim();
                        string[] anslist = anString.Split(")");
                        bool isTrue = anString.StartsWith("+");
                        

                        foreach(var answ in anslist)
                        {
                            if (answ.Trim().Equals("A") || answ.Trim().Equals("+A"))
                            {
                                continue;
                            }

                            Answer answer = new Answer();
                            var ans  = answ.Trim();
                            answer.Id = answerId++;
                            string title = string.Empty;

                            if (isTrue)
                            {
                                answer.isTrue = true;
                                isTrue = false;
                            }

                            if (ans.Contains("+"))
                            {
                                isTrue = true;
                            }
                            else
                            {
                               isTrue = false;
                            }

                            if (ans.Contains("B"))
                            {
                                ans = ans.Replace("B", "");
                            }

                            if (ans.Contains("C"))
                            {
                                ans = ans.Replace("C", "");
                            }

                            if (ans.Contains("D"))
                            {
                                ans = ans.Replace("D", "");
                            }

                            if (ans.Contains("+"))
                            {
                                ans = ans.Replace("+", "");
                            }

                            title = ans;

                            answer.Title = title.Trim();
                            answer.QuestionId = questionId - 1;
                            
                            answerList.Add(answer);
                        }
                       
                        question.Answers =  answerList.Where(p => p.QuestionId == question.Id).ToList();
                        questionlist.Add(question);
                    }
                }
            }
            return questionlist;
        }

        public void WritToFile(string filePath,ref List<Question> questions)
        {
            using (WordprocessingDocument doc = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = doc.AddMainDocumentPart();
                mainPart.Document = new Document();

                Body body = mainPart.Document.AppendChild(new Body());

                foreach (Question question in questions)
                {
                    int variantIndex = 1;
                    Paragraph para = body.AppendChild(new Paragraph());
                    Run run = para.AppendChild(new Run());
                    run.AppendChild(new Text($"#{question.Title}\n"));
                    foreach (Answer answer in question.Answers)
                    {
                        var k = string.Empty;
                        if (answer.isTrue)
                        {
                            k = "+";
                        }
                        var variant = Variant(variantIndex);
                        run.AppendChild(new Text($"{k}{variant}{answer.Title}\n"));
                        variantIndex++;
                    }
                }
            }
        }

        private int maxthree(int a , int b , int c)
        {
            if(a > b)
            {
                if(a > c)
                {
                    return a;
                }

                return c;
            }
            else
            {
                if(b > c)
                {
                    return b;
                }

                return c;
            }
        }

        private string Variant(int variandIndex)
        {
            string result = string.Empty;
            if(variandIndex == 1)
            {
                result = "A) ";
            }else if(variandIndex == 2)
            {
                result = "B) ";
            }else if(variandIndex == 3)
            {
                result = "C) ";
            }else if(variandIndex == 4)
            {
                result = "D) ";
            }
            else
            {
                result = "Menda bunday indexdagi variantni kiritish imkoniyati yo'q";
            }
            return result;
        }






        public async Task<string> GetStringInFileWithImage(IFormFile file, string uploadsFolder)
        {
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            using (WordprocessingDocument doc = WordprocessingDocument.Open(filePath, false))
            {
                string allText = "";
                var imageIndex = 0;
                foreach (var element in doc.MainDocumentPart.Document.Body.Elements())
                {
                    if(element is Paragraph)
                    {
                        var paragraph = (Paragraph)element;

                        if (paragraph.InnerText.Contains("#"))
                        {
                            imageIndex++;
                        }

                        allText += paragraph.InnerText;
                    }
                    else if(element is Drawing)
                    {
                        var drawing = (Drawing)element;
                        var blip = drawing.Descendants<DocumentFormat.OpenXml.Drawing.Blip >().FirstOrDefault();
                    
                        if(blip != null)
                        {
                            string imageReference = blip.Embed;
                            var imagePart = (ImagePart)doc.MainDocumentPart.GetPartById(imageReference);



                            string imageFileName = imageIndex.ToString() + ".jpg";

                            string imageFilePath = Path.Combine(uploadsFolder, imageFileName);

                            using(FileStream fileStream = new FileStream(imageFilePath, FileMode.Create))
                            {
                                imagePart.GetStream().CopyTo(fileStream);
                            }
;                        }
                    }


                }




                return allText;
            }
        }
    }
}
