using Kopilych.Application.Interfaces;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Mobile.Services
{
    public class FileService : IFileService
    {
        public async Task<byte[]> CreateFileAsync(string filePath, byte[] data, bool overwrite)
        {
            if (!overwrite && File.Exists(filePath))
            {
                throw new IOException("File already exists and overwrite is not allowed.");
            }

            // Запись данных в файл асинхронно
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await fs.WriteAsync(data, 0, data.Length);
            }

            return data; // Возвращаем записанные данные
        }

        public bool Exist(string filePath)
        {
            return File.Exists(filePath);
        }

        public async Task<byte[]> ReadFileAsync(string filePath)
        {
            if (!Exist(filePath))
            {
                throw new FileNotFoundException("File not found.", filePath);
            }

            // Чтение данных из файла асинхронно
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                byte[] data = new byte[fs.Length];
                await fs.ReadAsync(data, 0, (int)fs.Length);
                return data;
            }
        }
        public async Task RemoveFileAsync(string filePath)
        {
            if (!Exist(filePath))
            {
                throw new FileNotFoundException("File not found.", filePath);
            }

            // Удаление файла асинхронно
            await Task.Run(() => File.Delete(filePath));
        }

        public async Task SaveImageAsJpegAsync(byte[] inputBytes, string savePath)
        {
            // Декодируем байты в SKBitmap
            using var inputStream = new MemoryStream(inputBytes);
            var bitmap = SKBitmap.Decode(inputStream);

            if (bitmap == null)
            {
                throw new Exception("Не удалось декодировать изображение");
            }

            // Сохраняем в JPEG
            using var outData = bitmap.Encode(SKEncodedImageFormat.Jpeg, 100); // качество 100%
            using var outStream = File.Create(savePath);
            outData.SaveTo(outStream);
        }

        public string GetDefaultPath()
        {
            return FileSystem.AppDataDirectory;
        }
    }
}
