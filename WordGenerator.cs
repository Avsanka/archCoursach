using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Word = Microsoft.Office.Interop.Word;
using System.Windows;

namespace course
{
    class WordGenerator
    {
        private FileInfo fileInfo;

        public WordGenerator(string fileName)
        {
            if (File.Exists(fileName))
            {
                fileInfo = new FileInfo(fileName);
            }
            else
            {
                throw new ArgumentException("File not found");
            }
        }

        internal bool Process(Dictionary<string, string> items)
        {
            Word.Application app = null;
            try
            {
                app = new Word.Application();
                Object file = fileInfo.FullName; //подготовка объекта к передачи

                Object missing = Type.Missing; // Объект для передачи параметров

                app.Documents.Open(file); 
                foreach (var item in items)
                {
                    Word.Find find = app.Selection.Find;    //Объект для поиска
                    find.Text = item.Key;                   //Присваиваем текст, который будем искать
                    find.Replacement.Text = item.Value;     //То, на что будем менять

                    Object wrap = Word.WdFindWrap.wdFindContinue;
                    Object replace = Word.WdReplace.wdReplaceAll;

                    find.Execute(FindText: Type.Missing,
                        MatchCase: false,            
                        MatchWholeWord: false,       
                        MatchWildcards: false,      
                        MatchSoundsLike: missing,   
                        MatchAllWordForms: false,   
                        Forward: true,              
                        Wrap: wrap,                 
                        Format: false,             
                        ReplaceWith: missing,       
                        Replace: replace);          
                }

                Object newFileName = Path.Combine(fileInfo.DirectoryName, DateTime.Now.ToString("yyyyMMdd HHmmss") + " - otchet");
                app.ActiveDocument.SaveAs2(newFileName);
                app.ActiveDocument.Close();
                app.Quit();

                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("The something is went wrong ( " + e.Message + ")");
            }
            return false;
        }
    }
}
