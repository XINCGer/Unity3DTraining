//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Generated geometry class. All widgets have one.
/// This class separates the geometry creation into several steps, making it possible to perform
/// actions selectively depending on what has changed. For example, the widget doesn't need to be
/// rebuilt unless something actually changes, so its geometry can be cached. Likewise, the widget's
/// transformed coordinates only change if the widget's transform moves relative to the panel,
/// so that can be cached as well. In the end, using this class means using more memory, but at
/// the same time it allows for significant performance gains, especially when using widgets that
/// spit out a lot of vertices, such as UILabels.
/// </summary>

public class UIGeometry
{
	/// <summary>
	/// Widget's vertices (before they get transformed).
	/// </summary>

	public List<Vector3> verts = new List<Vector3>();

	/// <summary>
	/// Widget's texture coordinates for the geometry's vertices.
	/// </summary>

	public List<Vector2> uvs = new List<Vector2>();

	/// <summary>
	/// Array of colors for the geometry's vertices.
	/// </summary>

	public List<Color> cols = new List<Color>();

	/// <summary>
	/// Custom delegate called after WriteToBuffers finishes filling in the geometry.
	/// Use it to apply any and all modifications to vertices that you need.
	/// </summary>

	public OnCustomWrite onCustomWrite;
	public delegate void OnCustomWrite (List<Vector3> v, List<Vector2> u, List<Color> c, List<Vector3> n, List<Vector4> t, List<Vector4> u2);

	// Relative-to-panel vertices, normal, and tangent
	List<Vector3> mRtpVerts = new List<Vector3>();
	Vector3 mRtpNormal;
	Vector4 mRtpTan;

	/// <summary>
	/// Whether the geometry contains usable vertices.
	/// </summary>

	public bool hasVertices { get { return (verts.Count > 0); } }

	/// <summary>
	/// Whether the geometry has usable transformed vertex data.
	/// </summary>

	public bool hasTransformed { get { return (mRtpVerts != null) && (mRtpVerts.Count > 0) && (mRtpVerts.Count == verts.Count); } }

	/// <summary>
	/// Step 1: Prepare to fill the buffers -- make them clean and valid.
	/// </summary>

	public void Clear ()
	{
		verts.Clear();
		uvs.Clear();
		cols.Clear();
		mRtpVerts.Clear();
	}

	/// <summary>
	/// Step 2: Transform the vertices by the provided matrix.
	/// </summary>

	public void ApplyTransform (Matrix4x4 widgetToPanel, bool generateNormals = true)
	{
		if (verts.Count > 0)
		{
			mRtpVerts.Clear();
			for (int i = 0, imax = verts.Count; i < imax; ++i) mRtpVerts.Add(widgetToPanel.MultiplyPoint3x4(verts[i]));

			// Calculate the widget's normal and tangent
			if (generateNormals)
			{
				mRtpNormal = widgetToPanel.MultiplyVector(Vector3.back).normalized;
				Vector3 tangent = widgetToPanel.MultiplyVector(Vector3.right).normalized;
				mRtpTan = new Vector4(tangent.x, tangent.y, tangent.z, -1f);
			}
		}
		else mRtpVerts.Clear();
	}

	/// <summary>
	/// Step 3: Fill the specified buffer using the transformed values.
	/// </summary>

	public void WriteToBuffers (List<Vector3> v, List<Vector2> u, List<Color> c, List<Vector3> n, List<Vector4> t, List<Vector4> u2)
	{
		if (mRtpVerts != null && mRtpVerts.Count > 0)
		{
			if (n == null)
			{
				for (int i = 0, imax = mRtpVerts.Count; i < imax; ++i)
				{
					v.Add(mRtpVerts[i]);
					u.Add(uvs[i]);
					c.Add(cols[i]);
				}
			}
			else
			{
				for (int i = 0, imax = mRtpVerts.Count; i < imax; ++i)
				{
					v.Add(mRtpVerts[i]);
					u.Add(uvs[i]);
					c.Add(cols[i]);
					n.Add(mRtpNormal);
					t.Add(mRtpTan);
				}
			}

			if (u2 != null)
			{
				Vector4 uv2 = Vector4.zero;

				for (int i = 0, imax = verts.Count; i < imax; ++i)
				{
					uv2.x = verts[i].x;
					uv2.y = verts[i].y;
					u2.Add(uv2);
				}
			}

			if (onCustomWrite != null) onCustomWrite(v, u, c, n, t, u2);
		}
	}
}
