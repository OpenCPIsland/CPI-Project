using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace UnityTest
{
	public class DTOFormatter
	{
		private interface ITransferInterface
		{
			void Transfer(ref ResultDTO.MessageType val);

			void Transfer(ref TestResultState val);

			void Transfer(ref byte val);

			void Transfer(ref bool val);

			void Transfer(ref int val);

			void Transfer(ref float val);

			void Transfer(ref double val);

			void Transfer(ref string val);
		}

		private class Writer : ITransferInterface
		{
			private readonly Stream _stream;

			public Writer(Stream stream)
			{
				_stream = stream;
			}

			private void WriteConvertedNumber(byte[] bytes)
			{
				if (BitConverter.IsLittleEndian)
				{
					Array.Reverse(bytes);
				}
				_stream.Write(bytes, 0, bytes.Length);
			}

			public void Transfer(ref ResultDTO.MessageType val)
			{
				_stream.WriteByte((byte)val);
			}

			public void Transfer(ref TestResultState val)
			{
				_stream.WriteByte((byte)val);
			}

			public void Transfer(ref byte val)
			{
				_stream.WriteByte(val);
			}

			public void Transfer(ref bool val)
			{
				_stream.WriteByte((byte)(val ? 1 : 0));
			}

			public void Transfer(ref int val)
			{
				WriteConvertedNumber(BitConverter.GetBytes(val));
			}

			public void Transfer(ref float val)
			{
				WriteConvertedNumber(BitConverter.GetBytes(val));
			}

			public void Transfer(ref double val)
			{
				WriteConvertedNumber(BitConverter.GetBytes(val));
			}

			public void Transfer(ref string val)
			{
				byte[] bytes = Encoding.BigEndianUnicode.GetBytes(val);
				int val2 = bytes.Length;
				Transfer(ref val2);
				_stream.Write(bytes, 0, bytes.Length);
			}
		}

		private class Reader : ITransferInterface
		{
			private readonly Stream _stream;

			public Reader(Stream stream)
			{
				_stream = stream;
			}

			private byte[] ReadConvertedNumber(int size)
			{
				byte[] array = new byte[size];
				_stream.Read(array, 0, array.Length);
				if (BitConverter.IsLittleEndian)
				{
					Array.Reverse(array);
				}
				return array;
			}

			public void Transfer(ref ResultDTO.MessageType val)
			{
				val = (ResultDTO.MessageType)_stream.ReadByte();
			}

			public void Transfer(ref TestResultState val)
			{
				val = (TestResultState)_stream.ReadByte();
			}

			public void Transfer(ref byte val)
			{
				val = (byte)_stream.ReadByte();
			}

			public void Transfer(ref bool val)
			{
				val = (_stream.ReadByte() != 0);
			}

			public void Transfer(ref int val)
			{
				val = BitConverter.ToInt32(ReadConvertedNumber(4), 0);
			}

			public void Transfer(ref float val)
			{
				val = BitConverter.ToSingle(ReadConvertedNumber(4), 0);
			}

			public void Transfer(ref double val)
			{
				val = BitConverter.ToDouble(ReadConvertedNumber(8), 0);
			}

			public void Transfer(ref string val)
			{
				int val2 = 0;
				Transfer(ref val2);
				byte[] array = new byte[val2];
				int num = val2;
				int num2 = 0;
				do
				{
					int num3 = _stream.Read(array, num2, num);
					num -= num3;
					num2 += num3;
				}
				while (num > 0);
				val = Encoding.BigEndianUnicode.GetString(array);
			}
		}

		private void Transfer(ResultDTO dto, ITransferInterface transfer)
		{
			transfer.Transfer(ref dto.messageType);
			transfer.Transfer(ref dto.levelCount);
			transfer.Transfer(ref dto.loadedLevel);
			transfer.Transfer(ref dto.loadedLevelName);
			if (dto.messageType == ResultDTO.MessageType.Ping || dto.messageType == ResultDTO.MessageType.RunStarted || dto.messageType == ResultDTO.MessageType.RunFinished || dto.messageType == ResultDTO.MessageType.RunInterrupted || dto.messageType == ResultDTO.MessageType.AllScenesFinished)
			{
				return;
			}
			transfer.Transfer(ref dto.testName);
			transfer.Transfer(ref dto.testTimeout);
			if (dto.messageType != ResultDTO.MessageType.TestStarted)
			{
				if (transfer is Reader)
				{
					dto.testResult = new SerializableTestResult();
				}
				SerializableTestResult serializableTestResult = (SerializableTestResult)dto.testResult;
				transfer.Transfer(ref serializableTestResult.resultState);
				transfer.Transfer(ref serializableTestResult.message);
				transfer.Transfer(ref serializableTestResult.executed);
				transfer.Transfer(ref serializableTestResult.name);
				transfer.Transfer(ref serializableTestResult.fullName);
				transfer.Transfer(ref serializableTestResult.id);
				transfer.Transfer(ref serializableTestResult.isSuccess);
				transfer.Transfer(ref serializableTestResult.duration);
				transfer.Transfer(ref serializableTestResult.stackTrace);
			}
		}

		public void Serialize(Stream stream, ResultDTO dto)
		{
			Transfer(dto, new Writer(stream));
		}

		public object Deserialize(Stream stream)
		{
			ResultDTO resultDTO = (ResultDTO)FormatterServices.GetSafeUninitializedObject(typeof(ResultDTO));
			Transfer(resultDTO, new Reader(stream));
			return resultDTO;
		}
	}
}
