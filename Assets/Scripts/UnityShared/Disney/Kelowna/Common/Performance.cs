#define ENABLE_PROFILER
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using LitJson;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Tweaker.Core;
using UnityEngine;
using UnityEngine.Profiling;

namespace Disney.Kelowna.Common
{
	public class Performance
	{
		[Flags]
		public enum MemoryType
		{
			Mesh = 0x1,
			Texture = 0x2,
			Audio = 0x4,
			Animation = 0x8,
			Materials = 0x10,
			Other = 0x20
		}

		public interface IJsonSerializable
		{
			JsonData ToJson();

			void FromJson(JsonData json);
		}

		public interface IMetric : IJsonSerializable
		{
			void Update();

			void ForceUpdate();

			void ResetValues();
		}

		public class Metric<T> : IMetric, IJsonSerializable where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
		{
			public double MovingAvgAlpha = 0.3;

			public ulong UpdatorRate = 1uL;

			private ulong updatorCount;

			private Func<T, string> formatter;

			private Func<T> updator;

			private bool defaults = false;

			public string Name
			{
				get;
				private set;
			}

			public T Value
			{
				get;
				private set;
			}

			public string ValueFormatted
			{
				get
				{
					return formatter(Value);
				}
			}

			public double ExpMovingAverage
			{
				get;
				private set;
			}

			public string ExpMovingAverageFormatted
			{
				get
				{
					return formatter(convertFrom(ExpMovingAverage));
				}
			}

			public double Average
			{
				get;
				private set;
			}

			public string AverageFormatted
			{
				get
				{
					return formatter(convertFrom(Average));
				}
			}

			public T Min
			{
				get;
				private set;
			}

			public string MinFormatted
			{
				get
				{
					return formatter(Min);
				}
			}

			public T Max
			{
				get;
				private set;
			}

			public string MaxFormatted
			{
				get
				{
					return formatter(Max);
				}
			}

			public ulong UpdateCount
			{
				get;
				private set;
			}

			public Metric(string name, string formatter, Func<T> updator = null, ulong updatorRate = 1uL)
				: this(name, (Func<T, string>)((T v) => string.Format(formatter, v)), updator, updatorRate)
			{
			}

			public Metric(string name, Func<T, string> formatter = null, Func<T> updator = null, ulong updatorRate = 1uL)
			{
				if (formatter == null)
				{
					formatter = ((T value) => string.Format("{0:0.0}", value));
				}
				this.formatter = formatter;
				Name = name;
				this.updator = updator;
				UpdatorRate = Math.Max(1uL, updatorRate);
				ResetValues();
			}

			public void ResetValues()
			{
				defaults = true;
				Value = default(T);
				Average = 0.0;
				ExpMovingAverage = 0.0;
				Max = default(T);
				Min = default(T);
				UpdateCount = 0uL;
				updatorCount = 0uL;
			}

			public void Update()
			{
				update(false);
			}

			public void ForceUpdate()
			{
				update(true);
			}

			private void update(bool force)
			{
				if (updator != null)
				{
					if (updatorCount % UpdatorRate == 0 || force)
					{
						UpdateValue(updator());
					}
					updatorCount++;
				}
			}

			public void UpdateValueIncrementalReset()
			{
				Value = default(T);
			}

			public void UpdateValueIncremental(T next)
			{
				if (defaults)
				{
					double num3 = ExpMovingAverage = (Average = ConvertTo<double>(next));
					Min = next;
					Max = next;
					defaults = false;
				}
				Value = next;
			}

			public void UpdateValueIncrementalDone()
			{
				double num = ConvertTo<double>(Value);
				ExpMovingAverage = MovingAvgAlpha * num + (1.0 - MovingAvgAlpha) * ExpMovingAverage;
				Average = (Average * (double)UpdateCount + num) / (double)(UpdateCount + 1);
				Min = min(Min, Value);
				Max = max(Max, Value);
				UpdateCount++;
			}

			public void UpdateValue(T next)
			{
				UpdateValueIncrementalReset();
				UpdateValueIncremental(next);
				UpdateValueIncrementalDone();
			}

			public static TTarget ConvertTo<TTarget>(T value)
			{
				return convert<TTarget, T>(value);
			}

			private static T convertFrom<TTarget>(TTarget value)
			{
				return convert<T, TTarget>(value);
			}

			private static TConverted convert<TConverted, TSource>(TSource value)
			{
				switch (Type.GetTypeCode(typeof(TConverted)))
				{
				case TypeCode.Int16:
					return (TConverted)(object)Convert.ToInt16(value);
				case TypeCode.UInt16:
					return (TConverted)(object)Convert.ToUInt16(value);
				case TypeCode.Int32:
					return (TConverted)(object)Convert.ToInt32(value);
				case TypeCode.UInt32:
					return (TConverted)(object)Convert.ToUInt32(value);
				case TypeCode.Int64:
					return (TConverted)(object)Convert.ToInt64(value);
				case TypeCode.UInt64:
					return (TConverted)(object)Convert.ToUInt64(value);
				case TypeCode.Single:
					return (TConverted)(object)Convert.ToSingle(value);
				case TypeCode.Double:
					return (TConverted)(object)Convert.ToDouble(value);
				default:
					throw new InvalidOperationException("Unsupported type passed to Metric.convert<" + typeof(TConverted).FullName + ", " + typeof(TSource).FullName + ">");
				}
			}

			private static T max(T a, T b)
			{
				return (Comparer<T>.Default.Compare(a, b) > 0) ? a : b;
			}

			private static T min(T a, T b)
			{
				return (Comparer<T>.Default.Compare(a, b) > 0) ? b : a;
			}

			public JsonData ToJson()
			{
				JsonData jsonData = new JsonData();
				jsonData["name"] = Name;
				jsonData["value"] = new JsonData(Value);
				jsonData["average"] = Average;
				jsonData["expMovingAverage"] = ExpMovingAverage;
				jsonData["min"] = new JsonData(Min);
				jsonData["max"] = new JsonData(Max);
				return jsonData;
			}

			public void FromJson(JsonData json)
			{
				Name = (string)json["name"];
				Value = ConvertJsonData(json["value"]);
				Average = (double)json["average"];
				ExpMovingAverage = (double)json["expMovingAverage"];
				Min = ConvertJsonData(json["min"]);
				Max = ConvertJsonData(json["max"]);
			}

			public static T ConvertJsonData(JsonData data)
			{
				T result = default(T);
				switch (data.GetJsonType())
				{
				case JsonType.Double:
					return convertFrom((double)data);
				case JsonType.Int:
					return convertFrom((int)data);
				case JsonType.Long:
					return convertFrom((long)data);
				default:
					return result;
				}
			}
		}

		public const float KILO_BYTE = 1024f;

		public const float MEGA_BYTE = 1048576f;

		private static readonly Dictionary<MemoryType, Type> memoryTypeToEngineType = new Dictionary<MemoryType, Type>
		{
			{
				MemoryType.Mesh,
				typeof(Mesh)
			},
			{
				MemoryType.Texture,
				typeof(Texture)
			},
			{
				MemoryType.Audio,
				typeof(AudioClip)
			},
			{
				MemoryType.Animation,
				typeof(AnimationClip)
			},
			{
				MemoryType.Materials,
				typeof(Material)
			},
			{
				MemoryType.Other,
				typeof(UnityEngine.Object)
			}
		};

		private Dictionary<string, IMetric> metrics = new Dictionary<string, IMetric>();

		[Tweakable("Debug.Performance.ShowAssetMemory")]
		public bool AutomaticMemorySampling;

		[Tweakable("Debug.Performance.TrackPUBMemory")]
		public bool TrackProcessUsedMemory = true;

		[Tweakable("Debug.Performance.PUBMemoryUpdateRate")]
		public uint ProcessUsedMemoryUpdateRate = 150u;

		private float lastTime = 0f;

		private ulong frameCount = 0uL;

		private float lastFPSUpdate = 0f;

		private float lastAssetUpdate = 0f;

		private readonly Dictionary<Type, Metric<int>> engineTypeToMetric;

		private readonly Dictionary<MemoryType, Metric<int>> memoryTypeToMetric;

		private string currentPerfReport = string.Empty;

		private float currentZoneStart;

		private static readonly char[] pathSeparators = new char[2]
		{
			'/',
			'\\'
		};

		public Metric<float> FrameTime
		{
			get;
			private set;
		}

		public Metric<float> FramesPerSecond
		{
			get;
			private set;
		}

		public float TargetFrameTime
		{
			get
			{
				return 1000f / (float)Application.targetFrameRate;
			}
		}

		public int SystemMemoryLimit
		{
			get;
			private set;
		}

		public Metric<long> CodeMemoryUsed
		{
			get;
			private set;
		}

		public Metric<long> CodeMemoryFree
		{
			get;
			private set;
		}

		public Metric<int> MonoMemoryReserved
		{
			get;
			private set;
		}

		public Metric<int> MeshMemory
		{
			get;
			private set;
		}

		public Metric<int> TextureMemory
		{
			get;
			private set;
		}

		public Metric<int> AudioMemory
		{
			get;
			private set;
		}

		public Metric<int> AnimationMemory
		{
			get;
			private set;
		}

		public Metric<int> MaterialsMemory
		{
			get;
			private set;
		}

		public Metric<int> OtherAssetMemory
		{
			get;
			private set;
		}

		public Metric<int> AssetMemory
		{
			get;
			private set;
		}

		public Metric<int> AssetCount
		{
			get;
			private set;
		}

		public Metric<int> TextureCount
		{
			get;
			private set;
		}

		public Metric<long> TotalMemoryUsed
		{
			get;
			private set;
		}

		public Metric<ulong> ProcessUsedBytes
		{
			get;
			private set;
		}

		public Dictionary<MemoryType, Metric<int>>.Enumerator MemoryMetricsEnumerator
		{
			get
			{
				return memoryTypeToMetric.GetEnumerator();
			}
		}

		public bool SaveZoneUpdates
		{
			get;
			set;
		}

		public static Type GetMemoryClassType(MemoryType memoryType)
		{
			return memoryTypeToEngineType[memoryType];
		}

		public void AddMetric<T>(Metric<T> metric) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
		{
			metrics.Add(metric.Name, metric);
		}

		public Metric<T> GetMetric<T>(string name) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
		{
			IMetric value;
			if (!metrics.TryGetValue(name, out value))
			{
				return null;
			}
			return (Metric<T>)value;
		}

		public Performance()
		{
			lastTime = Time.realtimeSinceStartup;
			lastFPSUpdate = Time.realtimeSinceStartup;
			lastAssetUpdate = Time.realtimeSinceStartup;
			SystemMemoryLimit = 200;
			AddMetric(FrameTime = new Metric<float>("FrameTime", "{0:0.0} ms", null, 1uL));
			AddMetric(FramesPerSecond = new Metric<float>("FramesPerSecond", "FPS: {0:####}", null, 1uL));
			AddMetric(CodeMemoryUsed = new Metric<long>("CodeMemoryUsed", getMemoryFormatter<long>("um"), () => Profiler.GetTotalAllocatedMemoryLong(), 1uL));
			AddMetric(CodeMemoryFree = new Metric<long>("CodeMemoryFree", getMemoryFormatter<long>("fm"), () => Profiler.GetTotalUnusedReservedMemoryLong(), 1uL));
			AddMetric(MeshMemory = new Metric<int>("MeshMemory", getMemoryFormatter<int>("Meshes"), null, 1uL));
			AddMetric(TextureMemory = new Metric<int>("TextureMemory", getMemoryFormatter<int>("Textures"), null, 1uL));
			AddMetric(AudioMemory = new Metric<int>("AudioMemory", getMemoryFormatter<int>("Audio"), null, 1uL));
			AddMetric(AnimationMemory = new Metric<int>("AnimationMemory", getMemoryFormatter<int>("Animation"), null, 1uL));
			AddMetric(MaterialsMemory = new Metric<int>("MaterialsMemory", getMemoryFormatter<int>("Materials"), null, 1uL));
			AddMetric(OtherAssetMemory = new Metric<int>("OtherMemory", getMemoryFormatter<int>("Other"), null, 1uL));
			AddMetric(AssetMemory = new Metric<int>("AssetMemory", getMemoryFormatter<int>("Assets"), null, 1uL));
			AddMetric(TotalMemoryUsed = new Metric<long>("TotalMemoryUsed", getMemoryFormatter<long>("Total"), () => CodeMemoryUsed.Value + CodeMemoryFree.Value + AssetMemory.Value, 1uL));
			AddMetric(AssetCount = new Metric<int>("AssetsCount", "a:{0:0}", null, 1uL));
			AddMetric(TextureCount = new Metric<int>("TextureCount", "Texture#:{0:0}", null, 1uL));
			AddMetric(ProcessUsedBytes = new Metric<ulong>("ProcessUsedBytes", getMemoryFormatter<ulong>("pub"), () => TrackProcessUsedMemory ? Service.Get<MemoryMonitorManager>().GetProcessUsedBytes() : 0, ProcessUsedMemoryUpdateRate));
			engineTypeToMetric = new Dictionary<Type, Metric<int>>
			{
				{
					typeof(Mesh),
					MeshMemory
				},
				{
					typeof(Texture),
					TextureMemory
				},
				{
					typeof(AudioClip),
					AudioMemory
				},
				{
					typeof(AnimationClip),
					AnimationMemory
				},
				{
					typeof(Material),
					MaterialsMemory
				},
				{
					typeof(UnityEngine.Object),
					OtherAssetMemory
				}
			};
			memoryTypeToMetric = new Dictionary<MemoryType, Metric<int>>
			{
				{
					MemoryType.Mesh,
					MeshMemory
				},
				{
					MemoryType.Texture,
					TextureMemory
				},
				{
					MemoryType.Audio,
					AudioMemory
				},
				{
					MemoryType.Animation,
					AnimationMemory
				},
				{
					MemoryType.Materials,
					MaterialsMemory
				},
				{
					MemoryType.Other,
					OtherAssetMemory
				}
			};
			SaveZoneUpdates = false;
			StartZone();
		}

		public void BeginFrame()
		{
		}

		private void updateMetrics()
		{
			foreach (KeyValuePair<string, IMetric> metric in metrics)
			{
				metric.Value.Update();
			}
		}

		[Conditional("ENABLE_PROFILER")]
		[Invokable("Debug.Performance.RefreshAssetMemory")]
		public void UpdateAssetMemoryUsage(MemoryType memoryTypeFlags = (MemoryType)2147483647)
		{
			int num = 0;
			long num2 = 0L;
			foreach (KeyValuePair<Type, Metric<int>> item in engineTypeToMetric)
			{
				item.Value.UpdateValueIncrementalReset();
			}
			UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(UnityEngine.Object));
			num += array.Length;
			foreach (UnityEngine.Object @object in array)
			{
				Type baseEngineType = getBaseEngineType(@object.GetType());
				Metric<int> value;
				if (!engineTypeToMetric.TryGetValue(baseEngineType, out value))
				{
					value = OtherAssetMemory;
				}
				int num3 = (int)Profiler.GetRuntimeMemorySizeLong(@object);
				value.UpdateValueIncremental(value.Value + num3);
				num2 += num3;
			}
			AssetMemory.UpdateValue((int)num2);
			AssetCount.UpdateValue(num);
		}

		public void EndFrame()
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			float num = realtimeSinceStartup - lastAssetUpdate;
			if (num >= 10f)
			{
				lastAssetUpdate = realtimeSinceStartup;
				if (AutomaticMemorySampling)
				{
					UpdateAssetMemoryUsage();
				}
			}
			updateMetrics();
			num = realtimeSinceStartup - lastTime;
			lastTime = realtimeSinceStartup;
			FrameTime.UpdateValue(num * 1000f);
			frameCount++;
			num = realtimeSinceStartup - lastFPSUpdate;
			if (num >= 1f)
			{
				lastFPSUpdate = lastTime;
				FramesPerSecond.UpdateValue((int)Math.Round((float)frameCount / num));
				frameCount = 0uL;
			}
		}

		public void StartUnityProfiling()
		{
			if (!Profiler.supported)
			{
				throw new NotSupportedException();
			}
			string date = DateTime.Now.ToUniversalTime().ToString("u");
			Profiler.logFile = getProfileFileName(date);
			Profiler.enableBinaryLog = true;
			Profiler.enabled = true;
		}

		public void StopUnityProfiling()
		{
			if (!Profiler.supported)
			{
				throw new NotSupportedException();
			}
			if (Profiler.enabled)
			{
				Profiler.enabled = false;
			}
		}

		private string getMemoryFormat(string label)
		{
			return label + ":{0:0.##}MB";
		}

		private Func<T, string> getMemoryFormatter<T>(string label) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
		{
			return (T v) => string.Format(getMemoryFormat(label), Metric<T>.ConvertTo<float>(v) / 1048576f);
		}

		private Type getBaseEngineType(Type type)
		{
			Type result = typeof(UnityEngine.Object);
			foreach (KeyValuePair<Type, Metric<int>> item in engineTypeToMetric)
			{
				Type key = item.Key;
				if (key != typeof(UnityEngine.Object) && key.IsAssignableFrom(type))
				{
					result = key;
					break;
				}
			}
			return result;
		}

		private string getProfileFileName(string date)
		{
			return string.Format("ft-profile-{0}-{1}-{2}", ClientInfo.Instance.Platform, ClientInfo.Instance.BuildVersion, date);
		}

		private void createProfileLog()
		{
			string date = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd-THH.mm.ss");
			string str = Application.persistentDataPath + "/profile/" + getProfileFileName(date);
			str = (currentPerfReport = str + ".json");
			PerformanceReport performanceReport = new PerformanceReport();
			performanceReport.Platform = ClientInfo.Instance.Platform;
			performanceReport.OperatingSystem = SystemInfo.operatingSystem;
			performanceReport.Device = SystemInfo.deviceModel;
			performanceReport.BundleId = Application.identifier;
			performanceReport.Version = ClientInfo.Instance.BuildVersion;
			performanceReport.UnityVersion = Application.unityVersion;
			performanceReport.Date = date;
			performanceReport.StartTime = DateTime.Now.ToUniversalTime().ToFileTime();
			PerformanceReport performanceReport2 = performanceReport;
			saveToFileSystem(str, performanceReport2.ToJson());
		}

		public void ResetMetrics()
		{
			foreach (KeyValuePair<string, IMetric> metric in metrics)
			{
				metric.Value.ResetValues();
			}
		}

		public void StartZone()
		{
			currentZoneStart = Time.realtimeSinceStartup;
		}

		[Conditional("CP_PERF_ZONE_PROFILING")]
		[Conditional("UNITY_EDITOR")]
		public static void AddMetric<T>(string name, string format = "{0:0.0}", Func<T> updator = null) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
		{
			Performance performance = Service.Get<Performance>();
			if (performance != null)
			{
				performance.AddMetric(new Metric<T>(name, format, updator, 1uL));
			}
		}

		[Conditional("UNITY_EDITOR")]
		[Conditional("CP_PERF_ZONE_PROFILING")]
		public static void EndZone(string name)
		{
			Performance performance = Service.Get<Performance>();
			if (performance != null)
			{
				performance.endZoneImpl(name);
			}
		}

		private void endZoneImpl(string name)
		{
			if (!SaveZoneUpdates)
			{
				endZoneNoSave();
				return;
			}
			PerformanceReport performanceReport = new PerformanceReport();
			if (!loadFromFileSystem(currentPerfReport, performanceReport))
			{
				Log.LogErrorFormatted(this, "EndZone, Failed to load performance report file: {0}", currentPerfReport);
				return;
			}
			PerformanceZoneReport performanceZoneReport = new PerformanceZoneReport();
			performanceZoneReport.Name = name;
			performanceZoneReport.StartTime = currentZoneStart * 1000f;
			performanceZoneReport.Length = (Time.realtimeSinceStartup - currentZoneStart) * 1000f;
			performanceZoneReport.Frame = Time.frameCount;
			PerformanceZoneReport item = performanceZoneReport;
			updateMetrics();
			UpdateAssetMemoryUsage();
			endZoneNoSave();
			performanceReport.Zones.Add(item);
			saveToFileSystem(currentPerfReport, performanceReport.ToJson());
		}

		private void endZoneNoSave()
		{
			foreach (KeyValuePair<string, IMetric> metric in metrics)
			{
				IMetric value = metric.Value;
				value.ResetValues();
			}
			updateMetrics();
			UpdateAssetMemoryUsage();
			StartZone();
		}

		private static void saveToFileSystem(string path, JsonData json)
		{
			EnsureFilePathExists(path);
			File.WriteAllText(path, json.ToJsonPretty());
		}

		private static bool loadFromFileSystem(string path, PerformanceReport profile)
		{
			try
			{
				string json = File.ReadAllText(path);
				JsonData json2 = JsonMapper.ToObject(json);
				profile.FromJson(json2);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public static void EnsureFilePathExists(string filepath)
		{
			int num = filepath.LastIndexOfAny(pathSeparators);
			string str = (num != -1) ? filepath.Substring(0, num) : filepath;
			string currentDirectory = Directory.GetCurrentDirectory();
			currentDirectory += '/';
			currentDirectory += str;
			Directory.CreateDirectory(currentDirectory);
		}
	}
}
