using System;
using System.Security.Cryptography;
using System.Text;

namespace Montr.Tools.DbMigrator.Services
{
	public interface IHashProvider
	{
		string GetHash(string value);
	}

	public class DefaultHashProvider : IHashProvider
	{
		private readonly MD5CryptoServiceProvider _md5Provider = new MD5CryptoServiceProvider();

		public string GetHash(string value)
		{
			lock (_md5Provider)
			{
				var hash = _md5Provider.ComputeHash(Encoding.Unicode.GetBytes(value));

				return new Guid(hash).ToString();
			}
		}
	}
}
