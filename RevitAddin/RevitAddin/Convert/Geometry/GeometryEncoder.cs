using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Rhino.Geometry;
using DB = Autodesk.Revit.DB;

namespace RhinoInside.Revit.Convert.Geometry
{
  /// <summary>
  /// Methods in this class do a full geometry conversion.
  /// <para>It converts geometry from Active Rhino model units to Revit internal units.</para>
  /// <para>For direct conversion methods see <see cref="Raw.RawEncoder"/> class.</para>
  /// </summary>
  public static class GeometryEncoder
  {

    #region Geometry values
    public static DB::UV ToUV(this Point2f value)
    {
      double factor = UnitConverter.ToHostUnits;
      return new DB::UV(value.X * factor, value.Y * factor);
    }
    public static DB::UV ToUV(this Point2f value, double factor)
    {
      return factor == 1.0 ?
        new DB::UV(value.X, value.Y) :
        new DB::UV(value.X * factor, value.Y * factor);
    }

    public static DB::UV ToUV(this Point2d value)
    {
      double factor = UnitConverter.ToHostUnits;
      return new DB::UV(value.X * factor, value.Y * factor);
    }
    public static DB::UV ToUV(this Point2d value, double factor)
    {
      return factor == 1.0 ?
        new DB::UV(value.X, value.Y) :
        new DB::UV(value.X * factor, value.Y * factor);
    }

    public static DB::UV ToUV(this Vector2f value)
    {
      return new DB::UV(value.X, value.Y);
    }
    public static DB::UV ToUV(this Vector2f value, double factor)
    {
      return factor == 1.0 ?
        new DB::UV(value.X, value.Y) :
        new DB::UV(value.X * factor, value.Y * factor);
    }

    public static DB::UV ToUV(this Vector2d value)
    {
      return new DB::UV(value.X, value.Y);
    }
    public static DB::UV ToUV(this Vector2d value, double factor)
    {
      return factor == 1.0 ?
        new DB::UV(value.X, value.Y) :
        new DB::UV(value.X * factor, value.Y * factor);
    }

    public static DB::XYZ ToXYZ(this Point3f value)
    {
      double factor = UnitConverter.ToHostUnits;
      return new DB::XYZ(value.X * factor, value.Y * factor, value.Z * factor);
    }
    public static DB::XYZ ToXYZ(this Point3f value, double factor)
    {
      return factor == 1.0 ?
        new DB::XYZ(value.X, value.Y, value.Z) :
        new DB::XYZ(value.X * factor, value.Y * factor, value.Z * factor);
    }

    public static DB::XYZ ToXYZ(this Point3d value)
    {
      double factor = UnitConverter.ToHostUnits;
      return new DB::XYZ(value.X * factor, value.Y * factor, value.Z * factor);
    }
    public static DB::XYZ ToXYZ(this Point3d value, double factor)
    {
      return factor == 1.0 ?
        new DB::XYZ(value.X, value.Y, value.Z) :
        new DB::XYZ(value.X * factor, value.Y * factor, value.Z * factor);
    }

    public static DB::XYZ ToXYZ(this Vector3f value)
    {
      return new DB::XYZ(value.X, value.Y, value.Z);
    }
    public static DB::XYZ ToXYZ(this Vector3f value, double factor)
    {
      return factor == 1.0 ?
        new DB::XYZ(value.X, value.Y, value.Z) :
        new DB::XYZ(value.X * factor, value.Y * factor, value.Z * factor);
    }

    public static DB::XYZ ToXYZ(this Vector3d value)
    {
      return new DB::XYZ(value.X, value.Y, value.Z);
    }
    public static DB::XYZ ToXYZ(this Vector3d value, double factor)
    {
      return factor == 1.0 ?
        new DB::XYZ(value.X, value.Y, value.Z) :
        new DB::XYZ(value.X * factor, value.Y * factor, value.Z * factor);
    }

    public static DB.Plane ToPlane(this Plane value) => ToPlane(value, UnitConverter.ToHostUnits);
    public static DB.Plane ToPlane(this Plane value, double factor)
    {
      return DB.Plane.CreateByOriginAndBasis(value.Origin.ToXYZ(factor), value.XAxis.ToXYZ(), value.YAxis.ToXYZ());
    }

    public static DB.Transform ToTransform(this Transform value) => ToTransform(value, UnitConverter.ToHostUnits);
    public static DB.Transform ToTransform(this Transform value, double factor)
    {
      Debug.Assert(value.IsAffine);

            var result = factor == 1.0 ?
              DB.Transform.CreateTranslation(new DB.XYZ(value.M03, value.M13, value.M23)) :
              DB.Transform.CreateTranslation(new DB.XYZ(value.M03 * factor, value.M13 * factor, value.M23 * factor));

      result.BasisX = new DB.XYZ(value.M00, value.M10, value.M20);
      result.BasisY = new DB.XYZ(value.M01, value.M11, value.M21);
      result.BasisZ = new DB.XYZ(value.M02, value.M12, value.M22);
            Debug.WriteLine(result.Scale);
      return result;
    }

    public static DB.BoundingBoxXYZ ToBoundingBoxXYZ(this BoundingBox value) => ToBoundingBoxXYZ(value, UnitConverter.ToHostUnits);
    public static DB.BoundingBoxXYZ ToBoundingBoxXYZ(this BoundingBox value, double factor)
    {
      return new DB.BoundingBoxXYZ
      {
        Min = value.Min.ToXYZ(factor),
        Max = value.Min.ToXYZ(factor),
        Enabled = value.IsValid
      };
    }

    public static DB.BoundingBoxXYZ ToBoundingBoxXYZ(this Box value) => ToBoundingBoxXYZ(value, UnitConverter.ToHostUnits);
    public static DB.BoundingBoxXYZ ToBoundingBoxXYZ(this Box value, double factor)
    {
      return new DB.BoundingBoxXYZ
      {
        Transform = Transform.PlaneToPlane(Plane.WorldXY, value.Plane).ToTransform(factor),
        Min = new DB.XYZ(value.X.Min * factor, value.Y.Min * factor, value.Z.Min * factor),
        Max = new DB.XYZ(value.X.Max * factor, value.Y.Max * factor, value.Z.Max * factor),
        Enabled = value.IsValid
      };
    }

    public static DB.Outline ToOutline(this BoundingBox value) => ToOutline(value, UnitConverter.ToHostUnits);
    public static DB.Outline ToOutline(this BoundingBox value, double factor)
    {
      return new DB.Outline(value.Min.ToXYZ(factor), value.Max.ToXYZ(factor));
    }
    #endregion

    #region Curve values
    public static DB.Line ToLine(this Line value) => value.ToLine(UnitConverter.ToHostUnits);
    public static DB.Line ToLine(this Line value, double factor)
    {
      return DB.Line.CreateBound(value.From.ToXYZ(factor), value.To.ToXYZ(factor));
    }

    public static DB.Line[] ToLines(this Polyline value) => value.ToLines(UnitConverter.ToHostUnits);
    public static DB.Line[] ToLines(this Polyline value, double factor)
    {
      value = value.Duplicate();
            ///TODO
            ///value.DeleteShortSegments(Revit.ShortCurveTolerance / factor);

      int count = value.Count;
      var list = new DB.Line[Math.Max(0, count - 1)];
      if (count > 1)
      {
        var point = value[0];
        DB.XYZ end, start = new DB.XYZ(point.X * factor, point.Y * factor, point.Z * factor);
        for (int p = 1; p < count; start = end, ++p)
        {
          point = value[p];
          end = new DB.XYZ(point.X * factor, point.Y * factor, point.Z * factor);
          list[p-1] = DB.Line.CreateBound(start, end);
        }
      }

      return list;
    }

    public static DB.PolyLine ToPolyLine(this Polyline value) => value.ToPolyLine(UnitConverter.ToHostUnits);
    public static DB.PolyLine ToPolyLine(this Polyline value, double factor)
    {
      int count = value.Count;
      var points = new DB.XYZ[count];

      if (factor == 1.0)
      {
        for (int p = 0; p < count; ++p)
        {
          var point = value[p];
          points[p] = new DB.XYZ(point.X, point.Y, point.Z);
        }
      }
      else
      {
        for (int p = 0; p < count; ++p)
        {
          var point = value[p];
          points[p] = new DB.XYZ(point.X * factor, point.Y * factor, point.Z * factor);
        }
      }

      return DB.PolyLine.Create(points);
    }

    public static DB.Arc ToArc(this Arc value) => value.ToArc(UnitConverter.ToHostUnits);
    public static DB.Arc ToArc(this Arc value, double factor)
    {
      if (value.IsCircle)
        return DB.Arc.Create(value.Plane.ToPlane(factor), value.Radius * factor, 0.0, 2.0 * Math.PI);
      else
        return DB.Arc.Create(value.StartPoint.ToXYZ(factor), value.EndPoint.ToXYZ(factor), value.MidPoint.ToXYZ(factor));
    }

    public static DB.Arc ToArc(this Circle value) => value.ToArc(UnitConverter.ToHostUnits);
    public static DB.Arc ToArc(this Circle value, double factor)
    {
      return DB.Arc.Create(value.Plane.ToPlane(factor), value.Radius * factor, 0.0, 2.0 * Math.PI);
    }

    public static DB.Curve ToCurve(this Ellipse value) => value.ToCurve(new Interval(0.0, 2.0 * Math.PI), UnitConverter.ToHostUnits);
    public static DB.Curve ToCurve(this Ellipse value, double factor) => value.ToCurve(new Interval(0.0, 2.0 * Math.PI), factor);
    public static DB.Curve ToCurve(this Ellipse value, Interval interval) => value.ToCurve(interval, UnitConverter.ToHostUnits);
    public static DB.Curve ToCurve(this Ellipse value, Interval interval, double factor)
    {
            return null;

    }
    #endregion

    #region GeometryBase
    public static DB.Point ToPoint(this Point value) => value.ToPoint(UnitConverter.ToHostUnits);
    public static DB.Point ToPoint(this Point value, double factor)
    {
      return DB.Point.Create(value.Location.ToXYZ(factor));
    }

    public static DB.Point[] ToPoints(this PointCloud value) => value.ToPoints(UnitConverter.ToHostUnits);
    public static DB.Point[] ToPoints(this PointCloud value, double factor)
    {
      var array = new DB.Point[value.Count];
      int index = 0;
      if (factor == 1.0)
      {
        foreach (var point in value)
        {
          var location = point.Location;
          array[index++] = DB.Point.Create(new DB::XYZ(location.X, location.Y, location.Z));
        }
      }
      else
      {
        foreach (var point in value)
        {
          var location = point.Location;
          array[index++] = DB.Point.Create(new DB::XYZ(location.X * factor, location.Y * factor, location.Z * factor));
        }
      }

      return array;
    }

    public static DB.Curve ToCurve(this LineCurve value) => value.Line.ToLine(UnitConverter.ToHostUnits);
    public static DB.Curve ToCurve(this LineCurve value, double factor) => value.Line.ToLine(factor);

    public static DB.Curve ToCurve(this PolylineCurve value) => ToCurve(value, UnitConverter.ToHostUnits);
    public static DB.Curve ToCurve(this PolylineCurve value, double factor)
    {
            return null;
    }

    public static DB.Curve ToCurve(this ArcCurve value) => value.Arc.ToArc(UnitConverter.ToHostUnits);
    public static DB.Curve ToCurve(this ArcCurve value, double factor) => value.Arc.ToArc(factor);

    public static DB.Curve ToCurve(this NurbsCurve value) => value.ToCurve(UnitConverter.ToHostUnits);
    public static DB.Curve ToCurve(this NurbsCurve value, double factor)
    {
            return null;
    }

    public static DB.Curve ToCurve(this PolyCurve value) => ToCurve(value, UnitConverter.ToHostUnits);
    public static DB.Curve ToCurve(this PolyCurve value, double factor)
    {
            return null;
    }

    public static DB.Curve ToCurve(this Curve value) => value.ToCurve(UnitConverter.ToHostUnits);
    public static DB.Curve ToCurve(this Curve value, double factor)
    {
      switch (value)
      {
        case LineCurve line:
          return line.Line.ToLine(factor);

        case ArcCurve arc:
          return arc.Arc.ToArc(factor);

        case PolylineCurve polyline:
          return polyline.ToCurve(factor);

        case PolyCurve polyCurve:
          return polyCurve.ToCurve(factor);

        case NurbsCurve nurbsCurve:
          return nurbsCurve.ToCurve(factor);

        default:
          return value.ToNurbsCurve().ToCurve(factor);
      }
    }

    public static DB.CurveLoop ToCurveLoop(this Curve value)
    {
            return null;
    }

    public static DB.CurveArray ToCurveArray(this Curve value)
    {
            return null;
        }

    public static DB.CurveArrArray ToCurveArrayArray(this IList<Curve> value)
    {
      var curveArrayArray = new DB.CurveArrArray();
      foreach (var curve in value)
        curveArrayArray.Append(curve.ToCurveArray());

      return curveArrayArray;
    }

    #region ToSolid

    #endregion

    #region ToMesh

    #endregion

    public static DB.GeometryObject ToGeometryObject(this GeometryBase geometry) => ToGeometryObject(geometry, UnitConverter.ToHostUnits);
    public static DB.GeometryObject ToGeometryObject(this GeometryBase geometry, double scaleFactor)
    {
            return null;
    }
    #endregion

    public static IEnumerable<DB.Point> ToPointMany(this PointCloud value) => value.ToPointMany(UnitConverter.ToHostUnits);
    public static IEnumerable<DB.Point> ToPointMany(this PointCloud value, double factor)
    {
      if (factor == 1.0)
      {
        foreach (var point in value)
        {
          var location = point.Location;
          yield return DB.Point.Create(new DB::XYZ(location.X, location.Y, location.Z));
        }
      }
      else
      {
        foreach (var point in value)
        {
          var location = point.Location;
          yield return DB.Point.Create(new DB::XYZ(location.X * factor, location.Y * factor, location.Z * factor));
        }
      }
    }

    public static IEnumerable<DB.Curve> ToCurveMany(this NurbsCurve value) => value.ToCurveMany(UnitConverter.ToHostUnits);

    public static IEnumerable<DB.Curve> ToCurveMany(this PolylineCurve value) => value.ToCurveMany(UnitConverter.ToHostUnits);
    public static IEnumerable<DB.Curve> ToCurveMany(this PolylineCurve value, double factor)
    {
            return null;
    }

    public static IEnumerable<DB.Curve> ToCurveMany(this PolyCurve value) => value.ToCurveMany(UnitConverter.ToHostUnits);
    public static IEnumerable<DB.Curve> ToCurveMany(this PolyCurve value, double factor)
    {
            return null;
    }

    public static IEnumerable<DB.Curve> ToCurveMany(this Curve value) => value.ToCurveMany(UnitConverter.ToHostUnits);
    public static IEnumerable<DB.Curve> ToCurveMany(this Curve curve, double factor)
    {
            return null;
    }
  }
}
