// See https://aka.ms/new-console-template for more information
using System.Security.Cryptography;

namespace DepedencyInjectionContainer
{


	interface DATABASE
	{
		public void WriteData();
	}

	interface Encryption
	{
		
		public string Encryption();
		public string Decryption();

		public void SetValue(string value);
	}

	enum SupportedDependencies
	{
		DATABASE,
		Encryption,

	}

	enum SupportedImplementation 
	{
		MYSQL,
		AESEncryption
	}


	class AESEncryption : Encryption
	{
		
		private string _value;

		private byte[] encrypted_value;

		private AesManaged aes = new AesManaged();

		public void SetValue(string value)
		{
			_value = value;
		}
		public string Decryption()
		{
			string decrypted = DecryptAESImpl(encrypted_value, aes.Key, aes.IV);
			return decrypted;
		}


		public string Encryption()
		{
			try
			{
				
		
				encrypted_value = EncryptAESImpl(_value, aes.Key, aes.IV);
				return encrypted_value.ToString();
		

			}
			catch (Exception exp)
			{
				return "Not Encrypted";
				Console.WriteLine(exp.Message);
			}
		}
		static byte[] EncryptAESImpl(string plainText, byte[] Key, byte[] IV)
		{
			byte[] encrypted;
			// Create a new AesManaged.
			using (AesManaged aes = new AesManaged())
			{
				// Create encryptor
				ICryptoTransform encryptor = aes.CreateEncryptor(Key, IV);
				// Create MemoryStream
				using (MemoryStream ms = new MemoryStream())
				{
					// Create crypto stream using the CryptoStream class. This class is the key to encryption
					// and encrypts and decrypts data from any given stream. In this case, we will pass a memory stream
					// to encrypt
					using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
					{
						// Create StreamWriter and write data to a stream
						using (StreamWriter sw = new StreamWriter(cs))
							sw.Write(plainText);
						encrypted = ms.ToArray();
					}
				}
			}
			// Return encrypted data
			return encrypted;
		}

		static string DecryptAESImpl(byte[] cipherText, byte[] Key, byte[] IV)
		{
			string plaintext = null;
			// Create AesManaged
			using (AesManaged aes = new AesManaged())
			{
				// Create a decryptor
				ICryptoTransform decryptor = aes.CreateDecryptor(Key, IV);
				// Create the streams used for decryption.
				using (MemoryStream ms = new MemoryStream(cipherText))
				{
					// Create crypto stream
					using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
					{
						// Read crypto stream
						using (StreamReader reader = new StreamReader(cs))
							plaintext = reader.ReadToEnd();
					}
				}
			}
			return plaintext;
		}

	}


	class MYSQL: DATABASE
	{
		public MYSQL() {

		}

		public void WriteData()
		{
			Console.WriteLine("Writing Data");
		}
	}

	class DependencyInjectionContainer
	{
		private Dictionary <SupportedDependencies, SupportedImplementation> 
			_dependencies = new Dictionary<SupportedDependencies, SupportedImplementation> ();



		private Dictionary<SupportedDependencies, object> _instances = new Dictionary<SupportedDependencies, object>();

		public DependencyInjectionContainer(){
		
			}

		public void CreateDependency(SupportedDependencies DependencyName, SupportedImplementation DependencyImplementation)
		{
			_dependencies.Add(DependencyName, DependencyImplementation);
		}


		private Type DependencyContainerGetType(string type)
		{
			Type targetType = Type.GetType("DepedencyInjectionContainer."+type);
			return targetType;
		}

		public void CreateInstances()
		{
			foreach (var dependency in _dependencies)
			{
				string className = Enum.GetName(typeof(SupportedImplementation), dependency.Value);
				Type type = DependencyContainerGetType(className);
				Console.WriteLine(type);
				object instance = Activator.CreateInstance(type);
				_instances.Add(dependency.Key, instance);
			}
		}


		public T GetInstance<T>(SupportedDependencies Dependency) where T : class
		{
			return (T)_instances[Dependency];
		}

	}

	class Program
	{
		static public void Main(String[] args)
		{

			var dependency_injection_container = new DependencyInjectionContainer();
			dependency_injection_container.CreateDependency(SupportedDependencies.DATABASE, SupportedImplementation.MYSQL);
			dependency_injection_container.CreateDependency(SupportedDependencies.Encryption, SupportedImplementation.AESEncryption);

			dependency_injection_container.CreateInstances();
			var Encryption = dependency_injection_container.GetInstance<Encryption>(SupportedDependencies.Encryption);
			var Database = dependency_injection_container.GetInstance<DATABASE>(SupportedDependencies.DATABASE);

			Database.WriteData();

			Encryption.SetValue("This is a random message");
			var encrypted = Encryption.Encryption();
			var decrypted = Encryption.Decryption();
			Console.WriteLine("value encrypted -> " + encrypted.ToString());
			Console.WriteLine("value decrypted -> " + decrypted);

		}

	}


}
