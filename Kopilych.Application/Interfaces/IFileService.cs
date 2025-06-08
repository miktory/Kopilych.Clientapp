using Kopilych.Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.Interfaces
{
	public interface IFileService
	{
		public Task RemoveFileAsync(string filePath);
		public Task<byte[]> ReadFileAsync(string filePath);
		public Task<byte[]> CreateFileAsync(string filePath, byte[] data, bool overwrite);
		public bool Exist(string filePath);
		public Task SaveImageAsJpegAsync(byte[] inputBytes, string savePath);

        public string GetDefaultPath();

    }
}
