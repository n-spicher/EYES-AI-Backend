using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pdf_Processor.Services.PdfService;
//using PdfSharpCore.Pdf;
//using PdfSharpCore.Pdf.Advanced;
using PdfSharpCore.Pdf.IO;
using System;
using Pdf_Processor.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using static System.Net.Mime.MediaTypeNames;
using System.Globalization;
using CsvHelper.Configuration;
using Aspose.Cells;
//Rendering PDF documents to Images or Thumbnails
using IronPdf;
using System.Drawing;
using Microsoft.IdentityModel.Protocols;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using GroupDocs.Search.Options;

namespace Pdf_Processor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PdfController : ControllerBase
    {
        private readonly IPdfService pdfService;

        public PdfController(
            IPdfService pdfService
            )
        {
            this.pdfService = pdfService;
        }

        [HttpPost("[action]")]
        public async Task<object> UploadPdf([FromForm] IFormFile file)
        {
            var directoryPath = Path.Combine(
                           Directory.GetCurrentDirectory(), "wwwroot\\pdf-files"
                       );

            bool exists = System.IO.Directory.Exists(directoryPath);
            if (!exists)
            {
                System.IO.Directory.CreateDirectory(directoryPath);
            }

            var filePath = Path.Combine(
                directoryPath,
                file.FileName
                );

            using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
                stream.Close();
            }

            return new
            {
                FileName = file.FileName
            };
        }

        [HttpGet("[action]")]
        public async Task<object> GetFilesList()
        {
            var directoryPath = Path.Combine(
                           Directory.GetCurrentDirectory(), "wwwroot\\pdf-files"
                       );

            var files = Directory.GetFiles(directoryPath);

            return new
            {
                Files = files
            };
        }

        [HttpGet("[action]")]
        public async Task<object> Search(string fileName, string query)
        {
            System.Diagnostics.Debug.WriteLine("Test: " + query);
            var result = await this.pdfService.Search(fileName, query);

            return result;
        }

        /**
         * Look through all codes in first few pages to see if the value we are looking for is present.
         * This will cut time by not looking through the entire document
         */
        //[HttpGet("[action]")]
        //public async Task<object> testRegex()
        //{

        //    string filePath = Path.Combine(
        //        Directory.GetCurrentDirectory(),
        //        "wwwroot/pdf-file-one",
        //        "T Rowe Price Global Headquarters Spec - 1702 Pages - Pumps.pdf"
        //        );

        //    System.Diagnostics.Debug.WriteLine(filePath);

        //    List<object> pdfPagesContent = new();

        //    using (PdfDocument pdfDocument = PdfReader.Open(filePath, PdfDocumentOpenMode.Import))
        //    {
        //        for (int i = 311; i < 317; i++)
        //        {
        //            string text = string.Join("", pdfDocument.Pages[i].ExtractText().ToArray());
        //            text = text.Replace("\u0000", string.Empty);
        //            //System.Diagnostics.Debug.WriteLine("Test: " + text);
        //            //if (text.Contains("WARRANTY"))
        //            //{
        //                pdfPagesContent.Add(
        //                    new
        //                    {
        //                        page = i + 1,
        //                        text
        //                    }
        //                );
        //            //}
        //        }
        //    }

        //    return pdfPagesContent;
            
        //}

        [HttpGet("[action]")]
        public async Task<object> testPack()
        {

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/pdf-test",
                "ShellJr.pdf"
                );
            IronPdf.Installation.TempFolderPath = @"/tmp";
            var PDF = PdfDocument.FromFile(filePath);

            //Get all text to put in a search index
            string AllText = PDF.ExtractAllText();

     

            //Or even find the precise text and images for each page in the document
            for (var index = 0; index < PDF.PageCount; index++)
            {
                int PageNumber = index + 1;
                string Text = PDF.ExtractTextFromPage(index);
                
            }

            return new List<String> { "Test"};

        }


        [HttpGet("[action]")]
        public async Task<object> ironPdfTest()
        {

            // Extracting Image and Text content from Pdf Documents
            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/pdf-test",
                "ShellJr.pdf"
                );

            // open a 128 bit encrypted PDF
            var pdf = PdfDocument.FromFile(filePath);

            //Get all text to put in a search index
            string @AllText = @pdf.ExtractAllText();

            List<String> tst = new List<String>();

            tst.Add(AllText);
            

            string pattern1 = @"[\/n]{8}(.*23\s*21\s*13\s*-\s*8.*)[\/n]{8}";
            string pattern2 = @"[\\n]{8}([\s\S]*How to Start[\s\S]*)[\\n]{0,8}";
            string input = @"CMSC 216 Exercise #5 Spring 2016\r\nShell Jr (”Shellito”) Due: Tuesday Apr 19, 2016, 8:00PM\r\n1 Objectives\r\nTo practice fork() and exec by implementing a very simple shell.\r\n2 Overview\r\nThe first thing you need to do is to copy the directory shelljr we have left in the grace cluster under the exercises\r\ndirectory. Remember that you need that folder as it contains the .submit file that allows you to submit.\r\n3 Specifications\r\nFor this exercise you will implement a very simpliflied shell. A shell is a C program that executes commands\r\nby forking itself and using exec. We have been using tcsh, but there are other shells like ksh, sh, bash, etc. The\r\nname of our shell is shell jr .\r\n4 Shell Jr Functionality\r\nYour shell will have a loop that reads command lines and process them. The prompt for your shell will be\r\n”shell jr: ”. The commands your shell must handle are:\r\n1. exit - When the user enters the exit command the shell will stop executing by calling exit(). Before\r\nexecuting exit, the shell will print the message ”See you”.\r\n2. hastalavista - Has the same functionality as exit.\r\n3. cd - This command changes the current directory. You can assume the user will always provide a direc￾tory as an argument.\r\n4. A command with a maximum of one argument (e.g., wc location.txt). That means your shell should be\r\nable to handle commands like pwd, date, or wc location.txt .\r\n5 Requirements\r\n1. You must NOT use an exec* function to implement the functionality associated with the commands exit,\r\nhastalavista, and cd. For other commands you must create a child (via fork()) and use execvp() to execute\r\nthe command.\r\n2. If the user provides an invalid command, the message ”Failed to execute ” followed by the command\r\nname should be printed. In this case the child will exit returning the error code EX OSERR. Use printf\r\nto display the message and flush the output buffer (fflush(stdout)). Note that the shell is not terminated\r\nby executing an invalid command.\r\n3. You don’t need to handle the case where the user just types enter (you can assume the user will always\r\nprovide a command).\r\n4. Make sure you use printf to print the shell prompt and that you flush the buffer.\r\n5. It is your responsibility to verify that your program generates the expected results in the submit server.\r\n6. You must use execvp (and no other exec* system call).\r\n7. Your code must be written in the file shell jr.c.\r\n1\n\n\n\n8. You may not use dup2, read, write or pipes.\r\n9. You may not use system() in order to execute commands.\r\n10. You can assume a line of input will have a maximum of 1024 characters.\r\n11. Provide a makefile that builds an executable called shell jr. Name the target that builds the executable\r\nshell jr . Feel free to add any other targets you need.\r\n12. All your C programs in this course should be written using the compiler gcc, with the options defined\r\nin the gcc aliases info.txt file. This file can be found in the info folder of the public grace account.\r\n13. Your program should be written using good programming style as defined at\r\nhttp://www.cs.umd.edu/class/spring2016/cmsc216/content/resources/coding-style.html\r\n14. Common error: If you get the submit server message ”Execution error, exit code 126” execute ”make\r\nclean” before submitting your code.\r\n15. Common error: To forget to return the correct value (e.g., 0) in your code.\r\n6 How to Start\r\nYou should start by creating a loop that reads lines and displays them. Then you should begin to process\r\neach type line (starting with the exit and cd commands). You are free to process each line any way you want,\r\nhowever, reading a whole line using fgets and then processing the line using sscanf could make things simpler.\r\nKeep in mind that if sscanf cannot read a value into a string variable, it will not change the variable. This could\r\nhelp you identify when a command has an argument or does not.\r\n7 Submitting your assignment\r\n1. In the assignment directory execute the command submit.\r\n2. Your assignment must be electronically submitted by the date and time above to avoid losing credit. See\r\nthe course syllabus for details.\r\n8 Grading Criteria\r\nYour assignment grade will be determined with the following weights:\r\nResults of public tests 28%\r\nResults of release tests 72%\r\n9 Academic integrity statement\r\nPlease carefully read the academic honesty section of the course syllabus. Any evidence of impermissible\r\ncooperation on assignments, use of disallowed materials or resources, or unauthorized use of computer ac￾counts, will be submitted to the Student Honor Council, which could result in an XF for the course, or suspen￾sion or expulsion from the University. Be sure you understand what you are and what you are not permitted\r\nto do in regards to academic integrity when it comes to assignments. These policies apply to all students, and\r\nthe Student Honor Council does not consider lack of knowledge of the policies to be a defense for violating\r\nthem. Full information is found in the course syllabus– please review it at this time.\r\n2";
            string rawData = Regex.Escape(AllText.ToString());

            Match m = Regex.Match(input, pattern2, RegexOptions.IgnoreCase);
            System.Diagnostics.Debug.WriteLine("String: " + Regex.Escape(AllText.ToString()));
            while (m.Success)
            {
                System.Diagnostics.Debug.WriteLine("'{0}' found in the source code at position {1}.",
                                  m.Value, m.Index);
                m = m.NextMatch();
            }
            

            return tst;

            //Get all Images
            IEnumerable<System.Drawing.Image> AllImages = pdf.ExtractAllImages();

            //Or even find the precise text and images for each page in the document
            for (var index = 0; index < pdf.PageCount; index++)
            {
                int PageNumber = index + 1;
                string Text = pdf.ExtractTextFromPage(index);
                IEnumerable<System.Drawing.Image> Images = pdf.ExtractImagesFromPage(index);
                ///...
            }

            return new List<String>();
        }//End iornPdf tester



        [HttpGet("[action]")]
        public async Task<object> createTsv()
        {
            //allFinds will hold all the data that is found throughout the documents
            List<FoundKeywords> allFinds = new List<FoundKeywords>();

            //Create a FoundKeywords object 
            var tmpFound = new FoundKeywords
            {
                sectionId = "002468",
                sectionName = "Cooling Tower"
            };


            List<string> sentencesFan = new List<string>
            {"This is a fan", "Fan has been found"};

            List<string> sentencesTower = new List<string>
            {"This is a tower", "Tower has been found"};

            var matches = new Dictionary<string, List<string>>
                {
                   { "Fan" , sentencesFan},
                   {"Tower", sentencesTower }

                };

            tmpFound.matches = matches;

            allFinds.Add(tmpFound);

            //Create another FoundKeywords object 
            tmpFound = new FoundKeywords
            {
                sectionId = "007777",
                sectionName = "basin connection"
            };


            sentencesFan = new List<string>
            {"This is a basin", "basin has been found"};

            sentencesTower = new List<string>
            {"This is a connection", "connection has been found"};

            matches = new Dictionary<string, List<string>>
                {
                   { "basin" , sentencesFan},
                   {"connection", sentencesTower }

                };

            tmpFound.matches = matches;

            allFinds.Add(tmpFound);

            var directoryPath =  "/Users/jamesvclingerman/Desktop/Code/EYES-AI-Backend/Pdf Processor/wwwroot/pdf-filestest.csv";
            
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                NewLine = Environment.NewLine,
            };
            using (var writer = new StreamWriter(directoryPath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                
                csv.WriteField("Section ID");
                csv.WriteField("Section Name");
                csv.WriteField("Word Matched");
                csv.WriteField("Sentence Matched");
                csv.NextRecord();
                csv.NextRecord();

                foreach (var foundWords in allFinds)
                {
                   

                    // Get all keys from Dictionary
                    var matchedWords = new List<string>(foundWords.matches.Keys);

                    //Write each of keys (synonyms/keywords) what we are looking for
                    foreach (var heading in matchedWords)
                    {

                        //With all the keys go through dictionary and write all values
                        foreach (var sentenceFound in foundWords.matches[heading])
                        {
                            csv.WriteField(foundWords.sectionId);
                            csv.WriteField(foundWords.sectionName);
                            csv.WriteField(heading);
                            csv.WriteField(sentenceFound);
                            csv.NextRecord();

                        }

                        
                    }
                }

                

                //Newline character 
                csv.NextRecord();

               

               
                
            }
            //Console.WriteLine(workbook.Worksheets[0].Name);
            // Create LoadOptions for CSV file
            LoadOptions loadOptions = new LoadOptions(LoadFormat.CSV);

            // Create a Workbook object and initialize with CSV file's path and the LoadOptions object
            Workbook workbook = new Workbook(directoryPath, loadOptions);
            // Workbook workbooks = new Workbook("/Users/jamesvclingerman/Desktop/Code/EYES-AI-Backend/Pdf Processor/wwwroot/pdf-files/test.xlsx");

            // Save CSV file as XLSX
            workbook.Save("/Users/jamesvclingerman/Desktop/Code/EYES-AI-Backend/Pdf Processor/wwwroot/pdf-files/test.xlsx", SaveFormat.Xlsx);
            return allFinds;
        
        }
    }
}
