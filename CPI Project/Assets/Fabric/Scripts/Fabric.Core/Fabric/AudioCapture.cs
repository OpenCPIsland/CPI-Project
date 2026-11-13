using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Fabric
{
	[AddComponentMenu("Fabric/DSP/AudioCapture")]
	public class AudioCapture : MonoBehaviour
	{
		private const int HEADER_SIZE = 44;

		private List<float> samples;

		[HideInInspector]
		[SerializeField]
		public bool _capture;

		private void Awake()
		{
			samples = new List<float>();
		}

		private void OnAudioFilterRead(float[] data, int channels)
		{
			if (_capture)
			{
				for (int i = 0; i < data.Length; i++)
				{
					samples.Add(data[i]);
				}
			}
		}

		public bool Save(string filename)
		{
			if (!filename.ToLower().EndsWith(".wav"))
			{
				filename += ".wav";
			}
			string text = Path.Combine(Application.persistentDataPath, filename);
			Debug.Log(text);
			Directory.CreateDirectory(Path.GetDirectoryName(text));
			using (FileStream fileStream = CreateEmpty(text))
			{
				ConvertAndWrite(fileStream, samples.ToArray());
				WriteHeader(fileStream, 44100, 2, samples.Count);
				fileStream.Close();
			}
			return true;
		}

		public static bool Save(string filename, float[] source = null)
		{
			if (!filename.ToLower().EndsWith(".wav"))
			{
				filename += ".wav";
			}
			using (FileStream fileStream = CreateEmpty(filename))
			{
				ConvertAndWrite(fileStream, source);
				WriteHeader(fileStream, 44100, 1, source.Length);
				fileStream.Close();
			}
			return true;
		}

		private static FileStream CreateEmpty(string filepath)
		{
			FileStream fileStream = new FileStream(filepath, FileMode.Create);
			byte value = 0;
			for (int i = 0; i < 44; i++)
			{
				fileStream.WriteByte(value);
			}
			return fileStream;
		}

		private static void ConvertAndWrite(FileStream fileStream, float[] samples)
		{
			short[] array = new short[samples.Length];
			byte[] array2 = new byte[samples.Length * 2];
			int num = 32767;
			for (int i = 0; i < samples.Length; i++)
			{
				array[i] = (short)(samples[i] * (float)num);
				byte[] array3 = new byte[2];
				array3 = BitConverter.GetBytes(array[i]);
				array3.CopyTo(array2, i * 2);
			}
			fileStream.Write(array2, 0, array2.Length);
		}

		private static void WriteHeader(FileStream fileStream, int hz, int channels, int samples)
		{
			fileStream.Seek(0L, SeekOrigin.Begin);
			byte[] bytes = Encoding.UTF8.GetBytes("RIFF");
			fileStream.Write(bytes, 0, 4);
			byte[] bytes2 = BitConverter.GetBytes(fileStream.Length - 8);
			fileStream.Write(bytes2, 0, 4);
			byte[] bytes3 = Encoding.UTF8.GetBytes("WAVE");
			fileStream.Write(bytes3, 0, 4);
			byte[] bytes4 = Encoding.UTF8.GetBytes("fmt ");
			fileStream.Write(bytes4, 0, 4);
			byte[] bytes5 = BitConverter.GetBytes(16);
			fileStream.Write(bytes5, 0, 4);
			ushort value = 1;
			byte[] bytes6 = BitConverter.GetBytes(value);
			fileStream.Write(bytes6, 0, 2);
			byte[] bytes7 = BitConverter.GetBytes(channels);
			fileStream.Write(bytes7, 0, 2);
			byte[] bytes8 = BitConverter.GetBytes(hz);
			fileStream.Write(bytes8, 0, 4);
			byte[] bytes9 = BitConverter.GetBytes(hz * channels * 2);
			fileStream.Write(bytes9, 0, 4);
			ushort value2 = (ushort)(channels * 2);
			fileStream.Write(BitConverter.GetBytes(value2), 0, 2);
			ushort value3 = 16;
			byte[] bytes10 = BitConverter.GetBytes(value3);
			fileStream.Write(bytes10, 0, 2);
			byte[] bytes11 = Encoding.UTF8.GetBytes("data");
			fileStream.Write(bytes11, 0, 4);
			byte[] bytes12 = BitConverter.GetBytes(samples * channels * 2);
			fileStream.Write(bytes12, 0, 4);
		}
	}
}
