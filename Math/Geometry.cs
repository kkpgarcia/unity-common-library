using UnityEngine;

namespace Common.ObjectPointer.Math
{
    public static class Geometry
	{
		public static float LinePlaneDistance(Vector3 linePoint, Vector3 lineVec, Vector3 planePoint, Vector3 planeNormal)
		{
			//calculate the distance between the linePoint and the line-plane intersection point
			float dotNumerator = Vector3.Dot((planePoint - linePoint), planeNormal);
			float dotDenominator = Vector3.Dot(lineVec, planeNormal);

			//line and plane are not parallel
			if(dotDenominator != 0f)
			{
				return dotNumerator / dotDenominator;
			}

			return 0;
		}

		public static Vector3 LinePlaneIntersect(Vector3 linePoint, Vector3 lineVec, Vector3 planePoint, Vector3 planeNormal)
		{
			float distance = LinePlaneDistance(linePoint, lineVec, planePoint, planeNormal);

			if(distance != 0f)
			{
				return linePoint + (lineVec * distance);
			}

			return Vector3.zero;
		}

		public static IntersectPoints ClosestPointsOnTwoLines(Vector3 point1, Vector3 point1Direction, Vector3 point2, Vector3 point2Direction)
		{
			IntersectPoints intersections = new IntersectPoints();

			float a = Vector3.Dot(point1Direction, point1Direction);
			float b = Vector3.Dot(point1Direction, point2Direction);
			float e = Vector3.Dot(point2Direction, point2Direction);

			float d = a*e - b*b;

			if(d != 0f)
			{
				Vector3 r = point1 - point2;
				float c = Vector3.Dot(point1Direction, r);
				float f = Vector3.Dot(point2Direction, r);

				float s = (b*f - c*e) / d;
				float t = (a*f - c*b) / d;

				intersections.first = point1 + point1Direction * s;
				intersections.second = point2 + point2Direction * t;
			}else{
				//Lines are parallel, select any points next to eachother
				intersections.first = point1;
				intersections.second = point2 + Vector3.Project(point1 - point2, point2Direction);
			}

			return intersections;
		}

		public static IntersectPoints ClosestPointsOnSegmentToLine(Vector3 segment0, Vector3 segment1, Vector3 linePoint, Vector3 lineDirection)
		{
			IntersectPoints closests = ClosestPointsOnTwoLines(segment0, segment1 - segment0, linePoint, lineDirection);
			closests.first = ClampToSegment(closests.first, segment0, segment1);

			return closests;
		}

		//Assumes the point is already on the line somewhere
		public static Vector3 ClampToSegment(Vector3 point, Vector3 linePoint1, Vector3 linePoint2)
		{
			Vector3 lineDirection = linePoint2 - linePoint1;

			if(!Vector3Extentions.IsInDirection(point - linePoint1, lineDirection))
			{
				point = linePoint1;
			}
			else if(Vector3Extentions.IsInDirection(point - linePoint2, lineDirection))
			{
				point = linePoint2;
			}

			return point;
		}
	}
}