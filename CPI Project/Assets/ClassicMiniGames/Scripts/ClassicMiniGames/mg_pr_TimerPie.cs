using System;
using UnityEngine;

public class mg_pr_TimerPie : MonoBehaviour
{
	private float m_numberOfSections = 100f;

	private float m_currentAngleRad;

	private float m_currentAngle;

	private float m_sectionSize;

	private int m_currentSection;

	private Vector3[] m_outerPoints;

	private Vector3 m_leadingEdge;

	public float m_gameTime;

	public void StartTimer()
	{
		m_sectionSize = 360f / m_numberOfSections;
		m_outerPoints = new Vector3[(int)m_numberOfSections];
		for (int i = 0; (float)i < m_numberOfSections; i++)
		{
			m_outerPoints[i].x = Mathf.Cos((float)Math.PI * 2f * ((float)i / m_numberOfSections));
			m_outerPoints[i].z = Mathf.Sin((float)Math.PI * 2f * ((float)i / m_numberOfSections));
		}
		InvokeRepeating("BuildMesh", 1f, 1f);
	}

	private void BuildMesh()
	{
		m_currentAngle += 360f / m_gameTime;
		m_currentAngle = Mathf.Clamp(m_currentAngle, 0f, 360f);
		m_currentSection = (int)Mathf.Clamp(m_currentAngle / m_sectionSize, 0f, m_numberOfSections - 1f);
		m_currentAngleRad = m_currentAngle * ((float)Math.PI / 180f);
		m_leadingEdge.x = Mathf.Cos(m_currentAngleRad);
		m_leadingEdge.z = Mathf.Sin(m_currentAngleRad);
		MeshFilter component = GetComponent<MeshFilter>();
		Mesh mesh2 = component.mesh = new Mesh();
		Vector3[] array = new Vector3[(int)m_numberOfSections * 3];
		int[] array2 = new int[(int)m_numberOfSections * 3];
		array[0] = new Vector3(0f, 0f, 0f);
		for (int i = 1; (float)i <= m_numberOfSections; i++)
		{
			array[i] = m_outerPoints[i - 1];
		}
		if ((float)m_currentSection < m_numberOfSections)
		{
			array[m_currentSection + 1] = m_outerPoints[m_currentSection];
			array[m_currentSection + 2] = m_leadingEdge;
		}
		mesh2.vertices = array;
		int num = 0;
		int num2 = 0;
		for (int j = 0; j <= m_currentSection; j++)
		{
			array2[num * 3] = 0;
			array2[num * 3 + 1] = num2;
			num2 = (array2[num * 3 + 2] = num2 + 1);
			num++;
		}
		array2[0] = 0;
		array2[1] = m_currentSection + 1;
		array2[2] = m_currentSection + 2;
		mesh2.triangles = array2;
	}

	public void StopTimer()
	{
		CancelInvoke();
	}

	public void DestroyMesh()
	{
		StopTimer();
		UnityEngine.Object.Destroy(GetComponent<MeshFilter>().mesh);
		m_currentAngleRad = 0f;
		m_currentAngle = 0f;
		m_currentSection = 1;
	}
}
