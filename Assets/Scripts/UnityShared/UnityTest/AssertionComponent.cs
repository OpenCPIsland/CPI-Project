using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace UnityTest
{
	[Serializable]
	public class AssertionComponent : MonoBehaviour, IAssertionComponentConfigurator
	{
		[SerializeField]
		public float checkAfterTime = 1f;

		[SerializeField]
		public bool repeatCheckTime = true;

		[SerializeField]
		public float repeatEveryTime = 1f;

		[SerializeField]
		public int checkAfterFrames = 1;

		[SerializeField]
		public bool repeatCheckFrame = true;

		[SerializeField]
		public int repeatEveryFrame = 1;

		[SerializeField]
		public bool hasFailed;

		[SerializeField]
		public CheckMethod checkMethods = CheckMethod.Start;

		[SerializeField]
		private ActionBase m_ActionBase;

		[SerializeField]
		public int checksPerformed = 0;

		private int m_CheckOnFrame;

		private string m_CreatedInFilePath = "";

		private int m_CreatedInFileLine = -1;

		public ActionBase Action
		{
			get
			{
				return m_ActionBase;
			}
			set
			{
				m_ActionBase = value;
				m_ActionBase.go = base.gameObject;
			}
		}

		public int UpdateCheckStartOnFrame
		{
			set
			{
				checkAfterFrames = value;
			}
		}

		public int UpdateCheckRepeatFrequency
		{
			set
			{
				repeatEveryFrame = value;
			}
		}

		public bool UpdateCheckRepeat
		{
			set
			{
				repeatCheckFrame = value;
			}
		}

		public float TimeCheckStartAfter
		{
			set
			{
				checkAfterTime = value;
			}
		}

		public float TimeCheckRepeatFrequency
		{
			set
			{
				repeatEveryTime = value;
			}
		}

		public bool TimeCheckRepeat
		{
			set
			{
				repeatCheckTime = value;
			}
		}

		public AssertionComponent Component
		{
			get
			{
				return this;
			}
		}

		public UnityEngine.Object GetFailureReferenceObject()
		{
			return this;
		}

		public string GetCreationLocation()
		{
			if (!string.IsNullOrEmpty(m_CreatedInFilePath))
			{
				int startIndex = m_CreatedInFilePath.LastIndexOf("\\") + 1;
				return string.Format("{0}, line {1} ({2})", m_CreatedInFilePath.Substring(startIndex), m_CreatedInFileLine, m_CreatedInFilePath);
			}
			return "";
		}

		public void Awake()
		{
			if (!UnityEngine.Debug.isDebugBuild)
			{
				UnityEngine.Object.Destroy(this);
			}
			OnComponentCopy();
		}

		public void OnValidate()
		{
			if (Application.isEditor)
			{
				OnComponentCopy();
			}
		}

		private void OnComponentCopy()
		{
			if (m_ActionBase == null)
			{
				return;
			}
			IEnumerable<UnityEngine.Object> source = from o in Resources.FindObjectsOfTypeAll(typeof(AssertionComponent))
				where ((AssertionComponent)o).m_ActionBase == m_ActionBase && o != this
				select o;
			if (source.Any())
			{
				if (source.Count() > 1)
				{
					UnityEngine.Debug.LogWarning("More than one refence to comparer found. This shouldn't happen");
				}
				AssertionComponent assertionComponent = source.First() as AssertionComponent;
				m_ActionBase = assertionComponent.m_ActionBase.CreateCopy(assertionComponent.gameObject, base.gameObject);
			}
		}

		public void Start()
		{
			CheckAssertionFor(CheckMethod.Start);
			if (IsCheckMethodSelected(CheckMethod.AfterPeriodOfTime))
			{
				StartCoroutine("CheckPeriodically");
			}
			if (IsCheckMethodSelected(CheckMethod.Update))
			{
				m_CheckOnFrame = Time.frameCount + checkAfterFrames;
			}
		}

		public IEnumerator CheckPeriodically()
		{
			yield return new WaitForSeconds(checkAfterTime);
			CheckAssertionFor(CheckMethod.AfterPeriodOfTime);
			while (repeatCheckTime)
			{
				yield return new WaitForSeconds(repeatEveryTime);
				CheckAssertionFor(CheckMethod.AfterPeriodOfTime);
			}
		}

		public bool ShouldCheckOnFrame()
		{
			if (Time.frameCount > m_CheckOnFrame)
			{
				if (repeatCheckFrame)
				{
					m_CheckOnFrame += repeatEveryFrame;
				}
				else
				{
					m_CheckOnFrame = int.MaxValue;
				}
				return true;
			}
			return false;
		}

		public void OnDisable()
		{
			CheckAssertionFor(CheckMethod.OnDisable);
		}

		public void OnEnable()
		{
			CheckAssertionFor(CheckMethod.OnEnable);
		}

		public void OnDestroy()
		{
			CheckAssertionFor(CheckMethod.OnDestroy);
		}

		public void Update()
		{
			if (IsCheckMethodSelected(CheckMethod.Update) && ShouldCheckOnFrame())
			{
				CheckAssertionFor(CheckMethod.Update);
			}
		}

		public void FixedUpdate()
		{
			CheckAssertionFor(CheckMethod.FixedUpdate);
		}

		public void LateUpdate()
		{
			CheckAssertionFor(CheckMethod.LateUpdate);
		}

		public void OnControllerColliderHit()
		{
			CheckAssertionFor(CheckMethod.OnControllerColliderHit);
		}

		public void OnParticleCollision()
		{
			CheckAssertionFor(CheckMethod.OnParticleCollision);
		}

		public void OnJointBreak()
		{
			CheckAssertionFor(CheckMethod.OnJointBreak);
		}

		public void OnBecameInvisible()
		{
			CheckAssertionFor(CheckMethod.OnBecameInvisible);
		}

		public void OnBecameVisible()
		{
			CheckAssertionFor(CheckMethod.OnBecameVisible);
		}

		public void OnTriggerEnter()
		{
			CheckAssertionFor(CheckMethod.OnTriggerEnter);
		}

		public void OnTriggerExit()
		{
			CheckAssertionFor(CheckMethod.OnTriggerExit);
		}

		public void OnTriggerStay()
		{
			CheckAssertionFor(CheckMethod.OnTriggerStay);
		}

		public void OnCollisionEnter()
		{
			CheckAssertionFor(CheckMethod.OnCollisionEnter);
		}

		public void OnCollisionExit()
		{
			CheckAssertionFor(CheckMethod.OnCollisionExit);
		}

		public void OnCollisionStay()
		{
			CheckAssertionFor(CheckMethod.OnCollisionStay);
		}

		public void OnTriggerEnter2D()
		{
			CheckAssertionFor(CheckMethod.OnTriggerEnter2D);
		}

		public void OnTriggerExit2D()
		{
			CheckAssertionFor(CheckMethod.OnTriggerExit2D);
		}

		public void OnTriggerStay2D()
		{
			CheckAssertionFor(CheckMethod.OnTriggerStay2D);
		}

		public void OnCollisionEnter2D()
		{
			CheckAssertionFor(CheckMethod.OnCollisionEnter2D);
		}

		public void OnCollisionExit2D()
		{
			CheckAssertionFor(CheckMethod.OnCollisionExit2D);
		}

		public void OnCollisionStay2D()
		{
			CheckAssertionFor(CheckMethod.OnCollisionStay2D);
		}

		private void CheckAssertionFor(CheckMethod checkMethod)
		{
			if (IsCheckMethodSelected(checkMethod))
			{
				Assertions.CheckAssertions(this);
			}
		}

		public bool IsCheckMethodSelected(CheckMethod method)
		{
			return method == (checkMethods & method);
		}

		public static T Create<T>(CheckMethod checkOnMethods, GameObject gameObject, string propertyPath) where T : ActionBase
		{
			IAssertionComponentConfigurator configurator;
			return Create<T>(out configurator, checkOnMethods, gameObject, propertyPath);
		}

		public static T Create<T>(out IAssertionComponentConfigurator configurator, CheckMethod checkOnMethods, GameObject gameObject, string propertyPath) where T : ActionBase
		{
			return CreateAssertionComponent<T>(out configurator, checkOnMethods, gameObject, propertyPath);
		}

		public static T Create<T>(CheckMethod checkOnMethods, GameObject gameObject, string propertyPath, GameObject gameObject2, string propertyPath2) where T : ComparerBase
		{
			IAssertionComponentConfigurator configurator;
			return Create<T>(out configurator, checkOnMethods, gameObject, propertyPath, gameObject2, propertyPath2);
		}

		public static T Create<T>(out IAssertionComponentConfigurator configurator, CheckMethod checkOnMethods, GameObject gameObject, string propertyPath, GameObject gameObject2, string propertyPath2) where T : ComparerBase
		{
			T val = CreateAssertionComponent<T>(out configurator, checkOnMethods, gameObject, propertyPath);
			val.compareToType = ComparerBase.CompareToType.CompareToObject;
			val.other = gameObject2;
			val.otherPropertyPath = propertyPath2;
			return val;
		}

		public static T Create<T>(CheckMethod checkOnMethods, GameObject gameObject, string propertyPath, object constValue) where T : ComparerBase
		{
			IAssertionComponentConfigurator configurator;
			return Create<T>(out configurator, checkOnMethods, gameObject, propertyPath, constValue);
		}

		public static T Create<T>(out IAssertionComponentConfigurator configurator, CheckMethod checkOnMethods, GameObject gameObject, string propertyPath, object constValue) where T : ComparerBase
		{
			T val = CreateAssertionComponent<T>(out configurator, checkOnMethods, gameObject, propertyPath);
			if (constValue == null)
			{
				val.compareToType = ComparerBase.CompareToType.CompareToNull;
				return val;
			}
			val.compareToType = ComparerBase.CompareToType.CompareToConstantValue;
			val.ConstValue = constValue;
			return val;
		}

		private static T CreateAssertionComponent<T>(out IAssertionComponentConfigurator configurator, CheckMethod checkOnMethods, GameObject gameObject, string propertyPath) where T : ActionBase
		{
			AssertionComponent assertionComponent = gameObject.AddComponent<AssertionComponent>();
			assertionComponent.checkMethods = checkOnMethods;
			T result = (T)(assertionComponent.Action = ScriptableObject.CreateInstance<T>());
			assertionComponent.Action.go = gameObject;
			assertionComponent.Action.thisPropertyPath = propertyPath;
			configurator = assertionComponent;
			StackTrace stackTrace = new StackTrace(true);
			string fileName = stackTrace.GetFrame(0).GetFileName();
			for (int i = 1; i < stackTrace.FrameCount; i++)
			{
				StackFrame frame = stackTrace.GetFrame(i);
				if (frame.GetFileName() != fileName)
				{
					string text = assertionComponent.m_CreatedInFilePath = frame.GetFileName().Substring(Application.dataPath.Length - "Assets".Length);
					assertionComponent.m_CreatedInFileLine = frame.GetFileLineNumber();
					break;
				}
			}
			return result;
		}
	}
}
