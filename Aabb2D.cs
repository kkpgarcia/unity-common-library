using UnityEngine;
using System.Collections.Generic;


public class Aabb2D
{

	private Vector2 min;
	private Vector2 max;
	private const float BIG_NUMBER = 1e37f;

	public Aabb2D ()
	{
		this.min = new Vector2 ();
		this.max = new Vector2 ();
		Empty ();
	}

	public Aabb2D (Vector2 v1, Vector2 v2)
	{
		this.min = new Vector2 ();
		this.max = new Vector2 ();
		Empty ();

		AddToContain (v1);
		AddToContain (v2);
	}

	public Aabb2D (Rect rect)
	{
		this.min = new Vector2 ();
		this.max = new Vector2 ();

		Empty ();

		AddToContain (new Vector2 (rect.xMin, rect.yMin));
		AddToContain (new Vector2 (rect.xMax, rect.yMax));
	}

	public void Empty ()
	{
		this.min.x = BIG_NUMBER;
		this.min.y = BIG_NUMBER;

		this.max.x = -BIG_NUMBER;
		this.max.y = -BIG_NUMBER;
	}

	public bool IsEmpty ()
	{
		return (this.min.x > this.max.x) && (this.min.y > this.max.y);
	}

	public Vector2 GetMinimum ()
	{
		return this.min;
	}

	public Vector2 GetMaximum ()
	{
		return this.max;
	}

	public Vector2 GetCenter ()
	{
		return (this.min + this.max) * 0.5f;
	}

	public Vector2 GetSize ()
	{
		return this.max - this.min;
	}

	public Vector2 GetRadiusVector ()
	{
		return GetSize () * 0.5f;
	}

	public float GetRadius ()
	{
		return GetRadiusVector ().magnitude;
	}

	public void AddToContain (Vector2 v)
	{
		// expand min
		if (v.x < min.x) {
			min.x = v.x;
		}

		if (v.y < min.y) {
			min.y = v.y;
		}

		// expand max
		if (v.x > max.x) {
			max.x = v.x;
		}

		if (v.y > max.y) {
			max.y = v.y;
		}
	}

	public bool Contains (Vector2 v)
	{
		return (Comparison.TolerantLesserThanOrEquals (min.x, v.x) && Comparison.TolerantLesserThanOrEquals (v.x, max.x))
			&& (Comparison.TolerantLesserThanOrEquals (min.y, v.y) && Comparison.TolerantLesserThanOrEquals (v.y, max.y));
	}

	public bool IsOverlapping (Aabb2D otherBox)
	{
		// get all corners
		// return true if there is at least one corner that is contained within the bounding box
		return Contains (otherBox.GetTopLeft ())
			|| Contains (otherBox.GetBottomLeft ())
			|| Contains (otherBox.GetTopRight ())
			|| Contains (otherBox.GetBottomRight ());
	}

	public void Translate (Vector2 translation)
	{
		if (IsEmpty ()) {
			// no need to translate if it is empty
			return;
		}

		// transform to local space
		Vector2 center = GetCenter ();
		this.min -= center;
		this.max -= center;

		// translate
		this.min += translation;
		this.max += translation;
	}

	public Aabb2D GetAabbInLocalSpace ()
	{
		Aabb2D localAabb = new Aabb2D ();

		Vector2 center = GetCenter ();
		localAabb.AddToContain (this.min - center);
		localAabb.AddToContain (this.max - center);

		return localAabb;
	}

	public Vector2 GetTopLeft ()
	{
		return new Vector2 (min.x, max.y);
	}

	public Vector2 GetBottomLeft ()
	{
		return this.min;
	}

	public Vector2 GetTopRight ()
	{
		return this.max;
	}

	public Vector2 GetBottomRight ()
	{
		return new Vector2 (max.x, min.y);
	}

	public override string ToString ()
	{
		return "min: " + min + "; max: " + max;
	}
}

