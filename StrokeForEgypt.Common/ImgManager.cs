using Microsoft.AspNetCore.Http;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace StrokeForEgypt.Common
{
    public class ImgManager
    {
        private readonly string WebRootPath;

        public ImgManager(string WebRootPath)
        {
            this.WebRootPath = WebRootPath;
        }

        public async Task<string> UploudImage(string DomainName, string FileName, IFormFile ImgFile, string FolderURL)
        {
            string now = DateTime.UtcNow.ToString("ddMMyyyyhhmmssfffffffK");

            string extension = Path.GetExtension(ImgFile.FileName);
            FileName += now + extension;
            string FileFullPath = Path.Combine(WebRootPath, FolderURL);
            string FilePath = Path.Combine(FileFullPath, FileName);

            // Create new local file and copy contents of uploaded file
            using (FileStream localFile = File.OpenWrite(FilePath))
            using (Stream uploadedFile = ImgFile.OpenReadStream())
            {
                await uploadedFile.CopyToAsync(localFile);
            }
            string ImgPath = DomainName + "/" + FolderURL + "/" + FileName;

            return ImgPath;
        }

        public void DeleteImage(string FilePath, string DomainName)
        {
            FilePath = FilePath.Replace(DomainName, "");
            string FileFullPath = WebRootPath + FilePath;
            // If file with same name exists delete it
            if (File.Exists(FileFullPath))
            {
                File.Delete(FileFullPath);
            }
        }

        public IFormFile ResizeImage(IFormFile ImgFile, int Width = 600, int Height = 600)
        {
            Image image = Image.FromStream(ImgFile.OpenReadStream(), true, true);
            Bitmap newImage = new Bitmap(Width, Height);
            using Graphics g = Graphics.FromImage(newImage);
            g.DrawImage(image, 0, 0, Width, Height);

            return ImgFile;
        }
    }
}
